Imports System.Configuration

Public Class InitConfiguracion
    Public Property HoraEnvioResumenComprobantesSUNAT As String
    Public Property MinutosEnvioComprobantesSUNAT As String
    Public Property RutaCarpetaSUNAT As String


    Public Sub New()

        ''Se carga la configuracion del App.Config
        'Me.HoraEnvioResumenComprobantesSUNAT = ConfigurationManager.AppSettings("HoraEnvioResumenComprobantesSUNAT")
        'Me.MinutosEnvioComprobantesSUNAT = ConfigurationManager.AppSettings("MinutosEnvioComprobantesSUNAT")
        'Me.RutaCarpetaSUNAT = ConfigurationManager.AppSettings("RutaCarpetaSUNAT")

    End Sub

End Class
