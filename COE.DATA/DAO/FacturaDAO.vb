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

#End Region
Public Enum eEstadoSunat
    FirmarXML = 1
    PendienteEnvio = 2
    Aceptado = 3
    Rechazado = 4
    EnProceso = 5
    ControlInterno = 6
    PendienteCDR = 7
End Enum
Public Enum eEstadoFactura
    Anulado = 2
    Activo = 1
End Enum

Public Class FacturaDAO
    Public Property IDFactura As Int32
    Public Property BE As New FacturaBE
    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim EmisorBE As New EmisorBE
    Dim FacturaBE As New FacturaBE

    Public Sub CreateFileXML21(IDFactura As Int32)

        'Se obtiene datos del emisor
        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"

        'Se obtiene la entidad
        FacturaBE = Me.GetByID(IDFactura)

        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-" & FacturaBE.t07_tipdoc_c01 & "-" & FacturaBE.t08_numcorrelativo

        'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
        'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'ORIGEN
        'Se crea el nodo raiz
        writer.WriteStartElement("Invoice")
        writer.WriteAttributeString("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:ccts", "urn:un:unece:uncefact:documentation:2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
        'writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")
        'ext:UBLExtensions
        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        'La version del UBLVersionID es 2.1
        'Parte2
        writer.WriteElementString("cbc:UBLVersionID", FacturaBE.t36_versionubl)
        'Parte2

        'Parte3
        writer.WriteElementString("cbc:CustomizationID", FacturaBE.t37_versiondoc)
        'Parte3

        'Parte4
        writer.WriteStartElement("cbc:ProfileID")
        writer.WriteAttributeString("schemeName ", "SUNAT:Identificador de Tipo de Operación")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo17")
        writer.WriteString(FacturaBE.t51_tipoperacion_c17 + IIf(FacturaBE.t51_tipoperacion_c17 = "01", "01", "02")) 'Agregar Nuevo Campo si en algun momento se considera agregar mas tipos de operación se debe agregar un campo en FACTURA
        writer.WriteEndElement()
        'Parte4

        'Parte5
        writer.WriteElementString("cbc:ID", FacturaBE.t08_numcorrelativo)
        'Parte5
        'Parte6
        writer.WriteElementString("cbc:IssueDate", FacturaBE.t01_fecemision)
        'Parte6
        'Parte7
        writer.WriteElementString("cbc:IssueTime", FacturaBE.HoraEmision) 'hh-mm-ss.0z agregar dato de hora
        'Parte7

        'writer.WriteElementString("cbc:DueDate", FacturaBE.t01_fecemision) ' Agregar Fecha Vencimiento Opcional
        'writer.WriteElementString("cbc:InvoiceTypeCode", FacturaBE.t07_tipdoc_c01)

        'Tools.TagNodoAtributoValorValor(writer, "cbc:InvoiceTypeCode", "listAgencyName", "PE:SUNAT", FacturaBE.t07_tipdoc_c01)
        'Parte9 cbc:InvoiceTypeCode
        writer.WriteStartElement("cbc:InvoiceTypeCode")
        writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("listName", "Tipo de Documento")
        writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01")
        writer.WriteAttributeString("listID", FacturaBE.t51_tipoperacion_c17 + IIf(FacturaBE.t51_tipoperacion_c17 = "01", "01", "02"))
        writer.WriteAttributeString("name", "Tipo de Operacion")
        writer.WriteAttributeString("listSchemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo51")
        writer.WriteString(FacturaBE.t07_tipdoc_c01)
        writer.WriteEndElement()

        ''If (FacturaBE.tOrdenCompra <> "") Then
        ''    writer.WriteStartElement("cac:OrderReference")
        ''    writer.WriteElementString("cbc:ID", FacturaBE.tOrdenCompra)
        ''    writer.WriteEndElement()
        ''End If
        'Parte9 cbc:InvoiceTypeCode

        ' writer.WriteElementString("cbc:Note", FacturaBE.t01_fecemision) ' Agregar Fecha Vencimiento
        'Parte10 cbc:Note
        writer.WriteStartElement("cbc:Note")
        writer.WriteAttributeString("languageLocaleID", "1000")
        writer.WriteString(FacturaBE.t31_descripcionleyenda)
        writer.WriteEndElement()
        'Parte10 cbc:Note

        'Parte11 cbc:DocumentCurrencyCode
        writer.WriteStartElement("cbc:DocumentCurrencyCode")
        writer.WriteAttributeString("listID", "ISO 4217 Alpha")
        writer.WriteAttributeString("listName", "Currency")
        writer.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        writer.WriteString(FacturaBE.t28_tipmoneda_c02)
        writer.WriteEndElement()
        'Parte11 cbc:DocumentCurrencyCode

        'writer.WriteElementString("cbc:LineCountNumeric", FacturaBE.Detalle.Count)

        'Parte12
        'Parte12

        'Parte13
        'Parte13

        'Este bloque es si tiene orden de compra 'Se bloquea por el momento
        'If (FacturaBE.tOrdenCompra <> "") Then
        '    writer.WriteStartElement("cac:OrderReference")
        '    writer.WriteElementString("cbc:ID", FacturaBE.tOrdenCompra)
        '    writer.WriteEndElement()
        'End If
        'If (FacturaBE.t29_sernumguia <> "") Then
        '    writer.WriteStartElement("cac:DespatchDocumentReference")
        '    writer.WriteElementString("cbc:ID", FacturaBE.t29_sernumguia)
        '    writer.WriteStartElement("cbc:DocumentTypeCode")
        '    writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
        '    writer.WriteAttributeString("listName", "SUNAT:Identificador de guía relacionada")
        '    writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo12")
        '    writer.WriteString("09")
        '    writer.WriteEndElement()

        '    writer.WriteEndElement()
        'End If

        'Parte7
        'writer.WriteStartElement("cac:Signature")
        'writer.WriteEndElement()
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
        'Parte7

        ''Parte8
        'writer.WriteStartElement("cac:Signature")
        'writer.WriteEndElement()
        ''Parte8


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
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(EmisorBE.NumeroRUC)
        writer.WriteEndElement()
        'Parte16
        'Parte17
        'writer.WriteStartElement("cac:RegistrationAddress")
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

        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteStartElement("cbc:ID")
        writer.WriteAttributeString("schemeID", IIf(FacturaBE.t09_tipdoc_c06 = "-", "0", FacturaBE.t09_tipdoc_c06))
        writer.WriteAttributeString("schemeName", "Documento de Identidad")
        writer.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        writer.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        writer.WriteString(FacturaBE.t09_numdoc)
        writer.WriteEndElement()
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(FacturaBE.t10_nomadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        writer.WriteEndElement()
        writer.WriteEndElement()


        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(FacturaBE.t10_nomadquiriente)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
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
        writer.WriteElementString("cbc:Line", FacturaBE.DireccionAdquiriente)
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

        'Parte20 - Esto no corresponde
        'Parte20

        'Parte21 - Descuento Global
        If Double.Parse(FacturaBE.t50_descuentosglobales) > 0 Then
            writer.WriteStartElement("cac:AllowanceCharge")

            writer.WriteElementString("cbc:ChargeIndicator", "False")

            writer.WriteStartElement("cbc:AllowanceChargeReasonCode", "00")
            writer.WriteStartElement("cbc::MultiplierFactorNumeric", "") ' Nuevo Campo % de descuento 

            writer.WriteStartElement("cbc:Amount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(FacturaBE.t50_descuentosglobales)
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:BaseAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t50_descuentosglobales) + Double.Parse(FacturaBE.t27_totalimporte), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteEndElement()
            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte21


        writer.WriteStartElement("cac:TaxTotal")
        'If Double.Parse(FacturaBE.t22_totaligv) + Double.Parse(FacturaBE.t23_totalisc) + Double.Parse(FacturaBE.t24_totalotrostributos) > 0 Then
        'Parte22
        writer.WriteStartElement("cbc:TaxAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(String.Format("{0:0.00}", (Decimal.Parse(FacturaBE.t22_totaligv) + Decimal.Parse(FacturaBE.t23_totalisc) + Decimal.Parse(FacturaBE.t24_totalotrostributos))))
        writer.WriteEndElement()
        'Parte22
        'Parte23
        Dim SumatoriaTotal As Double
        Dim SumatoriaImpuesto As Double
        For Each item As FacturaItem In FacturaBE.Detalle
            If item.t16_tipafectacion_c07 = "10" Then ' Or item.t16_tipafectacion_c07 = "11" Or item.t16_tipafectacion_c07 = "12" Or item.t16_tipafectacion_c07 = "13" Or item.t16_tipafectacion_c07 = "14" Or item.t16_tipafectacion_c07 = "15" Or item.t16_tipafectacion_c07 = "16" Or item.t16_tipafectacion_c07 = "17" Then
                SumatoriaTotal = SumatoriaTotal + (Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad))

                'SumatoriaImpuesto = SumatoriaImpuesto + (Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad))
                'Se calcula el IGV
                SumatoriaImpuesto = SumatoriaImpuesto + Double.Parse(item.t16_totalitem)
            End If
        Next

        If SumatoriaImpuesto > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t18_totalvalorventagravadas), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
            writer.WriteElementString("cbc:Name", FacturaBE.t22_nomtributo_c05)
            writer.WriteElementString("cbc:TaxTypeCode", FacturaBE.t22_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte23

        'Parte24
        Dim SumatoriaTotal2 As Double
        Dim SumatoriaImpuesto2 As Double
        For Each item As FacturaItem In FacturaBE.Detalle
            If item.t16_tipafectacion_c07 = "20" Then
                SumatoriaTotal2 = SumatoriaTotal2 + (Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad))
                ' SumatoriaImpuesto2 = SumatoriaImpuesto2 + (Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es EXONERADO
                'Se calcula el IGV por Linea
                SumatoriaImpuesto2 = SumatoriaImpuesto2 + Double.Parse(item.t16_totalitem)  ' nuevo campo modificar el sp lectura para que cambie este dato cuando es EXONERADO
            End If
        Next

        If SumatoriaTotal2 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t20_totalvalorventaexonerada), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
            writer.WriteElementString("cbc:TaxTypeCode", FacturaBE.t22_tiptributointernacional_c05)

            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()
        End If
        'Parte24
        'Parte26
        Dim SumatoriaTotal3 As Double
        Dim SumatoriaImpuesto3 As Double
        For Each item As FacturaItem In FacturaBE.Detalle
            If item.t16_tipafectacion_c07 = "30" Then
                SumatoriaTotal3 = SumatoriaTotal3 + Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)
                'SumatoriaImpuesto3 = SumatoriaImpuesto3 + (Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
                'Se calcula el IGV Por linea
                SumatoriaImpuesto3 = SumatoriaImpuesto3 + Double.Parse(item.t16_totalitem)  ' nuevo campo modificar el sp lectura para que cambie este dato cuando es Exonerado Gratuito
            End If
        Next

        If SumatoriaTotal3 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t19_totalvalorventainafectas), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
        For Each item As FacturaItem In FacturaBE.Detalle
            If item.t16_tipafectacion_c07 = "21" Then
                SumatoriaTotal4 = SumatoriaTotal4 + Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)
                'SumatoriaImpuesto4 = SumatoriaImpuesto4 + (Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
                'Se calcula el IGV
                SumatoriaImpuesto4 = SumatoriaImpuesto4 + Double.Parse(item.t16_totalitem)  ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
            End If
        Next

        If SumatoriaTotal4 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t49_totalvalorventaoperacionesgratuitas), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
        For Each item As FacturaItem In FacturaBE.Detalle
            If Double.Parse(item.t17_totalitem) > 0 Then
                SumatoriaTotal5 = SumatoriaTotal5 + Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto5 = SumatoriaImpuesto5 + Double.Parse(item.t17_totalitem) * Double.Parse(item.t12_cantidad) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es ISC
            End If
        Next
        If SumatoriaImpuesto5 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(SumatoriaTotal5, 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
        For Each item As FacturaItem In FacturaBE.Detalle
            If item.t16_tipafectacion_c07 = "40" Then
                SumatoriaTotal6 = SumatoriaTotal6 + Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)
                SumatoriaImpuesto6 = SumatoriaImpuesto6 + (Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)) ' nuevo campo modificar el sp lectura para que cambie este dato cuando es INAFECTA
            End If
        Next

        If SumatoriaTotal6 > 0 Then
            writer.WriteStartElement("cac:TaxSubtotal")

            writer.WriteStartElement("cbc:TaxableAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(Math.Round(Double.Parse(FacturaBE.t20_totalvalorventaexonerada), 2).ToString("0.00"))
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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

        'Parte28
        'Parte29
        'Hay que agregar otros tributos en el detalle 
        'Parte29
        'End If
        writer.WriteEndElement()


        writer.WriteStartElement("cac:LegalMonetaryTotal")

        'Parte30
        writer.WriteStartElement("cbc:LineExtensionAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(FacturaBE.t18_totalvalorventagravadas) - Double.Parse(FacturaBE.t50_descuentosglobales), 2).ToString("0.00"))
        writer.WriteEndElement()
        'Parte30

        'Parte31
        writer.WriteStartElement("cbc:TaxInclusiveAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(FacturaBE.t27_totalimporte) - Double.Parse(FacturaBE.t25_totalotroscargos), 2).ToString("0.00"))
        writer.WriteEndElement()
        'Parte31

        'Parte32
        writer.WriteStartElement("cbc:AllowanceTotalAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(FacturaBE.t50_descuentosglobales)
        writer.WriteEndElement()
        'Parte32

        'Parte33
        writer.WriteStartElement("cbc:ChargeTotalAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(FacturaBE.t25_totalotroscargos)
        writer.WriteEndElement()
        'Parte33

        'Parte34
        writer.WriteStartElement("cbc:PayableAmount")
        writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
        writer.WriteString(Math.Round(Double.Parse(FacturaBE.t27_totalimporte) + Double.Parse(FacturaBE.t50_descuentosglobales), 2).ToString("0.00")) 'tener en cuenta este campo es el importe total de la venta
        writer.WriteEndElement()
        'Parte34

        writer.WriteEndElement()

        'Se agrega el detalle del comprobante
        Dim orden As Integer = 1
        For Each item As FacturaItem In FacturaBE.Detalle
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
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02) 'Nuevo Campo Agregar Catalogo 06
            writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
            writer.WriteEndElement()
            'Parte37

            'Parte38
            writer.WriteStartElement("cac:PricingReference")
            writer.WriteStartElement("cac:AlternativeConditionPrice")

            writer.WriteStartElement("cbc:PriceAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(item.t15_preciounitario)
            writer.WriteEndElement()

            writer.WriteStartElement("cbc:PriceTypeCode")
            'writer.WriteAttributeString("listName", "SUNAT:Indicador de Tipo de Precio")
            writer.WriteAttributeString("listName", "Tipo de Precio")
            writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
            writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16")
            writer.WriteString(item.t15_tipprecio_c16) ' nuevo campo agregar si es transferencia gratuita cambiar 01 por 02 
            writer.WriteEndElement()

            writer.WriteEndElement()
            writer.WriteEndElement()
            'Parte38
            'Parte40 ' nuevo campo agregar verificar que el sistema de ventas no envie datos de descuento al detalle cuando no conrresponda
            If Double.Parse(item.t51_descuentoitem) > 0 Then

                writer.WriteStartElement("cac:AllowanceCharge")
                writer.WriteElementString("cbc:ChargeIndicator", "false")
                writer.WriteElementString("cbc:AllowanceChargeReasonCode", "00")

                'If Double.Parse(item.PorcentajeDescuento) > 0 Then
                '    writer.WriteElementString("cbc:MultiplierFactorNumeric:", item.PorcentajeDescuento) ' Nuevo Campo agregar porcentaje de descuento
                'End If

                writer.WriteStartElement("cbc:Amount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(item.t51_descuentoitem)
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:BaseAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(item.t15_preciounitario)
                writer.WriteEndElement()

                writer.WriteEndElement()
            End If
            'Parte40

            'Parte41
            'COE NO TIENE OTROS CARGOS POR DETALLE
            'Parte41

            'If Double.Parse(item.t16_totalitem) > 0 Then

            'IGV en DETALLE
            writer.WriteStartElement("cac:TaxTotal")

            writer.WriteStartElement("cbc:TaxAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            'writer.WriteString(item.t16_totalitem + item.t17_totalitem)
            ' writer.WriteString(Math.Round((Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)) + (Double.Parse(item.t17_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
            'Se pasa el IGV de la Linea
            writer.WriteString(item.t16_totalitem) 'Se pasa el IGV de la Linea
            writer.WriteEndElement()

            'Parte42
            If item.t16_tipafectacion_c07 = "10" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                'writer.WriteString(Math.Round((Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                'Se pasa el IGV de la Linea
                writer.WriteString(item.t16_totalitem)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" 'SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", IIf(Porcentaje <> "", Porcentaje, 18)) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t16_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString(item.t16_tiptributo_c05)
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t16_nomtributo_c05)
                writer.WriteElementString("cbc:TaxTypeCode", item.t16_tiptributointernacional_c05)

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            If item.t16_tipafectacion_c07 = "20" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                'writer.WriteString(Math.Round((Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                'Se pasa el IGV de la Linea
                writer.WriteString(item.t16_totalitem)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                'writer.WriteAttributeString("schemeName", "Tax Category Identifier")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
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
                writer.WriteString(item.t16_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9997")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "EXO")
                writer.WriteElementString("cbc:TaxTypeCode", "VAT")

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            If item.t16_tipafectacion_c07 = "21" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                'writer.WriteString(Math.Round((Double.Parse(item.t16_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                'Se pasa el IGV de la Linea
                writer.WriteString(item.t16_totalitem)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("Z")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18.00" 'SistemaDAO.GetProcentajeIGVCentral()
                'writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional
                writer.WriteElementString("cbc:Percent", Porcentaje) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t16_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9996")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "GRA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            If item.t16_tipafectacion_c07 = "30" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString("0.00")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("O")
                writer.WriteEndElement()


                Dim Porcentaje As String = "18" ' SistemaDAO.GetProcentajeIGVCentral()
                writer.WriteElementString("cbc:Percent", 0) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteStartElement("cbc:TaxExemptionReasonCode")
                'writer.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
                writer.WriteAttributeString("listName", "Afectacion del IGV")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
                writer.WriteString(item.t16_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                writer.WriteString("9998")
                writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", "INA")
                writer.WriteElementString("cbc:TaxTypeCode", "FRE")

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            If item.t16_tipafectacion_c07 = "40" Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
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
                writer.WriteString(item.t16_tipafectacion_c07)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
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

            If Double.Parse(item.t17_totalitem) > 0 Then
                writer.WriteStartElement("cac:TaxSubtotal")

                writer.WriteStartElement("cbc:TaxableAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t14_valorunitario) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cbc:TaxAmount")
                writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
                writer.WriteString(Math.Round((Double.Parse(item.t17_totalitem) * Double.Parse(item.t12_cantidad)), 2).ToString("0.00"))
                writer.WriteEndElement()

                writer.WriteStartElement("cac:TaxCategory")

                writer.WriteStartElement("cbc:ID")
                writer.WriteAttributeString("schemeID", "UN/ECE 5305")
                writer.WriteAttributeString("schemeName", "Codigo de tributos")
                writer.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
                writer.WriteString("S")
                writer.WriteEndElement()

                'Dim Porcentaje As String = SistemaDAO.GetProcentajeISCCentral()
                'writer.WriteElementString("cbc:Percent", item.t53_porcentajeISC) 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional
                writer.WriteElementString("cbc:Percent", "0.00") 'Nuevo Campo Agregar porcentaje de impuesto en cabecera y detalle opcional

                writer.WriteElementString("cbc:TierRange", item.t17_tipsistema_c08)

                writer.WriteStartElement("cac:TaxScheme")

                writer.WriteElementString("cbc:ID", item.t17_tiptributo_c05)

                'writer.WriteStartElement("cbc:ID")
                'writer.WriteAttributeString("schemeID", "UN/ECE 5153")
                'writer.WriteAttributeString("schemeName", "Tax Scheme Identifier")
                'writer.WriteAttributeString("schemeAgencyID", "United Nations Economic Commission for Europe")
                'writer.WriteString(item.t17_tiptributo_c05)
                'writer.WriteEndElement()

                writer.WriteElementString("cbc:Name", item.t17_nomtributo_c05)
                writer.WriteElementString("cbc:TaxTypeCode", item.t17_tiptributointernacional_c05)

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            writer.WriteEndElement()

            'End If

            'If Double.Parse(item.t17_totalitem) > 0 Then

            'writer.WriteStartElement("cac:TaxTotal")

            'writer.WriteStartElement("cbc:TaxAmount")
            'writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            'writer.WriteString(item.t17_totalitem)
            'writer.WriteEndElement()

            'Parte43

            'Parte43
            'writer.WriteEndElement()

            'End If

            'Parte44
            writer.WriteStartElement("cac:Item")
            writer.WriteStartElement("cbc:Description")
            writer.WriteCData(item.t13_descripcion)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
            writer.WriteEndElement()
            'Parte45
            writer.WriteStartElement("cac:SellersItemIdentification")
            writer.WriteElementString("cbc:ID", item.t34_codproducto)
            writer.WriteEndElement()

            'codigo internacional
            If FacturaBE.t51_tipoperacion_c17 = "02" Then
                writer.WriteStartElement("cac:CommodityClassification")
                writer.WriteStartElement("cbc:ItemClassificationCode")
                writer.WriteAttributeString("listID", "UNSPSC")
                writer.WriteAttributeString("listAgencyName", "GS1 US")
                writer.WriteAttributeString("listName", "Item Classification")
                'writer.WriteString(EmisorBE.CodigoInternacionalArticulo)
                writer.WriteString("0000")
                writer.WriteEndElement()
                writer.WriteEndElement()


                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Número de documento del huesped")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4009")
                writer.WriteEndElement()
                writer.WriteElementString("cbc:Value", FacturaBE.NombreTipoDocumentoHuesped)

                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Código de tipo de documento de identidad del huesped")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4008")
                writer.WriteEndElement()
                writer.WriteElementString("cbc:Value", FacturaBE.TipoDocumentoHuesped)

                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Código país de emisión del pasaporte")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4000")
                writer.WriteEndElement()
                writer.WriteElementString("cbc:Value", FacturaBE.PaisPasaporte)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Apellidos y Nombres o denominación o razón social del huesped")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4007")
                writer.WriteEndElement()
                writer.WriteElementString("cbc:Value", FacturaBE.NombreHuesped)
                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Código del país de residencia del sujeto no domiciliado")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4001")
                writer.WriteEndElement()
                writer.WriteElementString("cbc:Value", FacturaBE.SunatResidencia)
                writer.WriteEndElement()

                '---------------------------------

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Fecha de Ingreso al país")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4002")
                writer.WriteEndElement()

                writer.WriteStartElement("cac:UsabilityPeriod")
                writer.WriteElementString("cbc:StartDate", FacturaBE.fFechaIngresoPais.ToString("yyyy-MM-dd")) '"2018-07-01"
                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Fecha de Ingreso al Establecimiento")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4003")
                writer.WriteEndElement()
                writer.WriteStartElement("cac:UsabilityPeriod")
                writer.WriteElementString("cbc:StartDate", FacturaBE.fLlegadaReserva.ToString("yyyy-MM-dd"))
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Fecha de salida del Establecimiento")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4004")
                writer.WriteEndElement()
                writer.WriteStartElement("cac:UsabilityPeriod")
                writer.WriteElementString("cbc:StartDate", FacturaBE.fSalidaReserva.ToString("yyyy-MM-dd"))
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Fecha de consumo")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4006")
                writer.WriteEndElement()
                writer.WriteStartElement("cac:UsabilityPeriod")
                writer.WriteElementString("cbc:StartDate", FacturaBE.fLlegadaReserva.ToString("yyyy-MM-dd"))
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("cac:AdditionalItemProperty")
                writer.WriteElementString("cbc:Name", "Número de Días de Permanencia")
                writer.WriteStartElement("cbc:NameCode")
                writer.WriteAttributeString("listName", "SUNAT:Identificador de la propiedad del ítem")
                writer.WriteAttributeString("listAgencyName", "PE:SUNAT")
                writer.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo55")
                writer.WriteString("4005")
                writer.WriteEndElement()
                writer.WriteStartElement("cac:UsabilityPeriod")
                writer.WriteStartElement("cbc:DurationMeasure")
                writer.WriteAttributeString("unitCode", "DAY")
                writer.WriteString(FacturaBE.Permanencia)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If


            'Parte45
            writer.WriteEndElement()
            'Parte44
            'Parte48
            writer.WriteStartElement("cac:Price")
            'writer.WriteElementString("cbc:ID", item.t34_codproducto)

            writer.WriteStartElement("cbc:PriceAmount")
            writer.WriteAttributeString("currencyID", FacturaBE.t28_tipmoneda_c02)
            writer.WriteString(item.t14_valorunitario)
            writer.WriteEndElement()

            writer.WriteEndElement()
            'Parte48
            writer.WriteEndElement()
            'Parte35
            orden = orden + 1
        Next
        writer.WriteEndElement()

        'ORIGEN
        'Se cierra el documento
        writer.WriteEndDocument()
        writer.Close()
        writer.Dispose()

        'Se usa para reemplazar la cadena de minusculas a mayusculas
        Dim Data As String = File.ReadAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML)
        Data = Data.Replace("iso-8859-1", "ISO-8859-1")
        File.WriteAllText(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Data)

        'Se guarda la ruta del archivo xml
        SaveRutaArchivoXML(IDFactura, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se firma el comprobante
        Dim FirmaXMLDAO As New FirmaXMLDAO
        FirmaXMLDAO.Create(EmisorBE, RutaArchivo & NombreArchivo & ExtensionArchivoXML, "01")

    End Sub
    Public Function ZipXML(IDFactura As Int32) As String
        EmisorBE = EmisorDAO.GetByID(1)

        'Se obtiene la entidad
        FacturaBE = Me.GetByID(IDFactura)

        'Se obtiene el nombre del archivo zip
        Dim RutaArchivoZIP As String = Path.ChangeExtension(FacturaBE.RutaComprobanteXML, "zip")

        Using zipToOpen As FileStream = New FileStream(RutaArchivoZIP, FileMode.Create)
            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(Path.GetFileName(FacturaBE.RutaComprobanteXML))
                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(FacturaBE.RutaComprobanteXML))
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
    Public Function GetByID(IDFactura As Int32) As FacturaBE
        Dim Factura As New List(Of FacturaBE)
        Dim FacturaDetalle As New List(Of FacturaItem)
        Dim FacturaAnticipos As New List(Of FacturaAnticipo)
        Dim FacturaBE As New FacturaBE
        Dim FacturaDet As New FacturaItem
        Dim FacturaAnticipo As New FacturaAnticipo
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_get_id"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then

                While dr.Read
                    With FacturaBE
                        .idfactura = dr.ReadNullAsEmptyString("idfactura")
                        .t01_fecemision = dr.ReadNullAsEmptyString("t01_fecemision")
                        .t07_tipdoc_c01 = dr.ReadNullAsEmptyString("t07_tipdoc_c01")
                        .t08_numcorrelativo = dr.ReadNullAsEmptyString("t08_numcorrelativo")
                        .t09_numdoc = dr.ReadNullAsEmptyString("t09_numdoc")
                        .t09_tipdoc_c06 = dr.ReadNullAsEmptyString("t09_tipdoc_c06")
                        .t10_nomadquiriente = dr.ReadNullAsEmptyString("t10_nomadquiriente")
                        .t18_tipmonto_c14 = dr.ReadNullAsEmptyString("t18_tipmonto_c14")
                        .t18_totalvalorventagravadas = dr.ReadNullAsEmptyString("t18_totalvalorventagravadas")
                        .t19_tipmonto_c14 = dr.ReadNullAsEmptyString("t19_tipmonto_c14")
                        .t19_totalvalorventainafectas = dr.ReadNullAsEmptyString("t19_totalvalorventainafectas")
                        .t20_tipmonto_c14 = dr.ReadNullAsEmptyString("t20_tipmonto_c14")
                        .t20_totalvalorventaexonerada = dr.ReadNullAsEmptyString("t20_totalvalorventaexonerada")
                        .t22_totaligv = dr.ReadNullAsEmptyString("t22_totaligv")
                        .t22_subtotaligv = dr.ReadNullAsEmptyString("t22_subtotaligv")
                        .t22_tiptributo_c05 = dr.ReadNullAsEmptyString("t22_tiptributo_c05")
                        .t22_nomtributo_c05 = dr.ReadNullAsEmptyString("t22_nomtributo_c05")
                        .t22_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t22_tiptributointernacional_c05")
                        .t23_totalisc = dr.ReadNullAsEmptyString("t23_totalisc")
                        .t23_subtotalisc = dr.ReadNullAsEmptyString("t23_subtotalisc")
                        .t23_tiptributo_c05 = dr.ReadNullAsEmptyString("t23_tiptributo_c05")
                        .t23_nomtributo_c05 = dr.ReadNullAsEmptyString("t23_nomtributo_c05")
                        .t23_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t23_tiptributointernacional_c05")
                        .t24_totalotrostributos = dr.ReadNullAsEmptyString("t24_totalotrostributos")
                        .t24_subtotalotrostributos = dr.ReadNullAsEmptyString("t24_subtotalotrostributos")
                        .t24_tiptributo_c05 = dr.ReadNullAsEmptyString("t24_tiptributo_c05")
                        .t24_nomtributo_c05 = dr.ReadNullAsEmptyString("t24_nomtributo_c05")
                        .t24_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t24_tiptributointernacional_c05")
                        .t25_totalotroscargos = dr.ReadNullAsEmptyString("t25_totalotroscargos")
                        .t26_tipmonto_c14 = dr.ReadNullAsEmptyString("t26_tipmonto_c14")
                        .t26_totaldescuentos = dr.ReadNullAsEmptyString("t26_totaldescuentos")
                        .t27_totalimporte = dr.ReadNullAsEmptyString("t27_totalimporte")
                        .t28_tipmoneda_c02 = dr.ReadNullAsEmptyString("t28_tipmoneda_c02")
                        .t29_sernumguia = dr.ReadNullAsEmptyString("t29_sernumguia")
                        .t29_tipdoc_c01 = dr.ReadNullAsEmptyString("t29_tipdoc_c01")
                        .t30_numdocrelacionado = dr.ReadNullAsEmptyString("t30_numdocrelacionado")
                        .t30_tipdoc_c12 = dr.ReadNullAsEmptyString("t30_tipdoc_c12")
                        .t31_tipleyenda_c15 = dr.ReadNullAsEmptyString("t31_tipleyenda_c15")
                        .t31_descripcionleyenda = dr.ReadNullAsEmptyString("t31_descripcionleyenda")
                        .t32_tipmonto_c14 = dr.ReadNullAsEmptyString("t32_tipmonto_c14")
                        .t32_baseimponiblepercepcion = dr.ReadNullAsEmptyString("t32_baseimponiblepercepcion")
                        .t32_montopercepcion = dr.ReadNullAsEmptyString("t32_montopercepcion")
                        .t32_montototalpercepcion = dr.ReadNullAsEmptyString("t32_montototalpercepcion")
                        .t36_versionubl = dr.ReadNullAsEmptyString("t36_versionubl")
                        .t37_versiondoc = dr.ReadNullAsEmptyString("t37_versiondoc")
                        .t38_tipconcepto_c14 = dr.ReadNullAsEmptyString("t38_tipconcepto_c14")
                        .t38_serviciotransporte = dr.ReadNullAsEmptyString("t38_serviciotransporte")
                        .t39_tipconcepto_c15 = dr.ReadNullAsEmptyString("t39_tipconcepto_c15")
                        .t39_nommatricula = dr.ReadNullAsEmptyString("t39_nommatricula")
                        .t40_tipconcepto_c15 = dr.ReadNullAsEmptyString("t40_tipconcepto_c15")
                        .t40_especievendida = dr.ReadNullAsEmptyString("t40_especievendida")
                        .t41_tipconcepto_c15 = dr.ReadNullAsEmptyString("t41_tipconcepto_c15")
                        .t41_lugardescarga = dr.ReadNullAsEmptyString("t41_lugardescarga")
                        .t42_tipconcepto_c15 = dr.ReadNullAsEmptyString("t42_tipconcepto_c15")
                        .t42_fecdescarga = dr.ReadNullAsEmptyString("t42_fecdescarga")
                        .t43_tipconcepto_c15 = dr.ReadNullAsEmptyString("t43_tipconcepto_c15")
                        .t43_rnumregistro = dr.ReadNullAsEmptyString("t43_rnumregistro")
                        .t44_tipconcepto_c15 = dr.ReadNullAsEmptyString("t44_tipconcepto_c15")
                        .t44_configuracionvehicular = dr.ReadNullAsEmptyString("t44_configuracionvehicular")
                        .t45_tipconcepto_c15 = dr.ReadNullAsEmptyString("t45_tipconcepto_c15")
                        .t45_puntoorigen = dr.ReadNullAsEmptyString("t45_puntoorigen")
                        .t46_tipconcepto_c15 = dr.ReadNullAsEmptyString("t46_tipconcepto_c15")
                        .t46_puntodestino = dr.ReadNullAsEmptyString("t46_puntodestino")
                        .t47_tipconcepto_c15 = dr.ReadNullAsEmptyString("t47_tipconcepto_c15")
                        .t47_descripcionvalorreferencial = dr.ReadNullAsEmptyString("t47_descripcionvalorreferencial")
                        .t47_valorreferencial = dr.ReadNullAsEmptyString("t47_valorreferencial")
                        .t48_tipconcepto_c15 = dr.ReadNullAsEmptyString("t48_tipconcepto_c15")
                        .t48_fecconsumo = dr.ReadNullAsEmptyString("t48_fecconsumo")
                        .t49_tipmonto_c14 = dr.ReadNullAsEmptyString("t49_tipmonto_c14")
                        .t49_totalvalorventaoperacionesgratuitas = dr.ReadNullAsEmptyString("t49_totalvalorventaoperacionesgratuitas")
                        .t50_descuentosglobales = dr.ReadNullAsEmptyString("t50_descuentosglobales")
                        .t51_tipoperacion_c17 = dr.ReadNullAsEmptyString("t51_tipoperacion_c17")
                        .estado = dr.ReadNullAsEmptyString("estado")
                        .fechahorasistemaexterno = dr.ReadNullAsEmptyDate("fechahorasistemaexterno")
                        .fechahorasunat = dr.ReadNullAsEmptyDate("fechahorasunat")
                        .idusuario = dr.ReadNullAsNumeric("IDUsuario")
                        .fecharegistro = dr.ReadNullAsEmptyDate("fecharegistro")
                        .ValorFirma = dr.ReadNullAsEmptyString("valorfirma")
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

                        'Nuevos Version 2.1
                        .HoraEmision = dr.ReadNullAsEmptyString("t01_horaemision")

                        'No se usan
                        .CodTipoOperacion = "0101"
                        .TipoMoneda = "PEN"
                        .NumOrdenCompra = "0"
                    End With
                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        FacturaDet = New FacturaItem
                        With FacturaDet
                            .idfacturadetalle = dr.ReadNullAsEmptyString("idfacturadetalle")
                            .idfactura = dr.ReadNullAsEmptyString("idfactura")
                            .t11_tipunidadmedida_c03 = dr.ReadNullAsEmptyString("t11_tipunidadmedida_c03")
                            .t12_cantidad = dr.ReadNullAsEmptyString("t12_cantidad")
                            .t13_descripcion = dr.ReadNullAsEmptyString("t13_descripcion")
                            .t14_valorunitario = dr.ReadNullAsEmptyString("t14_valorunitario")
                            .t15_preciounitario = dr.ReadNullAsEmptyString("t15_preciounitario")
                            .t15_tipprecio_c16 = dr.ReadNullAsEmptyString("t15_tipprecio_c16")
                            .t16_totalitem = dr.ReadNullAsEmptyString("t16_totalitem")
                            .t16_subtotalitem = dr.ReadNullAsEmptyString("t16_subtotalitem")
                            .t16_tipafectacion_c07 = dr.ReadNullAsEmptyString("t16_tipafectacion_c07")
                            .t16_tiptributo_c05 = dr.ReadNullAsEmptyString("t16_tiptributo_c05")
                            .t16_nomtributo_c05 = dr.ReadNullAsEmptyString("t16_nomtributo_c05")
                            .t16_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t16_tiptributointernacional_c05")
                            .t17_totalitem = dr.ReadNullAsEmptyString("t17_totalitem")
                            .t17_subtotalitem = dr.ReadNullAsEmptyString("t17_subtotalitem")
                            .t17_tipsistema_c08 = dr.ReadNullAsEmptyString("t17_tipsistema_c08")
                            .t17_tiptributo_c05 = dr.ReadNullAsEmptyString("t17_tiptributo_c05")
                            .t17_nomtributo_c05 = dr.ReadNullAsEmptyString("t17_nomtributo_c05")
                            .t17_tiptributointernacional_c05 = dr.ReadNullAsEmptyString("t17_tiptributointernacional_c05")
                            .t21_valorventaitem = dr.ReadNullAsEmptyString("t21_valorventaitem")
                            .t33_numordenitem = dr.ReadNullAsEmptyString("t33_numordenitem")
                            .t34_codproducto = dr.ReadNullAsEmptyString("t34_codproducto")
                            .t35_valorreferencialunitarioitem = dr.ReadNullAsEmptyString("t35_valorreferencialunitarioitem")
                            .t35_tipreferencia_c16 = dr.ReadNullAsEmptyString("t35_tipreferencia_c16")
                            .t51_tipdescuentoitem = dr.ReadNullAsEmptyString("t51_tipdescuentoitem")
                            .t51_descuentoitem = dr.ReadNullAsEmptyString("t51_descuentoitem")
                        End With
                        FacturaBE.Detalle.Add(FacturaDet)
                    End While

                    dr.NextResult()

                    If dr.HasRows Then
                        While dr.Read
                            FacturaAnticipo = New FacturaAnticipo
                            With FacturaAnticipo
                                .IDAnticipo = dr.ReadNullAsEmptyString("idanticipo")
                                .IDComprobante = dr.ReadNullAsEmptyString("idcomprobante")
                                .Referencia = dr.ReadNullAsEmptyString("referencia")
                                .TipoComprobante = dr.ReadNullAsEmptyString("tipocomprobante")
                                .Importe = dr.ReadNullAsEmptyString("importe")
                                .NumDocumento = dr.ReadNullAsEmptyString("numdoc")

                                .FechaAnticipo = "2018-10-10"
                                .HoraAnticipo = "12:12:12"
                            End With
                            FacturaBE.Anticipos.Add(FacturaAnticipo)
                        End While
                    End If

                    Factura.Add(FacturaBE)
                End If
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try

        Me.BE = FacturaBE
        Return FacturaBE
    End Function
    Public Function GetByReporteID(IDFactura As Int32) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_rpt_id"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
    Public Function GetByIDXML(IDFactura As Int32) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As System.Xml.XmlReader = Nothing
        Dim doc As New XmlDocument()
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_get_id_xml"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
            .CommandText = "coe_factura_get_all"
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

        Dim FacturaBE As New FacturaBE
        FacturaBE = Me.GetByID(IDComprobante)

        'Se establece la carpeta donde se almacena
        RutaCloud = "/r" & EmisorBE.NumeroRUC & "/"

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_web_ins"

            With .Parameters
                .Add("@IDComprobanteWeb", SqlDbType.Int).Direction = ParameterDirection.Output
                .Add("@IDComprobante", SqlDbType.Int).Value = FacturaBE.idfactura
                .Add("@Tipo", SqlDbType.Char, 2).Value = FacturaBE.TipoComprobante
                .Add("@NumeroRUC", SqlDbType.VarChar, 15).Value = EmisorBE.NumeroRUC
                .Add("@NumeroCorrelativo", SqlDbType.VarChar, 20).Value = FacturaBE.t08_numcorrelativo
                .Add("@FechaEmision", SqlDbType.VarChar, 10).Value = FacturaBE.t01_fecemision
                .Add("@Importe", SqlDbType.Decimal).Value = FacturaBE.t27_totalimporte
                .Add("@RutaArchivoPDF", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(FacturaBE.RutaComprobantePDF)
                .Add("@RutaArchivoXML", SqlDbType.VarChar, 500).Value = RutaCloud & Path.GetFileName(FacturaBE.RutaComprobanteXML)
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

        Dim FacturaBE As New FacturaBE
        FacturaBE = Me.GetByID(IDComprobante)

        If ToolsAzure.SaveStorageFiles("", "r" & EmisorBe.NumeroRUC, FacturaBE.RutaComprobanteXML) Then
            If ToolsAzure.SaveStorageFiles("", "r" & EmisorBe.NumeroRUC, FacturaBE.RutaComprobantePDF) Then
                Result = True
            End If
        End If

        Return Result
    End Function
    Public Function SaveRutaArchivoXML(IDFactura As Int32, RutaCarpetaXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_ruta_update"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
    Public Function SaveRutaArchivoCdrXML(IDFactura As Int32, RutaCarpetaCdrXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_ruta_cdr_update"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
    Public Function SaveValorFirma(IDFactura As Int32, ValorFirma As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_firma_update"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
    Public Function SaveConstanciaRecepcionZIP(IDFactura As Int32, ContenidoArchivoZIP As Byte()) As Boolean
        Dim Result As Boolean = False
        Dim NombreArchivo As String = String.Empty

        Try
            'Se obtiene la entidad
            FacturaBE = Me.GetByID(IDFactura)

            'Se obtiene el nombre del archivo xml, sin extension
            NombreArchivo = Path.GetFileNameWithoutExtension(FacturaBE.RutaComprobanteXML)

            'Se agrega la letra R al nombre (Respuesta) y se coloca la extension .zip
            NombreArchivo = "R-" & Path.ChangeExtension(NombreArchivo, ".zip")

            'Se concatena la ruta y el nuevo nombre del archivo zip
            NombreArchivo = Path.GetDirectoryName(FacturaBE.RutaComprobanteXML) & "\" & NombreArchivo

            'Se guarda el archivo zip que envia la sunat
            System.IO.File.WriteAllBytes(NombreArchivo, ContenidoArchivoZIP)

            'Se descomprime el archivo ZIP y extrae el XML
            Me.UnZipXML(NombreArchivo)

            'Se guarda el nombre del archivo de la constancia de recepcion CDR SUNAT
            Me.SaveRutaArchivoCdrXML(IDFactura, Path.ChangeExtension(NombreArchivo, ".xml"))

            'Se lee el contenido del archivo XML y se lee contenido
            Me.SaveConstanciaRecepcionXML(IDFactura)

            Result = True
        Catch ex As Exception
            Throw New Exception("No se guardo el archivo de respuesta de la SUNAT. " & ex.Message)
        End Try

        Return Result
    End Function
    Public Function SaveConstanciaRecepcionXML(IDFactura As Int32) As Boolean
        Dim Result As Boolean = False
        Dim Valor As String = String.Empty
        Dim CodigoRespuesta As String = String.Empty
        Dim NumeroComprobante As String = String.Empty
        Dim Descripcion As String = String.Empty

        'Se obtiene la entidad
        FacturaBE = Me.GetByID(IDFactura)

        'Se obtiene el archivo XML
        Dim ReaderXML As XmlTextReader = New XmlTextReader(FacturaBE.RutaRespuestaSunatXML)

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
            .CommandText = "coe_factura_estado_update"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
            .CommandText = "coe_factura_excepcion_upd"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = BE.IDComprobante
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
    Public Function SaveEstadoWeb(IDFactura As Int32, IDEstadoWeb As eEstadoWeb, FechaWeb As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_factura_estado_web"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = IDFactura
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
    ''' <summary>
    ''' Se genera el TAG Nodo+Atributo+ValorAtributo+ValorNodo
    ''' </summary>
    ''' <param name="doc">Documento XML</param>
    ''' <param name="E1">Nombre del nodo</param>
    ''' <param name="E2">Nombre del Atributo</param>
    ''' <param name="E3">Valor del atributo</param>
    ''' <param name="E4">Valor del nodo</param>
    ''' <remarks></remarks>
    Public Sub TagNodoAtributoValorValor(ByRef doc As XmlTextWriter, E1 As String, E2 As String, E3 As String, E4 As String)

        doc.WriteStartElement(E1)
        doc.WriteAttributeString(E2, E3)
        doc.WriteString(E4)
        doc.WriteEndElement()
    End Sub


    Public Sub HeaderSignature(ByRef xml As XmlTextWriter, EmisorBE As EmisorBE)
        xml.WriteStartElement("cac:Signature")
        xml.WriteElementString("cbc:ID", "IDSignSP")
        xml.WriteStartElement("cac:SignatoryParty")
        xml.WriteStartElement("cac:PartyIdentification")
        xml.WriteElementString("cbc:ID", EmisorBE.NumeroRUC)
        xml.WriteEndElement()
        xml.WriteStartElement("cac:PartyName")
        xml.WriteStartElement("cbc:Name")
        xml.WriteCData(EmisorBE.NombreComercial)  'Se agrega tag CDATA. Se usa el nombre legal de la empresa emisora
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteStartElement("cac:DigitalSignatureAttachment")
        xml.WriteStartElement("cac:ExternalReference")
        xml.WriteElementString("cbc:URI", "#SignatureSP")
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
    End Sub
    Public Sub HeaderAccountingSupplierParty(ByRef xml As XmlTextWriter, EmisorBE As EmisorBE)

        xml.WriteStartElement("cac:AccountingSupplierParty")
        xml.WriteStartElement("cac:Party")
        xml.WriteStartElement("cac:PartyName")
        xml.WriteStartElement("cbc:Name")
        xml.WriteCData(EmisorBE.NombreComercial)
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteStartElement("cac:PartyTaxScheme")

        xml.WriteStartElement("cbc:RegistrationName")
        xml.WriteCData(EmisorBE.NombreComercial)  '
        xml.WriteEndElement()

        xml.WriteStartElement("CompanyID")
        xml.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        xml.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        xml.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        xml.WriteAttributeString("schemeID", "6")
        xml.WriteString(EmisorBE.NumeroRUC)
        xml.WriteEndElement()
        xml.WriteStartElement("cac:RegistrationAddress")
        xml.WriteStartElement("cbc:AddressTypeCode")
        xml.WriteString("0001")
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteStartElement("cac:TaxScheme")
        xml.WriteStartElement("cbc:ID")
        xml.WriteString("-")
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()


    End Sub
    Public Sub HeaderAccountingCustomerParty(ByRef xml As XmlTextWriter, Facturabe As FacturaBE)
        xml.WriteStartElement("cac:AccountingCustomerParty")
        xml.WriteStartElement("cac:Party")

        xml.WriteStartElement("cac:PartyIdentification")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        xml.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        xml.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        xml.WriteAttributeString("schemeID", "6")
        xml.WriteString(Facturabe.t09_numdoc)
        xml.WriteEndElement()
        xml.WriteEndElement()

        xml.WriteStartElement("cac:PartyName")
        xml.WriteStartElement("cbc:Name")
        xml.WriteCData(Facturabe.t10_nomadquiriente)  '
        xml.WriteEndElement()
        xml.WriteEndElement()

        xml.WriteStartElement("cac:PartyLegalEntity")
        xml.WriteStartElement("cbc:RegistrationName")
        xml.WriteCData(Facturabe.t10_nomadquiriente)
        xml.WriteEndElement()


        xml.WriteStartElement("cac:RegistrationAddress")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeAgencyName", "PE:INEI")
        xml.WriteAttributeString("schemeName", "Ubigeos")
        xml.WriteStartElement("cac:AddressLine")
        xml.WriteStartElement("cbc:Line")
        xml.WriteCData("PUEBLO LIBRE")
        xml.WriteEndElement()
        xml.WriteEndElement()


        xml.WriteStartElement("cac:Country")
        xml.WriteStartElement("cbc:IdentificationCode")
        xml.WriteAttributeString("listID", "ISO 3166-1")
        xml.WriteAttributeString("listName", "Country")
        xml.WriteAttributeString("listAgencyName", "United Nations Economic Commission for Europe")
        xml.WriteEndElement()

        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()




        'xml.WriteStartElement("cac:PartyTaxScheme")
        'xml.WriteStartElement("cbc:RegistrationName")
        'xml.WriteCData(Facturabe.t10_nomadquiriente)
        'xml.WriteStartElement("CompanyID")
        'xml.WriteAttributeString("schemeURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06")
        'xml.WriteAttributeString("schemeAgencyName", "PE:SUNAT")
        'xml.WriteAttributeString("schemeName", "SUNAT:Identificador de Documento de Identidad")
        'xml.WriteAttributeString("schemeID", "6")
        'xml.WriteString(Facturabe.t09_numdoc)
        'xml.WriteEndElement()
        'xml.WriteStartElement("cac:TaxScheme")
        'xml.WriteStartElement("cbc:ID")
        'xml.WriteString("-")
        'xml.WriteEndElement()
        'xml.WriteEndElement()
        'xml.WriteEndElement()
        'xml.WriteEndElement()
        'xml.WriteEndElement()
        'xml.WriteEndElement()
    End Sub
    Public Sub HeaderDescuentoGlobales(ByRef xml As XmlTextWriter, Facturabe As FacturaBE)
        'TAG NUEVO : DESCUENTOS GLOBALES : 
        '---------------------------------------------------------------------------------------------
        '           cbc:ChargeIndicator Dado que no es un cargo, se debe asignar indicador “false”.
        '           cbc:AllowanceChargeReasonCode 0 CATALOGO 53 OTROS DESCUENTOS
        '           cbc:MultiplierFactorNumeric En este elemento se especifica el porcentaje que corresponde del descuento global aplicado. Se expresa en números decimales por ejemplo 5% será 0.05
        '           cbc:Amount Este campo representa el importe del descuento global
        '           cbc:BaseAmount A través de este campo se debe indicar el importe sobre el cual se está aplicando el descuento global.
        '<cac:AllowanceCharge>
        '<cbc:ChargeIndicator>False</cbc:ChargeIndicator>
        '<cbc:AllowanceChargeReasonCode> 0</cbc:AllowanceChargeReasonCode>
        '<cbc:MultiplierFactorNumeric>0.05</cbc:MultiplierFactorNumeric>
        '<cbc:Amount currencyID = "PEN" > 18976.27</cbc: Amount>
        '<cbc:BaseAmount currencyID="PEN">379525.42</cbc:BaseAmount>
        '</cac:AllowanceCharge>
        xml.WriteStartElement("cac:AllowanceCharge")
        xml.WriteElementString("cbc:ChargeIndicator", False)
        xml.WriteElementString("cbc:AllowanceChargeReasonCode", "00")
        xml.WriteElementString("cbc:MultiplierFactorNumeric", 0.00)
        Tools.TagNodoAtributoValorValor(xml, "cbc:Amount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t22_totaligv)
        Tools.TagNodoAtributoValorValor(xml, "cbc:BaseAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t26_totaldescuentos)
        xml.WriteEndElement()
    End Sub
    Public Sub HeaderTaxTotal(ByRef xml As XmlTextWriter, Facturabe As FacturaBE)

        xml.WriteStartElement("cac:TaxTotal")
        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t22_totaligv)

        xml.WriteStartElement("cac:TaxSubtotal")
        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxableAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t22_totaligv)
        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t22_totaligv)

        xml.WriteStartElement("cac:TaxCategory")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
        xml.WriteAttributeString("schemeName", "Tax Category Identifier")
        xml.WriteAttributeString("schemeID", "UN/ECE 5305")
        xml.WriteString(Facturabe.t22_tiptributo_c05)
        xml.WriteEndElement()

        xml.WriteStartElement("cac:TaxScheme")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeAgencyID", "6")
        xml.WriteAttributeString("schemeID", "UN/ECE 5153")
        xml.WriteString("1000")
        xml.WriteEndElement()

        xml.WriteElementString("cbc:Name", Facturabe.t22_nomtributo_c05)
        xml.WriteElementString("cbc:TaxTypeCode", Facturabe.t22_tiptributointernacional_c05)

        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
    End Sub
    Public Sub HeaderLegalMonetaryTotal(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, TotalAnticipos As Decimal)

        xml.WriteStartElement("cac:LegalMonetaryTotal")

        'Se establece tag para los anticipos
        If TotalAnticipos > 0 Then
            Tools.TagNodoAtributoValorValor(xml, "cbc:PrepaidAmount", "currencyID", Facturabe.t28_tipmoneda_c02, TotalAnticipos)
        End If

        Tools.TagNodoAtributoValorValor(xml, "cbc:PayableAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Facturabe.t27_totalimporte)

        xml.WriteEndElement()

    End Sub

    Public Sub ItemID(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)
        'TAG NUEVO :
        '   <cbc:InvoicedQuantity unitCode="BX" unitCodeListID="UN/ECE rec 20" unitCodeListAgencyName="United Nations Economic Commission forEurope">2000</cbc:InvoicedQuantity>
        '   Tools.TagNodoAtributoValorValor(writer, "cbc:InvoicedQuantity", "unitCode", Item.t11_tipunidadmedida_c03, Item.t12_cantidad)
        'writer.WriteElementString("cbc:ID", Item.t33_numordenitem)
        xml.WriteElementString("cbc:ID", Item.t33_numordenitem)
    End Sub
    Public Sub ItemCantidad(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)
        xml.WriteStartElement("cbc:InvoicedQuantity")
        xml.WriteAttributeString("unitCode", Item.t11_tipunidadmedida_c03)
        xml.WriteAttributeString("unitCodeListID", "TUN/ECE rec 20")
        xml.WriteAttributeString("unitCodeListAgencyName", "United Nations Economic Commission forEurope")
        xml.WriteString(Item.t12_cantidad)
        xml.WriteEndElement()
    End Sub
    Public Sub ItemLineExtension(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)
        Tools.TagNodoAtributoValorValor(xml, "cbc:LineExtensionAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t21_valorventaitem)
    End Sub
    Public Sub ItemPriceReference(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)

        'TAG NUEVO
        '   <cbc:PriceTypeCode listName="SUNAT:Indicador de Tipo de Precio" listAgencyName="PE:SUNAT" listURI="urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16">01</cbc:PriceTypeCode>
        '   writer.WriteElementString("cbc:PriceTypeCode", Item.t15_tipprecio_c16)
        xml.WriteStartElement("cac:PricingReference")
        xml.WriteStartElement("cac:AlternativeConditionPrice")

        Tools.TagNodoAtributoValorValor(xml, "cbc:PriceAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t15_preciounitario)

        xml.WriteStartElement("cbc:PriceTypeCode")
        xml.WriteAttributeString("listName", "SUNAT:Indicador de Tipo de Precio")
        xml.WriteAttributeString("listAgencyName", "PE:SUNAT")
        xml.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo16")
        xml.WriteString(Item.t15_tipprecio_c16)
        xml.WriteEndElement()
        xml.WriteEndElement()
        xml.WriteEndElement()
    End Sub
    Public Sub ItemTaxTotal(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)

        ''TAG BLOQUE: TAXTOTAL
        ''<cbc:ID schemeID="UN/ECE 5305" schemeName="Tax Category Identifier" schemeAgencyName="United Nations Economic Commission for Europe">S</cbc:ID>
        'writer.WriteElementString("cbc:ID", Item.t16_tiptributo_c05)
        xml.WriteStartElement("cac:TaxTotal")

        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t16_totalitem)
        xml.WriteStartElement("cac:TaxSubtotal")
        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxableAmoun", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t16_subtotalitem)
        Tools.TagNodoAtributoValorValor(xml, "cbc:TaxAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t16_subtotalitem)

        xml.WriteStartElement("cac:TaxCategory")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeID", "UN/ECE 5305")
        xml.WriteAttributeString("schemeName", "Tax Category Identifier")
        xml.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
        xml.WriteString(Item.t16_tiptributo_c05)
        xml.WriteEndElement()


        xml.WriteElementString("cbc:Percent", "18.00")
        xml.WriteStartElement("cbc:TaxExemptionReasonCode")
        xml.WriteAttributeString("listAgencyName", "PE:SUNAT")
        'xml.WriteAttributeString("listName", "SUNAT:Codigo de Tipo de Afectación del IGV")
        xml.WriteAttributeString("listName", "Afectacion del IGV")
        xml.WriteAttributeString("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07")
        xml.WriteString(Item.t16_tipafectacion_c07)
        xml.WriteEndElement()

        xml.WriteStartElement("cac:TaxScheme")
        xml.WriteStartElement("cbc:ID")
        xml.WriteAttributeString("schemeID", "UN/ECE 5153")
        xml.WriteAttributeString("schemeName", "Tax Scheme Identifier")
        xml.WriteAttributeString("schemeAgencyName", "United Nations Economic Commission for Europe")
        xml.WriteString("1000")
        xml.WriteEndElement()

        xml.WriteElementString("cbc:Name", Item.t16_nomtributo_c05.ToString.Trim)
        xml.WriteElementString("cbc:TaxTypeCode", Item.t16_tiptributointernacional_c05)
        xml.WriteEndElement()
        xml.WriteEndElement()

        xml.WriteEndElement()
        xml.WriteEndElement()
    End Sub
    Public Sub ItemItem(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)
        'TAG BLOQUE : ITEM
        '       <cac:CommodityClassification>
        '   	    <cbc:ItemClassificationCode listID="UNSPSC" listAgencyName="GS1 US" listName="Item Classification">10000000</cbc:ItemClassificationCode>
        '       </cac:CommodityClassification>
        xml.WriteStartElement("cac:Item")

        xml.WriteStartElement("cbc:Description")
        xml.WriteCData(Item.t13_descripcion)
        xml.WriteEndElement()

        xml.WriteStartElement("cac:SellersItemIdentification")
        xml.WriteElementString("cbc:ID", Item.t34_codproducto)
        xml.WriteEndElement()

        'Codigo de producto SUNAT.Es opcional. Catálogo N° 25
        If False Then
            xml.WriteStartElement("cac:CommodityClassification")
            xml.WriteStartElement("cbc:ItemClassificationCode ")
            xml.WriteAttributeString("listName", "Item Classification")
            xml.WriteAttributeString("listAgencyName", "GS1 US")
            xml.WriteAttributeString("listID", "UNSPSC")
            xml.WriteString("CODIGO SUNAT")
            xml.WriteEndElement()
            xml.WriteEndElement()
        End If

        xml.WriteEndElement()

    End Sub
    Public Sub ItemPrice(ByRef xml As XmlTextWriter, Facturabe As FacturaBE, Item As FacturaItem)
        xml.WriteStartElement("cac:Price")
        Tools.TagNodoAtributoValorValor(xml, "cbc:PriceAmount", "currencyID", Facturabe.t28_tipmoneda_c02, Item.t14_valorunitario)
        xml.WriteEndElement()
    End Sub

End Class
