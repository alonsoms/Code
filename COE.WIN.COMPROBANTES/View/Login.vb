Imports COE.DATA
Imports COE.FRAMEWORK

Public Class Login
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.Text = Application.ProductName & " " & Application.ProductVersion.ToString
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.txtUsuario.Properties.MaxLength = 15
        Me.txtContrasena.Properties.MaxLength = 15

        'Se ingresa solo letras, numeros y tres caracteres especiales
        txtUsuario.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx
        txtUsuario.Properties.Mask.EditMask = "[a-zA-Z0-9_$@]*"
        txtContrasena.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx
        txtContrasena.Properties.Mask.EditMask = "[a-zA-Z0-9_$@]*"

        Try
            'Se carga combo de emisores
            EmisorDAO.GetByConfigXML()

            ControlesDevExpress.InitGridLookUpEdit(cboEmisores, EmisorDAO.EmisorConfigXML, "RUC", "RazonSocial", 400)
            ControlesDevExpress.InitGridLookUpEditColumns(cboEmisores, "RUC", "RUC", 80)
            ControlesDevExpress.InitGridLookUpEditColumns(cboEmisores, "RAZON SOCIAL", "RazonSocial", 250)


            'Se asocia el evento KeyDown al procedimiento Teclado
            AddHandler Me.KeyDown, AddressOf Tools.Teclado

            'Se carga el usuario, solo si esta guardado en el AppSettings
            If Tools.ReadAppSettings("Usuario").Length > 0 Then
                txtUsuario.Text = Tools.ReadAppSettings("Usuario")
            End If

            If Tools.ReadAppSettings("EmisorRuc").Length > 0 Then
                cboEmisores.EditValue = Tools.ReadAppSettings("EmisorRuc")
            End If

            'Se activa el check
            chkRecordar.Checked = True
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Login_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.Exit()
    End Sub
    Private Sub btnIniciarSesion_Click(sender As Object, e As EventArgs) Handles btnIniciarSesion.Click

        If Validar() Then
            'Se oculta Formulario login
            Me.Hide()

            'Se guarda el usuario en el AppSettings, solo si el check esta activo
            If chkRecordar.Checked Then
                Tools.SaveAppSettings("Usuario", txtUsuario.Text.Trim)
                Tools.SaveAppSettings("EmisorRuc", cboEmisores.EditValue)
            Else
                Tools.SaveAppSettings("Usuario", "")
                Tools.SaveAppSettings("EmisorRuc", "")
            End If

            'Se carga la pantalla principal del programa
            DesktopMain.Show()
        End If
    End Sub
    Private Sub txtContrasena_KeyDown(sender As Object, e As KeyEventArgs) Handles txtContrasena.KeyDown
        If e.KeyCode = Keys.Enter Then
            If txtContrasena.Text.Trim.Length > 0 Then
                btnIniciarSesion.PerformClick()
            End If
        End If
    End Sub
    Private Function Validar() As Boolean
        Dim Result As Boolean = False
        Static NumIntentosLogin As Int16 = 0

        'Se valida los campos obligatorios
        If Tools.CampoObligatorio(cboEmisores, eTipoControl.GridLoopUpEdit) Then
            If Tools.CampoObligatorio(txtUsuario) Then
                If txtContrasena.Text = "" Then
                    txtContrasena.Text = txtUsuario.Text
                End If
                If Tools.CampoObligatorio(txtContrasena) Then
                    Result = True
                End If
            End If
        End If

        If Result Then

            'Se establece la cadena de conexion por cada emisor del Config.XML
            ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML.Find(Function(Item) Item.RUC = cboEmisores.EditValue).ConexionDB

            'Se obtiene los datos del Emisor
            EmisorDAO.GetByID()

            'Se establece la conexion a la base de datos de Azure
            ConexionDAO.ConexionDBCloud = "Server=tcp:ceserver.database.windows.net,1433;Initial Catalog=cedata;Persist Security Info=False;User ID=ceadmin2557;Password=Digit@l2557;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

            'Se carga los datos del usuario y sistema
            SistemaDAO.UsuarioBE = UsuarioDAO.GetByLoginPassword(txtUsuario.Text, txtContrasena.Text)
            SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)
            SistemaDAO.NombrePC = My.Computer.Name
            SistemaDAO.NombreAplicacion = Application.ProductName & " " & Application.ProductVersion

            If NumIntentosLogin > 3 Then
                MessageBox.Show("Ha superado el numero de intentos de validación del usuario.Contacte con el área de sistemas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Application.Exit()
            End If

            If SistemaDAO.UsuarioBE.IDUsuario = 0 Then
                NumIntentosLogin += 1
                txtUsuario.ErrorIconAlignment = ErrorIconAlignment.MiddleRight
                txtUsuario.ErrorText = "El nombre de usuario y contraseña no existe, intente con otro."
                txtContrasena.Text = ""
                txtUsuario.Focus()
                Result = False
            End If

        End If

        Return Result
    End Function


End Class