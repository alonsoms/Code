<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form4
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
        Me.btnLeerXML = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnLeerXML
        '
        Me.btnLeerXML.Location = New System.Drawing.Point(12, 12)
        Me.btnLeerXML.Name = "btnLeerXML"
        Me.btnLeerXML.Size = New System.Drawing.Size(176, 23)
        Me.btnLeerXML.TabIndex = 0
        Me.btnLeerXML.Text = "Leer XML"
        Me.btnLeerXML.UseVisualStyleBackColor = True
        '
        'Form4
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(344, 293)
        Me.Controls.Add(Me.btnLeerXML)
        Me.Name = "Form4"
        Me.Text = "Form4"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnLeerXML As Button
End Class
