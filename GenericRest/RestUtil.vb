Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization

Public Class RestUtil
    Public Shared ResultResponse As String

    Private Shared _DefaultConnectionLimit As Integer = 1000
    Private Shared _MaxServicePoints As Integer = 1000
    Private Shared _MaxServicePointIdleTime As Integer = 10000
    Public Shared Property RequestTimeout As Integer = 10000

    Public Shared Property DefaultConnectionLimit As Integer
        Get
            Return _DefaultConnectionLimit
        End Get
        Set(value As Integer)
            _DefaultConnectionLimit = value
        End Set
    End Property

    Public Shared Property MaxServicePoints As Integer
        Get
            Return _MaxServicePoints
        End Get
        Set(value As Integer)
            _MaxServicePoints = value
        End Set
    End Property

    Public Shared Property MaxServicePointIdleTime As Integer
        Get
            Return _MaxServicePointIdleTime
        End Get
        Set(value As Integer)
            _MaxServicePointIdleTime = value
        End Set
    End Property


    Public Shared Function GetResponse(Of T1, T2)(RestRequest As RestPost(Of T1)) As T2
        Dim result As String = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(RestRequest.MessageBody), RestRequest.Cookies, RestRequest.Headers)
        Try

            If GetType(T2) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T2)
            End If

            Return JsonConvert.DeserializeObject(Of T2
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try

    End Function

    Public Shared Async Function GetResponseAsync(Of T1, T2)(RestRequest As RestPost(Of T1)) As Task(Of T2)
        RestUtil.RequestTimeout = 100000
        Dim result As String = Await RestUtil.ProcessPostRequestAsync(RestRequest.URL, JsonConvert.SerializeObject(RestRequest.MessageBody), RestRequest.Cookies, RestRequest.Headers)
        Try


            If GetType(T2) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T2)
            End If

            Return JsonConvert.DeserializeObject(Of T2
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try

    End Function

    Public Shared Function GetResponse(Of T)(RestRequest As RestPost(Of T)) As T
        Dim result As String = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(Nothing), RestRequest.Cookies, RestRequest.Headers)
        Try


            If GetType(T) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T)
            End If

            Return JsonConvert.DeserializeObject(Of T
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try
    End Function

    Public Shared Async Function GetResponseAsync(Of T)(RestRequest As RestPost(Of T)) As Task(Of T)
        Dim result As String = Await RestUtil.ProcessPostRequestAsync(RestRequest.URL, JsonConvert.SerializeObject(Nothing), RestRequest.Cookies, RestRequest.Headers)
        Try


            If GetType(T) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T)
            End If

            Return JsonConvert.DeserializeObject(Of T
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try
    End Function

    'Public Shared Sub GetResponse(Of T)(RestRequest As RestPost(Of T))

    '    ResultResponse = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(RestRequest.MessageBody), RestRequest.Cookies, RestRequest.Headers)

    'End Sub
    Public Shared Function GetResponse(Of T)(RestRequest As RestGet) As T
        Dim result As String = RestUtil.ProcessGetRequest(RestRequest.URL, RestRequest.Cookies, RestRequest.Headers)
        Try

            If GetType(T) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T)
            End If

            Return JsonConvert.DeserializeObject(Of T
            )(result)
        Catch ex As Exception
            Dim TheEx As Exception = New Exception(ex.Message, ex)
            TheEx.Data.Add("Response", result)
            Throw TheEx
        End Try
    End Function
    Public Shared Async Function GetResponseAsync(Of T)(RestRequest As RestGet) As Task(Of T)
        Dim result As String = Await RestUtil.ProcessGetRequestAsync(RestRequest.URL, RestRequest.Cookies, RestRequest.Headers)
        Try

            If GetType(T) Is GetType(String) Then
                Return DirectCast(DirectCast(result, Object), T)
            End If

            Dim settings As New JsonSerializerSettings With {
    .ContractResolver = New DefaultContractResolver With {
        .NamingStrategy = New SnakeCaseNamingStrategy()
    }
}

            Return JsonConvert.DeserializeObject(Of T
            )(result, settings)

        Catch ex As Exception
            Dim TheEx As Exception = New Exception(ex.Message, ex)
            TheEx.Data.Add("Response", result)
            Throw TheEx
        End Try
    End Function

    Public Shared Sub GetResponse(RestRequest As RestGet)

        ResultResponse = RestUtil.ProcessGetRequest(RestRequest.URL, RestRequest.Cookies, RestRequest.Headers)

    End Sub

    Public Shared Async Sub GetResponseAsyc(RestRequest As RestGet)

        ResultResponse = Await RestUtil.ProcessGetRequestAsync(RestRequest.URL, RestRequest.Cookies, RestRequest.Headers)

    End Sub




    Public Shared Sub GetResponse(RestRequest As RestPost)

        ResultResponse = RestUtil.ProcessPostRequest(RestRequest.URL, Nothing, RestRequest.Cookies, RestRequest.Headers)

    End Sub
    Public Shared Async Sub GetResponseAsync(RestRequest As RestPost)

        ResultResponse = Await RestUtil.ProcessPostRequestAsync(RestRequest.URL, Nothing, RestRequest.Cookies, RestRequest.Headers)

    End Sub


    Public Shared Function GetObjectJson(O As Object) As String
        Try
            Return JsonConvert.SerializeObject(O)
        Catch ex As Exception
            Throw New Exception(O.ToString)
        End Try
    End Function

    Public Shared Async Function ProcessPostRequestAsync(restURL As String, bodyContent As String, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As Task(Of String)
        Dim responseString As String = String.Empty

        Try
            Using httpClient As New HttpClient()
                ' Configure the HttpClient timeout
                httpClient.Timeout = TimeSpan.FromMilliseconds(RequestTimeout)

                ' Add headers
                If Headers IsNot Nothing AndAlso Headers.Count > 0 Then
                    For Each header In Headers
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value)
                    Next
                End If

                ' Add cookies if provided
                If Cookies IsNot Nothing AndAlso Cookies.Count > 0 Then
                    Dim cookieHeader = String.Join("; ", Cookies.Select(Function(c) $"{c.Key}={c.Value}"))
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader)
                End If

                ' Set up the request content
                Dim content As New StringContent(bodyContent, Encoding.UTF8, "application/json")

                ' Send the POST request
                Using response As HttpResponseMessage = Await httpClient.PostAsync(restURL, content)
                    ' Ensure the response status code is successful
                    response.EnsureSuccessStatusCode()

                    ' Read the response content
                    responseString = Await response.Content.ReadAsStringAsync()
                End Using
            End Using

        Catch ex As HttpRequestException
            responseString &= $"HTTP Request Exception: {ex.Message}"
        Catch ex As TaskCanceledException
            responseString &= "Request timed out."
        Catch ex As Exception
            responseString &= $"Unexpected error: {ex.Message}"
        End Try

        Return responseString
    End Function

    Public Shared Function ProcessPostRequest(restURL As String, bodyContent As String, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String
        Dim responseString As String = String.Empty

        Try
            Dim request As HttpWebRequest = WebRequest.Create(restURL)
            request.UseDefaultCredentials = True
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Timeout = RequestTimeout

            AddCookies(request, Cookies)
            AddHeaders(request, Headers)

            SetServicePointConnections()

            Using sw As StreamWriter = New StreamWriter(request.GetRequestStream())
                sw.Write(bodyContent)
                sw.Flush()
                sw.Close()
            End Using

            Using response As HttpWebResponse = request.GetResponse()
                If (response.StatusCode = HttpStatusCode.OK) Then
                    Dim rStream As Stream = response.GetResponseStream()
                    Using r As StreamReader = New StreamReader(rStream)
                        responseString &= r.ReadToEnd()
                    End Using
                End If
            End Using

        Catch pv As ProtocolViolationException
            responseString &= pv.Message
        Catch we As WebException
            responseString &= GetWebException(we, restURL)
        Catch ex As Exception
            responseString &= ex.Message
        End Try

        Return responseString

    End Function
    Private Shared Sub AddHeaders(ByRef request As HttpWebRequest, Headers As List(Of KeyValuePair(Of String, String)))
        If Headers IsNot Nothing AndAlso Headers.Count > 0 Then
            For Each Header In Headers
                request.Headers(Header.Key) = Header.Value
            Next
        End If
    End Sub

    Private Shared Sub AddCookies(ByRef request As HttpWebRequest, cookies As List(Of KeyValuePair(Of String, String)))
        Dim Host As String

        If request.Host.StartsWith("localhost:") Then
            Host = "." & System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName
        Else
            Host = request.Host
        End If

        If cookies IsNot Nothing AndAlso cookies.Count > 0 Then
            request.CookieContainer = New CookieContainer
            For Each cookie In cookies
                request.CookieContainer.Add(New Net.Cookie(cookie.Key, cookie.Value, "/", Host))
            Next
        End If
    End Sub

    Public Shared Async Function ProcessGetRequestAsync(restURL As String, cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As Task(Of String)
        Dim responseString As String = String.Empty

        Try
            Using httpClient As New HttpClient()
                ' Configure the HttpClient timeout
                httpClient.Timeout = TimeSpan.FromMilliseconds(RequestTimeout)

                ' Add headers
                If Headers IsNot Nothing AndAlso Headers.Count > 0 Then
                    For Each header In Headers
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value)
                    Next
                End If

                ' Add cookies if provided
                If cookies IsNot Nothing AndAlso cookies.Count > 0 Then
                    Dim cookieHeader = String.Join("; ", cookies.Select(Function(c) $"{c.Key}={c.Value}"))
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader)
                End If

                ' Send the GET request
                Using response As HttpResponseMessage = Await httpClient.GetAsync(restURL)
                    ' Ensure the response status code is successful
                    response.EnsureSuccessStatusCode()

                    ' Read the response content
                    responseString = Await response.Content.ReadAsStringAsync()
                End Using
            End Using

        Catch ex As HttpRequestException
            responseString &= $"HTTP Request Exception: {ex.Message}"
        Catch ex As TaskCanceledException
            responseString &= "Request timed out."
        Catch ex As Exception
            responseString &= $"Unexpected error: {ex.Message}"
        End Try

        Return responseString
    End Function

    Public Shared Function ProcessGetRequest(restURL As String, cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String
        Dim responseString As String = String.Empty
        Try
            Dim request As HttpWebRequest = WebRequest.Create(restURL)
            request.UseDefaultCredentials = True
            request.Method = "GET"
            request.ContentType = "application/json"
            request.Timeout = RequestTimeout

            'If Headers IsNot Nothing Then
            '    For Each Item In Headers
            '        request.Headers("MyHeaderKey") = "MyHeaderValue"
            '    Next
            'End If

            AddCookies(request, cookies)
            AddHeaders(request, Headers)

            SetServicePointConnections()

            Using response As HttpWebResponse = TryCast(request.GetResponse(), HttpWebResponse) 'B.Bailey changed to use Using so 'Need to close so the socket will be freed --Close the response to free resources.
                If (response.StatusCode = HttpStatusCode.OK) Then
                    Dim rStream As Stream = response.GetResponseStream()
                    Using r As StreamReader = New StreamReader(rStream)
                        responseString &= r.ReadToEnd()
                    End Using
                End If
            End Using

        Catch pv As ProtocolViolationException
            responseString &= pv.Message
        Catch we As WebException
            responseString &= GetWebException(we, restURL)
        Catch ex As Exception
            responseString &= ex.Message
        End Try

        Return responseString

    End Function

    ''' <summary>
    ''' This method sets data for service point managers
    ''' </summary>
    Private Shared Sub SetServicePointConnections()
        System.Net.ServicePointManager.DefaultConnectionLimit = _DefaultConnectionLimit
        System.Net.ServicePointManager.MaxServicePoints = _MaxServicePoints
        System.Net.ServicePointManager.MaxServicePointIdleTime = _MaxServicePointIdleTime
    End Sub



    ''' <summary>
    ''' This method populates detailed message of Web Exception
    ''' </summary>     
    ''' <param name="we"><c>we</c> is the web exception.</param>
    ''' <param name="restURL"><c>restURL</c> is the url.</param>
    ''' <param name="includedStackTrace"><c>includedStackTrace</c> is option to whether to include the stack trace information.</param>
    Private Shared Function GetWebException(we As WebException, restURL As String, Optional includedStackTrace As Boolean = True) As String

        Dim message As String = we.Message
        Dim data As String = ""

        data = String.Format("URL: {0}. Connection status:{1}", restURL, we.Status.ToString) & Environment.NewLine & Environment.NewLine
        If we.InnerException IsNot Nothing Then
            If includedStackTrace Then
                data = String.Format("{0}. Error message:{1}. StackTrace:{2}", data, we.InnerException.Message, we.StackTrace)
            Else
                data = String.Format("{0}. Error message:{1}. ", data, we.InnerException.Message)
            End If
        End If

        If we.Response IsNot Nothing Then
            Dim rStream As Stream = we.Response.GetResponseStream()
            Using r As StreamReader = New StreamReader(rStream)
                data &= r.ReadToEnd()
            End Using
        End If

        Return New Exception(message & ":" & data).ToString

    End Function
End Class
