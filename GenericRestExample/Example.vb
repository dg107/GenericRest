Imports Generic

Public Class Example

    ' Retrieve a list of users via GET
    Public Function GetUsers() As List(Of User)
        ' This would send a GET request to: https://api.myapp.com/users
        Using req As New RestGet("https://api.myapp.com", "users", Nothing)
            Return RestUtil.GetResponse(Of List(Of User))(req)
        End Using
    End Function

    ' Submit a new order via POST
    Public Function SubmitOrder(customerId As Integer, itemIds As List(Of Integer)) As OrderResponse
        Dim order As New CreateOrderRequest With {
            .CustomerId = customerId,
            .ItemIds = itemIds
        }

        ' POST to: https://api.myapp.com/orders
        Using req As New RestPost(Of CreateOrderRequest, OrderResponse)("https://api.myapp.com", "orders", Nothing, order)
            Return RestUtil.GetResponse(Of CreateOrderRequest, OrderResponse)(req)
        End Using
    End Function

End Class
