


Imports System.Web

Friend Enum RestRequestType
    [GET]
    [POST]
End Enum

Public MustInherit Class RestRequest(Of T)
    Inherits RestRequest
    Private _Body As T
    Friend Sub New(BaseURL As String, Method As String, Parm As List(Of String), Body As T, RequestType As RestRequestType, Optional Cookies As List(Of KeyValuePair(Of String, String)) = Nothing, Optional Headers As List(Of KeyValuePair(Of String, String)) = Nothing)
        MyBase.New(BaseURL, Method, Parm, RequestType, Cookies, Headers)
        _Body = Body
    End Sub

    Public ReadOnly Property MessageBody As T
        Get
            Return _Body
        End Get
    End Property

End Class


Public MustInherit Class RestRequest
    Implements IDisposable

    Private _BaseURL As String
    Private _Method As String
    Private _Param As String
    Private _RequestType As RestRequestType
    Private _Cookies As List(Of KeyValuePair(Of String, String))
    Private _Headers As List(Of KeyValuePair(Of String, String))

    Friend ReadOnly Property RequestType As RestRequestType
        Get
            Return _RequestType
        End Get
    End Property

    Friend Sub New(BaseURL As String, Method As String, RequestType As RestRequestType, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String)))
        _BaseURL = BaseURL
        _Method = Method
        _Param = ""
        _RequestType = RequestType
        If Cookies Is Nothing Then _Cookies = New List(Of KeyValuePair(Of String, String)) Else _Cookies = Cookies
        If Headers Is Nothing Then _Headers = New List(Of KeyValuePair(Of String, String)) Else _Headers = Headers
    End Sub



    Friend Sub New(BaseURL As String, Method As String, Parm As List(Of String), RequestType As RestRequestType, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String)))
        _BaseURL = BaseURL
        _Method = Method
        _Param = ParmURL(Parm)
        _RequestType = RequestType
        If Cookies Is Nothing Then _Cookies = New List(Of KeyValuePair(Of String, String)) Else _Cookies = Cookies
        If Headers Is Nothing Then _Headers = New List(Of KeyValuePair(Of String, String)) Else _Headers = Headers
    End Sub

    Friend Sub New(BaseURL As String, Parm As List(Of String), RequestType As RestRequestType, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String)))
        _BaseURL = BaseURL
        _Method = ""
        _Param = ParmURL(Parm)
        _RequestType = RequestType
        If Cookies Is Nothing Then _Cookies = New List(Of KeyValuePair(Of String, String)) Else _Cookies = Cookies
        If Headers Is Nothing Then _Headers = New List(Of KeyValuePair(Of String, String)) Else _Headers = Headers
    End Sub


    Friend Function ParmURL(Parm As List(Of String)) As String
        Dim Parms As String = ""

        If Parm IsNot Nothing Then

            For Each p In Parm
                Parms &= "/" & p
            Next
        End If
        Parms = Uri.EscapeUriString(Parms)

        Return Parms
    End Function

    Public ReadOnly Property URL As String
        Get
            If _Method = "" And _Param = "" Then
                Return _BaseURL
            End If

            Return _BaseURL & "/" & _Method & _Param
        End Get
    End Property

    Public ReadOnly Property Cookies As List(Of KeyValuePair(Of String, String))
        Get
            Return _Cookies
        End Get
    End Property

    Public ReadOnly Property Headers As List(Of KeyValuePair(Of String, String))
        Get
            Return _Headers
        End Get
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                _BaseURL = Nothing
                _Method = Nothing
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class