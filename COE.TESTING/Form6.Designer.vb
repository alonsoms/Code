<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form6
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
        Me.btnServicioEnvioSunat = New System.Windows.Forms.Button()
        Me.btnValidar = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnServicioEnvioSunat
        '
        Me.btnServicioEnvioSunat.Location = New System.Drawing.Point(12, 12)
        Me.btnServicioEnvioSunat.Name = "btnServicioEnvioSunat"
        Me.btnServicioEnvioSunat.Size = New System.Drawing.Size(152, 23)
        Me.btnServicioEnvioSunat.TabIndex = 0
        Me.btnServicioEnvioSunat.Text = "Servicio Envio Sunat"
        Me.btnServicioEnvioSunat.UseVisualStyleBackColor = True
        '
        'btnValidar
        '
        Me.btnValidar.Location = New System.Drawing.Point(12, 41)
        Me.btnValidar.Name = "btnValidar"
        Me.btnValidar.Size = New System.Drawing.Size(152, 23)
        Me.btnValidar.TabIndex = 1
        Me.btnValidar.Text = "Validar"
        Me.btnValidar.UseVisualStyleBackColor = True
        '
        'Form6
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(422, 311)
        Me.Controls.Add(Me.btnValidar)
        Me.Controls.Add(Me.btnServicioEnvioSunat)
        Me.Name = "Form6"
        Me.Text = "Form6"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnServicioEnvioSunat As Button
    Friend WithEvents btnValidar As Button
End Class
