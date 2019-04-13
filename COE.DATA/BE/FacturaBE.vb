
Public Class FacturaBE
    Public Property idfactura() As Int32
    Public Property TipoComprobante As String = "01"
    Public Property t01_fecemision() As String
    Public Property t07_tipdoc_c01() As String
    Public Property t08_numcorrelativo() As String
    Public Property t09_numdoc() As String
    Public Property t09_tipdoc_c06() As String
    Public Property t10_nomadquiriente() As String
    Public Property t18_tipmonto_c14() As String
    Public Property t18_totalvalorventagravadas() As String
    Public Property t19_tipmonto_c14() As String
    Public Property t19_totalvalorventainafectas() As String
    Public Property t20_tipmonto_c14() As String
    Public Property t20_totalvalorventaexonerada() As String
    Public Property t22_totaligv() As String
    Public Property t22_subtotaligv() As String
    Public Property t22_tiptributo_c05() As String
    Public Property t22_nomtributo_c05() As String
    Public Property t22_tiptributointernacional_c05() As String
    Public Property t23_totalisc() As String
    Public Property t23_subtotalisc() As String
    Public Property t23_tiptributo_c05() As String
    Public Property t23_nomtributo_c05() As String
    Public Property t23_tiptributointernacional_c05() As String
    Public Property t24_totalotrostributos() As String
    Public Property t24_subtotalotrostributos() As String
    Public Property t24_tiptributo_c05() As String
    Public Property t24_nomtributo_c05() As String
    Public Property t24_tiptributointernacional_c05() As String
    Public Property t25_totalotroscargos() As String
    Public Property t26_tipmonto_c14() As String
    Public Property t26_totaldescuentos() As String
    Public Property t27_totalimporte() As String
    Public Property t28_tipmoneda_c02() As String
    Public Property t29_sernumguia() As String
    Public Property t29_tipdoc_c01() As String
    Public Property t30_numdocrelacionado() As String
    Public Property t30_tipdoc_c12() As String
    Public Property t31_tipleyenda_c15() As String
    Public Property t31_descripcionleyenda() As String
    Public Property t32_tipmonto_c14() As String
    Public Property t32_baseimponiblepercepcion() As String
    Public Property t32_montopercepcion() As String
    Public Property t32_montototalpercepcion() As String
    Public Property t36_versionubl() As String
    Public Property t37_versiondoc() As String
    Public Property t38_tipconcepto_c14() As String
    Public Property t38_serviciotransporte() As String
    Public Property t39_tipconcepto_c15() As String
    Public Property t39_nommatricula() As String
    Public Property t40_tipconcepto_c15() As String
    Public Property t40_especievendida() As String
    Public Property t41_tipconcepto_c15() As String
    Public Property t41_lugardescarga() As String
    Public Property t42_tipconcepto_c15() As String
    Public Property t42_fecdescarga() As String
    Public Property t43_tipconcepto_c15() As String
    Public Property t43_rnumregistro() As String
    Public Property t44_tipconcepto_c15() As String
    Public Property t44_configuracionvehicular() As String
    Public Property t45_tipconcepto_c15() As String
    Public Property t45_puntoorigen() As String
    Public Property t46_tipconcepto_c15() As String
    Public Property t46_puntodestino() As String
    Public Property t47_tipconcepto_c15() As String
    Public Property t47_descripcionvalorreferencial() As String
    Public Property t47_valorreferencial() As String
    Public Property t48_tipconcepto_c15() As String
    Public Property t48_fecconsumo() As String
    Public Property t49_tipmonto_c14() As String
    Public Property t49_totalvalorventaoperacionesgratuitas() As String
    Public Property t50_descuentosglobales() As String
    Public Property t51_tipoperacion_c17 As String


    Public Property ValorFirma As String
    Public Property estado() As Int32
    Public Property fechahorasistemaexterno() As DateTime
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

    ''Se crea las nuevas propiedades
    Public Property CodTipoOperacion As String
    Public Property HoraEmision As String
    Public Property TipoMoneda As String
    Public Property NumOrdenCompra As String


    ''Se crea despues de la entrega
    Public Property NombreTipoDocumentoHuesped As String
    Public Property TipoDocumentoHuesped As String
    Public Property PaisPasaporte As String
    Public Property NombreHuesped As String

    Public Property SunatResidencia As String

    Public Property fFechaIngresoPais As Date

    Public Property fLlegadaReserva As Date

    Public Property fSalidaReserva As Date

    Public Property Permanencia As String


    Property Detalle As New List(Of FacturaItem)
    Property Anticipos As New List(Of FacturaAnticipo)

End Class

Public Class FacturaItem
    Public Property idfacturadetalle() As Int32
    Public Property idfactura() As Int32
    Public Property t11_tipunidadmedida_c03() As String
    Public Property t12_cantidad() As String
    Public Property t13_descripcion() As String
    Public Property t14_valorunitario() As String
    Public Property t15_preciounitario() As String
    Public Property t15_tipprecio_c16() As String
    Public Property t16_totalitem() As String
    Public Property t16_subtotalitem() As String
    Public Property t16_tipafectacion_c07() As String
    Public Property t16_tiptributo_c05() As String
    Public Property t16_nomtributo_c05() As String
    Public Property t16_tiptributointernacional_c05() As String
    Public Property t17_totalitem() As String
    Public Property t17_subtotalitem() As String
    Public Property t17_tipsistema_c08() As String
    Public Property t17_tiptributo_c05() As String
    Public Property t17_nomtributo_c05() As String
    Public Property t17_tiptributointernacional_c05() As String
    Public Property t21_valorventaitem() As String
    Public Property t33_numordenitem() As String
    Public Property t34_codproducto() As String
    Public Property t35_valorreferencialunitarioitem() As String
    Public Property t35_tipreferencia_c16() As String
    Public Property t51_tipdescuentoitem() As String
    Public Property t51_descuentoitem() As String
End Class

Public Class FacturaAnticipo
    Public Property IDAnticipo As Int32
    Public Property IDComprobante As Int32
    Public Property Referencia As String
    Public Property TipoComprobante As String
    Public Property Importe As Decimal
    Public Property NumDocumento As String

    'Se crea nuevos campos
    Public Property FechaAnticipo As String
    Public Property HoraAnticipo As String
End Class