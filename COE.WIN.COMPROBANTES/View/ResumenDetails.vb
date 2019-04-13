#Region "Imports"
Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.IO
Imports System.Text
#End Region

Public Class ResumenDetails
    Dim ResumenBE As New ResumenBE2018

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y los controles
        Me.Text = "RESUMEN DIARIO - " & SistemaDAO.NombreAplicacion
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Normal
        Me.wbRegistroXML.IsWebBrowserContextMenuEnabled = False
        Me.wbRegistroXML.AllowWebBrowserDrop = False
        Me.wbRegistroXML.AllowNavigation = False
        Me.wbRegistroXML.ScriptErrorsSuppressed = False
        Me.wbComprobanteXML.IsWebBrowserContextMenuEnabled = False
        Me.wbComprobanteXML.AllowWebBrowserDrop = False
        Me.wbComprobanteXML.AllowNavigation = False
        Me.wbComprobanteXML.ScriptErrorsSuppressed = False
        Me.wbConstanciaRecepcionXML.IsWebBrowserContextMenuEnabled = False
        Me.wbConstanciaRecepcionXML.AllowWebBrowserDrop = False
        Me.wbConstanciaRecepcionXML.AllowNavigation = False
        Me.wbConstanciaRecepcionXML.ScriptErrorsSuppressed = False
        Me.txtIDResumne.ReadOnly = True
        Me.txtNumeroResumen.ReadOnly = True
        Me.txtFirma.ReadOnly = True
        Me.txtObservacion.ReadOnly = True
        Me.txtRutaComprobanteXML.ReadOnly = True
        Me.txtRutaConstanciaRecepcionXML.ReadOnly = True
        Me.txtFechaResumen.ReadOnly = True
        Me.txtFechaEmision.ReadOnly = True

        'Se inicializa los controles
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se carga la entidad
        LoadEntidad()

    End Sub
    Private Sub FacturaDetails_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnComprobanteXML.ItemClick, btnConstanciaRecepcionXML.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Descargar Comprobante XML" : DescargarComprobanteXML()
                Case "Descargar Constancia Recepción XML" : DescargarConstanciaRecepcionXML()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub FacturaDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Public Sub LoadEntidad()
        Try
            'Se obtiene la entidad
            ResumenBE = ResumenDAO.GetByID(ResumenDAO.IDResumen)

            'Se muestra datos del comprobante
            Me.Text = String.Format("ID.RESUMEN: {0} - NUM.RESUMEN: {1} ", ResumenBE.idresumen, ResumenBE.t17_numcorrelativo)

            'Se carga la entidad
            With ResumenBE
                txtIDResumne.Text = .idresumen
                txtNumeroResumen.Text = .t17_numcorrelativo
                '   txtFirma.Text = .ValorFirma
                txtObservacion.Text = .Observacion
                txtRutaComprobanteXML.Text = .RutaComprobanteXML
                txtRutaConstanciaRecepcionXML.Text = .RutaRespuestaSunatXML
                txtFechaResumen.Text = .t18_fecresumen
                txtFechaEmision.Text = .t03_fecemision
            End With

            'Se carga el comprobante XML
            If File.Exists(ResumenBE.RutaComprobanteXML) Then
                wbComprobanteXML.DocumentText = My.Computer.FileSystem.ReadAllText(ResumenBE.RutaComprobanteXML)
            Else
                wbComprobanteXML.DocumentText = "Archivo no existe"
            End If

            'Se carga la respuesta sunat xml
            If File.Exists(ResumenBE.RutaRespuestaSunatXML) Then
                wbConstanciaRecepcionXML.DocumentText = My.Computer.FileSystem.ReadAllText(ResumenBE.RutaRespuestaSunatXML)
            Else
                wbConstanciaRecepcionXML.DocumentText = "Archivo no existe"
            End If

            'Se visualiza el registro XML, directamente de las tablas
            wbRegistroXML.DocumentText = ResumenDAO.GetByIDXML(ResumenBE.idresumen)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Public Sub DescargarComprobanteXML()
        Dim RutaComprobante As String

        With FolderBrowserDialog1
            .Description = "Selecione la carpeta donde se guardará los archivo en formato XML"
            .RootFolder = Environment.SpecialFolder.DesktopDirectory
            .ShowNewFolderButton = False
        End With

        Try
            'Se obtiene la carpeta donde se guardara el comproante XML
            If FolderBrowserDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

                RutaComprobante = ResumenBE.RutaComprobanteXML

                'SE copia el archivo XML
                File.Copy(RutaComprobante, FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante), True)

                If MessageBox.Show("¿Desea abrir el comprobante XML?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then
                    Process.Start(FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante))
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub DescargarConstanciaRecepcionXML()
        Dim RutaComprobante As String

        With FolderBrowserDialog1
            .Description = "Selecione la carpeta donde se guardará los archivo en formato XML"
            .RootFolder = Environment.SpecialFolder.DesktopDirectory
            .ShowNewFolderButton = False
        End With

        Try
            'Se obtiene la carpeta donde se guardara los documentos PDF y XML
            If FolderBrowserDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

                RutaComprobante = ResumenBE.RutaRespuestaSunatXML

                'SE copia el archivo XML
                File.Copy(RutaComprobante, FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante), True)

                If MessageBox.Show("¿Desea abrir la  constancia de recepción XML?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then
                    Process.Start(FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante))
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub

End Class