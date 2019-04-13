Public Class ComunicacionBajaBE
    Public Property idcomunicacion As Int32
    Public Property t03_fecemisiondoc As String
    Public Property t09_numcorrelativo As String
    Public Property t10_feccomunicacion As String
    Public Property t12_versionubl As String
    Public Property t13_versiondoc As String
    Public Property ValorFirma As String
    Public Property NumeroTicket As String
    Public Property estado As Int32
    Public Property fechahorasistemaexterno() As DateTime
    Public Property fechahorasunat As Nullable(Of DateTime)
    Public Property fechahorarespuestasunat() As DateTime
    Public Property RutaComprobanteXML As String
    Public Property RutaComprobanteZIP As String
    Public Property RutaComprobantePDF As String
    Public Property RutaRespuestaSunatXML As String
    Public Property RutaRespuestaSunatZIP As String
    Public Property Observacion As String
    Public Property CodigoRespuesta As String
    Public Property idusuario As Int32
    Public Property fecharegistro As String

    Public Property Detalle As List(Of ComunicacionBajaItem)
End Class

Public Class ComunicacionBajaItem
    Public Property idcomunicaciondetalle As Int32
    Public Property idcomunicacion As Int32
    Public Property t04_tipdoc_c01 As String
    Public Property t05_serdocbaja As String
    Public Property t06_numdocbaja As String
    Public Property t07_motivobaja As String
    Public Property t08_numordenitem As String
End Class