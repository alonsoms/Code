Imports System.IO
Imports System.ServiceModel
Imports COE.DATA
Imports COE.FRAMEWORK
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraSplashScreen

Public Class ResumenList
    Dim bsResumen As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.Text = "RESUMEN DIARIO"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.btnFechaInicial.EditValue = DateTime.Now.Date.AddDays(-30)
        Me.btnFechaFinal.EditValue = DateTime.Now.Date
        Me.GridControl1.DataSource = bsResumen
        Me.btnFechaEmision.EditValue = DateTime.Now.Date.AddDays(-1)

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.RESUMEN", "idresumen", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.RESUMEN", "numresumen", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TIPO.RESUMEN", "TipoResumen", 180, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.EMISION", "fechaemision", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.RESUMEN", "fecharesumen", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.SUNAT", "estadoenvio", 130, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.TICKET", "ticket", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "OBSERVACION", "observacion", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "RESUMEN XML", "archivoxml", 130, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "SUNAT XML", "rutarespuestasunatxml", 130, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SISTEMA", "fechahorasistemaexterno", 100, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SUNAT", "fechahorasunat", 100, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "fecharegistro", 100, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TERMINAL", "nombrecomputadora", 65, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "USUARIO", "nombreusuario", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.ENVIO", "idestadoenvio", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.RESPUESTA", "codigorespuesta", 0, False)

        'Se configura eventos
        AddHandler GridView1.DoubleClick, AddressOf btnVerComprobante.PerformClick

        'Se carga los registros
        btnActualizar.PerformClick()


    End Sub

    Private Sub AbrirIcono(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Resumen Diario", eMenuFormulario.Open)
    End Sub
    Private Sub CerrarIcono(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Resumen Diario", eMenuFormulario.Close)
    End Sub

    Private Sub ResumenList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub BoletasList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnFirmarComprobante.ItemClick, btnEnviar.ItemClick, btnVerComprobante.ItemClick, btnEnviarTicket.ItemClick, btnBuscar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Crear y Firmar Resumen Diario" : CrearFirmarZipComprobante()
                Case "Enviar Resumen Diario" : Enviar()
                Case "Enviar Ticket" : EnviarTicket()
                Case "Actualizar" : Actualizar()
                Case "Buscar" : Actualizar()
                Case "Cerrar" : Cerrar()
                Case "Ver Resumen Diario" : Ver()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender

        'Se cambia el estilo de fuente
        e.Appearance.Font = New Font(e.Appearance.Font, FontStyle.Regular)

        'Se cambia el color del estado de envio
        If e.Column.FieldName = "estadoenvio" Then

            Dim IDEstadoEnvio As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("idestadoenvio"))
            Select Case IDEstadoEnvio
                Case eEstadoSunat.PendienteEnvio, eEstadoSunat.FirmarXML, eEstadoSunat.EnProceso
                    e.Appearance.ForeColor = Color.Goldenrod
                Case eEstadoSunat.Aceptado
                    e.Appearance.ForeColor = Color.Green
                Case eEstadoSunat.Rechazado
                    e.Appearance.ForeColor = Color.Red
                Case eEstadoSunat.PendienteCDR
                    e.Appearance.ForeColor = Color.Yellow
            End Select
        End If

        'Se cambia el campo de observacion / excepcion
        If e.Column.FieldName = "observacion" Then

            Dim CodigoRespuesta As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("codigorespuesta"))
            Select Case CodigoRespuesta
                Case "0"
                    e.Appearance.ForeColor = Color.Green
                Case Else
                    e.Appearance.ForeColor = Color.Red
            End Select
        End If

    End Sub

    Public Sub CrearFirmarZipComprobante()
        Dim IDResumen As Int32 = 0
        Dim dt As New DataTable
        Dim FecEmision As DateTime = Convert.ToDateTime(btnFechaEmision.EditValue).ToString
        Dim NumComprobantes As Int32 = 0

        Try
            'Se obtiene el numero de comprobantes pendientes de firmar
            NumComprobantes = ResumenDAO.GetByPendientesFirmar(FecEmision).Rows.Count

            'Se valida que el RB no se envia el mismo dia de la fecha del sistema
            If Convert.ToDateTime(btnFechaEmision.EditValue).ToString >= DateTime.Now.Date Then
                MessageBox.Show("El resumen de boletas de ventas no se puede enviar el mismo día de venta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se valida que no existan comprobantes pendientes de firmar
            If NumComprobantes > 0 Then
                MessageBox.Show(String.Format("Existen {0} comprobantes pendientes de firmar, en el día {1}.", NumComprobantes, FecEmision.ToString("dd/MM/yyyy")), "No se puede generar el Resumen de boletas", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se crea el comprobante XML, Se firma y Se comprime
            If MessageBox.Show(String.Format("¿Esta seguro de crear el resumen diario para la fecha {0}?", FecEmision.ToString("dd/MM/yyyy")), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then

                'Se crea el resumen diario
                If ResumenDAO.SaveResumen(btnFechaEmision.EditValue, SistemaDAO.UsuarioBE.IDUsuario, SistemaDAO.NombrePC) Then

                    'Se carga el resumen de boletas pendientes para crear y firmar
                    dt = ResumenDAO.GetResumenBoletasPendientes(btnFechaEmision.EditValue)

                    'Se crea el archivo xml y se guarda
                    For Each dr As DataRow In dt.Rows
                        ResumenDAO.CreateXML(dr("idresumen"))
                        ResumenDAO.SignatureXML(dr("idresumen"))
                        ResumenDAO.ZipXML(dr("idresumen"))
                    Next
                    Actualizar()
                Else
                    MessageBox.Show(String.Format("No se creo el resumen diario para la fecha {0} indicada.", Convert.ToDateTime(btnFechaEmision.EditValue).ToString("dd/MM/yyyy")), "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Enviar()
        Dim ResumenBE As New ResumenBE2018
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDResumen As Int32 = GridView1.GetFocusedRowCellValue("idresumen")
        'Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            ResumenBE = ResumenDAO.GetByID(IDResumen)

            'Se valida que el estado no sea FirmarXML
            If ResumenBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el resumen diario {0}", IDResumen), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el resumen diario {0} a la SUNAT.", IDResumen), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            ResumenDAO.IDResumen = IDResumen

            'Se muestra el formulario de espera
            'SplashScreenManager.ShowWaitForm()
            Tools.WinProcess(Me, False)

            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se agrega las credenciales en el objeto del Behavior
            Dim PB = New PasswordBehavior(EmisorDAO.EmisorBE.SunatUser, EmisorDAO.EmisorBE.SunatPass)
            SunatSE.Endpoint.EndpointBehaviors.Add(PB)

            'Se abre el servicio de la SUNAT
            SunatSE.Open()

            'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
            Dim NumetoTicket As String
            NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ResumenBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ResumenBE.RutaComprobanteZIP))

            'Se guarda el numero de ticket que envia la SUNAT
            ResumenDAO.SaveTicket(NumetoTicket)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDResumen
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDResumen
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()

                'Se oculta el formulario de espera
                'SplashScreenManager.CloseWaitForm()
                Tools.WinProcess(Me, True)
            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub Ver()

        'Se obtiene el ID
        ResumenDAO.IDResumen = GridView1.GetFocusedRowCellValue("idresumen")

        'Se muestra el comprobante
        Dim MiForm As New ResumenDetails
        MiForm.Show()

    End Sub
    Public Sub EnviarTicket()
        Dim ResumenBE As New ResumenBE2018
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDResumen As Int32 = GridView1.GetFocusedRowCellValue("idresumen")

        Try
            'Se obtiene la entidad
            ResumenBE = ResumenDAO.GetByID(IDResumen)

            'Se valida que tenga la firma el comprobante
            If ResumenBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el resumen diario {0}", IDResumen), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se valida que tenga el ticket del RB
            If ResumenBE.Ticket = "" Then
                MessageBox.Show(String.Format("Falta ticket para el Resumen diario {0}", IDResumen), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el ticket del resumen diario {0}?", IDResumen), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If


            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatSE.ClientCredentials.CreateSecurityTokenManager()

            'Se agrega las credenciales en el objeto del Behavior
            Dim PB = New PasswordBehavior(EmisorDAO.EmisorBE.SunatUser, EmisorDAO.EmisorBE.SunatPass)
            SunatSE.Endpoint.EndpointBehaviors.Add(PB)

            'Se abre el servicio de la SUNAT
            SunatSE.Open()

            'Se obtiene la respuesta del envio del ticket
            Dim RespuestaSUNAT As SunatSE.statusResponse
            RespuestaSUNAT = SunatSE.getStatus(ResumenBE.Ticket)

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            If RespuestaSUNAT.content.Length > 0 Then
                ResumenDAO.SaveConstanciaRecepcionZIP(IDResumen, RespuestaSUNAT.content)

                'Se actualiza el estado de las boletas y notas de credito vinculada a boletas como aceptadas
                ResumenDAO.SaveEstadoComprobantes(IDResumen)
            Else
                Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
            End If

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDResumen
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDResumen
                .Descripcion = ex2.Message.ToString
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ResumenDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()
            End If

            'Se actualiza los registros
            Actualizar()
        End Try
    End Sub
    Public Sub Actualizar()
        If DateDiff(DateInterval.Day, Convert.ToDateTime(btnFechaInicial.EditValue), Convert.ToDateTime(btnFechaFinal.EditValue)) > 30 Then
            If MessageBox.Show("El rango de fechas de emisión, supera los 30 días, ¿Desea Continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                bsResumen.DataSource = ResumenDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
            End If
        Else
            'Se establece la fuente de datos del Binding
            bsResumen.DataSource = ResumenDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
        End If
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub


End Class