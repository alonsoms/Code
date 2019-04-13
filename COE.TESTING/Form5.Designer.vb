<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form5
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
        Me.btnEnviarComprobantes = New System.Windows.Forms.Button()
        Me.btnServicioEnvioWeb = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnEnviarComprobantes
        '
        Me.btnEnviarComprobantes.Location = New System.Drawing.Point(12, 12)
        Me.btnEnviarComprobantes.Name = "btnEnviarComprobantes"
        Me.btnEnviarComprobantes.Size = New System.Drawing.Size(214, 23)
        Me.btnEnviarComprobantes.TabIndex = 0
        Me.btnEnviarComprobantes.Text = "Envio de Comprobantes Azure Web"
        Me.btnEnviarComprobantes.UseVisualStyleBackColor = True
        '
        'btnServicioEnvioWeb
        '
        Me.btnServicioEnvioWeb.Location = New System.Drawing.Point(12, 84)
        Me.btnServicioEnvioWeb.Name = "btnServicioEnvioWeb"
        Me.btnServicioEnvioWeb.Size = New System.Drawing.Size(214, 23)
        Me.btnServicioEnvioWeb.TabIndex = 1
        Me.btnServicioEnvioWeb.Text = "Servicio Envio Web"
        Me.btnServicioEnvioWeb.UseVisualStyleBackColor = True
        '
        'Form5
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(385, 295)
        Me.Controls.Add(Me.btnServicioEnvioWeb)
        Me.Controls.Add(Me.btnEnviarComprobantes)
        Me.Name = "Form5"
        Me.Text = "Form5"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnEnviarComprobantes As Button
    Friend WithEvents btnServicioEnvioWeb As Button
End Class
