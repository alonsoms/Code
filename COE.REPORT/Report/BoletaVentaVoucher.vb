Imports System.Drawing.Printing
Imports DevExpress.XtraPrinting.BarCode

Public Class BoletaVentaVoucher
    Private Sub BoletaVentaVoucher_BeforePrint(sender As Object, e As PrintEventArgs) Handles Me.BeforePrint

        With XrBarCode1
            .AutoModule = True
            .ShowText = False
        End With

        'Especificacion SUNAT
        'Nivel de correccion de error = Nivel Q
        Dim EspecificacionQR As QRCodeGenerator = New QRCodeGenerator()
        With EspecificacionQR
            .CompactionMode = QRCodeCompactionMode.Byte
            .ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q
            .Version = QRCodeVersion.AutoVersion

        End With
        'Se establece Especificacion del QR
        XrBarCode1.Symbology = EspecificacionQR
    End Sub
End Class