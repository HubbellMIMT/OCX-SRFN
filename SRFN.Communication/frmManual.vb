Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Public Class frmManual
	Private WithEvents _commManager As CommManager2
	Private _script As TestScript

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub frmManual_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		_commManager = New CommManager2()
	End Sub

#Region "Comm Manager Events"

	Private Sub DisplayResults(ByVal msg As String, ByVal msgColor As Color, ByVal IsDisplay As Boolean)
		If IsDisplay Then
			rtbDisplay.SelectedText = String.Empty
			rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Bold)
			rtbDisplay.SelectionColor = msgColor
			rtbDisplay.AppendText(msg)
			rtbDisplay.ScrollToCaret()
		Else
			rtbResult.SelectedText = String.Empty
			rtbResult.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
			rtbResult.SelectionColor = msgColor
			rtbResult.AppendText(msg)
			rtbResult.ScrollToCaret()
		End If
	End Sub
	Private Sub Local_WriteDisplay(ByVal msg As String)
		rtbDisplay.Clear()
		rtbResult.Clear()
		rtbTestResults.Clear()
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
		rtbDisplay.SelectionColor = Color.Red
		rtbDisplay.AppendText(msg & vbCrLf)
		rtbDisplay.ScrollToCaret()
		Threading.Thread.Sleep(250)
		Me.Refresh()
	End Sub
	Private Sub Local_DataWritten(ByVal msg As String)
		rtbDisplay.Clear()
		rtbResult.Clear()
		rtbTestResults.Clear()
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Bold)
		rtbDisplay.SelectionColor = Color.Green
		rtbDisplay.AppendText(msg & vbCrLf)
		rtbDisplay.ScrollToCaret()
		rtbResult.AppendText(msg & vbCrLf)
		rtbResult.ScrollToCaret()
		Me.Refresh()
	End Sub
	Private Sub Local_DataReceived(ByVal msg As String)
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Bold)
		rtbDisplay.SelectionColor = Color.Blue
		rtbDisplay.AppendText(msg & vbCrLf)
		rtbDisplay.ScrollToCaret()
		rtbResult.AppendText(msg & vbCrLf)
		rtbResult.ScrollToCaret()
		Me.Refresh
	End Sub

	Private Sub Local_StatusMessagePosted(ByVal msg As String)
		'DisplayResults(msg, Color.Black)
	End Sub

	Private Sub Local_ErrorMessagePosted(ByVal msg As String)
		'DisplayResults(msg, Color.Red)
	End Sub

	Private Sub Local_TestProgressUpdated(ByVal progress As Integer)
		prgTestProgress.Value = progress
	End Sub

	Private Sub Local_TestProgressCompleted(ByVal script As TestScript)

		rtbTestResults.Text = rtbTestResults.Text.Replace("RETRY" & vbCrLf & vbCrLf, "")
		prgTestProgress.Value = 0
		rtbTestResults.Text += "Test Results - " & IIf(script.TestSuccessful, "TEST PASS", "FAIL").ToString() & vbCrLf
		rtbTestResults.ScrollToCaret()
		rtbTestResults.BackColor = CType(IIf(script.TestSuccessful, Color.Green, Color.Red), Color)

	End Sub

	Private Sub Local_TestCommandFailed(ByVal scriptCommand As TestScriptCommand)

		rtbTestResults.Clear()
		rtbTestResults.BackColor = Color.Yellow
		rtbTestResults.Text += "Test Step - " & scriptCommand.RefNum.ToString() & " " & scriptCommand.Command.CommandDesc & vbCrLf
		rtbTestResults.Text += "Outbnd......." & scriptCommand.Command.CommandName & vbCrLf
		rtbTestResults.Text += "Inbnd........" & scriptCommand.ActualResponse & vbCrLf
		rtbTestResults.Text += "Exp.........." & scriptCommand.ExpectedResponse & vbCrLf
		rtbTestResults.Text += "Time........." & Now.ToString("HH:mm:ss.fff") & vbCrLf
		rtbTestResults.Text += "Result.......FAIL" & vbCrLf & vbCrLf
		rtbTestResults.SelectionStart = rtbTestResults.Text.Length
		rtbTestResults.ScrollToCaret()
		Refresh()
		Threading.Thread.Sleep(1500)
	End Sub

	Private Sub Local_TestCommandSucceeded(ByVal scriptCommand As TestScriptCommand)

		rtbTestResults.Clear()
		rtbTestResults.BackColor = Color.White
		rtbTestResults.Text += "Test Step - " & scriptCommand.RefNum.ToString() & " " & scriptCommand.Command.CommandDesc & vbCrLf
		rtbTestResults.Text += "Outbnd......." & scriptCommand.Command.CommandName & vbCrLf
		rtbTestResults.Text += "Inbnd........" & scriptCommand.ActualResponse & vbCrLf
		rtbTestResults.Text += "Exp.........." & scriptCommand.ExpectedResponse & vbCrLf
		rtbTestResults.Text += "Time........." & Now.ToString("HH:mm:ss.fff") & vbCrLf
		rtbTestResults.Text += "Result.......PASS" & vbCrLf
		rtbTestResults.SelectionStart = rtbTestResults.Text.Length
		rtbTestResults.ScrollToCaret()
		Me.Refresh()
		Threading.Thread.Sleep(500)
	End Sub
	Private Sub Local_WaitingForSelfTestCommandReceived()
		If rtbDisplay.Text <> "Waiting for Micro Reset" & vbLf & "Self Test Completion" & vbCrLf Then
			rtbDisplay.Text = String.Empty
			rtbDisplay.SelectedText = String.Empty
			rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
			rtbDisplay.SelectionColor = Color.Red
			rtbDisplay.AppendText("Waiting for Micro Reset" & vbLf & "Self Test Completion" & vbCrLf)
			rtbDisplay.ScrollToCaret()
		End If
	End Sub

	Private Sub Local_SelfTestCommandReceived()
		ClearDisplays()
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Bold)
		rtbDisplay.SelectionColor = Color.Blue
		rtbDisplay.AppendText("SelfTest 0000" & vbCrLf & "SelfTest Pass" & vbCrLf)
		rtbDisplay.ScrollToCaret()

		rtbResult.AppendText("SelfTest 0000" & vbCrLf & "SelfTest Pass" & vbCrLf)
		rtbResult.ScrollToCaret()
		Me.Refresh()
		Threading.Thread.Sleep(1000)
		If _commManager.TestProgress = 0 Then _commManager.TestProgress += 1 '....SelfTest Completed, now getFirmware
	End Sub

	Private Sub CommManager_CheckQuietMode(ByVal msg As String, ByVal delay As Integer) Handles _commManager.EnablingQuietMode
		If Me.InvokeRequired Then
			Dim enablingquietmode As New DataWrittenEventHandler(AddressOf Local_WriteDisplay)
			Me.Invoke(enablingquietmode, New Object() {msg})
		Else
			Local_WriteDisplay(msg)
		End If
		Me.Refresh()
		Threading.Thread.Sleep(delay)
	End Sub
	Private Sub CommManager_DataWritten(ByVal msg As String) Handles _commManager.DataWritten
		If Me.InvokeRequired Then
			Dim dataWritten As New DataWrittenEventHandler(AddressOf Local_DataWritten)
			Me.Invoke(dataWritten, New Object() {msg})
		Else
			Local_DataWritten(msg)
		End If
	End Sub
	Private Sub CommManager_DataReceived(ByVal msg As String) Handles _commManager.DataReceived
		If Me.InvokeRequired Then
			Dim dataReceived As New DataReceivedEventHandler(AddressOf Local_DataReceived)
			Me.Invoke(dataReceived, New Object() {msg})
		Else
			Local_DataReceived(msg)
			If _commManager.TestProgress = 1 Then
				_commManager.TestProgress += 1 '....GetFirmware Complete
			End If
		End If
	End Sub
	Private Sub CommManager_StatusMessagePosted(ByVal msg As String) Handles _commManager.StatusMessagePosted
		If Me.InvokeRequired Then
			Dim statusMessagePosted As New StatusMessagePostedEventHandler(AddressOf Local_StatusMessagePosted)
			Me.Invoke(statusMessagePosted, New Object() {vbCrLf & msg & vbCrLf})
		Else
			'DisplayResults(vbCrLf + msg + vbCrLf, Color.Black)
		End If
	End Sub
	Private Sub CommManager_ErrorMessagePosted(ByVal msg As String) Handles _commManager.ErrorMessagePosted
		If Me.InvokeRequired Then
			Dim errorMessagePosted As New ErrorMessagePostedEventHandler(AddressOf Local_ErrorMessagePosted)
			Me.Invoke(errorMessagePosted, New Object() {vbCrLf & msg & vbCrLf})
		Else
			DisplayResults(vbCrLf & msg & vbCrLf, Color.Red, True)
		End If
	End Sub
	Private Sub CommManager_TestProgressUpdated(ByVal progress As Integer) Handles _commManager.TestProgressUpdated
		If Me.InvokeRequired Then
			Dim updateProgress As New TestProgressUpdatedEventHandler(AddressOf Local_TestProgressUpdated)
			Me.Invoke(updateProgress, New Object() {progress})
		Else
			Local_TestProgressUpdated(progress)
		End If
	End Sub
	Private Sub CommManager_TestProgressCompleted(ByVal script As TestScript) Handles _commManager.TestProgressCompleted
		If Me.InvokeRequired Then
			Dim testComplete As New TestProgressCompletedEventHandler(AddressOf Local_TestProgressCompleted)
			Me.Invoke(testComplete, New Object() {script})
		Else
			Local_TestProgressCompleted(script)
		End If
	End Sub
	Private Sub CommManager_TestCommandFailed(ByVal scriptCommand As TestScriptCommand) Handles _commManager.TestCommandFailed
		If Me.InvokeRequired Then
			Dim commFailed As New TestCommandFailedEventHandler(AddressOf Local_TestCommandFailed)
			Me.Invoke(commFailed, New Object() {scriptCommand})
		Else
			Local_TestCommandFailed(scriptCommand)
		End If
	End Sub
	Private Sub CommManager_TestCommandSucceeded(ByVal scriptCommand As TestScriptCommand) Handles _commManager.TestCommandSucceeded
		If Me.InvokeRequired Then
			Dim commSuccess As New TestCommandSucceededEventHandler(AddressOf Local_TestCommandSucceeded)
			Me.Invoke(commSuccess, New Object() {scriptCommand})
		Else
			Local_TestCommandSucceeded(scriptCommand)
		End If
	End Sub
	Private Sub CommManager_WaitingForSelfTestCommandReceived() Handles _commManager.WaitingForSelfTestCommandReceived
		If Me.InvokeRequired Then
			Dim waitForSelfTest As New WaitingForSelfTestCommandReceivedEventHandler(AddressOf Local_WaitingForSelfTestCommandReceived)
			Me.Invoke(waitForSelfTest)
		Else
			Local_WaitingForSelfTestCommandReceived()
		End If
	End Sub
	Private Sub CommManager_SelfTestCommandReceived() Handles _commManager.SelfTestCommandReceived
		If Me.InvokeRequired Then
			Dim selfTest As New SelfTestCommandReceivedEventHandler(AddressOf Local_SelfTestCommandReceived)
			Me.Invoke(selfTest)
		Else
			Local_SelfTestCommandReceived()
		End If
	End Sub
