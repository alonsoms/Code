Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports System.Xml
Imports System.IO

Public Class FirmaXMLDAO
    Public Sub Create(EmisorBE As EmisorBE, PathArchivoXML As String, TipoComprobante As String)
        Dim xmlDoc As XmlDocument = New XmlDocument()
        Dim xmlPath As String = String.Empty
        Dim NameArchivoXML As String = String.Empty

        Try
            'Se valida que exista el certificado
            If Not File.Exists(EmisorBE.RutaCarpetaArchivosCertificados) Then
                Throw New Exception("No existe el certificado.")
            End If

            'Se valida que exista el archivo XML
            If Not File.Exists(PathArchivoXML) Then
                Throw New Exception("No existe el archivo XML")
            End If

            'Se obtiene solo el nombre del archivo XML, se quita el Path
            NameArchivoXML = System.IO.Path.GetFileName(PathArchivoXML)

            'Se genera objeto certificado para firmar los documentos XML
            Dim MiCertificado As X509Certificate2 = New X509Certificate2(EmisorBE.RutaCarpetaArchivosCertificados, EmisorBE.ClaveCertificado)

            'Se carga el archivo XML
            xmlDoc.PreserveWhitespace = True
            xmlDoc.Load(PathArchivoXML)

            'Se crea el objeto firma a partir del documento XML
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

            Dim nsMgr As XmlNamespaceManager
            nsMgr = New XmlNamespaceManager(xmlDoc.NameTable)
            nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
            nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2")
            nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance")

            'Se agrega los namespace de acuerdo al tipo de comprobante
            Select Case TipoComprobante
                Case "01", "03" 'Factura / Boleta de Venta
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")
                    xmlPath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"

                Case "07" 'Nota de credito
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2")
                    xmlPath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"


                Case "08" 'Nota de debito
                    nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2")
                    xmlPath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"

                Case "RA"  'Communicacion de baja
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1")
                    xmlPath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent"

                Case "RC" 'Resumen de diario de boletas
                    nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1")
                    xmlPath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension[1]/ext:ExtensionContent"
            End Select

            'Se agrega namespace adicionales
            nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
            nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2")
            nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
            nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2")
            nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
            nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#")


            'Se guarda el archivo XML con la firma generada
            xmlDoc.SelectSingleNode(xmlPath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, True))

            'xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signature, True))
            xmlDoc.Save(PathArchivoXML)

            Dim nodeList As XmlNodeList = xmlDoc.GetElementsByTagName("ds:Signature")

            'Se valida que el nodo <ds:Signature> debe existir unicamente 1 vez
            If nodeList.Count <> 1 Then
                Throw New Exception("Se produjo un error en la firma del documento")
            End If
            signedXml.LoadXml(CType(nodeList(0), XmlElement))

            'Se valida si la firma fue generada
            If signedXml.CheckSignature() = False Then
                Throw New Exception("Se produjo un error en la firma del documento")
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
