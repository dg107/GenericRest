Imports System.IO
Imports System.Net
Imports Newtonsoft.Json

Public Class RestUtil
    Public Shared ResultResponse As String

    Private Shared _DefaultConnectionLimit As Integer = 1000
    Private Shared _MaxServicePoints As Integer = 1000
    Private Shared _MaxServicePointIdleTime As Integer = 10000

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
        Dim result As String = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(RestRequest.MessageBody), RestRequest.Cookies)
        Try
            Return JsonConvert.DeserializeObject(Of T2
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try

    End Function
    Public Shared Function GetResponse(Of T)(RestRequest As RestPost) As T
        Dim result As String = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(Nothing), RestRequest.Cookies)
        Try
            Return JsonConvert.DeserializeObject(Of T
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try
    End Function

    Public Shared Sub GetResponse(Of T)(RestRequest As RestPost(Of T))

        ResultResponse = RestUtil.ProcessPostRequest(RestRequest.URL, JsonConvert.SerializeObject(RestRequest.MessageBody), RestRequest.Cookies)

    End Sub
    Public Shared Function GetResponse(Of T)(RestRequest As RestGet) As T
        Dim result As String = RestUtil.ProcessGetRequest(RestRequest.URL, RestRequest.Cookies)
        Try
            Return JsonConvert.DeserializeObject(Of T
            )(result)
        Catch ex As Exception
            Throw New Exception(result)
        End Try
    End Function
    Public Shared Sub GetResponse(RestRequest As RestGet)

        ResultResponse = RestUtil.ProcessGetRequest(RestRequest.URL, RestRequest.Cookies)

    End Sub

    Public Shared Sub GetResponse(RestRequest As RestPost)

        ResultResponse = RestUtil.ProcessPostRequest(RestRequest.URL, Nothing, RestRequest.Cookies)

    End Sub

    Public Shared Function GetObjectJson(O As Object) As String
        Try
            Return JsonConvert.SerializeObject(O)
        Catch ex As Exception
            Throw New Exception(O.ToString)
        End Try
    End Function

    Public Shared Function ProcessPostRequest(restURL As String, bodyContent As String, Cookies As List(Of KeyValuePair(Of String, String))) As String
        Dim responseString As String = String.Empty

        Try
            Dim request As HttpWebRequest = WebRequest.Create(restURL)
            request.UseDefaultCredentials = True
            request.Method = "POST"
            request.ContentType = "application/json"

            Dim CookieContents As String = AddCookies(request, Cookies)

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

    Private Shared Function AddCookies(ByRef request As HttpWebRequest, cookies As List(Of KeyValuePair(Of String, String))) As String
        Dim CookieContents As String = ""

        If (Not Debugger.IsAttached) Then
            If cookies IsNot Nothing AndAlso cookies.Count > 0 Then
                request.CookieContainer = New CookieContainer
                For Each cookie In cookies
                    request.CookieContainer.Add(New Net.Cookie(cookie.Key, cookie.Value, "/", request.Host))
                    CookieContents &= " Key: " & cookie.Key & " Value: " & cookie.Value
                Next
            End If
        End If
        Return CookieContents
    End Function

    Public Shared Function ProcessGetRequest(restURL As String, cookies As List(Of KeyValuePair(Of String, String))) As String
        Dim responseString As String = String.Empty
        Try
            Dim request As HttpWebRequest = WebRequest.Create(restURL)
            request.UseDefaultCredentials = True
            request.Method = "GET"
            request.ContentType = "application/json"

            Dim CookieContents As String = AddCookies(request, cookies)

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
