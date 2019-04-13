Imports COE.DATA
Imports COE.FRAMEWORK

Public Class SunatList
    Dim bsExcepciones As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.GridControl1.DataSource = bsExcepciones

        'Se inicializa los controles DevExpress
        ControlesDevExpress.InitRibbonControl(RibbonControl)
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.EXCEPCION", "codigo", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "DESCRIPCION", "nombre", 550, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "...", "", 50, True)

        'Se configura los eventos
        AddHandler Me.KeyDown, AddressOf Tools.Teclado

        'Se carga los registros
        Actualizar()

    End Sub
    Private Sub SunatList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Actualizar" : Actualizar()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub SunatList_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Sunat", eMenuFormulario.Open)
    End Sub
    Private Sub SunatList_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Sunat", eMenuFormulario.Close)
    End Sub
    Public Sub Actualizar()
        bsExcepciones.DataSource = SunatDAO.GetByALL()
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub

End Class