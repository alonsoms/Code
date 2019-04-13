Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports COE.DATA
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob

Partial Class Content_ConsultaCE
    Inherits System.Web.UI.UserControl

    Dim ComprobanteDAO As New ComprobanteWebDAO
    Dim ComprobanteWebBE As New ComprobanteWebBE
    Dim ComprobanteWeb As New ComprobanteWeb

    Public Property NumRuc As String
    Public Property Logo As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Page.IsPostBack Then
            Exit Sub
        End If

        'Se establece valores por defecto
        ASPxDateEdit1.Value = DateTime.Now
        ASPxCaptcha1.RefreshButton.ShowImage = True

    End Sub

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        'Se valida el Captcha y el literal se queda vacio para borrar la visualizacion del PDF
        If Not ASPxCaptcha1.IsValid Then
            Literal1.Text = ""
            Exit Sub
        End If

        'Se valida numero de comprobante
        If txtSerDoc.Text.Trim.Length = 0 Then
            Exit Sub
        End If

        'Se obtiene los datos del formulario
        With ComprobanteWebBE
            .Tipo = cboDocumentos2.Value
            .NumeroCorrelativo = txtSerDoc.Text
            .FechaEmision = ASPxDateEdit1.Value
            .Importe = Convert.ToDecimal(txtMonTot2.Text)
            .NumeroRUC = Me.NumRuc
        End With


        Try
            'Se obtiene la entidad
            ComprobanteWebBE = ComprobanteDAO.GetByNumero(ComprobanteWebBE)

            'Se guarda la sesion
            Session("ComprobanteWeb") = ComprobanteWebBE

            'Se copia los archivos a la carpeta temporal
            Literal1.Text = ComprobanteWeb.CopiarComprobanteCarpetaTemporal(ComprobanteWebBE, NumRuc)

        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class
