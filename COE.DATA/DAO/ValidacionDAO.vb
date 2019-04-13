Imports System.Data.SqlClient

Public Class ValidacionDAO

    Public Enum eValidar
        ComunicacionBajaFechaUnica = 1
        ComunicacionBajaExistaComprobantesAnulados = 2
    End Enum

    Public Function Validar(Tipo As eValidar, Fecha As DateTime) As Boolean
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim Result As Boolean

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_validacion"
            .Parameters.Add("@tipo", SqlDbType.Int).Value = Tipo
            .Parameters.Add("@fecha", SqlDbType.DateTime).Value = Fecha
        End With

        Try
            cnx.Open()
            Result = cmd.ExecuteScalar()

        Catch ex As Exception
            Throw
        Finally
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        End Try
        Return Result
    End Function

End Class
