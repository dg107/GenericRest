Imports Generic

Public Class Example


    Public Function GetData(RequestIDList As List(Of Integer)) As List(Of Integer)

        'Your request will have a body of a list of integers

        'BaseURL = The URL of the service
        'MethodName = "Name of the Method on the service (ie service/method
        'Here I am passing in Nothing for the Param value, params are added to the end of the URL like: http://service/method/{Param1}/{Param2} ect
        'RequestIDList is the list of integers you are passing to the rest service in the body

        Using req As RestPost(Of List(Of Integer)) = New RestPost(Of List(Of Integer))("http://serviceURL", "MethodName", Nothing, RequestIDList)
            'your response will be the returned value from the service, in a list of integers
            'note: you need to know what the service is going to return, it can't process any other response
            'The first value is what you are sending (list of Integer) the second value is what you are getting back (list of Integer)
            Return RestUtil.GetResponse(Of List(Of Integer), List(Of Integer))(req)
        End Using


    End Function

End Class