#Region "Verify QuietMode"
	Public Sub Local_CheckingQuietMode()
		rtbDisplay.Text = String.Empty
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
		rtbDisplay.SelectionColor = Color.Red
		rtbDisplay.AppendText("Test Step 0" & vbCrLf & "Checking Quiet Mode Enabled" & vbCrLf)
		rtbDisplay.ScrollToCaret()
		Threading.Thread.Sleep(1000)
	End Sub
	Private Sub Local_RecievedVarParams()
		rtbDisplay.Text = String.Empty
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
		rtbDisplay.SelectionColor = Color.Red
		rtbDisplay.AppendText("Test Step 0" & vbCrLf & "Checking Quiet Mode Enabled" & vbCrLf)
		rtbDisplay.ScrollToCaret()
		Threading.Thread.Sleep(1000)
	End Sub
	Private Sub Local_QuietModeEnabled()
		rtbDisplay.Text = String.Empty
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Regular)
		rtbDisplay.SelectionColor = Color.Red
		rtbDisplay.AppendText("Test Step 2" & vbCrLf & "Quiet Mode Enabled" & vbCrLf)
		rtbDisplay.ScrollToCaret()
		Threading.Thread.Sleep(1000)
	End Sub
#End Region
#End Region
#Region "Comm Actions"
	Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
		With _commManager
			.BarCode = txtBarcode.Text
			.BaudRate = CInt(cboBaud.SelectedItem)
			.PortName = cboPort.SelectedItem.ToString()
			.Parity = CType([Enum].Parse(GetType(Ports.Parity), cboParity.SelectedItem.ToString()), Ports.Parity)
			.StopBits = CType([Enum].Parse(GetType(Ports.StopBits), cboStop.SelectedItem.ToString()), Ports.StopBits)
			.DataBits = CInt(cboData.SelectedItem)
			.BarCode = txtBarcode.Text
			.IsHashing = ChkHash.Checked
			.IsText = rdoText.Checked
			.ProductFamily = txtProductFamily.Text
			If cboMeterForm.Items.Count = 0 Then .MeterForm = "" Else .MeterForm = cboMeterForm.SelectedItem.ToString()
			.CustomerName = txtCustomerName.Text
			.CustomerId = txtCustomerID.Text
			.TestFile = IIf(cboTestFile.Items.Count = 0, "", cboTestFile.SelectedItem.ToString()).ToString()
			.HashValue = txtHashVal.Text
			.MeterSerialNumber = txtMeterSerial.Text
			.UtilitySerialNumber = txtUtilitySerial.Text
			.MacAddress = txtMacAddress.Text
		End With

		If _commManager.OpenPort() Then
			btnQuietON.Enabled = True
			btnReadFirmwareManual.Enabled = True
			btnReadMacAddress.Enabled = True
			btnClosePort.Enabled = True
			cmdReset.Enabled = True
			btnOpen.Enabled = False
			txtSend.Focus()
			cmdSend.Enabled = True
		Else
			MsgBox("Unable to open port.")
		End If
	End Sub
	Private Sub btnClosePort_Click(sender As Object, e As EventArgs) Handles btnClosePort.Click
		_commManager.ClosePort()
		btnOpen.Enabled = True
		btnClosePort.Enabled = False
		btnReadFirmwareManual.Enabled = False
		btnReadMacAddress.Enabled = False
		btnQuietON.Enabled = False
		cmdSend.Enabled = False
		cmdReset.Enabled = False
	End Sub

	Private Async Sub btnQuietON_Click(sender As Object, e As EventArgs) Handles btnQuietON.Click
		ClearDisplays()
		If btnQuietON.Text = "Quite OFF" Then
			btnQuietON.Text = "Quite ON"
			btnQuietON.BackColor = Color.Transparent
			Await _commManager.WriteData("quietMode 0" & vbCrLf)
		Else
			btnQuietON.Text = "Quite OFF"
			Await _commManager.WriteData("quietMode 1" & vbCrLf)
			btnQuietON.BackColor = Color.Yellow
		End If
		ClearDisplays()
		txtSend.Focus()
	End Sub

	Private Async Sub btnQuietOFF_Click(sender As Object, e As EventArgs)
		ClearDisplays()
		txtSend.Focus()
		Await _commManager.WriteData("quietMode 0" & vbCrLf)
	End Sub

	Private Async Sub btnReadMacAddress_Click(sender As Object, e As EventArgs) Handles btnReadMacAddress.Click
		ClearDisplays()
		txtSend.Focus()
		Await _commManager.WriteData("comDeviceMACAddress" & vbCrLf)
	End Sub

	Private Async Sub btnReadFirmwareManual_Click(sender As Object, e As EventArgs) Handles btnReadFirmwareManual.Click
		ClearDisplays()
		txtSend.Focus()
		Await _commManager.WriteData("comDeviceFirmwareVersion" & vbCrLf)
	End Sub

	Private Async Sub cmdSend_Click(sender As Object, e As EventArgs) Handles cmdSend.Click
		ClearDisplays()
		txtSend.Focus()
		Await _commManager.WriteData(txtSend.Text & vbCrLf)
	End Sub

	Private Sub cmdReset_Click(sender As Object, e As EventArgs) Handles cmdReset.Click
		ClearDisplays()
		rtbDisplay.Text = String.Empty
		rtbDisplay.SelectedText = String.Empty
		rtbDisplay.SelectionFont = New Font(rtbDisplay.SelectionFont, FontStyle.Bold)
		rtbDisplay.SelectionColor = Color.Red
		rtbDisplay.AppendText("Test Step 0" & vbCrLf & "Power On Self Test" & vbCrLf & vbCrLf & "POWER UNIT ON" & vbCrLf)
		rtbDisplay.ScrollToCaret()
		txtSend.Focus()
	End Sub

