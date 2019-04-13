Public Class Form3
    Private Sub btnAutenticar_Click(sender As Object, e As EventArgs) Handles btnAutenticar.Click
        'Dim SunatCDRSE As New SunatSE.billServiceClient
        'Dim RespuestaSUNAT As SunatSE.statusResponse

        ''Se configura los parametros de seguridad
        'System.Net.ServicePointManager.UseNagleAlgorithm = True
        'System.Net.ServicePointManager.Expect100Continue = False
        'System.Net.ServicePointManager.CheckCertificateRevocationList = True

        'Try
        '    'Se crea la credencial
        '    SunatCDRSE.ClientCredentials.CreateSecurityTokenManager()

        '    'Se agrega las credenciales en el objeto del Behavior
        '    Dim PB = New PasswordBehavior("20492883281JLRAMOS8", "jj2007ra")
        '    SunatCDRSE.Endpoint.EndpointBehaviors.Add(PB)

        '    'Se abre el servicio de la SUNAT
        '    SunatCDRSE.Open()

        '    'Se obtiene la respuesta del envio del resumen
        '    ' RespuestaSUNAT = SunatCDRSE.getStatusCdr("20492883281", "01", "F011", "00000097")

        'Catch ex As Exception
        '    MessageBox.Show(ex.Message, "Advertencia")
        'Finally
        '    SunatCDRSE.Close()
        'End Try
    End Sub
End Class