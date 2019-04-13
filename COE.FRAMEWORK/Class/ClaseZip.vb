Imports System.IO
Imports System.IO.Compression
Imports System.IO.Compression.ZipArchive

Public Class ComprimeZIP

    Shared Sub EmpaquetaZIP(NombreArchivoZip As String, NombreArchivoXML As String, RutaArchivoXML As String)

        Using zipToOpen As FileStream = New FileStream(NombreArchivoZip, FileMode.Create)

            Using archive As ZipArchive = New ZipArchive(zipToOpen, ZipArchiveMode.Create)
                Dim readmeEntry As ZipArchiveEntry = archive.CreateEntry(NombreArchivoXML)

                Dim writer As StreamWriter = New StreamWriter(readmeEntry.Open())

                writer.Write(My.Computer.FileSystem.ReadAllText(RutaArchivoXML))
                writer.Flush()
                writer.Close()

            End Using
        End Using

    End Sub

    Shared Sub Empaqueta(ByVal Archivo As FileInfo)
        Try
            Dim Entrada As FileStream = Archivo.OpenRead
            If (File.GetAttributes(Archivo.FullName) And FileAttributes.Hidden) <> FileAttributes.Hidden Then
                'Using Comprimido As FileStream = File.Create(Archivo.FullName.Replace(".xml", ".zip"))
                'Comprimido.
                Using Comprimido As FileStream = File.Create(Archivo.FullName & ".zip")
                    Using compresion As GZipStream = New GZipStream(Comprimido, CompressionMode.Compress, True)

                        Entrada.CopyTo(compresion)
                    End Using
                End Using
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Shared Sub Desempaqueta(ByVal archivo As FileInfo)
        Try
            Using Entrada As FileStream = archivo.OpenRead()
                Dim nombrearchivo As String = archivo.FullName
                Dim nuevoarchivo = nombrearchivo.Remove(nombrearchivo.Length - archivo.Extension.Length)
                Using descomprimido As FileStream = File.Create(nuevoarchivo)
                    Using descompresion As GZipStream = New GZipStream(Entrada, CompressionMode.Decompress)
                        descompresion.CopyTo(descomprimido)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
