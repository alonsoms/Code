<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FacturasList
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
        Dim SuperToolTip1 As DevExpress.Utils.SuperToolTip = New DevExpress.Utils.SuperToolTip()
        Dim ToolTipTitleItem1 As DevExpress.Utils.ToolTipTitleItem = New DevExpress.Utils.ToolTipTitleItem()
        Dim ToolTipItem1 As DevExpress.Utils.ToolTipItem = New DevExpress.Utils.ToolTipItem()
        Dim SuperToolTip2 As DevExpress.Utils.SuperToolTip = New DevExpress.Utils.SuperToolTip()
        Dim ToolTipTitleItem2 As DevExpress.Utils.ToolTipTitleItem = New DevExpress.Utils.ToolTipTitleItem()
        Dim ToolTipItem2 As DevExpress.Utils.ToolTipItem = New DevExpress.Utils.ToolTipItem()
        Dim SuperToolTip3 As DevExpress.Utils.SuperToolTip = New DevExpress.Utils.SuperToolTip()
        Dim ToolTipTitleItem3 As DevExpress.Utils.ToolTipTitleItem = New DevExpress.Utils.ToolTipTitleItem()
        Dim ToolTipItem3 As DevExpress.Utils.ToolTipItem = New DevExpress.Utils.ToolTipItem()
        Dim SuperToolTip4 As DevExpress.Utils.SuperToolTip = New DevExpress.Utils.SuperToolTip()
        Dim ToolTipTitleItem4 As DevExpress.Utils.ToolTipTitleItem = New DevExpress.Utils.ToolTipTitleItem()
        Dim ToolTipItem4 As DevExpress.Utils.ToolTipItem = New DevExpress.Utils.ToolTipItem()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FacturasList))
        Me.RibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.btnEnviar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnRecuperar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnActualizar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnFechaInicial = New DevExpress.XtraBars.BarEditItem()
        Me.RepositoryItemDateEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemDateEdit()
        Me.btnFechaFinal = New DevExpress.XtraBars.BarEditItem()
        Me.RepositoryItemDateEdit2 = New DevExpress.XtraEditors.Repository.RepositoryItemDateEdit()
        Me.btnVerComprobante = New DevExpress.XtraBars.BarButtonItem()
        Me.btnFirmarComprobante = New DevExpress.XtraBars.BarButtonItem()
        Me.btnEmailComprobante = New DevExpress.XtraBars.BarButtonItem()
        Me.btnImprimir = New DevExpress.XtraBars.BarButtonItem()
        Me.btnEnviarWebComprobante = New DevExpress.XtraBars.BarButtonItem()
        Me.btnBuscar = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPage1 = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroup2 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroup3 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.GridControl1 = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemDateEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemDateEdit1.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemDateEdit2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemDateEdit2.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RibbonControl
        '
        Me.RibbonControl.ExpandCollapseItem.Id = 0
        Me.RibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.RibbonControl.ExpandCollapseItem, Me.btnEnviar, Me.btnRecuperar, Me.btnActualizar, Me.btnCerrar, Me.btnFechaInicial, Me.btnFechaFinal, Me.btnVerComprobante, Me.btnFirmarComprobante, Me.btnEmailComprobante, Me.btnImprimir, Me.btnEnviarWebComprobante, Me.btnBuscar})
        Me.RibbonControl.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl.MaxItemId = 17
        Me.RibbonControl.Name = "RibbonControl"
        Me.RibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPage1})
        Me.RibbonControl.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.RepositoryItemDateEdit1, Me.RepositoryItemDateEdit2})
        Me.RibbonControl.Size = New System.Drawing.Size(936, 143)
        '
        'btnEnviar
        '
        Me.btnEnviar.Caption = "Enviar Comprobante"
        Me.btnEnviar.Id = 1
        Me.btnEnviar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.EnviarCE32
        Me.btnEnviar.Name = "btnEnviar"
        ToolTipTitleItem1.Text = "Enviar Comprobante"
        ToolTipItem1.LeftIndent = 6
        ToolTipItem1.Text = "Este proceso envia el comprobante empaquetado a los servidores de la SUNAT"
        SuperToolTip1.Items.Add(ToolTipTitleItem1)
        SuperToolTip1.Items.Add(ToolTipItem1)
        Me.btnEnviar.SuperTip = SuperToolTip1
        '
        'btnRecuperar
        '
        Me.btnRecuperar.Caption = "Recuperar Constancia CDR"
        Me.btnRecuperar.Id = 2
        Me.btnRecuperar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.DescargeCDR32
        Me.btnRecuperar.Name = "btnRecuperar"
        ToolTipTitleItem2.Text = "Recuperar Constancia CDR"
        ToolTipItem2.LeftIndent = 6
        ToolTipItem2.Text = "Este proceso recupera la constancia de recepción de los comprobantes de la SUNAT " &
    ""
        SuperToolTip2.Items.Add(ToolTipTitleItem2)
        SuperToolTip2.Items.Add(ToolTipItem2)
        Me.btnRecuperar.SuperTip = SuperToolTip2
        '
        'btnActualizar
        '
        Me.btnActualizar.Caption = "Actualizar"
        Me.btnActualizar.Id = 3
        Me.btnActualizar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.Refrescar32
        Me.btnActualizar.Name = "btnActualizar"
        '
        'btnCerrar
        '
        Me.btnCerrar.Caption = "Cerrar"
        Me.btnCerrar.Id = 4
        Me.btnCerrar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.Cerrar32
        Me.btnCerrar.Name = "btnCerrar"
        '
        'btnFechaInicial
        '
        Me.btnFechaInicial.Caption = "Fecha Inicial"
        Me.btnFechaInicial.Edit = Me.RepositoryItemDateEdit1
        Me.btnFechaInicial.Id = 5
        Me.btnFechaInicial.Name = "btnFechaInicial"
        Me.btnFechaInicial.Width = 120
        '
        'RepositoryItemDateEdit1
        '
        Me.RepositoryItemDateEdit1.AutoHeight = False
        Me.RepositoryItemDateEdit1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemDateEdit1.Name = "RepositoryItemDateEdit1"
        '
        'btnFechaFinal
        '
        Me.btnFechaFinal.Caption = "Fecha Final"
        Me.btnFechaFinal.Edit = Me.RepositoryItemDateEdit2
        Me.btnFechaFinal.Id = 6
        Me.btnFechaFinal.Name = "btnFechaFinal"
        Me.btnFechaFinal.Width = 125
        '
        'RepositoryItemDateEdit2
        '
        Me.RepositoryItemDateEdit2.AutoHeight = False
        Me.RepositoryItemDateEdit2.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemDateEdit2.Name = "RepositoryItemDateEdit2"
        '
        'btnVerComprobante
        '
        Me.btnVerComprobante.Caption = "Ver Comprobante"
        Me.btnVerComprobante.Id = 7
        Me.btnVerComprobante.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.VerCE32
        Me.btnVerComprobante.Name = "btnVerComprobante"
        '
        'btnFirmarComprobante
        '
        Me.btnFirmarComprobante.Caption = "Firmar Comprobante"
        Me.btnFirmarComprobante.Id = 11
        Me.btnFirmarComprobante.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.FirmarCE32
        Me.btnFirmarComprobante.Name = "btnFirmarComprobante"
        ToolTipTitleItem3.Text = "Firmar Comprobante"
        ToolTipItem3.LeftIndent = 6
        ToolTipItem3.Text = "Este procesa crea, firma y empaqueta el comprobante XML"
        SuperToolTip3.Items.Add(ToolTipTitleItem3)
        SuperToolTip3.Items.Add(ToolTipItem3)
        Me.btnFirmarComprobante.SuperTip = SuperToolTip3
        '
        'btnEmailComprobante
        '
        Me.btnEmailComprobante.Caption = "Enviar Email Comprobante"
        Me.btnEmailComprobante.Id = 13
        Me.btnEmailComprobante.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.EmailCE32
        Me.btnEmailComprobante.Name = "btnEmailComprobante"
        ToolTipTitleItem4.Text = "Enviar Email Comprobante"
        ToolTipItem4.LeftIndent = 6
        ToolTipItem4.Text = "Este proceso envia el comprobante electronico al adquiriente."
        SuperToolTip4.Items.Add(ToolTipTitleItem4)
        SuperToolTip4.Items.Add(ToolTipItem4)
        Me.btnEmailComprobante.SuperTip = SuperToolTip4
        '
        'btnImprimir
        '
        Me.btnImprimir.Caption = "Imprimir Comprobante"
        Me.btnImprimir.Id = 14
        Me.btnImprimir.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.CEImpresion32
        Me.btnImprimir.Name = "btnImprimir"
        '
        'btnEnviarWebComprobante
        '
        Me.btnEnviarWebComprobante.Caption = "Enviar Web Comprobante"
        Me.btnEnviarWebComprobante.Id = 15
        Me.btnEnviarWebComprobante.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.ComprobanteWeb32
        Me.btnEnviarWebComprobante.Name = "btnEnviarWebComprobante"
        '
        'btnBuscar
        '
        Me.btnBuscar.Caption = "Buscar"
        Me.btnBuscar.Id = 16
        Me.btnBuscar.LargeGlyph = Global.COE.WINDOWS.My.Resources.Resources.Find_32x32
        Me.btnBuscar.Name = "btnBuscar"
        '
        'RibbonPage1
        '
        Me.RibbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroup1, Me.RibbonPageGroup2, Me.RibbonPageGroup3})
        Me.RibbonPage1.Name = "RibbonPage1"
        Me.RibbonPage1.Text = "Facturas"
        '
        'RibbonPageGroup1
        '
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnFirmarComprobante)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnEnviar)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnRecuperar)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnEmailComprobante)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnEnviarWebComprobante)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnImprimir)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnVerComprobante)
        Me.RibbonPageGroup1.Name = "RibbonPageGroup1"
        Me.RibbonPageGroup1.Text = "Operaciones"
        '
        'RibbonPageGroup2
        '
        Me.RibbonPageGroup2.ItemLinks.Add(Me.btnFechaInicial)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.btnFechaFinal)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.btnBuscar)
        Me.RibbonPageGroup2.Name = "RibbonPageGroup2"
        Me.RibbonPageGroup2.Text = "Buscar x Fecha de Emisión"
        '
        'RibbonPageGroup3
        '
        Me.RibbonPageGroup3.ItemLinks.Add(Me.btnActualizar)
        Me.RibbonPageGroup3.ItemLinks.Add(Me.btnCerrar)
        Me.RibbonPageGroup3.Name = "RibbonPageGroup3"
        Me.RibbonPageGroup3.Text = "Formulario"
        '
        'GridControl1
        '
        Me.GridControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridControl1.Location = New System.Drawing.Point(0, 143)
        Me.GridControl1.MainView = Me.GridView1
        Me.GridControl1.MenuManager = Me.RibbonControl
        Me.GridControl1.Name = "GridControl1"
        Me.GridControl1.Size = New System.Drawing.Size(936, 394)
        Me.GridControl1.TabIndex = 2
        Me.GridControl1.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        Me.GridView1.GridControl = Me.GridControl1
        Me.GridView1.Name = "GridView1"
        '
        'FacturasList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(936, 537)
        Me.Controls.Add(Me.GridControl1)
        Me.Controls.Add(Me.RibbonControl)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "FacturasList"
        Me.Ribbon = Me.RibbonControl
        Me.Text = "FACTURAS"
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemDateEdit1.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemDateEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemDateEdit2.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemDateEdit2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents RibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
    Friend WithEvents RibbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage
    Friend WithEvents RibbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents btnEnviar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnRecuperar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents GridControl1 As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents btnActualizar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnCerrar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents RibbonPageGroup2 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonPageGroup3 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents btnFechaInicial As DevExpress.XtraBars.BarEditItem
    Friend WithEvents RepositoryItemDateEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemDateEdit
    Friend WithEvents btnFechaFinal As DevExpress.XtraBars.BarEditItem
    Friend WithEvents RepositoryItemDateEdit2 As DevExpress.XtraEditors.Repository.RepositoryItemDateEdit
    Friend WithEvents btnVerComprobante As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnFirmarComprobante As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents SplashScreenManager1 As DevExpress.XtraSplashScreen.SplashScreenManager
    Friend WithEvents btnEmailComprobante As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnImprimir As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnEnviarWebComprobante As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnBuscar As DevExpress.XtraBars.BarButtonItem

End Class
