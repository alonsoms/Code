Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports COE.DATA
Imports Microsoft.Azure
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob


Partial Class UI_cevatexsac
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-PE")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-PE")


        'Se establece el numero de RUC del cliente
        ConsultaCE.NumRuc = "20546266908"
        ConsultaCE.Logo = "logo.jpg"

    End Sub


End Class
