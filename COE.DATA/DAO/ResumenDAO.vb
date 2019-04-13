Imports COE.FRAMEWORK
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports System.Xml
Imports System.Data.SqlClient
Imports System.Text

Public Enum eGetResumen
    ResumenPendientesEnvio = 1
    TicketsPendientesCDR = 2
End Enum

Public Class ResumenDAO
    Public Property IDResumen As Int32
    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim EmisorBE As New EmisorBE
    Dim ResumenBE As New ResumenBE2018

    Public Function CreateXML(IDResumen As Int32) As String

        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"

        'Se obtiene la entidad
        Me.ResumenBE = Me.GetByID(IDResumen)

        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-RC-" & Me.ResumenBE.t18_fecresumen.Replace("-", "") & "-" & Me.ResumenBE.t17_numcorrelativo


        'Se crea el documento XML con todas las propiedades requeridas de la sunat. 
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'Se crea el nodo raiz
        writer.WriteStartElement("SummaryDocuments")
        writer.WriteAttributeString("xmlns:udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
        writer.WriteAttributeString("xmlns:qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")
        writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1")

        'Se define este tag es para guardar la firma digital
        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'La version del UBLVersionID es 2.0
        writer.WriteElementString("cbc:UBLVersionID", ResumenBE.t20_versionubl)

        'La version del CustomizationID es 1.0
        writer.WriteElementString("cbc:CustomizationID", ResumenBE.t21_versiondoc)

        'Se establece el id del documentos
        writer.WriteElementString("cbc:ID", "RC-" & ResumenBE.t18_fecresumen.Replace("-", "") & "-" & ResumenBE.t17_numcorrelativo)

        'Se coloca la fecha de emision de los comprobantes
        writer.WriteElementString("cbc:ReferenceDate", ResumenBE.t03_fecemision)

        'Se coloca le fecha cuando se genera el resumen
        writer.WriteElementString("cbc:IssueDate", ResumenBE.t18_fecresumen)

        'Se establece la informacion de emisor
        writer.WriteStartElement("cac:Signature")
        writer.WriteElementString("cbc:ID", "IDSignSP")
        writer.WriteStartElement("cac:SignatoryParty")
        writer.WriteStartElement("cac:PartyIdentification")
        writer.WriteElementString("cbc:ID", EmisorBE.NumeroRUC)
        writer.WriteEndElement()

        writer.WriteStartElement("cac:PartyName")
        writer.WriteStartElement("cbc:Name")
        writer.WriteCData(EmisorBE.RazonSocial)
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
        writer.WriteElementString("cbc:CustomerAssignedAccountID", EmisorBE.NumeroRUC)
        writer.WriteElementString("cbc:AdditionalAccountID", "6")
        writer.WriteStartElement("cac:Party")

        writer.WriteStartElement("cac:PartyLegalEntity")
        writer.WriteStartElement("cbc:RegistrationName")
        writer.WriteCData(EmisorBE.NombreComercial)
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'Se genera los items o las lineas del Resumen de Boletas
        For Each Item In ResumenBE.Lineas

            writer.WriteStartElement("sac:SummaryDocumentsLine")
            writer.WriteElementString("cbc:LineID", Item.t01_numfila)
            writer.WriteElementString("cbc:DocumentTypeCode", Item.t02_tipdoc_c01)
            writer.WriteElementString("cbc:ID", Item.t02_numcorrelativo)

            writer.WriteStartElement("cac:AccountingCustomerParty")
            writer.WriteElementString("cbc:CustomerAssignedAccountID", Item.t03_numdoc_adquiriente)
            writer.WriteElementString("cbc:AdditionalAccountID", Item.t03_tipdoc_adquiriente)
            writer.WriteEndElement()

            'Para el caso de Notas de credito de boletas
            If Item.t02_tipdoc_c01 = "07" Then

                writer.WriteStartElement("cac:BillingReference")
                writer.WriteStartElement("cac:InvoiceDocumentReference")

                writer.WriteElementString("cbc:ID", Item.t04_numcorrelativo_modifica)
                writer.WriteElementString("cbc:DocumentTypeCode", Item.t04_tipdoc_c01)

                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            writer.WriteStartElement("cac:Status")
            writer.WriteElementString("cbc:ConditionCode", Item.t05_estadoitem_c19)
            writer.WriteEndElement()

            'Se establece el Importe Total
            Tools.TagNodoAtributoValorValor(writer, "sac:TotalAmount", "currencyID", "PEN", Item.t06_importetotal)

            'Se establece el valor ventas gravadas
            writer.WriteStartElement("sac:BillingPayment")
            Tools.TagNodoAtributoValorValor(writer, "cbc:PaidAmount", "currencyID", "PEN", Item.t07_totalvalorventagravadas)
            writer.WriteElementString("cbc:InstructionID", Item.t07_totalvalorventagravadas_c11)
            writer.WriteEndElement()

            'Se establece el valor de ventas Exoneradas
            If Item.t08_totalvalorventaexoneradas > 0 Then
                writer.WriteStartElement("sac:BillingPayment")
                Tools.TagNodoAtributoValorValor(writer, "cbc:PaidAmount", "currencyID", "PEN", Item.t08_totalvalorventaexoneradas)
                writer.WriteElementString("cbc:InstructionID", Item.t08_totalvalorventaexoneradas_c11)
                writer.WriteEndElement()
            End If

            'Se establece el valor de operaciones inafectas
            If Item.t09_totalvalorventainafectas > 0 Then
                writer.WriteStartElement("sac:BillingPayment")
                Tools.TagNodoAtributoValorValor(writer, "cbc:PaidAmount", "currencyID", "PEN", Item.t09_totalvalorventainafectas)
                writer.WriteElementString("cbc:InstructionID", Item.t09_totalvalorventainafectas_c11)
                writer.WriteEndElement()
            End If

            'Se estalece el importe total otros cargos
            If Item.t11_importetotalotroscargos > 0 Then
                writer.WriteStartElement("cac:AllowanceCharge")
                writer.WriteElementString("cbc:ChargeIndicator", Item.t11_indicadorcargo)
                Tools.TagNodoAtributoValorValor(writer, "cbc:Amount", "currencyID", "PEN", Item.t11_importetotalotroscargos)
                writer.WriteEndElement()
            End If

            'Se estable el impuesto ISC 2000
            If Item.t12_totalisc > 0 Then
                writer.WriteStartElement("cac:TaxTotal")
                Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t12_totalisc)
                writer.WriteStartElement("cac:TaxSubtotal")
                Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t12_totalisc_item)
                writer.WriteStartElement("cac:TaxCategory")
                writer.WriteStartElement("cac:TaxScheme")
                writer.WriteElementString("cbc:ID", Item.t12_totalisc_codtributo_c05)
                writer.WriteElementString("cbc:Name", Item.t12_totalisc_nomtributo_c05.ToString.Trim)
                writer.WriteElementString("cbc:TaxTypeCode", Item.t12_totalisc_codintertributo_c05)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            'Se establece impuesto IGV
            writer.WriteStartElement("cac:TaxTotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t13_totaligv)
            writer.WriteStartElement("cac:TaxSubtotal")
            Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t13_totaligvitem)
            writer.WriteStartElement("cac:TaxCategory")
            writer.WriteStartElement("cac:TaxScheme")
            writer.WriteElementString("cbc:ID", Item.t13_totaligv_codtributo_c05)
            writer.WriteElementString("cbc:Name", Item.t13_totaligv_nomtributo_c05.ToString.Trim)
            writer.WriteElementString("cbc:TaxTypeCode", Item.t13_totaligv_codintertributo_c05)
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()

            'Se obtiene otros tributos
            If Item.t14_totalotrostributos > 0 Then
                writer.WriteStartElement("cac:TaxTotal")
                Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t14_totalotrostributos)
                writer.WriteStartElement("cac:TaxSubtotal")
                Tools.TagNodoAtributoValorValor(writer, "cbc:TaxAmount", "currencyID", "PEN", Item.t14_totalotrostributos_item)
                writer.WriteStartElement("cac:TaxCategory")
                writer.WriteStartElement("cac:TaxScheme")
                writer.WriteElementString("cbc:ID", Item.t14_totalotrostributos_codintertributo_c05)
                writer.WriteElementString("cbc:Name", Item.t14_totalotrostributos_nomtributo_c05.ToString.Trim)
                writer.WriteElementString("cbc:TaxTypeCode", Item.t14_totalotrostributos_codintertributo_c05)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If

            writer.WriteEndElement()
        Next

        'Se finaliza el CreditNote
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
        SaveRutaArchivoXML(ResumenBE.idresumen, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se retorna el nombre de archivo
        Return RutaArchivo & NombreArchivo & ExtensionArchivoXML

    End Function
    Public Function SignatureXML(IDResumen As Int32) As String
        Dim RutaCertificado As String = EmisorBE.RutaCarpetaArchivosCertificados
        Dim ClaveCertificado As String = EmisorBE.ClaveCertificado
        Dim Result As String = String.Empty

        'Se obtiene la entidad
        ResumenBE = Me.GetByID(IDResumen)

        Try
            Dim local_xmlArchivo As String = ResumenBE.RutaComprobanteXML
            Dim local_nombreXML As String = System.IO.Path.GetFileName(local_xmlArchivo)
            Dim local_typoDocumento As String = "RC"

            Dim MiCertificado As X509Certificate2 = New X509Certificate2(RutaCertificado, ClaveCertificado)
            Dim xmlDoc As XmlDocument = New XmlDocument()
            xmlDoc.PreserveWhitespace = True
            xmlDoc.Load(local_xmlArchivo)

            Dim signedXml As SignedXml = New SignedXml(xmlDoc)
            signedXml.SigningKey = MiCertificado.PrivateKey

            Dim KeyInfo As KeyInfo = New KeyInfo()

            Dim Reference As Reference = New Reference()
            Reference.Uri = ""

            Reference.AddTransform(New XmlDsigEnvelopedSignatureTransform())

            signedXml.AddReference(Reference)

            Dim X509Chain As X509Chain = New X509Chain()
            X509Chain.Build(MiCertificado)

            Dim local_element As X509ChainElement = X509Chain.ChainElements(0)
            Dim x509Data As KeyInfoX509Data = New KeyInfoX509Data(local_element.Certificate)
            Dim subjectName As String = local_element.Certificate.Subject

            x509Data.AddSubjectName(subjectName)
            KeyInfo.AddClause(x509Data)

            signedXml.KeyInfo = KeyInfo
            signedXml.ComputeSignature()

            Dim signature As XmlElement = signedXml.GetXml()
            signature.Prefix = "ds"
            signedXml.ComputeSignature()

            For Each node As XmlNode In signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']")
                If node.LocalName = "Signature" Then
                    Dim newAttribute As XmlAttribute = xmlDoc.CreateAttribute("Id")
                    newAttribute.Value = "SignatureSP"
                    node.Attributes.Append(newAttribute)
                    Exit For
                End If
            Next node

            Dim local_xpath As String = String.Empty
            Dim nsMgr As XmlNamespaceManager
            nsMgr = New XmlNamespaceManager(xmlDoc.NameTable)
            nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
            nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2")
            nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance")

            Select Case local_typoDocumento
                Case "01", "03" 'factura / boleta
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
                    local_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

                Case "07" 'nota de credito
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")
                    local_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

                Case "08" 'nota de debito
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2")
                    local_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent"

                Case "RA"  'Communicacion de baja
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1")
                    local_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"

                Case "RC" 'Resumen de diario
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1")
                    local_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent"
            End Select

            nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
            nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
            nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
            nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
            nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
            nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#")

            xmlDoc.SelectSingleNode(local_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, True))
            xmlDoc.Save(local_xmlArchivo)

            Dim nodeList As XmlNodeList = xmlDoc.GetElementsByTagName("ds:Signature")

            'el nodo <ds:Signature> debe existir unicamente 1 vez
            If nodeList.Count <> 1 Then
                Throw New Exception("Se produjo un error en la firma del documento")
            End If
            signedXml.LoadXml(CType(nodeList(0), XmlElement))

            'verificacion de la firma generada
            If signedXml.CheckSignature() = False Then
                Throw New Exception("Se produjo un error en la firma del documento")
            End If

            'Se recupera el valor de la firma
            Dim ValorFirma As String = String.Empty
            Dim Valor As String
            'Se obtiene el archivo XML
            Dim ReaderXML As XmlTextReader = New XmlTextReader(local_xmlArchivo)

            'Se obtiene el valor de la firma
            While (ReaderXML.Read())

                'Se lee la raiz de cada nodo
                Valor = ReaderXML.Name

                Select Case ReaderXML.NodeType
                    Case XmlNodeType.Element

                        'Se obtiene el hash DigisValue, del archivo XML Firmado
                        If ReaderXML.Name = "DigestValue" Then
                            ReaderXML.Read()
                            ValorFirma = ReaderXML.Value
                            Exit While
                        End If

                        ''Se obtiene el valor de la firma del archivo XML Firmado
                        'If ReaderXML.Name = "SignatureValue" Then
                        '    ReaderXML.Read()
                        '    ValorFirma = ReaderXML.Value
                        '    Exit While
                        'End If

                        'Se obtiene el texto de cada elemento. No Se usa
                    Case XmlNodeType.Text
                        'Console.WriteLine(ReaderXML.Value)

                        'Se obtiene el fin del elemento. No se usa
                    Case XmlNodeType.EndElement
                        'Console.Write("</" + ReaderXML.Name)
                        'Console.WriteLine(">")
                End Select
            End While

            'Se guarda el valor de la firma
            Me.SaveValorFirma(IDResumen, ValorFirma)

        Catch ex As Exception
            Throw ex
        End Try
        Return Result
    End Function
    Public Function ZipXML(IDResumen As Int32) As String

        'Se obtiene la entidad
        ResumenBE = Me.GetByID(IDResumen)

        'Se obtiene el nombre del archivo zip
        Dim RutaArchivoZIP As String = Path.ChangeExtension(ResumenBE.RutaComprobanteXML, "zip")

        Using zipToOpen As FileStream = New FileStream(RutaArchivoZIP, FileMode.Create)
            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(Path.GetFileName(ResumenBE.RutaComprobanteXML))
                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(ResumenBE.RutaComprobanteXML))
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

    Public Function GetByID(IDResumen As Int32) As ResumenBE2018
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim ResumenBE As New ResumenBE2018
        Dim ResumenLinea As New List(Of ResumenLinea2018)

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_id"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then

                While dr.Read
                    With ResumenBE
                        .idresumen = dr.ReadNullAsEmptyString("idresumen")
                        .t03_fecemision = dr.ReadNullAsEmptyString("t03_fecemision")
                        .t17_numcorrelativo = dr.ReadNullAsEmptyString("t17_numcorrelativo")
                        .t18_fecresumen = dr.ReadNullAsEmptyString("t18_fecresumen")
                        .t20_versionubl = dr.ReadNullAsEmptyString("t20_versionubl")
                        .t21_versiondoc = dr.ReadNullAsEmptyString("t21_versiondoc")
                        .RutaComprobanteXML = dr.ReadNullAsEmptyString("archivoxml")
                        .RutaRespuestaSunatXML = dr.ReadNullAsEmptyString("rutarespuestasunatxml")
                        .Observacion = dr.ReadNullAsEmptyString("observacion")
                        .CodigoRespuesta = dr.ReadNullAsEmptyString("codigorespuesta")
                        .Ticket = dr.ReadNullAsEmptyString("ticket")
                        .ValorFirma = dr.ReadNullAsEmptyString("digestvalue")
                        .RutaComprobanteXML = dr.ReadNullAsEmptyString("archivoxml")
                        .RutaComprobanteZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")
                        .RutaComprobantePDF = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "pdf")
                        .RutaRespuestaSunatXML = dr.ReadNullAsEmptyString("rutarespuestasunatxml")
                        .RutaRespuestaSunatZIP = Path.ChangeExtension(dr.ReadNullAsEmptyString("Archivoxml"), "zip")
                    End With
                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        Dim ResumenItem As New ResumenLinea2018

                        With ResumenItem
                            .idresumendetalle = dr.ReadNullAsEmptyString("idresumendetalle")
                            .idresumen = dr.ReadNullAsEmptyString("idresumen")
                            .t01_numfila = dr.ReadNullAsEmptyString("t01_numfila")
                            .t02_tipdoc_c01 = dr.ReadNullAsEmptyString("t02_tipdoc_c01")
                            .t02_numcorrelativo = dr.ReadNullAsEmptyString("t02_numcorrelativo")
                            .t03_numdoc_adquiriente = dr.ReadNullAsEmptyString("t03_numdoc_adquiriente")
                            .t03_tipdoc_adquiriente = dr.ReadNullAsEmptyString("t03_tipdoc_adquiriente")
                            .t04_numcorrelativo_modifica = dr.ReadNullAsEmptyString("t04_numcorrelativo_modifica")
                            .t04_tipdoc_c01 = dr.ReadNullAsEmptyString("t04_tipdoc_c01")
                            .t05_estadoitem_c19 = dr.ReadNullAsEmptyString("t05_estadoitem_c19")
                            .t06_importetotal = dr.ReadNullAsEmptyString("t06_importetotal")
                            .t07_totalvalorventagravadas = dr.ReadNullAsEmptyString("t07_totalvalorventagravadas")
                            .t07_totalvalorventagravadas_c11 = dr.ReadNullAsEmptyString("t07_totalvalorventagravadas_c11")
                            .t08_totalvalorventaexoneradas = dr.ReadNullAsEmptyString("t08_totalvalorventaexoneradas")
                            .t08_totalvalorventaexoneradas_c11 = dr.ReadNullAsEmptyString("t08_totalvalorventaexoneradas_c11")
                            .t09_totalvalorventainafectas = dr.ReadNullAsEmptyString("t09_totalvalorventainafectas")
                            .t09_totalvalorventainafectas_c11 = dr.ReadNullAsEmptyString("t09_totalvalorventainafectas_c11")
                            .t10_totalvalorventasgratuitas = dr.ReadNullAsEmptyString("t10_totalvalorventasgratuitas")
                            .t10_totalvalorventasgratuitas_c11 = dr.ReadNullAsEmptyString("t10_totalvalorventasgratuitas_c11")
                            .t11_indicadorcargo = dr.ReadNullAsEmptyString("t11_indicadorcargo")
                            .t11_importetotalotroscargos = dr.ReadNullAsEmptyString("t11_importetotalotroscargos")
                            .t12_totalisc = dr.ReadNullAsEmptyString("t12_totalisc")
                            .t12_totalisc_item = dr.ReadNullAsEmptyString("t12_totalisc_item")
                            .t12_totalisc_codtributo_c05 = dr.ReadNullAsEmptyString("t12_totalisc_codtributo_c05")
                            .t12_totalisc_nomtributo_c05 = dr.ReadNullAsEmptyString("t12_totalisc_nomtributo_c05")
                            .t12_totalisc_codintertributo_c05 = dr.ReadNullAsEmptyString("t12_totalisc_codintertributo_c05")
                            .t13_totaligv = dr.ReadNullAsEmptyString("t13_totaligv")
                            .t13_totaligvitem = dr.ReadNullAsEmptyString("t13_totaligvitem")
                            .t13_totaligv_codtributo_c05 = dr.ReadNullAsEmptyString("t13_totaligv_codtributo_c05")
                            .t13_totaligv_nomtributo_c05 = dr.ReadNullAsEmptyString("t13_totaligv_nomtributo_c05")
                            .t13_totaligv_codintertributo_c05 = dr.ReadNullAsEmptyString("t13_totaligv_codintertributo_c05")
                            .t14_totalotrostributos = dr.ReadNullAsEmptyString("t14_totalotrostributos")
                            .t14_totalotrostributos_item = dr.ReadNullAsEmptyString("t14_totalotrostributos_item")
                            .t14_totalotrostributos_codtributo_c05 = dr.ReadNullAsEmptyString("t14_totalotrostributos_codtributo_c05")
                            .t14_totalotrostributos_nomtributo_c05 = dr.ReadNullAsEmptyString("t14_totalotrostributos_nomtributo_c05")
                            .t14_totalotrostributos_codintertributo_c05 = dr.ReadNullAsEmptyString("t14_totalotrostributos_codintertributo_c05")
                        End With
                        ResumenLinea.Add(ResumenItem)
                    End While
                End If

                ResumenBE.Lineas = ResumenLinea
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return ResumenBE
    End Function
    Public Function GetByReporteID() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim dr As SqlDataReader = Nothing

        Dim FacturaBE As New FacturaBE
        Dim FacturaDet As New FacturaItem
        Dim FacturaAnticipo As New FacturaAnticipo

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_rpt_id"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = Me.IDResumen
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
    Public Function GetByIDXML(IDResumen As Int32) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As System.Xml.XmlReader = Nothing
        Dim doc As New XmlDocument()
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_id_xml"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
            .CommandText = "coe_resumen_get_all"
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

    Public Function GetByPendientesFirmar(FecEmision As Date) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_001"
            .Parameters.Add("@FecEmision", SqlDbType.Date).Value = FecEmision
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

    Public Function GetByAll2(Tipo As eGetResumen) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_all_2"
            .Parameters.Add("@tipo", SqlDbType.Int).Value = Tipo
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
    Public Function GetResumenBoletasPendientes(FechaEmision As Date) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_pendientes"
            .Parameters.Add("@fechaemision", SqlDbType.Date).Value = FechaEmision
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

    Public Function GetFechaEmision() As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim FechaEmision As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_get_fechaemision"
        End With

        Try
            cnx.Open()
            FechaEmision = cmd.ExecuteScalar.ToString
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return FechaEmision
    End Function
    Public Function SaveResumen(FechaEmision As Date, IDUsuario As Int32, NombreMaquina As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandTimeout = 0
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_insert"
            .Parameters.Add("@fechaemision", SqlDbType.Date).Value = FechaEmision
            .Parameters.Add("@idusuario", SqlDbType.Int).Value = IDUsuario
            .Parameters.Add("@maquina", SqlDbType.VarChar, 50).Value = NombreMaquina
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
    Public Function SaveRutaArchivoXML(IDResumen As Int32, RutaCarpetaXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_ruta_update"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
    Public Function SaveRutaArchivoCdrXML(IDResumen As Int32, RutaCarpetaCdrXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_ruta_cdr_update"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
    Public Function SaveValorFirma(IDResumen As Int32, ValorFirma As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_firma_update"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
    Public Function SaveConstanciaRecepcionZIP(IDResumen As Int32, ContenidoArchivoZIP As Byte()) As Boolean
        Dim Result As Boolean = False
        Dim NombreArchivo As String = String.Empty

        Try
            'Se obtiene la entidad
            ResumenBE = Me.GetByID(IDResumen)

            'Se obtiene el nombre del archivo xml, sin extension
            NombreArchivo = Path.GetFileNameWithoutExtension(ResumenBE.RutaComprobanteXML)

            'Se agrega la letra R al nombre (Respuesta) y se coloca la extension .zip
            NombreArchivo = "R-" & Path.ChangeExtension(NombreArchivo, ".zip")

            'Se concatena la ruta y el nuevo nombre del archivo zip
            NombreArchivo = Path.GetDirectoryName(ResumenBE.RutaComprobanteXML) & "\" & NombreArchivo

            'Se guarda el archivo zip que envia la sunat
            System.IO.File.WriteAllBytes(NombreArchivo, ContenidoArchivoZIP)

            'Se descomprime el archivo ZIP y extrae el XML
            Me.UnZipXML(NombreArchivo)

            'Se guarda el nombre del archivo de la constancia de recepcion CDR SUNAT
            Me.SaveRutaArchivoCdrXML(IDResumen, Path.ChangeExtension(NombreArchivo, ".xml"))

            'Se lee el contenido del archivo XML y se lee contenido
            Me.SaveConstanciaRecepcionXML(IDResumen)

            Result = True
        Catch ex As Exception
            Throw New Exception("No se guardo el archivo de respuesta de la SUNAT. " & ex.Message)
        End Try

        Return Result
    End Function
    Public Function SaveConstanciaRecepcionXML(IDResumen As Int32) As Boolean
        Dim Result As Boolean = False
        Dim Valor As String = String.Empty
        Dim CodigoRespuesta As String = String.Empty
        Dim NumeroComprobante As String = String.Empty
        Dim Descripcion As String = String.Empty

        'Se obtiene la entidad
        ResumenBE = Me.GetByID(IDResumen)

        'Se obtiene el archivo XML
        Dim ReaderXML As XmlTextReader = New XmlTextReader(ResumenBE.RutaRespuestaSunatXML)

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
            .CommandText = "coe_resumen_estado_update"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
            .CommandText = "coe_resumen_excepcion_upd"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = BE.IDComprobante
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
    Public Function SaveEstadoWeb(IDResumen As Int32, IDEstadoWeb As eEstadoWeb, FechaWeb As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_estado_web"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDResumen
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
    Public Function SaveTicket(Ticket As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        'Se guarda el estado del ticket en proceso
        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_ticket_update"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = Me.IDResumen
            .Parameters.Add("@ticket", SqlDbType.VarChar, 100).Value = Ticket
            .Parameters.Add("@estado", SqlDbType.Int).Value = eEstadoSunat.PendienteCDR
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

    Public Function SaveEstadoComprobantes(IDREsumen As Int32) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        'Se guarda el estado del ticket en proceso
        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_resumen_upd_01"
            .Parameters.Add("@idresumen", SqlDbType.Int).Value = IDREsumen
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
