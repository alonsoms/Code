#Region "IMPORTS"
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports COE.DATA
Imports COE.FRAMEWORK
Imports COE.REPORT
Imports DevExpress.Printing.Core
Imports DevExpress.Utils
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraReports.UI
#End Region

Public Class ImpresionSE
    Dim ServicioDAO As New ServicioDAO
    Dim Tiempo As New System.Timers.Timer

#Region "Eventos del Servicio"
    Protected Overrides Sub OnStart(ByVal args() As String)


        'Se carga la cultura de Peru en la aplicacion
        Dim MiCultura As New CultureInfo("es-PE", False)

        'Se establece la cultura de peru
        System.Threading.Thread.CurrentThread.CurrentCulture = MiCultura
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

        'Se estable el evento
        AddHandler Tiempo.Elapsed, AddressOf GeneraImpresion

        'Se activa el timer. Se convierte minutos a milisegundos 1 Minutos=60,000 milisegundos
        Tiempo.Interval = 3000 ' 60000 * 5
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

    Public Sub GeneraImpresion()

        Dim FacturaDAO As New FacturaDAO
        Dim BoletaDAO As New BoletaVentaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO
        Dim NotaDebitoDAO As New NotaDebitoDAO
        Dim SistemaDAO As New SistemaDAO
        Dim ServicioDAO As New ServicioDAO
        Dim ComprobanteBE As New Object
        Dim EmisorDAO As New EmisorDAO
        Dim MiReporte As Object = Nothing
        Dim IDTipoComprobante As Int32 = 0
        Dim SerieComprobante As String = String.Empty
        Dim dt As New DataTable

        Try
            'Se detiene el timer
            Tiempo.Stop()

            'Se obtiene los datos del Emisor
            SistemaDAO.EmisorBE = EmisorDAO.GetByID(1)

            'Se obtiene los comprobantes para enviarlo a imprimir
            dt = ServicioDAO.GetByIDServicio(eServicio.ImprimirComprobante)

            'Se explora cada comprobante
            For Each dr As DataRow In dt.Rows
                Try

                    Select Case dr("TipoComprobante").ToString
                        Case "01"
                            ComprobanteBE = FacturaDAO.GetByID(dr("IDComprobante"))
                            MiReporte = New COE.REPORT.FacturaVoucher

                            'Se carga los datos del reporte
                            MiReporte.DataSource = FacturaDAO.GetByReporteID(dr("IDComprobante"))
                            MiReporte.DataMember = "coe_factura_rpt_id"
                            IDTipoComprobante = 1
                            SerieComprobante = ComprobanteBE.t08_numcorrelativo.substring(0, 4)

                        Case "03"
                            ComprobanteBE = BoletaDAO.GetByID(dr("IDComprobante"))
                            MiReporte = New COE.REPORT.BoletaVentaVoucher

                            'Se carga los datos del reporte
                            MiReporte.DataSource = BoletaDAO.GetByReporteID(dr("IDComprobante"))
                            MiReporte.DataMember = "coe_boleta_rpt_id"
                            IDTipoComprobante = 2
                            SerieComprobante = ComprobanteBE.t07_numcorrelativo.substring(0, 4)

                        Case "07"
                            ComprobanteBE = NotaCreditoDAO.GetByID(dr("IDComprobante"))
                            MiReporte = New COE.REPORT.NotaCreditoVoucher

                            'Se carga los datos del reporte
                            MiReporte.DataSource = NotaCreditoDAO.GetByReporteID(dr("IDComprobante"))
                            MiReporte.DataMember = "coe_nota_credito_rpt_id"
                            IDTipoComprobante = 3
                            SerieComprobante = ComprobanteBE.t08_numcorrelativo.substring(0, 4)
                    End Select

                    'Se muestra el reporte
                    Dim printTool As New ReportPrintTool(MiReporte)

                    'Se imprime sin previsualizar
                    printTool.Print(EmisorDAO.GetBySerie(IDTipoComprobante, SerieComprobante))


                    'Se elimina el registro
                    ServicioDAO.Delete(dr("IDServicioComprobante"))
                Catch ex As Exception
                    ServicioDAO.Save(dr("TipoComprobante").ToString, dr("IDComprobante"), eEstadoServicio.Excepcion, eServicio.ImprimirComprobante, "ServicioDAO.GeneraImpresion :" & ex.Message)
                End Try
            Next
        Catch ex As Exception
            Tools.SaveLog("COE SERVICE IMPRESION", ex.Message, EventLogEntryType.Error)
        Finally
            Tiempo.Start()
        End Try
    End Sub

End Class
