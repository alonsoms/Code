#Region "Imports"
Imports System.IO
Imports System.Net.Mail
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports COE.DATA
Imports COE.FRAMEWORK
Imports DevExpress.Utils
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraSplashScreen
#End Region

Public Class ServicioList
    Dim bsServicios As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.GridControl1.DataSource = bsServicios
        Me.Text = "VISOR DE TAREAS"

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.SERVICIO", "IDServicioComprobante", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "SERVICIO", "NombreServicio", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ESTADO", "NombreEstado", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TIPO", "TipoComprobante", 70, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.COMPROBANTE", "IDComprobante", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EXCEPCION", "Descripcion", 250, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "FechaRegistro", 110, True, ControlesDevExpress.eGridViewFormato.FechaHora)

        'Se carga los registros
        btnActualizar.PerformClick()

    End Sub

    Private Sub AbrirIcono(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Tareas", eMenuFormulario.Open)
    End Sub
    Private Sub CerrarIcono(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Tareas", eMenuFormulario.Close)
    End Sub


    Private Sub BoletasList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub BoletasList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnEliminar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Eliminar" : Eliminar()
                Case "Actualizar" : Actualizar()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender

        'Se cambia el estilo de fuente
        e.Appearance.Font = New Font(e.Appearance.Font, FontStyle.Regular)

        'Si existe excepcion cambia de color
        If e.Column.FieldName = "Descripcion" Then
            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("Descripcion")).ToString.Length = 0 Then
                e.Appearance.ForeColor = Color.Red
            End If
        End If
    End Sub
    Public Sub Eliminar()
        'Se obtiene el ID
        ServicioDAO.BE.IDServicioComprobante = GridView1.GetFocusedRowCellValue("IDServicioComprobante")

        If MessageBox.Show("¿Esta seguro de eliminar el registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then
            ServicioDAO.Delete(ServicioDAO.BE.IDServicioComprobante)
            Actualizar()
        End If
    End Sub

    Public Sub Actualizar()
        'Se establece la fuente de datos del Binding
        bsServicios.DataSource = ServicioDAO.GetByIDServicio(100)
    End Sub
    Public Sub Cerrar()
        'Se activa el boton de Locales
        Me.Close()
    End Sub

End Class