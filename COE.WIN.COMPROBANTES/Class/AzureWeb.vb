Imports System.IO
Imports System.Net.Mail
Imports COE.DATA
Imports COE.FRAMEWORK


Public Class AzureWeb
    Shared Property objForm As Form

    Public Function EnviarWeb(TipoComprobante As eTipoComprobante, IDComprobante As Int32, ByRef EmisorDAO As EmisorDAO) As Boolean
        Dim ComprobanteBE As New Object
        Dim NumComprobante As String = String.Empty
        Dim Result As Boolean = False
        Dim RutaXML As String = String.Empty
        Dim RutaPDF As String = String.Empty

        Try
            'Se obtiene el comprobante
            Select Case TipoComprobante
                Case eTipoComprobante.Factura
                    ComprobanteBE = FacturaDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t08_numcorrelativo
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF

                Case eTipoComprobante.Boleta
                    ComprobanteBE = BoletaDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t07_numcorrelativo
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF

                Case eTipoComprobante.NotaCredito
                    ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t08_numcorrelativo
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF
            End Select

            'Se valida que el estado del comprobante sea Aceptado por SUNAT para publicarlo en la web
            If ComprobanteBE.estado <> eEstadoSunat.Aceptado Then
                MessageBox.Show(String.Format("No se puede publicar el comprobante {0}, es necesario que tenga el estado Aceptado por la SUNAT", NumComprobante), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se valida que exista el archivo XML
            If Not File.Exists(RutaXML) Then
                MessageBox.Show(String.Format("El archivo XML no existe {0}", RutaXML), "No se puede enviar el correo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se valida que exista el archivo PDF
            If Not File.Exists(RutaPDF) Then
                MessageBox.Show(String.Format("El archivo PDF no existe {0}", RutaPDF), "No se puede enviar el correo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If


            'Se pregunta si esta seguro de publicarlo en la web
            If MessageBox.Show(String.Format("¿Esta seguro de publicar el comprobante {0} en la Web?", NumComprobante), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Try
            End If

            Tools.WinProcess(objForm, False)

            Select Case TipoComprobante
                Case eTipoComprobante.Factura
                    If FacturaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                        If FacturaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                            FacturaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                            Result = True
                        End If
                    End If

                Case eTipoComprobante.Boleta
                    If BoletaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                        If BoletaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                            BoletaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                            Result = True
                        End If
                    End If

                Case eTipoComprobante.NotaCredito
                    If NotaCreditoDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                        If NotaCreditoDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                            NotaCreditoDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                            Result = True
                        End If
                    End If
            End Select


        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Tools.WinProcess(objForm, True)
        End Try

        Return Result
    End Function

    Public Function EnviarEmail(TipoComprobante As eTipoComprobante, IDComprobante As Int32, ByRef EmisorDAO As EmisorDAO) As Boolean
        Dim ComprobanteBE As New Object
        Dim NumComprobante As String = String.Empty
        Dim EmailCliente As String = String.Empty
        Dim RutaXML As String = String.Empty
        Dim RutaPDF As String = String.Empty
        Dim Result As Boolean = False

        Try

            'Se obtiene el comprobante
            Select Case TipoComprobante
                Case eTipoComprobante.Factura
                    ComprobanteBE = FacturaDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t08_numcorrelativo
                    EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF

                Case eTipoComprobante.Boleta
                    ComprobanteBE = BoletaDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t07_numcorrelativo
                    EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF

                Case eTipoComprobante.NotaCredito
                    ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)
                    NumComprobante = ComprobanteBE.t08_numcorrelativo
                    EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                    RutaXML = ComprobanteBE.RutaComprobanteXML
                    RutaPDF = ComprobanteBE.RutaComprobantePDF
            End Select

            'Se valida que el cliente tenga un email
            If EmailCliente.Trim.Length = 0 Then
                MessageBox.Show(String.Format("El comprobante {0} no tiene el correo electrónico del cliente.", NumComprobante), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se valida que exista el archivo XML
            If Not File.Exists(RutaXML) Then
                MessageBox.Show(String.Format("El archivo XML no existe {0}", RutaXML), "No se puede enviar el correo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se valida que exista el archivo PDF
            If Not File.Exists(RutaPDF) Then
                MessageBox.Show(String.Format("El archivo PDF no existe {0}", RutaPDF), "No se puede enviar el correo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se valida que el estado del comprobante sea Aceptado por SUNAT para enviarlo por correo
            If ComprobanteBE.estado <> eEstadoSunat.Aceptado Then
                MessageBox.Show(String.Format("No se puede enviar el comprobante {0}", NumComprobante), "No tiene el estado aceptado de SUNAT", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            'Se pregunta si esta seguro de enviar por correo
            If MessageBox.Show(String.Format("¿Esta seguro de enviar el comprobante {0} ?", NumComprobante), "Enviar comprobante por correo electrónico", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Try
            End If

            Tools.WinProcess(objForm, False)

            Dim Mail As New MailMessage()
            Dim SmtpServer As New SmtpClient()
            Dim EmailFuente As String = EmisorDAO.EmisorBE.CorreoEnvio
            Dim EmailFuenteContrasena As String = EmisorDAO.EmisorBE.CorreoContrasena
            Dim EmailBody As String = EmisorDAO.EmisorBE.CorreoMensaje
            Dim EmailAsunto As String = EmisorDAO.EmisorBE.CorreoAsunto
            Dim ServidorHostURL As String = EmisorDAO.EmisorBE.ServidorHost
            Dim ServidorHostPuerto As String = EmisorDAO.EmisorBE.ServidorPuerto

            'Se configura para servidor de GMail
            SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
            SmtpServer.Port = ServidorHostPuerto
            SmtpServer.Host = ServidorHostURL
            SmtpServer.EnableSsl = True

            'Se configura el Correo
            Mail = New MailMessage()
            Mail.From = New MailAddress(EmailFuente, EmisorDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
            Mail.To.Add(EmailCliente)
            Mail.Subject = EmailAsunto
            Mail.Body = EmailBody
            Mail.Attachments.Add(New Attachment(RutaXML))
            Mail.Attachments.Add(New Attachment(RutaPDF))
            Mail.IsBodyHtml = True

            Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

            'Se envia el correo
            SmtpServer.Send(Mail)

            Result = True

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Tools.WinProcess(objForm, True)
        End Try

        Return Result
    End Function
End Class
