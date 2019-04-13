<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form7
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
        Me.btnServicioUnificado = New System.Windows.Forms.Button()
        Me.btnEnviarXML = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnServicioUnificado
        '
        Me.btnServicioUnificado.Location = New System.Drawing.Point(13, 13)
        Me.btnServicioUnificado.Name = "btnServicioUnificado"
        Me.btnServicioUnificado.Size = New System.Drawing.Size(183, 23)
        Me.btnServicioUnificado.TabIndex = 0
        Me.btnServicioUnificado.Text = "Servicio Unificado"
        Me.btnServicioUnificado.UseVisualStyleBackColor = True
        '
        'btnEnviarXML
        '
        Me.btnEnviarXML.Location = New System.Drawing.Point(12, 42)
        Me.btnEnviarXML.Name = "btnEnviarXML"
        Me.btnEnviarXML.Size = New System.Drawing.Size(184, 23)
        Me.btnEnviarXML.TabIndex = 1
        Me.btnEnviarXML.Text = "Enviar EmailXML"
        Me.btnEnviarXML.UseVisualStyleBackColor = True
        '
        'Form7
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(465, 340)
        Me.Controls.Add(Me.btnEnviarXML)
        Me.Controls.Add(Me.btnServicioUnificado)
        Me.Name = "Form7"
        Me.Text = "Form7"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnServicioUnificado As Button
    Friend WithEvents btnEnviarXML As Button
End Class
