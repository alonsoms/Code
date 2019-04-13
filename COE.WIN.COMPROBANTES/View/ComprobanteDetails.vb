Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.IO

Public Class ComprobanteDetails
    Public Property ComprobanteBE As Object

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa los controles
        Me.Text = "COMPROBANTES ELECTRONICOS V " & Application.ProductVersion
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Maximized

        'Se inicializa los controles
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        If ComprobanteBE Is Nothing Then
            Exit Sub
        End If

    End Sub
    Private Sub ComprobanteDetails_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Se carga el comprobante XML y Respuesta SUNAT XML
        If ComprobanteBE.rutacomprobantexml.ToString.Length > 0 Then
            '   wbComprobanteXML.DocumentText = ComprobanteDAO.GetComprobanteXML(ComprobanteBE.RutaComprobanteXML)
        End If

        If ComprobanteBE.rutarespuestasunatxml.ToString.Length > 0 Then
            ' wbRespuestaXML.DocumentText = ComprobanteDAO.GetComprobanteXML(ComprobanteBE.RutaRespuestaSunatXML)
        End If

        txtObservaciones.Text = ComprobanteBE.CodigoRespuesta & " " & ComprobanteBE.Observacion
    End Sub
    Private Sub ComprobanteDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

    End Sub
    Private Sub btnCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick

        Me.Close()
    End Sub

    Private Sub btnDescargarArchivos_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnDescargarArchivos.ItemClick
        Dim RutaComprobante As String

        With FolderBrowserDialog1
            .Description = "Selecione la carpeta donde se guardará los documentos en formato PDF y XML"
            .RootFolder = Environment.SpecialFolder.DesktopDirectory
            .ShowNewFolderButton = False
        End With

        Try
            'Se obtiene la carpeta donde se guardara los documentos PDF y XML
            If FolderBrowserDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

                RutaComprobante = ComprobanteBE.RutaComprobanteXML

                'SE copia el archivo XML
                File.Copy(RutaComprobante, FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante), True)

                'SE copia el archivo PDF
                Dim RutaPDF As String = Path.ChangeExtension(RutaComprobante, "pdf")
                File.Copy(RutaPDF, FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileName(RutaComprobante).Replace("XML", "PDF"), True)

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class