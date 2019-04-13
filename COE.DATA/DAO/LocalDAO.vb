Imports System.Data.SqlClient

Public Class LocalDAO

    Public Function GetByALL() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantesWeb)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_local_get_all"
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw ex
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
End Class
