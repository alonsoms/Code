<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim Code128Generator1 As DevExpress.XtraPrinting.BarCode.Code128Generator = New DevExpress.XtraPrinting.BarCode.Code128Generator()
        Me.btnCrearXMLFirmar = New System.Windows.Forms.Button()
        Me.barCodeControl1 = New DevExpress.XtraEditors.BarCodeControl()
        Me.btnQR = New System.Windows.Forms.Button()
        Me.btnImprimir = New DevExpress.XtraEditors.SimpleButton()
        Me.btnEnviarRC = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.btnCorreo = New System.Windows.Forms.Button()
        Me.btnEnviarWb = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnCrearXMLFirmar
        '
        Me.btnCrearXMLFirmar.Location = New System.Drawing.Point(144, 237)
        Me.btnCrearXMLFirmar.Name = "btnCrearXMLFirmar"
        Me.btnCrearXMLFirmar.Size = New System.Drawing.Size(116, 23)
        Me.btnCrearXMLFirmar.TabIndex = 0
        Me.btnCrearXMLFirmar.Text = "Crear y Firmar XML"
        Me.btnCrearXMLFirmar.UseVisualStyleBackColor = True
        '
        'barCodeControl1
        '
        Me.barCodeControl1.Location = New System.Drawing.Point(12, 12)
        Me.barCodeControl1.Name = "barCodeControl1"
        Me.barCodeControl1.Padding = New System.Windows.Forms.Padding(10, 2, 10, 0)
        Me.barCodeControl1.Size = New System.Drawing.Size(113, 23)
        Me.barCodeControl1.Symbology = Code128Generator1
        Me.barCodeControl1.TabIndex = 1
        '
        'btnQR
        '
        Me.btnQR.Location = New System.Drawing.Point(144, 266)
        Me.btnQR.Name = "btnQR"
        Me.btnQR.Size = New System.Drawing.Size(116, 23)
        Me.btnQR.TabIndex = 2
        Me.btnQR.Text = "QR"
        Me.btnQR.UseVisualStyleBackColor = True
        '
        'btnImprimir
        '
        Me.btnImprimir.Location = New System.Drawing.Point(12, 295)
        Me.btnImprimir.Name = "btnImprimir"
        Me.btnImprimir.Size = New System.Drawing.Size(113, 23)
        Me.btnImprimir.TabIndex = 3
        Me.btnImprimir.Text = "Imprimir"
        '
        'btnEnviarRC
        '
        Me.btnEnviarRC.Location = New System.Drawing.Point(235, 12)
        Me.btnEnviarRC.Name = "btnEnviarRC"
        Me.btnEnviarRC.Size = New System.Drawing.Size(184, 23)
        Me.btnEnviarRC.TabIndex = 4
        Me.btnEnviarRC.Text = "Enviar RC"
        Me.btnEnviarRC.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 65)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(407, 20)
        Me.TextBox1.TabIndex = 5
        '
        'btnCorreo
        '
        Me.btnCorreo.Location = New System.Drawing.Point(12, 237)
        Me.btnCorreo.Name = "btnCorreo"
        Me.btnCorreo.Size = New System.Drawing.Size(113, 23)
        Me.btnCorreo.TabIndex = 6
        Me.btnCorreo.Text = "EnviarCorreo"
        Me.btnCorreo.UseVisualStyleBackColor = True
        '
        'btnEnviarWb
        '
        Me.btnEnviarWb.Location = New System.Drawing.Point(12, 266)
        Me.btnEnviarWb.Name = "btnEnviarWb"
        Me.btnEnviarWb.Size = New System.Drawing.Size(113, 23)
        Me.btnEnviarWb.TabIndex = 7
        Me.btnEnviarWb.Text = "subir web"
        Me.btnEnviarWb.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(431, 341)
        Me.Controls.Add(Me.btnEnviarWb)
        Me.Controls.Add(Me.btnCorreo)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.btnEnviarRC)
        Me.Controls.Add(Me.btnImprimir)
        Me.Controls.Add(Me.btnQR)
        Me.Controls.Add(Me.barCodeControl1)
        Me.Controls.Add(Me.btnCrearXMLFirmar)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCrearXMLFirmar As System.Windows.Forms.Button
    Friend WithEvents barCodeControl1 As DevExpress.XtraEditors.BarCodeControl
    Friend WithEvents btnQR As Button
    Friend WithEvents btnImprimir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnEnviarRC As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents btnCorreo As Button
    Friend WithEvents btnEnviarWb As Button
End Class
