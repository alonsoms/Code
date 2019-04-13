Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports COE.DATA
Imports COE.FRAMEWORK
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob

Public Class EnvioWebSE
    Dim EmisorDAO As New EmisorDAO
    Dim ServicioDAO As New ServicioDAO
    Dim Tiempo As New System.Timers.Timer

#Region "Eventos del Servicio"
    Protected Overrides Sub OnStart(ByVal args() As String)

        'Se carga la cultura de Peru en la aplicacion
        Dim MiCultura As New CultureInfo("es-PE", False)

        'Se establece la cultura de peru
        System.Threading.Thread.CurrentThread.CurrentCulture = MiCultura
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

        'Se estable el evento
        AddHandler Tiempo.Elapsed, AddressOf ExecuteService

        'Se activa el timer cada 6 minutos. 
        Tiempo.Interval = Tools.MinutosToMilisegundos(6)
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

    Public Sub ExecuteService()
        Try
            'Se detiene el servicio para ejecutar la tarea
            Tiempo.Stop()

            'Se carga los emisores
            EmisorDAO.GetByConfigXML()

            'Se envia los comprobantes a Azure
            EmisorDAO.EnviarComprobantesAzure()


        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", "Envio Web : " & ex.Message, EventLogEntryType.Error)
        Finally
            Tiempo.Start()
        End Try
    End Sub


    Public Sub EnviarAlertas()
        Dim AlertasDAO As New AlertasDAO
        Dim SistemaDAO As New SistemaDAO
        Dim Mensaje As String = String.Empty

        Try


            'Se obtiene las alertas a enviar
            Mensaje = AlertasDAO.GetByAll

            'Se valida si hay alertas que enviar
            If Mensaje.ToString.Trim <> "" Then

                'Se obtiene los datos del Emisor
                SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

                Dim Mail As New MailMessage()
                Dim SmtpServer As New SmtpClient()
                Dim EmailFuente As String = SistemaDAO.EmisorBE.CorreoEnvio
                Dim EmailFuenteContrasena As String = SistemaDAO.EmisorBE.CorreoContrasena
                Dim EmailBody As String = Mensaje
                Dim EmailAsunto As String = "ALERTA DE COMPROBANTES ELECTRONICOS"
                Dim ServidorHostURL As String = SistemaDAO.EmisorBE.ServidorHost
                Dim ServidorHostPuerto As String = SistemaDAO.EmisorBE.ServidorPuerto
                Dim EmailAlerta As String = SistemaDAO.EmisorBE.CorreoAlertas

                'Se configura para servidor de correos 
                SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
                SmtpServer.Port = ServidorHostPuerto
                SmtpServer.Host = ServidorHostURL
                SmtpServer.EnableSsl = True
                SmtpServer.ServicePoint.ConnectionLeaseTimeout = 1
                SmtpServer.ServicePoint.MaxIdleTime = 1
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network
                ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True

                'Se configura el Correo
                Mail = New MailMessage()
                Mail.From = New MailAddress(EmailFuente, SistemaDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
                Mail.To.Add(EmailAlerta)
                Mail.Subject = EmailAsunto
                Mail.Body = EmailBody
                Mail.IsBodyHtml = True
                Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

                'Se envia el correo
                SmtpServer.Send(Mail)
            End If
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio Alertas: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    'Public Function SaveWebComprobante(ByRef EmisorDAO As EmisorDAO, ByRef ComprobanteWebDAO As ComprobanteWebDAO, ByRef ComprobanteBE As Object, dr As DataRow) As Boolean
    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO

    '    Dim TipoComprobante As String = String.Empty
    '    Dim IDComprobante As Int32
    '    Dim IDServicioComprobante As Int32
    '    Dim Result As Boolean = False

    '    IDServicioComprobante = dr("IDServicioComprobante")
    '    TipoComprobante = dr("TipoComprobante")
    '    IDComprobante = dr("IDComprobante")

    '    'Se crea guarda el comprobante en la web, segun el tipo de comprobante
    '    Select Case TipoComprobante
    '        Case "01"
    '            If FacturaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
    '                If FacturaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
    '                    FacturaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
    '                    Result = True
    '                End If
    '            End If
    '        Case "03"
    '            If BoletaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
    '                If BoletaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
    '                    BoletaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
    '                    Result = True
    '                End If
    '            End If
    '        Case "07"
    '            If NotaCreditoDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
    '                If NotaCreditoDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
    '                    NotaCreditoDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
    '                    Result = True
    '                End If
    '            End If
    '    End Select
    '    Return Result
    'End Function
    Public Sub SaveWebComprobantePdfXML(EmisorDAO As EmisorDAO, ComprobanteBE As Object)
        Dim storageAccount As CloudStorageAccount
        storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings("StorageConnectionString"))

        'Se recupera el blob
        Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

        'Se recupera el contenedor previamente creado en Azure. El nombre es minusculas por estandar
        'Se establece el nombre del contenedor Codigo Local
        Dim Contenedor As CloudBlobContainer = blobClient.GetContainerReference(EmisorDAO.EmisorBE.CodigoLocal)

        'Se crea el contenedor si no existe
        Contenedor.CreateIfNotExists()

        'Se crea dos bloques para los archivos XML y PDF
        Dim Bloque1 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobanteXML))
        Dim Bloque2 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobantePDF))

        'Se publica bloque 1
        Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobanteXML, FileMode.Open)
            Bloque1.UploadFromStream(fs)
        End Using

        'Se publica bloque 1
        Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobantePDF, FileMode.Open)
            Bloque2.UploadFromStream(fs)
        End Using
    End Sub

    'Public Sub EnviarComprobantesWeb()
    '    Dim ServicioDAO As New ServicioDAO
    '    Dim SistemaDAO As New SistemaDAO
    '    Dim ComprobanteWebDAO As New ComprobanteWebDAO
    '    Dim FacturaDAO As New FacturaDAO
    '    Dim BoletaDAO As New BoletaVentaDAO
    '    Dim NotaCreditoDAO As New NotaCreditoDAO
    '    Dim NotaDebitoDAO As New NotaDebitoDAO
    '    Dim EmisorDAO As New EmisorDAO
    '    Dim IDServicioComprobante As Int32 = 0
    '    Dim TipoComprobante As String = String.Empty
    '    Dim IDComprobante As Int32

    '    Dim FacturaBE As New FacturaBE
    '    Dim BoletaBE As New BoletaBE
    '    Dim NotaCreditoBE As New NotaCreditoBE
    '    Dim NotaDebitoBE As New NotaDebitoBE
    '    Dim ExcepcionBE As New ExcepcionBE
    '    Dim dt As New DataTable
    '    Dim ComprobanteBE As New Object
    '    Dim Result As Boolean = False

    '    Try

    '        'Se detiene el servicio
    '        Tiempo.Stop()

    '        'Se obtiene los datos del Emisor
    '        SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

    '        'Se obtiene los comprobantes para enviarlos a la Web Azure
    '        dt = ServicioDAO.GetByIDServicio(eServicio.EnviarWeb)

    '        For Each dr As DataRow In dt.Rows
    '            Result = False

    '            Try
    '                IDServicioComprobante = dr("IDServicioComprobante")
    '                TipoComprobante = dr("TipoComprobante")
    '                IDComprobante = dr("IDComprobante")

    '                'Se crea guarda el comprobante en la web, segun el tipo de comprobante
    '                Select Case TipoComprobante
    '                    Case "01"
    '                        ComprobanteBE = FacturaDAO.GetByID(IDComprobante)
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "01", ComprobanteBE, SistemaDAO.EmisorBE.CodigoLocal) Then
    '                            FacturaDAO.SaveEstadoWeb(ComprobanteBE.idfactura, eEstadoWeb.Publicado, DateTime.Now)
    '                            Result = True
    '                        End If
    '                    Case "03"
    '                        ComprobanteBE = BoletaDAO.GetByID(IDComprobante)
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "03", ComprobanteBE, SistemaDAO.EmisorBE.CodigoLocal) Then
    '                            BoletaDAO.SaveEstadoWeb(ComprobanteBE.idboleta, eEstadoWeb.Publicado, DateTime.Now)
    '                            Result = True
    '                        End If
    '                    Case "07"
    '                        ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)
    '                        If ComprobanteWebDAO.Save(SistemaDAO.EmisorBE.NumeroRUC, "07", ComprobanteBE, SistemaDAO.EmisorBE.CodigoLocal) Then
    '                            NotaCreditoDAO.SaveEstadoWeb(ComprobanteBE.idnotacredito, eEstadoWeb.Publicado, DateTime.Now)
    '                            Result = True
    '                        End If
    '                End Select

    '                If Result Then

    '                    Dim storageAccount As CloudStorageAccount
    '                    storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings("StorageConnectionString"))

    '                    ''Se recupera el blob
    '                    Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

    '                    'Se recupera el contenedor previamente creado en Azure. El nombre es minusculas por estandar
    '                    'Se establece el nombre del contenedor Codigo Local
    '                    Dim Contenedor As CloudBlobContainer = blobClient.GetContainerReference(SistemaDAO.EmisorBE.CodigoLocal)

    '                    'Se crea el contenedor si no existe
    '                    Contenedor.CreateIfNotExists()

    '                    'Se crea dos bloques para los archivos XML y PDF
    '                    Dim Bloque1 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobanteXML))
    '                    Dim Bloque2 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobantePDF))

    '                    'Se publica bloque 1
    '                    Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobanteXML, FileMode.Open)
    '                        Bloque1.UploadFromStream(fs)
    '                    End Using

    '                    'Se publica bloque 1
    '                    Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobantePDF, FileMode.Open)
    '                        Bloque2.UploadFromStream(fs)
    '                    End Using

    '                    'Se elimina
    '                    ServicioDAO.Delete(IDServicioComprobante)
    '                End If
    '            Catch ex As Exception
    '                ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.EnviarWeb, "ServicioDAO.EnviarWeb :" & ex.Message)
    '            End Try
    '        Next
    '    Catch ex As Exception
    '        Tools.SaveLog("COE SERVICE WEB", ex.Message, EventLogEntryType.Error)
    '    Finally
    '        Tiempo.Start()
    '    End Try

    'End Sub
End Class
