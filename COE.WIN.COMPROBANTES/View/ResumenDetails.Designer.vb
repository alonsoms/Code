﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ResumenDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ResumenDetails))
        Me.RibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.btnCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.btnComprobanteXML = New DevExpress.XtraBars.BarButtonItem()
        Me.btnConstanciaRecepcionXML = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPage1 = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroup2 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.XtraTabControl1 = New DevExpress.XtraTab.XtraTabControl()
        Me.XtraTabPage3 = New DevExpress.XtraTab.XtraTabPage()
        Me.LayoutControl1 = New DevExpress.XtraLayout.LayoutControl()
        Me.txtFechaResumen = New DevExpress.XtraEditors.TextEdit()
        Me.txtFechaEmision = New DevExpress.XtraEditors.TextEdit()
        Me.txtRutaConstanciaRecepcionXML = New DevExpress.XtraEditors.TextEdit()
        Me.txtRutaComprobanteXML = New DevExpress.XtraEditors.TextEdit()
        Me.txtObservacion = New DevExpress.XtraEditors.TextEdit()
        Me.txtFirma = New DevExpress.XtraEditors.TextEdit()
        Me.txtNumeroResumen = New DevExpress.XtraEditors.TextEdit()
        Me.txtIDResumne = New DevExpress.XtraEditors.TextEdit()
        Me.LayoutControlGroup1 = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.LayoutControlItem1 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem2 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem5 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem6 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem7 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem8 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem3 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItem9 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.XtraTabPage1 = New DevExpress.XtraTab.XtraTabPage()
        Me.wbComprobanteXML = New System.Windows.Forms.WebBrowser()
        Me.XtraTabPage2 = New DevExpress.XtraTab.XtraTabPage()
        Me.wbConstanciaRecepcionXML = New System.Windows.Forms.WebBrowser()
        Me.XtraTabPage4 = New DevExpress.XtraTab.XtraTabPage()
        Me.wbRegistroXML = New System.Windows.Forms.WebBrowser()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XtraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.XtraTabControl1.SuspendLayout()
        Me.XtraTabPage3.SuspendLayout()
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LayoutControl1.SuspendLayout()
        CType(Me.txtFechaResumen.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtFechaEmision.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtRutaConstanciaRecepcionXML.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtRutaComprobanteXML.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtObservacion.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtFirma.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtNumeroResumen.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtIDResumne.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlGroup1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItem9, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.XtraTabPage1.SuspendLayout()
        Me.XtraTabPage2.SuspendLayout()
        Me.XtraTabPage4.SuspendLayout()
        Me.SuspendLayout()
        '
        'RibbonControl
        '
        Me.RibbonControl.ExpandCollapseItem.Id = 0
        Me.RibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.RibbonControl.ExpandCollapseItem, Me.btnCerrar, Me.btnComprobanteXML, Me.btnConstanciaRecepcionXML})
        Me.RibbonControl.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl.MaxItemId = 5
        Me.RibbonControl.Name = "RibbonControl"
        Me.RibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPage1})
        Me.RibbonControl.Size = New System.Drawing.Size(637, 143)
        '
        'btnCerrar
        '
        Me.btnCerrar.Caption = "Cerrar"
        Me.btnCerrar.Id = 1
        Me.btnCerrar.ImageOptions.LargeImage = Global.COE.WINDOWS.My.Resources.Resources.Cerrar32
        Me.btnCerrar.Name = "btnCerrar"
        '
        'btnComprobanteXML
        '
        Me.btnComprobanteXML.Caption = "Descargar Comprobante XML"
        Me.btnComprobanteXML.Id = 2
        Me.btnComprobanteXML.ImageOptions.Image = Global.COE.WINDOWS.My.Resources.Resources.exporttoxml_16x16
        Me.btnComprobanteXML.ImageOptions.LargeImage = Global.COE.WINDOWS.My.Resources.Resources.exporttoxml_32x32
        Me.btnComprobanteXML.Name = "btnComprobanteXML"
        '
        'btnConstanciaRecepcionXML
        '
        Me.btnConstanciaRecepcionXML.Caption = "Descargar Constancia Recepción XML"
        Me.btnConstanciaRecepcionXML.Id = 4
        Me.btnConstanciaRecepcionXML.ImageOptions.Image = Global.COE.WINDOWS.My.Resources.Resources.exporttomht_16x16
        Me.btnConstanciaRecepcionXML.ImageOptions.LargeImage = Global.COE.WINDOWS.My.Resources.Resources.exporttomht_32x32
        Me.btnConstanciaRecepcionXML.Name = "btnConstanciaRecepcionXML"
        '
        'RibbonPage1
        '
        Me.RibbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroup1, Me.RibbonPageGroup2})
        Me.RibbonPage1.Name = "RibbonPage1"
        Me.RibbonPage1.Text = "Resumen Diario"
        '
        'RibbonPageGroup1
        '
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnComprobanteXML)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.btnConstanciaRecepcionXML)
        Me.RibbonPageGroup1.Name = "RibbonPageGroup1"
        Me.RibbonPageGroup1.Text = "Operaciones"
        '
        'RibbonPageGroup2
        '
        Me.RibbonPageGroup2.ItemLinks.Add(Me.btnCerrar)
        Me.RibbonPageGroup2.Name = "RibbonPageGroup2"
        Me.RibbonPageGroup2.Text = "Formulario"
        '
        'XtraTabControl1
        '
        Me.XtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.XtraTabControl1.Location = New System.Drawing.Point(0, 143)
        Me.XtraTabControl1.Name = "XtraTabControl1"
        Me.XtraTabControl1.SelectedTabPage = Me.XtraTabPage3
        Me.XtraTabControl1.Size = New System.Drawing.Size(637, 333)
        Me.XtraTabControl1.TabIndex = 2
        Me.XtraTabControl1.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.XtraTabPage3, Me.XtraTabPage1, Me.XtraTabPage2, Me.XtraTabPage4})
        '
        'XtraTabPage3
        '
        Me.XtraTabPage3.Controls.Add(Me.LayoutControl1)
        Me.XtraTabPage3.Name = "XtraTabPage3"
        Me.XtraTabPage3.Size = New System.Drawing.Size(631, 305)
        Me.XtraTabPage3.Text = "DATOS GENERALES"
        '
        'LayoutControl1
        '
        Me.LayoutControl1.Controls.Add(Me.txtFechaResumen)
        Me.LayoutControl1.Controls.Add(Me.txtFechaEmision)
        Me.LayoutControl1.Controls.Add(Me.txtRutaConstanciaRecepcionXML)
        Me.LayoutControl1.Controls.Add(Me.txtRutaComprobanteXML)
        Me.LayoutControl1.Controls.Add(Me.txtObservacion)
        Me.LayoutControl1.Controls.Add(Me.txtFirma)
        Me.LayoutControl1.Controls.Add(Me.txtNumeroResumen)
        Me.LayoutControl1.Controls.Add(Me.txtIDResumne)
        Me.LayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LayoutControl1.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControl1.Name = "LayoutControl1"
        Me.LayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = New System.Drawing.Rectangle(1243, 333, 650, 400)
        Me.LayoutControl1.Root = Me.LayoutControlGroup1
        Me.LayoutControl1.Size = New System.Drawing.Size(631, 305)
        Me.LayoutControl1.TabIndex = 0
        Me.LayoutControl1.Text = "LayoutControl1"
        '
        'txtFechaResumen
        '
        Me.txtFechaResumen.Location = New System.Drawing.Point(190, 177)
        Me.txtFechaResumen.MenuManager = Me.RibbonControl
        Me.txtFechaResumen.Name = "txtFechaResumen"
        Me.txtFechaResumen.Size = New System.Drawing.Size(429, 20)
        Me.txtFechaResumen.StyleController = Me.LayoutControl1
        Me.txtFechaResumen.TabIndex = 14
        '
        'txtFechaEmision
        '
        Me.txtFechaEmision.Location = New System.Drawing.Point(190, 153)
        Me.txtFechaEmision.MenuManager = Me.RibbonControl
        Me.txtFechaEmision.Name = "txtFechaEmision"
        Me.txtFechaEmision.Size = New System.Drawing.Size(429, 20)
        Me.txtFechaEmision.StyleController = Me.LayoutControl1
        Me.txtFechaEmision.TabIndex = 12
        '
        'txtRutaConstanciaRecepcionXML
        '
        Me.txtRutaConstanciaRecepcionXML.Location = New System.Drawing.Point(190, 273)
        Me.txtRutaConstanciaRecepcionXML.MenuManager = Me.RibbonControl
        Me.txtRutaConstanciaRecepcionXML.Name = "txtRutaConstanciaRecepcionXML"
        Me.txtRutaConstanciaRecepcionXML.Size = New System.Drawing.Size(429, 20)
        Me.txtRutaConstanciaRecepcionXML.StyleController = Me.LayoutControl1
        Me.txtRutaConstanciaRecepcionXML.TabIndex = 11
        '
        'txtRutaComprobanteXML
        '
        Me.txtRutaComprobanteXML.Location = New System.Drawing.Point(190, 249)
        Me.txtRutaComprobanteXML.MenuManager = Me.RibbonControl
        Me.txtRutaComprobanteXML.Name = "txtRutaComprobanteXML"
        Me.txtRutaComprobanteXML.Size = New System.Drawing.Size(429, 20)
        Me.txtRutaComprobanteXML.StyleController = Me.LayoutControl1
        Me.txtRutaComprobanteXML.TabIndex = 10
        '
        'txtObservacion
        '
        Me.txtObservacion.Location = New System.Drawing.Point(190, 225)
        Me.txtObservacion.MenuManager = Me.RibbonControl
        Me.txtObservacion.Name = "txtObservacion"
        Me.txtObservacion.Properties.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.[True]
        Me.txtObservacion.Properties.ContextImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.[True]
        Me.txtObservacion.Size = New System.Drawing.Size(429, 20)
        Me.txtObservacion.StyleController = Me.LayoutControl1
        Me.txtObservacion.TabIndex = 9
        '
        'txtFirma
        '
        Me.txtFirma.Location = New System.Drawing.Point(190, 201)
        Me.txtFirma.MenuManager = Me.RibbonControl
        Me.txtFirma.Name = "txtFirma"
        Me.txtFirma.Size = New System.Drawing.Size(429, 20)
        Me.txtFirma.StyleController = Me.LayoutControl1
        Me.txtFirma.TabIndex = 8
        '
        'txtNumeroResumen
        '
        Me.txtNumeroResumen.Location = New System.Drawing.Point(190, 129)
        Me.txtNumeroResumen.MenuManager = Me.RibbonControl
        Me.txtNumeroResumen.Name = "txtNumeroResumen"
        Me.txtNumeroResumen.Size = New System.Drawing.Size(429, 20)
        Me.txtNumeroResumen.StyleController = Me.LayoutControl1
        Me.txtNumeroResumen.TabIndex = 5
        '
        'txtIDResumne
        '
        Me.txtIDResumne.Location = New System.Drawing.Point(190, 12)
        Me.txtIDResumne.MenuManager = Me.RibbonControl
        Me.txtIDResumne.Name = "txtIDResumne"
        Me.txtIDResumne.Size = New System.Drawing.Size(429, 20)
        Me.txtIDResumne.StyleController = Me.LayoutControl1
        Me.txtIDResumne.TabIndex = 4
        '
        'LayoutControlGroup1
        '
        Me.LayoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.[True]
        Me.LayoutControlGroup1.GroupBordersVisible = False
        Me.LayoutControlGroup1.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.LayoutControlItem1, Me.LayoutControlItem2, Me.LayoutControlItem5, Me.LayoutControlItem6, Me.LayoutControlItem7, Me.LayoutControlItem8, Me.LayoutControlItem3, Me.LayoutControlItem9})
        Me.LayoutControlGroup1.Name = "Root"
        Me.LayoutControlGroup1.Size = New System.Drawing.Size(631, 305)
        Me.LayoutControlGroup1.TextVisible = False
        '
        'LayoutControlItem1
        '
        Me.LayoutControlItem1.Control = Me.txtIDResumne
        Me.LayoutControlItem1.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlItem1.Name = "LayoutControlItem1"
        Me.LayoutControlItem1.Size = New System.Drawing.Size(611, 117)
        Me.LayoutControlItem1.Text = "ID.Resumen"
        Me.LayoutControlItem1.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem2
        '
        Me.LayoutControlItem2.Control = Me.txtNumeroResumen
        Me.LayoutControlItem2.Location = New System.Drawing.Point(0, 117)
        Me.LayoutControlItem2.Name = "LayoutControlItem2"
        Me.LayoutControlItem2.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem2.Text = "Numero de resumen"
        Me.LayoutControlItem2.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem5
        '
        Me.LayoutControlItem5.Control = Me.txtFirma
        Me.LayoutControlItem5.Location = New System.Drawing.Point(0, 189)
        Me.LayoutControlItem5.Name = "LayoutControlItem5"
        Me.LayoutControlItem5.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem5.Text = "Firma"
        Me.LayoutControlItem5.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem6
        '
        Me.LayoutControlItem6.Control = Me.txtObservacion
        Me.LayoutControlItem6.Location = New System.Drawing.Point(0, 213)
        Me.LayoutControlItem6.Name = "LayoutControlItem6"
        Me.LayoutControlItem6.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem6.Text = "Observación/Excepción"
        Me.LayoutControlItem6.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem7
        '
        Me.LayoutControlItem7.Control = Me.txtRutaComprobanteXML
        Me.LayoutControlItem7.Location = New System.Drawing.Point(0, 237)
        Me.LayoutControlItem7.Name = "LayoutControlItem7"
        Me.LayoutControlItem7.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem7.Text = "Comprobante XML"
        Me.LayoutControlItem7.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem8
        '
        Me.LayoutControlItem8.Control = Me.txtRutaConstanciaRecepcionXML
        Me.LayoutControlItem8.Location = New System.Drawing.Point(0, 261)
        Me.LayoutControlItem8.Name = "LayoutControlItem8"
        Me.LayoutControlItem8.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem8.Text = "Constancia de recepción SUNAT XML"
        Me.LayoutControlItem8.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem3
        '
        Me.LayoutControlItem3.Control = Me.txtFechaEmision
        Me.LayoutControlItem3.Location = New System.Drawing.Point(0, 141)
        Me.LayoutControlItem3.Name = "LayoutControlItem3"
        Me.LayoutControlItem3.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem3.Text = "Fecha de Emisión"
        Me.LayoutControlItem3.TextSize = New System.Drawing.Size(175, 13)
        '
        'LayoutControlItem9
        '
        Me.LayoutControlItem9.Control = Me.txtFechaResumen
        Me.LayoutControlItem9.Location = New System.Drawing.Point(0, 165)
        Me.LayoutControlItem9.Name = "LayoutControlItem9"
        Me.LayoutControlItem9.Size = New System.Drawing.Size(611, 24)
        Me.LayoutControlItem9.Text = "Fecha de Resumen"
        Me.LayoutControlItem9.TextSize = New System.Drawing.Size(175, 13)
        '
        'XtraTabPage1
        '
        Me.XtraTabPage1.Controls.Add(Me.wbComprobanteXML)
        Me.XtraTabPage1.Name = "XtraTabPage1"
        Me.XtraTabPage1.Size = New System.Drawing.Size(890, 416)
        Me.XtraTabPage1.Text = "COMPROBANTE XML"
        '
        'wbComprobanteXML
        '
        Me.wbComprobanteXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbComprobanteXML.Location = New System.Drawing.Point(0, 0)
        Me.wbComprobanteXML.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbComprobanteXML.Name = "wbComprobanteXML"
        Me.wbComprobanteXML.Size = New System.Drawing.Size(890, 416)
        Me.wbComprobanteXML.TabIndex = 2
        '
        'XtraTabPage2
        '
        Me.XtraTabPage2.Controls.Add(Me.wbConstanciaRecepcionXML)
        Me.XtraTabPage2.Name = "XtraTabPage2"
        Me.XtraTabPage2.Size = New System.Drawing.Size(890, 416)
        Me.XtraTabPage2.Text = "CONSTANCIA DE RECEPCION SUNAT XML"
        '
        'wbConstanciaRecepcionXML
        '
        Me.wbConstanciaRecepcionXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbConstanciaRecepcionXML.Location = New System.Drawing.Point(0, 0)
        Me.wbConstanciaRecepcionXML.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbConstanciaRecepcionXML.Name = "wbConstanciaRecepcionXML"
        Me.wbConstanciaRecepcionXML.Size = New System.Drawing.Size(890, 416)
        Me.wbConstanciaRecepcionXML.TabIndex = 0
        '
        'XtraTabPage4
        '
        Me.XtraTabPage4.Controls.Add(Me.wbRegistroXML)
        Me.XtraTabPage4.Name = "XtraTabPage4"
        Me.XtraTabPage4.Size = New System.Drawing.Size(890, 416)
        Me.XtraTabPage4.Text = "REGISTRO XML"
        '
        'wbRegistroXML
        '
        Me.wbRegistroXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbRegistroXML.Location = New System.Drawing.Point(0, 0)
        Me.wbRegistroXML.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbRegistroXML.Name = "wbRegistroXML"
        Me.wbRegistroXML.Size = New System.Drawing.Size(890, 416)
        Me.wbRegistroXML.TabIndex = 2
        '
        'ResumenDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(637, 476)
        Me.Controls.Add(Me.XtraTabControl1)
        Me.Controls.Add(Me.RibbonControl)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "ResumenDetails"
        Me.Ribbon = Me.RibbonControl
        Me.Text = "ResumenDetails"
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XtraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.XtraTabControl1.ResumeLayout(False)
        Me.XtraTabPage3.ResumeLayout(False)
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LayoutControl1.ResumeLayout(False)
        CType(Me.txtFechaResumen.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtFechaEmision.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtRutaConstanciaRecepcionXML.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtRutaComprobanteXML.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtObservacion.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtFirma.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtNumeroResumen.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtIDResumne.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlGroup1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItem9, System.ComponentModel.ISupportInitialize).EndInit()
        Me.XtraTabPage1.ResumeLayout(False)
        Me.XtraTabPage2.ResumeLayout(False)
        Me.XtraTabPage4.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents RibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
    Friend WithEvents RibbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage
    Friend WithEvents RibbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents XtraTabControl1 As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents XtraTabPage1 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents XtraTabPage2 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents XtraTabPage3 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents btnCerrar As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents btnComprobanteXML As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents XtraTabPage4 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents btnConstanciaRecepcionXML As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents RibbonPageGroup2 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents wbRegistroXML As System.Windows.Forms.WebBrowser
    Friend WithEvents wbComprobanteXML As System.Windows.Forms.WebBrowser
    Friend WithEvents wbConstanciaRecepcionXML As System.Windows.Forms.WebBrowser
    Friend WithEvents LayoutControl1 As DevExpress.XtraLayout.LayoutControl
    Friend WithEvents txtNumeroResumen As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtIDResumne As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LayoutControlGroup1 As DevExpress.XtraLayout.LayoutControlGroup
    Friend WithEvents LayoutControlItem1 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlItem2 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents txtObservacion As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtFirma As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LayoutControlItem5 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlItem6 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents txtRutaConstanciaRecepcionXML As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtRutaComprobanteXML As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LayoutControlItem7 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlItem8 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents txtFechaResumen As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtFechaEmision As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LayoutControlItem3 As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlItem9 As DevExpress.XtraLayout.LayoutControlItem


End Class
