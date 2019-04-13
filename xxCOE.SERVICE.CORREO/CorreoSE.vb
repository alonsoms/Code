Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.Globalization
Imports System.Net.Mail
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security

Public Class CorreoSE
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
        AddHandler Tiempo.Elapsed, AddressOf EnviarCorreoElectronico

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

    Public Sub EnviarCorreoElectronico()
        Dim FacturaDAO As New FacturaDAO
        Dim BoletaDAO As New BoletaVentaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO
        Dim NotaDebitoDAO As New NotaDebitoDAO
        Dim SistemaDAO As New SistemaDAO
        Dim ServicioDAO As New ServicioDAO
        Dim ComprobanteBE As New Object
        Dim EmisorDAO As New EmisorDAO
        Dim dt As New DataTable

        'Se obtiene los datos del Emisor
        SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

        'Se obtiene los comprobantes para enviarlo via correo electronico
        dt = ServicioDAO.GetByIDServicio(eServicio.EnviarCorreo)

        'Se obtiene explora cada comprobante
        For Each dr As DataRow In dt.Rows
            Try

                'Se obtiene los comprobantes
                Select Case dr("TipoComprobante").ToString
                    Case "01"
                        ComprobanteBE = FacturaDAO.GetByID(dr("IDComprobante"))
                    Case "03"
                        ComprobanteBE = BoletaDAO.GetByID(dr("IDComprobante"))
                    Case "07"
                        ComprobanteBE = NotaCreditoDAO.GetByID(dr("IDComprobante"))
                    Case "08"
                        ComprobanteBE = NotaDebitoDAO.GetByID(dr("IDComprobante"))
                End Select

                Dim Mail As New MailMessage()
                Dim SmtpServer As New SmtpClient()
                Dim EmailFuente As String = SistemaDAO.EmisorBE.CorreoEnvio
                Dim EmailFuenteContrasena As String = SistemaDAO.EmisorBE.CorreoContrasena
                Dim EmailBody As String = SistemaDAO.EmisorBE.CorreoMensaje
                Dim EmailAsunto As String = SistemaDAO.EmisorBE.CorreoAsunto
                Dim ServidorHostURL As String = SistemaDAO.EmisorBE.ServidorHost
                Dim ServidorHostPuerto As String = SistemaDAO.EmisorBE.ServidorPuerto

                'Se configura para servidor de GMail
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
                Mail.To.Add(ComprobanteBE.EmailAdquiriente)
                Mail.Subject = EmailAsunto
                Mail.Body = EmailBody
                Mail.Attachments.Add(New Attachment(ComprobanteBE.RutaComprobanteXML))
                Mail.Attachments.Add(New Attachment(ComprobanteBE.RutaComprobantePDF))
                Mail.IsBodyHtml = True

                Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

                'Se envia el correo
                SmtpServer.Send(Mail)

                'Se elimina el registro
                ServicioDAO.Delete(dr("IDServicioComprobante"))
            Catch ex As Exception
                ServicioDAO.Save(dr("TipoComprobante").ToString, dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.EnviarCorreo, "ServicioDAO.EnviarCorreoElectronico :" & ex.Message)
            End Try
        Next
    End Sub

End Class
