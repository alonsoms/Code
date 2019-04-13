Imports COE.DATA

Module InitClass

    'Se declaran todas las clases de la aplicacion. En el metodo New se carga la configuracion de la clase
    Public SistemaDAO As New SistemaDAO
    Public UsuarioDAO As New UsuarioDAO

    Public FacturaDAO As New FacturaDAO
    Public BoletaDAO As New BoletaVentaDAO
    Public EmisorDAO As New EmisorDAO
    Public NotaCreditoDAO As New NotaCreditoDAO
    Public NotaDebitoDAO As New NotaDebitoDAO
    Public EmpresaDAO As New EmpresaDAO

    Public ComunicacionBajaDAO As New ComunicacionBajaDAO
    Public ResumenDAO As New ResumenDAO
    Public SunatDAO As New SunatDAO
    Public ComprobanteWebDAO As New ComprobanteWebDAO
    Public ValidacionDAO As New ValidacionDAO
    Public ServicioDAO As New ServicioDAO

    Public ComprobanteDAO As New ComprobanteDAO

End Module


