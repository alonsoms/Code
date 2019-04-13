Imports System.Data.SqlClient
Imports COE.FRAMEWORK

Public Enum eServicio
    CrearXMLFirmar = 1
    EnviarSunat = 2
    EnviarWeb = 3
    EnviarCorreo = 4
    ImprimirComprobante = 5
    Alertas = 6
End Enum
Public Enum eEstadoServicio
    Pendiente = 1
    EnProceso = 2
    Excepcion = 3
    Satifastorio=4
End Enum

Public Class ServicioDAO
    Public Property BE As New ServicioBE

    'Dim FacturaDAO As New FacturaDAO
    'Dim BoletaDAO As New BoletaVentaDAO
    'Dim NotaCreditoDAO As New NotaCreditoDAO
    'Dim NotaDebitoDAO As New NotaDebitoDAO

    Public Function Save(TipoComprobante As String, IDComprobante As Int32, IDEstado As eEstadoServicio, IDServicio As eServicio, Descripcion As String) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_update"
            .Parameters.Add("@tipocomprobante", SqlDbType.Char, 2).Value = TipoComprobante
            .Parameters.Add("@idcomprobante", SqlDbType.Int).Value = IDComprobante
            .Parameters.Add("@idestado", SqlDbType.Int).Value = IDEstado
            .Parameters.Add("@idservicio", SqlDbType.Int).Value = IDServicio
            .Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value = Descripcion
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function
    Public Function Save() As Int32
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Int32 = 0

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_update"
            .Parameters.Add("@IDServicioComprobante", SqlDbType.Int).Value = BE.IDServicioComprobante

            With .Parameters
                .Add("@IDServicio", SqlDbType.Int).Value = Me.BE.IDServicio
                .Add("@IDEstado", SqlDbType.Int).Value = Me.BE.IDEstado
                .Add("@TipoComprobante", SqlDbType.Char, 2).Value = Me.BE.TipoComprobante
                .Add("@IDComprobante", SqlDbType.Int).Value = Me.BE.IDComprobante
                .Add("@Descripcion", SqlDbType.VarChar, 250).Value = Me.BE.Descripcion
                .Add("@FechaRegistro", SqlDbType.DateTime).Value = Me.BE.FechaRegistro
            End With
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                Me.BE.IDServicioComprobante = cmd.Parameters("@IDServicioComprobante").Value
            End If
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function
    Public Function Delete(IDServicioComprobante As Int32) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_delete"
            .Parameters.Add("@IDServicioComprobante", SqlDbType.Int).Value = IDServicioComprobante
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery > 0 Then
                Result = True
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result

    End Function

    Public Function GetByID() As ServicioBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim BE As New ServicioBE

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_getid"
            With .Parameters
                .Add("@IDServicioComprobante", SqlDbType.Int).Value = Me.BE.IDServicioComprobante
            End With
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read()
                    With BE
                        .IDServicioComprobante = dr.ReadNullAsEmptyString("IDServicioComprobante")
                        .IDServicio = dr.ReadNullAsEmptyString("IDServicio")
                        .IDEstado = dr.ReadNullAsEmptyString("IDEstado")
                        .TipoComprobante = dr.ReadNullAsEmptyString("TipoComprobante")
                        .IDComprobante = dr.ReadNullAsEmptyString("IDComprobante")
                        .Descripcion = dr.ReadNullAsEmptyString("Descripcion")
                        .FechaRegistro = dr.ReadNullAsEmptyString("FechaRegistro")
                    End With

                End While
                dr.Close()
            End If
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return BE
    End Function
    Public Function GetByIDServicio(IDServicio As Int32) As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_get_all"
            .Parameters.Add("@IDServicio", SqlDbType.Int).Value = IDServicio
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
    Public Function GetByALL() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_comprobante_get_all2"
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
    Public Function GetByALLServicio() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_servicio_get_all"
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function

    'Public Sub CrearXMLFirmar()
    '    Dim TipoComprobante As String = String.Empty
    '    Dim IDComprobante As Int32
    '    Dim IDServicioComprobante As Int32
    '    Dim dt As New DataTable
    '    Dim ComprobanteBE As New Object

    '    'Se obtiene los comprobantes para firmarlos 01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
    '    dt = Me.GetByIDServicio(eServicio.CrearXMLFirmar)

    '    'Se crea la firma para cada comprobante
    '    For Each dr As DataRow In dt.Rows

    '        Try
    '            IDServicioComprobante = dr("IDServicioComprobante")
    '            TipoComprobante = dr("TipoComprobante")
    '            IDComprobante = dr("IDComprobante")

    '            'Se crea XML y firma compobantes  01=Factura, 03=Boleta Venta, 07=Nota de Credito, 08=Nota de Debito
    '            Select Case TipoComprobante
    '                Case "01"
    '                    FacturaDAO.CreateXML(IDComprobante)
    '                    FacturaDAO.SignatureXML(IDComprobante)
    '                    FacturaDAO.ZipXML(IDComprobante)

    '                    'Se obtiene la entidad
    '                    ComprobanteBE = FacturaDAO.GetByID(IDComprobante)

    '                    'Se crea la instancia del reporte
    '                    Dim MiReporte As New COE.REPORT.FacturaVoucher

    '                    'Se carga los datos del reporte
    '                    MiReporte.DataSource = FacturaDAO.GetByReporteID(IDComprobante)
    '                    MiReporte.DataMember = "coe_factura_rpt_id"

    '                    'Se exporta en formato PDF
    '                    MiReporte.ExportToPdf(ComprobanteBE.RutaComprobantePDF)
    '                Case "03"
    '                    BoletaDAO.CreateXML(IDComprobante)
    '                    BoletaDAO.SignatureXML(IDComprobante)
    '                    BoletaDAO.ZipXML(IDComprobante)

    '                    'Se obtiene la entidad
    '                    ComprobanteBE = BoletaDAO.GetByID(IDComprobante)

    '                    'Se crea la instancia del reporte
    '                    Dim MiReporte As New COE.REPORT.BoletaVentaVoucher

    '                    'Se carga los datos del reporte
    '                    MiReporte.DataSource = BoletaDAO.GetByReporteID(IDComprobante)
    '                    MiReporte.DataMember = "coe_boleta_get_id_rpt"

    '                    'Se exporta en formato PDF
    '                    MiReporte.ExportToPdf(ComprobanteBE.RutaComprobantePDF)
    '                Case "07"
    '                    NotaCreditoDAO.CreateXML(IDComprobante)
    '                    NotaCreditoDAO.SignatureXML(IDComprobante)
    '                    NotaCreditoDAO.ZipXML(IDComprobante)
    '                Case "08"
    '                    NotaDebitoDAO.CreateXML(IDComprobante)
    '                    NotaDebitoDAO.SignatureXML(IDComprobante)
    '                    NotaDebitoDAO.ZipXML(IDComprobante)
    '            End Select

    '            'Se elimina el registro
    '            Me.Delete(IDServicioComprobante)
    '        Catch ex As Exception
    '            Me.Save(TipoComprobante, IDComprobante, eEstadoServicio.Excepcion, eServicio.CrearXMLFirmar, "ServicioDAO.CrearXMLFirmar :" & ex.Message)
    '        End Try
    '    Next
    'End Sub
End Class
