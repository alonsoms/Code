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

#End Region

Public Class NotaDebitoList
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
        ' Me.spsManager.ActiveSplashFormTypeInfo()

        'Se inicializa el Ribbon
        ControlesDevExpress.InitRibbonControl(RibbonControl)

        'Se configura el control GridControl
        ControlesDevExpress.InitGridControl(GridControl1)


        'Se desactiva los botones hasta reviasr lo envios de las notas de debito
        Me.btnEnviar.Enabled = False
        Me.btnFirmarComprobante.Enabled = False
        Me.btnRecuperar.Enabled = False
        Me.btnEnviar.Enabled = False
        Me.btnEmailComprobante.Enabled = False

        'Se configura el control GridView
        ControlesDevExpress.InitGridView(GridView1)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.NOTA.DEBITO", "idnotadebito", 110, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.NOTA.DEBITO", "numcomprobante", 110, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "NUM.COM.AFECTADO", "numcomprobanteafectado", 110, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.EMISION", "fechaemision", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.SUNAT", "estadoenvio", 110, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "EST.NOTA.DEBITO", "estadocomprobante", 110, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ESTA.WEB", "estadoweb", 70, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "RUC/DNI", "numerodocumento", 70, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "CLIENTE", "cliente", 150, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TOT.IMPORTE", "totalimporte", 100, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "OBSERVACION", "observacion", 130, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SISTEMA", "fechahorasistemaexterno", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.SUNAT", "fechahorasunat", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.REGISTRO", "fecharegistro", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "FEC.WEB", "fechahoraweb", 90, True, ControlesDevExpress.eGridViewFormato.FechaHora)
        ControlesDevExpress.InitGridViewColumn(GridView1, "TERMINAL", "nombrecomputadora", 65, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "USUARIO", "nombreusuario", 90, True)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.NOTA.DEBITO", "idestadonotadebito", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "ID.ESTADO.ENVIO", "idestadoenvio", 0, False)
        ControlesDevExpress.InitGridViewColumn(GridView1, "COD.RESPUESTA", "codigorespuesta", 0, False)
        'Se configura eventos
        AddHandler GridView1.DoubleClick, AddressOf btnVerComprobante.PerformClick

        'Se carga los registros
        Actualizar()

    End Sub

    Private Sub AbrirIcono(sender As Object, e As EventArgs) Handles Me.Activated
        DesktopMain.MenuBar("Notas de Debito", eMenuFormulario.Open)
    End Sub
    Private Sub CerrarIcono(sender As Object, e As EventArgs) Handles Me.Deactivate
        DesktopMain.MenuBar("Notas de Debito", eMenuFormulario.Close)
    End Sub
    Private Sub FacturasList_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If
    End Sub
    Private Sub FacturasList_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnActualizar.ItemClick, btnFirmarComprobante.ItemClick, btnEnviar.ItemClick, btnVerComprobante.ItemClick, btnRecuperar.ItemClick, btnEmailComprobante.ItemClick, btnImprimir.ItemClick, btnBuscar.ItemClick
        Try
            Select Case e.Item.Caption
                Case "Firmar Comprobante" : CrearFirmarZipComprobante()
                Case "Enviar Comprobante" : Enviar()
                Case "Recuperar Constancia CDR" : RecuperarCDR()
                Case "Enviar Email Comprobante" : EnviarEmail()
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

            Dim IDEstadoFactura As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("idestadonotadebito"))
            Select Case IDEstadoFactura
                Case eEstadoFactura.Activo
                    e.Appearance.ForeColor = Color.Green
                Case eEstadoFactura.Anulado
                    e.Appearance.ForeColor = Color.Red
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
        Dim NotaDebitoBE As New NotaDebitoBE
        Dim IDNotaDebito As Int32 = 0

        Try
            'Se obtiene el ID
            IDNotaDebito = GridView1.GetFocusedRowCellValue("idnotadebito")

            'Se crea el comprobante XML, Se firma y Se comprime
            If MessageBox.Show(String.Format("¿Esta seguro de firmar el comprobante {0}?", IDNotaDebito.ToString), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.Yes Then
                'NotaDebitoDAO.CreateXML(IDNotaDebito)
                'NotaDebitoDAO.SignatureXML(IDNotaDebito)

                NotaDebitoDAO.CreateFileXMLV21(IDNotaDebito)
                NotaDebitoDAO.ZipXML(IDNotaDebito)

                'Se obtiene la entidad
                NotaDebitoBE = NotaDebitoDAO.GetByID(IDNotaDebito)

                'Se crea la instancia del reporte
                ' Dim MiReporte As New COE.REPORT.FacturaVoucher

                ''Se carga los datos del reporte
                'MiReporte.DataSource = FacturaDAO.GetByReporteID
                'MiReporte.DataMember = "coe_notadebito_rpt_id"

                'Se exporta en formato PDF
                ' MiReporte.ExportToPdf(NotaDebitoBE.RutaComprobantePDF)

                'Se carga los registros
                Actualizar()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Enviar()
        Dim NotaDebitoBE As New NotaDebitoBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatSE As New SunatSE.billServiceClient
        Dim IDNotaDebito As Int32 = GridView1.GetFocusedRowCellValue("idnotadebito")
        Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            NotaDebitoBE = NotaDebitoDAO.GetByID(IDNotaDebito)

            'Se valida que el estado no sea FirmarXML
            If NotaDebitoBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el comprobante {0}", IDNotaDebito), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el comprobante {0} a la SUNAT.", IDNotaDebito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
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
            Dim RespuestaSUNAT As Byte()
            RespuestaSUNAT = SunatSE.sendBill(Path.GetFileName(NotaDebitoBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(NotaDebitoBE.RutaComprobanteZIP))

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            NotaDebitoDAO.SaveConstanciaRecepcionZIP(IDNotaDebito, RespuestaSUNAT)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDNotaDebito
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaDebitoDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDNotaDebito
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaDebitoDAO.SaveExcepcion(ExcepcionBE)

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
    Public Sub RecuperarCDR()
        Dim NotaDebitoBE As New NotaDebitoBE
        Dim ExcepcionBE As New ExcepcionBE
        Dim SunatCDRSE As New SunatCDRSE.billServiceClient
        Dim IDNotaDebito As Int32 = GridView1.GetFocusedRowCellValue("idnotadebito")
        Dim SplashScreenManager As New DevExpress.XtraSplashScreen.SplashScreenManager(Me, GetType(Global.COE.FRAMEWORK.WaitForm), True, True)

        Try
            'Se obtiene la entidad
            NotaDebitoBE = NotaDebitoDAO.GetByID(IDNotaDebito)

            'Se valida que tenga la firma el comprobante
            If NotaDebitoBE.estado = eEstadoSunat.FirmarXML Then
                MessageBox.Show(String.Format("Falta firmar el comprobante {0}", IDNotaDebito), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de recuperar el CDR del comprobante {0}?", IDNotaDebito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
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

            'Se abre el servicio de la SUNAT
            SunatCDRSE.Open()

            'Se obtiene la respuesta del envio del resumen
            Dim RespuestaSUNAT As SunatCDRSE.statusResponse
            RespuestaSUNAT = SunatCDRSE.getStatusCdr(SistemaDAO.EmisorBE.NumeroRUC, "08", NotaDebitoBE.SerieComprobante, NotaDebitoBE.NumeroComprobante)

            'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
            FacturaDAO.SaveConstanciaRecepcionZIP(IDNotaDebito, RespuestaSUNAT.content)

        Catch ex1 As FaultException
            'Se guarda la excepcion de SUNAT
            With ExcepcionBE
                .IDComprobante = IDNotaDebito
                .Descripcion = ex1.Message
                .CodigoExcepcion = ex1.Code.Name.ToString
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaDebitoDAO.SaveExcepcion(ExcepcionBE)

        Catch ex2 As Exception
            'Se guarda la excepcion del CLIENTE
            With ExcepcionBE
                .IDComprobante = IDNotaDebito
                .Descripcion = ex2.Message & vbCritical & ex2.InnerException.ToString & vbCritical & ex2.InnerException.Message
                .CodigoExcepcion = "9999"
                .IDEstado = eEstadoSunat.EnProceso
                .FechaHora = DateTime.Now
            End With
            NotaDebitoDAO.SaveExcepcion(ExcepcionBE)

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
        Dim NotaDebitoBE As New NotaDebitoBE

        Try
            'Se obtiene la entidad
            NotaDebitoDAO.IDNotaDebito = GridView1.GetFocusedRowCellValue("idnotadebito")
            NotaDebitoBE = NotaDebitoDAO.GetByID()

            'Se valida que el estado no sea FirmarXML
            If NotaDebitoBE.estado <> eEstadoSunat.Aceptado Then
                MessageBox.Show(String.Format("El comprobante {0} no esta aceptado por la SUNAT", NotaDebitoDAO.IDNotaDebito), "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If MessageBox.Show(String.Format("¿Esta seguro de enviar el comprobante via email {0} al cliente?", NotaDebitoDAO.IDNotaDebito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            Dim Mail As New MailMessage()
            Dim SmtpServer As New SmtpClient()
            Dim EmailFuente As String = SistemaDAO.EmisorBE.CorreoEnvio
            Dim EmailFuenteContrasena As String = SistemaDAO.EmisorBE.CorreoContrasena
            Dim EmailBody As String = SistemaDAO.EmisorBE.CorreoMensaje
            Dim EmailAsunto As String = SistemaDAO.EmisorBE.CorreoAsunto
            Dim ServidorHostURL As String = SistemaDAO.EmisorBE.ServidorHost
            Dim ServidorHostPuerto As String = SistemaDAO.EmisorBE.ServidorPuerto

            'Se configura para servidor de GMail
            SmtpServer.Credentials = New Net.NetworkCredential(EmailFuente, EmailFuenteContrasena)
            SmtpServer.Port = ServidorHostPuerto
            SmtpServer.Host = ServidorHostURL
            SmtpServer.EnableSsl = True

            'Se configura el Correo
            Mail = New MailMessage()
            Mail.From = New MailAddress(EmailFuente, SistemaDAO.EmisorBE.NombreComercial, System.Text.Encoding.UTF8)
            Mail.To.Add(NotaDebitoBE.EmailAdquiriente)
            Mail.Subject = EmailAsunto
            Mail.Body = EmailBody
            Mail.Attachments.Add(New Attachment(NotaDebitoBE.RutaComprobanteXML))
            Mail.Attachments.Add(New Attachment(NotaDebitoBE.RutaComprobantePDF))
            Mail.IsBodyHtml = True

            Mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure

            'Se envia el correo
            SmtpServer.Send(Mail)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Public Sub Ver()

        'Se obtiene el ID
        NotaDebitoDAO.IDNotaDebito = GridView1.GetFocusedRowCellValue("idnotadebito")

        'Se muestra el comprobante
        Dim MiForm As New NotaDebitoDetails
        MiForm.Show()

    End Sub
    Public Sub Imprimir()
        Dim NotaDebitoBE As New NotaDebitoBE
        Dim ExcepcionBE As New ExcepcionBE

        Try
            'Se obtiene el ID de Nota de Debito
            NotaDebitoDAO.IDNotaDebito = GridView1.GetFocusedRowCellValue("idnotadebito")

            'Se obtiene la entidad
            NotaDebitoBE = NotaDebitoDAO.GetByID()

            If MessageBox.Show(String.Format("¿Esta seguro de imprimir el comprobante {0}?", NotaDebitoDAO.IDNotaDebito), "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = System.Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            ''Se crea la instancia del reporte
            'Dim MiReporte As New COE.REPORT.NotaDebitoVoucher

            ''Se carga los datos del reporte
            'MiReporte.DataSource = NotaDebitoDAO.GetByReporteID
            'MiReporte.DataMember = "coe_nota_debito_rpt_id"

            ''Se exporta en formato PDF
            'MiReporte.ExportToPdf(NotaDebitoBE.RutaComprobantePDF)

            ''Se muestra el reporte
            'Dim printTool As New ReportPrintTool(MiReporte)
            'printTool.ShowPreview()

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
                bsComprobantes.DataSource = NotaDebitoDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
            End If
        Else
            'Se establece la fuente de datos del Binding
            bsComprobantes.DataSource = NotaDebitoDAO.GetByAll(btnFechaInicial.EditValue, btnFechaFinal.EditValue)
        End If
    End Sub
    Public Sub Cerrar()
        'Se activa el boton de Locales
        Me.Close()
    End Sub

End Class