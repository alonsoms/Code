Imports COE.DATA
Imports COE.FRAMEWORK
Imports DevExpress.XtraPrinting.BarCode
Imports DevExpress.XtraReports.UI
Imports System.Net.Mail
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Configuration
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob
Imports System.IO
'Imports COE.SERVICE.SUNAT
Imports System.ServiceModel

Public Class Form1
    Private Sub btnEnviarWb_Click(sender As Object, e As EventArgs) Handles btnEnviarWb.Click

    End Sub

    Private Sub btnCrearXMLFirmar_Click(sender As Object, e As EventArgs) Handles btnCrearXMLFirmar.Click

    End Sub

    Private Sub btnCorreo_Click(sender As Object, e As EventArgs) Handles btnCorreo.Click

    End Sub
    'Dim ComunicacionBajaDAO As New ComunicacionBajaDAO

    'Private Sub btnCrearXMLFirmar_Click(sender As Object, e As EventArgs) Handles btnCrearXMLFirmar.Click
    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO
    '    Dim NotaDebitoDAO As New NotaDebitoDAO
    '    Dim SistemaDAO As New SistemaDAO
    '    Dim ServicioDAO As New ServicioDAO
    '    Dim ComprobanteBE As New Object
    '    Dim EmisorDAO As New EmisorDAO

    '    Dim IDTipoComprobante As Int32 = 0


    '    Dim TipoComprobante As String = String.Empty
    '    Dim IDComprobante As Int32
    '    Dim IDServicioComprobante As Int32

    '    Dim dt As New DataTable

    '    Try
    '        'Se detiene el timer


    '        'Se obtiene los comprobantes para firmarlos 01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
    '        dt = ServicioDAO.GetByIDServicio(eServicio.CrearXMLFirmar)

    '        'Se crea la firma para cada comprobante
    '        For Each dr As DataRow In dt.Rows

    '            Try
    '                IDServicioComprobante = dr("IDServicioComprobante")
    '                TipoComprobante = dr("TipoComprobante")
    '                IDComprobante = dr("IDComprobante")

    '                'Se crea XML y firma compobantes  01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
    '                Select Case TipoComprobante
    '                    Case "01"
    '                        FacturaDAO.CreateXML(IDComprobante)
    '                        FacturaDAO.SignatureXML(IDComprobante)
    '                        FacturaDAO.ZipXML(IDComprobante)

    '                        'Se obtiene la entidad
    '                        ComprobanteBE = FacturaDAO.GetByID(IDComprobante)

    '                        'Se crea la instancia del reporte
    '                        Dim MiReporte As New COE.REPORT.FacturaVoucher

    '                        'Se carga los datos del reporte
    '                        MiReporte.DataSource = FacturaDAO.GetByReporteID(IDComprobante)
    '                        MiReporte.DataMember = "coe_factura_rpt_id"

    '                        'Se exporta en formato PDF
    '                        MiReporte.ExportToPdf(ComprobanteBE.RutaComprobantePDF)
    '                    Case "03"
    '                        BoletaDAO.CreateXML(IDComprobante)
    '                        BoletaDAO.SignatureXML(IDComprobante)
    '                        BoletaDAO.ZipXML(IDComprobante)

    '                        'Se obtiene la entidad
    '                        ComprobanteBE = BoletaDAO.GetByID(IDComprobante)

    '                        'Se crea la instancia del reporte
    '                        Dim MiReporte As New COE.REPORT.BoletaVentaVoucher

    '                        'Se carga los datos del reporte
    '                        MiReporte.DataSource = BoletaDAO.GetByReporteID(IDComprobante)
    '                        MiReporte.DataMember = "coe_boleta_get_id_rpt"

    '                        'Se exporta en formato PDF
    '                        MiReporte.ExportToPdf(ComprobanteBE.RutaComprobantePDF)
    '                    Case "07"
    '                        NotaCreditoDAO.CreateXML(IDComprobante)
    '                        NotaCreditoDAO.SignatureXML(IDComprobante)
    '                        NotaCreditoDAO.ZipXML(IDComprobante)


    '                        'Se obtiene la entidad
    '                        ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)

    '                        Dim MiReporte As New COE.REPORT.NotaCreditoVoucher
    '                        MiReporte.DataSource = NotaCreditoDAO.GetByReporteID(IDComprobante)
    '                        MiReporte.DataMember = "coe_nota_credito_rpt_id"

    '                        MiReporte.ExportToPdf(ComprobanteBE.RutaComprobantePDF)

    '                    Case "08"
    '                        NotaDebitoDAO.CreateXML(IDComprobante)
    '                        NotaDebitoDAO.SignatureXML(IDComprobante)
    '                        NotaDebitoDAO.ZipXML(IDComprobante)
    '                End Select

    '                'Se elimina el registro
    '                ServicioDAO.Delete(IDServicioComprobante)
    '            Catch ex As Exception
    '                ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.CrearXMLFirmar, "ServicioDAO.GeneraFirmaElectronica :" & ex.Message)
    '            End Try
    '        Next
    '    Catch ex As Exception
    '        Tools.SaveLog("COE SERVICE FIRMA", ex.Message, EventLogEntryType.Error)
    '    Finally

    '    End Try
    'End Sub
    'Private Sub btnQR_Click(sender As Object, e As EventArgs) Handles btnQR.Click


    '    'Dim barCodeControl1 As BarCodeControl = New BarCodeControl()
    '    barCodeControl1.Parent = Me
    '    barCodeControl1.Size = New System.Drawing.Size(150, 150)
    '    barCodeControl1.AutoModule = True
    '    barCodeControl1.Text = "20492883281|01|F999-00000150|10.37|68.00|2018-01-12|6|20100035392"
    '    barCodeControl1.ShowText = False


    '    'Especificacion SUNAT
    '    'Nivel de correccion de error = Nivel Q
    '    Dim EspecificacionQR As QRCodeGenerator = New QRCodeGenerator()
    '    With EspecificacionQR
    '        '.SymbologyCode = Native.BarCodeSymbology.QRCode.QRCode
    '        .CompactionMode = QRCodeCompactionMode.Byte
    '        .ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q
    '        .Version = QRCodeVersion.AutoVersion

    '    End With
    '    'Se establece Especificacion del QR
    '    barCodeControl1.Symbology = EspecificacionQR

    'End Sub
    'Private Sub btnEnviarRC_Click(sender As Object, e As EventArgs) Handles btnEnviarRC.Click
    '    Dim ResumenDAO As New ResumenDAO
    '    Dim ComunicacionBajaDAO As New ComunicacionBajaDAO
    '    Dim EmisorDAO As New EmisorDAO
    '    Dim EmisorBE As New EmisorBE

    '    Dim FechaEmisionRB As String = String.Empty
    '    Dim FechaEmisionCB As String = String.Empty

    '    Try
    '        'Se obtiene los datos del emisor
    '        EmisorBE = EmisorDAO.GetByID(1)

    '        'Se obtiene la hora de ejecucion para procesar las tareas
    '        Dim HoraServicio As DateTime = EmisorBE.FechaEnvioResumenComunicacion
    '        Dim HoraActual As DateTime = DateTime.Now

    '        'Si la hora de servicio supera la hora actual se ejecuta las tareas
    '        If HoraActual > HoraServicio Then
    '            Dim HoraInicio As DateTime = DateTime.Now

    '            'Se obtiene la fecha de emision a generar. RB=Resumen de Boletas. CB=Comunicacion de Baja
    '            FechaEmisionRB = ResumenDAO.GetFechaEmision
    '            FechaEmisionCB = ComunicacionBajaDAO.GetFechaEmision

    '            'Se valida la fecha de emision
    '            If FechaEmisionRB = "" Then
    '                FechaEmisionRB = Convert.ToDateTime(EmisorBE.FechaEnvioResumenComunicacion).Date.ToString
    '                FechaEmisionCB = Convert.ToDateTime(EmisorBE.FechaEnvioResumenComunicacion).Date.ToString
    '            End If

    '            'Se crea el resumen de boletas pendientes para crear xml, firmar y empaquetar
    '            'Se envia los resumenes pendientes a SUNAT
    '            'Se envia los tickets pendientes de CDR. con una diferencia de 12 horas
    '            CrearResumenXML(FechaEmisionRB)
    '            EnviarResumenPendientes()
    '            EnviarResumenTickets()

    '            'Se crea la comunicacion de baja pendientes para crear xml, firmar y empaquetar
    '            'Se envia las comunicaciones Pendientes
    '            'Se envia los tickets pendientes de CDE con una diferencia de 12 horas
    '            CrearComunicacionXML(FechaEmisionCB)
    '            EnviarComunicacionPendientes()
    '            EnviarComunicacionTickets()

    '            'Se guarda la nueva fecha y hora para ejecutar las tareas
    '            EmisorBE.FechaEnvioResumenComunicacion = HoraServicio.AddDays(1)
    '            EmisorDAO.Save(EmisorBE)
    '        End If

    '        TextBox1.Text = "Fecha Emision RB: " & FechaEmisionRB & " Fecha Emision CB:" & FechaEmisionCB
    '    Catch ex As Exception
    '        MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try


    'End Sub
    'Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click

    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO
    '    Dim NotaDebitoDAO As New NotaDebitoDAO
    '    Dim SistemaDAO As New SistemaDAO
    '    Dim ServicioDAO As New ServicioDAO
    '    Dim ComprobanteBE As New Object
    '    Dim EmisorDAO As New EmisorDAO
    '    Dim MiReporte As Object = Nothing
    '    Dim IDTipoComprobante As Int32 = 0
    '    Dim SerieComprobante As String = String.Empty
    '    Dim dt As New DataTable

    '    Try


    '        'Se obtiene los datos del Emisor
    '        SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

    '        'Se obtiene los comprobantes para enviarlo a imprimir
    '        dt = ServicioDAO.GetByIDServicio(eServicio.ImprimirComprobante)

    '        'Se explora cada comprobante
    '        For Each dr As DataRow In dt.Rows
    '            Try

    '                Select Case dr("TipoComprobante").ToString
    '                    Case "01"
    '                        ComprobanteBE = FacturaDAO.GetByID(dr("IDComprobante"))
    '                        MiReporte = New COE.REPORT.FacturaVoucher

    '                        'Se carga los datos del reporte
    '                        MiReporte.DataSource = FacturaDAO.GetByReporteID(dr("IDComprobante"))
    '                        MiReporte.DataMember = "coe_factura_rpt_id"
    '                        IDTipoComprobante = 1
    '                        SerieComprobante = ComprobanteBE.t08_numcorrelativo.substring(0, 4)

    '                    Case "03"
    '                        ComprobanteBE = BoletaDAO.GetByID(dr("IDComprobante"))
    '                        MiReporte = New COE.REPORT.BoletaVentaVoucher

    '                        'Se carga los datos del reporte
    '                        MiReporte.DataSource = BoletaDAO.GetByReporteID(dr("IDComprobante"))
    '                        MiReporte.DataMember = "coe_boleta_rpt_id"
    '                        IDTipoComprobante = 2
    '                        SerieComprobante = ComprobanteBE.t07_numcorrelativo.substring(0, 4)

    '                    Case "07"
    '                        ComprobanteBE = NotaCreditoDAO.GetByID(dr("IDComprobante"))
    '                        MiReporte = New COE.REPORT.NotaCreditoVoucher

    '                        'Se carga los datos del reporte
    '                        MiReporte.DataSource = NotaCreditoDAO.GetByReporteID(dr("IDComprobante"))
    '                        MiReporte.DataMember = "coe_nota_credito_rpt_id"
    '                        IDTipoComprobante = 3
    '                        SerieComprobante = ComprobanteBE.t08_numcorrelativo.substring(0, 4)
    '                End Select

    '                'Se muestra el reporte
    '                Dim printTool As New ReportPrintTool(MiReporte)

    '                'Se imprime sin previsualizar
    '                printTool.Print(EmisorDAO.GetBySerie(IDTipoComprobante, SerieComprobante))


    '                'Se elimina el registro
    '                ServicioDAO.Delete(dr("IDServicioComprobante"))
    '            Catch ex As Exception
    '                ServicioDAO.Save(dr("TipoComprobante").ToString, dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.ImprimirComprobante, "ServicioDAO.GeneraImpresion :" & ex.Message)
    '            End Try
    '        Next
    '    Catch ex As Exception
    '        Tools.SaveLog("COE SERVICE IMPRESION", ex.Message, EventLogEntryType.Error)
    '    Finally

    '    End Try

    'End Sub
    'Private Sub btnCorreo_Click(sender As Object, e As EventArgs) Handles btnCorreo.Click
    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO
    '    Dim NotaDebitoDAO As New NotaDebitoDAO
    '    Dim SistemaDAO As New SistemaDAO
    '    Dim ServicioDAO As New ServicioDAO
    '    Dim ComprobanteBE As New Object
    '    Dim EmisorDAO As New EmisorDAO
    '    Dim dt As New DataTable

    '    'Se obtiene los datos del Emisor
    '    SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

    '    'Se obtiene los comprobantes para enviarlo via correo electronico
    '    dt = ServicioDAO.GetByIDServicio(eServicio.EnviarCorreo)

    '    'Se obtiene explora cada comprobante
    '    For Each dr As DataRow In dt.Rows
    '        Try

    '            'Se obtiene los comprobantes
    '            Select Case dr("TipoComprobante").ToString
    '                Case "01"
    '                    ComprobanteBE = FacturaDAO.GetByID(dr("IDComprobante"))
    '                Case "03"
    '                    ComprobanteBE = BoletaDAO.GetByID(dr("IDComprobante"))
    '                Case "07"
    '                    ComprobanteBE = NotaCreditoDAO.GetByID(dr("IDComprobante"))
    '                Case "08"
    '                    ComprobanteBE = NotaDebitoDAO.GetByID(dr("IDComprobante"))
    '            End Select

    '            Dim Mail As New MailMessage()
    '            Dim SmtpServer As New SmtpClient()
    '            Dim EmailFuente As String = SistemaDAO.EmisorBE.CorreoEnvio
    '            Dim EmailFuenteContrasena As String = SistemaDAO.EmisorBE.CorreoContrasena
    '            Dim EmailBody As String = SistemaDAO.EmisorBE.CorreoMensaje
    '            Dim EmailAsunto As String = SistemaDAO.EmisorBE.CorreoAsunto
    '            Dim ServidorHostURL As String = SistemaDAO.EmisorBE.ServidorHost
    '            Dim ServidorHostPuerto As String = SistemaDAO.EmisorBE.ServidorPuerto

    '            'Se configura para servidor de GMail
    '            SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
    '            SmtpServer.Port = ServidorHostPuerto
    '            SmtpServer.Host = ServidorHostURL
    '            SmtpServer.EnableSsl = True
    '            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network
    '            SmtpServer.ServicePoint.ConnectionLeaseTimeout = 1
    '            SmtpServer.ServicePoint.MaxIdleTime = 1

    '            ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True

    '            'Se configura el Correo
    '            Mail = New MailMessage()
    '            Mail.From = New MailAddress(EmailFuente, SistemaDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
    '            Mail.To.Add(ComprobanteBE.EmailAdquiriente)
    '            Mail.Subject = EmailAsunto
    '            Mail.Body = EmailBody
    '            Mail.Attachments.Add(New Attachment(ComprobanteBE.RutaComprobanteXML))
    '            Mail.Attachments.Add(New Attachment(ComprobanteBE.RutaComprobantePDF))
    '            Mail.IsBodyHtml = True

    '            Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

    '            'Se envia el correo
    '            SmtpServer.Send(Mail)

    '            'Se elimina el registro
    '            ServicioDAO.Delete(dr("IDServicioComprobante"))
    '        Catch ex As Exception
    '            '  ServicioDAO.Save(dr("TipoComprobante").ToString, dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.EnviarCorreo, "ServicioDAO.EnviarCorreoElectronico :" & ex.Message)
    '        End Try
    '    Next
    'End Sub
    'Private Sub btnEnviarWb_Click(sender As Object, e As EventArgs) Handles btnEnviarWb.Click
    '    Dim ServicioDAO As New ServicioDAO
    '    Dim IDServicioComprobante As Int32 = 0
    '    Dim TipoComprobante As String = String.Empty
    '    Dim IDComprobante As Int32
    '    Dim SistemaDAO As New SistemaDAO
    '    Dim EmisorDAO As New EmisorDAO
    '    Dim ComprobanteWebDAO As New ComprobanteWebDAO
    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO
    '    Dim NotaDebitoDAO As New NotaDebitoDAO
    '    Dim FacturaBE As New FacturaBE
    '    Dim BoletaBE As New BoletaBE
    '    Dim NotaCreditoBE As New NotaCreditoBE
    '    Dim NotaDebitoBE As New NotaDebitoBE
    '    Dim ExcepcionBE As New ExcepcionBE
    '    Dim dt As New DataTable
    '    Dim ComprobanteBE As New Object

    '    Try

    '        'Se detiene el servicio


    '        'Se obtiene los datos del Emisor
    '        SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

    '        'Se obtiene los comprobantes para enviarlos a la Web Azure
    '        dt = ServicioDAO.GetByIDServicio(eServicio.EnviarWeb)

    '        'Se crea la firma para cada comprobante
    '        For Each dr As DataRow In dt.Rows

    '            Try
    '                IDServicioComprobante = dr("IDServicioComprobante")
    '                TipoComprobante = dr("TipoComprobante")
    '                IDComprobante = dr("IDComprobante")


    '                'Se crea guarda el comprobante en la web
    '                Select Case TipoComprobante
    '                    Case "01"
    '                        ComprobanteBE = FacturaDAO.GetByID(IDComprobante)
    '                        'Se guarda el comprobante en la Web      
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "01", ComprobanteBE, "01") Then
    '                            FacturaDAO.SaveEstadoWeb(ComprobanteBE.idfactura, eEstadoWeb.Publicado, DateTime.Now)
    '                        End If
    '                    Case "03"
    '                        ComprobanteBE = BoletaDAO.GetByID(IDComprobante)
    '                        'Se guarda el comprobante en la Web            
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "03", ComprobanteBE, "01") Then
    '                            BoletaDAO.SaveEstadoWeb(ComprobanteBE.idboleta, eEstadoWeb.Publicado, DateTime.Now)
    '                        End If
    '                    Case "07"
    '                        ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)
    '                        'Se guarda el comprobante en la Web            
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "07", ComprobanteBE, "01") Then
    '                            NotaCreditoDAO.SaveEstadoWeb(ComprobanteBE.idnotacredito, eEstadoWeb.Publicado, DateTime.Now)
    '                        End If
    '                End Select

    '                Dim storageAccount As CloudStorageAccount
    '                storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings("StorageConnectionString"))

    '                ''Se recupera el blob
    '                Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

    '                'Se recupera el contenedor previamente creado en Azure. El nombre es minusculas por estandar
    '                'Se establece el nombre del contenedor C + Codigo Local
    '                Dim Contenedor As CloudBlobContainer = blobClient.GetContainerReference(SistemaDAO.EmisorBE.CodigoLocal)

    '                'Se crea el contenedor si no existe
    '                Contenedor.CreateIfNotExists()

    '                'Se crea dos bloques para los archivos XML y PDF
    '                Dim Bloque1 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobanteXML))
    '                Dim Bloque2 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobantePDF))

    '                'Se publica bloque 1
    '                Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobanteXML, FileMode.Open)
    '                    Bloque1.UploadFromStream(fs)
    '                End Using

    '                'Se publica bloque 1
    '                Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobantePDF, FileMode.Open)
    '                    Bloque2.UploadFromStream(fs)
    '                End Using

    '                'Se elimina
    '                ServicioDAO.Delete(IDServicioComprobante)

    '            Catch ex As Exception
    '                ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.EnviarWeb, "ServicioDAO.EnviarWeb :" & ex.Message)
    '            End Try
    '        Next
    '    Catch ex As Exception
    '        Tools.SaveLog("COE SERVICE WEB", ex.Message, EventLogEntryType.Error)
    '    Finally

    '    End Try

    'End Sub

    'Public Sub CrearResumenXML(FechaEmision As Date)
    '    Dim ResumenDAO As New ResumenDAO

    '    'Se crea el resumen de boletas segun la fecha de emision
    '    ResumenDAO.SaveResumen(FechaEmision, 1, My.Computer.Name)

    '    'Se carga el resumen de boletas para crear XML,Firmar y Empaquetar
    '    Dim dt As New DataTable
    '    dt = ResumenDAO.GetResumenBoletasPendientes(FechaEmision)

    '    'Se crea el archivo xml y se guarda
    '    For Each dr As DataRow In dt.Rows
    '        ResumenDAO.CreateXML(dr("idresumen"))
    '        ResumenDAO.SignatureXML(dr("idresumen"))
    '        ResumenDAO.ZipXML(dr("idresumen"))
    '    Next
    'End Sub
    'Public Sub EnviarResumenPendientes()
    '    Dim dt As New DataTable
    '    Dim ResumenDAO As New ResumenDAO
    '    Dim ResumenBE As New ResumenBE2018
    '    Dim ExcepcionBE As New ExcepcionBE
    '    '   Dim SunatSE As New SunatSE.billServiceClient

    '    Try


    '        'Se configura los parametros de seguridad
    '        System.Net.ServicePointManager.UseNagleAlgorithm = True
    '        System.Net.ServicePointManager.Expect100Continue = False
    '        System.Net.ServicePointManager.CheckCertificateRevocationList = True

    '        'Se crea la credencial
    '        SunatSE.ClientCredentials.CreateSecurityTokenManager()

    '        'Se abre el servicio de la SUNAT
    '        SunatSE.Open()

    '        'Se obtiene resumen pendientes de envio
    '        dt = ResumenDAO.GetByAll2(eGetResumen.ResumenPendientesEnvio)

    '        For Each item As DataRow In dt.Rows

    '            'Se obtiene la entidad
    '            ResumenBE = ResumenDAO.GetByID(item("idresumen"))
    '            ResumenDAO.IDResumen = (item("idresumen"))


    '            'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
    '            Dim NumetoTicket As String
    '            '   NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ResumenBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ResumenBE.RutaComprobanteZIP))

    '            'Se guarda el numero de ticket que envia la SUNAT
    '            ResumenDAO.SaveTicket(NumetoTicket)
    '        Next
    '    Catch ex1 As FaultException
    '        'Se guarda la excepcion de SUNAT
    '        With ExcepcionBE
    '            .IDComprobante = ResumenBE.idresumen
    '            .Descripcion = ex1.Message
    '            .CodigoExcepcion = ex1.Code.Name.ToString
    '            .IDEstado = eEstadoSunat.EnProceso
    '            .FechaHora = DateTime.Now
    '        End With
    '        ResumenDAO.SaveExcepcion(ExcepcionBE)

    '    Catch ex2 As Exception
    '        'Se guarda la excepcion del CLIENTE
    '        With ExcepcionBE
    '            .IDComprobante = ResumenBE.idresumen
    '            .Descripcion = ex2.Message.ToString
    '            .CodigoExcepcion = "9999"
    '            .IDEstado = eEstadoSunat.EnProceso
    '            .FechaHora = DateTime.Now
    '        End With
    '        ResumenDAO.SaveExcepcion(ExcepcionBE)

    '    Finally

    '        'Se cierra la conexion del servicio
    '        '      If SunatSE.State = CommunicationState.Opened Then
    '        '     SunatSE.Close()
    '        End If
    '    End Try
    'End Sub
    'Public Sub EnviarResumenTickets()
    '    'Dim dt As New DataTable
    '    'Dim ResumenDAO As New ResumenDAO
    '    'Dim ResumenBE As New ResumenBE2018
    '    'Dim ExcepcionBE As New ExcepcionBE
    '    ''   Dim SunatSE As New SunatSE.billServiceClient

    '    'Try

    '    '    'Se configura los parametros de seguridad
    '    '    System.Net.ServicePointManager.UseNagleAlgorithm = True
    '    '    System.Net.ServicePointManager.Expect100Continue = False
    '    '    System.Net.ServicePointManager.CheckCertificateRevocationList = True

    '    '    'Se crea la credencial
    '    '    SunatSE.ClientCredentials.CreateSecurityTokenManager()

    '    '    'Se abre el servicio de la SUNAT
    '    '    SunatSE.Open()

    '    '    'Se obtiene los tickets pendientes del CDR
    '    '    dt = ResumenDAO.GetByAll2(eGetResumen.TicketsPendientesCDR)

    '    '    For Each item As DataRow In dt.Rows

    '    '        'Se obtiene la entidad
    '    '        ResumenBE = ResumenDAO.GetByID(item("idresumen"))


    '    '        'Se obtiene la respuesta del envio del ticket
    '    '        '   Dim RespuestaSUNAT As SunatSE.statusResponse
    '    '        '    RespuestaSUNAT = SunatSE.getStatus(ResumenBE.Ticket)

    '    '        'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
    '    '        If RespuestaSUNAT.content.Length > 0 Then
    '    '            ResumenDAO.SaveConstanciaRecepcionZIP(ResumenBE.idresumen, RespuestaSUNAT.content)
    '    '        Else
    '    '            Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
    '    '        End If
    '    '    Next
    '    'Catch ex1 As FaultException
    '    '    'Se guarda la excepcion de SUNAT
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ResumenBE.idresumen
    '    '        .Descripcion = ex1.Message
    '    '        .CodigoExcepcion = ex1.Code.Name.ToString
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ResumenDAO.SaveExcepcion(ExcepcionBE)
    '    'Catch ex2 As Exception
    '    '    'Se guarda la excepcion del CLIENTE
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ResumenBE.idresumen
    '    '        .Descripcion = ex2.Message.ToString
    '    '        .CodigoExcepcion = "9999"
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ResumenDAO.SaveExcepcion(ExcepcionBE)
    '    'Finally
    '    '    '      If SunatSE.State = CommunicationState.Opened Then
    '    '    '      SunatSE.Close()
    '    '    End If
    '    'End Try
    'End Sub

    'Public Sub CrearComunicacionXML(FechaEmision As Date)
    '    Dim IDComunicacion As Int32 = 0
    '    IDComunicacion = ComunicacionBajaDAO.SaveComunicacionBajaXML(FechaEmision, 1, My.Computer.Name)
    '    If IDComunicacion <> 0 Then
    '        'Se crea el XML, Se firma y Se empaqueta
    '        ComunicacionBajaDAO.CreateXML(IDComunicacion)
    '        ComunicacionBajaDAO.SignatureXML(IDComunicacion)
    '        ComunicacionBajaDAO.ZipXML(IDComunicacion)

    '    End If

    'End Sub
    'Public Sub EnviarComunicacionPendientes()
    '    'Dim dt As New DataTable
    '    'Dim ComunicacionBajaBE As New ComunicacionBajaBE
    '    'Dim ExcepcionBE As New ExcepcionBE
    '    'Dim SunatSE As New SunatSE.billServiceClient

    '    'Try
    '    '    dt = ComunicacionBajaDAO.GetByAll2(eGetComunicacion.ComunicacionPendientesEnvio)

    '    '    'Se configura los parametros de seguridad
    '    '    System.Net.ServicePointManager.UseNagleAlgorithm = True
    '    '    System.Net.ServicePointManager.Expect100Continue = False
    '    '    System.Net.ServicePointManager.CheckCertificateRevocationList = True

    '    '    'Se crea la credencial
    '    '    SunatSE.ClientCredentials.CreateSecurityTokenManager()

    '    '    'Se abre el servicio de la SUNAT
    '    '    SunatSE.Open()
    '    '    For Each item As DataRow In dt.Rows

    '    '        'Se obtiene la entidad
    '    '        ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(item("idcomunicacion"))
    '    '        ComunicacionBajaDAO.IDComunicacion = item("idcomunicacion")

    '    '        'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
    '    '        Dim NumetoTicket As String
    '    '        NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ComunicacionBajaBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ComunicacionBajaBE.RutaComprobanteZIP))

    '    '        'Se guarda el numero de ticket que envia la SUNAT
    '    '        ComunicacionBajaDAO.SaveTicket(NumetoTicket)
    '    '    Next

    '    'Catch ex1 As FaultException
    '    '    'Se guarda la excepcion de SUNAT
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ComunicacionBajaBE.idcomunicacion
    '    '        .Descripcion = ex1.Message
    '    '        .CodigoExcepcion = ex1.Code.Name.ToString
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

    '    'Catch ex2 As Exception
    '    '    'Se guarda la excepcion del CLIENTE
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ComunicacionBajaBE.idcomunicacion
    '    '        .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
    '    '        .CodigoExcepcion = "9999"
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

    '    'Finally
    '    '    If SunatSE.State = CommunicationState.Opened Then
    '    '        SunatSE.Close()
    '    '    End If
    '    'End Try
    'End Sub
    'Public Sub EnviarComunicacionTickets()
    '    'Dim dt As New DataTable
    '    'Dim ComunicacionBajaBE As New ComunicacionBajaBE
    '    'Dim ExcepcionBE As New ExcepcionBE
    '    'Dim SunatSE As New SunatSE.billServiceClient
    '    'Dim RespuestaSUNAT As SunatSE.statusResponse = Nothing


    '    'Try

    '    '    'Se configura los parametros de seguridad
    '    '    System.Net.ServicePointManager.UseNagleAlgorithm = True
    '    '    System.Net.ServicePointManager.Expect100Continue = False
    '    '    System.Net.ServicePointManager.CheckCertificateRevocationList = True

    '    '    'Se crea la credencial
    '    '    SunatSE.ClientCredentials.CreateSecurityTokenManager()

    '    '    'Se abre el servicio de la SUNAT
    '    '    SunatSE.Open()
    '    '    dt = ComunicacionBajaDAO.GetByAll2(eGetComunicacion.TicketsPendientesCDR)

    '    '    For Each item As DataRow In dt.Rows

    '    '        'Se obtiene la entidad
    '    '        ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(item("idcomunicacion"))


    '    '        'Se obtiene la respuesta del envio del ticket
    '    '        RespuestaSUNAT = SunatSE.getStatus(ComunicacionBajaBE.NumeroTicket)

    '    '        'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
    '    '        If RespuestaSUNAT.content.Length > 0 Then
    '    '            ComunicacionBajaDAO.SaveConstanciaRecepcionZIP(ComunicacionBajaBE.idcomunicacion, RespuestaSUNAT.content)
    '    '        Else
    '    '            Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
    '    '        End If
    '    '    Next

    '    'Catch ex1 As FaultException
    '    '    'Se guarda la excepcion de SUNAT
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ComunicacionBajaBE.idcomunicacion
    '    '        .Descripcion = ex1.Message.ToString
    '    '        .CodigoExcepcion = ex1.Code.Name.ToString
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

    '    'Catch ex2 As Exception
    '    '    'Se guarda la excepcion del CLIENTE
    '    '    With ExcepcionBE
    '    '        .IDComprobante = ComunicacionBajaBE.idcomunicacion
    '    '        .Descripcion = ex2.Message.ToString
    '    '        .CodigoExcepcion = "9999"
    '    '        .IDEstado = eEstadoSunat.EnProceso
    '    '        .FechaHora = DateTime.Now
    '    '    End With
    '    '    ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

    '    'Finally
    '    '    If SunatSE.State = CommunicationState.Opened Then
    '    '        SunatSE.Close()
    '    '    End If
    '    'End Try
    'End Sub

End Class
