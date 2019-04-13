Imports COE.FRAMEWORK

#Region "Enumeracion"
Public Enum eZipXML
    Comprime = 0
    Descomprime = 1
End Enum
Public Enum eTipoComprobante
    Factura = 1
    Boleta = 2
    NotaCredito = 3
    NotaDebito = 4
    Resumen = 5
    ComunicacionBaja = 6
End Enum
Public Enum eEstadoComprobante2
    Pendiente = 1
    Aceptado = 2
    Rechazado = 3
    EnProceso = 4
End Enum
#End Region

Public Class ComprobanteDAO

    Public Function GetByALL(Optional Tipo As Int16 = 1) As DataTable
        Return Tools.DAOGetByALL(ConexionDAO.GetConexionDBComprobantes, "coe_comprobante_tipo_get_all", {"Tipo"}, {Tipo})
    End Function


    Public Function GetRptLiquidacion(FechaInicial As Date, FechaFinal As Date) As DataTable
        Return Tools.DAOGetByALL(ConexionDAO.GetConexionDBComprobantes, "coe_comprobante_rpt_liquidacion", {"FechaInicial", "FechaFinal"}, {FechaInicial, FechaFinal})
    End Function


End Class
