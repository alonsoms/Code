Imports System.Xml
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports Microsoft.Web.Services3.Security.Tokens

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
