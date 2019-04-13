Imports System.IO

Public Class Form4
    Private Sub btnLeerXML_Click(sender As Object, e As EventArgs) Handles btnLeerXML.Click
        Dim Ruta As String = "D:\Proyectos\1804-INGESISE-CE\CODE\COE.TESTING\Config.xml"
        Dim Archivo As String = My.Computer.FileSystem.ReadAllText(Ruta)


        'Se transforma el objeto OrdenXML al objeto OrdenBE
        Dim XMLDoc As XElement = XElement.Parse(Archivo)

        'Se obtiene los datos de las empresas
        Dim ListaRuc As IEnumerable(Of String) = From Item In XMLDoc...<RUC> Select Item.Value
        Dim ListaRazonSocial As IEnumerable(Of String) = From Item In XMLDoc...<RAZONSOCIAL> Select Item.Value
        Dim ListaConexionDB As IEnumerable(Of String) = From Item In XMLDoc...<CONEXIONDB> Select Item.Value
        Dim ListaUser As IEnumerable(Of String) = From Item In XMLDoc...<USER> Select Item.Value
        Dim ListaPass As IEnumerable(Of String) = From Item In XMLDoc...<PASS> Select Item.Value

        Dim ListaEmpresas As New List(Of EmpresasBE)

        For index = 0 To ListaRuc.Count - 1
            Dim EmpresaBE As New EmpresasBE

            With EmpresaBE
                .RUC = ListaRuc(index)
                .RazonSocial = ListaRazonSocial(index)
                .ConexionDB = ListaConexionDB(index)
                .User = ListaUser(index)
                .Pass = ListaPass(index)
            End With

            ListaEmpresas.Add(EmpresaBE)
        Next

        MessageBox.Show(ListaEmpresas.Count)



    End Sub
End Class

Public Class EmpresasBE
    Public Property RUC As String
    Public Property RazonSocial As String
    Public Property ConexionDB As String
    Public Property User As String
    Public Property Pass As String

End Class