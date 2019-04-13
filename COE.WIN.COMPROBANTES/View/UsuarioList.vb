Imports COE.DATA
Imports COE.FRAMEWORK

Public Class UsuarioList
    Dim bsUsuarios As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.GridControl1.DataSource = bsUsuarios

        'Se inicializa controles DevExpress
        ControlesDevExpress.InitRibbonControl(RibbonControl)
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.USUARIO", "IDUsuario", 80, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NOMBRES", "Nombres", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "AP.PATERNO", "ApellidoPaterno", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "AP.MATERNO", "ApellidoMaterno", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "LOGIN", "Login", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "PASSWORD", "Password", 80, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "...", "", 250, True)

        'Se configura los eventos
        AddHandler Me.KeyDown, AddressOf Tools.Teclado

        'Se carga los registros
        btnActualizar.PerformClick()

    End Sub

    Private Sub UsuarioList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Actualizar" : Actualizar()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub UsuarioList_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Usuarios", eMenuFormulario.Open)
    End Sub
    Private Sub UsuarioList_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Usuarios", eMenuFormulario.Close)
    End Sub

    Public Sub Actualizar()
        bsUsuarios.DataSource = UsuarioDAO.GetByALL()
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub

End Class