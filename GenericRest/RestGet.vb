Public Class RestGet
    Inherits RestRequest

    Public Sub New(BaseURL As String, Method As String, Optional Parm As List(Of String) = Nothing, Optional Cookies As List(Of KeyValuePair(Of String, String)) = Nothing)
        MyBase.New(BaseURL, Method, Parm, RestRequestType.GET, Cookies)
    End Sub

End Class