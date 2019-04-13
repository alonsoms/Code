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

Public Class NotaCreditoList
    Dim bsComprobantes As New BindingSource

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y controles
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.btnFechaInicial.EditValue = DateTime.Now.Date.AddDays(-15)
        Me.btnFechaFinal.EditValue = DateTime.Now.Date
        Me.GridControl1.DataSource = bsComprobantes
        'Me.spsManager.ActiveSplashFormTypeInfo()

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.NOTA.CREDITO", "idnotacredito", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.NOTA.CREDITO", "numcomprobante", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.COM.AFECTADO", "numcomprobanteafectado", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.EMISION", "fechaemision", 80, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.SUNAT", "estadoenvio", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.NOTA.CREDITO", "estadocomprobante", 80, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.WEB", "estadoweb", 60, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "RUC/DNI", "numerodocumento", 70, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "CLIENTE", "cliente", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TOT.IMPORTE", "totalimporte", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "OBSERVACION", "observacion", 120, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SISTEMA", "fechahorasistemaexterno", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SUNAT", "fechahorasunat", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "fecharegistro", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.WEB", "fechahoraweb", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TERMINAL", "nombrecomputadora", 65, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "USUARIO", "nombreusuario", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.ENVIO", "idestadoenvio", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.FACTURA", "idestadofactura", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.RESPUESTA", "codigorespuesta", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.WEB", "idestadoweb", 0, False)
        'Se configura eventos
        AddHandler GridView1.DoubleClick, AddressOf btnVerComprobante.PerformClick

        'Se carga los registros
        Actualizar()

    End Sub

    Private Sub AbrirIcono(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Notas de Credito", eMenuFormulario.Open)
    End Sub
    Private Sub CerrarIcono(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Notas de Credito", eMenuFormulario.Close)
    End Sub
    Private Sub NotaCreditoList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub NotaCreditoList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnFirmarComprobante.ItemClick, btnEnviar.ItemClick, btnVerComprobante.ItemClick, btnRecuperar.ItemClick, btnEmailComprobante.ItemClick, btnImprimir.ItemClick, btnEnviarComprobanteWeb.ItemClick, btnBuscar.ItemClick
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

    Public Sub CrearFirmarZipComprobante()
        Dim NotaCreditoBE As New NotaCreditoBE
        Dim IDNotaCredito As Int32 = 0

        Try
            'Se obtiene el ID
            IDNotaCredito = GridView1.GetFocusedRowCellValue("idnotacredito")

            'Se obtiene la entidad
            NotaCreditoBE = NotaCreditoDAO.GetByID(IDNotaCredito)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If NotaCreditoBE.estado <> eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", NotaCreditoBE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If


            'Se crea el comprobante XML, Se firma y Se comprime
            If MessageBox.Show(String.Format("¿Esta seguro de firmar el comprobante {0}?", IDNotaCredito.ToString), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then

                NotaCreditoDAO.CreateFileXML21(IDNotaCredito)
                NotaCreditoDAO.ZipXML(IDNotaCredito)

                'Se obtiene la entidad
                NotaCreditoBE = NotaCreditoDAO.GetByID(IDNotaCredito)

                ''Se crea la instancia del reporte
                'Dim MiReporte As New COE.REPORT.NotaCreditoVoucher

                ''Se carga los datos del reporte
                'MiReporte.DataSource = NotaCreditoDAO.GetByReporteID(IDNotaCredito)
                'MiReporte.DataMember = "coe_nota_credito_rpt_id"

                ''Se exporta en formato PDF
                'MiReporte.ExportToPdf(NotaCreditoBE.RutaComprobantePDF)

                'Se carga los registros
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Enviar()
        Dim NotaCreditoBE As New NotaCreditoBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDNotaCredito As Int32 = GridView1.GetFocusedRowCellValue("idnotacredito")


        Try
            'Se obtiene la entidad
            NotaCreditoBE = NotaCreditoDAO.GetByID(IDNotaCredito)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If NotaCreditoBE.estado <> eEstadoSunat.PendienteEnvio Then
                If NotaCreditoBE.estado <> eEstadoSunat.EnProceso Then
                    MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", NotaCreditoBE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

            End If

            'Se envia solo Notas de credito asociadas a Facturas
            If NotaCreditoBE.t33_tipdoc_c01 <> "01" Then
                MessageBox.Show(String.Format("La nota de credito {0} vinculada a una boleta de venta no se envia.", NotaCreditoBE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If


            If MessageBox.Show(String.Format("¿Esta seguro de enviar el comprobante {0} a la SUNAT.", NotaCreditoBE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

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
            Dim RespuestaSUNAT As Byte()
            RespuestaSUNAT = SunatSE.sendBill(Path.GetFileName(NotaCreditoBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(NotaCreditoBE.RutaComprobanteZIP))

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            NotaCreditoDAO.SaveConstanciaRecepcionZIP(IDNotaCredito, RespuestaSUNAT)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDNotaCredito
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaCreditoDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDNotaCredito
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaCreditoDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatSE.State = CommunicationState.Opened Then
                SunatSE.Close()

            End If


            Tools.WinProcess(Me, True)

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub RecuperarCDR()
        Dim NotaCreditoBE As New NotaCreditoBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatCDRSE As New SunatCDRSE.billServiceClient
        Dim IDNotaCredito As Int32 = GridView1.GetFocusedRowCellValue("idnotacredito")

        Try
            'Se obtiene la entidad
            NotaCreditoBE = NotaCreditoDAO.GetByID(IDNotaCredito)

            'Se valida que el comprobante tenga el estado adecuado para este proceso
            If NotaCreditoBE.estado <> eEstadoSunat.Aceptado Then
                MessageBox.Show(String.Format("El comprobante {0} no tiene el estado adecuado para este proceso.", NotaCreditoBE.t08_numcorrelativo), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If


            If MessageBox.Show(String.Format("¿Esta seguro de recuperar el CDR del comprobante {0}?", IDNotaCredito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If


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
            RespuestaSUNAT = SunatCDRSE.getStatusCdr(SistemaDAO.EmisorBE.NumeroRUC, "07", NotaCreditoBE.SerieComprobante, NotaCreditoBE.NumeroComprobante)

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            NotaCreditoDAO.SaveConstanciaRecepcionZIP(IDNotaCredito, RespuestaSUNAT.content)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDNotaCredito
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaCreditoDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDNotaCredito
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaCreditoDAO.SaveExcepcion(ExcepcionBE)

        Finally

            'Se cierra la conexion del servicio
            If SunatCDRSE.State = CommunicationState.Opened Then
                SunatCDRSE.Close()

            End If

            'Se actualiza los registros
            Actualizar()
        End Try

    End Sub
    Public Sub EnviarEmail()
        Dim AzureWeb As New AzureWeb
        Dim IDComprobante As Int32 = GridView1.GetFocusedRowCellValue("idnotacredito")

        Try
            If AzureWeb.EnviarEmail(eTipoComprobante.NotaCredito, IDComprobante, EmisorDAO) Then
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub EnviarWeb()
        Dim AzureWeb As New AzureWeb
        Dim IDComprobante As Int32 = GridView1.GetFocusedRowCellValue("idnotacredito")

        Try
            If AzureWeb.EnviarWeb(eTipoComprobante.NotaCredito, IDComprobante, EmisorDAO) Then
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Ver()

        'Se obtiene el ID
        NotaCreditoDAO.IDNotaCredito = GridView1.GetFocusedRowCellValue("idnotacredito")

        'Se muestra el comprobante
        Dim MiForm As New NotaCreditoDetails
        MiForm.Show()

    End Sub
    Public Sub Imprimir()
        Dim NotaCreditoBE As New NotaCreditoBE
        Dim IDNotaCredito As Int32 = GridView1.GetFocusedRowCellValue("idnotacredito")

        Try
            'Se obtiene la entidad
            NotaCreditoDAO.IDNotaCredito = IDNotaCredito

            'Se obtiene la entidad
            NotaCreditoBE = NotaCreditoDAO.GetByID(IDNotaCredito)

            If MessageBox.Show(String.Format("¿Esta seguro de imprimir el comprobante {0}?", IDNotaCredito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'Se crea la instancia del reporte
            Dim MiReporte As New COE.REPORT.NotaCreditoVoucher

            'Se carga los datos del reporte
            MiReporte.DataSource = NotaCreditoDAO.GetByReporteID(IDNotaCredito)
            MiReporte.DataMember = "coe_nota_credito_rpt_id"

            'Se muestra el reporte
            Dim printTool As New ReportPrintTool(MiReporte)
            ' printTool.Print(SistemaDAO.EmisorBE.NombreImpresora)

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
                bsComprobantes.DataSource = NotaCreditoDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
            End If
        Else
            'Se establece la fuente de datos del Binding
            bsComprobantes.DataSource = NotaCreditoDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
        End If
    End Sub
    Public Sub Cerrar()
        'Se activa el boton de Locales
        Me.Close()
    End Sub

End Class