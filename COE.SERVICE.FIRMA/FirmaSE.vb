Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.Globalization

Public Class FirmaSE
    Dim EmisorDAO As New EmisorDAO
    Dim ServicioDAO As New ServicioDAO
    Dim FacturaDAO As New FacturaDAO
    Dim BoletaDAO As New BoletaVentaDAO
    Dim NotaCreditoDAO As New NotaCreditoDAO
    Dim NotaDebitoDAO As New NotaDebitoDAO
    Dim Tiempo As New System.Timers.Timer

#Region "Eventos del Servicio"
    Protected Overrides Sub OnStart(ByVal args() As String)

        'Se carga la cultura de Peru en la aplicacion
        Dim MiCultura As New CultureInfo("es-PE", False)

        'Se establece la cultura de peru
        System.Threading.Thread.CurrentThread.CurrentCulture = MiCultura
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

        'Se estable el evento
        AddHandler Tiempo.Elapsed, AddressOf ExecuteService

        'Se activa el timer en milisegundos
        'Se multiplica los segundos por 1,000 para obtener milisegundos
        'Se establece a 2 segundos
        Tiempo.Interval = Convert.ToInt32(Tools.ReadAppSettings("FirmarXMLSegundos").ToString) * 1000
        Tiempo.Enabled = True
        Tiempo.Start()

    End Sub
    Protected Overrides Sub OnStop()
        Tiempo.Enabled = False
    End Sub
    Protected Overrides Sub OnContinue()
        Tiempo.Enabled = True
    End Sub
#End Region

    Public Sub ExecuteService()
        Try
            'Se detiene el servicio para ejecutar la tarea
            Tiempo.Stop()

            'Se carga los emisores
            EmisorDAO.GetByConfigXML()

            'Se firma los comprobantes
            FirmarComprobantes()
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service FirmaXML", ex.Message, EventLogEntryType.Error)
        Finally
            Tiempo.Start()
        End Try
    End Sub
    Public Sub FirmarComprobantes()
        Dim TipoComprobante As String = String.Empty
        Dim IDComprobante As Int32
        Dim IDServicioComprobante As Int32
        Dim dt As New DataTable

        Try
            'Se procesa cada emisor de la lista
            For Index = 0 To EmisorDAO.EmisorConfigXML.Count - 1

                'Se establece la cadena de conexion por cada emisor del Config.XML
                ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML(Index).ConexionDB

                'Se obtiene los comprobantes para firmarlos de cada emisor 
                dt = ServicioDAO.GetByIDServicio(eServicio.CrearXMLFirmar)

                'Se crea la firma para cada comprobante
                For Each dr As DataRow In dt.Rows
                    Try
                        IDServicioComprobante = dr("IDServicioComprobante")
                        IDComprobante = dr("IDComprobante")
                        TipoComprobante = dr("TipoComprobante")

                        'Se crea XML y firma compobantes  01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
                        Select Case TipoComprobante
                            Case "01"
                                FacturaDAO.CreateFileXML21(IDComprobante)
                                FacturaDAO.ZipXML(IDComprobante)
                            Case "03"
                                BoletaDAO.CreateFileXML21(IDComprobante)
                                'BoletaDAO.SignatureXML(IDComprobante)
                                BoletaDAO.ZipXML(IDComprobante)
                            Case "07"
                                NotaCreditoDAO.CreateFileXML21(IDComprobante)
                                NotaCreditoDAO.ZipXML(IDComprobante)
                            Case "08"
                                'NotaDebitoDAO.CreateXML(IDComprobante)
                                'NotaDebitoDAO.SignatureXML(IDComprobante)
                                'NotaDebitoDAO.ZipXML(IDComprobante)
                        End Select

                        'Se elimina el registro
                        ServicioDAO.Delete(IDServicioComprobante)
                    Catch ex As Exception
                        ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.CrearXMLFirmar, "ServicioDAO.GeneraFirmaElectronica :" & ex.Message)
                    End Try
                Next
            Next
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service FirmaXML", ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class
