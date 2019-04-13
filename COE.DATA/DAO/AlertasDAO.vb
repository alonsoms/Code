#Region "Imports"
Imports System.Data
Imports System.Data.SqlClient
#End Region

Public Class AlertasDAO

    Public Function GetByAll() As String
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim Result As String = String.Empty

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_comprobante_alerta"
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)

            'Se valida si hay registros
            If dt.Rows.Count > 0 Then
                Result = ConvertTableHTML(dt)
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
    Public Function ConvertTableHTML(dt As DataTable) As String
        Dim HTMLStyle As String = "<head><style>.datagrid table { border-collapse: collapse; text-align: left; width: 100%; } .datagrid {font: normal 12px/150% Arial, Helvetica, sans-serif; background: #fff; overflow: hidden; border: 1px solid #000000; -webkit-border-radius: 3px; -moz-border-radius: 3px; border-radius: 3px; }.datagrid table td, .datagrid table th { padding: 3px 1px; }.datagrid table thead th {background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #006699), color-stop(1, #00557F) );background:-moz-linear-gradient( center top, #006699 5%, #00557F 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#006699', endColorstr='#00557F');background-color:#006699; color:#FFFFFF; font-size: 10px; font-weight: bold; border-left: 1px solid #0070A8; } .datagrid table thead th:first-child { border: none; }.datagrid table tbody td { color: #00496B; border-left: 1px solid #E1EEF4;font-size: 12px;font-weight: normal; }.datagrid table tbody .alt td { background: #E1EEF4; color: #00496B; }.datagrid table tbody td:first-child { border-left: none; }.datagrid table tbody tr:last-child td { border-bottom: none; }</style></head>"
        Dim HTMLHeaderTable As String = "<div class=""datagrid""><table><thead><tr><th>DOC</th><th>MOTIVO</th><th>FECHA</th><th>NUMERO</th><th>ID.COMPROBANTE</th></tr></thead><tbody>"
        Dim HTMLBodyLine1Table As String = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>"
        Dim HTMLBodyLine2Table As String = "<tr class=""alt""><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>"
        Dim HTMLFooterTable As String = "</tbody></table></div>"
        Dim Result As String = String.Empty

        'Se crea el cuerpo de la tabla
        For Index As Int32 = 0 To dt.Rows.Count - 1
            If Index Mod 2 = 0 Then
                Result &= String.Format(HTMLBodyLine1Table, dt.Rows(Index).Item("tipo").ToString, dt.Rows(Index).Item("motivo").ToString, dt.Rows(Index).Item("fechaemision").ToString, dt.Rows(Index).Item("numero").ToString, dt.Rows(Index).Item("idcomprobante").ToString)
            Else
                Result &= String.Format(HTMLBodyLine2Table, dt.Rows(Index).Item("tipo").ToString, dt.Rows(Index).Item("motivo").ToString, dt.Rows(Index).Item("fechaemision").ToString, dt.Rows(Index).Item("numero").ToString, dt.Rows(Index).Item("idcomprobante").ToString)
            End If
        Next

        'Se agrega el estilo,cabecera, cuerpo y pie de la tabla
        Result = HTMLStyle & HTMLHeaderTable & Result & HTMLFooterTable

        Return Result
    End Function
End Class
