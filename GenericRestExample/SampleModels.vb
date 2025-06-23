Public Class User
    Public Property Id As Integer
    Public Property Name As String
    Public Property Email As String
End Class

Public Class CreateOrderRequest
    Public Property CustomerId As Integer
    Public Property ItemIds As List(Of Integer)
End Class

Public Class OrderResponse
    Public Property OrderId As Integer
    Public Property Status As String
End Class
