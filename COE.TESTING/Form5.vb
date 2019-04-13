Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports COE.DATA
Imports COE.FRAMEWORK
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob

Public Class Form5
    Dim EmisorDAO As New EmisorDAO
    Dim ComprobanteWebDAO As New ComprobanteWebDAO
    Dim ServicioDAO As New ServicioDAO
    Dim FacturaDAO As New FacturaDAO
    Dim BoletaDAO As New BoletaVentaDAO
    Dim NotaCreditoDAO As New NotaCreditoDAO
    Dim NotaDebitoDAO As New NotaDebitoDAO
    Private Sub btnEnviarComprobantes_Click(sender As Object, e As EventArgs) Handles btnEnviarComprobantes.Click

        'Se lee el archivo de Config.XML Se carga las cadenas de conexion a las base de datos
        EmisorDAO.GetByConfigXML()


        EnviarWeb()
    End Sub
    Public Sub EnviarWeb()
        Dim ServicioDAO As New ServicioDAO

        Dim IDServicioComprobante As Int32 = 0
        Dim TipoComprobante As String = String.Empty
        Dim IDComprobante As Int32

        Dim FacturaDAO As New FacturaDAO
        Dim BoletaDAO As New BoletaVentaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO
        Dim NotaDebitoDAO As New NotaDebitoDAO

        Dim dt As New DataTable
        Dim ComprobanteBE As New Object
        Dim Result As Boolean = False


        Try

            For Index = 0 To EmisorDAO.EmisorConfigXML.Count - 1

                'Se establece la cadena de conexion por cada emisor del Config.XML
                ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML(Index).ConexionDB

                'Se obtiene los datos del Emisor
                EmisorDAO.GetByID()

                'Se establece la conexion a la base de datos de Azure
                ConexionDAO.ConexionDBCloud = EmisorDAO.EmisorBE.ConexionDataBaseCloud


                'Se obtiene los comprobantes para enviarlos a la Web Azure
                dt = ServicioDAO.GetByIDServicio(eServicio.EnviarWeb)

                For Each dr As DataRow In dt.Rows
                    Result = False

                    Try
                        IDServicioComprobante = dr("IDServicioComprobante")
                        TipoComprobante = dr("TipoComprobante")
                        IDComprobante = dr("IDComprobante")

                        'Se crea guarda el comprobante en la web, segun el tipo de comprobante
                        Select Case TipoComprobante
                            Case "01"
                                If FacturaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                                    If FacturaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                                        FacturaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                                        Result = True
                                    End If
                                End If
                            Case "03"
                                If BoletaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                                    If BoletaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                                        BoletaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                                        Result = True
                                    End If
                                End If
                            Case "07"
                                If NotaCreditoDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                                    If NotaCreditoDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                                        NotaCreditoDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                                        Result = True
                                    End If
                                End If
                        End Select

                        If Result Then
                            'Se elimina la tarea de Envio Web
                            ServicioDAO.Delete(IDServicioComprobante)
                        End If
                    Catch ex As Exception
                        ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.EnviarWeb, "ServicioDAO.EnviarWeb :" & ex.Message)
                    End Try
                Next

            Next
        Catch ex As Exception
            Tools.SaveLog("COE SERVICE WEB", ex.Message, EventLogEntryType.Error)
        Finally
            'Tiempo.Start()
        End Try

    End Sub


    Public Sub FirmarComprobantes()
        'Dim TipoComprobante As String = String.Empty
        'Dim IDComprobante As Int32
        'Dim IDServicioComprobante As Int32
        'Dim dt As New DataTable

        'Try

        '    'Se procesa cada emisor de la lista
        '    For Index = 0 To EmisorDAO.EmisorConfigXML.Count - 1

        '        'Se establece la cadena de conexion por cada emisor del Config.XML
        '        ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML(Index).ConexionDB

        '        'Se obtiene los comprobantes para firmarlos de cada emisor 
        '        dt = ServicioDAO.GetByIDServicio(eServicio.CrearXMLFirmar)

        '        'Se crea la firma para cada comprobante
        '        For Each dr As DataRow In dt.Rows
        '            Try
        '                IDServicioComprobante = dr("IDServicioComprobante")
        '                IDComprobante = dr("IDComprobante")
        '                TipoComprobante = dr("TipoComprobante")

        '                'Se crea XML y firma compobantes  01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
        '                Select Case TipoComprobante
        '                    Case "01"
        '                        FacturaDAO.CreateXML(IDComprobante)
        '                        FacturaDAO.SignatureXML2(IDComprobante)
        '                        FacturaDAO.ZipXML(IDComprobante)
        '                    Case "03"
        '                        BoletaDAO.CreateXML(IDComprobante)
        '                        BoletaDAO.SignatureXML(IDComprobante)
        '                        BoletaDAO.ZipXML(IDComprobante)
        '                    Case "07"
        '                        NotaCreditoDAO.CreateXML(IDComprobante)
        '                        NotaCreditoDAO.SignatureXML(IDComprobante)
        '                        NotaCreditoDAO.ZipXML(IDComprobante)
        '                    Case "08"
        '                        NotaDebitoDAO.CreateXML(IDComprobante)
        '                        NotaDebitoDAO.SignatureXML(IDComprobante)
        '                        NotaDebitoDAO.ZipXML(IDComprobante)
        '                End Select

        '                'Se elimina el registro
        '                ServicioDAO.Delete(IDServicioComprobante)
        '            Catch ex As Exception
        '                ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.CrearXMLFirmar, "ServicioDAO.GeneraFirmaElectronica :" & ex.Message)
        '            End Try
        '        Next
        '    Next
        'Catch ex As Exception
        '    Tools.SaveLog("DigitalPro Service FirmaXML", ex.Message, EventLogEntryType.Error)
        'End Try
    End Sub

    Private Sub btnServicioEnvioWeb_Click(sender As Object, e As EventArgs) Handles btnServicioEnvioWeb.Click
        ExecuteService()
    End Sub
    Public Sub ExecuteService()
        Try
            'Se detiene el servicio para ejecutar la tarea
            'Tiempo.Stop()

            'Se carga los emisores
            EmisorDAO.GetByConfigXML()

            'Se envia los comprobantes a Sunat
            EnviarComprobantesWeb()
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", "Envio Web: " & ex.Message, EventLogEntryType.Error)
        Finally
            'Tiempo.Start()
        End Try
    End Sub
    Public Sub EnviarComprobantesWeb()
        Dim ComprobanteWebDAO As New ComprobanteWebDAO
        Dim ServicioDAO As New ServicioDAO
        Dim ComprobanteBE As New Object
        Dim dt As New DataTable


        Try
            'Se procesa cada emisor de la lista
            For Index = 0 To EmisorDAO.EmisorConfigXML.Count - 1

                'Se establece la cadena de conexion por cada emisor del Config.XML
                ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML(Index).ConexionDB

                'Se carga el emisor
                EmisorDAO.GetByID(1)

                'Se obtiene los comprobantes para enviarlos a la Web Azure
                dt = ServicioDAO.GetByIDServicio(eServicio.EnviarWeb)

                If dt.Rows.Count = 0 Then
                    Continue For
                End If

                'Se envia cada comprobante a la Web
                For Each dr As DataRow In dt.Rows
                    Try
                        'Se guarda datos del comprobante en la web, tambien los archivos PDF y XML
                        If Me.SaveWebComprobante(EmisorDAO, ComprobanteWebDAO, ComprobanteBE, dr) Then
                            'Se elimina la tarea del envio web
                            ServicioDAO.Delete(dr("IDServicioComprobante"))
                        End If

                    Catch ex As Exception
                        ServicioDAO.Save(dr("TipoComprobante"), dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.EnviarWeb, "ServicioDAO.EnviarWeb :" & ex.Message)
                    End Try
                Next
            Next
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio Web: " & ex.Message, EventLogEntryType.Error)
        Finally
            'Tiempo.Start()
        End Try

    End Sub
    Public Function SaveWebComprobante(ByRef EmisorDAO As EmisorDAO, ByRef ComprobanteWebDAO As ComprobanteWebDAO, ByRef ComprobanteBE As Object, dr As DataRow) As Boolean
        Dim FacturaDAO As New FacturaDAO
        Dim BoletaDAO As New BoletaVentaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO

        Dim TipoComprobante As String = String.Empty
        Dim IDComprobante As Int32
        Dim IDServicioComprobante As Int32
        Dim Result As Boolean = False

        IDServicioComprobante = dr("IDServicioComprobante")
        TipoComprobante = dr("TipoComprobante")
        IDComprobante = dr("IDComprobante")

        'Se crea guarda el comprobante en la web, segun el tipo de comprobante
        Select Case TipoComprobante
            Case "01"
                If FacturaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                    If FacturaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                        FacturaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                        Result = True
                    End If
                End If
            Case "03"
                If BoletaDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                    If BoletaDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                        BoletaDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                        Result = True
                    End If
                End If
            Case "07"
                If NotaCreditoDAO.SaveCloud(EmisorDAO.EmisorBE, IDComprobante) Then
                    If NotaCreditoDAO.SaveCloudArchivos(EmisorDAO.EmisorBE, IDComprobante) Then
                        NotaCreditoDAO.SaveEstadoWeb(IDComprobante, eEstadoWeb.Publicado, DateTime.Now)
                        Result = True
                    End If
                End If
        End Select
        Return Result
    End Function
    'Public Sub SaveWebComprobantePdfXML(EmisorDAO As EmisorDAO, ComprobanteBE As Object)
    '    Dim storageAccount As CloudStorageAccount
    '    storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings("StorageConnectionString"))

    '    'Se recupera el blob
    '    Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

    '    'Se recupera el contenedor previamente creado en Azure. El nombre es minusculas por estandar
    '    'Se establece el nombre del contenedor Codigo Local
    '    Dim Contenedor As CloudBlobContainer = blobClient.GetContainerReference(EmisorDAO.EmisorBE.CodigoLocal)

    '    'Se crea el contenedor si no existe
    '    Contenedor.CreateIfNotExists()

    '    'Se crea dos bloques para los archivos XML y PDF
    '    Dim Bloque1 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobanteXML))
    '    Dim Bloque2 As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(ComprobanteBE.RutaComprobantePDF))

    '    'Se publica bloque 1
    '    Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobanteXML, FileMode.Open)
    '        Bloque1.UploadFromStream(fs)
    '    End Using

    '    'Se publica bloque 1
    '    Using fs As FileStream = New FileStream(ComprobanteBE.RutaComprobantePDF, FileMode.Open)
    '        Bloque2.UploadFromStream(fs)
    '    End Using
    'End Sub

End Class