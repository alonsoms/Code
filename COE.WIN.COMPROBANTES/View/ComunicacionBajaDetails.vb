#Region "Imports"
Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.IO
Imports System.Text
#End Region

Public Class ComunicacionBajaDetails
    Dim ComunicacionBajaBE As New ComunicacionBajaBE

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y los controles
        Me.Text = "COMUNICACION DE BAJA - " & SistemaDAO.NombreAplicacion
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
        Me.txtIDComunicacion.ReadOnly = True
        Me.txtNumeroComunicacion.ReadOnly = True
        Me.txtFirma.ReadOnly = True
        Me.txtObservacion.ReadOnly = True
        Me.txtRutaComprobanteXML.ReadOnly = True
        Me.txtRutaConstanciaRecepcionXML.ReadOnly = True
        Me.txtFechaComunicacion.ReadOnly = True
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
            ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(ComunicacionBajaDAO.IDComunicacion)

            'Se muestra datos del comprobante
            Me.Text = String.Format("ID.COMUNICACION: {0} - NUM.COMUNICACION: {1} ", ComunicacionBajaBE.idcomunicacion, ComunicacionBajaBE.t09_numcorrelativo)

            'Se carga la entidad
            With ComunicacionBajaBE
                txtIDComunicacion.Text = .idcomunicacion
                txtNumeroComunicacion.Text = .t09_numcorrelativo
                txtFirma.Text = .ValorFirma
                txtObservacion.Text = .Observacion
                txtRutaComprobanteXML.Text = .RutaComprobanteXML
                txtRutaConstanciaRecepcionXML.Text = .RutaRespuestaSunatXML
                txtFechaComunicacion.Text = .t10_feccomunicacion
                txtFechaEmision.Text = .t03_fecemisiondoc
            End With

            'Se carga el comprobante XML
            If File.Exists(ComunicacionBajaBE.RutaComprobanteXML) Then
                wbComprobanteXML.DocumentText = My.Computer.FileSystem.ReadAllText(ComunicacionBajaBE.RutaComprobanteXML)
            Else
                wbComprobanteXML.DocumentText = "Archivo no existe"
            End If

            'Se carga la respuesta sunat xml
            If File.Exists(ComunicacionBajaBE.RutaRespuestaSunatXML) Then
                wbConstanciaRecepcionXML.DocumentText = My.Computer.FileSystem.ReadAllText(ComunicacionBajaBE.RutaRespuestaSunatXML)
            Else
                wbConstanciaRecepcionXML.DocumentText = "Archivo no existe"
            End If

            'Se visualiza el registro XML, directamente de las tablas
            wbRegistroXML.DocumentText = ComunicacionBajaDAO.GetByIDXML(ComunicacionBajaBE.idcomunicacion)

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

                RutaComprobante = ComunicacionBajaBE.RutaComprobanteXML

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

                RutaComprobante = ComunicacionBajaBE.RutaRespuestaSunatXML

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