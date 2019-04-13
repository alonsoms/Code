Imports System.Configuration
Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization
Imports DevExpress.Utils
Imports DevExpress.XtraBars
Imports DevExpress.XtraEditors
Imports DevExpress.XtraLayout
Imports DevExpress.XtraSplashScreen

Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail



Public Enum eTipoControl
    TextEdit = 1
    SpinEdit = 2
    GridLoopUpEdit = 3
    SearchLookUpEdit = 4
    DateEdit = 5
End Enum
Public Enum eTipoValidacion
    ValorObligatorio = 1
    ExisteCarpeta = 2
    ExisteArchivo = 3
End Enum

Public Enum eIcon
    FolderCerrado = 1
    FolderAbierto = 2
    Configuracion = 3
    Comprobantes = 4
End Enum
Public Class Tools

    Shared Sub WinProcess(ByRef objForm As Form, Enabled As Boolean)

        'Se muestra mensaje en la barra de titulo
        If Enabled Then
            'System.Windows.Forms.Form.ActiveForm.Text = Application.ProductName & " " & Application.ProductVersion.ToString
            'objForm.Text = Application.ProductName & " " & Application.ProductVersion.ToString
            'objForm.Enabled = True
        Else
            'System.Windows.Forms.Form.ActiveForm.Text = "Espere un momento, el proceso puede tardar algunos segundos..."
            'objForm.Text = "Espere un momento, el proceso puede tardar algunos segundos..."
            'objForm.Enabled = False
            'objForm.ResumeLayout()
        End If
        '      System.Windows.Forms.Form.ActiveForm.Refresh()

        'Se activa/inactiva depende del valor del parametro
        'System.Windows.Forms.Form.ActiveForm.Enabled = Enabled

    End Sub

    Shared Function GetIcono(ID As eIcon) As System.Drawing.Bitmap
        Dim Icono As System.Drawing.Bitmap = Nothing

        Select Case ID
            Case eIcon.FolderAbierto : Icono = My.Resources.FolderOpen24
            Case eIcon.FolderCerrado : Icono = My.Resources.FolderClosed24
            Case eIcon.Configuracion : Icono = My.Resources.Configuracion24
            Case eIcon.Comprobantes : Icono = My.Resources.Comprobantes24
        End Select

        Return Icono
    End Function

    Shared Function Teclado(Optional sender As Object = Nothing, Optional e As KeyEventArgs = Nothing) As KeyEventArgs

        Select Case e.KeyCode
            Case Keys.Escape
                sender.Close()
            Case Keys.F1
                MessageBox.Show("Ayuda no esta disponible", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select

        Return e
    End Function
    Shared Function Num(ByVal value As String) As String
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "\d+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        'Return Convert.ToInt32(returnVal)
        Return returnVal
    End Function
    Shared Function GetNumberFromStringUsingSB(ByVal theString As String) As Long
        Dim sb As New System.Text.StringBuilder(theString.Length)
        For Each ch As Char In theString
            If Char.IsDigit(ch) Then sb.Append(ch)
        Next
        Return Long.Parse(sb.ToString)
    End Function

    Shared Function ReadConexionString(Key As String) As String
        Dim Cnx As ConnectionStringSettings
        Dim Value As String = String.Empty

        Try
            'Se lee la cadena de conexion
            Cnx = ConfigurationManager.ConnectionStrings(Key)

            If Not Cnx Is Nothing Then
                Value = Cnx.ConnectionString
            End If
        Catch ex As Exception
            Throw ex
        End Try

        Return Value
    End Function
    Shared Function ReadAppSettings(Key As String) As String
        Dim Value As String = String.Empty
        Try
            Value = ConfigurationManager.AppSettings(Key)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return Value
    End Function
    Shared Function SaveAppSettings(Key As String, Value As String) As Boolean
        Dim Result As Boolean = False

        Try
            Dim configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim settings = configFile.AppSettings.Settings

            If IsNothing(settings(Key)) Then
                settings.Add(Key, Value)
            Else
                settings(Key).Value = Value
            End If
            configFile.Save(ConfigurationSaveMode.Modified)

            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name)
        Catch ex As ConfigurationErrorsException
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return Result
    End Function
    Shared Function CampoObligatorio(objControl As Object, Optional ByVal TipoControl As eTipoControl = eTipoControl.TextEdit, Optional TipoValidacion As eTipoValidacion = eTipoValidacion.ValorObligatorio) As Boolean
        Dim Result As Boolean = True

        If TipoValidacion = eTipoValidacion.ValorObligatorio Then
            'Se valida que la propiedad Text no este vacia
            If TipoControl = eTipoControl.TextEdit Then

                If objControl.Text = "" Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
                If objControl.Text = "0.00" Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
                If objControl.Text = "0" Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
            End If

            If TipoControl = eTipoControl.SpinEdit Then
                Dim Control As New SpinEdit

                Control = CType(objControl, SpinEdit)

                If Control.EditValue = 0 Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
            End If

            If TipoControl = eTipoControl.GridLoopUpEdit Then
                Dim Control As New GridLookUpEdit

                Control = CType(objControl, GridLookUpEdit)

                If Control.EditValue = 0 Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
            End If

            If TipoControl = eTipoControl.SearchLookUpEdit Then
                Dim Control As New SearchLookUpEdit

                Control = CType(objControl, SearchLookUpEdit)

                If Control.EditValue = 0 Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If
            End If
            If TipoControl = eTipoControl.DateEdit Then
                Dim Control As New DateEdit
                Control = CType(objControl, DateEdit)

                If Control.EditValue Is Nothing Then
                    objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                    objControl.ErrorText = "El campo es obligatorio"
                    Result = False
                End If

            End If
        End If

        'Se valida que exista la carpeta
        If TipoValidacion = eTipoValidacion.ExisteCarpeta Then
            If Not Directory.Exists(objControl.Text) Then
                objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                objControl.ErrorText = "La carpeta no existe"
                Result = False
            End If
        End If

        'Se valida que exista el archivo
        If TipoValidacion = eTipoValidacion.ExisteArchivo Then
            If Not File.Exists(objControl.Text) Then
                objControl.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                objControl.ErrorText = "El archivo no existe"
                Result = False
            End If
        End If

        Return Result
    End Function

    Shared Function SendEmail(EmisorDAO As Object, EmailCliente As String, RutaXML As String, RutaPDF As String)
        Dim Mail As New MailMessage()
        Dim SmtpServer As New SmtpClient()
        Dim EmailFuente As String = EmisorDAO.EmisorBE.CorreoEnvio
        Dim EmailFuenteContrasena As String = EmisorDAO.EmisorBE.CorreoContrasena
        Dim EmailBody As String = EmisorDAO.EmisorBE.CorreoMensaje
        Dim EmailAsunto As String = EmisorDAO.EmisorBE.CorreoAsunto
        Dim ServidorHostURL As String = EmisorDAO.EmisorBE.ServidorHost
        Dim ServidorHostPuerto As String = EmisorDAO.EmisorBE.ServidorPuerto
        Dim Result As Boolean = False

        'Se configura para servidor de GMail
        SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
        SmtpServer.Port = ServidorHostPuerto
        SmtpServer.Host = ServidorHostURL
        SmtpServer.EnableSsl = True

        'Se configura el Correo
        Mail = New MailMessage()
        Mail.From = New MailAddress(EmailFuente, EmisorDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
        Mail.To.Add(EmailCliente)
        Mail.Subject = EmailAsunto
        Mail.Body = EmailBody
        Mail.Attachments.Add(New Attachment(RutaXML))
        Mail.Attachments.Add(New Attachment(RutaPDF))
        Mail.IsBodyHtml = True
        Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

        Try
            SmtpServer.Send(Mail)
            Result = True
        Catch ex As Exception
            Throw ex
        End Try

        Return Result
    End Function

    ''' <summary>
    ''' Se genera el TAG Nodo+Atributo+Valor
    ''' </summary>
    ''' <param name="doc">Documento XML</param>
    ''' <param name="E1">Nombre del nodo</param>
    ''' <param name="E2">Nombre del atributo</param>
    ''' <param name="E3">Valor del atributo</param>
    ''' <remarks></remarks>
    Shared Sub TagNodoAtributoValor(ByRef doc As XmlTextWriter, E1 As String, E2 As String, E3 As String)

        doc.WriteStartElement(E1)
        doc.WriteAttributeString(E2, E3)
        doc.WriteEndElement()
    End Sub

    ''' <summary>
    ''' Se genera el TAG Nodo+Atributo+ValorAtributo+ValorNodo
    ''' </summary>
    ''' <param name="doc">Documento XML</param>
    ''' <param name="E1">Nombre del nodo</param>
    ''' <param name="E2">Nombre del Atributo</param>
    ''' <param name="E3">Valor del atributo</param>
    ''' <param name="E4">Valor del nodo</param>
    ''' <remarks></remarks>
    Shared Sub TagNodoAtributoValorValor(ByRef doc As XmlTextWriter, E1 As String, E2 As String, E3 As String, E4 As String)

        doc.WriteStartElement(E1)
        doc.WriteAttributeString(E2, E3)
        doc.WriteString(E4)
        doc.WriteEndElement()
    End Sub

    Shared Sub SaveLog(Fuente As String, ByVal Mensaje As String, Tipo As EventLogEntryType)
        Dim Log As New EventLog("Application", ".", Fuente)

        Try

            If Not EventLog.SourceExists(Fuente, ".") Then
                EventLog.CreateEventSource(Fuente, "Application")
            End If

            Log.WriteEntry(Mensaje, Tipo)
        Catch ex As Exception

            Log.WriteEntry(ex.Message, EventLogEntryType.Error)

        End Try
    End Sub

    Shared Function DAOGetByALL(CadenaConexion As String, NombreProcedimientoAlmacenado As String) As DataTable
        Dim cnx As New SqlConnection(CadenaConexion)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = NombreProcedimientoAlmacenado
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
    Shared Function DAOGetByALL(CadenaConexion As String, NombreProcedimientoAlmacenado As String, Parametros() As String, Valores() As String) As DataTable
        Dim cnx As New SqlConnection(CadenaConexion)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = NombreProcedimientoAlmacenado

            For Index = 0 To Parametros.Length - 1

                If Parametros(Index).Contains("Fec") Then
                    .Parameters.Add(Parametros(Index), SqlDbType.Date).Value = Valores(Index)
                Else
                    .Parameters.AddWithValue(Parametros(Index), Valores(Index))
                End If


            Next
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
    Shared Function DAODelete(CadenaConexion As String, NombreProcedimeintoAlmacenado As String, Parametros() As String, Valores() As String) As Boolean
        Dim cnx As New SqlConnection(CadenaConexion)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = NombreProcedimeintoAlmacenado

            For Index = 0 To Parametros.Length - 1
                .Parameters.AddWithValue(Parametros(Index), Valores(Index))
            Next
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


    Shared Function MinutosToMilisegundos(Minutos As Decimal) As Decimal
        'Se define 1 minuto es 60,000 milisegundos
        Return Minutos * 60000
    End Function

End Class
