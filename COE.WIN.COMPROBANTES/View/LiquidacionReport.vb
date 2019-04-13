Imports COE.DATA
Imports COE.FRAMEWORK
Imports DevExpress.XtraReports.UI

Public Class LiquidacionReport

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se configura el formulario y controles
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Icon = DesktopMain.Icon
        Me.Text = "Reporte de Liquidación de Servicio"
        Me.FormBorderStyle = FormBorderStyle.Fixed3D
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Normal
        Me.dtpFechaInicial.EditValue = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        Me.dtpFechaFinal.EditValue = DateTime.Now.Date

        'Se configura los eventos
        AddHandler Me.KeyDown, AddressOf Tools.Teclado

    End Sub

    Private Sub btnProcesar_Click(sender As Object, e As EventArgs) Handles btnProcesar.Click
        Dim ComprobantesDAO As New ComprobanteDAO

        Try
            'Se crea la instancia del reporte
            Dim MiReporte As New COE.REPORT.Liquidacion

            'Se carga los datos del reporte
            MiReporte.DataSource = ComprobanteDAO.GetRptLiquidacion(dtpFechaInicial.EditValue, dtpFechaFinal.EditValue)
            MiReporte.DataMember = "coe_comprobante_rpt_liquidacion"

            'Se muestra el reporte
            Dim printTool As New ReportPrintTool(MiReporte)
            printTool.ShowRibbonPreviewDialog()

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try
    End Sub

    Private Sub btnCerrar_Click(sender As Object, e As EventArgs) Handles btnCerrar.Click
        Me.Close()
    End Sub

End Class