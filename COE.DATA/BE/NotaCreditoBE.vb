
Public Class NotaCreditoBE

    Public Property idnotacredito() As Int32
    Public Property TipoComprobante As String = "07"
    Public Property idcomprobante() As Int32
    Public Property t01_fecemision() As String
    Public Property t07_sernumdocafectado() As String
    Public Property t07_tipnotacredito_c09() As String
    Public Property t08_numcorrelativo() As String
    Public Property t09_numdocadquiriente() As String
    Public Property t09_tipdocadquiriente_c06() As String
    Public Property t10_nomadquiriente() As String
    Public Property t11_motivo() As String
    Public Property t20_tipmonto_c14() As String
    Public Property t20_totalvalorventasgravadas() As String
    Public Property t21_tipmonto_c14() As String
    Public Property t21_totalvalorventainafectas() As String
    Public Property t22_tipmonto_c14() As String
    Public Property t22_totalvalorventaexonerada() As String
    Public Property t24_totaligv() As String
    Public Property t24_subtotaligv() As String
    Public Property t24_tiptributo_c05() As String
    Public Property t24_nomtributo_c05() As String
    Public Property t24_tiptributointernacional_c05() As String
    Public Property t25_totalisc() As String
    Public Property t25_subtotalisc() As String
    Public Property t25_tiptributo_c05() As String
    Public Property t25_nomtributo_c05() As String
    Public Property t25_tiptributointernacional_c05() As String
    Public Property t26_totalotrostributos() As String
    Public Property t26_subtotalotrostributos() As String
    Public Property t26_tiptributo_c05() As String
    Public Property t26_nomtributo_c05() As String
    Public Property t26_tiptributointernacional_c05() As String
    Public Property t27_totalotroscargos() As String
    Public Property t28_tipmonto_c14() As String
    Public Property t28_totaldescuentos() As String
    Public Property t29_totalimporte() As String
    Public Property t30_tipomoneda_c02() As String
    Public Property t31_sernumdocmodifica() As String
    Public Property t32_tipdoc_c01() As String
    Public Property t33_sernumguia() As String
    Public Property t33_tipdoc_c01() As String
    Public Property t33_numdocrelacionado() As String
    Public Property t33_tipdoc_c12() As String
    Public Property t36_versionubl() As String
    Public Property t37_versiondoc() As String
    Public Property ValorFirma As String
    Public Property estado As Int32
    Public Property fechahorasistemaexterno As DateTime
    Public Property fechahorasunat As Nullable(Of DateTime)
    Public Property fechahorarespuestasunat() As DateTime
    Public Property RutaComprobanteXML As String
    Public Property RutaComprobanteZIP As String
    Public Property RutaComprobantePDF As String
    Public Property RutaRespuestaSunatXML As String
    Public Property RutaRespuestaSunatZIP As String
    Public Property DireccionAdquiriente As String
    Public Property EmailAdquiriente As String
    Public Property Observacion As String
    Public Property CodigoRespuesta As String
    Public Property idusuario() As Int32
    Public Property fecharegistro() As DateTime
    Public Property SerieComprobante As String
    Public Property NumeroComprobante As Int32

    'Nuevos propiedades
    Public Property descripcionleyenda As String
    Public Property HoraEmision As String

    Public Property Detalle As List(Of NotaCreditoItem)
End Class

Public Class NotaCreditoItem

    Public Property idnotacreditodetalle() As Int32
    Public Property idnotacredito() As Int32
    Public Property t12_tipunidadmedida_c03() As String
    Public Property t13_cantidad() As String
    Public Property t14_codproducto() As String
    Public Property t15_descripcion() As String
    Public Property t16_valorunitario() As String
    Public Property t17_preciounitario() As String
    Public Property t17_tipprecio_c16() As String
    Public Property t18_totalitem() As String
    Public Property t18_subtotalitem() As String
    Public Property t18_tipafectacion_c07() As String
    Public Property t18_tiptributo_c05() As String
    Public Property t18_nomtributo() As String
    Public Property t18_tiptributointernacional() As String
    Public Property t19_totalitem() As String
    Public Property t19_subtotalitem() As String
    Public Property t19_tipafectacion_c07() As String
    Public Property t19_tiptributo_c05() As String
    Public Property t19_nomtributo() As String
    Public Property t19_tiptributointernacional() As String
    Public Property t23_valorventaitem() As String
    Public Property t34_numordenitem() As String
    Public Property t35_valorreferenciaunitario() As String
    Public Property t35_tipprecio_c16() As String

    'Nuevos
    Public Property t53_porcentajeISC As String
    Public Property t19_tipsistema_c08 As String

End Class
