Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports COE.DATA
Imports COE.FRAMEWORK
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob
Public Class Form7
    Dim EmisorDAO As New EmisorDAO
    Dim ServicioDAO As New ServicioDAO

    Private Sub btnServicioUnificado_Click(sender As Object, e As EventArgs) Handles btnServicioUnificado.Click
        Try

            'Se carga la configuracion de cada emisor
            EmisorDAO.GetByConfigXML()

            'Se envia los comprobantes a Azure
            EmisorDAO.EnviarComprobantesAzure()

            'Se envia las alertas al correo electronico
            'EmisorDAO.EnviarEmailAlertas()


        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", "Envio Web y Alertas: " & ex.Message, EventLogEntryType.Error)
        Finally

        End Try
    End Sub

    Private Sub btnEnviarXML_Click(sender As Object, e As EventArgs) Handles btnEnviarXML.Click

        'Se carga los emisores
        EmisorDAO.GetByConfigXML()

        EnviarComprobantesEmail()

    End Sub

    Public Sub EnviarComprobantesEmail()
        Dim FacturaDAO As New FacturaDAO
        Dim BoletaDAO As New BoletaVentaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO
        Dim FacturaBE As New FacturaBE
        Dim NotaCreditoBE As New NotaCreditoBE

        Dim ServicioDAO As New ServicioDAO
        Dim TipoComprobante As String = String.Empty
        Dim IDComprobante As Int32
        Dim ExcepcionBE As New ExcepcionBE

        Dim ComprobanteBE As New Object
        Dim NumComprobante As String = String.Empty
        Dim EmailCliente As String = String.Empty
        Dim RutaXML As String = String.Empty
        Dim RutaPDF As String = String.Empty


        Dim IDServicioComprobante As Int32 = 0
        Dim dt As New DataTable

        Try

            'Se procesa cada emisor de la lista
            For Index = 0 To EmisorDAO.EmisorConfigXML.Count - 1

                'Se establece la cadena de conexion por cada emisor del Config.XML
                ConexionDAO.ConexionDBNet = EmisorDAO.EmisorConfigXML(Index).ConexionDB

                'Se carga el emisor
                EmisorDAO.GetByID(1)

                'Se obtiene los comprobantes para enviarlos a sunat
                dt = ServicioDAO.GetByIDServicio(eServicio.EnviarCorreo)

                If dt.Rows.Count = 0 Then
                    Continue For
                End If


                'Se crea la firma para cada comprobante
                For Each dr As DataRow In dt.Rows

                    Try
                        IDServicioComprobante = dr("IDServicioComprobante")
                        TipoComprobante = dr("TipoComprobante")
                        IDComprobante = dr("IDComprobante")

                        'Se obtiene el comprobante
                        Select Case TipoComprobante
                            Case "01"
                                ComprobanteBE = FacturaDAO.GetByID(IDComprobante)
                                NumComprobante = ComprobanteBE.t08_numcorrelativo
                                EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                                RutaXML = ComprobanteBE.RutaComprobanteXML
                                RutaPDF = ComprobanteBE.RutaComprobantePDF

                            Case "03"
                                ComprobanteBE = BoletaDAO.GetByID(IDComprobante)
                                NumComprobante = ComprobanteBE.t07_numcorrelativo
                                EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                                RutaXML = ComprobanteBE.RutaComprobanteXML
                                RutaPDF = ComprobanteBE.RutaComprobantePDF

                            Case "07"
                                ComprobanteBE = NotaCreditoDAO.GetByID(IDComprobante)
                                NumComprobante = ComprobanteBE.t08_numcorrelativo
                                EmailCliente = ComprobanteBE.EmailAdquiriente.Trim
                                RutaXML = ComprobanteBE.RutaComprobanteXML
                                RutaPDF = ComprobanteBE.RutaComprobantePDF
                        End Select

                        'Se envia el email
                        If Tools.SendEmail(EmisorDAO, EmailCliente, RutaXML, RutaPDF) Then
                            'Se elimina la tarea 
                            ServicioDAO.Delete(IDServicioComprobante)
                        End If

                    Catch ex As Exception
                        ServicioDAO.Save(TipoComprobante, IDServicioComprobante, eEstadoServicio.Excepcion, eServicio.EnviarCorreo, "ServicioDAO.SendXML :" & ex.Message)
                    End Try
                Next

            Next
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio EmailXML: " & ex.Message, EventLogEntryType.Error)
        Finally

        End Try

    End Sub
End Class