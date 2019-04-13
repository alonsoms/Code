#Region "Imports"
Imports System.Globalization
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports COE.DATA
Imports COE.FRAMEWORK
#End Region

Public Class AlertasSE
    Dim AlertasDAO As New AlertasDAO
    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim Tiempo As New System.Timers.Timer

#Region "Eventos del Servicio"
    Protected Overrides Sub OnStart(ByVal args() As String)

        'Se carga la cultura de Peru en la aplicacion
        Dim MiCultura As New CultureInfo("es-PE", False)

        'Se establece la cultura de peru
        System.Threading.Thread.CurrentThread.CurrentCulture = MiCultura
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

        'Se estable el evento
        AddHandler Tiempo.Elapsed, AddressOf EnviarAlertas

        'Se activa el timer. Se convierte minutos a milisegundos 1 Minutos=60,000 milisegundos
        'Se configura para cada 30 minutos = 1,800,000 ms
        Tiempo.Interval = 1800000
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

    Public Sub EnviarAlertas()
        Dim Mensaje As String = String.Empty

        Try
            'Se detiene el timer
            Tiempo.Stop()

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
            Tools.SaveLog("COE SERVICE ALERTAS", ex.Message, EventLogEntryType.Error)
        Finally
            Tiempo.Start()
        End Try
    End Sub

End Class
