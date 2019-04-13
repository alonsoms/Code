#Region "Imports"
Imports COE.FRAMEWORK
Imports System.IO
Imports System.IO.Compression
Imports System
Imports System.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Text.RegularExpressions

#End Region

Public Enum eEstadoBoleta
    Anulado = 0
    Activo = 1
End Enum

Public Class BoletaVentaDAO
    Public Property IDBoleta As Int32
    Public Property BE As New BoletaBE

    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim EmisorBE As New EmisorBE
    Dim BoletaBE As New BoletaBE

    Public Function CreateXML(IDBoleta As Int32) As String
        'Se carga los datos el emisor
        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"

        'Se obtiene la entidad
        BoletaBE = Me.GetByID(IDBoleta)

        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-" & BoletaBE.t06_tipdoc_c01 & "-" & BoletaBE.t07_numcorrelativo

        'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
        'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'Se crea el nodo raiz
        writer.WriteStartElement("Invoice")
        writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:gdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
        writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")

        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteStartElement("sac:AdditionalInformation")

        'Revisar: Catálogo No. 14: Códigos - Otros conceptos tributarios
        'Tag Catálogo No. 14 - 1001 Total valor de venta - operaciones gravadas
        writer.WriteStartElement("sac:AdditionalMonetaryTotal")
        writer.WriteElementString("cbc:ID", BoletaBE.t15_tipmonto_c14)
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t15_totalvalorventagravadas)
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Tag Catálogo No. 14 - 1002 Total valor de venta - operaciones inafectas
        writer.WriteStartElement("sac:AdditionalMonetaryTotal")
        writer.WriteElementString("cbc:ID", BoletaBE.t16_tipmonto_c14)
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t16_totalvalorventainafectas)
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Tag Catálogo No. 14 -  1003 Total valor de venta - operaciones exoneradas
        writer.WriteStartElement("sac:AdditionalMonetaryTotal")
        writer.WriteElementString("cbc:ID", BoletaBE.t17_tipmonto_c14)
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t17_totalvalorventaexonerada)
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Tag Catálogo No. 14 -  1004 Total valor de venta – Operaciones gratuitas No se tiene este campo
        writer.WriteStartElement("sac:AdditionalMonetaryTotal")
        writer.WriteElementString("cbc:ID", BoletaBE.t40_tipmonto_c14)
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t40_totalvalorventaoperacionesgratuitas)
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Tag Catálogo No. 14 -  2001 Percepciones. Solo se crea el tag si hay percepcion
        If BoletaBE.t34_montopercepcion > 0 Then
            writer.WriteStartElement("sac:AdditionalMonetaryTotal")
            writer.WriteElementString("cbc:ID", BoletaBE.t34_tipmonto_c14)
            writer.WriteStartElement("cbc:PayableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(BoletaBE.t34_montopercepcion)
            writer.WriteEndElement()
            writer.WriteStartElement("sac:TotalAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(BoletaBE.t34_montototalpercepcion)
            writer.WriteEndElement()
            writer.WriteEndElement()
        End If
        'Tag Catálogo No. 14 -  2005 Total descuentos
        writer.WriteStartElement("sac:AdditionalMonetaryTotal")
        writer.WriteElementString("cbc:ID", BoletaBE.t22_tipmonto_c14)
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t22_totaldescuentos)
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Tag: Catálogo No. 15: Códigos - Elementos adicionales en la Factura Electrónica y/o Boleta de Venta Electrónica 
        '1000 Monto en Letras
        writer.WriteStartElement("sac:AdditionalProperty")
        writer.WriteElementString("cbc:ID", BoletaBE.t26_tipleyenda_c15)
        writer.WriteElementString("cbc:Value", BoletaBE.t26_descripcionleyenda)
        writer.WriteEndElement()

        'Catalogo 17 para indicar tipos de operaciones. Este bloque es nuevo y no estaba antes, aparecio para los anticipos
        writer.WriteStartElement("sac:SUNATTransaction")
        writer.WriteElementString("cbc:ID", BoletaBE.t42_tipoperacion_c17)
        writer.WriteEndElement()

        'Tag es para las percepciones. Revisar este caso 46 se envio y fue aceptado. Estos campos no aparecen en la tabla
        'writer.WriteStartElement("sac:AdditionalProperty")
        'writer.WriteElementString("cbc:ID", "2000")
        'writer.WriteElementString("cbc:Value", "COMPROBANTE DE PERCEPCION")
        'writer.WriteEndElement()

        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Se define este tag es para guardar la firma digital
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Bloque dos
        'La version del UBLVersionID es 2.0
        writer.WriteElementString("cbc:UBLVersionID", BoletaBE.t37_versionubl)
        'La version del CustomizationID es 1.0
        writer.WriteElementString("cbc:CustomizationID", BoletaBE.t38_versiondoc)
        writer.WriteElementString("cbc:ID", BoletaBE.t07_numcorrelativo)
        writer.WriteElementString("cbc:IssueDate", BoletaBE.t01_fecemision)
        writer.WriteElementString("cbc:InvoiceTypeCode", BoletaBE.t06_tipdoc_c01)
        writer.WriteElementString("cbc:DocumentCurrencyCode", BoletaBE.t24_tipmoneda_c02)

        'bloque tres Informacion de emisosr
        writer.WriteStartElement("cac:Signature")
        writer.WriteElementString("cbc:ID", "IDSignSP")
        writer.WriteStartElement("cac:SignatoryParty")
        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteElementString("cbc:ID", EmisorBE.NumeroRUC)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:DigitalSignatureAttachment")
        writer.WriteStartElement("cac:ExternalReference")
        writer.WriteElementString("cbc:URI", "#SignatureSP")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Bloque cuatro informacion emisor
        writer.WriteStartElement("cac:AccountingSupplierParty")
        writer.WriteElementString("cbc:CustomerAssignedAccountID", EmisorBE.NumeroRUC)
        writer.WriteElementString("cbc:AdditionalAccountID", "6")
        writer.WriteStartElement("cac:Party")

        'Se agrega tag CDATA. Se usa el nombre comercial del laboratorio, siempre que en el registro del ruc este declarado
        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(EmisorBE.NombreComercial)
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PostalAddress")
        writer.WriteElementString("cbc:ID", EmisorBE.CodigoUbigeo)
        writer.WriteElementString("cbc:StreetName", EmisorBE.NombreDireccion)
        writer.WriteElementString("cbc:CitySubdivisionName", EmisorBE.NombreUrbanizacion)
        writer.WriteElementString("cbc:CityName", EmisorBE.NombreDepartamento)
        writer.WriteElementString("cbc:CountrySubentity", EmisorBE.NombreProvincia)
        writer.WriteElementString("cbc:District", EmisorBE.NombreDistrito)

        writer.WriteStartElement("cac:Country")
        writer.WriteElementString("cbc:IdentificationCode", "PE")
        writer.WriteEndElement()
        writer.WriteEndElement()
        'TagNodoAtributoValorValor(writer, "cac:Country", "currencyID", FacturaBE.tipomoneda, FacturaBE.sumatoriaigvtotal)

        'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(EmisorBE.RazonSocial)
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Bloque cinco informacion de adquiriente
        writer.WriteStartElement("cac:AccountingCustomerParty")
        writer.WriteElementString("cbc:CustomerAssignedAccountID", BoletaBE.t08_numdoc)
        writer.WriteElementString("cbc:AdditionalAccountID", BoletaBE.t08_tipdoc_c06)
        writer.WriteStartElement("cac:Party")
        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(BoletaBE.t09_nomadquiriente) 'Se agrega tag CDATA
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()


        'Bloque nuevo para los anticipos. Solo se crea cuando existe anticipos. Catalogo 17 04=Venta Interna Anticipos
        Dim TotalAnticipos As Decimal = 0
        If Not BoletaBE.Anticipos Is Nothing Then
            For Each Anticipo As BoletaAnticipo In BoletaBE.Anticipos
                writer.WriteStartElement("cac:PrepaidPayment")
                Tools.TagNodoAtributoValorValor(writer, "cbc:ID", "schemeID", Anticipo.TipoComprobante, Anticipo.Referencia)
                Tools.TagNodoAtributoValorValor(writer, "cbc:PaidAmount", "currencyID", "PEN", Anticipo.Importe)
                Tools.TagNodoAtributoValorValor(writer, "cbc:InstructionID ", "schemeID", "6", Anticipo.NumDocumento)
                writer.WriteEndElement()
                TotalAnticipos += Anticipo.Importe
            Next
        End If

        'bloque seis Sumatoria de Impuestos Falta revisar los impuestos
        writer.WriteStartElement("cac:TaxTotal")
        Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t18_totaligv)

        writer.WriteStartElement("cac:TaxSubtotal")
        Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t18_subtotaligv)

        writer.WriteStartElement("cac:TaxCategory")
        writer.WriteStartElement("cac:TaxScheme")
        writer.WriteElementString("cbc:ID", BoletaBE.t18_tiptributo_c05)

        writer.WriteElementString("cbc:Name", BoletaBE.t18_nomtributo_c05)
        writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t18_tiptributointernacional_c05)
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()


        'Bloque: Otros impuestos: RC - Recargo por Consumo para Restaurantes y Hoteles
        If BoletaBE.t20_totalotrostributos > 0 Then
            writer.WriteStartElement("cac:TaxTotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t20_totalotrostributos)

            writer.WriteStartElement("cac:TaxSubtotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t20_totalotrostributos)

            writer.WriteStartElement("cac:TaxCategory")
            writer.WriteStartElement("cac:TaxScheme")
            writer.WriteElementString("cbc:ID", BoletaBE.t20_tiptributo_c05)

            writer.WriteElementString("cbc:Name", BoletaBE.t20_nomtributo_c05)
            writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t20_tiptributointernacional_c05)
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
        End If

        'bloque siete
        writer.WriteStartElement("cac:LegalMonetaryTotal")

        'Tag nuevo revisar con los otros. Este es el descuento global
        Tools.TagNodoAtributoValorValor(writer, "cbc:AllowanceTotalAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t41_descuentosglobales)

        'Se establece tag para los anticipos
        If TotalAnticipos > 0 Then
            Tools.TagNodoAtributoValorValor(writer, "cbc:PrepaidAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, TotalAnticipos)
        End If

        Tools.TagNodoAtributoValorValor(writer, "cbc:PayableAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, BoletaBE.t23_totalimporte)
        writer.WriteEndElement()

        'bloque ocho: Este bloque tiene los items de la boleta de venta
        For Each Item In BoletaBE.Detalle

            writer.WriteStartElement("cac:InvoiceLine")
            writer.WriteElementString("cbc:ID", Item.t29_numordenitem)
            Tools.TagNodoAtributoValorValor(writer, "cbc:InvoicedQuantity", "unitCode", Item.t11_tipunidadmedida_c03, Item.t12_cantidad)

            'Revisar los otros casos se cambio el tag
            'TagNodoAtributoValorValor(writer, "cbc:LineExtensionAmount", "currencyID", FacturaBE.t28_tipmoneda_c02, Item.t14_valorunitario)
            Tools.TagNodoAtributoValorValor(writer, "cbc:LineExtensionAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t32_valorventaitem)

            writer.WriteStartElement("cac:PricingReference")
            writer.WriteStartElement("cac:AlternativeConditionPrice")
            Tools.TagNodoAtributoValorValor(writer, "cbc:PriceAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t14_preciounitario)
            writer.WriteElementString("cbc:PriceTypeCode", Item.t14_tipprecio_c16)
            writer.WriteEndElement()

            'Este tag es para los gratis se hizo para el caso 23
            'Se comenta porque no usamos este tag 
            If Item.t33_tipreferencia_c16 = "02" And Item.t33_valorreferencialunitarioitem > 0 Then
                writer.WriteStartElement("cac:AlternativeConditionPrice")
                Tools.TagNodoAtributoValorValor(writer, "cbc:PriceAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t33_valorreferencialunitarioitem)
                writer.WriteElementString("cbc:PriceTypeCode", Item.t33_tipreferencia_c16)
                writer.WriteEndElement()
            End If
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxTotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t27_totalitem)
            writer.WriteStartElement("cac:TaxSubtotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t27_subtotalitem)
            writer.WriteStartElement("cac:TaxCategory")
            writer.WriteElementString("cbc:TaxExemptionReasonCode", Item.t27_tipafectacion_c07)

            writer.WriteStartElement("cac:TaxScheme")
            writer.WriteElementString("cbc:ID", Item.t27_tiptributo_c05)
            writer.WriteElementString("cbc:Name", Item.t27_nomtributo_c05.ToString.Trim)
            writer.WriteElementString("cbc:TaxTypeCode", Item.t27_tiptributointernacional_c05)
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()

            writer.WriteStartElement("cac:Item")
            writer.WriteStartElement("cbc:Description")
            writer.WriteCData(Item.t13_descripcion)
            writer.WriteEndElement()
            writer.WriteStartElement("cac:SellersItemIdentification")

            '' writer.WriteElementString("cbc:ID", Item.t30_codproducto)
            writer.WriteStartElement("cbc:ID")
            writer.WriteCData(Item.t30_codproducto)
            writer.WriteEndElement()

            writer.WriteEndElement()
            writer.WriteEndElement()

            writer.WriteStartElement("cac:Price")
            Tools.TagNodoAtributoValorValor(writer, "cbc:PriceAmount", "currencyID", BoletaBE.t24_tipmoneda_c02, Item.t31_valorunitario)
            writer.WriteEndElement()
            writer.WriteEndElement()

        Next

        'Se finaliza el Invoice
        writer.WriteEndElement()

        'Se cierra el documento
        writer.WriteEndDocument()
        writer.Close()
        writer.Dispose()

        'Se usa para reemplazar la cadena de minusculas a mayusculas
        Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
        Data = Data.Replace("iso-8859-1", "ISO-8859-1")
        File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)

        'Se guarda la ruta del archivo xml
        SaveRutaArchivoXML(IDBoleta, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se retorna el nombre de archivo
        Return RutaArchivo & NombreArchivo & ExtensionArchivoXML
    End Function
    'Public Sub CreateFileXML21(IDBoleta As Int32)
    '    'Se carga los datos el emisor
    '    EmisorBE = EmisorDAO.GetByID(1)

    '    'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
    '    'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
    '    Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
    '    Dim NombreArchivo As String = String.Empty
    '    Dim ExtensionArchivoXML As String = ".xml"
    '    'Se obtiene la entidad
    '    BoletaBE = Me.GetByID(IDBoleta)

    '    'Se obtiene el nombre del archivo XML
    '    NombreArchivo = EmisorBE.NumeroRUC
    '    NombreArchivo &= "-" & BoletaBE.t06_tipdoc_c01 & "-" & BoletaBE.t07_numcorrelativo

    '    'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
    '    'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
    '    Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
    '    writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
    '    writer.Formatting = Formatting.Indented
    '    writer.Indentation = 0

    '    'Origen
    '    writer.WriteStartElement("Invoice")
    '    writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
    '    writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
    '    writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
    '    writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
    '    writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
    '    writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
    '    writer.WriteAttributeString("xmlns:gdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
    '    writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
    '    writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
    '    writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")


    '    writer.WriteStartElement("ext:UBLExtensions")
    '    writer.WriteStartElement("ext:UBLExtension")
    '    writer.WriteStartElement("ext:ExtensionContent")

    '    'La version del UBLVersionID es 2.1
    '    'Parte2
    '    writer.WriteElementString("cbc:UBLVersionID", "2.1") 'BoletaBE.t37_versionubl)
    '    'Parte2

    '    'Parte3
    '    writer.WriteElementString("cbc:CustomizationID", "2.0") 'BoletaBE.t38_versiondoc)
    '    'Parte3

    '    'Parte4
    '    writer.WriteStartElement("cbc:ProfileID")
    '    writer.WriteAttributeString("schemeName ", "SUNAT:Identificador de Tipo de Operación")
    '    writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
    '    writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo17")
    '    writer.WriteString("0101") 'Agregar Nuevo Campo si en algun momento se considera agregar mas tipos de operación se debe agregar un campo en FACTURA
    '    writer.WriteEndElement()
    '    'Parte4

    '    'Parte5
    '    writer.WriteElementString("cbc:ID", BoletaBE.t07_numcorrelativo)
    '    'Parte5
    '    'Parte6
    '    writer.WriteElementString("cbc:IssueDate", BoletaBE.t01_fecemision)
    '    'Parte6
    '    'Parte7
    '    writer.WriteElementString("cbc:IssueTime", BoletaBE.t01_horaemision)  'hh-mm-ss.0z agregar dato de hora
    '    'Parte7

    '    'Parte9 cbc:InvoiceTypeCode
    '    writer.WriteStartElement("cbc:InvoiceTypeCode")
    '    writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
    '    writer.WriteAttributeString("listName", "SUNAT:Identificador de Tipo de Documento")
    '    writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01")
    '    writer.WriteString(BoletaBE.t08_tipdoc_c06)
    '    writer.WriteEndElement()
    '    'Parte9 cbc:InvoiceTypeCode

    '    'Parte10 cbc:Note
    '    writer.WriteStartElement("cbc:Note")
    '    writer.WriteAttributeString("languageLocaleID", "1000")
    '    writer.WriteString(BoletaBE.t26_descripcionleyenda)
    '    writer.WriteEndElement()
    '    'Parte10 cbc:Note

    '    'Parte11 cbc:DocumentCurrencyCode
    '    writer.WriteStartElement("cbc:DocumentCurrencyCode")
    '    writer.WriteAttributeString("listID", "ISO 4217 Alpha")
    '    writer.WriteAttributeString("listName", "Currency")
    '    writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
    '    writer.WriteString(BoletaBE.t24_tipmoneda_c02)
    '    writer.WriteEndElement()
    '    'Parte11 cbc:DocumentCurrencyCode

    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Parte7
    '    writer.WriteStartElement("cac:Signature")
    '    writer.WriteEndElement()
    '    'Parte7

    '    writer.WriteStartElement("cac:AccountingSupplierParty")
    '    writer.WriteStartElement("cac:Party")

    '    'Parte14
    '    writer.WriteStartElement("cac:PartyName")
    '    writer.WriteStartElement("cbc:Name")
    '    writer.WriteCData(EmisorBE.NombreComercial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    'Parte14

    '    writer.WriteStartElement("cac:PartyTaxScheme")

    '    'Parte15
    '    writer.WriteStartElement("cbc:RegistrationName")
    '    writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
    '    writer.WriteEndElement()
    '    'Parte15

    '    'Parte16
    '    writer.WriteStartElement("cbc:CompanyID")
    '    writer.WriteAttributeString("schemeID", "6") 'Nuevo Campo Agregar Catalogo 06 
    '    writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
    '    writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
    '    writer.WriteAttributeString("schemeURI ", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
    '    writer.WriteString(EmisorBE.NumeroRUC)
    '    writer.WriteEndElement()
    '    'Parte16

    '    'Parte17
    '    writer.WriteStartElement("cac:RegistrationAddress")
    '    writer.WriteElementString("cbc:AddressTypeCode", "0000")
    '    writer.WriteEndElement()
    '    'Parte17
    '    writer.WriteEndElement()

    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Parte18
    '    writer.WriteStartElement("cac:AccountingCustomerParty")
    '    writer.WriteStartElement("cac:Party")
    '    writer.WriteStartElement("cac:PartyTaxScheme")

    '    'Parte19
    '    writer.WriteStartElement("cbc:RegistrationName")
    '    writer.WriteCData(BoletaBE.t09_nomadquiriente)
    '    writer.WriteEndElement()
    '    'Parte19

    '    writer.WriteStartElement("cbc:CompanyID")
    '    writer.WriteAttributeString("schemeID", BoletaBE.t08_tipdoc_c06) 'Tener en cuenta que la apliación de venta envie el tipo de documento correcto
    '    writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
    '    writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
    '    writer.WriteAttributeString("schemeURI ", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
    '    writer.WriteString(BoletaBE.t08_numdoc)
    '    writer.WriteEndElement()

    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    'Parte18

    '    'Parte21 - Descuento Global
    '    If Double.Parse(BoletaBE.t41_descuentosglobales) > 0 Then
    '        writer.WriteStartElement("cac:AllowanceCharge")

    '        writer.WriteElementString("cbc:ChargeIndicator", "False")

    '        writer.WriteStartElement("cbc:AllowanceChargeReasonCode", "00")
    '        writer.WriteStartElement("cbc::MultiplierFactorNumeric", "") ' Nuevo Campo % de descuento 

    '        writer.WriteStartElement("cbc:Amount")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '        writer.WriteString(BoletaBE.t41_descuentosglobales)
    '        writer.WriteEndElement()

    '        writer.WriteStartElement("cbc:BaseAmount")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '        writer.WriteString((Double.Parse(BoletaBE.t41_descuentosglobales) + Double.Parse(BoletaBE.t23_totalimporte)).ToString())
    '        writer.WriteEndElement()

    '        writer.WriteEndElement()
    '        writer.WriteEndElement()

    '        writer.WriteEndElement()
    '    End If
    '    'Parte21

    '    writer.WriteStartElement("cac:TaxTotal")
    '    If Double.Parse(BoletaBE.t18_totaligv) + Double.Parse(BoletaBE.t19_totalisc) + Double.Parse(BoletaBE.t20_totalotrostributos) > 0 Then
    '        'Parte22
    '        writer.WriteStartElement("cbc:TaxAmount")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '        writer.WriteString((Double.Parse(BoletaBE.t18_totaligv) + Double.Parse(BoletaBE.t19_totalisc) + Double.Parse(BoletaBE.t20_totalotrostributos)).ToString())
    '        writer.WriteEndElement()
    '        'Parte22

    '        'Parte23
    '        Dim SumatoriaTotal As Double
    '        Dim SumatoriaImpuesto As Double
    '        For Each item As BoletaItem In BoletaBE.Detalle
    '            If item.t27_tipafectacion_c07 = "10" Then ' Or item.t16_tipafectacion_c07 = "11" Or item.t16_tipafectacion_c07 = "12" Or item.t16_tipafectacion_c07 = "13" Or item.t16_tipafectacion_c07 = "14" Or item.t16_tipafectacion_c07 = "15" Or item.t16_tipafectacion_c07 = "16" Or item.t16_tipafectacion_c07 = "17" Then
    '                SumatoriaTotal = SumatoriaTotal + item.t31_valorunitario
    '                SumatoriaImpuesto = SumatoriaImpuesto + item.t27_totalitem
    '            End If
    '        Next

    '        If SumatoriaImpuesto > 0 Then
    '            writer.WriteStartElement("cac:TaxSubtotal")

    '            writer.WriteStartElement("cbc:TaxableAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaTotal, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaImpuesto, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxCategory")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '            writer.WriteString("S")
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxScheme")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '            writer.WriteAttributeString("schemeAgencyID", "6")
    '            writer.WriteString("1000")
    '            writer.WriteEndElement()

    '            writer.WriteElementString("cbc:Name", BoletaBE.t18_nomtributo_c05)
    '            writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t18_tiptributointernacional_c05)

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte23

    '        'Parte24
    '        Dim SumatoriaTotal2 As Double
    '        Dim SumatoriaImpuesto2 As Double
    '        For Each item As BoletaItem In BoletaBE.Detalle
    '            If item.t27_tipafectacion_c07 = "20" Then
    '                SumatoriaTotal2 = SumatoriaTotal2 + item.t31_valorunitario
    '                SumatoriaImpuesto2 = SumatoriaImpuesto2 + item.t27_totalitem ' nuevo campo modificar el sp lectura para que cambie este dato cuando es EXONERADO
    '            End If
    '        Next

    '        If SumatoriaImpuesto2 > 0 Then
    '            writer.WriteStartElement("cac:TaxSubtotal")

    '            writer.WriteStartElement("cbc:TaxableAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaTotal2, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaImpuesto2, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxCategory")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '            writer.WriteString("E")
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxScheme")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '            writer.WriteAttributeString("schemeAgencyID", "6")
    '            writer.WriteString("9997")
    '            writer.WriteEndElement()

    '            writer.WriteElementString("cbc:Name", "EXONERADO")
    '            writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t18_tiptributointernacional_c05)

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte24

    '        'Parte26
    '        Dim SumatoriaTotal3 As Double
    '        Dim SumatoriaImpuesto3 As Double
    '        For Each item As BoletaItem In BoletaBE.Detalle
    '            If item.t27_tipafectacion_c07 = "30" Then
    '                SumatoriaTotal3 = SumatoriaTotal3 + item.t31_valorunitario
    '                SumatoriaImpuesto3 = SumatoriaImpuesto3 + item.t27_totalitem ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
    '            End If
    '        Next

    '        If SumatoriaImpuesto3 > 0 Then
    '            writer.WriteStartElement("cac:TaxSubtotal")

    '            writer.WriteStartElement("cbc:TaxableAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaTotal3, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaImpuesto3, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxCategory")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '            writer.WriteString("O")
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxScheme")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '            writer.WriteAttributeString("schemeAgencyID", "6")
    '            writer.WriteString("9998")
    '            writer.WriteEndElement()

    '            writer.WriteElementString("cbc:Name", "INAFECTO")
    '            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte26

    '        'Parte25
    '        Dim SumatoriaTotal4 As Double
    '        Dim SumatoriaImpuesto4 As Double
    '        For Each item As BoletaItem In BoletaBE.Detalle
    '            If item.t27_tipafectacion_c07 = "21" Then
    '                SumatoriaTotal4 = SumatoriaTotal4 + item.t31_valorunitario
    '                SumatoriaImpuesto4 = SumatoriaImpuesto4 + item.t27_totalitem ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
    '            End If
    '        Next

    '        If SumatoriaImpuesto4 > 0 Then
    '            writer.WriteStartElement("cac:TaxSubtotal")

    '            writer.WriteStartElement("cbc:TaxableAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaTotal4, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaImpuesto4, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxCategory")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '            writer.WriteString("Z")
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxScheme")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '            writer.WriteAttributeString("schemeAgencyID", "6")
    '            writer.WriteString("9996")
    '            writer.WriteEndElement()

    '            writer.WriteElementString("cbc:Name", "GRATUITO")
    '            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte25

    '        'Parte28
    '        Dim SumatoriaTotal5 As Double
    '        Dim SumatoriaImpuesto5 As Double
    '        For Each item As BoletaItem In BoletaBE.Detalle
    '            If Double.Parse(item.t28_totalitem) > 0 Then
    '                SumatoriaTotal5 = SumatoriaTotal5 + item.t31_valorunitario
    '                SumatoriaImpuesto5 = SumatoriaImpuesto5 + item.t28_totalitem ' nuevo campo modificar el sp lectura para que cambie este dato cuando es ISC
    '            End If
    '        Next
    '        If SumatoriaImpuesto5 > 0 Then
    '            writer.WriteStartElement("cac:TaxSubtotal")

    '            writer.WriteStartElement("cbc:TaxableAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaTotal5, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(Math.Round(SumatoriaImpuesto5, 2).ToString())
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxCategory")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '            writer.WriteString("S")
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cac:TaxScheme")

    '            writer.WriteStartElement("cbc:ID")
    '            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '            writer.WriteAttributeString("schemeAgencyID", "6")
    '            writer.WriteString("2000")
    '            writer.WriteEndElement()

    '            writer.WriteElementString("cbc:Name", "ISC")
    '            writer.WriteElementString("cbc:TaxTypeCode", "EXC")

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte28
    '        'Parte29
    '        'Hay que agregar otros tributos en el detalle 
    '        'Parte29
    '    End If
    '    writer.WriteEndElement()

    '    writer.WriteStartElement("cac:LegalMonetaryTotal")

    '    'Parte30
    '    writer.WriteStartElement("cbc:LineExtensionAmount")
    '    writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '    'writer.WriteString(BoletaBE.t15_totalvalorventagravadas - BoletaBE.t41_descuentosglobales)
    '    writer.WriteString(Math.Round(Double.Parse(BoletaBE.t15_totalvalorventagravadas) - Double.Parse(BoletaBE.t41_descuentosglobales), 2).ToString("0.00"))

    '    writer.WriteEndElement()
    '    'Parte30

    '    'Parte31
    '    writer.WriteStartElement("cbc:TaxInclusiveAmount")
    '    writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '    writer.WriteString(BoletaBE.t23_totalimporte)
    '    writer.WriteEndElement()
    '    'Parte31

    '    'Parte32
    '    writer.WriteStartElement("cbc:AllowanceTotalAmount")
    '    writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '    writer.WriteString(BoletaBE.t41_descuentosglobales)
    '    writer.WriteEndElement()
    '    'Parte32

    '    'Parte33
    '    writer.WriteStartElement("cbc:ChargeTotalAmount")
    '    writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '    writer.WriteString(BoletaBE.t21_totalotroscargos)
    '    writer.WriteEndElement()
    '    'Parte33

    '    'Parte34
    '    writer.WriteStartElement("cbc:PayableAmount")
    '    writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '    'writer.WriteString(BoletaBE.t23_totalimporte + BoletaBE.t41_descuentosglobales + BoletaBE.t21_totalotroscargos) 'tener en cuenta este campo es el importe total de la venta
    '    writer.WriteString(Math.Round(Double.Parse(BoletaBE.t23_totalimporte) + Double.Parse(BoletaBE.t41_descuentosglobales) + Double.Parse(BoletaBE.t21_totalotroscargos), 2).ToString("0.00")) 'tener en cuenta este campo es el importe total de la venta
    '    writer.WriteEndElement()
    '    'Parte34

    '    writer.WriteEndElement()

    '    Dim orden As Integer = 1
    '    For Each item As BoletaItem In BoletaBE.Detalle
    '        'Parte35
    '        writer.WriteStartElement("cac:InvoiceLine")
    '        writer.WriteElementString("cbc:ID", orden.ToString())

    '        'Parte36
    '        writer.WriteStartElement("cbc:InvoicedQuantity")
    '        writer.WriteAttributeString("unitCode", "NIU") 'Nuevo Campo Agregar Catalogo 06
    '        writer.WriteAttributeString("unitCodeListID", "UN/ECE rec 20")
    '        writer.WriteAttributeString("unitCodeListAgencyName", "United Nations Economic Commission for Europe")
    '        writer.WriteString(item.t12_cantidad)
    '        writer.WriteEndElement()
    '        'Parte36

    '        'Parte37
    '        writer.WriteStartElement("cbc:LineExtensionAmount")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02) 'Nuevo Campo Agregar Catalogo 06
    '        writer.WriteString(item.t31_valorunitario)
    '        writer.WriteEndElement()
    '        'Parte37

    '        'Parte38
    '        writer.WriteStartElement("cac:PricingReference")
    '        writer.WriteStartElement("cac:AlternativeConditionPrice")

    '        writer.WriteStartElement("cbc:PriceAmount")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '        writer.WriteString(item.t14_preciounitario)
    '        writer.WriteEndElement()

    '        writer.WriteStartElement("cbc:PriceTypeCode")
    '        writer.WriteAttributeString("listName", "SUNAT:Indicador de Tipo de Precio")
    '        writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
    '        writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16")
    '        writer.WriteString(item.t14_tipprecio_c16) ' nuevo campo agregar si es transferencia gratuita cambiar 01 por 02 
    '        writer.WriteEndElement()

    '        writer.WriteEndElement()
    '        writer.WriteEndElement()
    '        'Parte38

    '        'Parte40 ' nuevo campo agregar verificar que el sistema de ventas no envie datos de descuento al detalle cuando no conrresponda
    '        If Double.Parse(item.t42_descuentoitem) > 0 Then

    '            writer.WriteStartElement("cac:AllowanceCharge")
    '            writer.WriteElementString("cbc:ChargeIndicator", "false")
    '            writer.WriteElementString("cbc:AllowanceChargeReasonCode", "00")

    '            'If Double.Parse(item.PorcentajeDescuento) > 0 Then
    '            '    writer.WriteElementString("cbc:MultiplierFactorNumeric:", item.PorcentajeDescuento) ' Nuevo Campo agregar porcentaje de descuento
    '            'End If

    '            writer.WriteStartElement("cbc:Amount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(item.t42_descuentoitem)
    '            writer.WriteEndElement()

    '            writer.WriteStartElement("cbc:BaseAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(item.t14_preciounitario)
    '            writer.WriteEndElement()

    '            writer.WriteEndElement()
    '        End If
    '        'Parte40

    '        'Parte41
    '        'COE NO TIENE OTROS CARGOS POR DETALLE
    '        'Parte41

    '        If Double.Parse(item.t27_totalitem) > 0 Then

    '            writer.WriteStartElement("cac:TaxTotal")

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(item.t27_totalitem)
    '            writer.WriteEndElement()

    '            'Parte42
    '            If item.t27_tipafectacion_c07 = "10" Then
    '                writer.WriteStartElement("cac:TaxSubtotal")

    '                writer.WriteStartElement("cbc:TaxableAmount")
    '                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '                writer.WriteString(item.t31_valorunitario)
    '                writer.WriteEndElement()

    '                writer.WriteStartElement("cbc:TaxAmount")
    '                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '                writer.WriteString(item.t27_totalitem)
    '                writer.WriteEndElement()

    '                writer.WriteStartElement("cac:TaxCategory")

    '                writer.WriteStartElement("cbc:ID")
    '                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '                writer.WriteString("S")
    '                writer.WriteEndElement()


    '                Dim Porcentaje As String = "18" ' SistemaDAO.GetProcentajeIGVCentral()
    '                writer.WriteElementString("cbc:Percent", IIf(Porcentaje <> "", Porcentaje, 18)) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

    '                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
    '                writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
    '                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
    '                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
    '                writer.WriteString(item.t27_tipafectacion_c07)
    '                writer.WriteEndElement()

    '                writer.WriteStartElement("cac:TaxScheme")

    '                writer.WriteStartElement("cbc:ID")
    '                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
    '                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
    '                writer.WriteString(item.t27_tiptributo_c05)
    '                writer.WriteEndElement()

    '                writer.WriteElementString("cbc:Name", item.t27_nomtributo_c05)
    '                writer.WriteElementString("cbc:TaxTypeCode", item.t27_tiptributointernacional_c05)

    '                writer.WriteEndElement()

    '                writer.WriteEndElement()

    '                writer.WriteEndElement()
    '            End If
    '            'Parte42

    '            writer.WriteEndElement()

    '        End If

    '        If Double.Parse(item.t28_totalitem) > 0 Then

    '            writer.WriteStartElement("cac:TaxTotal")

    '            writer.WriteStartElement("cbc:TaxAmount")
    '            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '            writer.WriteString(item.t28_totalitem)
    '            writer.WriteEndElement()

    '            'Parte43
    '            If Double.Parse(item.t28_totalitem) > 0 Then
    '                writer.WriteStartElement("cac:TaxSubtotal")

    '                writer.WriteStartElement("cbc:TaxableAmount")
    '                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '                writer.WriteString(item.t14_preciounitario)
    '                writer.WriteEndElement()

    '                writer.WriteStartElement("cbc:TaxAmount")
    '                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '                writer.WriteString(item.t28_totalitem)
    '                writer.WriteEndElement()

    '                writer.WriteStartElement("cac:TaxCategory")

    '                writer.WriteStartElement("cbc:ID")
    '                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
    '                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
    '                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
    '                writer.WriteString("S")
    '                writer.WriteEndElement()

    '                Dim Porcentaje As String = "18" ' SistemaDAO.GetProcentajeIGVCentral()
    '                writer.WriteElementString("cbc:Percent", IIf(Porcentaje <> "", Porcentaje, 18)) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

    '                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
    '                writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
    '                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
    '                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
    '                writer.WriteString(item.t27_tipafectacion_c07)
    '                writer.WriteEndElement()

    '                writer.WriteElementString("cbc:TierRange", item.t28_tipsistema_c08)

    '                writer.WriteStartElement("cac:TaxScheme")

    '                writer.WriteElementString("cbc:ID", item.t28_tiptributo_c05)

    '                'writer.WriteStartElement("cbc:ID")
    '                'writer.WriteAttributeString("schemeID", "UN/ECE 5153")
    '                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
    '                'writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
    '                'writer.WriteString(item.t28_tiptributo_c05)
    '                'writer.WriteEndElement()

    '                writer.WriteElementString("cbc:Name", item.t28_nomtributo_c05)
    '                writer.WriteElementString("cbc:TaxTypeCode", item.t28_tiptributointernacional_c05)

    '                writer.WriteEndElement()

    '                writer.WriteEndElement()

    '                writer.WriteEndElement()
    '            End If
    '            'Parte43
    '            writer.WriteEndElement()

    '        End If

    '        'Parte44
    '        writer.WriteStartElement("cac:Item")
    '        writer.WriteStartElement("cbc:Description")
    '        writer.WriteCData(item.t13_descripcion)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
    '        writer.WriteEndElement()

    '        'Parte45
    '        writer.WriteStartElement("cac:SellersItemIdentification")
    '        writer.WriteElementString("cbc:ID", item.t30_codproducto)
    '        writer.WriteEndElement()
    '        'Parte45

    '        'Parte46 opcional no se usa
    '        'writer.WriteStartElement("cac:CommodityClassification")
    '        'writer.WriteStartElement("cbc:ItemClassificationCode")
    '        'writer.WriteAttributeString("listID", "UNSPSC") 'Nuevo Campo Agregar Catalogo 06
    '        'writer.WriteAttributeString("listAgencyName", "GS1 US")
    '        'writer.WriteAttributeString("listName", "Item Classification")
    '        'writer.WriteString(FacturaBE.t09_numdoc)
    '        'writer.WriteEndElement()
    '        'writer.WriteEndElement()
    '        'Parte46

    '        'Parte47
    '        'no hay información adicional
    '        'Parte47

    '        'Parte48
    '        writer.WriteStartElement("cac:Price")
    '        'writer.WriteElementString("cbc:ID", item.t34_codproducto)

    '        writer.WriteStartElement("cbc:PriceAmount ")
    '        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
    '        writer.WriteString(item.t31_valorunitario)
    '        writer.WriteEndElement()

    '        writer.WriteEndElement()
    '        'Parte48

    '        writer.WriteEndElement()
    '        'Parte44



    '        writer.WriteEndElement()
    '        'Parte35

    '        orden = orden + 1
    '    Next

    '    writer.WriteEndElement()
    '    'Origen

    '    'Se cierra el documento
    '    writer.WriteEndDocument()

    '    writer.Close()
    '    writer.Dispose()

    '    'Se usa para reemplazar la cadena de minusculas a mayusculas
    '    Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
    '    Data = Data.Replace("iso-8859-1", "ISO-8859-1")
    '    File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)


    '    'Se guarda la ruta del archivo xml
    '    SaveRutaArchivoXML(IDBoleta, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

    '    'Se firma el comprobante
    '    Dim FirmaXMLDAO As New FirmaXMLDAO
    '    FirmaXMLDAO.Create(EmisorBE, RutaArchivo & NombreArchivo & ExtensionArchivoXML, "03")


    'End Sub
    '


    'Public Function SignatureXML(IDBoleta As Int32) As String

    '    'Se carga los datos el emisor
    '    EmisorBE = EmisorDAO.GetByID(1)

    '    Dim RutaCertificado As String = EmisorBE.RutaCarpetaArchivosCertificados
    '    Dim ClaveCertificado As String = EmisorBE.ClaveCertificado
    '    Dim Result As String = String.Empty

    '    'Se obtiene la entidad
    '    BoletaBE = Me.GetByID(IDBoleta)

    '    Try
    '        Dim local_xmlArchivo As String = BoletaBE.RutaComprobanteXML
    '        Dim local_nombreXML As String = System.IO.Path.GetFileName(local_xmlArchivo)
    '        Dim local_typoDocumento As String = "03"

    '        Dim MiCertificado As X509Certificate2 = New X509Certificate2(RutaCertificado, ClaveCertificado)
    '        Dim xmlDoc As XmlDocument = New XmlDocument()
    '        xmlDoc.PreserveWhitespace = True
    '        xmlDoc.Load(local_xmlArchivo)

    '        Dim signedXml As SignedXml = New SignedXml(xmlDoc)
    '        signedXml.SigningKey = MiCertificado.PrivateKey

    '        Dim KeyInfo As KeyInfo = New KeyInfo()

    '        Dim Reference As Reference = New Reference()
    '        Reference.Uri = ""

    '        Reference.AddTransform(New XmlDsigEnvelopedSignatureTransform())

    '        signedXml.AddReference(Reference)

    '        Dim X509Chain As X509Chain = New X509Chain()
    '        X509Chain.Build(MiCertificado)

    '        Dim local_element As X509ChainElement = X509Chain.ChainElements(0)
    '        Dim x509Data As KeyInfoX509Data = New KeyInfoX509Data(local_element.Certificate)
    '        Dim subjectName As String = local_element.Certificate.Subject

    '        x509Data.AddSubjectName(subjectName)
    '        KeyInfo.AddClause(x509Data)

    '        signedXml.KeyInfo = KeyInfo
    '        signedXml.ComputeSignature()

    '        Dim signature As XmlElement = signedXml.GetXml()
    '        signature.Prefix = "ds"
    '        signedXml.ComputeSignature()

    '        For Each node As XmlNode In signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']")
    '            If node.LocalName = "Signature" Then
    '                Dim newAttribute As XmlAttribute = xmlDoc.CreateAttribute("Id")
    '                newAttribute.Value = "SignatureSP"
    '                node.Attributes.Append(newAttribute)
    '                Exit For
    '            End If
    '        Next node

    '        Dim local_xpath As String = String.Empty
    '        Dim nsMgr As XmlNamespaceManager
    '        nsMgr = New XmlNamespaceManager(xmlDoc.NameTable)
    '        nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
    '        nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2")
    '        nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance")

    '        Select Case local_typoDocumento
    '            Case "01", "03" 'factura / boleta
    '                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
    '                local_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

    '            Case "07" 'nota de credito
    '                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")
    '                local_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

    '            Case "08" 'nota de debito
    '                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2")
    '                local_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

    '            Case "RA"  'Communicacion de baja
    '                nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1")
    '                local_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"

    '            Case "RC" 'Resumen de diario
    '                nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1")
    '                local_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent"
    '        End Select

    '        nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
    '        nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
    '        nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
    '        nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
    '        nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
    '        nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#")

    '        xmlDoc.SelectSingleNode(local_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, True))
    '        xmlDoc.Save(local_xmlArchivo)

    '        Dim nodeList As XmlNodeList = xmlDoc.GetElementsByTagName("ds:Signature")

    '        'el nodo <ds:Signature> debe existir unicamente 1 vez
    '        If nodeList.Count <> 1 Then
    '            Throw New Exception("Se produjo un error en la firma del documento")
    '        End If
    '        signedXml.LoadXml(CType(nodeList(0), XmlElement))

    '        'verificacion de la firma generada
    '        If signedXml.CheckSignature() = False Then
    '            Throw New Exception("Se produjo un error en la firma del documento")
    '        End If

    '        'Se recupera el valor de la firma
    '        Dim ValorFirma As String = String.Empty
    '        Dim Valor As String
    '        'Se obtiene el archivo XML
    '        Dim ReaderXML As XmlTextReader = New XmlTextReader(local_xmlArchivo)

    '        'Se obtiene el valor de la firma
    '        While (ReaderXML.Read())

    '            'Se lee la raiz de cada nodo
    '            Valor = ReaderXML.Name

    '            Select Case ReaderXML.NodeType
    '                Case XmlNodeType.Element

    '                    'Se obtiene el hash DigisValue, del archivo XML Firmado
    '                    If ReaderXML.Name = "DigestValue" Then
    '                        ReaderXML.Read()
    '                        ValorFirma = ReaderXML.Value
    '                        Exit While
    '                    End If

    '                    ''Se obtiene el valor de la firma del archivo XML Firmado
    '                    'If ReaderXML.Name = "SignatureValue" Then
    '                    '    ReaderXML.Read()
    '                    '    ValorFirma = ReaderXML.Value
    '                    '    Exit While
    '                    'End If

    '                    'Se obtiene el texto de cada elemento. No Se usa
    '                Case XmlNodeType.Text
    '                    'Console.WriteLine(ReaderXML.Value)

    '                    'Se obtiene el fin del elemento. No se usa
    '                Case XmlNodeType.EndElement
    '                    'Console.Write("</" + ReaderXML.Name)
    '                    'Console.WriteLine(">")
    '            End Select
    '        End While

    '        'Se guarda el valor de la firma
    '        Me.SaveValorFirma(IDBoleta, ValorFirma)

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    '    Return Result
    'End Function

    Public Sub CreateFileXML21(IDBoleta As Int32)
        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"
        'Se obtiene la entidad
        BoletaBE = Me.GetByID(IDBoleta)
        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-" & BoletaBE.t06_tipdoc_c01 & "-" & BoletaBE.t07_numcorrelativo

        'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
        'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'Origen
        writer.WriteStartElement("Invoice")
        writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:gdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
        'writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")


        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        'La version del UBLVersionID es 2.1
        'Parte2
        writer.WriteElementString("cbc:UBLVersionID", BoletaBE.t37_versionubl)
        'Parte2

        'Parte3
        writer.WriteElementString("cbc:CustomizationID", BoletaBE.t38_versiondoc)
        'Parte3

        'Parte4
        writer.WriteStartElement("cbc:ProfileID")
        writer.WriteAttributeString("schemeName ", "SUNAT:Identificador de Tipo de Operación")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo17")
        writer.WriteString("0101") 'Agregar Nuevo Campo si en algun momento se considera agregar mas tipos de operación se debe agregar un campo en FACTURA
        writer.WriteEndElement()
        'Parte4

        'Parte5
        writer.WriteElementString("cbc:ID", BoletaBE.t07_numcorrelativo)
        'Parte5
        'Parte6
        writer.WriteElementString("cbc:IssueDate", BoletaBE.t01_fecemision)
        'Parte6
        'Parte7
        writer.WriteElementString("cbc:IssueTime", BoletaBE.t01_horaemision) 'hh-mm-ss.0z agregar dato de hora
        'Parte7

        'Parte9 cbc:InvoiceTypeCode
        writer.WriteStartElement("cbc:InvoiceTypeCode")
        writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("listName", "Tipo de Documento")
        writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01")
        writer.WriteAttributeString("listID", BoletaBE.t42_tipoperacion_c17 + IIf(BoletaBE.t42_tipoperacion_c17 = "01", "01", "02"))
        writer.WriteAttributeString("name", "Tipo de Operacion")
        writer.WriteAttributeString("listSchemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51")
        writer.WriteString(BoletaBE.t06_tipdoc_c01)
        writer.WriteEndElement()
        'Parte9 cbc:InvoiceTypeCode

        'Parte10 cbc:Note
        writer.WriteStartElement("cbc:Note")
        writer.WriteAttributeString("languageLocaleID", "1000")
        writer.WriteString(BoletaBE.t26_descripcionleyenda)
        writer.WriteEndElement()
        'Parte10 cbc:Note

        'Parte11 cbc:DocumentCurrencyCode
        writer.WriteStartElement("cbc:DocumentCurrencyCode")
        writer.WriteAttributeString("listID", "ISO 4217 Alpha")
        writer.WriteAttributeString("listName", "Currency")
        writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        writer.WriteString(BoletaBE.t24_tipmoneda_c02)
        writer.WriteEndElement()
        'Parte11 cbc:DocumentCurrencyCode

        writer.WriteStartElement("cac:Signature")
        writer.WriteElementString("cbc:ID", "IDSignSP")
        writer.WriteStartElement("cac:SignatoryParty")
        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteElementString("cbc:ID", EmisorBE.NumeroRUC)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:DigitalSignatureAttachment")
        writer.WriteStartElement("cac:ExternalReference")
        writer.WriteElementString("cbc:URI", "#SignatureSP")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte7

        writer.WriteStartElement("cac:AccountingSupplierParty")
        writer.WriteStartElement("cac:Party")

        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", "6")
        writer.WriteAttributeString("schemeName", "Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte14
        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(EmisorBE.NombreComercial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte14

        writer.WriteStartElement("cac:PartyTaxScheme")

        'Parte15
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        'Parte15

        'Parte16
        writer.WriteStartElement("cbc:CompanyID")
        writer.WriteAttributeString("schemeID", "6") 'Nuevo Campo Agregar Catalogo 06 
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI ", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)
        writer.WriteEndElement()
        'Parte16

        writer.WriteStartElement("cac:TaxScheme")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", "6")
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte17
        writer.WriteEndElement()
        'aquiaqui
        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()

        writer.WriteStartElement("cac:RegistrationAddress")

        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeName", "Ubigeos")
        writer.WriteAttributeString("schemeAgencyName", "PE:INEI")
        writer.WriteString(EmisorBE.CodigoUbigeo)
        writer.WriteEndElement()

        writer.WriteStartElement("cbc:AddressTypeCode")
        writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("listName", "Establecimientos anexos")
        writer.WriteString("0000")
        writer.WriteEndElement()
        writer.WriteElementString("cbc:CityName", EmisorBE.NombreProvincia)
        writer.WriteElementString("cbc:CountrySubentity", EmisorBE.NombreDepartamento)
        writer.WriteElementString("cbc:District", EmisorBE.NombreDistrito)

        writer.WriteStartElement("cac:AddressLine")
        writer.WriteElementString("cbc:Line", EmisorBE.NombreDireccion)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:Country")
        writer.WriteStartElement("cbc:IdentificationCode")
        writer.WriteAttributeString("listID", "ISO 3166-1")
        writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        writer.WriteAttributeString("listName", "Country")
        writer.WriteString("PE")
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteEndElement()

        writer.WriteEndElement()

        writer.WriteEndElement()
        writer.WriteEndElement()

        'Parte18
        writer.WriteStartElement("cac:AccountingCustomerParty")
        writer.WriteStartElement("cac:Party")
        'Parte19
        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", IIf(BoletaBE.t08_tipdoc_c06 = "-", "0", BoletaBE.t08_tipdoc_c06))
        writer.WriteAttributeString("schemeName", "Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(BoletaBE.t08_numdoc)
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(BoletaBE.t09_nomadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(BoletaBE.t09_nomadquiriente)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:RegistrationAddress")

        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeName", "Ubigeos")
        writer.WriteAttributeString("schemeAgencyName", "PE:INEI")
        writer.WriteEndElement()

        writer.WriteStartElement("cac:AddressLine")
        writer.WriteElementString("cbc:Line", BoletaBE.DireccionAdquiriente)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:Country")
        writer.WriteStartElement("cbc:IdentificationCode")
        writer.WriteAttributeString("listID", "ISO 3166-1")
        writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        writer.WriteAttributeString("listName", "Country")
        writer.WriteString("PE")
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteEndElement()

        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte18

        'Parte21 - Descuento Global
        If Double.Parse(BoletaBE.t41_descuentosglobales) > 0 Then
            writer.WriteStartElement("cac:AllowanceCharge")

            writer.WriteElementString("cbc:ChargeIndicator", "False")

            writer.WriteElementString("cbc:AllowanceChargeReasonCode", "00")
            writer.WriteElementString("cbc:MultiplierFactorNumeric", "") ' Nuevo Campo % de descuento 

            writer.WriteStartElement("cbc:Amount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(BoletaBE.t41_descuentosglobales)
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:BaseAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t41_descuentosglobales) + Double.Parse(BoletaBE.t23_totalimporte), 2).ToString("0.00"))

            writer.WriteEndElement()

            writer.WriteEndElement()
            'writer.WriteEndElement()

            'writer.WriteEndElement()
        End If
        'Parte21

        writer.WriteStartElement("cac:TaxTotal")
        'If Double.Parse(BoletaBE.t18_totaligv) + Double.Parse(BoletaBE.t19_totalisc) + Double.Parse(BoletaBE.t20_totalotrostributos) > 0 Then
        'Parte22
        writer.WriteStartElement("cbc:TaxAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(String.Format("{0:0.00}", (Decimal.Parse(BoletaBE.t18_totaligv) + Decimal.Parse(BoletaBE.t19_totalisc) + Decimal.Parse(BoletaBE.t20_totalotrostributos))))
        writer.WriteEndElement()
        'Parte22
        'Parte23
        Dim SumatoriaTotal As Double
        Dim SumatoriaImpuesto As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If item.t27_tipafectacion_c07 = "10" Then ' Or item.t16_tipafectacion_c07 = "11" Or item.t16_tipafectacion_c07 = "12" Or item.t16_tipafectacion_c07 = "13" Or item.t16_tipafectacion_c07 = "14" Or item.t16_tipafectacion_c07 = "15" Or item.t16_tipafectacion_c07 = "16" Or item.t16_tipafectacion_c07 = "17" Then
                SumatoriaTotal = SumatoriaTotal + (Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad))
                SumatoriaImpuesto = SumatoriaImpuesto + (Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad))
            End If
        Next

        If SumatoriaImpuesto > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t15_totalvalorventagravadas), 2).ToString("0.00"))
            writer.WriteEndElement()
            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("S")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("1000")
            writer.WriteEndElement()
            writer.WriteElementString("cbc:Name", BoletaBE.t18_nomtributo_c05)
            writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t18_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte23

        'Parte24
        Dim SumatoriaTotal2 As Double
        Dim SumatoriaImpuesto2 As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If item.t27_tipafectacion_c07 = "20" Then
                SumatoriaTotal2 = SumatoriaTotal2 + (Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad))
                SumatoriaImpuesto2 = SumatoriaImpuesto2 + (Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es EXONERADO
            End If
        Next

        If SumatoriaTotal2 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t17_totalvalorventaexonerada), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto2, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("E")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("9997")
            writer.WriteEndElement()

            writer.WriteElementString("cbc:Name", "EXO")
            writer.WriteElementString("cbc:TaxTypeCode", BoletaBE.t18_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte24
        'Parte26
        Dim SumatoriaTotal3 As Double
        Dim SumatoriaImpuesto3 As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If item.t27_tipafectacion_c07 = "30" Then
                SumatoriaTotal3 = SumatoriaTotal3 + Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto3 = SumatoriaImpuesto3 + (Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
            End If
        Next

        If SumatoriaTotal3 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t16_totalvalorventainafectas), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto3, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("O")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("9998")
            writer.WriteEndElement()

            writer.WriteElementString("cbc:Name", "INA")
            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte26

        'Parte25
        Dim SumatoriaTotal4 As Double
        Dim SumatoriaImpuesto4 As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If item.t27_tipafectacion_c07 = "21" Then
                SumatoriaTotal4 = SumatoriaTotal4 + Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto4 = SumatoriaImpuesto4 + (Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
            End If
        Next

        If SumatoriaTotal4 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t40_totalvalorventaoperacionesgratuitas), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto4, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("Z")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("9996")
            writer.WriteEndElement()

            writer.WriteElementString("cbc:Name", "GRA")
            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte25

        'Parte28
        Dim SumatoriaTotal5 As Double
        Dim SumatoriaImpuesto5 As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If Double.Parse(item.t28_totalitem) > 0 Then
                SumatoriaTotal5 = SumatoriaTotal5 + Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto5 = SumatoriaImpuesto5 + Double.Parse(item.t28_totalitem) * Double.Parse(item.t12_cantidad) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es ISC
            End If
        Next
        If SumatoriaImpuesto5 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaTotal5, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto5, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("S")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("2000")
            writer.WriteEndElement()

            writer.WriteElementString("cbc:Name", "ISC")
            writer.WriteElementString("cbc:TaxTypeCode", "EXC")

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte28
        Dim SumatoriaTotal6 As Double
        Dim SumatoriaImpuesto6 As Double
        For Each item As BoletaItem In BoletaBE.Detalle
            If item.t27_tipafectacion_c07 = "40" Then
                SumatoriaTotal6 = SumatoriaTotal6 + Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto6 = SumatoriaImpuesto6 + (Double.Parse(item.t28_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
            End If
        Next

        If SumatoriaTotal6 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(BoletaBE.t17_totalvalorventaexonerada), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto6, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxCategory")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString("O")
            writer.WriteEndElement()

            writer.WriteStartElement("cac:TaxScheme")

            writer.WriteStartElement("cbc:ID")
            writer.WriteAttributeString("schemeID", "UN/ECE 5153")
            writer.WriteAttributeString("schemeAgencyID", "6")
            writer.WriteString("9995")
            writer.WriteEndElement()

            writer.WriteElementString("cbc:Name", "EXP")
            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        writer.WriteEndElement()

        writer.WriteStartElement("cac:LegalMonetaryTotal")

        'Parte30
        writer.WriteStartElement("cbc:LineExtensionAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(BoletaBE.t15_totalvalorventagravadas) - Double.Parse(BoletaBE.t41_descuentosglobales), 2).ToString("0.00"))

        writer.WriteEndElement()
        'Parte30

        'Parte31
        writer.WriteStartElement("cbc:TaxInclusiveAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(BoletaBE.t23_totalimporte) - Double.Parse(BoletaBE.t21_totalotroscargos), 2).ToString("0.00"))
        writer.WriteEndElement()
        'Parte31

        'Parte32
        writer.WriteStartElement("cbc:AllowanceTotalAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t41_descuentosglobales)
        writer.WriteEndElement()
        'Parte32

        'Parte33
        writer.WriteStartElement("cbc:ChargeTotalAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(BoletaBE.t21_totalotroscargos)
        writer.WriteEndElement()
        'Parte33

        'Parte34
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(BoletaBE.t23_totalimporte) + Double.Parse(BoletaBE.t41_descuentosglobales), 2).ToString("0.00")) 'tener en cuenta este campo es el importe total de la venta

        writer.WriteEndElement()
        'Parte34

        writer.WriteEndElement()

        Dim orden As Integer = 1
        For Each item As BoletaItem In BoletaBE.Detalle
            'Parte35
            writer.WriteStartElement("cac:InvoiceLine")
            writer.WriteElementString("cbc:ID", orden.ToString())

            'Parte36
            writer.WriteStartElement("cbc:InvoicedQuantity")
            writer.WriteAttributeString("unitCode", "NIU") 'Nuevo Campo Agregar Catalogo 06
            writer.WriteAttributeString("unitCodeListID", "UN/ECE rec 20")
            writer.WriteAttributeString("unitCodeListAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString(item.t12_cantidad)
            writer.WriteEndElement()
            'Parte36

            'Parte37
            writer.WriteStartElement("cbc:LineExtensionAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02) 'Nuevo Campo Agregar Catalogo 06
            writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))

            writer.WriteEndElement()
            'Parte37

            'Parte38
            writer.WriteStartElement("cac:PricingReference")
            writer.WriteStartElement("cac:AlternativeConditionPrice")

            writer.WriteStartElement("cbc:PriceAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(item.t14_preciounitario)
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:PriceTypeCode")
            ' writer.WriteAttributeString("listName", "SUNAT:Indicador de Tipo de Precio")
            writer.WriteAttributeString("listName", "Tipo de Precio")
            writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
            writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16")
            writer.WriteString(item.t14_tipprecio_c16) ' nuevo campo agregar si es transferencia gratuita cambiar 01 por 02 
            writer.WriteEndElement()

            writer.WriteEndElement()
            writer.WriteEndElement()
            'Parte38

            'Parte40 ' nuevo campo agregar verificar que el sistema de ventas no envie datos de descuento al detalle cuando no conrresponda
            If Double.Parse(item.t42_descuentoitem) > 0 Then

                writer.WriteStartElement("cac:AllowanceCharge")
                writer.WriteElementString("cbc:ChargeIndicator", "false")
                writer.WriteElementString("cbc:AllowanceChargeReasonCode", "00")

                'If Double.Parse(item.PorcentajeDescuento) > 0 Then
                '    writer.WriteElementString("cbc:MultiplierFactorNumeric:", item.PorcentajeDescuento) ' Nuevo Campo agregar porcentaje de descuento
                'End If

                writer.WriteStartElement("cbc:Amount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(item.t42_descuentoitem)
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:BaseAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(item.t14_preciounitario)
                writer.WriteEndElement()

                writer.WriteEndElement()
            End If
            'Parte40

            'Parte41
            'COE NO TIENE OTROS CARGOS POR DETALLE
            'Parte41

            'If Double.Parse(item.t27_totalitem) > 0 Then

            writer.WriteStartElement("cac:TaxTotal")

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            'writer.WriteString(item.t27_totalitem)
            writer.WriteString(Math.Round((Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)) + (Double.Parse(item.t28_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
            writer.WriteEndElement()

            'Parte42
            If item.t27_tipafectacion_c07 = "10" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))

                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()


                Dim Porcentaje As String = "0" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", IIf(Porcentaje <> "", Porcentaje, 18)) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                ' writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t27_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString(item.t27_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t27_nomtributo_c05)
                writer.WriteElementString("cbc:TaxTypeCode", item.t27_tiptributointernacional_c05)

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t27_tipafectacion_c07 = "20" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("E")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t27_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9997")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "EXO")
                writer.WriteElementString("cbc:TaxTypeCode", "VAT")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t27_tipafectacion_c07 = "21" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t27_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("Z")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t27_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9996")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "GRA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t27_tipafectacion_c07 = "30" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString("0.00")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("O")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t27_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9998")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "INA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t27_tipafectacion_c07 = "40" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString("0.00")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("O")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                ' writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t27_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9995")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "EXP")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            'Parte42

            If Double.Parse(item.t28_totalitem) > 0 Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t31_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t28_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()

                'Dim Porcentaje As String = SistemaDAO.GetProcentajeISCCentral()
                writer.WriteElementString("cbc:Percent", item.t44_porcentajeISC) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteElementString("cbc:TierRange", item.t28_tipsistema_c08)

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteElementString("cbc:ID", item.t28_tiptributo_c05)

                'writer.WriteStartElement("cbc:ID")
                'writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                'writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                'writer.WriteString(item.t17_tiptributo_c05)
                'writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t28_nomtributo_c05)
                writer.WriteElementString("cbc:TaxTypeCode", item.t28_tiptributointernacional_c05)

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            writer.WriteEndElement()


            'Parte44
            writer.WriteStartElement("cac:Item")
            writer.WriteStartElement("cbc:Description")
            writer.WriteCData(item.t13_descripcion)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
            writer.WriteEndElement()

            'Parte45
            writer.WriteStartElement("cac:SellersItemIdentification")
            writer.WriteElementString("cbc:ID", item.t30_codproducto)
            writer.WriteEndElement()

            'Parte45
            writer.WriteEndElement()


            'Parte48
            writer.WriteStartElement("cac:Price")
            'writer.WriteElementString("cbc:ID", item.t34_codproducto)

            writer.WriteStartElement("cbc:PriceAmount ")
            writer.WriteAttributeString("currencyID", BoletaBE.t24_tipmoneda_c02)
            writer.WriteString(item.t31_valorunitario)
            writer.WriteEndElement()

            writer.WriteEndElement()
            'Parte48

            writer.WriteEndElement()
            'Parte35

            orden = orden + 1
        Next

        writer.WriteEndElement()
        'Origen

        'Se cierra el documento
        writer.WriteEndDocument()

        writer.Close()
        writer.Dispose()

        'Se usa para reemplazar la cadena de minusculas a mayusculas
        Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
        Data = Data.Replace("iso-8859-1", "ISO-8859-1")
        File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)

        'Se guarda la ruta del archivo xml
        SaveRutaArchivoXML(IDBoleta, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se firma el comprobante
        Dim FirmaXMLDAO As New FirmaXMLDAO
        FirmaXMLDAO.Create(EmisorBE, RutaArchivo & NombreArchivo & ExtensionArchivoXML, "03")

    End Sub


    Public Function ZipXML(IDBoleta As Int32) As String

        'Se obtiene la entidad
        BoletaBE = Me.GetByID(IDBoleta)

        'Se obtiene el nombre del archivo zip
        Dim RutaArchivoZIP As String = Path.ChangeExtension(BoletaBE.RutaComprobanteXML, "zip")

        Using zipToOpen As FileStream = New FileStream(RutaArchivoZIP, FileMode.Create)
            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(Path.GetFileName(BoletaBE.RutaComprobanteXML))
                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(BoletaBE.RutaComprobanteXML))
                writer.Flush()
                writer.Close()
            End Using
        End Using

        Return RutaArchivoZIP
    End Function
    Public Function UnZipXML(NombreArchivo As String) As Boolean
        Dim Result As Boolean = False

        Using archive As ZipArchive = ZipFile.OpenRead(NombreArchivo)
            For Each entry As ZipArchiveEntry In archive.Entries
                If entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) Then

                    'Si el archivo existe, se sobreescribe el archivo
                    entry.ExtractToFile(Path.Combine(Path.GetDirectoryName(NombreArchivo), entry.FullName), True)
                    Result = True
                End If
            Next
        End Using

        Return Result
    End Function

    Public Function GetByID(IDBoleta As Int32) As BoletaBE
        Dim BoletaDetalle As New List(Of BoletaItem)
        Dim BoletaAnticipos As New List(Of BoletaAnticipo)
        Dim BoletaBE As New BoletaBE
        Dim BoletaDet As New BoletaItem
        Dim BoletaAnticipo As New BoletaAnticipo
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_get_id"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then

                While dr.Read
                    With BoletaBE
                        .idboleta = dr.ReadNullAsEmptyString("idboleta")
                        .t01_fecemision = dr.ReadNullAsEmptyString("t01_fecemision")
                        .t06_tipdoc_c01 = dr.ReadNullAsEmptyString("t06_tipdoc_c01")
                        .t07_numcorrelativo = dr.ReadNullAsEmptyString("t07_numcorrelativo")
                        .t08_numdoc = dr.ReadNullAsEmptyString("t08_numdoc")
                        .t08_tipdoc_c06 = dr.ReadNullAsEmptyString("t08_tipdoc_c06")
                        .t09_nomadquiriente = dr.ReadNullAsEmptyString("t09_nomadquiriente")
                        .t10_direccionadquiriente = dr.ReadNullAsEmptyString("t10_direccionadquiriente")
                        .t15_tipmonto_c14 = dr.ReadNullAsEmptyString("t15_tipmonto_c14")
                        .t15_totalvalorventagravadas = dr.ReadNullAsEmptyString("t15_totalvalorventagravadas")
                        .t16_tipmonto_c14 = dr.ReadNullAsEmptyString("t16_tipmonto_c14")
                        .t16_totalvalorventainafectas = dr.ReadNullAsEmptyString("t16_totalvalorventainafectas")
                        .t17_tipmonto_c14 = dr.ReadNullAsEmptyString("t17_tipmonto_c14")
                        .t17_totalvalorventaexonerada = dr.ReadNullAsEmptyString("t17_totalvalorventaexonerada")
                        .t18_totaligv = dr.ReadNullAsEmptyString("t18_totaligv")
                        .t18_subtotaligv = dr.ReadNullAsEmptyString("t18_subtotaligv")
                        .t18_tiptributo_c05 = dr.ReadNullAsEmptyString("t18_tiptributo_c05")
                        .t18_nomtributo_c05 = dr.ReadNullAsEmptyString("t18_nomtributo_c05")
                        .t18_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t18_tiptributointernacional_c05")
                        .t19_totalisc = dr.ReadNullAsEmptyString("t19_totalisc")
                        .t19_subtotalisc = dr.ReadNullAsEmptyString("t19_subtotalisc")
                        .t19_tiptributo_c05 = dr.ReadNullAsEmptyString("t19_tiptributo_c05")
                        .t19_nomtributo_c05 = dr.ReadNullAsEmptyString("t19_nomtributo_c05")
                        .t19_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t19_tiptributointernacional_c05")
                        .t20_totalotrostributos = dr.ReadNullAsEmptyString("t20_totalotrostributos")
                        .t20_subtotalotrostributos = dr.ReadNullAsEmptyString("t20_subtotalotrostributos")
                        .t20_tiptributo_c05 = dr.ReadNullAsEmptyString("t20_tiptributo_c05")
                        .t20_nomtributo_c05 = dr.ReadNullAsEmptyString("t20_nomtributo_c05")
                        .t20_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t20_tiptributointernacional_c05")
                        .t21_totalotroscargos = dr.ReadNullAsEmptyString("t21_totalotroscargos")
                        .t22_tipmonto_c14 = dr.ReadNullAsEmptyString("t22_tipmonto_c14")
                        .t22_totaldescuentos = dr.ReadNullAsEmptyString("t22_totaldescuentos")
                        .t23_totalimporte = dr.ReadNullAsEmptyString("t23_totalimporte")
                        .t24_tipmoneda_c02 = dr.ReadNullAsEmptyString("t24_tipmoneda_c02")
                        .t25_sernumguia = dr.ReadNullAsEmptyString("t25_sernumguia")
                        .t25_tipdoc_c01 = dr.ReadNullAsEmptyString("t25_tipdoc_c01")
                        .t26_tipleyenda_c15 = dr.ReadNullAsEmptyString("t26_tipleyenda_c15")
                        .t26_descripcionleyenda = dr.ReadNullAsEmptyString("t26_descripcionleyenda")
                        .t34_tipmonto_c14 = dr.ReadNullAsEmptyString("t34_tipmonto_c14")
                        .t34_montopercepcion = dr.ReadNullAsEmptyString("t34_montopercepcion")
                        .t34_montototalpercepcion = dr.ReadNullAsEmptyString("t34_montototalpercepcion")
                        .t35_numdocrelacionado = dr.ReadNullAsEmptyString("t35_numdocrelacionado")
                        .t35_tipdoc_c12 = dr.ReadNullAsEmptyString("t35_tipdoc_c12")
                        .t37_versionubl = dr.ReadNullAsEmptyString("t37_versionubl")
                        .t38_versiondoc = dr.ReadNullAsEmptyString("t38_versiondoc")
                        .t39_tipdoc_c14 = dr.ReadNullAsEmptyString("t39_tipdoc_c14")
                        .t39_baseimponiblepercepcion = dr.ReadNullAsEmptyString("t39_baseimponiblepercepcion")
                        .t39_montopercepcion = dr.ReadNullAsEmptyString("t39_montopercepcion")
                        .t39_montototalpercepcion = dr.ReadNullAsEmptyString("t39_montototalpercepcion")
                        .t40_tipmonto_c14 = dr.ReadNullAsEmptyString("t40_tipmonto_c14")
                        .t40_totalvalorventaoperacionesgratuitas = dr.ReadNullAsEmptyString("t40_totalvalorventaoperacionesgratuitas")
                        .t41_descuentosglobales = dr.ReadNullAsEmptyString("t41_descuentosglobales")
                        .t42_tipoperacion_c17 = dr.ReadNullAsEmptyString("t42_tipoperacion_c17")

                        .CodigoRespuesta = dr.ReadNullAsEmptyString("codigorespuesta")
                        .Observacion = dr.ReadNullAsEmptyString("observacion")
                        .estado = dr.ReadNullAsEmptyString("estado")
                        .estadoboleta = dr.ReadNullAsEmptyString("estadoboleta")
                        .digestvalue = dr.ReadNullAsEmptyString("digestvalue")
                        .archivoxml = dr.ReadNullAsEmptyString("archivoxml")
                        .RutaRespuestaSunatXML = dr.ReadNullAsEmptyString("rutarespuestasunatxml")
                        .RutaComprobanteXML = dr.ReadNullAsEmptyString("archivoxml")
                        .RutaComprobanteZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")
                        .RutaComprobantePDF = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "pdf")
                        .RutaRespuestaSunatXML = dr.ReadNullAsEmptyString("rutarespuestasunatxml")
                        .RutaRespuestaSunatZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")

                        .maquina = dr.ReadNullAsEmptyString("maquina")
                        .DireccionAdquiriente = dr.ReadNullAsEmptyString("direccionadquiriente")
                        .EmailAdquiriente = dr.ReadNullAsEmptyString("emailadquiriente")
                        .idcomprobanteformato = dr.ReadNullAsNumeric("idcomprobanteformato")
                        .idusuario = dr.ReadNullAsNumeric("idusuario")
                        .fechahorasistemaexterno = dr.ReadNullAsEmptyString("fechahorasistemaexterno")
                        .fechahorasunat = dr.ReadNullAsEmptyDate("fechahorasunat")
                        .fecharegistro = dr.ReadNullAsEmptyDate("fecharegistro")

                        .t01_horaemision = dr.ReadNullAsEmptyDate("t01_horaemision")
                    End With
                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        BoletaDet = New BoletaItem
                        With BoletaDet
                            .idboletadetalle = dr.ReadNullAsEmptyString("idboletadetalle")
                            .idboleta = dr.ReadNullAsEmptyString("idboleta")
                            .t11_tipunidadmedida_c03 = dr.ReadNullAsEmptyString("t11_tipunidadmedida_c03")
                            .t12_cantidad = dr.ReadNullAsEmptyString("t12_cantidad")
                            .t13_descripcion = dr.ReadNullAsEmptyString("t13_descripcion")
                            .t14_preciounitario = dr.ReadNullAsEmptyString("t14_preciounitario")
                            .t14_tipprecio_c16 = dr.ReadNullAsEmptyString("t14_tipprecio_c16")
                            .t27_totalitem = dr.ReadNullAsEmptyString("t27_totalitem")
                            .t27_subtotalitem = dr.ReadNullAsEmptyString("t27_subtotalitem")
                            .t27_tipafectacion_c07 = dr.ReadNullAsEmptyString("t27_tipafectacion_c07")
                            .t27_tiptributo_c05 = dr.ReadNullAsEmptyString("t27_tiptributo_c05")
                            .t27_nomtributo_c05 = dr.ReadNullAsEmptyString("t27_nomtributo_c05")
                            .t27_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t27_tiptributointernacional_c05")
                            .t28_totalitem = dr.ReadNullAsEmptyString("t28_totalitem")
                            .t28_subtotalitem = dr.ReadNullAsEmptyString("t28_subtotalitem")
                            .t28_tipsistema_c08 = dr.ReadNullAsEmptyString("t28_tipsistema_c08")
                            .t28_tiptributo_c05 = dr.ReadNullAsEmptyString("t28_tiptributo_c05")
                            .t28_nomtributo_c05 = dr.ReadNullAsEmptyString("t28_nomtributo_c05")
                            .t28_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t28_tiptributointernacional_c05")
                            .t29_numordenitem = dr.ReadNullAsEmptyString("t29_numordenitem")
                            .t30_codproducto = dr.ReadNullAsEmptyString("t30_codproducto")
                            .t31_valorunitario = dr.ReadNullAsEmptyString("t31_valorunitario")
                            .t32_valorventaitem = dr.ReadNullAsEmptyString("t32_valorventaitem")
                            .t33_valorreferencialunitarioitem = dr.ReadNullAsEmptyString("t33_valorreferencialunitarioitem")
                            .t33_tipreferencia_c16 = dr.ReadNullAsEmptyString("t33_tipreferencia_c16")
                            .t42_tipdescuentoitem = dr.ReadNullAsEmptyString("t42_tipdescuentoitem")
                            .t42_descuentoitem = dr.ReadNullAsEmptyString("t42_descuentoitem")
                        End With

                        BoletaDetalle.Add(BoletaDet)
                    End While
                End If

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        BoletaAnticipo = New BoletaAnticipo
                        With BoletaAnticipo
                            .IDAnticipo = dr.ReadNullAsEmptyString("idanticipo")
                            .IDComprobante = dr.ReadNullAsEmptyString("idcomprobante")
                            .Referencia = dr.ReadNullAsEmptyString("referencia")
                            .TipoComprobante = dr.ReadNullAsEmptyString("tipocomprobante")
                            .Importe = dr.ReadNullAsEmptyString("importe")
                            .NumDocumento = dr.ReadNullAsEmptyString("numdoc")
                        End With
                        BoletaBE.Anticipos.Add(BoletaAnticipo)
                    End While
                End If

                BoletaBE.Detalle = BoletaDetalle
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Me.BE = BoletaBE

        Return BoletaBE
    End Function
    Public Function GetByReporteID(IDBoleta As Int32) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_rpt_id"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
    Public Function GetByIDXML(IDBoleta As Int32) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As System.Xml.XmlReader = Nothing
        Dim doc As New XmlDocument()
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_get_id_xml"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteXmlReader
            dr.Read()

            'Se carga el XML en objeto XMLDocument
            doc.Load(dr)

            'Se obtiene el XML completo de factura y detalle
            Result = doc.InnerXml

        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return "<?xml version=""1.0""?>" & Result

    End Function
    Public Function GetByAll(FechaInicial As Date, FechaFinal As Date) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_get_all"
            .Parameters.Add("@fechainicial", SqlDbType.Date).Value = FechaInicial
            .Parameters.Add("@fechafinal", SqlDbType.Date).Value = FechaFinal
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt

    End Function

    Public Function SaveCloud(EmisorBE As EmisorBE, IDComprobante As Int32) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantesWeb)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False
        Dim RutaCloud As String = String.Empty

        Dim BoletaBE As New BoletaBE
        BoletaBE = Me.GetByID(IDComprobante)

        'Se establece la carpeta donde se almacena
        RutaCloud = "/r" & EmisorBE.NumeroRUC & "/"

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_web_ins"

            With .Parameters
                .Add("@IDComprobanteWeb", SqlDbType.Int).Direction = ParameterDirection.Output
                .Add("@IDComprobante", SqlDbType.Int).Value = BoletaBE.idboleta
                .Add("@Tipo", SqlDbType.Char, 2).Value = BoletaBE.TipoComprobante
                .Add("@NumeroRUC", SqlDbType.VarChar, 15).Value = EmisorBE.NumeroRUC
                .Add("@NumeroCorrelativo", SqlDbType.VarChar, 20).Value = BoletaBE.t07_numcorrelativo
                .Add("@FechaEmision", SqlDbType.VarChar, 10).Value = BoletaBE.t01_fecemision
                .Add("@Importe", SqlDbType.Decimal).Value = BoletaBE.t23_totalimporte
                .Add("@RutaArchivoPDF", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(BoletaBE.RutaComprobantePDF)
                .Add("@RutaArchivoXML", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(BoletaBE.RutaComprobanteXML)
                .Add("@FechaRegistro", SqlDbType.DateTime).Value = DateTime.Now
                .Add("@CodLocal", SqlDbType.VarChar, 10).Value = EmisorBE.CodigoLocal
            End With
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function
    Public Function SaveCloudArchivos(EmisorBe As EmisorBE, IDComprobante As Int32) As Boolean
        Dim Result As Boolean = False

        Dim BoletaBE As New BoletaBE
        BoletaBE = Me.GetByID(IDComprobante)

        If ToolsAzure.SaveStorageFiles(EmisorBe.ConexionStorageCloud, "r" & EmisorBe.NumeroRUC, BoletaBE.RutaComprobanteXML) Then
            If ToolsAzure.SaveStorageFiles(EmisorBe.ConexionStorageCloud, "r" & EmisorBe.NumeroRUC, BoletaBE.RutaComprobantePDF) Then
                Result = True
            End If
        End If

        Return Result
    End Function

    Public Function SaveRutaArchivoXML(IDBoleta As Int32, RutaCarpetaXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_upd_ruta"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
            .Parameters.Add("@rutaarchivoxml", SqlDbType.VarChar, 500).Value = RutaCarpetaXML
            .Parameters.Add("@estadosunat", SqlDbType.Int).Value = eEstadoSunat.PendienteEnvio
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
    Public Function SaveRutaArchivoCdrXML(IDBoleta As Int32, RutaCarpetaCdrXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_upd_ruta_cdr"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
            .Parameters.Add("@rutaarchivocdrxml", SqlDbType.VarChar, 500).Value = RutaCarpetaCdrXML
            .Parameters.Add("@estadosunat", SqlDbType.Int).Value = eEstadoSunat.EnProceso
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
    Public Function SaveValorFirma(IDBoleta As Int32, ValorFirma As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_upd_firma"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
            .Parameters.Add("@valorfirma", SqlDbType.VarChar, 250).Value = ValorFirma
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
    Public Function SaveConstanciaRecepcionZIP(IDBoleta As Int32, ContenidoArchivoZIP As Byte()) As Boolean
        Dim Result As Boolean = False
        Dim NombreArchivo As String = String.Empty

        Try
            'Se obtiene la entidad
            BoletaBE = Me.GetByID(IDBoleta)

            'Se obtiene el nombre del archivo xml, sin extension
            NombreArchivo = Path.GetFileNameWithoutExtension(BoletaBE.RutaComprobanteXML)

            'Se agrega la letra R al nombre (Respuesta) y se coloca la extension .zip
            NombreArchivo = "R-" & Path.ChangeExtension(NombreArchivo, ".zip")

            'Se concatena la ruta y el nuevo nombre del archivo zip
            NombreArchivo = Path.GetDirectoryName(BoletaBE.RutaComprobanteXML) & "\" & NombreArchivo

            'Se guarda el archivo zip que envia la sunat
            System.IO.File.WriteAllBytes(NombreArchivo, ContenidoArchivoZIP)

            'Se descomprime el archivo ZIP y extrae el XML
            Me.UnZipXML(NombreArchivo)

            'Se guarda el nombre del archivo de la constancia de recepcion CDR SUNAT
            Me.SaveRutaArchivoCdrXML(IDBoleta, Path.ChangeExtension(NombreArchivo, ".xml"))

            'Se lee el contenido del archivo XML y se lee contenido
            Me.SaveConstanciaRecepcionXML(IDBoleta)

            Result = True
        Catch ex As Exception
            Throw New Exception("No se guardo el archivo de respuesta de la SUNAT. " & ex.Message)
        End Try

        Return Result
    End Function
    Public Function SaveConstanciaRecepcionXML(IDBoleta As Int32) As Boolean
        Dim Result As Boolean = False
        Dim Valor As String = String.Empty
        Dim CodigoRespuesta As String = String.Empty
        Dim NumeroComprobante As String = String.Empty
        Dim Descripcion As String = String.Empty

        'Se obtiene la entidad
        BoletaBE = Me.GetByID(IDBoleta)

        'Se obtiene el archivo XML
        Dim ReaderXML As XmlTextReader = New XmlTextReader(BoletaBE.RutaRespuestaSunatXML)

        While (ReaderXML.Read())

            'Se lee la raiz de cada nodo
            Valor = ReaderXML.Name

            Select Case ReaderXML.NodeType
                Case XmlNodeType.Element

                    'Se obtiene el codigo de respuesta
                    If ReaderXML.Name = "cbc:ResponseCode" Then
                        ReaderXML.Read()
                        CodigoRespuesta = ReaderXML.Value
                    End If

                    'Se obtiene el codigo de comprobante serie y numero
                    If ReaderXML.Name = "cbc:ReferenceID" Then
                        ReaderXML.Read()
                        NumeroComprobante = ReaderXML.Value
                    End If

                    'Se obtiene la descripcion de la respuesta
                    If ReaderXML.Name = "cbc:Description" Then
                        ReaderXML.Read()
                        Descripcion = ReaderXML.Value
                    End If

                    'Se obtiene atributos del nodo. No se usa
                    If ReaderXML.HasAttributes Then
                        While ReaderXML.MoveToNextAttribute()
                            'Console.Write(" {0}='{1}'", ReaderXML.Name, ReaderXML.Value)
                        End While
                    End If

                    'Se obtiene el texto de cada elemento. No Se usa
                Case XmlNodeType.Text
                    'Console.WriteLine(ReaderXML.Value)

                    'Se obtiene el fin del elemento. No se usa
                Case XmlNodeType.EndElement
                    'Console.Write("</" + ReaderXML.Name)
                    'Console.WriteLine(">")
            End Select
        End While

        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_upd_estado"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
            .Parameters.Add("@estado", SqlDbType.Int).Value = If(CodigoRespuesta = "0", eEstadoSunat.Aceptado, eEstadoSunat.Rechazado)
            .Parameters.Add("@codigorespuesta", SqlDbType.VarChar, 4).Value = CodigoRespuesta
            .Parameters.Add("@observacion", SqlDbType.VarChar, 4000).Value = Descripcion
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try

        Return Result

    End Function
    Public Function SaveExcepcion(BE As ExcepcionBE) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        'Se extrae solo los numeros del codigo de excepcion
        BE.CodigoExcepcion = Tools.Num(BE.CodigoExcepcion)

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_upd_excepcion"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = BE.IDComprobante
            .Parameters.Add("@idestado", SqlDbType.Int).Value = BE.IDEstado
            .Parameters.Add("@codigoexcepcion", SqlDbType.VarChar, 4).Value = BE.CodigoExcepcion
            .Parameters.Add("@descripcion", SqlDbType.VarChar, 4000).Value = BE.Descripcion
            .Parameters.Add("@fechahora", SqlDbType.DateTime).Value = BE.FechaHora
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
    Public Function SaveEstadoWeb(IDBoleta As Int32, IDEstadoWeb As eEstadoWeb, FechaWeb As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_boleta_estado_web"
            .Parameters.Add("@idboleta", SqlDbType.Int).Value = IDBoleta
            .Parameters.Add("@idestadoweb", SqlDbType.Int).Value = IDEstadoWeb
            .Parameters.Add("@fechahoraweb", SqlDbType.DateTime).Value = FechaWeb
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
End Class
