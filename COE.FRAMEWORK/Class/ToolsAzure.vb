Imports System.IO
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob
Public Class ToolsAzure
    Shared Function SaveStorageFiles(Optional ByVal CadenaConexionStorage As String = "", Optional NombreStorage As String = "", Optional NombreFile As String = "") As Boolean
        Dim Result As Boolean = False

        'Se valida que exista el archivo
        If Not File.Exists(NombreFile) Then
            Throw New Exception(String.Format("Archivo [0] no existe", NombreFile))
        End If

        'Si no el parametro esta vacio se asigna el interno
        If CadenaConexionStorage = "" Then
            CadenaConexionStorage = "DefaultEndpointsProtocol=https;AccountName=cedigital;AccountKey=zAreV/LXFa7l/BffNls6YaRBsOP+W1p3KjgrC0n0Wukj2wLtbRwUMqYlJVfo4NVmmlswynXQ0iJnFc0aj8oy8A==;EndpointSuffix=core.windows.net"
        End If
        Dim storageAccount As CloudStorageAccount
        storageAccount = CloudStorageAccount.Parse(CadenaConexionStorage)

        'Se recupera el blob
        Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

        'Se recupera el contenedor previamente creado en Azure. El nombre es minusculas por estandar
        'Se establece el nombre del contenedor 
        Dim Contenedor As CloudBlobContainer = blobClient.GetContainerReference(NombreStorage)

        'Se crea el contenedor si no existe
        Contenedor.CreateIfNotExists()

        'Se crea un bloque para el archivo
        Dim Bloque As CloudBlockBlob = Contenedor.GetBlockBlobReference(Path.GetFileName(NombreFile))

        'Se publica bloque 1
        Using fs As FileStream = New FileStream(NombreFile, FileMode.Open)
            Bloque.UploadFromStream(fs)
            Result = True
        End Using

        Return Result
    End Function
End Class
