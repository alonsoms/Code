Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.ServiceModel
Imports COE.DATA
Imports COE.FRAMEWORK

Public Class EnvioSunatSE
    Dim ServicioDAO As New ServicioDAO
    Dim Tiempo As New System.Timers.Timer
    Dim ResumenDAO As New ResumenDAO
    Dim ComunicacionBajaDAO As New ComunicacionBajaDAO

#Region "Eventos del Servicio"
    Protected Overrides Sub OnStart(ByVal args() As String)

        'Se carga la cultura de Peru en la aplicacion
        Dim MiCultura As New CultureInfo("es-PE", False)

        'Se establece la cultura de peru
        System.Threading.Thread.CurrentThread.CurrentCulture = MiCultura
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

        'Se estable el evento
        AddHandler Tiempo.Elapsed, AddressOf EnvioSUNAT

        'Se activa el timer. Se convierte minutos a milisegundos 1 Minutos=60,000 milisegundos
        Tiempo.Interval = 30000 ' 60000 * 5
        Tiempo.Enabled = True
        Tiempo.Start()

    End Sub
    Protected Overrides Sub OnStop()
        Tiempo.Enabled = False
    End Sub
    Protected Overrides Sub OnContinue()
        Tiempo.Enabled = True
    End Sub
#End Region

    Public Sub EnvioSUNAT()
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim EmisorDAO As New EmisorDAO
        Dim EmisorBE As New EmisorBE
        Dim FechaEmisionRB As String = String.Empty
        Dim FechaEmisionCB As String = String.Empty

        Try
            'Se detiene el servicio
            Tiempo.Stop()

            'Se obtiene los datos del emisor
            EmisorBE = EmisorDAO.GetByID(1)

            'Se obtiene la hora de ejecucion para procesar las tareas
            Dim HoraServicio As DateTime = EmisorBE.FechaEnvioResumenComunicacion
            Dim HoraActual As DateTime = DateTime.Now

            'Si la hora de servicio supera la hora actual se ejecuta las tareas
            If HoraActual > HoraServicio Then
                Dim HoraInicio As DateTime = DateTime.Now

                'Se obtiene la fecha de emision a generar. RB=Resumen de Boletas. CB=Comunicacion de Baja
                FechaEmisionRB = ResumenDAO.GetFechaEmision
                FechaEmisionCB = ComunicacionBajaDAO.GetFechaEmision

                'Se valida la fecha de emision
                If FechaEmisionRB = "" Then
                    FechaEmisionRB = Convert.ToDateTime(EmisorBE.FechaEnvioResumenComunicacion).Date.ToString
                    FechaEmisionCB = Convert.ToDateTime(EmisorBE.FechaEnvioResumenComunicacion).Date.ToString
                End If

                'Se crea el resumen de boletas pendientes para crear xml, firmar y empaquetar
                'Se envia los resumenes pendientes a SUNAT
                'Se envia los tickets pendientes de CDR. con una diferencia de 12 horas
                CrearResumenXML(FechaEmisionRB)
                EnviarResumenPendientes()
                EnviarResumenTickets()

                'Se crea la comunicacion de baja pendientes para crear xml, firmar y empaquetar
                'Se envia las comunicaciones Pendientes
                'Se envia los tickets pendientes de CDE con una diferencia de 12 horas
                CrearComunicacionXML(FechaEmisionCB)
                EnviarComunicacionPendientes()
                EnviarComunicacionTickets()

                'Se guarda la nueva fecha y hora para ejecutar las tareas
                EmisorBE.FechaEnvioResumenComunicacion = HoraServicio.AddDays(1)
                EmisorDAO.Save(EmisorBE)
            End If

        Catch ex As Exception
            Tools.SaveLog("COE SERVICE SUNAT RC", ex.Message, EventLogEntryType.Error)
        Finally
            Tiempo.Start()
        End Try

    End Sub

    Public Sub CrearResumenXML(FechaEmision As Date)
        Dim ResumenDAO As New ResumenDAO

        'Se crea el resumen de boletas segun la fecha de emision
        ResumenDAO.SaveResumen(FechaEmision, 1, My.Computer.Name)

        'Se carga el resumen de boletas para crear XML,Firmar y Empaquetar
        Dim dt As New DataTable
        dt = ResumenDAO.GetResumenBoletasPendientes(FechaEmision)

        'Se crea el archivo xml y se guarda
        For Each dr As DataRow In dt.Rows
            ResumenDAO.CreateXML(dr("idresumen"))
            ResumenDAO.SignatureXML(dr("idresumen"))
            ResumenDAO.ZipXML(dr("idresumen"))
        Next
    End Sub
    Public Sub EnviarResumenPendientes()
        Dim dt As New DataTable
        Dim ResumenDAO As New ResumenDAO
        Dim ResumenBE As New ResumenBE2018
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient

        Try


            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se abre el servicio de la SUNAT
            SunatSE.Open()

            'Se obtiene resumen pendientes de envio
            dt = ResumenDAO.GetByAll2(eGetResumen.ResumenPendientesEnvio)

            For Each item As DataRow In dt.Rows

                'Se obtiene la entidad
                ResumenBE = ResumenDAO.GetByID(item("idresumen"))
                ResumenDAO.IDResumen = (item("idresumen"))


                'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
                Dim NumetoTicket As String
                NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ResumenBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ResumenBE.RutaComprobanteZIP))

                'Se guarda el numero de ticket que envia la SUNAT
                ResumenDAO.SaveTicket(NumetoTicket)
            Next
        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = ResumenBE.idresumen
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = ResumenBE.idresumen
                .Descripcion = ex2.Message.ToString
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()
            End If
        End Try
    End Sub
    Public Sub EnviarResumenTickets()
        Dim dt As New DataTable
        Dim ResumenDAO As New ResumenDAO
        Dim ResumenBE As New ResumenBE2018
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient

        Try

            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se abre el servicio de la SUNAT
            SunatSE.Open()

            'Se obtiene los tickets pendientes del CDR
            dt = ResumenDAO.GetByAll2(eGetResumen.TicketsPendientesCDR)

            For Each item As DataRow In dt.Rows

                'Se obtiene la entidad
                ResumenBE = ResumenDAO.GetByID(item("idresumen"))


                'Se obtiene la respuesta del envio del ticket
                Dim RespuestaSUNAT As SunatSE.statusResponse
                RespuestaSUNAT = SunatSE.getStatus(ResumenBE.Ticket)

                'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
                If RespuestaSUNAT.content.Length > 0 Then
                    ResumenDAO.SaveConstanciaRecepcionZIP(ResumenBE.idresumen, RespuestaSUNAT.content)
                Else
                    Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
                End If
            Next
        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = ResumenBE.idresumen
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)
        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = ResumenBE.idresumen
                .Descripcion = ex2.Message.ToString
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)
        Finally
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()
            End If
        End Try
    End Sub

    Public Sub CrearComunicacionXML(FechaEmision As Date)
        Dim IDComunicacion As Int32 = 0
        IDComunicacion = ComunicacionBajaDAO.SaveComunicacionBajaXML(FechaEmision, 1, My.Computer.Name)
        If IDComunicacion <> 0 Then
            'Se crea el XML, Se firma y Se empaqueta
            ComunicacionBajaDAO.CreateXML(IDComunicacion)
            ComunicacionBajaDAO.SignatureXML(IDComunicacion)
            ComunicacionBajaDAO.ZipXML(IDComunicacion)

        End If

    End Sub
    Public Sub EnviarComunicacionPendientes()
        Dim dt As New DataTable
        Dim ComunicacionBajaBE As New ComunicacionBajaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient

        Try
            dt = ComunicacionBajaDAO.GetByAll2(eGetComunicacion.ComunicacionPendientesEnvio)

            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se abre el servicio de la SUNAT
            SunatSE.Open()
            For Each item As DataRow In dt.Rows

                'Se obtiene la entidad
                ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(item("idcomunicacion"))
                ComunicacionBajaDAO.IDComunicacion = item("idcomunicacion")

                'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
                Dim NumetoTicket As String
                NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ComunicacionBajaBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ComunicacionBajaBE.RutaComprobanteZIP))

                'Se guarda el numero de ticket que envia la SUNAT
                ComunicacionBajaDAO.SaveTicket(NumetoTicket)
            Next

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = ComunicacionBajaBE.idcomunicacion
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = ComunicacionBajaBE.idcomunicacion
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Finally
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()
            End If
        End Try
    End Sub
    Public Sub EnviarComunicacionTickets()
        Dim dt As New DataTable
        Dim ComunicacionBajaBE As New ComunicacionBajaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim RespuestaSUNAT As SunatSE.statusResponse = Nothing


        Try

            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se abre el servicio de la SUNAT
            SunatSE.Open()
            dt = ComunicacionBajaDAO.GetByAll2(eGetComunicacion.TicketsPendientesCDR)

            For Each item As DataRow In dt.Rows

                'Se obtiene la entidad
                ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(item("idcomunicacion"))


                'Se obtiene la respuesta del envio del ticket
                RespuestaSUNAT = SunatSE.getStatus(ComunicacionBajaBE.NumeroTicket)

                'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
                If RespuestaSUNAT.content.Length > 0 Then
                    ComunicacionBajaDAO.SaveConstanciaRecepcionZIP(ComunicacionBajaBE.idcomunicacion, RespuestaSUNAT.content)
                Else
                    Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
                End If
            Next

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = ComunicacionBajaBE.idcomunicacion
                .Descripcion = ex1.Message.ToString
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = ComunicacionBajaBE.idcomunicacion
                .Descripcion = ex2.Message.ToString
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Finally
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()
            End If
        End Try
    End Sub



End Class
