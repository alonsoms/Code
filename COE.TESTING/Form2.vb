Imports System.IdentityModel.Tokens
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports System.Xml
Imports Microsoft.Web.Services3.Security.Tokens

Public Class Form2
    Private Sub btnUsuarioDinamico_Click(sender As Object, e As EventArgs) Handles btnUsuarioDinamico.Click
        'Dim SunatCDRSE As New SunatSE.billServiceClient
        'Dim RespuestaSUNAT As SunatSE.statusResponse


        ''Se configura los parametros de seguridad
        'System.Net.ServicePointManager.UseNagleAlgorithm = True
        'System.Net.ServicePointManager.Expect100Continue = False
        'System.Net.ServicePointManager.CheckCertificateRevocationList = True


        'Try
        '    'Se crea la credencial
        '    SunatCDRSE.ClientCredentials.CreateSecurityTokenManager()

        '    'Se agrega las credenciales en el objeto del Behavior
        '    Dim PB = New PasswordBehavior("20492883281JLRAMOS8", "jj2007ra")
        '    SunatCDRSE.Endpoint.EndpointBehaviors.Add(PB)

        '    'Se abre el servicio de la SUNAT
        '    SunatCDRSE.Open()

        '    'Se obtiene la respuesta del envio del resumen
        '    'RespuestaSUNAT = SunatCDRSE.getStatusCdr("20492883281", "01", "F011", "00000097")

        'Catch ex As Exception
        '    MessageBox.Show(ex.Message, "Advertencia")
        'Finally
        '    SunatCDRSE.Close()
        'End Try


    End Sub
End Class


Public Class PasswordBehavior
    Implements IEndpointBehavior
    Public Property UserName As String
    Public Property PassWord As String

    Public Sub New(Username As String, Password As String)
        Me.UserName = Username
        Me.PassWord = Password
    End Sub

    Public Sub AddBindingParameters(endpoint As ServiceEndpoint, bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters
        'No se usa, pero es parte de la implementacion de la interface
    End Sub

    Public Sub ApplyClientBehavior(endpoint As ServiceEndpoint, clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior
        clientRuntime.ClientMessageInspectors.Add(New PasswordInspector(Me.UserName, Me.PassWord))
    End Sub

    Public Sub ApplyDispatchBehavior(endpoint As ServiceEndpoint, endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
        Throw New NotImplementedException()
    End Sub

    Public Sub Validate(endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate
        'No se usa, pero es parte de la implementacion de la interface
    End Sub
End Class

Public Class PasswordInspector
    Implements IClientMessageInspector
    Public Property Username As String
    Public Property Password As String

    Public Sub New(UserName As String, Password As String)
        Me.Username = UserName
        Me.Password = Password
    End Sub
    Public Function BeforeSendRequest(ByRef request As Message, channel As IClientChannel) As Object Implements IClientMessageInspector.BeforeSendRequest
        Dim Token As New UsernameToken(Me.Username, Me.Password, PasswordOption.SendPlainText)
        Dim SecurityToken As XmlElement = Token.GetXml(New XmlDocument())

        'Se modifica el XML Generado
        Dim nodo = SecurityToken.GetElementsByTagName("wsse:Nonce").Item(0)

        nodo.RemoveAll()

        Dim securityHeader As MessageHeader
        securityHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", SecurityToken, False)

        request.Headers.Add(securityHeader)

        Return Convert.DBNull

    End Function

    Public Sub AfterReceiveReply(ByRef reply As Message, correlationState As Object) Implements IClientMessageInspector.AfterReceiveReply
        'No se usa, pero es parte de la implementacion de la interface
    End Sub
End Class