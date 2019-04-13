Public Class ResumenBE
    Public Property idresumen As Int32
    Public Property t03_fecemision As String
    Public Property t17_numcorrelativo As String
    Public Property t18_fecresumen As String
    Public Property t20_versionubl As String
    Public Property t21_versiondoc As String
    Public Property RutaComprobanteXML As String
    Public Property RutaRespuestaSunatXML As String
    Public Property RutaComprobanteZIP As String
    Public Property RutaComprobantePDF As String
    Public Property RutaRespuestaSunatZIP As String

    Public Property Observacion As String
    Public Property CodigoRespuesta As String
    Public Property Ticket As String
    Public Property Detalle As List(Of ResumenItem)
    Public Property estado As Int32
    Public Property digestvalue As String
    Public Property ValorFirma As String
    Public Property archivoxml As String
    Public Property maquina As String
    Public Property idcomprobanteformato As Int32
    Public Property fechahorasistemaexterno As Nullable(Of DateTime)
End Class

Public Class ResumenItem
    Public Property idresumendetalle As Int32
    Public Property idresumen As Int32
    Public Property t16_numordenitem As String
    Public Property t04_tipdoc_c01 As String
    Public Property t05_numserdoc As String
    Public Property t06_numcorrelativoinicio As String
    Public Property t07_numcorrelativofin As String
    Public Property t08_totalvalorventasgravadas As String
    Public Property t08_tipmonto_c11 As String
    Public Property t09_totalvalorventasexoneradas As String
    Public Property t09_tipmonto_c11 As String
    Public Property t10_totalvalorventasinafectas As String
    Public Property t10_tipmonto_c11 As String
    Public Property t11_totalimporteotroscargos As String
    Public Property t11_indicadorcargo As String
    Public Property t12_totalisc As String
    Public Property t12_subtotalisc As String
    Public Property t12_tiptributo_c05 As String
    Public Property t12_nomtributo_c05 As String
    Public Property t12_tiptributointernacional_c05 As String
    Public Property t13_totaligv As String
    Public Property t13_subtotaligv As String
    Public Property t13_tiptributo_c05 As String
    Public Property t13_nomtributo_c05 As String
    Public Property t13_tiptributointernacional_c05 As String
    Public Property t14_totalotrostributos As String
    Public Property t14_subtotalotrostributos As String
    Public Property t14_tiptributo_c05 As String
    Public Property t14_nomtributo_c05 As String
    Public Property t14_tiptributointernacional_c05 As String
    Public Property t15_totalimporteventa As String
    Public Property t15_tipmoneda_c02 As String
End Class
