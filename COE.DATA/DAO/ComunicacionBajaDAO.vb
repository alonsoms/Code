Imports COE.FRAMEWORK
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Xml
Imports System.Xml
Imports System.Data.SqlClient
Imports System.Text

Public Enum eGetComunicacion
    ComunicacionPendientesEnvio = 1
    TicketsPendientesCDR = 2
End Enum


Public Class ComunicacionBajaDAO
    Public Property IDComunicacion As Int32
    Dim SistemaDAO As New SistemaDAO
    Dim EmisorDAO As New EmisorDAO
    Dim EmisorBE As New EmisorBE
    Dim ComunicacionBajaBE As New ComunicacionBajaBE


    Public Function CreateXML(IDComunicacion As Int32) As String

        EmisorBE = EmisorDAO.GetByID(1)

        'Se genera el nombre y ruta del archivo XML. En  el manual del programador se encuentra el formato del nombre de archivo
        'Se obtiene la ruta y carpeta donde se guarda los archivos de la sunat
        Dim RutaArchivo As String = SistemaDAO.GetRutaCarpetaSUNAT(EmisorBE)
        Dim NombreArchivo As String = String.Empty
        Dim ExtensionArchivoXML As String = ".xml"

        'Se obtiene la entidad
        ComunicacionBajaBE = Me.GetByID(IDComunicacion)

        'Se obtiene el nombre del archivo XML
        NombreArchivo = EmisorBE.NumeroRUC
        NombreArchivo &= "-RA-" & ComunicacionBajaBE.t10_feccomunicacion.Replace("-", "") & "-" & ComunicacionBajaBE.t09_numcorrelativo

        'Se crea el documento XML con todas las propiedades requeridas por la sunat. 
        'A pesar que el encoding esta en Mayuscula, lo pasa como minusculas. Mas abajo se hace la correcion
        Dim writer As New XmlTextWriter(RutaArchivo & NombreArchivo & ExtensionArchivoXML, Encoding.GetEncoding("ISO-8859-1"))
        writer.WriteStartDocument(False)  'este deberia colocar el stalone=no, si no aparece colocar Nothing
        writer.Formatting = Formatting.Indented
        writer.Indentation = 0

        'Se crea el nodo raiz
        writer.WriteStartElement("VoidedDocuments")
        writer.WriteAttributeString("xmlns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1")
        writer.WriteAttributeString("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")
        writer.WriteAttributeString("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")
        writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#")
        writer.WriteAttributeString("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")
        writer.WriteAttributeString("xmlns:sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1")
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")

        'Se define este tag es para guardar la firma digital
        writer.WriteStartElement("ext:UBLExtensions")
        writer.WriteStartElement("ext:UBLExtension")
        writer.WriteStartElement("ext:ExtensionContent")
        writer.WriteEndElement()
        writer.WriteEndElement()
        writer.WriteEndElement()

        'La version del UBLVersionID es 2.0
        writer.WriteElementString("cbc:UBLVersionID", ComunicacionBajaBE.t12_versionubl)

        'La version del CustomizationID es 1.0
        writer.WriteElementString("cbc:CustomizationID", ComunicacionBajaBE.t13_versiondoc)

        'Se establece el id del documentos
        writer.WriteElementString("cbc:ID", "RA-" & ComunicacionBajaBE.t10_feccomunicacion.Replace("-", "") & "-" & ComunicacionBajaBE.t09_numcorrelativo)

        'Se coloca la fecha de emision de los comprobantes
        writer.WriteElementString("cbc:ReferenceDate", ComunicacionBajaBE.t03_fecemisiondoc)

        'Se coloca le fecha cuando se genera el resumen
        writer.WriteElementString("cbc:IssueDate", ComunicacionBajaBE.t10_feccomunicacion)

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
        For Each Item In ComunicacionBajaBE.Detalle

            'Se define los rangos de los comprobantes Inicio y Fin
            writer.WriteStartElement("sac:VoidedDocumentsLine")
            writer.WriteElementString("cbc:LineID", Item.t08_numordenitem)
            writer.WriteElementString("cbc:DocumentTypeCode", Item.t04_tipdoc_c01)
            writer.WriteElementString("sac:DocumentSerialID", Item.t05_serdocbaja)
            writer.WriteElementString("sac:DocumentNumberID", Item.t06_numdocbaja)
            writer.WriteElementString("sac:VoidReasonDescription", Item.t07_motivobaja)
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
        SaveRutaArchivoXML(IDComunicacion, RutaArchivo & NombreArchivo & ExtensionArchivoXML)

        'Se retorna el nombre de archivo
        Return RutaArchivo & NombreArchivo & ExtensionArchivoXML
    End Function
    Public Function SignatureXML(IDComunicacion As Int32) As String
        Dim RutaCertificado As String = EmisorBE.RutaCarpetaArchivosCertificados
        Dim ClaveCertificado As String = EmisorBE.ClaveCertificado
        Dim Result As String = String.Empty

        'Se obtiene la entidad
        ComunicacionBajaBE = Me.GetByID(IDComunicacion)

        Try
            Dim local_xmlArchivo As String = ComunicacionBajaBE.RutaComprobanteXML
            Dim local_nombreXML As String = System.IO.Path.GetFileName(local_xmlArchivo)
            Dim local_typoDocumento As String = "RA"

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
            Me.SaveValorFirma(IDComunicacion, ValorFirma)

        Catch ex As Exception
            Throw ex
        End Try
        Return Result
    End Function
    Public Function ZipXML(IDComunicacion As Int32) As String

        'Se obtiene la entidad
        ComunicacionBajaBE = Me.GetByID(IDComunicacion)

        'Se obtiene el nombre del archivo zip
        Dim RutaArchivoZIP As String = Path.ChangeExtension(ComunicacionBajaBE.RutaComprobanteXML, "zip")

        Using zipToOpen As FileStream = New FileStream(RutaArchivoZIP, FileMode.Create)
            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(Path.GetFileName(ComunicacionBajaBE.RutaComprobanteXML))
                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(ComunicacionBajaBE.RutaComprobanteXML))
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

    Public Function GetByID(IDComunicacion As Int32) As ComunicacionBajaBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim ComunicacionBajaBE As New ComunicacionBajaBE
        Dim ComunicacionBajaDetalle As New List(Of ComunicacionBajaItem)
        Dim dr As SqlDataReader = Nothing

        'Se configura el comando
        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_get_id"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then

                While dr.Read
                    With ComunicacionBajaBE
                        .idcomunicacion = dr.ReadNullAsEmptyString("idcomunicacion")
                        .t03_fecemisiondoc = dr.ReadNullAsEmptyString("t03_fecemisiondoc")
                        .t09_numcorrelativo = dr.ReadNullAsEmptyString("t09_numcorrelativo")
                        .t10_feccomunicacion = dr.ReadNullAsEmptyString("t10_feccomunicacion")
                        .t12_versionubl = dr.ReadNullAsEmptyString("t12_versionubl")
                        .t13_versiondoc = dr.ReadNullAsEmptyString("t13_versiondoc")
                        .estado = dr.ReadNullAsEmptyString("estado")
                        .NumeroTicket = dr.ReadNullAsEmptyString("ticket")
                        .CodigoRespuesta = dr.ReadNullAsEmptyString("codigorespuesta")
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
                    End With
                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read
                        Dim ComunicacionBajaItem As New ComunicacionBajaItem
                        With ComunicacionBajaItem
                            .idcomunicaciondetalle = dr.ReadNullAsEmptyString("idcomunicaciondetalle")
                            .idcomunicacion = dr.ReadNullAsEmptyString("idcomunicacion")
                            .t04_tipdoc_c01 = dr.ReadNullAsEmptyString("t04_tipdoc_c01")
                            .t05_serdocbaja = dr.ReadNullAsEmptyString("t05_serdocbaja")
                            .t06_numdocbaja = dr.ReadNullAsEmptyString("t06_numdocbaja")
                            .t07_motivobaja = dr.ReadNullAsEmptyString("t07_motivobaja")
                            .t08_numordenitem = dr.ReadNullAsEmptyString("t08_numordenitem")
                        End With

                        ComunicacionBajaDetalle.Add(ComunicacionBajaItem)
                    End While
                End If
                ComunicacionBajaBE.Detalle = ComunicacionBajaDetalle
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return ComunicacionBajaBE
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
            .CommandText = "coe_comunicacion_baja_rpt_id"
            .Parameters.Add("@idfactura", SqlDbType.Int).Value = Me.IDComunicacion
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
    Public Function GetByIDXML(IDComunicacion As Int32) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As System.Xml.XmlReader = Nothing
        Dim doc As New XmlDocument()
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_get_id_xml"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
            .CommandText = "coe_comunicacion_baja_get_all"
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

    Public Function GetByAll2(Tipo As eGetComunicacion) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_get_all_2"
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

    Public Function GetFechaEmision() As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim FechaEmision As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_get_fechaemision"
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

    Public Function SaveComunicacionBajaXML(FechaEmision As Date, IDUsuario As Int32, NombreMaquina As String) As Int32
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand

        'Se genera la comunicacion de baja, de los comprobantes anulados en el dia como Facturas, Boletas de Venta y Notas de Credito
        'Se pasa el parametro de fecha de emision, pero se genera del dia anterior 
        'Se guarda el estado del documento en 1=Pendiente de envio
        With cmd
            .Connection = cnx
            .CommandTimeout = 0
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_insert"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Direction = ParameterDirection.Output
            .Parameters.Add("@fechaemision", SqlDbType.Date).Value = FechaEmision
            .Parameters.Add("@idusuario", SqlDbType.Int).Value = IDUsuario
            .Parameters.Add("@maquina", SqlDbType.VarChar, 50).Value = NombreMaquina
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Me.IDComunicacion = cmd.Parameters("@IDComunicacion").Value
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Me.IDComunicacion
    End Function
    Public Function SaveRutaArchivoXML(IDComunicacion As Int32, RutaCarpetaXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_ruta_update"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
    Public Function SaveRutaArchivoCdrXML(IDComunicacion As Int32, RutaCarpetaCdrXML As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_ruta_cdr_update"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
    Public Function SaveValorFirma(IDComunicacion As Int32, ValorFirma As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_firma_update"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
    Public Function SaveConstanciaRecepcionZIP(IDComunicacion As Int32, ContenidoArchivoZIP As Byte()) As Boolean
        Dim Result As Boolean = False
        Dim NombreArchivo As String = String.Empty

        Try
            'Se obtiene la entidad
            ComunicacionBajaBE = Me.GetByID(IDComunicacion)

            'Se obtiene el nombre del archivo xml, sin extension
            NombreArchivo = Path.GetFileNameWithoutExtension(ComunicacionBajaBE.RutaComprobanteXML)

            'Se agrega la letra R al nombre (Respuesta) y se coloca la extension .zip
            NombreArchivo = "R-" & Path.ChangeExtension(NombreArchivo, ".zip")

            'Se concatena la ruta y el nuevo nombre del archivo zip
            NombreArchivo = Path.GetDirectoryName(ComunicacionBajaBE.RutaComprobanteXML) & "\" & NombreArchivo

            'Se guarda el archivo zip que envia la sunat
            System.IO.File.WriteAllBytes(NombreArchivo, ContenidoArchivoZIP)

            'Se descomprime el archivo ZIP y extrae el XML
            Me.UnZipXML(NombreArchivo)

            'Se guarda el nombre del archivo de la constancia de recepcion CDR SUNAT
            Me.SaveRutaArchivoCdrXML(IDComunicacion, Path.ChangeExtension(NombreArchivo, ".xml"))

            'Se lee el contenido del archivo XML y se lee contenido
            Me.SaveConstanciaRecepcionXML(IDComunicacion)

            Result = True
        Catch ex As Exception
            Throw New Exception("No se guardo el archivo de respuesta de la SUNAT. " & ex.Message)
        End Try

        Return Result
    End Function
    Public Function SaveConstanciaRecepcionXML(IDComunicacion As Int32) As Boolean
        Dim Result As Boolean = False
        Dim Valor As String = String.Empty
        Dim CodigoRespuesta As String = String.Empty
        Dim NumeroComprobante As String = String.Empty
        Dim Descripcion As String = String.Empty

        'Se obtiene la entidad
        ComunicacionBajaBE = Me.GetByID(IDComunicacion)

        'Se obtiene el archivo XML
        Dim ReaderXML As XmlTextReader = New XmlTextReader(ComunicacionBajaBE.RutaRespuestaSunatXML)

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
            .CommandText = "coe_comunicacion_baja_estado_update"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
            .CommandText = "coe_comunicacion_baja_excepcion_upd"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = BE.IDComprobante
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
    Public Function SaveEstadoWeb(IDComunicacion As Int32, IDEstadoWeb As eEstadoWeb, FechaWeb As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comunicacion_baja_estado_web"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = IDComunicacion
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
            .CommandText = "coe_comunicacion_baja_ticket_update"
            .Parameters.Add("@idcomunicacion", SqlDbType.Int).Value = Me.IDComunicacion
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
End Class
