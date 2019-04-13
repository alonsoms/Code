Imports System.IO
Imports System.ServiceModel
Imports COE.DATA
Imports COE.FRAMEWORK
Imports System.Globalization

Public Class Form6
    Dim EmisorDAO As New EmisorDAO

    Private Sub btnServicioEnvioSunat_Click(sender As Object, e As EventArgs) Handles btnServicioEnvioSunat.Click
        ExecuteService()
    End Sub

    Public Sub ExecuteService()
        Try
            'Se detiene el servicio para ejecutar la tarea
            'Tiempo.Stop()

            'Se carga los emisores
            EmisorDAO.GetByConfigXML()

            'Se envia los comprobantes a Sunat
            EnviarComprobantesSUNAT()
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", "Envio Sunat: " & ex.Message, EventLogEntryType.Error)
        Finally
            'Tiempo.Start()
        End Try
    End Sub

    Public Sub EnviarComprobantesSUNAT()
        Dim FacturaDAO As New FacturaDAO
        Dim NotaCreditoDAO As New NotaCreditoDAO
        Dim FacturaBE As New FacturaBE
        Dim NotaCreditoBE As New NotaCreditoBE

        Dim ServicioDAO As New ServicioDAO
        Dim TipoComprobante As String = String.Empty
        Dim IDComprobante As Int32
        Dim ExcepcionBE As New ExcepcionBE


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
                dt = ServicioDAO.GetByIDServicio(eServicio.EnviarSunat)

                If dt.Rows.Count = 0 Then
                    Continue For
                End If

                'Se configura los parametros de seguridad
                System.Net.ServicePointManager.UseNagleAlgorithm = True
                System.Net.ServicePointManager.Expect100Continue = False
                System.Net.ServicePointManager.CheckCertificateRevocationList = True

                'Se crea la credencial
                Dim SunatSE As New SunatSE.billServiceClient
                SunatSE.ClientCredentials.CreateSecurityTokenManager()

                'Se agrega las credenciales en el objeto del Behavior
                Dim PB = New PasswordBehavior(EmisorDAO.EmisorBE.SunatUser, EmisorDAO.EmisorBE.SunatPass)
                SunatSE.Endpoint.EndpointBehaviors.Add(PB)

                'Se abre el servicio de la SUNAT
                SunatSE.Open()

                'Se crea la firma para cada comprobante
                For Each dr As DataRow In dt.Rows

                    Try
                        IDServicioComprobante = dr("IDServicioComprobante")
                        TipoComprobante = dr("TipoComprobante")
                        IDComprobante = dr("IDComprobante")

                        'Se crea XML y firma compobantes  01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
                        Select Case TipoComprobante
                            Case "01"
                                FacturaBE = FacturaDAO.GetByID(IDComprobante)
                            Case "07"
                                NotaCreditoBE = NotaCreditoDAO.GetByID(IDComprobante)
                        End Select

                        'Se pasa como parametros solo el nombre del archivo ZIP y el contenido del archivo zip. No se debe pasar la ruta del archivo
                        Dim RespuestaSUNAT As Byte()

                        'Se envia el comprobante a la SUNAT
                        'Se guarda la respuesta de la sunat en formato ZIP, Se descomprime, Se lee el contenido XML y se guarda el estado en el comprobante
                        Select Case TipoComprobante
                            Case "01"
                                RespuestaSUNAT = SunatSE.sendBill(Path.GetFileName(FacturaBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(FacturaBE.RutaComprobanteZIP), "")
                                FacturaDAO.SaveConstanciaRecepcionZIP(IDComprobante, RespuestaSUNAT)
                            Case "07"
                                RespuestaSUNAT = SunatSE.sendBill(Path.GetFileName(NotaCreditoBE.RutaComprobanteZIP), My.Computer.FileSystem.ReadAllBytes(NotaCreditoBE.RutaComprobanteZIP), "")
                                NotaCreditoDAO.SaveConstanciaRecepcionZIP(IDComprobante, RespuestaSUNAT)
                        End Select

                        'Se elimina la tarea del envio
                        ServicioDAO.Delete(IDServicioComprobante)

                    Catch ex1 As FaultException
                        'Se guarda la excepcion de SUNAT
                        With ExcepcionBE
                            .IDComprobante = IDComprobante
                            .Descripcion = ex1.Message
                            .CodigoExcepcion = ex1.Code.Name.ToString
                            .IDEstado = eEstadoSunat.EnProceso
                            .FechaHora = DateTime.Now
                        End With

                        Select Case TipoComprobante
                            Case "01" : FacturaDAO.SaveExcepcion(ExcepcionBE)
                            Case "07" : NotaCreditoDAO.SaveExcepcion(ExcepcionBE)
                        End Select
                        ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.EnviarSunat, "ServicioDAO.EnviarSunat :" & ex1.Message)
                    Catch ex2 As Exception
                        'Se guarda la excepcion del CLIENTE
                        With ExcepcionBE
                            .IDComprobante = IDComprobante
                            .Descripcion = ex2.Message
                            .CodigoExcepcion = "9999"
                            .IDEstado = eEstadoSunat.EnProceso
                            .FechaHora = DateTime.Now
                        End With

                        Select Case TipoComprobante
                            Case "01" : FacturaDAO.SaveExcepcion(ExcepcionBE)
                            Case "07" : NotaCreditoDAO.SaveExcepcion(ExcepcionBE)
                        End Select
                        ServicioDAO.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.EnviarSunat, "ServicioDAO.EnviarSunat :" & ex2.Message)
                    End Try
                Next
                'Se cierra la conexion del servicio
                If SunatSE.State = CommunicationState.Opened Then
                    SunatSE.Close()
                End If
            Next
        Catch ex As Exception
            Tools.SaveLog("DigitalPro Service", " Envio Sunat: " & ex.Message, EventLogEntryType.Error)
        Finally

            ' Tiempo.Start()
        End Try

    End Sub

    Private Sub btnValidar_Click(sender As Object, e As EventArgs) Handles btnValidar.Click
        Dim ValidarSE As New Validar.billValidServiceClient


    End Sub
End Class