#Region "Imports"
Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.IO
Imports System.Text
#End Region

Public Class NotaCreditoDetails
    Dim NotaCreditoBE As New NotaCreditoBE

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y los controles
        Me.Text = SistemaDAO.NombreAplicacion
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
        Me.txtIDNotaCredito.ReadOnly = True
        Me.txtNumeroFactura.ReadOnly = True
        Me.txtNumeroDocumento.ReadOnly = True
        Me.txtComprobanteAfectado.ReadOnly = True
        Me.txtAdquiriente.ReadOnly = True
        Me.txtFirma.ReadOnly = True
        Me.txtObservacion.ReadOnly = True
        Me.txtRutaComprobanteXML.ReadOnly = True
        Me.txtRutaConstanciaRecepcionXML.ReadOnly = True

        'Se inicializa los controles
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se carga la entidad
        LoadEntidad()

    End Sub
    Private Sub NotaCreditoDetails_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnComprobanteXML.ItemClick, btnComprobantePDF.ItemClick, btnConstanciaRecepcionXML.ItemClick

        Try
            Select Case e.Item.Caption
                Case "Descargar Comprobante XML" : DescargarComprobanteXML()
                Case "Descargar Comprobante PDF" : DescargarComprobantePDF()
                Case "Descargar Constancia Recepción XML" : DescargarConstanciaRecepcionXML()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub NotaCreditoDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If

    End Sub

    Public Sub LoadEntidad()

        Try
            'Se obtiene la entidad
            NotaCreditoBE = NotaCreditoDAO.GetByID(NotaCreditoDAO.IDNotaCredito)

            'Se muestra datos del comprobante
            Me.Text = String.Format("ID.NOTA.CREDITO: {0} - NUM.COMPROBANTE: {1} ", NotaCreditoBE.idnotacredito, NotaCreditoBE.t08_numcorrelativo)

            'Se carga la entidad
            With NotaCreditoBE
                txtIDNotaCredito.Text = .idnotacredito
                txtNumeroFactura.Text = .t08_numcorrelativo
                txtNumeroDocumento.Text = .t09_tipdocadquiriente_c06
                txtComprobanteAfectado.Text = .t07_sernumdocafectado
                txtAdquiriente.Text = .t10_nomadquiriente
                txtFirma.Text = .ValorFirma
                txtObservacion.Text = .Observacion
                txtRutaComprobanteXML.Text = .RutaComprobanteXML
                txtRutaConstanciaRecepcionXML.Text = .RutaRespuestaSunatXML
            End With

            'Se carga el comprobante XML
            If File.Exists(NotaCreditoBE.RutaComprobanteXML) Then
                wbComprobanteXML.DocumentText = My.Computer.FileSystem.ReadAllText(NotaCreditoBE.RutaComprobanteXML)
            Else
                wbComprobanteXML.DocumentText = "Archivo no existe"
            End If

            'Se carga la respuesta sunat xml
            If File.Exists(NotaCreditoBE.RutaRespuestaSunatXML) Then
                wbConstanciaRecepcionXML.DocumentText = My.Computer.FileSystem.ReadAllText(NotaCreditoBE.RutaRespuestaSunatXML)
            Else
                wbConstanciaRecepcionXML.DocumentText = "Archivo no existe"
            End If

            'Se visualiza el registro XML, directamente de las tablas
            wbRegistroXML.DocumentText = NotaCreditoDAO.GetByIDXML(NotaCreditoBE.idnotacredito)

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

                RutaComprobante = NotaCreditoBE.RutaComprobanteXML

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
    Public Sub DescargarComprobantePDF()
        Dim RutaComprobante As String

        With FolderBrowserDialog1
            .Description = "Selecione la carpeta donde se guardará los archivo en formato PDF"
            .RootFolder = Environment.SpecialFolder.DesktopDirectory
            .ShowNewFolderButton = False
        End With

        Try
            'Se obtiene la carpeta donde se guardara los documentos PDF y XML
            If FolderBrowserDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

                RutaComprobante = Path.ChangeExtension(NotaCreditoBE.RutaComprobanteXML, ".pdf")

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

                RutaComprobante = NotaCreditoBE.RutaRespuestaSunatXML

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