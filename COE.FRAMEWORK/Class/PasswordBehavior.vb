Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports Microsoft.Web.Services3.Security.Tokens

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
