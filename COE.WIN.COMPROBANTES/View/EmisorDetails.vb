Imports COE.DATA
Imports COE.FRAMEWORK

Public Class EmisorDetails
    Dim EmisorBE As New EmisorBE
    Dim bsImpresoras As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa los controles
        Me.Text = "EMISOR"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Normal
        Me.GridControl1.DataSource = bsImpresoras

        'Se inicializa los controles DevExpress
        ControlesDevExpress.InitRibbonControl(RibbonControl)
        ControlesDevExpress.TextEditFormat(txtIDEmisor, ControlesDevExpress.eTextEditFormat.Ninguno, 10, True)
        ControlesDevExpress.TextEditFormat(txtFechaRegistro, ControlesDevExpress.eTextEditFormat.Ninguno, 10, True)
        ControlesDevExpress.TextEditFormat(txtNumeroRUC, ControlesDevExpress.eTextEditFormat.Fixed, 11)
        ControlesDevExpress.TextEditFormat(txtCodigoUbigeo, ControlesDevExpress.eTextEditFormat.Fixed, 6)
        ControlesDevExpress.TextEditFormat(txtRazonSocial, ControlesDevExpress.eTextEditFormat.Ninguno, 250)
        ControlesDevExpress.TextEditFormat(txtNombreComercial, ControlesDevExpress.eTextEditFormat.Ninguno, 250)
        ControlesDevExpress.TextEditFormat(txtNombreDepartamento, ControlesDevExpress.eTextEditFormat.Ninguno, 100)
        ControlesDevExpress.TextEditFormat(txtNombreProvincia, ControlesDevExpress.eTextEditFormat.Ninguno, 100)
        ControlesDevExpress.TextEditFormat(txtNombreDistrito, ControlesDevExpress.eTextEditFormat.Ninguno, 100)
        ControlesDevExpress.TextEditFormat(txtNombreUrbanizacion, ControlesDevExpress.eTextEditFormat.Ninguno, 100)
        ControlesDevExpress.TextEditFormat(txtDireccion, ControlesDevExpress.eTextEditFormat.Ninguno, 250)
        ControlesDevExpress.TextEditFormat(txtRutaCarpetaArchivosXML, ControlesDevExpress.eTextEditFormat.Ninguno, 500)
        ControlesDevExpress.TextEditFormat(txtRutaCarpetaArchivosPDF, ControlesDevExpress.eTextEditFormat.Ninguno, 500)
        ControlesDevExpress.TextEditFormat(txtRutaCarpetaCertificados, ControlesDevExpress.eTextEditFormat.Ninguno, 500)
        ControlesDevExpress.TextEditFormat(txtClaveCertificado, ControlesDevExpress.eTextEditFormat.Ninguno, 5000)
        ControlesDevExpress.TextEditFormat(txtResolucion, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtServidorHost, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtServidorPuerto, ControlesDevExpress.eTextEditFormat.Ninguno, 10)
        ControlesDevExpress.TextEditFormat(txtCorreoEnvio, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtCorreoContrasena, ControlesDevExpress.eTextEditFormat.Ninguno, 20)
        ControlesDevExpress.TextEditFormat(txtCorreoAsunto, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtCorreoAlertas, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtCodLocal, ControlesDevExpress.eTextEditFormat.Ninguno, 150)
        ControlesDevExpress.TextEditFormat(txtCorreoMensaje, ControlesDevExpress.eTextEditFormat.Ninguno, 4000)
        ControlesDevExpress.TextEditFormat(txtImpresora, ControlesDevExpress.eTextEditFormat.Ninguno, 250)
        ControlesDevExpress.TextEditFormat(txtSerieComprobante, ControlesDevExpress.eTextEditFormat.Ninguno, 10)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.IMPRESORA", "IDImpresora", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TIPO", "NombreTipoComprobante", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "SERIE", "SerieComprobante", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "IMPRESORA", "NombreImpresora", 250, True)

        'Se carga el estado del documento
        ControlesDevExpress.InitGridLookUpEdit(cboTipoComprobantes, ComprobanteDAO.GetByALL, "IDTipoComprobante", "Nombre")
        ControlesDevExpress.InitGridLookUpEditColumns(cboTipoComprobantes, "Tipo", "Nombre", 100)

        'Se configura los eventos
        AddHandler Me.KeyDown, AddressOf Tools.Teclado

        'Se carga la entidad
        LoadEntidad()

    End Sub
    Private Sub EmisorDetails_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnGuardar.ItemClick, btnGuardarCerrar.ItemClick

        Try
            Select Case e.Item.Caption
                Case "Guardar" : Guardar()
                Case "Guardar y Cerrar" : GuardarCerrar()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub EmisorDetailsList_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Emisor", eMenuFormulario.Open)
    End Sub
    Private Sub EmisorDetailsList_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Emisor", eMenuFormulario.Close)
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Dim ImpresoraBE As New EmisorBE.ImpresoraBE
        Dim Result As Boolean = False

        'Se valida los campos
        If Tools.CampoObligatorio(cboTipoComprobantes, eTipoControl.GridLoopUpEdit) Then
            If Tools.CampoObligatorio(txtSerieComprobante, eTipoControl.TextEdit) Then
                If Tools.CampoObligatorio(txtImpresora, eTipoControl.TextEdit) Then
                    Result = True
                End If
            End If
        End If

        If Not Result Then
            Exit Sub
        End If

        Try
            With ImpresoraBE
                .IDImpresora = 0
                .IDTipoComprobante = cboTipoComprobantes.EditValue
                .SerieComprobante = txtSerieComprobante.Text
                .NombreImpresora = txtImpresora.Text
            End With
            If MessageBox.Show("¿Esta seguro de agregar impresora?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                EmisorDAO.SaveImpresora(ImpresoraBE)
                LoadEntidad()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub btnQuitar_Click(sender As Object, e As EventArgs) Handles btnQuitar.Click
        Dim IDImpresora As Int32 = 0

        Try
            'Se valida que exista filas
            If GridView1.RowCount = 0 Then
                Exit Sub
            End If

            'Se obtiene el IDImpresora
            IDImpresora = GridView1.GetFocusedRowCellValue("IDImpresora")

            'Se crea el comprobante XML, Se firma y Se comprime
            If MessageBox.Show(String.Format("¿Esta seguro de quitar la impresora {0}?", IDImpresora.ToString), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then

                EmisorDAO.DeleteImpresora(IDImpresora)
                LoadEntidad()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub LoadEntidad()

        Try
            'Se obtiene la entidad
            EmisorBE = EmisorDAO.GetByID(1)

            With EmisorBE
                txtIDEmisor.Text = .IDEmisor
                txtNumeroRUC.Text = .NumeroRUC
                txtRazonSocial.Text = .RazonSocial
                txtNombreComercial.Text = .NombreComercial
                txtCodigoUbigeo.Text = .CodigoUbigeo
                txtNombreDepartamento.Text = .NombreDepartamento
                txtNombreProvincia.Text = .NombreProvincia
                txtNombreDistrito.Text = .NombreDistrito
                txtNombreUrbanizacion.Text = .NombreUrbanizacion
                txtDireccion.Text = .NombreDireccion
                txtRutaCarpetaArchivosXML.Text = .RutaCarpetaArchivosXML
                txtRutaCarpetaArchivosPDF.Text = .RutaCarpetaArchivosPDF
                txtRutaCarpetaCertificados.Text = .RutaCarpetaArchivosCertificados
                txtClaveCertificado.Text = .ClaveCertificado
                txtResolucion.Text = .Resolucion
                txtServidorHost.Text = .ServidorHost
                txtServidorPuerto.Text = .ServidorPuerto
                txtCorreoEnvio.Text = .CorreoEnvio
                txtCorreoContrasena.Text = .CorreoContrasena
                txtCorreoAsunto.Text = .CorreoAsunto
                txtCorreoMensaje.Text = .CorreoMensaje
                txtCorreoAlertas.Text = .CorreoAlertas
                txtCodLocal.Text = .CodigoLocal
                txtFechaEnvio.Text = .FechaEnvioResumenComunicacion.ToString
                txtFechaRegistro.Text = .FechaRegistro
                txtSunatUser.Text = .SunatUser
                txtSunatPass.Text = .SunatPass
            End With

            bsImpresoras.DataSource = EmisorBE.Impresoras

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Public Sub Guardar()
        Validar()
        If EmisorBE.ReglasValidacion Then
            EmisorDAO.Save(EmisorBE)
        End If
    End Sub
    Public Sub GuardarCerrar()
        Validar()
        If EmisorBE.ReglasValidacion Then
            EmisorDAO.Save(EmisorBE)
            'Se carga los datos
            SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)
            Cerrar()
        End If
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub
    Public Sub Validar()
        EmisorBE.ReglasValidacion = False

        'Se valida los campos obligatorios
        If Tools.CampoObligatorio(txtNumeroRUC, eTipoControl.TextEdit) Then
            If Tools.CampoObligatorio(txtRazonSocial, eTipoControl.TextEdit) Then
                If Tools.CampoObligatorio(txtNombreComercial, eTipoControl.TextEdit) Then
                    If Tools.CampoObligatorio(txtCodigoUbigeo, eTipoControl.TextEdit) Then
                        If Tools.CampoObligatorio(txtNombreDepartamento, eTipoControl.TextEdit) Then
                            If Tools.CampoObligatorio(txtNombreProvincia, eTipoControl.TextEdit) Then
                                If Tools.CampoObligatorio(txtNombreDistrito, eTipoControl.TextEdit) Then
                                    If Tools.CampoObligatorio(txtDireccion, eTipoControl.TextEdit) Then
                                        If Tools.CampoObligatorio(txtRutaCarpetaArchivosXML, eTipoControl.TextEdit) Then
                                            If Tools.CampoObligatorio(txtRutaCarpetaCertificados, eTipoControl.TextEdit) Then
                                                If Tools.CampoObligatorio(txtClaveCertificado, eTipoControl.TextEdit) Then
                                                    If Tools.CampoObligatorio(txtResolucion, eTipoControl.TextEdit) Then
                                                        If Tools.CampoObligatorio(txtRutaCarpetaArchivosXML, eTipoControl.TextEdit, eTipoValidacion.ExisteCarpeta) Then
                                                            If Tools.CampoObligatorio(txtRutaCarpetaArchivosPDF, eTipoControl.TextEdit, eTipoValidacion.ExisteCarpeta) Then
                                                                If Tools.CampoObligatorio(txtRutaCarpetaCertificados, eTipoControl.TextEdit, eTipoValidacion.ExisteArchivo) Then
                                                                    If Tools.CampoObligatorio(txtServidorHost, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                        If Tools.CampoObligatorio(txtServidorPuerto, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                            If Tools.CampoObligatorio(txtCorreoEnvio, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                If Tools.CampoObligatorio(txtCorreoContrasena, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                    If Tools.CampoObligatorio(txtCorreoAsunto, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                        If Tools.CampoObligatorio(txtCorreoMensaje, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                            If Tools.CampoObligatorio(txtCorreoAlertas, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                                If Tools.CampoObligatorio(txtCodLocal, eTipoControl.TextEdit, eTipoValidacion.ValorObligatorio) Then
                                                                                                    EmisorBE.ReglasValidacion = True
                                                                                                End If
                                                                                            End If
                                                                                        End If
                                                                                    End If
                                                                                End If
                                                                            End If
                                                                        End If
                                                                    End If

                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

        'Se carga la entidad, siempre que cumpla con todas las reglas de validación
        If EmisorBE.ReglasValidacion Then
            With EmisorBE
                .IDEmisor = txtIDEmisor.Text
                .NumeroRUC = txtNumeroRUC.Text
                .RazonSocial = txtRazonSocial.Text
                .NombreComercial = txtNombreComercial.Text
                .CodigoUbigeo = txtCodigoUbigeo.Text
                .NombreDepartamento = txtNombreDepartamento.Text
                .NombreProvincia = txtNombreProvincia.Text
                .NombreDistrito = txtNombreDistrito.Text
                .NombreUrbanizacion = txtNombreUrbanizacion.Text
                .NombreDireccion = txtDireccion.Text
                .RutaCarpetaArchivosXML = txtRutaCarpetaArchivosXML.Text
                .RutaCarpetaArchivosPDF = txtRutaCarpetaArchivosPDF.Text
                .RutaCarpetaArchivosCertificados = txtRutaCarpetaCertificados.Text
                .ClaveCertificado = txtClaveCertificado.Text
                .Resolucion = txtResolucion.Text
                .ServidorHost = txtServidorHost.Text
                .ServidorPuerto = txtServidorPuerto.Text
                .CorreoEnvio = txtCorreoEnvio.Text
                .CorreoContrasena = txtCorreoContrasena.Text
                .CorreoAsunto = txtCorreoAsunto.Text
                .CorreoMensaje = txtCorreoMensaje.Text
                .CorreoAlertas = txtCorreoAlertas.Text
                .CodigoLocal = txtCodLocal.Text
                .FechaEnvioResumenComunicacion = txtFechaEnvio.Text
                .FechaRegistro = DateTime.Now
                .ReglasValidacion = True
                .SunatUser = txtSunatUser.Text
                .SunatPass = txtSunatPass.Text
            End With
        Else
            MessageBox.Show("El registro de datos no cumple con las reglas de validación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

End Class