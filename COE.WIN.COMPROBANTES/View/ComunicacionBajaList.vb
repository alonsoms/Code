#Region "Imports"
Imports COE.DATA
Imports COE.FRAMEWORK
Imports COE.REPORT
Imports DevExpress.XtraGrid.Views.Grid
Imports System.IO
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports DevExpress.Utils
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraSplashScreen
Imports System.Net.Mail
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Auth
Imports Microsoft.WindowsAzure.Storage.Blob
Imports System.Configuration
#End Region

Public Class ComunicacionBajaList
    Dim bsComunicacion As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.Text = "COMUNICACION DE BAJA"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.btnFechaInicial.EditValue = DateTime.Now.Date.AddDays(-15)
        Me.btnFechaFinal.EditValue = DateTime.Now.Date
        Me.GridControl1.DataSource = bsComunicacion
        Me.btnFechaEmision.EditValue = DateTime.Now.Date.AddDays(-1)

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.COMUNICACION", "idcomunicacion", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.COMUNICACION", "t09_numcorrelativo", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.EMISION", "t03_fecemisiondoc", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.COMUNICACION", "t10_feccomunicacion", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.SUNAT", "estadoenvio", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.TICKET", "ticket", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "OBSERVACION/EXCEPCION", "observacion", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COMUNICACION XML", "archivoxml", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "SUNAT XML", "rutarespuestasunatxml", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SUNAT", "fechahorasunat", 100, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "fecharegistro", 100, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TERMINAL", "nombrecomputadora", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "USUARIO", "nombreusuario", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.ENVIO", "idestadoenvio", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.RESPUESTA", "codigorespuesta", 0, False)

        'Se configura eventos
        AddHandler GridView1.DoubleClick, AddressOf btnVerComprobante.PerformClick

        'Se carga los registros
        Actualizar()

    End Sub

    Private Sub AbrirIcono(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Comunicación de Baja", eMenuFormulario.Open)
    End Sub
    Private Sub CerrarIcono(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Comunicación de Baja", eMenuFormulario.Close)
    End Sub

    Private Sub ComunicacionBajaList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub ComunicacionBajaList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnFirmarComprobante.ItemClick, btnEnviar.ItemClick, btnVerComprobante.ItemClick, btnEnviarTicket.ItemClick, btnBuscar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Crear y Firmar Comunicación de Baja" : CrearFirmarZipComprobante()
                Case "Enviar Comunicación" : Enviar()
                Case "Enviar Ticket" : EnviarTicket()
                Case "Actualizar" : Actualizar()
                Case "Buscar" : Actualizar()
                Case "Ver Comunicación" : Ver()
                Case "Cerrar" : Cerrar()
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
                Case eEstadoSunat.ControlInterno
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
        Dim IDComunicacion As Int32 = 0

        'Se valida que la fecha de emision no se igual o mayor a la fecha del sistema
        If btnFechaEmision.EditValue >= DateTime.Now.Date Then
            MessageBox.Show("La fecha de emisión debe ser menor a la fecha actual del sistema.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show(String.Format("¿Esta seguro de crear la comunicación de baja para la fecha {0}?", Convert.ToDateTime(btnFechaEmision.EditValue).ToString("dd/MM/yyyy")), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then

            'Se valida la fecha de emision, solo debe existir una
            If ValidacionDAO.Validar(ValidacionDAO.eValidar.ComunicacionBajaFechaUnica, Convert.ToDateTime(btnFechaEmision.EditValue).ToString("yyyy-MM-dd")) Then
                MessageBox.Show("La fecha de comunicación de baja existe, intente con otro.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se valida que exista facturas, boletas y notas de credito que sean anuladas para reportar en la comunicacion de baja
            'If Not ValidacionDAO.Validar(ValidacionDAO.eValidar.ComunicacionBajaExistaComprobantesAnulados, Convert.ToDateTime(btnFechaEmision.EditValue).ToString("yyyy-MM-dd")) Then
            '    MessageBox.Show("No existe comprobantes para reportar en la comunicación de baja.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Exit Sub
            'End If

            'Se crea la comunicacion de baja del dia
            IDComunicacion = ComunicacionBajaDAO.SaveComunicacionBajaXML(btnFechaEmision.EditValue, SistemaDAO.UsuarioBE.IDUsuario, SistemaDAO.NombrePC)

            'Se crea el XML, Se firma y Se empaqueta
            ComunicacionBajaDAO.CreateXML(IDComunicacion)
            ComunicacionBajaDAO.SignatureXML(IDComunicacion)
            ComunicacionBajaDAO.ZipXML(IDComunicacion)

            Actualizar()
        End If
    End Sub
    Public Sub Enviar()
        Dim ComunicacionBajaBE As New ComunicacionBajaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDComunicacion As Int32 = GridView1.GetFocusedRowCellValue("idcomunicacion")
        Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(IDComunicacion)

            'Se valida que el estado no sea FirmarXML
            If ComunicacionBajaBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el comprobante {0}", IDComunicacion), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se valida el estado de control interno
            If ComunicacionBajaBE.estado = eEstadoSunat.ControlInterno Then
                MessageBox.Show(String.Format("La comunicación {0} es de control interno.No se envía a la SUNAT.", IDComunicacion), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar la comunicación de baja  {0} a la SUNAT.", IDComunicacion), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'Se muestra el formulario de espera
            SplashScreenManager.ShowWaitForm()

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
            NumetoTicket = SunatSE.sendSummary(Path.GetFileName(ComunicacionBajaBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(ComunicacionBajaBE.RutaComprobanteZIP))

            'Se guarda el numero de ticket que envia la SUNAT
            ComunicacionBajaDAO.SaveTicket(NumetoTicket)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDComunicacion
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDComunicacion
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Finally
            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()

                'Se oculta el formulario de espera
                SplashScreenManager.CloseWaitForm()
            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub EnviarTicket()
        Dim ComunicacionBajaBE As New ComunicacionBajaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim RespuestaSUNAT As SunatSE.statusResponse = Nothing
        Dim IDComunicacion As Int32 = GridView1.GetFocusedRowCellValue("idcomunicacion")
        Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            ComunicacionBajaBE = ComunicacionBajaDAO.GetByID(IDComunicacion)

            'Se valida que tenga la firma el comprobante
            If ComunicacionBajaBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el comprobante {0}", IDComunicacion), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            'Se valida el estado de control interno
            If ComunicacionBajaBE.estado = eEstadoSunat.ControlInterno Then
                MessageBox.Show(String.Format("La comunicación {0} es de control interno.No se envía a la SUNAT.", IDComunicacion), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el ticket de la comunicación de baja {0}?", IDComunicacion), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'Se muestra el formulario de espera
            SplashScreenManager.ShowWaitForm()

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
            RespuestaSUNAT = SunatSE.getStatus(ComunicacionBajaBE.NumeroTicket)

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            If RespuestaSUNAT.content.Length > 0 Then
                ComunicacionBajaDAO.SaveConstanciaRecepcionZIP(IDComunicacion, RespuestaSUNAT.content)
            Else
                Throw New FaultException(RespuestaSUNAT.statusCode.ToString & " StatusCode: En proceso. No hay archivo de respuesta")
            End If

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDComunicacion
                .Descripcion = ex1.Message.ToString
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDComunicacion
                .Descripcion = ex2.Message.ToString
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            ComunicacionBajaDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()

                'Se oculta el formulario de espera
                SplashScreenManager.CloseWaitForm()
            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub Ver()

        'Se obtiene el ID
        ComunicacionBajaDAO.IDComunicacion = GridView1.GetFocusedRowCellValue("idcomunicacion")

        'Se muestra el comprobante
        Dim MiForm As New ComunicacionBajaDetails
        MiForm.Show()

    End Sub
    Public Sub Actualizar()
        If DateDiff(DateInterval.Day, Convert.ToDateTime(btnFechaInicial.EditValue), Convert.ToDateTime(btnFechaFinal.EditValue)) > 30 Then
            If MessageBox.Show("El rango de fechas de emisión, supera los 30 días, ¿Desea Continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                'Se establece la fuente de datos del Binding
                bsComunicacion.DataSource = ComunicacionBajaDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
            End If
        Else
            'Se establece la fuente de datos del Binding
            bsComunicacion.DataSource = ComunicacionBajaDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
        End If
    End Sub
    Public Sub Cerrar()
        Me.Close()
    End Sub


End Class