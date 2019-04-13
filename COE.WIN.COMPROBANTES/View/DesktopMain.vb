Imports System.Drawing.Printing
Imports System.Globalization
Imports COE.DATA
Imports COE.FRAMEWORK
Imports DevExpress.LookAndFeel
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.XtraNavBar
Imports DevExpress.XtraReports.UI

Public Enum eMenuFormulario
    Close = 1
    Open = 2
End Enum
Public Class DesktopMain

    Public Sub New()

        'Se carga el skin por defecto
        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()
        UserLookAndFeel.Default.SetSkinStyle(Tools.ReadAppSettings("Skin"))

        'Se actualiza la fuente de la aplicacion
        DevExpress.Utils.AppearanceObject.DefaultFont = New Font(Tools.ReadAppSettings("FuenteName"), Convert.ToSingle(Tools.ReadAppSettings("FuenteSize")), CType(Tools.ReadAppSettings("FuenteStyle"), System.Drawing.FontStyle))
        LookAndFeelHelper.ForceDefaultLookAndFeelChanged()

        'Se inicializa los componentes del formulario
        InitializeComponent()

        'Se configura el formulario
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Maximized
        Me.Text = SistemaDAO.NombreAplicacion

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control NavBar
        NavBarControl1.BeginUpdate()

        ControlesDevExpress.InitNavBar(NavBarControl1)

        ControlesDevExpress.InitNavBarMenu(NavBarControl1,
                                           {"COMPROBANTES", "Facturas", "Boletas de Venta", "Notas de Credito", "Notas de Debito", "Resumen Diario", "Comunicación de Baja", "Tareas", "Reportes"},
                                           {Tools.GetIcono(eIcon.Comprobantes),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado)})


        ControlesDevExpress.InitNavBarMenu(NavBarControl1,
                                            {"CONFIGURACION", "Usuarios", "Emisor", "Sunat"},
                                           {Tools.GetIcono(eIcon.Configuracion),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado),
                                            Tools.GetIcono(eIcon.FolderCerrado)})

        'Se enlaza evento al Control NavBar
        AddHandler NavBarControl1.LinkClicked, AddressOf DesktopMain_LinkClicked

        'Se activa grupo por defecto
        ' NavBarControl1.Groups(0).Expanded = True
        ' NavBarControl1.Groups(1).Expanded = True

        NavBarControl1.EndUpdate()

        'Se inicia el timer
        Timer1.Interval = 1500
        Timer1.Start()


    End Sub
    Private Sub InitSkinGallery()
        ' SkinHelper.InitSkinGallery(rgbiSkins, True)
    End Sub

    Private Sub DesktopMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            'Se configura la cultura de la aplicacion
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-PE")

            'Se configura control NavBar
            '  InitControlNavBar()

            RibbonControl.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always

            'Se inicia el timer
            Timer1.Start()

            'Se obtiene el numero de versión
            'Se carga los valores de la barra de estado
            Me.Text = SistemaDAO.NombreAplicacion
            Me.btnUsuario.Caption = SistemaDAO.UsuarioBE.Nombres & " " & SistemaDAO.UsuarioBE.ApellidoPaterno
            Me.btnComputadora.Caption = SistemaDAO.NombrePC
            Me.btnFechaHora.Caption = DateTime.Now
            Me.btnEmpresa.Caption = SistemaDAO.EmisorBE.RazonSocial & " - " & SistemaDAO.EmisorBE.NumeroRUC

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DesktopMain_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnFuente.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Fuente" : Fuente()
                Case "Salir del programa" : Salir()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DesktopMain_LinkClicked(ByVal sender As Object, ByVal e As NavBarLinkEventArgs)

        Select Case e.Link.Caption
            Case "Facturas"
                FacturasList.MdiParent = Me
                FacturasList.WindowState = FormWindowState.Maximized
                FacturasList.Show()

            Case "Boletas de Venta"
                BoletasList.MdiParent = Me
                BoletasList.WindowState = FormWindowState.Maximized
                BoletasList.Show()

            Case "Notas de Credito"
                NotaCreditoList.MdiParent = Me
                NotaCreditoList.WindowState = FormWindowState.Maximized
                NotaCreditoList.Show()

            Case "Notas de Debito"
                NotaDebitoList.MdiParent = Me
                NotaDebitoList.WindowState = FormWindowState.Maximized
                NotaDebitoList.Show()

            Case "Comunicación de Baja"
                ComunicacionBajaList.MdiParent = Me
                ComunicacionBajaList.WindowState = FormWindowState.Maximized
                ComunicacionBajaList.Show()

            Case "Tareas"
                ServicioList.MdiParent = Me
                ServicioList.WindowState = FormWindowState.Maximized
                ServicioList.Show()

            Case "Resumen Diario"
                ResumenList.MdiParent = Me
                ResumenList.WindowState = FormWindowState.Maximized
                ResumenList.Show()

            Case "Emisor"
                EmisorDetails.MdiParent = Me
                EmisorDetails.WindowState = FormWindowState.Maximized
                EmisorDetails.Show()

            Case "Sunat"
                SunatList.MdiParent = Me
                SunatList.WindowState = FormWindowState.Maximized
                SunatList.Show()

            Case "Usuarios"
                UsuarioList.MdiParent = Me
                UsuarioList.WindowState = FormWindowState.Maximized
                UsuarioList.Show()

            Case "Reportes"
                ReporteLiquidacion()
        End Select

    End Sub


    Public Sub ReporteLiquidacion()
        Dim MiForm As New LiquidacionReport
        MiForm.ShowDialog()

    End Sub

    Public Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        'Se muestra la fecha y hora cada 15 segundos
        btnFechaHora.Caption = DateTime.Now

    End Sub

    Private Sub rgbiSkins_GalleryItemClick(sender As Object, e As GalleryItemClickEventArgs) Handles rgbiSkins.GalleryItemClick
        Try
            'Se registra el skin que selecciona el usuario
            Tools.SaveAppSettings("Skin", e.Item.Caption)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub Salir()
        If MessageBox.Show("¿Esta seguro de salir del programa?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then
            Timer1.Stop()
            Application.Exit()
        End If
    End Sub
    Public Sub Fuente()
        Try
            'Se cambia la fuente de la aplicacion
            If FontDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

                'Se actualiza la fuente de la aplicacion
                DevExpress.Utils.AppearanceObject.DefaultFont = New Font(FontDialog1.Font.Name, FontDialog1.Font.Size, FontDialog1.Font.Style)

                'Se reinicia estilo de la fuente
                LookAndFeelHelper.ForceDefaultLookAndFeelChanged()

                'Se registra la configuracion de la fuente
                Tools.SaveAppSettings("FuenteName", FontDialog1.Font.Name)
                Tools.SaveAppSettings("FuenteSize", FontDialog1.Font.Size)
                Tools.SaveAppSettings("FuenteStyle", FontDialog1.Font.Style)

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Function GetDefaultPrinter() As String
        Dim settings As PrinterSettings = New PrinterSettings()
        Dim Result As String = settings.PrinterName

        If Result = "" Then
            Result = "No hay impresora"
        End If

        Return settings.PrinterName
    End Function

    Private Sub RibbonControl_Merge(sender As Object, e As DevExpress.XtraBars.Ribbon.RibbonMergeEventArgs) Handles RibbonControl.Merge
        'Se establece el RibbonPage Activo, cada vez que un formulario se Maximiza
        Me.Ribbon.SelectedPage = Me.Ribbon.MergedPages.Item(0)
    End Sub

    Public Sub MenuBar(KeyForm As String, Estilo As eMenuFormulario)

        'Se busca el menu y se cambia los iconos
        For Index1 = 0 To NavBarControl1.Groups.Count - 1
            For Index2 = 0 To NavBarControl1.Groups(Index1).ItemLinks.Count - 1
                If NavBarControl1.Groups(Index1).ItemLinks(Index2).ItemName = KeyForm Then
                    NavBarControl1.Groups(Index1).ItemLinks(Index2).Item.SmallImage = Tools.GetIcono(Estilo)
                    NavBarControl1.SelectedLink = If(Estilo = eMenuFormulario.Open, NavBarControl1.Groups(Index1).ItemLinks(Index2), Nothing)
                End If
            Next
        Next

    End Sub

    Private Sub DesktopMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Application.Exit()
    End Sub
End Class