Public Interface IWebClient
    Function ProcessPostRequest(restURL As String, bodyContent As String, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String
    Function ProcessGetRequest(restURL As String, cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String

End Interface
