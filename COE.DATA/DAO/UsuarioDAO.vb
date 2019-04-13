Imports System.Data.SqlClient

Public Class UsuarioDAO
    Public Property IDUsuario As Int32

    Public Function GetByID() As UsuarioBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim UsuarioBE As New UsuarioBE

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_usuario_get_id"
            .Parameters.Add("@idusuario", SqlDbType.Int).Value = Me.IDUsuario
        End With

        Try
            'Se valida que el IDUsuario sea diferente de cero
            If Me.IDUsuario = 0 Then
                Throw New Exception("IDUsuario es cero.")
            End If

            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read()
                    With UsuarioBE
                        .IDUsuario = dr.ReadNullAsEmptyString("idusuario")
                        .Nombres = dr.ReadNullAsEmptyString("nombres")
                        .ApellidoPaterno = dr.ReadNullAsEmptyString("apellidopaterno")
                        .ApellidoMaterno = dr.ReadNullAsEmptyString("apellidomaterno")
                        .Login = dr.ReadNullAsEmptyString("login")
                        .Password = dr.ReadNullAsEmptyString("password")
                    End With
                End While
                dr.Close()
            End If
        Catch ex As Exception
            Throw
        Finally
            cnx.Close()
        End Try
        Return UsuarioBE
    End Function
    Public Function GetByLoginPassword(ByVal Login As String, Password As String) As UsuarioBE
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim UsuarioBE As New UsuarioBE

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_usuario_get_login"
            .Parameters.Add("@Login", SqlDbType.VarChar, 15).Value = Login
            .Parameters.Add("@Password", SqlDbType.VarChar, 15).Value = Password
        End With

        Try
            cnx.Open()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                While dr.Read()
                    With UsuarioBE
                        .IDUsuario = dr.ReadNullAsEmptyString("idusuario")
                        .Nombres = dr.ReadNullAsEmptyString("nombres")
                        .ApellidoPaterno = dr.ReadNullAsEmptyString("apellidopaterno")
                        .ApellidoMaterno = dr.ReadNullAsEmptyString("apellidomaterno")
                        .Login = dr.ReadNullAsEmptyString("login")
                        .Password = dr.ReadNullAsEmptyString("password")
                    End With
                End While
                dr.Close()
            End If
        Catch ex As Exception
            Throw
        Finally
            cnx.Close()
        End Try
        Return UsuarioBE
    End Function
    Public Function GetByALL() As DataTable
        Dim cnx As New SqlConnection(ConexionDAO.GetConexionDBComprobantes)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable

        With cmd
            .Connection = cnx
            .CommandType = CommandType.StoredProcedure
            .CommandText = "coe_usuario_get_all"
        End With

        Try
            cnx.open()
            dt.load(cmd.ExecuteReader)
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
