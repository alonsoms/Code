<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.btnUsuarioDinamico = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnUsuarioDinamico
        '
        Me.btnUsuarioDinamico.Location = New System.Drawing.Point(12, 12)
        Me.btnUsuarioDinamico.Name = "btnUsuarioDinamico"
        Me.btnUsuarioDinamico.Size = New System.Drawing.Size(132, 23)
        Me.btnUsuarioDinamico.TabIndex = 0
        Me.btnUsuarioDinamico.Text = "Uusuario Dinamico"
        Me.btnUsuarioDinamico.UseVisualStyleBackColor = True
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(536, 370)
        Me.Controls.Add(Me.btnUsuarioDinamico)
        Me.Name = "Form2"
        Me.Text = "Form2"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnUsuarioDinamico As Button
End Class
