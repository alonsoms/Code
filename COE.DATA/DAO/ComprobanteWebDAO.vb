Imports System.Data.SqlClient 
Imports System.IO

Public Class ComprobanteWebDAO
    Public Property IDComprobanteWeb As Int32 = 0

    Public Function Save(NumeroRuc As String, TipoComprobante As String, ComprobanteBE As Object, CodLocal As String) As Int32
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantesWeb)
        Dim cmd As New SqlCommand
        Dim Result As Boolean = False
        Dim BE As New ComprobanteWebBE
        Dim RutaAzure As String = "https://macroscem.blob.core.windows.net/"


        'Se establece el codigo de local, como parte de la ruta
        RutaAzure = RutaAzure & CodLocal & "/"

        'Se copia los datos de Facturas en Comprobante Web
        Select Case TipoComprobante
            Case "01" 'Factura
                With BE
                    .NumeroRUC = NumeroRuc
                    .IDComprobante = ComprobanteBE.idfactura
                    .NumeroCorrelativo = ComprobanteBE.t08_numcorrelativo
                    .Tipo = TipoComprobante
                    .Importe = ComprobanteBE.t27_totalimporte
                    .FechaEmision = ComprobanteBE.t01_fecemision
                    .RutaArchivoPDF = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobantePDF)
                    .RutaArchivoXML = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobanteXML)
                    .FechaRegistro = DateTime.Now
                    .CodLocal = CodLocal
                End With

            Case "03" 'Boleta de Venta
                With BE
                    .NumeroRUC = NumeroRuc
                    .IDComprobante = ComprobanteBE.idboleta
                    .NumeroCorrelativo = ComprobanteBE.t07_numcorrelativo
                    .Tipo = TipoComprobante
                    .Importe = ComprobanteBE.t23_totalimporte
                    .FechaEmision = ComprobanteBE.t01_fecemision
                    .RutaArchivoPDF = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobantePDF)
                    .RutaArchivoXML = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobanteXML)
                    .FechaRegistro = DateTime.Now
                    .CodLocal = CodLocal
                End With

            Case "07" 'Nota de Credito
                With BE
                    .NumeroRUC = NumeroRuc
                    .IDComprobante = ComprobanteBE.idnotacredito
                    .NumeroCorrelativo = ComprobanteBE.t08_numcorrelativo
                    .Tipo = TipoComprobante
                    .Importe = ComprobanteBE.t29_totalimporte
                    .FechaEmision = ComprobanteBE.t01_fecemision
                    .RutaArchivoPDF = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobantePDF)
                    .RutaArchivoXML = RutaAzure & Path.GetFileName(ComprobanteBE.RutaComprobanteXML)
                    .FechaRegistro = DateTime.Now
                    .CodLocal = CodLocal
                End With
        End Select

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_web_ins"

            With .Parameters
                .Add("@IDComprobanteWeb", SqlDbType.Int).Direction = ParameterDirection.Output
                .Add("@IDComprobante", SqlDbType.Int).Value = BE.IDComprobante
                .Add("@Tipo", SqlDbType.Char, 2).Value = BE.Tipo
                .Add("@NumeroRUC", SqlDbType.VarChar, 15).Value = BE.NumeroRUC
                .Add("@NumeroCorrelativo", SqlDbType.VarChar, 20).Value = BE.NumeroCorrelativo
                .Add("@FechaEmision", SqlDbType.VarChar, 10).Value = BE.FechaEmision
                .Add("@Importe", SqlDbType.Decimal).Value = BE.Importe
                .Add("@RutaArchivoPDF", SqlDbType.VarChar, 500).Value = BE.RutaArchivoPDF
                .Add("@RutaArchivoXML", SqlDbType.VarChar, 500).Value = BE.RutaArchivoXML
                .Add("@FechaRegistro", SqlDbType.DateTime).Value = BE.FechaRegistro
                .Add("@CodLocal", SqlDbType.VarChar, 10).Value = BE.CodLocal
            End With
        End With

        Try
            cnx.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                IDComprobanteWeb = cmd.Parameters("@IDComprobanteWeb").Value
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
    Public Function GetByNumero(BE As ComprobanteWebBE) As ComprobanteWebBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantesWeb)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing

        'Se establece formato de fecha
        BE.FechaEmision = Convert.ToDateTime(BE.FechaEmision).ToString("yyyy-MM-dd")

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_web_get_numero"

            With .Parameters
                .Add("@NumeroRuc", SqlDbType.VarChar, 15).Value = BE.NumeroRUC
                .Add("@Tipo", SqlDbType.Char, 2).Value = BE.Tipo
                .Add("@NumeroCorrelativo", SqlDbType.VarChar, 20).Value = BE.NumeroCorrelativo
                .Add("@FechaEmision", SqlDbType.VarChar, 10).Value = BE.FechaEmision
                .Add("@Importe", SqlDbType.Decimal).Value = BE.Importe
            End With
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read()
                    With BE
                        .IDComprobanteWeb = dr.ReadNullAsEmptyString("IDComprobanteWeb")
                        .IDComprobante = dr.ReadNullAsEmptyString("IDComprobante")
                        .Tipo = dr.ReadNullAsEmptyString("Tipo")
                        .NumeroRUC = dr.ReadNullAsEmptyString("NumeroRUC")
                        .NumeroCorrelativo = dr.ReadNullAsEmptyString("NumeroCorrelativo")
                        .FechaEmision = dr.ReadNullAsEmptyString("FechaEmision")
                        .Importe = dr.ReadNullAsNumeric("Importe")
                        .RutaArchivoPDF = dr.ReadNullAsEmptyString("RutaArchivoPDF")
                        .RutaArchivoXML = dr.ReadNullAsEmptyString("RutaArchivoXML")
                        .FechaRegistro = dr.ReadNullAsEmptyDate("FechaRegistro")
                        .CodLocal = dr.ReadNullAsEmptyString("CodLocal")
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

End Class
