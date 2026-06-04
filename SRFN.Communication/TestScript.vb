Option Strict On

Public Class TestScript

	Public Property TestSuccessful As Boolean
	Public Property TestFilePath As String
	Public Property CalculatedHash As String
	Public Property ProductFamily As String
	Public Property Drawing As String
	Public Property CustomerName As String
	Public Property CustomerID As String
	Public Property HashValue As String
	Public Property ProgressMax As Integer
	Public Property LastStep As Integer
	Public Property TestScriptCommands As List(Of TestScriptCommand) = New List(Of TestScriptCommand)

	''' <summary>
	''' Used to set a existing test read commands expected response.
	''' </summary>
	''' <remarks>Should be called after loading test commands.</remarks>
	Public Sub SetMappedReadCommandResponse(ByVal commandName As String, ByVal expectedResponse As String)
		Dim command = TestScriptCommands.FirstOrDefault(Function(x) x.Command.CommandName = commandName AndAlso x.IsReadCommand)
		If command Is Nothing Then Return
		'command.ExpectedResponse = expectedResponse
		For x As Integer = 0 To TestScriptCommands.Count - 1
			If TestScriptCommands(x).Command.CommandName = commandName Then
				TestScriptCommands(x).ExpectedResponse = expectedResponse
			End If
		Next
	End Sub
	Public Sub SetMappedWriteCommand(ByVal commandName As String, ByVal commandValue As String)
		Dim command = TestScriptCommands.FirstOrDefault(Function(x) x.Command.CommandName = commandName <> x.IsReadCommand)
		If command Is Nothing Then Return
		For x As Integer = 0 To TestScriptCommands.Count - 1
			If TestScriptCommands(x).Command.CommandName = commandName AndAlso TestScriptCommands(x).Comm_Val <> "" Then
				TestScriptCommands(x).ExpectedResponse = commandValue
				TestScriptCommands(x).Comm_Val = commandValue
			End If
		Next
	End Sub
End Class
