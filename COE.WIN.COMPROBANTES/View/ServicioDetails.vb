#Region "Imports"
Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.IO
Imports System.Text
#End Region

Public Class ServicioDetails

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Se inicializa el formulario y los controles
        Me.Text = SistemaDAO.NombreAplicacion
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Normal
        'Me.wbRegistroXML.IsWebBrowserContextMenuEnabled = False
        'Me.wbRegistroXML.AllowWebBrowserDrop = False
        'Me.wbRegistroXML.AllowNavigation = False
        'Me.wbRegistroXML.ScriptErrorsSuppressed = False
        'Me.wbComprobanteXML.IsWebBrowserContextMenuEnabled = False
        'Me.wbComprobanteXML.AllowWebBrowserDrop = False
        'Me.wbComprobanteXML.AllowNavigation = False
        'Me.wbComprobanteXML.ScriptErrorsSuppressed = False
        'Me.wbConstanciaRecepcionXML.IsWebBrowserContextMenuEnabled = False
        'Me.wbConstanciaRecepcionXML.AllowWebBrowserDrop = False
        'Me.wbConstanciaRecepcionXML.AllowNavigation = False
        'Me.wbConstanciaRecepcionXML.ScriptErrorsSuppressed = False
        'Me.txtIDBoleta.ReadOnly = True
        'Me.txtNumeroFactura.ReadOnly = True
        'Me.txtNumeroDocumento.ReadOnly = True
        'Me.txtAdquiriente.ReadOnly = True
        'Me.txtFirma.ReadOnly = True
        'Me.txtObservacion.ReadOnly = True
        'Me.txtRutaComprobanteXML.ReadOnly = True
        'Me.txtRutaConstanciaRecepcionXML.ReadOnly = True

        'Se inicializa los controles
        ControlesDevExpress.InitRibbonControl(RibbonControl)


        ControlesDevExpress.InitGridLookUpEdit(cboServicios, ServicioDAO.GetByALLServicio, "IDServicio", "Nombre")


        'Se carga la entidad
        LoadEntidad()

    End Sub
    Private Sub BoletaDetails_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnCerrar.ItemClick, btnGuardar.ItemClick

        Try
            Select Case e.Item.Caption
                Case "Guardar" : Guardar()
                Case "Cerrar" : Cerrar()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub BoletaDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Escape Then
            Cerrar()
        End If

    End Sub

    Public Sub LoadEntidad()

        Try
            'Se obtiene la entidad
            ServicioDAO.GetByID()

            'Se carga la entidad
            With ServicioDAO.BE
                'txtIDServicioComprobante.Text = .IDServicioComprobante
                'txtNumeroFactura.Text = .t07_numcorrelativo
                'txtNumeroDocumento.Text = .t08_numdoc
                'txtAdquiriente.Text = .t09_nomadquiriente
                'txtFirma.Text = .digestvalue
                'txtObservacion.Text = .Observacion
                'txtRutaComprobanteXML.Text = .RutaComprobanteXML
                'txtRutaConstanciaRecepcionXML.Text = .RutaRespuestaSunatXML
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub Guardar()
        
    End Sub
  
    Public Sub Cerrar()
        Me.Close()
    End Sub

End Class