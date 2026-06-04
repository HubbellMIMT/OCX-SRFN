Option Strict On

Public Class FormListProduct
	Public Property Product As String
	Public Property MeterType As String

End Class

Public Class FormList

	Public Property HashValue As String
	Public Property FormListProducts As List(Of FormListProduct) = New List(Of FormListProduct)()

End Class
