Public Class EmisorBE
    Public Property IDEmisor As Int32
    Public Property NumeroRUC As String
    Public Property RazonSocial As String
    Public Property NombreComercial As String
    Public Property CodigoUbigeo As String
    Public Property NombreDepartamento As String
    Public Property NombreProvincia As String
    Public Property NombreDistrito As String
    Public Property NombreDireccion As String
    Public Property NombreUrbanizacion As String
    Public Property RutaCarpetaArchivosXML As String
    Public Property RutaCarpetaArchivosPDF As String
    Public Property RutaCarpetaArchivosCertificados As String
    Public Property ClaveCertificado As String
    Public Property Resolucion As String
    Public Property ServidorHost As String
    Public Property ServidorPuerto As String
    Public Property CorreoEnvio As String
    Public Property CorreoContrasena As String
    Public Property CorreoAsunto As String
    Public Property CorreoMensaje As String
    Public Property CorreoAlertas As String
    Public Property FechaRegistro As String
    Public Property ReglasValidacion As Boolean
    Public Property CodigoLocal As String
    Public Property FechaEnvioResumenComunicacion As DateTime
    Public Property Impresoras As New List(Of ImpresoraBE)

    Public Property ConexionStorageCloud As String
    Public Property ConexionDataBaseCloud As String
    Public Property NombreStorageCloud As String
    Public Property SunatUser As String
    Public Property SunatPass As String

    Public Class ImpresoraBE
        Public Property IDImpresora As Int32
        Public Property NombreTipoComprobante As String
        Public Property IDTipoComprobante As Int32
        Public Property SerieComprobante As String
        Public Property NombreImpresora As String
    End Class

    Public Class ConfiguracionBE
        Public Property RUC As String
        Public Property RazonSocial As String
        Public Property ConexionDB As String

    End Class
End Class
