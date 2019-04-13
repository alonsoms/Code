Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports COE.FRAMEWORK

Public Class EmisorDAO
    Public Property EmisorBE As New EmisorBE
    Public Property EmisorConfigXML As List(Of EmisorBE.ConfiguracionBE)

    Public Function Save(ByVal BE As EmisorBE) As Int32
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Int32 = 0

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            If BE.IDEmisor = 0 Then
                .CommandText = "coe_emisor_ins"
                .Parameters.Add("@IDEmisor", SqlDbType.Int).Direction = ParameterDirection.Output
            Else
                .CommandText = "coe_emisor_upd"
                .Parameters.Add("@IDEmisor", SqlDbType.Int).Value = BE.IDEmisor
            End If
            With .Parameters
                .Add("@NumeroRUC", SqlDbType.VarChar, 15).Value = BE.NumeroRUC
                .Add("@RazonSocial", SqlDbType.VarChar, 250).Value = BE.RazonSocial
                .Add("@NombreComercial", SqlDbType.VarChar, 250).Value = BE.NombreComercial
                .Add("@CodigoUbigeo", SqlDbType.VarChar, 10).Value = BE.CodigoUbigeo
                .Add("@NombreDepartamento", SqlDbType.VarChar, 100).Value = BE.NombreDepartamento
                .Add("@NombreProvincia", SqlDbType.VarChar, 100).Value = BE.NombreProvincia
                .Add("@NombreDistrito", SqlDbType.VarChar, 100).Value = BE.NombreDistrito
                .Add("@NombreDireccion", SqlDbType.VarChar, 250).Value = BE.NombreDireccion
                .Add("@NombreUrbanizacion", SqlDbType.VarChar, 100).Value = BE.NombreUrbanizacion
                .Add("@RutaCarpetaArchivosXML", SqlDbType.VarChar, 500).Value = BE.RutaCarpetaArchivosXML
                .Add("@RutaCarpetaArchivosPDF", SqlDbType.VarChar, 500).Value = BE.RutaCarpetaArchivosPDF
                .Add("@RutaCarpetaArchivosCertificados", SqlDbType.VarChar, 500).Value = BE.RutaCarpetaArchivosCertificados
                .Add("@ClaveCertificado", SqlDbType.VarChar, 50).Value = BE.ClaveCertificado
                .Add("@Resolucion", SqlDbType.VarChar, 150).Value = BE.Resolucion
                .Add("@ServidorHost", SqlDbType.VarChar, 150).Value = BE.ServidorHost
                .Add("@ServidorPuerto", SqlDbType.VarChar, 10).Value = BE.ServidorPuerto
                .Add("@CorreoEnvio", SqlDbType.VarChar, 150).Value = BE.CorreoEnvio
                .Add("@CorreoContrasena", SqlDbType.VarChar, 20).Value = BE.CorreoContrasena
                .Add("@CorreoAsunto", SqlDbType.VarChar, 150).Value = BE.CorreoAsunto
                .Add("@CorreoMensaje", SqlDbType.VarChar, 4000).Value = BE.CorreoMensaje
                .Add("@CorreoAlertas", SqlDbType.VarChar, 150).Value = BE.CorreoAlertas
                .Add("@CodLocal", SqlDbType.VarChar, 10).Value = BE.CodigoLocal
                .Add("@FechaEnvioResumenComunicacion", SqlDbType.DateTime).Value = BE.FechaEnvioResumenComunicacion
                .Add("@FechaRegistro", SqlDbType.DateTime).Value = BE.FechaRegistro
                .Add("@SunatUser", SqlDbType.VarChar, 20).Value = BE.SunatUser
                .Add("@SunatPass", SqlDbType.VarChar, 20).Value = BE.SunatPass
            End With
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                Me.EmisorBE.IDEmisor = cmd.Parameters("@IDEmisor").Value
            End If
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function
    Public Function SaveImpresora(ByVal BE As EmisorBE.ImpresoraBE) As Int32
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Int32 = 0

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_emisor_impresora_ins"
            .Parameters.Add("@IDImpresora", SqlDbType.Int).Direction = ParameterDirection.Output

            With .Parameters
                .Add("@IDTipoComprobante", SqlDbType.Int).Value = BE.IDTipoComprobante
                .Add("@SerieComprobante", SqlDbType.VarChar, 10).Value = BE.SerieComprobante
                .Add("@NombreImpresora", SqlDbType.VarChar, 250).Value = BE.NombreImpresora
            End With
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                Result = cmd.Parameters("@IDImpresora").Value
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
    Public Function DeleteImpresora(ByVal IDImpresora As Int32) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_emisor_impresora_del"
            .Parameters.Add("@IDImpresora", SqlDbType.Int).Value = IDImpresora
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function
    Public Function GetByID(Optional IDEmisor As Int32 = 1) As EmisorBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_emisor_get_id"
            .Parameters.Add("@IDEmisor", SqlDbType.Int).Value = IDEmisor
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read()
                    With Me.EmisorBE
                        .IDEmisor = dr.ReadNullAsNumeric("IDEmisor")
                        .NumeroRUC = dr.ReadNullAsEmptyString("NumeroRUC")
                        .RazonSocial = dr.ReadNullAsEmptyString("RazonSocial")
                        .NombreComercial = dr.ReadNullAsEmptyString("NombreComercial")
                        .CodigoUbigeo = dr.ReadNullAsEmptyString("CodigoUbigeo")
                        .NombreDepartamento = dr.ReadNullAsEmptyString("NombreDepartamento")
                        .NombreProvincia = dr.ReadNullAsEmptyString("NombreProvincia")
                        .NombreDistrito = dr.ReadNullAsEmptyString("NombreDistrito")
                        .NombreDireccion = dr.ReadNullAsEmptyString("NombreDireccion")
                        .NombreUrbanizacion = dr.ReadNullAsEmptyString("NombreUrbanizacion")
                        .RutaCarpetaArchivosXML = dr.ReadNullAsEmptyString("RutaCarpetaArchivosXML")
                        .RutaCarpetaArchivosPDF = dr.ReadNullAsEmptyString("RutaCarpetaArchivosPDF")
                        .RutaCarpetaArchivosCertificados = dr.ReadNullAsEmptyString("RutaCarpetaArchivosCertificados")
                        .ClaveCertificado = dr.ReadNullAsEmptyString("ClaveCertificado")
                        .Resolucion = dr.ReadNullAsEmptyString("Resolucion")
                        .ServidorHost = dr.ReadNullAsEmptyString("ServidorHost")
                        .ServidorPuerto = dr.ReadNullAsEmptyString("ServidorPuerto")
                        .CorreoEnvio = dr.ReadNullAsEmptyString("CorreoEnvio")
                        .CorreoContrasena = dr.ReadNullAsEmptyString("CorreoContrasena")
                        .CorreoAsunto = dr.ReadNullAsEmptyString("CorreoAsunto")
                        .CorreoMensaje = dr.ReadNullAsEmptyString("CorreoMensaje")
                        .CorreoAlertas = dr.ReadNullAsEmptyString("CorreoAlertas")
                        .CodigoLocal = dr.ReadNullAsEmptyString("CodLocal")
                        .FechaEnvioResumenComunicacion = dr.ReadNullAsEmptyDate("FechaEnvioResumenComunicacion")
                        .FechaRegistro = dr.ReadNullAsEmptyDate("FechaRegistro")

                        .NombreStorageCloud = dr.ReadNullAsEmptyString("NombreStorageCloud")
                        .SunatUser = dr.ReadNullAsEmptyString("SunatUser")
                        .SunatPass = dr.ReadNullAsEmptyString("SunatPass")
                    End With

                End While

                dr.NextResult()

                If dr.HasRows Then
                    While dr.Read()
                        Dim ImpresoraBE As New EmisorBE.ImpresoraBE
                        With ImpresoraBE
                            .IDImpresora = dr.ReadNullAsNumeric("IDImpresora")
                            .IDTipoComprobante = dr.ReadNullAsNumeric("IDTipoComprobante")
                            .NombreImpresora = dr.ReadNullAsEmptyString("NombreImpresora")
                            .NombreTipoComprobante = dr.ReadNullAsEmptyString("NombreTipoComprobante")
                            .SerieComprobante = dr.ReadNullAsEmptyString("SerieComprobante")
                        End With
                        Me.EmisorBE.Impresoras.Add(ImpresoraBE)
                    End While

                End If
                dr.Close()
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Me.EmisorBE
    End Function
    Public Function GetBySerie(ByVal IDTipoComprobante As Int32, SerieComprobante As String) As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_emisor_get_serie"
            .Parameters.Add("@IDTipoComprobante", SqlDbType.Int).Value = IDTipoComprobante
            .Parameters.Add("@SerieComprobante", SqlDbType.VarChar, 10).Value = SerieComprobante
        End With

        Try
            cnx.Open()
            Result = cmd.ExecuteScalar
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function
    Public Sub GetByConfigXML()

        Try
            'Se establece la ruta del archivo Config.XML. El archivo debe estar en la misma ruta donde se ejecuta la aplicacion
            Dim Ruta As String = Path.Combine(My.Application.Info.DirectoryPath, "Config.xml")

            'Se valida que exista el archivo
            If Not File.Exists(Ruta) Then
                Throw New Exception("El archivo Config.XML no se encontro.")
            End If

            'Se carga el archivo Config.XML
            Dim Archivo As String = My.Computer.FileSystem.ReadAllText(Ruta)

            'Se transforma el archivo a formato XML
            Dim XMLDoc As XElement = XElement.Parse(Archivo)

            'Se obtiene los datos del archivo de configuracion Config.XML
            Dim ListaRuc As IEnumerable(Of String) = From Item In XMLDoc...<RUC> Select Item.Value
            Dim ListaRazonSocial As IEnumerable(Of String) = From Item In XMLDoc...<RAZONSOCIAL> Select Item.Value
            Dim ListaConexionDB As IEnumerable(Of String) = From Item In XMLDoc...<CONEXIONDB> Select Item.Value

            'Dim ListaCodLocal As IEnumerable(Of String) = From Item In XMLDoc...<CODLOCAL> Select Item.Value
            'Dim ListaUser As IEnumerable(Of String) = From Item In XMLDoc...<USER> Select Item.Value
            'Dim ListaPass As IEnumerable(Of String) = From Item In XMLDoc...<PASS> Select Item.Value

            Dim Lista As New List(Of EmisorBE.ConfiguracionBE)

            For index = 0 To ListaRuc.Count - 1
                Dim EmpresaBE As New EmisorBE.ConfiguracionBE

                With EmpresaBE
                    .RUC = ListaRuc(index)
                    .RazonSocial = ListaRazonSocial(index)
                    .ConexionDB = ListaConexionDB(index)
                End With

                Lista.Add(EmpresaBE)
            Next
            'Se establece la lista de conexiones de los emisores
            Me.EmisorConfigXML = Lista
        Catch ex As Exception
            Throw ex
        End Try


    End Sub

    Public Sub EnviarComprobantesAzure()
        Dim ComprobanteWebDAO As New ComprobanteWebDAO
        Dim ServicioDAO As New ServicioDAO
        Dim ComprobanteBE As New Object
        Dim dt As New DataTable


        Try
            'Se procesa cada emisor de la lista
            For Index = 0 To Me.EmisorConfigXML.Count - 1

                'Se establece la cadena de conexion por cada emisor del Config.XML
                ConexionDAO.ConexionDBNet = Me.EmisorConfigXML(Index).ConexionDB

                'Se carga el emisor
                Me.GetByID(1)

                'Se obtiene los comprobantes para enviarlos a la Web Azure
                dt = ServicioDAO.GetByIDServicio(eServicio.EnviarWeb)

                If dt.Rows.Count = 0 Then
                    Continue For
                End If

                'Se envia cada comprobante a la Web
                For Each dr As DataRow In dt.Rows
                    Try
                        'Se guarda datos del comprobante en la web, tambien los archivos PDF y XML
                        Me.SaveWebComprobante(Me, ComprobanteWebDAO, ComprobanteBE, dr)

                        'Se elimina la tarea del envio web
                        ServicioDAO.Delete(dr("IDServicioComprobante"))
                    Catch ex As Exception
                        ServicioDAO.Save(dr("TipoComprobante"), dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.EnviarWeb, "ServicioDAO.EnviarWeb :" & ex.Message)
                    End Try
                Next
            Next
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio Comprobantes Azure: " & ex.Message, EventLogEntryType.Error)
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

    Public Sub EnviarEmailAlertas()
        Dim AlertasDAO As New AlertasDAO
        Dim SistemaDAO As New SistemaDAO
        Dim Mensaje As String = String.Empty

        Try

            'Se obtiene las alertas a enviar
            Mensaje = AlertasDAO.GetByAll

            'Se valida si hay alertas que enviar
            If Mensaje.ToString.Trim <> "" Then

                'Se obtiene los datos del Emisor
                SistemaDAO.EmisorBE = Me.GetByID(1)

                Dim Mail As New MailMessage()
                Dim SmtpServer As New SmtpClient()
                Dim EmailFuente As String = SistemaDAO.EmisorBE.CorreoEnvio
                Dim EmailFuenteContrasena As String = SistemaDAO.EmisorBE.CorreoContrasena
                Dim EmailBody As String = Mensaje
                Dim EmailAsunto As String = "ALERTA DE COMPROBANTES ELECTRONICOS"
                Dim ServidorHostURL As String = SistemaDAO.EmisorBE.ServidorHost
                Dim ServidorHostPuerto As String = SistemaDAO.EmisorBE.ServidorPuerto
                Dim EmailAlerta As String = SistemaDAO.EmisorBE.CorreoAlertas

                'Se configura para servidor de correos 
                SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
                SmtpServer.Port = ServidorHostPuerto
                SmtpServer.Host = ServidorHostURL
                SmtpServer.EnableSsl = True
                SmtpServer.ServicePoint.ConnectionLeaseTimeout = 1
                SmtpServer.ServicePoint.MaxIdleTime = 1
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network
                ServicePointManager.ServerCertificateValidationCallback = Function(s As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True

                'Se configura el Correo
                Mail = New MailMessage()
                Mail.From = New MailAddress(EmailFuente, SistemaDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
                Mail.To.Add(EmailAlerta)
                Mail.Subject = EmailAsunto
                Mail.Body = EmailBody
                Mail.IsBodyHtml = True
                Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

                'Se envia el correo
                SmtpServer.Send(Mail)
            End If
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio Alertas: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class
