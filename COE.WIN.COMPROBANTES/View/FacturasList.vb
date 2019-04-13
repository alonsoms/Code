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
Imports DevExpress.XtraGrid.Views.Base

Public Class FacturasList
    Dim bsComprobantes As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.btnFechaInicial.EditValue = DateTime.Now.Date.AddDays(-20)
        Me.btnFechaFinal.EditValue = DateTime.Now.Date
        Me.GridControl1.DataSource = bsComprobantes
        'Me.spsManager.ActiveSplashFormTypeInfo()

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.FACTURA", "idfactura", 80, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.FACTURA", "numcomprobante", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.EMISION", "fechaemision", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.SUNAT", "estadoenvio", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.FACTURA", "estadocomprobante", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TOT.IMPORTE", "totalimporte", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.WEB", "estadoweb", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "RUC/DNI", "numerodocumento", 70, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "CLIENTE", "cliente", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "OBSERVACION", "observacion", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SISTEMA", "fechahorasistemaexterno", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SUNAT", "fechahorasunat", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.WEB", "fechahoraweb", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "fecharegistro", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TERMINAL", "nombrecomputadora", 65, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "USUARIO", "nombreusuario", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.ENVIO", "idestadoenvio", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.FACTURA", "idestadofactura", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.WEB", "idestadoweb", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.RESPUESTA", "codigorespuesta", 0, False)

        'Se configura eventos
        AddHandler GridView1.DoubleClick, AddressOf btnVerComprobante.PerformClick

        'Se carga los registros
        btnActualizar.PerformClick()

    End Sub

    Private Sub FacturasList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub FacturasList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnFirmarComprobante.ItemClick, btnEnviar.ItemClick, btnVerComprobante.ItemClick, btnRecuperar.ItemClick, btnEmailComprobante.ItemClick, btnImprimir.ItemClick, btnEnviarWebComprobante.ItemClick, btnBuscar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Firmar Comprobante" : CrearFirmarZipComprobante()
                Case "Enviar Comprobante" : Enviar()
                Case "Recuperar Constancia CDR" : RecuperarCDR()
                Case "Enviar Email Comprobante" : EnviarEmail()
                Case "Enviar Web Comprobante" : EnviarWeb()
                Case "Imprimir Comprobante" : Imprimir()
                Case "Actualizar" : Actualizar()
                Case "Buscar" : Actualizar()
                Case "Cerrar" : Cerrar()
                Case "Actualizar" : Actualizar()
                Case "Ver Comprobante" : Ver()
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
            End Select
        End If

        'Se cambia el estado de la factura
        If e.Column.FieldName = "estadocomprobante" Then

            Dim IDEstadoFactura As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("idestadofactura"))
            Select Case IDEstadoFactura
                Case eEstadoFactura.Activo
                    e.Appearance.ForeColor = Color.Green
                Case eEstadoFactura.Anulado
                    e.Appearance.ForeColor = Color.Red
            End Select
        End If

        'Se cambia el estado web
        If e.Column.FieldName = "estadoweb" Then

            Dim IDEstadoWeb As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("idestadoweb"))
            Select Case IDEstadoWeb
                Case eEstadoWeb.PendientePublicacion
                    e.Appearance.ForeColor = Color.DarkGoldenrod
                Case eEstadoWeb.Publicado
                    e.Appearance.ForeColor = Color.Green
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
    Private Sub FacturasList_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Facturas", eMenuFormulario.Open)
    End Sub
    Private Sub FacturasList_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Facturas", eMenuFormulario.Close)
    End Sub

    Public Sub CrearFirmarZipComprobante()
        Dim FacturaBE As New FacturaBE
        Dim IDFactura As Int32 = 0

        Try
            'Se obtiene el ID
            IDFactura = GridView1.GetFocusedRowCellValue("idfactura")

            'Se obtiene el comprobante
            FacturaDAO.GetByID(IDFactura)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If FacturaDAO.BE.estado <> eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", FacturaDAO.BE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            'Se crea el comprobante XML, Se firma y Se comprime
            If MessageBox.Show(String.Format("¿Esta seguro de firmar el comprobante {0}?", IDFactura.ToString), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then


                'Se valida el estado sunat del comprobante
                FacturaDAO.CreateFileXML21(IDFactura)
                FacturaDAO.ZipXML(IDFactura)

                'Se obtiene la entidad
                FacturaBE = FacturaDAO.GetByID(IDFactura)

                'Se carga los registros
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Enviar()
        Dim FacturaBE As New FacturaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDFactura As Int32 = GridView1.GetFocusedRowCellValue("idfactura")

        Try

            'Se obtiene el comprobante
            FacturaDAO.GetByID(IDFactura)

            FacturaBE = FacturaDAO.GetByID(IDFactura)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If FacturaDAO.BE.estado <> eEstadoSunat.PendienteEnvio Then
                If FacturaDAO.BE.estado <> eEstadoSunat.EnProceso Then
                    MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", FacturaDAO.BE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el comprobante {0} a la SUNAT.", IDFactura), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
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

            'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
            Dim RespuestaSUNAT As Byte()
            RespuestaSUNAT = SunatSE.sendBill(Path.GetFileName(FacturaBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(FacturaBE.RutaComprobanteZIP))


            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            FacturaDAO.SaveConstanciaRecepcionZIP(IDFactura, RespuestaSUNAT)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDFactura
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            FacturaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDFactura
                .Descripcion = ex2.Message & vbCritical & If(ex2.InnerException.ToString Is Nothing, "", ex2.InnerException.ToString) & vbCritical & If(ex2.InnerException.Message Is Nothing, "", ex2.InnerException.Message)
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            FacturaDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()

                'Se oculta el formulario de espera
                ' SplashScreenManager.CloseWaitForm()
            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub RecuperarCDR()
        Dim FacturaBE As New FacturaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatCDRSE As New SunatCDRSE.billServiceClient
        Dim IDFactura As Int32 = GridView1.GetFocusedRowCellValue("idfactura")
        Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            FacturaBE = FacturaDAO.GetByID(IDFactura)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If FacturaDAO.BE.estado <> eEstadoSunat.Aceptado Then
                MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", FacturaDAO.BE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de recuperar el CDR del comprobante {0}?", IDFactura), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'Se muestra el formulario de espera
            SplashScreenManager.ShowWaitForm()

            'Se configura los parametros de seguridad
            System.Net.ServicePointManager.UseNagleAlgorithm = True
            System.Net.ServicePointManager.Expect100Continue = False
            System.Net.ServicePointManager.CheckCertificateRevocationList = True

            'Se crea la credencial
            SunatCDRSE.ClientCredentials.CreateSecurityTokenManager()

            'Se agrega las credenciales en el objeto del Behavior
            Dim PB = New PasswordBehavior(EmisorDAO.EmisorBE.SunatUser, EmisorDAO.EmisorBE.SunatPass)
            SunatCDRSE.Endpoint.EndpointBehaviors.Add(PB)

            'Se abre el servicio de la SUNAT
            SunatCDRSE.Open()

            'Se obtiene la respuesta del envio del resumen
            Dim RespuestaSUNAT As SunatCDRSE.statusResponse
            RespuestaSUNAT = SunatCDRSE.getStatusCdr(SistemaDAO.EmisorBE.NumeroRUC, FacturaBE.t07_tipdoc_c01, FacturaBE.SerieComprobante, FacturaBE.NumeroComprobante)

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            FacturaDAO.SaveConstanciaRecepcionZIP(IDFactura, RespuestaSUNAT.content)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDFactura
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            FacturaDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDFactura
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            FacturaDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatCDRSE.State = CommunicationState.Opened Then
                SunatCDRSE.Close()

                'Se oculta el formulario de espera
                SplashScreenManager.CloseWaitForm()
            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub EnviarEmail()
        Dim AzureWeb As New AzureWeb
        Dim IDComprobante As Int32 = GridView1.GetFocusedRowCellValue("idfactura")

        Try

            If AzureWeb.EnviarEmail(eTipoComprobante.Factura, IDComprobante, EmisorDAO) Then
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Public Sub EnviarWeb()
        Dim AzureWeb As New AzureWeb
        Dim IDComprobante As Int32 = GridView1.GetFocusedRowCellValue("idfactura")

        Try
            AzureWeb.objForm = Me

            If AzureWeb.EnviarWeb(eTipoComprobante.Factura, IDComprobante, EmisorDAO) Then
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Public Sub Ver()

        'Se obtiene el ID
        FacturaDAO.IDFactura = GridView1.GetFocusedRowCellValue("idfactura")

        'Se muestra el comprobante
        Dim MiForm As New FacturaDetails
        MiForm.Show()

    End Sub
    Public Sub Imprimir()
        Dim FacturaBE As New FacturaBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim IDFactura As Int32 = GridView1.GetFocusedRowCellValue("idfactura")

        Try
            'Se obtiene la entidad
            FacturaDAO.IDFactura = IDFactura

            'Se obtiene la entidad
            FacturaBE = FacturaDAO.GetByID(IDFactura)

            If MessageBox.Show(String.Format("¿Esta seguro de imprimir el comprobante {0}?", IDFactura), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'Se crea la instancia del reporte
            Dim MiReporte As New COE.REPORT.FacturaVoucher

            'Se carga los datos del reporte
            MiReporte.DataSource = FacturaDAO.GetByReporteID(IDFactura)
            MiReporte.DataMember = "coe_factura_rpt_id"

            'Se muestra el reporte
            Dim printTool As New ReportPrintTool(MiReporte)
            'printTool.Print(SistemaDAO.EmisorBE.NombreImpresora)

            'Se imprime sin previsualizar
            'printTool.Print()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Question)
        Finally
            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub Actualizar()
        If DateDiff(DateInterval.Day, Convert.ToDateTime(btnFechaInicial.EditValue), Convert.ToDateTime(btnFechaFinal.EditValue)) > 30 Then
            If MessageBox.Show("El rango de fechas de emisión, supera los 30 días, ¿Desea Continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                bsComprobantes.DataSource = FacturaDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
            End If
        Else
            'Se establece la fuente de datos del Binding
            bsComprobantes.DataSource = FacturaDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
        End If
    End Sub
    Public Sub Cerrar()
        'Se activa el boton de Locales
        Me.Close()
    End Sub


End Class