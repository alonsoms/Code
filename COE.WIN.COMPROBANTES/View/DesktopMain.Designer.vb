<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesktopMain
    Inherits DevExpress.XtraBars.Ribbon.RibbonForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DesktopMain))
        Me.RibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.btnCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnUsuario = New DevExpress.XtraBars.BarButtonItem()
        Me.btnComputadora = New DevExpress.XtraBars.BarButtonItem()
        Me.btnFechaHora = New DevExpress.XtraBars.BarButtonItem()
        Me.rgbiSkins = New DevExpress.XtraBars.SkinRibbonGalleryBarItem()
        Me.btnFuente = New DevExpress.XtraBars.BarButtonItem()
        Me.btnEmpresa = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPage7 = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonStatusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()
        Me.GalleryDropDown1 = New DevExpress.XtraBars.Ribbon.GalleryDropDown(Me.components)
        Me.ImageCollection1 = New DevExpress.Utils.ImageCollection(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.NavBarControl1 = New DevExpress.XtraNavBar.NavBarControl()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GalleryDropDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ImageCollection1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NavBarControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RibbonControl
        '
        Me.RibbonControl.ExpandCollapseItem.Id = 0
        Me.RibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.RibbonControl.ExpandCollapseItem, Me.btnCerrar, Me.btnUsuario, Me.btnComputadora, Me.btnFechaHora, Me.rgbiSkins, Me.btnFuente, Me.btnEmpresa})
        Me.RibbonControl.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl.MaxItemId = 11
        Me.RibbonControl.Name = "RibbonControl"
        Me.RibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPage7})
        Me.RibbonControl.Size = New System.Drawing.Size(633, 143)
        Me.RibbonControl.StatusBar = Me.RibbonStatusBar
        '
        'btnCerrar
        '
        Me.btnCerrar.Caption = "Salir del programa"
        Me.btnCerrar.Glyph = Global.COE.WINDOWS.My.Resources.Resources.close_16x16
        Me.btnCerrar.Id = 2
        Me.btnCerrar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.close_32x32
        Me.btnCerrar.Name = "btnCerrar"
        '
        'btnUsuario
        '
        Me.btnUsuario.Caption = "Usuario"
        Me.btnUsuario.Glyph = Global.COE.WINDOWS.My.Resources.Resources.Usuario16
        Me.btnUsuario.Id = 3
        Me.btnUsuario.Name = "btnUsuario"
        '
        'btnComputadora
        '
        Me.btnComputadora.Caption = "Computadora"
        Me.btnComputadora.Glyph = Global.COE.WINDOWS.My.Resources.Resources.Estacion16
        Me.btnComputadora.Id = 4
        Me.btnComputadora.Name = "btnComputadora"
        '
        'btnFechaHora
        '
        Me.btnFechaHora.Caption = "FechaHora"
        Me.btnFechaHora.Glyph = Global.COE.WINDOWS.My.Resources.Resources.Calendario16
        Me.btnFechaHora.Id = 5
        Me.btnFechaHora.Name = "btnFechaHora"
        '
        'rgbiSkins
        '
        Me.rgbiSkins.Caption = "SkinRibbonGalleryBarItem1"
        Me.rgbiSkins.Id = 6
        Me.rgbiSkins.Name = "rgbiSkins"
        '
        'btnFuente
        '
        Me.btnFuente.Caption = "Fuente"
        Me.btnFuente.Glyph = Global.COE.WINDOWS.My.Resources.Resources.changefontstyle_16x16
        Me.btnFuente.Id = 7
        Me.btnFuente.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.changefontstyle_32x32
        Me.btnFuente.Name = "btnFuente"
        '
        'btnEmpresa
        '
        Me.btnEmpresa.Caption = "Empresa"
        Me.btnEmpresa.Glyph = Global.COE.WINDOWS.My.Resources.Resources.Empresa16
        Me.btnEmpresa.Id = 8
        Me.btnEmpresa.Name = "btnEmpresa"
        '
        'RibbonPage7
        '
        Me.RibbonPage7.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroup1})
        Me.RibbonPage7.Name = "RibbonPage7"
        Me.RibbonPage7.Text = "Sistema"
        '
        'RibbonPageGroup1
        '
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnCerrar)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.rgbiSkins)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnFuente)
        Me.RibbonPageGroup1.Name = "RibbonPageGroup1"
        Me.RibbonPageGroup1.Text = "Operaciones"
        '
        'RibbonStatusBar
        '
        Me.RibbonStatusBar.ItemLinks.Add(Me.btnUsuario)
        Me.RibbonStatusBar.ItemLinks.Add(Me.btnComputadora)
        Me.RibbonStatusBar.ItemLinks.Add(Me.btnFechaHora)
        Me.RibbonStatusBar.ItemLinks.Add(Me.btnEmpresa)
        Me.RibbonStatusBar.Location = New System.Drawing.Point(0, 466)
        Me.RibbonStatusBar.Name = "RibbonStatusBar"
        Me.RibbonStatusBar.Ribbon = Me.RibbonControl
        Me.RibbonStatusBar.Size = New System.Drawing.Size(633, 31)
        '
        'GalleryDropDown1
        '
        Me.GalleryDropDown1.Name = "GalleryDropDown1"
        Me.GalleryDropDown1.Ribbon = Me.RibbonControl
        '
        'ImageCollection1
        '
        Me.ImageCollection1.ImageStream = CType(resources.GetObject("ImageCollection1.ImageStream"), DevExpress.Utils.ImageCollectionStreamer)
        Me.ImageCollection1.InsertGalleryImage("cube_32x32.png", "images/function%20library/cube_32x32.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/function%20library/cube_32x32.png"), 0)
        Me.ImageCollection1.Images.SetKeyName(0, "cube_32x32.png")
        '
        'Timer1
        '
        '
        'NavBarControl1
        '
        Me.NavBarControl1.ActiveGroup = Nothing
        Me.NavBarControl1.Dock = System.Windows.Forms.DockStyle.Left
        Me.NavBarControl1.Location = New System.Drawing.Point(0, 143)
        Me.NavBarControl1.Name = "NavBarControl1"
        Me.NavBarControl1.OptionsNavPane.ExpandedWidth = 175
        Me.NavBarControl1.Size = New System.Drawing.Size(175, 323)
        Me.NavBarControl1.TabIndex = 3
        Me.NavBarControl1.Text = "NavBarControl1"
        '
        'DesktopMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(633, 497)
        Me.Controls.Add(Me.NavBarControl1)
        Me.Controls.Add(Me.RibbonStatusBar)
        Me.Controls.Add(Me.RibbonControl)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.Name = "DesktopMain"
        Me.Ribbon = Me.RibbonControl
        Me.StatusBar = Me.RibbonStatusBar
        Me.Text = "DesktopMain"
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GalleryDropDown1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ImageCollection1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NavBarControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents RibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
    Friend WithEvents RibbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
    Friend WithEvents ImageCollection1 As DevExpress.Utils.ImageCollection
    Friend WithEvents RibbonPage7 As DevExpress.XtraBars.Ribbon.RibbonPage
    Friend WithEvents btnCerrar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents RibbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents btnUsuario As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnComputadora As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnFechaHora As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents GalleryDropDown1 As DevExpress.XtraBars.Ribbon.GalleryDropDown
    Friend WithEvents rgbiSkins As DevExpress.XtraBars.SkinRibbonGalleryBarItem
    Friend WithEvents btnFuente As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents FontDialog1 As System.Windows.Forms.FontDialog
    Friend WithEvents btnEmpresa As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents NavBarControl1 As DevExpress.XtraNavBar.NavBarControl

End Class
