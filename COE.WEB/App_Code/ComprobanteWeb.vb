Imports System.IO
Imports COE.DATA
Imports Microsoft.Azure
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Blob

Public Class ComprobanteWeb

    Public Function CopiarComprobanteCarpetaTemporal(BE As ComprobanteWebBE, NumeroRuc As String) As String
        Dim RutaFolderTemporal As String = "https://cedigital.blob.core.windows.net"
        Dim NombreTemporal As String = Path.GetRandomFileName.Replace(".", String.Empty)
        Dim scriptHTML As String = "<table><tr><td><h4>Haga clic para descargar su comprobante electrónico</h4></td></tr><tr><td><br/><center><a href=""{0}"" target=""_blank""><img src=""../Content/img/pdfs128.png"" alt=""Descargar comprobante formato PDF"" border=""0""/></a> <a href=""{1}"" target=""_blank"" ><img src=""../Content/img/xml128.png""alt=""Descargar comprobante formato XML"" border=""0""/></a></center></td></tr></table>"
        Dim Result As String = String.Empty

        Try
            'Si el Nombre de archivo no existe, el frame se carga en blanco
            If BE.RutaArchivoXML = "" Then
                'Literal1.Text = ""
                Return Result
            End If

            'Se recupera la cadena de conexion
            'Dim storageAccount As CloudStorageAccount = CloudStorageAccount.Parse(Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageConnectionString"))
            Dim storageAccount As CloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings("StorageConnectionString"))


            ' Create the blob client.
            Dim blobClient As CloudBlobClient = storageAccount.CreateCloudBlobClient()

            'Se obtiene el contenedor del storage, tiene como nombre el ruc de la empresa
            Dim container As CloudBlobContainer = blobClient.GetContainerReference("r" & NumeroRuc)

            'Se crea el contenedor si no existe
            container.CreateIfNotExists()

            Dim sasConstraints As New SharedAccessBlobPolicy()

            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5)
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read

            Dim sasBlobToken = container.GetSharedAccessSignature(sasConstraints)

            'Se muestra el documento PDF en la carpeta temporal
            Dim RutaPDF As String = RutaFolderTemporal & BE.RutaArchivoPDF & sasBlobToken
            Dim RutaXML As String = RutaFolderTemporal & BE.RutaArchivoXML & sasBlobToken

            Result = String.Format(scriptHTML, RutaPDF, RutaXML)

        Catch ex As Exception
            'Throw ex
        End Try
        Return Result
    End Function
End Class
