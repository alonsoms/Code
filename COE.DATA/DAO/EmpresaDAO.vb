Imports COE.FRAMEWORK

Public Class EmpresaDAO

    'Public Function GetByAll() As List(Of EmpresaBE)

    '    'Se lee el archivo de configuracion para cargar la ruta del archico config.xml
    '    Dim Ruta As String = Tools.ReadAppSettings("RutaConfigXML")

    '    'Se carga el archivo Config.XML
    '    Dim Archivo As String = My.Computer.FileSystem.ReadAllText(Ruta)

    '    'Se transforma el archivo a formato XML
    '    Dim XMLDoc As XElement = XElement.Parse(Archivo)

    '    'Se obtiene los datos del archivo de configuracion Config.XML
    '    Dim ListaRuc As IEnumerable(Of String) = From Item In XMLDoc...<RUC> Select Item.Value
    '    Dim ListaRazonSocial As IEnumerable(Of String) = From Item In XMLDoc...<RAZONSOCIAL> Select Item.Value
    '    Dim ListaCodLocal As IEnumerable(Of String) = From Item In XMLDoc...<CODLOCAL> Select Item.Value

    '    Dim ListaConexionDB As IEnumerable(Of String) = From Item In XMLDoc...<CONEXIONDB> Select Item.Value
    '    Dim ListaUser As IEnumerable(Of String) = From Item In XMLDoc...<USER> Select Item.Value
    '    Dim ListaPass As IEnumerable(Of String) = From Item In XMLDoc...<PASS> Select Item.Value

    '    Dim ListaEmpresas As New List(Of EmpresaBE)

    '    For index = 0 To ListaRuc.Count - 1
    '        Dim EmpresaBE As New EmpresaBE

    '        With EmpresaBE
    '            .RUC = ListaRuc(index)
    '            .RazonSocial = ListaRazonSocial(index)
    '            .CodLocal = ListaCodLocal(index)
    '            .ConexionDB = ListaConexionDB(index)
    '            .User = ListaUser(index)
    '            .Pass = ListaPass(index)
    '        End With

    '        ListaEmpresas.Add(EmpresaBE)
    '    Next

    '    Return ListaEmpresas
    'End Function

End Class
