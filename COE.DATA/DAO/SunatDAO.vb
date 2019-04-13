Imports System.Data.SqlClient 

Public Class SunatDAO
    Public Property Codigo As Int32 = 0

    Public Function GetByALL() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_sunat_errores_get_all"
        End With

        Try
            cnx.Open()
            dt.Load(cmd.ExecuteReader)
        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return dt
    End Function
End Class
