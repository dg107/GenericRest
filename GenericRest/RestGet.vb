Public Class RestGet
    Inherits RestRequest

    Public Sub New(BaseURL As String, Method As String, Parm As List(Of String), Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String)))
        MyBase.New(BaseURL, Method, Parm, RestRequestType.GET, Cookies, Headers)
    End Sub

    Public Sub New(BaseURL As String)
        MyBase.New(BaseURL, "", RestRequestType.GET, Nothing, Nothing)
    End Sub

    Public Sub New(BaseURL As String, Method As String, Headers As List(Of KeyValuePair(Of String, String)))
        MyBase.New(BaseURL, Method, RestRequestType.GET, Nothing, Headers)
    End Sub

    Public Sub New(BaseURL As String, Parm As List(Of String), Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String)))
        MyBase.New(BaseURL, Parm, RestRequestType.GET, Cookies, Headers)
    End Sub


    Public Sub New(BaseURL As String, Parm As List(Of String))
        MyBase.New(BaseURL, Parm, RestRequestType.GET, Nothing, Nothing)
    End Sub

End Class