Public Class BoletaBE
    Public Property idboleta As Int32
    Public Property TipoComprobante As String = "03"
    Public Property t01_fecemision As String
    Public Property t01_horaemision As String

    Public Property t06_tipdoc_c01 As String
    Public Property t07_numcorrelativo As String
    Public Property t08_numdoc As String
    Public Property t08_tipdoc_c06 As String
    Public Property t09_nomadquiriente As String
    Public Property t10_direccionadquiriente As String
    Public Property t15_tipmonto_c14 As String
    Public Property t15_totalvalorventagravadas As String
    Public Property t16_tipmonto_c14 As String
    Public Property t16_totalvalorventainafectas As String
    Public Property t17_tipmonto_c14 As String
    Public Property t17_totalvalorventaexonerada As String
    Public Property t18_totaligv As String
    Public Property t18_subtotaligv As String
    Public Property t18_tiptributo_c05 As String
    Public Property t18_nomtributo_c05 As String
    Public Property t18_tiptributointernacional_c05 As String
    Public Property t19_totalisc As String
    Public Property t19_subtotalisc As String
    Public Property t19_tiptributo_c05 As String
    Public Property t19_nomtributo_c05 As String
    Public Property t19_tiptributointernacional_c05 As String
    Public Property t20_totalotrostributos As String
    Public Property t20_subtotalotrostributos As String
    Public Property t20_tiptributo_c05 As String
    Public Property t20_nomtributo_c05 As String
    Public Property t20_tiptributointernacional_c05 As String
    Public Property t21_totalotroscargos As String
    Public Property t22_tipmonto_c14 As String
    Public Property t22_totaldescuentos As String
    Public Property t23_totalimporte As String
    Public Property t24_tipmoneda_c02 As String
    Public Property t25_sernumguia As String
    Public Property t25_tipdoc_c01 As String
    Public Property t26_tipleyenda_c15 As String
    Public Property t26_descripcionleyenda As String
    Public Property t34_tipmonto_c14 As String
    Public Property t34_montopercepcion As String
    Public Property t34_montototalpercepcion As String
    Public Property t35_numdocrelacionado As String
    Public Property t35_tipdoc_c12 As String
    Public Property t37_versionubl As String
    Public Property t38_versiondoc As String
    Public Property t39_tipdoc_c14 As String
    Public Property t39_baseimponiblepercepcion As String
    Public Property t39_montopercepcion As String
    Public Property t39_montototalpercepcion As String
    Public Property t40_tipmonto_c14 As String
    Public Property t40_totalvalorventaoperacionesgratuitas As String
    Public Property t41_descuentosglobales As String
    Public Property t42_tipoperacion_c17 As String


    Public Property estado As Int32
    Public Property estadoboleta As Int32
    Public Property digestvalue As String
    Public Property archivoxml As String
    Public Property maquina As String
    Public Property idcomprobanteformato As Int32
    Public Property fechahorasistemaexterno As Nullable(Of DateTime)


    Public Property fechahorasilc As Nullable(Of DateTime)
    Public Property fechahorasunat As Nullable(Of DateTime)
    Public Property fechahorarespuestasunat As Nullable(Of DateTime)
    Public Property RutaComprobanteXML As String
    Public Property RutaComprobanteZIP As String
    Public Property RutaComprobantePDF As String
    Public Property RutaRespuestaSunatXML As String
    Public Property RutaRespuestaSunatZIP As String
    Public Property SerieComprobante As String
    Public Property NumeroComprobante As String
    Public Property CodigoRespuesta As String
    Public Property Observacion As String
    Public Property DireccionAdquiriente As String
    Public Property EmailAdquiriente As String
    Public Property idusuario As Int32
    Public Property fecharegistro As Nullable(Of DateTime)
    Public Property Detalle As List(Of BoletaItem)
    Public Property Anticipos As New List(Of BoletaAnticipo)
End Class
Public Class BoletaItem
    Public Property idboletadetalle As Int32
    Public Property idboleta As Int32
    Public Property t11_tipunidadmedida_c03 As String
    Public Property t12_cantidad As String
    Public Property t13_descripcion As String
    Public Property t14_preciounitario As String
    Public Property t14_tipprecio_c16 As String
    Public Property t27_totalitem As String
    Public Property t27_subtotalitem As String
    Public Property t27_tipafectacion_c07 As String
    Public Property t27_tiptributo_c05 As String
    Public Property t27_nomtributo_c05 As String
    Public Property t27_tiptributointernacional_c05 As String
    Public Property t28_totalitem As String
    Public Property t28_subtotalitem As String
    Public Property t28_tipsistema_c08 As String
    Public Property t28_tiptributo_c05 As String
    Public Property t28_nomtributo_c05 As String
    Public Property t28_tiptributointernacional_c05 As String
    Public Property t29_numordenitem As String
    Public Property t30_codproducto As String
    Public Property t31_valorunitario As String
    Public Property t32_valorventaitem As String
    Public Property t33_valorreferencialunitarioitem As String
    Public Property t33_tipreferencia_c16 As String
    Public Property t42_tipdescuentoitem As String
    Public Property t42_descuentoitem As String
    Public Property t44_porcentajeISC As String
End Class
Public Class BoletaAnticipo
    Public Property IDAnticipo As Int32
    Public Property IDComprobante As Int32
    Public Property Referencia As String
    Public Property TipoComprobante As String
    Public Property Importe As Decimal
    Public Property NumDocumento As String
End Class