#End Region

#Region "Test Commands"
	Private Async Sub btnRunTest_Click(sender As Object, e As EventArgs) Handles btnRunTest.Click
		ClearDisplays()
		btnResetTest.Enabled = True
		btnRunTest.Enabled = False
		btnOpen_Click(sender, e)
		_script.SetMappedReadCommandResponse("comDeviceMACAddress", txtMacAddress.Text)
		_script.SetMappedReadCommandResponse("edUtilitySerialNumber", txtUtilitySerial.Text)
		_script.SetMappedReadCommandResponse("edMfgSerialNumber", txtMeterSerial.Text)
		Await _commManager.ExecuteTestScript(_script)
	End Sub
	Private Sub btnResetTest_Click(sender As Object, e As EventArgs) Handles btnResetTest.Click
		_script = Nothing
		_commManager.RunTest = False
		btnRunTest.Enabled = False
		btnResetTest.Enabled = False
		txtProductFamily.Text = ""
		txtCustomerName.Text = ""
		txtCustomerID.Text = ""
		txtHashVal.Text = ""
		txtUtilitySerial.Text = ""
		txtMeterSerial.Text = ""
		txtMacAddress.Text = ""
		cboMeterForm.DataSource = Nothing
		lblTestFile.Text = "Test File"
		rtbDisplay.Text = String.Empty
		rtbResult.Text = String.Empty
		rtbTestResults.Text = String.Empty
		rtbTestResults.BackColor = Color.White
		txtMacAddress.Focus()
	End Sub
	Private Sub btnLoadTestScript_Click(sender As Object, e As EventArgs) Handles btnLoadTestScript.Click
		If chkManualScript.Checked Then
			'_script = _commManager.ReadManualScriptFile(cboTestFile.SelectedItem.ToString())
		Else
			'_script = _commManager.ReadTestScriptFile(cboTestFile.SelectedItem.ToString())
		End If
		txtProductFamily.Text = _script.ProductFamily
		txtCustomerName.Text = _script.CustomerName
		txtCustomerID.Text = _script.CustomerID
		txtHashVal.Text = _script.HashValue
		cboMeterForm.DataSource = _commManager.ReadFormList().FormListProducts.Where(Function(x) x.MeterType = _script.ProductFamily).ToList()
		If txtMacAddress.Text.Trim().Length = 0 Then
			txtMacAddress.BackColor = Color.Red
			txtMacAddress.Focus()
		Else
			txtMacAddress.BackColor = Color.White
			btnRunTest.Enabled = True
		End If
		lblTestFile.Text = "Test File (Loaded)"
	End Sub
	Private Sub chkManualScript_CheckedChanged(sender As Object, e As EventArgs) Handles chkManualScript.CheckedChanged
		If chkManualScript.Checked Then
			'cboTestFile.DataSource = _commManager.ReadManualScriptFiles
		Else
			'cboTestFile.DataSource = _commManager.ReadTestFiles
		End If
	End Sub
