Public Class ResumenBE2018
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
    Public Property Lineas As List(Of ResumenLinea2018)

    Public Property estado As Int32
    Public Property digestvalue As String
    Public Property ValorFirma As String
    Public Property archivoxml As String
    Public Property maquina As String
    Public Property idcomprobanteformato As Int32
    Public Property fechahorasistemaexterno As Nullable(Of DateTime)
End Class

Public Class ResumenLinea2018
    Public Property idresumendetalle As Int32
    Public Property idresumen As Int32
    Public Property t01_numfila As String
    Public Property t02_tipdoc_c01 As String
    Public Property t02_numcorrelativo As String
    Public Property t03_numdoc_adquiriente As String
    Public Property t03_tipdoc_adquiriente As String
    Public Property t04_numcorrelativo_modifica As String
    Public Property t04_tipdoc_c01 As String
    Public Property t05_estadoitem_c19 As String
    Public Property t06_importetotal As String
    Public Property t07_totalvalorventagravadas As String
    Public Property t07_totalvalorventagravadas_c11 As String
    Public Property t08_totalvalorventaexoneradas As String
    Public Property t08_totalvalorventaexoneradas_c11 As String
    Public Property t09_totalvalorventainafectas As String
    Public Property t09_totalvalorventainafectas_c11 As String
    Public Property t10_totalvalorventasgratuitas As String
    Public Property t10_totalvalorventasgratuitas_c11 As String
    Public Property t11_indicadorcargo As String
    Public Property t11_importetotalotroscargos As String
    Public Property t12_totalisc As String
    Public Property t12_totalisc_item As String
    Public Property t12_totalisc_codtributo_c05 As String
    Public Property t12_totalisc_nomtributo_c05 As String
    Public Property t12_totalisc_codintertributo_c05 As String
    Public Property t13_totaligv As String
    Public Property t13_totaligvitem As String
    Public Property t13_totaligv_codtributo_c05 As String
    Public Property t13_totaligv_nomtributo_c05 As String
    Public Property t13_totaligv_codintertributo_c05 As String
    Public Property t14_totalotrostributos As String
    Public Property t14_totalotrostributos_item As String
    Public Property t14_totalotrostributos_codtributo_c05 As String
    Public Property t14_totalotrostributos_nomtributo_c05 As String
    Public Property t14_totalotrostributos_codintertributo_c05 As String
End Class