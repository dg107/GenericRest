
Public Class RestPost
    Inherits RestRequest

    Public Sub New(BaseURL As String, Method As String, Parm As List(Of String), Optional Cookies As List(Of KeyValuePair(Of String, String)) = Nothing, Optional Headers As List(Of KeyValuePair(Of String, String)) = Nothing)
        MyBase.New(BaseURL, Method, Parm, RestRequestType.POST, Cookies, Headers)
    End Sub

End Class

Public Class RestPost(Of T)
    Inherits RestRequest(Of T)

    Public Sub New(BaseURL As String, Method As String, Parm As List(Of String), body As T, Optional Cookies As List(Of KeyValuePair(Of String, String)) = Nothing, Optional Headers As List(Of KeyValuePair(Of String, String)) = Nothing)
        MyBase.New(BaseURL, Method, Parm, body, RestRequestType.POST, Cookies, Headers)

    End Sub
End Class

Public Class RestPost(Of T1, T2)
        Inherits RestRequest(Of T1)

        Public Sub New(BaseURL As String, Method As String, Parm As List(Of String), body As T1, Optional Cookies As List(Of KeyValuePair(Of String, String)) = Nothing, Optional Headers As List(Of KeyValuePair(Of String, String)) = Nothing)
            MyBase.New(BaseURL, Method, Parm, body, RestRequestType.POST, Cookies, Headers)

        End Sub

    End Class