#End Region
#Region "Support Functions"

	Private Sub ClearDisplays()
		rtbDisplay.Text = ""
		rtbDisplay.BackColor = Color.White
		rtbResult.Text = ""
		rtbResult.BackColor = Color.White
		rtbTestResults.Text = ""
		rtbTestResults.BackColor = Color.White
	End Sub

	Private Sub txtMeterSerial_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtMeterSerial.KeyPress
		If Asc(e.KeyChar) = CInt(txtBarcode.Text) Then '13 is default
			Dim temp As String = txtMeterSerial.Text
			Do While temp.Length > 0
				If Not IsNumeric(Mid(temp, 1, 1)) Then
					txtMeterSerial.Text = ""
					Return
				Else
					temp = Mid(temp, 2)
				End If
			Loop
			txtMacAddress.Focus()
		End If
	End Sub

	Private Sub txtMacAddress_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtMacAddress.KeyPress
		lblMacScan.Text = txtMacAddress.Text  '13 is default
		Dim macad As String = ""
		If Asc(e.KeyChar) = CInt(txtBarcode.Text) Then
			macad = Mid(txtMacAddress.Text, InStrRev(lblMacScan.Text, Chr(32)) + 1)
			macad = Replace(macad, "-", "")
			macad = Replace(macad, " ", "")
			macad = Replace(macad, ":", "")
			macad = Replace(macad, ";", "")
			txtMacAddress.Text = macad
			lblMacScan.Text = "Unit Under Test - " & lblMacScan.Text

			StartSelfTest()
		End If
	End Sub

	Private Sub StartSelfTest()
		If txtMeterSerial.Text = "" OrElse txtMacAddress.Text = "" Then Return
		btnResetTest.BackColor = Color.Yellow
		lblMacScan.BackColor = Color.Yellow
	End Sub

	Private Sub cboTestFile_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestFile.SelectedIndexChanged
		txtProductFamily.Text = ""
		txtCustomerName.Text = ""
		txtHashVal.Text = ""
		txtCustomerID.Text = ""
		txtMeterSerial.Text = ""
		txtUtilitySerial.Text = ""
		txtMacAddress.Text = ""
		btnRunTest.Enabled = False
		btnResetTest.Enabled = False
	End Sub

	Private btnStartLocation As Point
	Private grpTestResultsLocation As Point
	Private originalFormSize As Size

	Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

		If btnStart.Text = "Start" Then
			btnStartLocation = btnStart.Location
			grpTestResultsLocation = grpTestResults.Location
			originalFormSize = Me.Size

			grpTestResults.Left = grpRawOutput.Left
			grpTestResults.Top = grpRawOutput.Top + grpRawOutput.Height + 16
			btnStart.Text = "<<< Back"
			Me.Height = Me.Height + grpTestResults.Height + 60
			btnStart.Left = grpTestResults.Left
			btnStart.Top = grpTestResults.Top + grpTestResults.Height + 16
			Me.Width = grpRawOutput.Left + grpRawOutput.Width + 20

			CenterForm()
			ClearDisplays()
			txtMacAddress.Text = ""
			txtMeterSerial.Text = ""
			txtUtilitySerial.Text = ""
			_script = _commManager.ReadTestScriptFile("Test.xml")

			btnOpen_Click(sender, e)

			Await _commManager.ExecuteTestScript(_script)
		Else
			btnStart.Text = "Start"
			btnStart.Location = btnStartLocation
			grpTestResults.Location = grpTestResultsLocation
			Me.Size = originalFormSize
			CenterForm()
			ClearDisplays()
		End If

	End Sub

	Private Sub CenterForm()
		Dim r As Rectangle
		If Parent IsNot Nothing Then
			r = Parent.RectangleToScreen(Parent.ClientRectangle)
		Else
			r = Screen.FromPoint(Me.Location).WorkingArea
		End If
		Dim x As Integer = r.Left + (r.Width - Me.Width) \ 2
		Dim y As Integer = r.Top + (r.Height - Me.Height) \ 2
		Me.Location = New Point(x, y)
	End Sub
