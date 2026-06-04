Option Strict On

Public Class TestScriptCommand

	Public Property MappedCommands As Dictionary(Of String, String)
	Public Property RefNum As Integer
	Public Property Command As SerialCommand
	Public Property Comm_Val As String
	Public Property ExpectedResponse As String
	Private _actualResponse As String
	Public Property ActualResponse As String
		Get
			Return _actualResponse.Replace(Command.CommandName, "").Replace(vbCr, "").Replace(vbLf, "").Trim()
		End Get
		Set(value As String)
			_actualResponse = value
		End Set
	End Property
	Public Property InbTime As String
	Public Property OutbTime As String
	Public Property Sleep As Integer = 250
	Public Property Compare As Integer
	Private _retries As Integer = 0
	Public ReadOnly Property Retries As Integer
		Get
			Return _retries
		End Get
	End Property
	Public Property RetryAction As String = ""
	Public Property RetryJumpToStep As Integer = -1
	Public Property RetryDelayMs As Integer = 0
	Public Property RetriesUsed As Integer = 0
	Public Sub ParseRetriesField(value As String)
		If String.IsNullOrWhiteSpace(value) Then Return
		Dim n As Integer
		' Plain integer (legacy): "3"
		If Integer.TryParse(value.Trim(), n) Then
			_retries = n
			Return
		End If
		' Extended format: "N>>Action>>Ds>>stepX"
		Dim parts As String() = value.Split(New String() {">>"}, StringSplitOptions.None)
		If parts.Length >= 1 AndAlso Integer.TryParse(parts(0).Trim(), n) Then
			_retries = n
			If parts.Length >= 2 Then RetryAction = parts(1).Trim()
			If parts.Length >= 3 Then
				Dim delayStr As String = parts(2).Trim().ToLower()
				If delayStr.EndsWith("s") Then delayStr = delayStr.Substring(0, delayStr.Length - 1)
				Dim secs As Integer
				If Integer.TryParse(delayStr, secs) Then RetryDelayMs = secs * 1000
			End If
			If parts.Length >= 4 Then
				Dim stepStr As String = parts(3).Trim().ToLower()
				If stepStr.StartsWith("step") Then stepStr = stepStr.Substring(4)
				Dim stepNum As Integer
				If Integer.TryParse(stepStr, stepNum) Then RetryJumpToStep = stepNum
			End If
		End If
	End Sub
	Public Property ErrCode As String
	Public Property Attempts As Integer

	Public ReadOnly Property IsReadCommand As Boolean
		Get
			Return String.IsNullOrEmpty(Comm_Val)
		End Get
	End Property

	Public ReadOnly Property IsSuccess As Boolean
		Get
			Dim actual As Long
			Dim expected As Long
			Select Case Compare
				Case 0
					Return True
				Case 1
					'............. update 9/25/2017, find null inbound
					If ActualResponse.Length = 0 Then
						Return False
					End If
					If ActualResponse.Length <> ExpectedResponse.Length Then
						Dim tempactual As String = ActualResponse
						If System.Text.RegularExpressions.Regex.Split(tempactual, " ").Length - 1 > 0
							Dim tmp as string = trim(UCase((Mid(tempactual, 1 + InStrRev(tempactual, " ")))))
							'If ExpectedResponse = trim(Mid(tempactual, 1 + InStrRev(tempactual, " "))) '...........look at last section of string
							If ExpectedResponse = tmp
								Return True
							End If
						End If
					End If
					'.............
					If Trim(ActualResponse) = ExpectedResponse OrElse UCase(Trim(ActualResponse)) = ExpectedResponse then 'or InStr(ExpectedResponse, ActualResponse) > 0 Then
						Return True
						ActualResponse = ""
					Else
						Return False
						ActualResponse = ""
					End If
				Case 2
					Long.TryParse(ActualResponse, actual)
					Long.TryParse(ExpectedResponse, expected)
					Return ActualResponse.Length > 0 AndAlso actual > expected
				Case 3
					Long.TryParse(ActualResponse, actual)
					Long.TryParse(ExpectedResponse, expected)
					Return ActualResponse.Length > 0 AndAlso actual < expected
				Case 4
				Case 5
					Dim startPos As Integer = InStrRev(_actualResponse, " ")
					Dim temp As String = ActualResponse
					'Dim tmp As String = _actualResponse.Substring(startPos + 1, 6).Replace(".", "")
					Dim tmp As String = Mid(_actualResponse, startPos + 1, 25)
					tmp = tmp.Replace(".", "")
					Long.TryParse(tmp, actual)
					Long.TryParse(ExpectedResponse, expected)
					'Return actual <= expected
					Return actual >= expected
				Case Else
					Return True
			End Select
			Return True
		End Get
	End Property

End Class
