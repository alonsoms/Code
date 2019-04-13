Imports System.IO

Public Class SistemaDAO
    Public Property UsuarioBE As UsuarioBE
    Public Property EmisorBE As EmisorBE
    Public Property NombrePC As String
    Public Property NombreAplicacion As String

    Public Function GetRutaCarpetaSUNAT(EmisorBE As EmisorBE) As String
        Dim Result As String = String.Empty
        Try
            'Se obtiene la ruta donde se guardara los archivos de la SUNAT. 
            Dim RutaCarpeta As String = EmisorBE.RutaCarpetaArchivosXML

            'Se obtiene el numero de año
            Dim CarpetaAnio As String
            CarpetaAnio = DateTime.Now.Year.ToString & "\"

            'Se obtiene el numero de mes
            Dim CarpetaMes As String
            CarpetaMes = DateTime.Now.Month.ToString("00")

            'Se obtiene el numero de dia
            Dim CarpetaDia As String
            CarpetaDia = DateTime.Now.Day.ToString("00")

            'Si es que no existe, Se crea la carpeta donde se guardara los archivos de la sunat
            If Not Directory.Exists(RutaCarpeta) Then
                Directory.CreateDirectory(RutaCarpeta)
            End If

            'Si es que no existe, Se crea la carpeta segun el año actual del sistema.
            If Not Directory.Exists(RutaCarpeta & CarpetaAnio) Then
                Directory.CreateDirectory(RutaCarpeta & CarpetaAnio)
            End If

            'Si es que no existe, Se crea la carpeta segun el mes actual del sistema.
            If Not Directory.Exists(RutaCarpeta & CarpetaAnio & CarpetaMes) Then
                Directory.CreateDirectory(RutaCarpeta & CarpetaAnio & CarpetaMes)
            End If

            'Si es que no existe, Se crea la carpeta segun el dia actual del sistema.
            If Not Directory.Exists(RutaCarpeta & CarpetaAnio & CarpetaMes & "\" & CarpetaDia) Then
                Directory.CreateDirectory(RutaCarpeta & CarpetaAnio & CarpetaMes & "\" & CarpetaDia)
            End If

            Result = RutaCarpeta & CarpetaAnio & CarpetaMes & "\" & CarpetaDia & "\"
        Catch ex As Exception
            Throw New Exception("No se creo la carpeta en el año, mes y dia donde se guarda los archivos de la SUNAT. " & ex.Message)
        End Try
        Return Result
    End Function
End Class
