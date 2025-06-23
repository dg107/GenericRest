Imports System.IO
Imports System.Net

Public Class GenericHttpWebRequest
    Implements IWebClient

    Private Shared _DefaultConnectionLimit As Integer = 1000
    Private Shared _MaxServicePoints As Integer = 1000
    Private Shared _MaxServicePointIdleTime As Integer = 10000
    Public Shared Property RequestTimeout As Integer = 10000

    Public Function ProcessPostRequest(restURL As String, bodyContent As String, Cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String Implements IWebClient.ProcessPostRequest

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

    Public Function ProcessGetRequest(restURL As String, cookies As List(Of KeyValuePair(Of String, String)), Headers As List(Of KeyValuePair(Of String, String))) As String Implements IWebClient.ProcessGetRequest
        Dim responseString As String = String.Empty
        Try
            Dim request As HttpWebRequest = WebRequest.Create(restURL)
            request.UseDefaultCredentials = True
            request.Method = "GET"
            request.ContentType = "application/json"
            request.Timeout = RequestTimeout

            If Headers IsNot Nothing Then
                For Each Item In Headers
                    request.Headers("MyHeaderKey") = "MyHeaderValue"
                Next
            End If

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

    Private Shared Sub SetServicePointConnections()
        System.Net.ServicePointManager.DefaultConnectionLimit = _DefaultConnectionLimit
        System.Net.ServicePointManager.MaxServicePoints = _MaxServicePoints
        System.Net.ServicePointManager.MaxServicePointIdleTime = _MaxServicePointIdleTime
    End Sub

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
