Imports System.Configuration
Imports COE.FRAMEWORK

Public Class ConexionDAO
    Shared Property ConexionDBNet As String
    Shared Property ConexionDBCloud As String

    Shared Function GetConexionDBComprobantes() As String
        Dim strCnx As String = String.Empty

        If ConexionDBNet = "" Then
            strCnx = Tools.ReadConexionString("CnxComprobantes")
        Else
            strCnx = ConexionDBNet
        End If

        Return strCnx
    End Function
    Shared Function GetConexionDBComprobantesWeb() As String
        Dim strCnx As String = String.Empty

        If ConexionDBCloud = "" Then
            'strCnx = Tools.ReadConexionString("CnxComprobantesWeb")
            'Se establece la conexion a la base de datos de Azure
            strCnx = "Server=tcp:ceserver.database.windows.net,1433;Initial Catalog=cedata;Persist Security Info=False;User ID=ceadmin2557;Password=Digit@l2557;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

        Else
            strCnx = ConexionDBCloud
        End If

        Return strCnx
    End Function

End Class