#End Region
	Public Sub Reset()
		_commManager.ClosePort()
	End Sub
	''' <summary>
	''' This function is to test the communication with the devise on the specified port with given test parameters.
	''' </summary>
	''' <param name="varParam1">
	''' A string that is a comma delimited list of parameters that must be in the order:
	''' MeterSerialNumber, MacAddress, SQLName, UtilitySerialName
	''' "63940110,001D2400010002E1,STL7130D\MSSQLSERVER2012,123456"
	''' </param>
	''' <param name="varParam2">
	''' A string that is a comma delimited list of parameters that must be in the order:
	''' CustomerID, ProductFamily, FormID, RFCarrier, PackPath
	''' "31751500,SRFN-I-210+,01120,00,C:\Pack"
	''' </param>
	''' <param name="port">The com port number.</param>
	''' <returns>Pass/Fail as a string.</returns>
	Public Async Function Team_INTEGRATIONDotNetTest(ByVal port As Integer, ByVal varParam1 As String, ByVal varParam2 As String) As Task(Of String)
		grpTestResultsLocation = grpTestResults.Location
		originalFormSize = Me.Size
		Me.Height = 500
		Me.Width = 310
		GroupBox1.Height = 135
		GroupBox1.Width = 280
		grpRawOutput.Height = 135
		grpRawOutput.Width = 280  '.....Mid
		grpTestResults.Height = 150
		grpTestResults.Width = 280  '.....Bot
		rtbDisplay.Height = 100
		rtbDisplay.Width = 267
		rtbResult.Height = 100
		rtbResult.Width = 267
		rtbTestResults.Height = 150
		rtbTestResults.Width = 267
		CenterForm()
		ClearDisplays()
		txtMacAddress.Text = ""
		txtMeterSerial.Text = ""
		txtUtilitySerial.Text = ""
		Return Await _commManager.Team_INTEGRATIONDotNetTest(port, varParam1, varParam2)
	End Function
	Private Sub txtMacAddress_TextChanged(sender As Object, e As EventArgs) Handles txtMacAddress.TextChanged
		If txtMacAddress.Text.Trim().Length = 0 Then
			txtMacAddress.BackColor = Color.Red
			btnRunTest.Enabled = False
		Else
			txtMacAddress.BackColor = Color.White
			If _script IsNot Nothing Then btnRunTest.Enabled = True
		End If
	End Sub
End Class
