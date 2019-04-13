#Region "Imports"
Imports COE.FRAMEWORK
Imports System.IO
Imports System.IO.Compression
Imports System.IO.Compression.ZipArchive
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
Imports System.Net.Mail
Imports System.Globalization

#End Region

Public Enum eEstadoNotaCredito
    Anulado = 0
    Activo = 1
End Enum

Public Class NotaCreditoDAO
    Public Property IDNotaCredito As Int32
    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim EmisorBE As New EmisorBE
    Dim NotaCreditoBE As New NotaCreditoBE


    'Public Function CreateXML(IDNotaCredito As Int32) As String

    '    'Se carga los datos el emisor
    '    EmisorBE = EmisorDAO.GetByID(1)

    '    'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
    '    'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
    '    Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
    '    Dim NombreArchivo As String = String.Empty
    '    Dim ExtensionArchivoXML As String = ".xml"

    '    'Se obtiene la entidad
    '    NotaCreditoBE = Me.GetByID(IDNotaCredito)

    '    'Se obtiene el nombre del archivo XML
    '    NombreArchivo = EmisorBE.NumeroRUC
    '    NombreArchivo &= "-" & "07" & "-" & NotaCreditoBE.t08_numcorrelativo

    '    'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
    '    'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
    '    Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
    '    writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
    '    writer.Formatting = Formatting.Indented
    '    writer.Indentation = 0

    '    'Se crea el nodo raiz
    '    writer.WriteStartElement("CreditNote")
    '    writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")
    '    writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
    '    writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
    '    writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
    '    writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
    '    writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
    '    writer.WriteAttributeString("xmlns:qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
    '    writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
    '    writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
    '    writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")

    '    writer.WriteStartElement("ext:UBLExtensions")
    '    writer.WriteStartElement("ext:UBLExtension")
    '    writer.WriteStartElement("ext:ExtensionContent")
    '    writer.WriteStartElement("sac:AdditionalInformation")

    '    'Revisar: Catálogo No. 14: Códigos - Otros conceptos tributarios
    '    'Tag Catálogo No. 14 - 1001 Total valor de venta - operaciones gravadas
    '    writer.WriteStartElement("sac:AdditionalMonetaryTotal")
    '    writer.WriteElementString("cbc:ID", NotaCreditoBE.t20_tipmonto_c14)
    '    writer.WriteStartElement("cbc:PayableAmount")
    '    writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
    '    writer.WriteString(NotaCreditoBE.t20_totalvalorventasgravadas)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Se cierra el UBLExtension
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Se define este tag es para guardar la firma digital
    '    writer.WriteStartElement("ext:UBLExtension")
    '    writer.WriteStartElement("ext:ExtensionContent")
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Bloque dos
    '    'La version del UBLVersionID es 2.0
    '    writer.WriteElementString("cbc:UBLVersionID", NotaCreditoBE.t36_versionubl)

    '    'La version del CustomizationID es 1.0
    '    writer.WriteElementString("cbc:CustomizationID", NotaCreditoBE.t37_versiondoc)

    '    writer.WriteElementString("cbc:ID", NotaCreditoBE.t08_numcorrelativo)
    '    writer.WriteElementString("cbc:IssueDate", NotaCreditoBE.t01_fecemision)
    '    writer.WriteElementString("cbc:DocumentCurrencyCode", NotaCreditoBE.t30_tipomoneda_c02)

    '    'Bloques Tres (Nuevo bloque de Nota credito)
    '    writer.WriteStartElement("cac:DiscrepancyResponse")
    '    writer.WriteElementString("cbc:ReferenceID", NotaCreditoBE.t07_sernumdocafectado)
    '    writer.WriteElementString("cbc:ResponseCode", NotaCreditoBE.t07_tipnotacredito_c09)
    '    writer.WriteElementString("cbc:Description", NotaCreditoBE.t11_motivo)
    '    writer.WriteEndElement()

    '    'Bloque cuatro datos del documento afectado
    '    writer.WriteStartElement("cac:BillingReference")
    '    writer.WriteStartElement("cac:InvoiceDocumentReference")
    '    writer.WriteElementString("cbc:ID", NotaCreditoBE.t07_sernumdocafectado)
    '    writer.WriteElementString("cbc:DocumentTypeCode", NotaCreditoBE.t33_tipdoc_c01)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'bloque cinco Informacion de emisor
    '    writer.WriteStartElement("cac:Signature")
    '    writer.WriteElementString("cbc:ID", "IDSignSP")
    '    writer.WriteStartElement("cac:SignatoryParty")
    '    writer.WriteStartElement("cac:PartyIdentification")
    '    writer.WriteElementString("cbc:ID", EmisorBE.NumeroRUC)
    '    writer.WriteEndElement()

    '    writer.WriteStartElement("cac:PartyName")
    '    writer.WriteStartElement("cbc:Name")
    '    writer.WriteCData(EmisorBE.RazonSocial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    writer.WriteStartElement("cac:DigitalSignatureAttachment")
    '    writer.WriteStartElement("cac:ExternalReference")
    '    writer.WriteElementString("cbc:URI", "#SignatureSP")
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Bloque cuatro informacion emisor
    '    writer.WriteStartElement("cac:AccountingSupplierParty")
    '    writer.WriteElementString("cbc:CustomerAssignedAccountID", EmisorBE.NumeroRUC)
    '    writer.WriteElementString("cbc:AdditionalAccountID", "6")
    '    writer.WriteStartElement("cac:Party")

    '    'Se agrega tag CDATA. Se usa el nombre comercial del emisor, siempre que en el registro del ruc este declarado
    '    writer.WriteStartElement("cac:PartyName")
    '    writer.WriteStartElement("cbc:Name")
    '    writer.WriteCData(EmisorBE.NombreComercial)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    writer.WriteStartElement("cac:PostalAddress")
    '    writer.WriteElementString("cbc:ID", EmisorBE.CodigoUbigeo)
    '    writer.WriteElementString("cbc:StreetName", EmisorBE.NombreDireccion)
    '    writer.WriteElementString("cbc:CitySubdivisionName", EmisorBE.NombreUrbanizacion)
    '    writer.WriteElementString("cbc:CityName", EmisorBE.NombreDepartamento)
    '    writer.WriteElementString("cbc:CountrySubentity", EmisorBE.NombreProvincia)
    '    writer.WriteElementString("cbc:District", EmisorBE.NombreDistrito)

    '    writer.WriteStartElement("cac:Country")
    '    writer.WriteElementString("cbc:IdentificationCode", "PE")
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
    '    writer.WriteStartElement("cac:PartyLegalEntity")
    '    writer.WriteStartElement("cbc:RegistrationName")
    '    writer.WriteCData(EmisorBE.RazonSocial)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'Bloque cinco informacion de adquiriente.  'Se agrega tag CDATA al Nombre del adquiriente
    '    writer.WriteStartElement("cac:AccountingCustomerParty")
    '    writer.WriteElementString("cbc:CustomerAssignedAccountID", NotaCreditoBE.t09_numdocadquiriente)
    '    writer.WriteElementString("cbc:AdditionalAccountID", NotaCreditoBE.t09_tipdocadquiriente_c06)
    '    writer.WriteStartElement("cac:Party")
    '    writer.WriteStartElement("cac:PartyLegalEntity")
    '    writer.WriteStartElement("cbc:RegistrationName")
    '    writer.WriteCData(NotaCreditoBE.t10_nomadquiriente)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'bloque seis Sumatoria de Impuestos Falta revisar los impuestos
    '    writer.WriteStartElement("cac:TaxTotal")
    '    Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, NotaCreditoBE.t24_totaligv) '.sumatoriaigvtotal) ') 'NotaCreditoBE.tipomoneda, NotaCreditoBE.sumatoriaigvtotal)

    '    writer.WriteStartElement("cac:TaxSubtotal")
    '    Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, NotaCreditoBE.t24_subtotaligv) '.sumatoriaigvtotal) 'NotaCreditoBE.tipomoneda, NotaCreditoBE.sumatoriaigvtotal)

    '    writer.WriteStartElement("cac:TaxCategory")
    '    writer.WriteStartElement("cac:TaxScheme")
    '    writer.WriteElementString("cbc:ID", NotaCreditoBE.t24_tiptributo_c05)
    '    writer.WriteElementString("cbc:Name", NotaCreditoBE.t24_nomtributo_c05)
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()
    '    writer.WriteEndElement()

    '    'bloque siete
    '    writer.WriteStartElement("cac:LegalMonetaryTotal")
    '    Tools.TagNodoAtributoValorValor(writer, "cbc:PayableAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, NotaCreditoBE.t29_totalimporte) '.importetotalventa)  ') ' NotaCreditoBE.tipomoneda, NotaCreditoBE.importetotalventa)
    '    writer.WriteEndElement()

    '    'bloque ocho: Este bloque tiene los items de la factura
    '    For Each Item In NotaCreditoBE.Detalle

    '        ' NumeroOrden += 1
    '        writer.WriteStartElement("cac:CreditNoteLine")
    '        writer.WriteElementString("cbc:ID", Item.t34_numordenitem)
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:CreditedQuantity", "unitCode", Item.t12_tipunidadmedida_c03, Item.t13_cantidad)
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:LineExtensionAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, Item.t17_preciounitario * Item.t13_cantidad) 'Precio * Cantidad sin igv

    '        writer.WriteStartElement("cac:PricingReference")
    '        writer.WriteStartElement("cac:AlternativeConditionPrice")
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:PriceAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, Item.t17_preciounitario) 'Precio venta incluido IGV
    '        writer.WriteElementString("cbc:PriceTypeCode", Item.t17_tipprecio_c16)
    '        writer.WriteEndElement()
    '        writer.WriteEndElement()

    '        'REVISAR
    '        writer.WriteStartElement("cac:TaxTotal")
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, Item.t18_totalitem)
    '        writer.WriteStartElement("cac:TaxSubtotal")
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, Item.t18_subtotalitem)
    '        writer.WriteStartElement("cac:TaxCategory")
    '        writer.WriteElementString("cbc:TaxExemptionReasonCode", Item.t18_tipafectacion_c07)

    '        writer.WriteStartElement("cac:TaxScheme")
    '        writer.WriteElementString("cbc:ID", Item.t18_tiptributo_c05)
    '        writer.WriteElementString("cbc:Name", Item.t18_nomtributo.ToString.Trim)
    '        writer.WriteElementString("cbc:TaxTypeCode", Item.t18_tiptributointernacional)
    '        writer.WriteEndElement()
    '        writer.WriteEndElement()
    '        writer.WriteEndElement()
    '        writer.WriteEndElement()

    '        writer.WriteStartElement("cac:Item")
    '        writer.WriteStartElement("cbc:Description")
    '        writer.WriteCData(Item.t15_descripcion)
    '        writer.WriteEndElement()
    '        writer.WriteStartElement("cac:SellersItemIdentification")

    '        'writer.WriteElementString("cbc:ID", Item.t14_codproducto)
    '        writer.WriteStartElement("cbc:ID")
    '        writer.WriteCData(Item.t14_codproducto)
    '        writer.WriteEndElement()


    '        writer.WriteEndElement()
    '        writer.WriteEndElement()

    '        writer.WriteStartElement("cac:Price")
    '        Tools.TagNodoAtributoValorValor(writer, "cbc:PriceAmount", "currencyID", NotaCreditoBE.t30_tipomoneda_c02, Item.t16_valorunitario)
    '        writer.WriteEndElement()
    '        writer.WriteEndElement()
    '    Next

    '    'Se finaliza el Invoice
    '    writer.WriteEndElement()

    '    'Se cierra el documento
    '    writer.WriteEndDocument()
    '    writer.Close()
    '    writer.Dispose()

    '    'Se usa para reemplazar la cadena de minusculas a mayusculas
    '    Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
    '    Data = Data.Replace("iso-8859-1", "ISO-8859-1")
    '    File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)

    '    'Se guarda la ruta del archivo xml
    '    SaveRutaArchivoXML(IDNotaCredito, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

    '    'Se retorna el nombre de archivo
    '    Return RutaArchivo & NombreArchivo & ExtensionArchivoXML
    'End Function

    Public Sub CreateFileXML21(IDNotaCredito As Int32)

        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"

        'Se obtiene la entidad
        NotaCreditoBE = Me.GetByID(IDNotaCredito)

        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-" & "07" & "-" & NotaCreditoBE.t08_numcorrelativo

        'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
        'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'ORIGEN
        writer.WriteStartElement("CreditNote")
        writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
        'writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")

        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Parte2
        writer.WriteElementString("cbc:UBLVersionID", NotaCreditoBE.t36_versionubl)
        'Parte2

        'Parte3
        writer.WriteElementString("cbc:CustomizationID", NotaCreditoBE.t37_versiondoc)
        'Parte3

        'Parte5
        writer.WriteElementString("cbc:ID", NotaCreditoBE.t08_numcorrelativo)
        'Parte5
        'Parte6
        writer.WriteElementString("cbc:IssueDate", NotaCreditoBE.t01_fecemision)
        'Parte6
        'Parte7
        'writer.WriteElementString("cbc:IssueTime", Right("00" + NotaCreditoBE.fechahorasistemaexterno.Hour.ToString(), 2) + ":" + Right("00" + NotaCreditoBE.fechahorasistemaexterno.Minute.ToString(), 2) + ":" + Right("00" + NotaCreditoBE.fechahorasistemaexterno.Second.ToString(), 2)) 'hh-mm-ss.0z agregar dato de hora
        writer.WriteElementString("cbc:IssueTime", NotaCreditoBE.HoraEmision) 'hh-mm-ss.0z agregar dato de hora
        'Parte7

        'Parte10 cbc:Note
        writer.WriteStartElement("cbc:Note")
        writer.WriteAttributeString("languageLocaleID", "1000")
        writer.WriteString(NotaCreditoBE.descripcionleyenda)
        writer.WriteEndElement()
        'Parte10 cbc:Note

        'Parte11 cbc:DocumentCurrencyCode
        writer.WriteStartElement("cbc:DocumentCurrencyCode")
        writer.WriteAttributeString("listID", "ISO 4217 Alpha")
        writer.WriteAttributeString("listName", "Currency")
        writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        writer.WriteString(NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteEndElement()
        'Parte11 cbc:DocumentCurrencyCode

        'writer.WriteEndElement()
        'writer.WriteEndElement()
        'writer.WriteEndElement()


        'Parte 8 y 9
        writer.WriteStartElement("cac:DiscrepancyResponse")
        writer.WriteElementString("cbc:ReferenceID", NotaCreditoBE.t07_sernumdocafectado)
        writer.WriteElementString("cbc:ResponseCode", NotaCreditoBE.t07_tipnotacredito_c09)
        writer.WriteStartElement("cbc:Description")
        writer.WriteCData(NotaCreditoBE.t11_motivo)
        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte 8 y 9

        'Parte 10 y 11
        writer.WriteStartElement("cac:BillingReference")
        writer.WriteStartElement("cac:InvoiceDocumentReference")

        writer.WriteElementString("cbc:ID", NotaCreditoBE.t07_sernumdocafectado)
        writer.WriteElementString("cbc:DocumentTypeCode", NotaCreditoBE.t33_tipdoc_c01)

        writer.WriteEndElement()
        writer.WriteEndElement()
        'Parte 10 y 11

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

        writer.WriteStartElement("cbc:CompanyID")
        writer.WriteAttributeString("schemeID", "6")
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)
        writer.WriteEndElement()


        'writer.WriteStartElement("cac:RegistrationAddres")
        'writer.WriteElementString("cbc:AddressTypeCode", "0000")
        'writer.WriteEndElement()

        writer.WriteStartElement("cac:TaxScheme")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", "6")
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()

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

        writer.WriteStartElement("cac:AccountingCustomerParty")
        writer.WriteStartElement("cac:Party")

        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", IIf(NotaCreditoBE.t09_tipdocadquiriente_c06 = "-", "0", NotaCreditoBE.t09_tipdocadquiriente_c06))
        writer.WriteAttributeString("schemeName", "Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(NotaCreditoBE.t09_numdocadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(NotaCreditoBE.t10_nomadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyTaxScheme")

        'Parte19
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(NotaCreditoBE.t10_nomadquiriente)
        writer.WriteEndElement()
        'Parte19

        writer.WriteStartElement("cbc:CompanyID")
        writer.WriteAttributeString("schemeID", NotaCreditoBE.t09_tipdocadquiriente_c06)
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see: gem:catalogos:catalogo06")
        writer.WriteString(NotaCreditoBE.t09_numdocadquiriente)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:TaxScheme")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", NotaCreditoBE.t09_tipdocadquiriente_c06)
        writer.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(NotaCreditoBE.t10_nomadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()

        writer.WriteStartElement("cac:RegistrationAddress")

        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeName", "Ubigeos")
        writer.WriteAttributeString("schemeAgencyName", "PE:INEI")
        writer.WriteEndElement()

        'writer.WriteElementString("cbc:CityName", EmisorBE.NombreProvincia)
        'writer.WriteElementString("cbc:CountrySubentity", EmisorBE.NombreDepartamento)
        'writer.WriteElementString("cbc:District", EmisorBE.NombreDistrito)

        writer.WriteStartElement("cac:AddressLine")
        writer.WriteElementString("cbc:Line", NotaCreditoBE.DireccionAdquiriente)
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

        writer.WriteStartElement("cac:TaxTotal")
        'If Double.Parse(NotaCreditoBE.t24_totaligv) + Double.Parse(NotaCreditoBE.t25_totalisc) + Double.Parse(NotaCreditoBE.t26_totalotrostributos) > 0 Then

        writer.WriteStartElement("cbc:TaxAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t24_totaligv) + Double.Parse(NotaCreditoBE.t25_totalisc) + Double.Parse(NotaCreditoBE.t26_totalotrostributos), 2).ToString("0.00", CultureInfo.InvariantCulture))
        writer.WriteEndElement()


        Dim SumatoriaTotal As Double
        Dim SumatoriaImpuesto As Double
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
            If item.t18_tipafectacion_c07 = "10" Then ' Or item.t16_tipafectacion_c07 = "11" Or item.t16_tipafectacion_c07 = "12" Or item.t16_tipafectacion_c07 = "13" Or item.t16_tipafectacion_c07 = "14" Or item.t16_tipafectacion_c07 = "15" Or item.t16_tipafectacion_c07 = "16" Or item.t16_tipafectacion_c07 = "17" Then
                SumatoriaTotal = SumatoriaTotal + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
                SumatoriaImpuesto = SumatoriaImpuesto + (Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad))
            End If
        Next

        If SumatoriaTotal > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t20_totalvalorventasgravadas), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto, 2).ToString("0.00", CultureInfo.InvariantCulture))
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
            writer.WriteElementString("cbc:Name", NotaCreditoBE.t24_nomtributo_c05)
            writer.WriteElementString("cbc:TaxTypeCode", NotaCreditoBE.t24_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If

        Dim SumatoriaTotal2 As Double
        Dim SumatoriaImpuesto2 As Double
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
            If item.t18_tipafectacion_c07 = "20" Then
                SumatoriaTotal2 = SumatoriaTotal2 + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
                SumatoriaImpuesto2 = SumatoriaImpuesto2 + (Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es EXONERADO
            End If
        Next

        If SumatoriaTotal2 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t22_totalvalorventaexonerada), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto2, 2).ToString("0.00", CultureInfo.InvariantCulture))
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

            writer.WriteElementString("cbc:Name", "EXONERADO")
            writer.WriteElementString("cbc:TaxTypeCode", NotaCreditoBE.t24_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If


        Dim SumatoriaTotal3 As Double
        Dim SumatoriaImpuesto3 As Double
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
            If item.t18_tipafectacion_c07 = "30" Then
                SumatoriaTotal3 = SumatoriaTotal3 + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
                SumatoriaImpuesto3 = SumatoriaImpuesto3 + (Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
            End If
        Next

        If SumatoriaTotal3 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t21_totalvalorventainafectas), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto3, 2).ToString("0.00", CultureInfo.InvariantCulture))
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

            writer.WriteElementString("cbc:Name", "INAFECTO")
            writer.WriteElementString("cbc:TaxTypeCode", "FRE")

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If

        'Dim SumatoriaTotal4 As Double
        'Dim SumatoriaImpuesto4 As Double
        'For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
        '    If item.t18_tipafectacion_c07 = "21" Then
        '        SumatoriaTotal4 = SumatoriaTotal4 + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
        '        SumatoriaImpuesto4 = SumatoriaImpuesto4 + (Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
        '    End If
        'Next

        'If SumatoriaImpuesto4 > 0 Then
        '    writer.WriteStartElement("cac:TaxSubtotal")

        '    writer.WriteStartElement("cbc:TaxableAmount")
        '    writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        '    writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.), 2).ToString("0.00", CultureInfo.InvariantCulture))
        '    writer.WriteEndElement()

        '    writer.WriteStartElement("cbc:TaxAmount")
        '    writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        '    writer.WriteString(Math.Round(SumatoriaImpuesto4, 2).ToString("0.00", CultureInfo.InvariantCulture))
        '    writer.WriteEndElement()

        '    writer.WriteStartElement("cac:TaxCategory")

        '    writer.WriteStartElement("cbc:ID")
        '    writer.WriteAttributeString("schemeID", "UN/ECE 5305")
        '    writer.WriteAttributeString("schemeName", "Tax Category Identifier")
        '    writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
        '    writer.WriteString("Z")
        '    writer.WriteEndElement()

        '    writer.WriteStartElement("cac:TaxScheme")

        '    writer.WriteStartElement("cbc:ID")
        '    writer.WriteAttributeString("schemeID", "UN/ECE 5153")
        '    writer.WriteAttributeString("schemeAgencyID", "6")
        '    writer.WriteString("9996")
        '    writer.WriteEndElement()

        '    writer.WriteElementString("cbc:Name", "GRATUITO")
        '    writer.WriteElementString("cbc:TaxTypeCode", "FRE")

        '    writer.WriteEndElement()

        '    writer.WriteEndElement()

        '    writer.WriteEndElement()
        'End If

        Dim SumatoriaTotal5 As Double
        Dim SumatoriaImpuesto5 As Double
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
            If Double.Parse(item.t19_totalitem) > 0 Then
                SumatoriaTotal5 = SumatoriaTotal5 + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
                SumatoriaImpuesto5 = SumatoriaImpuesto5 + (Double.Parse(item.t19_totalitem) * Double.Parse(item.t13_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es ISC
            End If
        Next
        If SumatoriaImpuesto5 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t20_totalvalorventasgravadas), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto5, 2).ToString("0.00", CultureInfo.InvariantCulture))
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

        Dim SumatoriaTotal6 As Double
        Dim SumatoriaImpuesto6 As Double
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle
            If item.t18_tipafectacion_c07 = "40" Then
                SumatoriaTotal6 = SumatoriaTotal6 + (Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad))
                SumatoriaImpuesto6 = SumatoriaImpuesto6 + (Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
            End If
        Next

        If SumatoriaTotal6 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t22_totalvalorventaexonerada), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(Math.Round(SumatoriaImpuesto6, 2).ToString("0.00", CultureInfo.InvariantCulture))
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

        'End If
        writer.WriteEndElement()


        writer.WriteStartElement("cac:LegalMonetaryTotal")

        'Parte30
        writer.WriteStartElement("cbc:LineExtensionAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t20_totalvalorventasgravadas) - Double.Parse(NotaCreditoBE.t28_totaldescuentos), 2).ToString("0.00", CultureInfo.InvariantCulture))
        writer.WriteEndElement()
        'Parte30

        'Parte31
        writer.WriteStartElement("cbc:TaxInclusiveAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t29_totalimporte) - Double.Parse(NotaCreditoBE.t27_totalotroscargos), 2).ToString("0.00", CultureInfo.InvariantCulture))
        writer.WriteEndElement()
        'Parte31

        'Parte32
        writer.WriteStartElement("cbc:AllowanceTotalAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(NotaCreditoBE.t28_totaldescuentos)
        writer.WriteEndElement()
        'Parte32

        'Parte33
        writer.WriteStartElement("cbc:ChargeTotalAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(NotaCreditoBE.t27_totalotroscargos)
        writer.WriteEndElement()
        'Parte33

        'Parte34
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(NotaCreditoBE.t29_totalimporte) + Double.Parse(NotaCreditoBE.t28_totaldescuentos), 2).ToString("0.00", CultureInfo.InvariantCulture)) 'tener en cuenta este campo es el importe total de la venta
        writer.WriteEndElement()
        'Parte34

        writer.WriteEndElement()

        Dim orden As Integer = 1
        For Each item As NotaCreditoItem In NotaCreditoBE.Detalle

            writer.WriteStartElement("cac:CreditNoteLine")
            writer.WriteElementString("cbc:ID", orden.ToString())

            writer.WriteStartElement("cbc:CreditedQuantity")
            writer.WriteAttributeString("unitCode", "NIU")
            writer.WriteAttributeString("unitCodeListID", "UN/ECE rec 20")
            writer.WriteAttributeString("unitCodeListAgencyName", "United Nations Economic Commission for Europe")
            writer.WriteString(item.t13_cantidad)
            writer.WriteEndElement()


            writer.WriteStartElement("cbc:LineExtensionAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02) 'Nuevo Campo Agregar Catalogo 06
            writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

            writer.WriteStartElement("cac:PricingReference")
            writer.WriteStartElement("cac:AlternativeConditionPrice")

            writer.WriteStartElement("cbc:PriceAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(item.t17_preciounitario)
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:PriceTypeCode")
            'writer.WriteAttributeString("listName", "SUNAT:Indicador de Tipo de Precio")
            writer.WriteAttributeString("listName", "Tipo de Precio")
            writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
            writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16")
            writer.WriteString(item.t17_tipprecio_c16) ' nuevo campo agregar si es transferencia gratuita cambiar 01 por 02 
            writer.WriteEndElement()

            writer.WriteEndElement()
            writer.WriteEndElement()

            'If Double.Parse(item.t18_totalitem) > 0 Then

            writer.WriteStartElement("cac:TaxTotal")

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            'writer.WriteString(Math.Round((Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
            writer.WriteString(Math.Round((Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)) + (Double.Parse(item.t19_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))

            writer.WriteEndElement()

            'Parte42
            If item.t18_tipafectacion_c07 = "10" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", IIf(Porcentaje <> "", Porcentaje, 18)) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                ' writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t18_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString(item.t18_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t18_nomtributo)
                writer.WriteElementString("cbc:TaxTypeCode", item.t18_tiptributointernacional)

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If
            'Parte42

            If item.t18_tipafectacion_c07 = "20" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("E")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                ' writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t18_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9997") 'item.t18_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "EXO")
                writer.WriteElementString("cbc:TaxTypeCode", "VAT")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t18_tipafectacion_c07 = "21" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t18_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
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
                writer.WriteString(item.t18_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9996") 'item.t18_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "GRA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t18_tipafectacion_c07 = "30" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString("0.00")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
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
                writer.WriteString(item.t18_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9998") 'item.t18_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "INA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If item.t18_tipafectacion_c07 = "40" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString("0.00")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
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
                writer.WriteString(item.t18_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9995") 'item.t18_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "EXP")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            If Double.Parse(item.t19_totalitem) > 0 Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t16_valorunitario) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t19_totalitem) * Double.Parse(item.t13_cantidad)), 2).ToString("0.00", CultureInfo.InvariantCulture))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()

                'Dim Porcentaje As String = SistemaDAO.GetProcentajeISCCentral()
                writer.WriteElementString("cbc:Percent", item.t53_porcentajeISC) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteElementString("cbc:TierRange", item.t19_tipsistema_c08)

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteElementString("cbc:ID", item.t19_tiptributo_c05)

                'writer.WriteStartElement("cbc:ID")
                'writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                'writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                'writer.WriteString(item.t17_tiptributo_c05)
                'writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t19_nomtributo)
                writer.WriteElementString("cbc:TaxTypeCode", item.t19_tiptributointernacional)

                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndElement()
            End If

            writer.WriteEndElement()

            'End If
            'isc
            ''If Double.Parse(item.t19_totalitem) > 0 Then

            ''    writer.WriteStartElement("cac:TaxTotal")

            ''    writer.WriteStartElement("cbc:TaxAmount")
            ''    writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            ''    writer.WriteString(item.t19_totalitem)
            ''    writer.WriteEndElement()

            ''    'Parte43
            ''    If Double.Parse(item.t19_totalitem) > 0 Then
            ''        writer.WriteStartElement("cac:TaxSubtotal")

            ''        writer.WriteStartElement("cbc:TaxableAmount")
            ''        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            ''        writer.WriteString(item.t17_preciounitario)
            ''        writer.WriteEndElement()

            ''        writer.WriteStartElement("cbc:TaxAmount")
            ''        writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            ''        writer.WriteString(item.t19_totalitem)
            ''        writer.WriteEndElement()

            ''        writer.WriteStartElement("cac:TaxCategory")

            ''        writer.WriteStartElement("cbc:ID")
            ''        writer.WriteAttributeString("schemeID", "UN/ECE 5305")
            ''        writer.WriteAttributeString("schemeName", "Tax Category Identifier")
            ''        writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
            ''        writer.WriteString("S")
            ''        writer.WriteEndElement()

            ''        'Dim Porcentaje As String = SistemaDAO.GetProcentajeISCCentral()
            ''        writer.WriteElementString("cbc:Percent", item.t53_porcentajeISC) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

            ''        writer.WriteStartElement("cbc:TaxExemptionReasonCode")
            ''        writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
            ''        writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
            ''        writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
            ''        writer.WriteString(item.t18_tipafectacion_c07)
            ''        writer.WriteEndElement()

            ''        writer.WriteElementString("cbc:TierRange", item.t19_tipafectacion_c07)

            ''        writer.WriteStartElement("cac:TaxScheme")

            ''        writer.WriteElementString("cbc:ID", item.t19_tiptributo_c05)
            ''        writer.WriteElementString("cbc:Name", item.t19_nomtributo)
            ''        writer.WriteElementString("cbc:TaxTypeCode", item.t19_tiptributointernacional)

            ''        writer.WriteEndElement()

            ''        writer.WriteEndElement()

            ''        writer.WriteEndElement()
            ''    End If
            ''    'Parte43
            ''    writer.WriteEndElement()

            ''End If

            writer.WriteStartElement("cac:Item")
            writer.WriteStartElement("cbc:Description")
            writer.WriteCData(item.t15_descripcion)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
            writer.WriteEndElement()

            writer.WriteStartElement("cac:SellersItemIdentification")
            writer.WriteElementString("cbc:ID", item.t14_codproducto)
            writer.WriteEndElement()

            writer.WriteEndElement()


            writer.WriteStartElement("cac:Price")
            'writer.WriteElementString("cbc:ID", item.t34_codproducto)

            writer.WriteStartElement("cbc:PriceAmount")
            writer.WriteAttributeString("currencyID", NotaCreditoBE.t30_tipomoneda_c02)
            writer.WriteString(item.t16_valorunitario)
            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()

            orden = orden + 1
        Next
        writer.WriteEndElement()
        'ORIGEN

        writer.WriteEndDocument()
        writer.Close()
        writer.Dispose()

        'Se usa para reemplazar la cadena de minusculas a mayusculas
        Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
        Data = Data.Replace("iso-8859-1", "ISO-8859-1")
        File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)

        'Se guarda la ruta del archivo xml
        SaveRutaArchivoXML(IDNotaCredito, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se retorna el nombre de archivo
        'Return RutaArchivo & NombreArchivo & ExtensionArchivoXML


        'Se guarda la ruta del archivo xml
        'SaveRutaArchivoXML(IDFactura, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se retorna el nombre de archivo
        ' Return RutaArchivo & NombreArchivo & ExtensionArchivoXML

        'Se firma el comprobante
        Dim FirmaXMLDAO As New FirmaXMLDAO

        FirmaXMLDAO.Create(EmisorBE, RutaArchivo & NombreArchivo & ExtensionArchivoXML, "07")


    End Sub



    'Public Function SignatureXML(IDNotaCredito As Int32) As String

    '    'Se carga los datos el emisor
    '    EmisorBE = EmisorDAO.GetByID(1)

    '    Dim RutaCertificado As String = EmisorBE.RutaCarpetaArchivosCertificados
    '    Dim ClaveCertificado As String = EmisorBE.ClaveCertificado
    '    Dim Result As String = String.Empty

    '    'Se obtiene la entidad
    '    NotaCreditoBE = Me.GetByID(IDNotaCredito)

    '    Try
    '        Dim local_xmlArchivo As String = NotaCreditoBE.RutaComprobanteXML
    '        Dim local_nombreXML As String = System.IO.Path.GetFileName(local_xmlArchivo)
    '        Dim local_typoDocumento As String = "07"

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
    '        Me.SaveValorFirma(IDNotaCredito, ValorFirma)

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    '    Return Result
    'End Function
    Public Function ZipXML(IDNotaCredito As Int32) As String
        'Se carga los datos el emisor
        EmisorBE = EmisorDAO.GetByID(1)

        'Se obtiene la entidad
        NotaCreditoBE = Me.GetByID(IDNotaCredito)

        'Se obtiene el nombre del archivo zip
        Dim RutaArchivoZIP As String = Path.ChangeExtension(NotaCreditoBE.RutaComprobanteXML, "zip")

        Using zipToOpen As FileStream = New FileStream(RutaArchivoZIP, FileMode.Create)
            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(Path.GetFileName(NotaCreditoBE.RutaComprobanteXML))
                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(NotaCreditoBE.RutaComprobanteXML))
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

    Public Function GetByID(IDNotaCredito As Int32) As NotaCreditoBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim NotaCreditoBE As New NotaCreditoBE
        Dim NotaCreditoItems As New List(Of NotaCreditoItem)

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_get_id"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read
                    With NotaCreditoBE
                        .idnotacredito = dr.ReadNullAsEmptyString("idnotacredito")
                        .idcomprobante = dr.ReadNullAsEmptyString("idcomprobante")
                        .t01_fecemision = dr.ReadNullAsEmptyString("t01_fecemision")
                        .t07_sernumdocafectado = dr.ReadNullAsEmptyString("t07_sernumdocafectado")
                        .t07_tipnotacredito_c09 = dr.ReadNullAsEmptyString("t07_tipnotacredito_c09")
                        .t08_numcorrelativo = dr.ReadNullAsEmptyString("t08_numcorrelativo")
                        .t09_numdocadquiriente = dr.ReadNullAsEmptyString("t09_numdocadquiriente")
                        .t09_tipdocadquiriente_c06 = dr.ReadNullAsEmptyString("t09_tipdocadquiriente_c06")
                        .t10_nomadquiriente = dr.ReadNullAsEmptyString("t10_nomadquiriente")
                        .t11_motivo = dr.ReadNullAsEmptyString("t11_motivo")
                        .t20_tipmonto_c14 = dr.ReadNullAsEmptyString("t20_tipmonto_c14")
                        .t20_totalvalorventasgravadas = dr.ReadNullAsEmptyString("t20_totalvalorventasgravadas")
                        .t21_tipmonto_c14 = dr.ReadNullAsEmptyString("t21_tipmonto_c14")
                        .t21_totalvalorventainafectas = dr.ReadNullAsEmptyString("t21_totalvalorventainafectas")
                        .t22_tipmonto_c14 = dr.ReadNullAsEmptyString("t22_tipmonto_c14")
                        .t22_totalvalorventaexonerada = dr.ReadNullAsEmptyString("t22_totalvalorventaexonerada")
                        .t24_totaligv = dr.ReadNullAsEmptyString("t24_totaligv")
                        .t24_subtotaligv = dr.ReadNullAsEmptyString("t24_subtotaligv")
                        .t24_tiptributo_c05 = dr.ReadNullAsEmptyString("t24_tiptributo_c05")
                        .t24_nomtributo_c05 = dr.ReadNullAsEmptyString("t24_nomtributo_c05")
                        .t24_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t24_tiptributointernacional_c05")
                        .t25_totalisc = dr.ReadNullAsEmptyString("t25_totalisc")
                        .t25_subtotalisc = dr.ReadNullAsEmptyString("t25_subtotalisc")
                        .t25_tiptributo_c05 = dr.ReadNullAsEmptyString("t25_tiptributo_c05")
                        .t25_nomtributo_c05 = dr.ReadNullAsEmptyString("t25_nomtributo_c05")
                        .t25_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t25_tiptributointernacional_c05")
                        .t26_totalotrostributos = dr.ReadNullAsEmptyString("t26_totalotrostributos")
                        .t26_subtotalotrostributos = dr.ReadNullAsEmptyString("t26_subtotalotrostributos")
                        .t26_tiptributo_c05 = dr.ReadNullAsEmptyString("t26_tiptributo_c05")
                        .t26_nomtributo_c05 = dr.ReadNullAsEmptyString("t26_nomtributo_c05")
                        .t26_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t26_tiptributointernacional_c05")
                        .t27_totalotroscargos = dr.ReadNullAsEmptyString("t27_totalotroscargos")
                        .t28_tipmonto_c14 = dr.ReadNullAsEmptyString("t28_tipmonto_c14")
                        .t28_totaldescuentos = dr.ReadNullAsEmptyString("t28_totaldescuentos")
                        .t29_totalimporte = dr.ReadNullAsEmptyString("t29_totalimporte")
                        .t30_tipomoneda_c02 = dr.ReadNullAsEmptyString("t30_tipomoneda_c02")
                        .t31_sernumdocmodifica = dr.ReadNullAsEmptyString("t31_sernumdocmodifica")
                        .t32_tipdoc_c01 = dr.ReadNullAsEmptyString("t32_tipdoc_c01")
                        .t33_sernumguia = dr.ReadNullAsEmptyString("t33_sernumguia")
                        .t33_tipdoc_c01 = dr.ReadNullAsEmptyString("t33_tipdoc_c01")
                        .t33_numdocrelacionado = dr.ReadNullAsEmptyString("t33_numdocrelacionado")
                        .t33_tipdoc_c12 = dr.ReadNullAsEmptyString("t33_tipdoc_c12")
                        .t36_versionubl = dr.ReadNullAsEmptyString("t36_versionubl")
                        .t37_versiondoc = dr.ReadNullAsEmptyString("t37_versiondoc")

                        .estado = dr.ReadNullAsEmptyString("estado")
                        .fechahorasistemaexterno = dr.ReadNullAsEmptyDate("fechahorasistemaexterno")
                        .fechahorasunat = dr.ReadNullAsEmptyDate("fechahorasunat")
                        .idusuario = dr.ReadNullAsNumeric("IDUsuario")
                        .fecharegistro = dr.ReadNullAsEmptyDate("fecharegistro")
                        .ValorFirma = dr.ReadNullAsEmptyString("digestvalue")
                        .RutaComprobanteXML = dr.ReadNullAsEmptyString("archivoxml")
                        .RutaComprobanteZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")
                        .RutaComprobantePDF = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "pdf")
                        .RutaRespuestaSunatXML = dr.ReadNullAsEmptyString("rutarespuestasunatxml")
                        .RutaRespuestaSunatZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")
                        .Observacion = dr.ReadNullAsEmptyString("observacion")
                        .CodigoRespuesta = dr.ReadNullAsEmptyString("codigorespuesta")
                        .SerieComprobante = dr.ReadNullAsEmptyString("t08_numcorrelativo").ToString.Substring(0, 4)
                        .NumeroComprobante = Convert.ToInt32(dr.ReadNullAsEmptyString("t08_numcorrelativo").ToString.Substring(5, 8))
                        .DireccionAdquiriente = dr.ReadNullAsEmptyString("direccionadquiriente")
                        .EmailAdquiriente = dr.ReadNullAsEmptyString("emailadquiriente")
                        .HoraEmision = dr.ReadNullAsEmptyString("t01_horaemision")
                        .descripcionleyenda = dr.ReadNullAsEmptyString("t38_descripcionleyenda")
                    End With
                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        Dim BE As New NotaCreditoItem
                        With BE
                            .idnotacreditodetalle = dr.ReadNullAsEmptyString("idnotacreditodetalle")
                            .idnotacredito = dr.ReadNullAsEmptyString("idnotacredito")
                            .t12_tipunidadmedida_c03 = dr.ReadNullAsEmptyString("t12_tipunidadmedida_c03")
                            .t13_cantidad = dr.ReadNullAsEmptyString("t13_cantidad")
                            .t14_codproducto = dr.ReadNullAsEmptyString("t14_codproducto")
                            .t15_descripcion = dr.ReadNullAsEmptyString("t15_descripcion")
                            .t16_valorunitario = dr.ReadNullAsEmptyString("t16_valorunitario")
                            .t17_preciounitario = dr.ReadNullAsEmptyString("t17_preciounitario")
                            .t17_tipprecio_c16 = dr.ReadNullAsEmptyString("t17_tipprecio_c16")
                            .t18_totalitem = dr.ReadNullAsEmptyString("t18_totalitem")
                            .t18_subtotalitem = dr.ReadNullAsEmptyString("t18_subtotalitem")
                            .t18_tipafectacion_c07 = dr.ReadNullAsEmptyString("t18_tipafectacion_c07")
                            .t18_tiptributo_c05 = dr.ReadNullAsEmptyString("t18_tiptributo_c05")
                            .t18_nomtributo = dr.ReadNullAsEmptyString("t18_nomtributo")
                            .t18_tiptributointernacional = dr.ReadNullAsEmptyString("t18_tiptributointernacional")
                            .t19_totalitem = dr.ReadNullAsEmptyString("t19_totalitem")
                            .t19_subtotalitem = dr.ReadNullAsEmptyString("t19_subtotalitem")
                            .t19_tipafectacion_c07 = dr.ReadNullAsEmptyString("t19_tipafectacion_c07")
                            .t19_tiptributo_c05 = dr.ReadNullAsEmptyString("t19_tiptributo_c05")
                            .t19_nomtributo = dr.ReadNullAsEmptyString("t19_nomtributo")
                            .t19_tiptributointernacional = dr.ReadNullAsEmptyString("t19_tiptributointernacional")
                            .t23_valorventaitem = dr.ReadNullAsEmptyString("t23_valorventaitem")
                            .t34_numordenitem = dr.ReadNullAsEmptyString("t34_numordenitem")
                            .t35_valorreferenciaunitario = dr.ReadNullAsEmptyString("t35_valorreferenciaunitario")
                            .t35_tipprecio_c16 = dr.ReadNullAsEmptyString("t35_tipprecio_c16")
                        End With

                        NotaCreditoItems.Add(BE)
                    End While
                End If

                NotaCreditoBE.Detalle = NotaCreditoItems
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try

        Return NotaCreditoBE
    End Function
    Public Function GetByReporteID(IDNotaCredito As Int32) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_rpt_id"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
    Public Function GetByIDXML(IDNotaCredito As Int32) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As System.Xml.XmlReader = Nothing
        Dim doc As New XmlDocument()
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_get_id_xml"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
            .CommandText = "coe_nota_credito_get_all"
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

        Dim NotaCreditoBE As New NotaCreditoBE
        NotaCreditoBE = Me.GetByID(IDComprobante)

        'Se establece la carpeta donde se almacena
        RutaCloud = "/r" & EmisorBE.NumeroRUC & "/"

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_web_ins"

            With .Parameters
                .Add("@IDComprobanteWeb", SqlDbType.Int).Direction = ParameterDirection.Output
                .Add("@IDComprobante", SqlDbType.Int).Value = NotaCreditoBE.idnotacredito
                .Add("@Tipo", SqlDbType.Char, 2).Value = NotaCreditoBE.TipoComprobante
                .Add("@NumeroRUC", SqlDbType.VarChar, 15).Value = EmisorBE.NumeroRUC
                .Add("@NumeroCorrelativo", SqlDbType.VarChar, 20).Value = NotaCreditoBE.t08_numcorrelativo
                .Add("@FechaEmision", SqlDbType.VarChar, 10).Value = NotaCreditoBE.t01_fecemision
                .Add("@Importe", SqlDbType.Decimal).Value = NotaCreditoBE.t29_totalimporte
                .Add("@RutaArchivoPDF", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(NotaCreditoBE.RutaComprobantePDF)
                .Add("@RutaArchivoXML", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(NotaCreditoBE.RutaComprobanteXML)
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

        Dim NotaCreditoBE As New NotaCreditoBE
        NotaCreditoBE = Me.GetByID(IDComprobante)

        If ToolsAzure.SaveStorageFiles(EmisorBe.ConexionStorageCloud, "r" & EmisorBe.NumeroRUC, NotaCreditoBE.RutaComprobanteXML) Then
            If ToolsAzure.SaveStorageFiles(EmisorBe.ConexionStorageCloud, "r" & EmisorBe.NumeroRUC, NotaCreditoBE.RutaComprobantePDF) Then
                Result = True
            End If
        End If

        Return Result
    End Function
    Public Function SaveRutaArchivoXML(IDNotaCredito As Int32, RutaCarpetaXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_upd_ruta"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
    Public Function SaveRutaArchivoCdrXML(IDNotaCredito As Int32, RutaCarpetaCdrXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_upd_ruta_cdr"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
    Public Function SaveValorFirma(IDNotaCredito As Int32, ValorFirma As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_upd_firma"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
            .Parameters.Add("@valorfirma", SqlDbType.VarChar, 500).Value = ValorFirma
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
    Public Function SaveConstanciaRecepcionZIP(IDNotaCredito As Int32, ContenidoArchivoZIP As Byte()) As Boolean
        Dim Result As Boolean = False
        Dim NombreArchivo As String = String.Empty

        Try
            'Se obtiene la entidad
            NotaCreditoBE = Me.GetByID(IDNotaCredito)

            'Se obtiene el nombre del archivo xml, sin extension
            NombreArchivo = Path.GetFileNameWithoutExtension(NotaCreditoBE.RutaComprobanteXML)

            'Se agrega la letra R al nombre (Respuesta) y se coloca la extension .zip
            NombreArchivo = "R-" & Path.ChangeExtension(NombreArchivo, ".zip")

            'Se concatena la ruta y el nuevo nombre del archivo zip
            NombreArchivo = Path.GetDirectoryName(NotaCreditoBE.RutaComprobanteXML) & "\" & NombreArchivo

            'Se guarda el archivo zip que envia la sunat
            System.IO.File.WriteAllBytes(NombreArchivo, ContenidoArchivoZIP)

            'Se descomprime el archivo ZIP y extrae el XML
            Me.UnZipXML(NombreArchivo)

            'Se guarda el nombre del archivo de la constancia de recepcion CDR SUNAT
            Me.SaveRutaArchivoCdrXML(IDNotaCredito, Path.ChangeExtension(NombreArchivo, ".xml"))

            'Se lee el contenido del archivo XML y se lee contenido
            Me.SaveConstanciaRecepcionXML(IDNotaCredito)

            Result = True
        Catch ex As Exception
            Throw New Exception("No se guardo el archivo de respuesta de la SUNAT. " & ex.Message)
        End Try

        Return Result
    End Function
    Public Function SaveConstanciaRecepcionXML(IDNotaCredito As Int32) As Boolean
        Dim Result As Boolean = False
        Dim Valor As String = String.Empty
        Dim CodigoRespuesta As String = String.Empty
        Dim NumeroComprobante As String = String.Empty
        Dim Descripcion As String = String.Empty

        'Se obtiene la entidad
        NotaCreditoBE = Me.GetByID(IDNotaCredito)

        'Se obtiene el archivo XML
        Dim ReaderXML As XmlTextReader = New XmlTextReader(NotaCreditoBE.RutaRespuestaSunatXML)

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
            .CommandText = "coe_nota_credito_upd_estado"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
            .CommandText = "coe_nota_credito_upd_excepcion"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = BE.IDComprobante
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
    Public Function SaveEstadoWeb(IDNotaCredito As Int32, IDEstadoWeb As eEstadoWeb, FechaWeb As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_nota_credito_estado_web"
            .Parameters.Add("@idnotacredito", SqlDbType.Int).Value = IDNotaCredito
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
