Option Strict On
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml

Public Delegate Sub DataWrittenEventHandler(ByVal msg As String)
Public Delegate Sub DataReceivedEventHandler(ByVal msg As String)
Public Delegate Sub StatusMessagePostedEventHandler(ByVal msg As String)
Public Delegate Sub ErrorMessagePostedEventHandler(ByVal msg As String)
Public Delegate Sub TestProgressUpdatedEventHandler(ByVal progress As Integer)
Public Delegate Sub TestProgressCompletedEventHandler(ByVal script As TestScript)
Public Delegate Sub TestCommandFailedEventHandler(ByVal scriptCommand As TestScriptCommand)
Public Delegate Sub TestCommandSucceededEventHandler(ByVal scriptCommand As TestScriptCommand)
Public Delegate Sub SelfTestCommandReceivedEventHandler()
Public Delegate Sub WaitingForSelfTestCommandReceivedEventHandler()

''' <summary>
''' This is the class responsible for interacting with a COM Port.
''' </summary>
Public Class CommManager2
	Implements IDisposable

	'============================================== Global log callback set by Form1
	Public Shared LogAction As Action(Of String)
	Public Shared BatchLogAction As Action(Of String)
	Public Shared ColorAction As Action(Of Color)
	Public Shared MassEraseAction As Func(Of Task(Of Boolean))
	Public Shared UpdateFirmwareAction As Func(Of String, Task(Of Boolean))
	Public Shared ToggleUSBAction As Func(Of Integer, Task)
	Public Shared VerifyUSBCommAction As Func(Of Task)
	Public Shared StopAtStep As Integer = -1
	'--- RA6 / relay config (set by Form1 on startup; DLL reads these for self-contained execution)
	Public Shared RA6ProgPath As String = ""
	Public Shared RelayPort As String = ""
	Public Shared UsbIsOn As Boolean = False
	Public Shared SqlServerName As String = ""
	Public Shared CurrentProductFamily As String = ""
	Public Shared FirmwareConnStr As String = ""
	Public Shared CustomerValuesConnStr As String = ""
	Public Shared TestResultsConnStr As String = ""
	Public Shared DLLRevisionConnStr As String = ""
	Public Shared FallbackFirmwareVersion As String = ""
	Private Shared _relay As New USBRelay()
	Shared Sub New()
		MassEraseAction = AddressOf ExecMassErase
		UpdateFirmwareAction = AddressOf ExecUpdateFirmware
		ToggleUSBAction = AddressOf ExecToggleUSB
		VerifyUSBCommAction = AddressOf ExecVerifyUSBComm
		Try
			Dim fv As New XmlDocument()
			fv.Load(Application.StartupPath & "\FormValues.xml")
			Dim cf As XmlNode = fv.SelectSingleNode("/Data/CurrentForm")
			If cf IsNot Nothing Then
				Dim n As XmlNode
				n = cf.SelectSingleNode("RA6ProgPath") : If n IsNot Nothing Then RA6ProgPath = n.InnerText
				n = cf.SelectSingleNode("relayport") : If n IsNot Nothing Then RelayPort = n.InnerText
				n = cf.SelectSingleNode("txtProductFamily") : If n IsNot Nothing Then CurrentProductFamily = n.InnerText
			End If
		Catch
		End Try
		Try
			Dim sv As New XmlDocument()
			sv.Load(Application.StartupPath & "\SQLValues.xml")
			Dim n As XmlNode
			n = sv.SelectSingleNode("/Data/Database/txtSQLServer")       : If n IsNot Nothing Then SqlServerName = n.InnerText
			n = sv.SelectSingleNode("/Data/Database/SRFN_CustomerValues") : If n IsNot Nothing Then CustomerValuesConnStr = n.InnerText
			n = sv.SelectSingleNode("/Data/Database/SRFN_TestResults")    : If n IsNot Nothing Then TestResultsConnStr = n.InnerText
			n = sv.SelectSingleNode("/Data/Database/DLL_Revision")        : If n IsNot Nothing Then DLLRevisionConnStr = n.InnerText
		Catch
		End Try
	End Sub
	Public Shared Sub BatchLog(msg As String)
		Try
			If BatchLogAction IsNot Nothing Then BatchLogAction(msg)
		Catch
		End Try
	End Sub
	Private Shared Sub CommLog(msg As String)
		Try
			If LogAction IsNot Nothing Then
				LogAction(msg)
			Else
				Dim logDir As String = IO.Path.Combine(Application.StartupPath, "Logs")
				IO.Directory.CreateDirectory(logDir)
				IO.File.AppendAllText(IO.Path.Combine(logDir, "ErrorLog_" & Now.ToString("yyyyMMdd") & ".txt"), "[" & Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & "] " & msg & Environment.NewLine)
			End If
		Catch
		End Try
	End Sub
	Public Event EnablingQuietMode(ByVal msg As String, ByVal wait As Integer)
	Public Event DataWritten(ByVal msg As String)
	Public Event DataReceived(ByVal msg As String)
	Public Event StatusMessagePosted(ByVal msg As String)
	Public Event ErrorMessagePosted(ByVal msg As String)
	Public Event TestProgressUpdated(ByVal progress As Integer)
	Public Event TestProgressCompleted(ByVal script As TestScript)
	Public Event TestCommandFailed(ByVal scriptCommand As TestScriptCommand)
	Public Event TestCommandSucceeded(ByVal scriptCommand As TestScriptCommand)
	Public Event SelfTestCommandReceived()
	Public Event EnteringquietmodeCommandReceived()
	Public Event WaitingForSelfTestCommandReceived()
	Public Event WaitingForEnteringquietmodeReceived()
	Private _TestFail As Boolean = True
	Public FailMsg As String = ""
	Private _WaitToStart As Integer = 0
	Public halt As Integer
	Public logit As Boolean = False
	Public dcnDB As New ADODB.Connection
	Public rsData As New ADODB.Recordset
	Public DLLver As String = ""
	Public outtime As String
	Public intime As String
	Public Sub New()
		DebugPort = New SerialPort()
		ComPort = New SerialPort()

		With ComPort
			'DefaultBaudRate = "38400"
			'.DefaultBaudRate = GetBaudRate()
			'.BaudRate = Integer.Parse(BaudRate)
			.BaudRate = GetBaudRate()
			.Parity = CType([Enum].Parse(GetType(Ports.Parity), DefaultParity), Ports.Parity)
			.StopBits = CType([Enum].Parse(GetType(Ports.StopBits), DefaultStopBits), Ports.StopBits)
			.DataBits = CInt(DefaultDataBits)
		End With
		GetPortOptions()
		AddHandler ComPort.DataReceived, AddressOf ReadData
		AddHandler ComPort.ErrorReceived, AddressOf ErrorReceived

		With DebugPort
			DefaultBaudRate = "115200"
			.Parity = CType([Enum].Parse(GetType(Ports.Parity), DefaultParity), Ports.Parity)
			.StopBits = CType([Enum].Parse(GetType(Ports.StopBits), DefaultStopBits), Ports.StopBits)
			.DataBits = CInt(DefaultDataBits)
		End With
		AddHandler DebugPort.DataReceived, AddressOf ReadDebugData
		AddHandler DebugPort.ErrorReceived, AddressOf ErrorReceived
	End Sub
	Public Shared Function DLLVersion As String
		Dim myFileVersionInfo As FileVersionInfo = FileVersionInfo.GetVersionInfo(Reflection.[Assembly].GetExecutingAssembly().Location)
		if myFileVersionInfo.FileVersion.Contains("v") Then
			Return myFileVersionInfo.FileVersion
		Else
			Return "v" & myFileVersionInfo.FileVersion
		End If
	End Function
	Private Function GetBaudRate() as Integer
		GetBaudRate = 38400
		Try
			Dim doc As New XmlDocument()
			Dim xmldoc As String = Application.StartupPath & "\FormValues.xml"
			doc.Load(xmldoc)
			Dim nodes = doc.SelectNodes("/Data/CurrentForm")
			Dim node As XmlNode
			For each node In nodes
				Dim nodeid As XmlNode = node.SelectSingleNode("BaudRate")
				GetBaudRate = Cint(nodeid.InnerText)
			Next
		Catch ex As Exception
			CommLog("GetBaudRate: " & ex.ToString)
		End Try
	End Function
	Private Sub GetPortOptions()
		Try
			Dim doc As New XmlDocument()
			Dim xmldoc As String = Application.StartupPath & "\PortSettings.xml"
			doc.Load(xmldoc)
			Dim formnodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/PortSettings")
			For Each node As XmlNode In formnodes
				'BaudRate = Cint(node.SelectSingleNode("BaudRate").InnerText)
				Me.Parity = CType([Enum].Parse(GetType(Ports.Parity), node.SelectSingleNode("Parity").InnerText), Ports.Parity)
				Me.StopBits = CType([Enum].Parse(GetType(Ports.StopBits), node.SelectSingleNode("Stop").InnerText), Ports.StopBits)
				DataBits = Cint(node.SelectSingleNode("Data").InnerText)
				'HashValue =  node.SelectSingleNode("Hash").InnerText
			Next
		Catch ex As Exception
			CommLog("GetPortOptions: " & ex.ToString)
		End Try
	End Sub
	Public Sub New(ByVal baud As String, ByVal par As String, ByVal sBits As String, ByVal dBits As String, ByVal name As String)

		If Not Integer.TryParse(baud, BaudRate) Then RaiseEvent ErrorMessagePosted("Error: Invalid BaudRate") : Return
		Me.Parity = CType([Enum].Parse(GetType(Ports.Parity), par), Ports.Parity)
		Me.StopBits = CType([Enum].Parse(GetType(Ports.StopBits), sBits), Ports.StopBits)
		'If Not StopBits.TryParse(Of StopBits)(sBits, StopBits) Then RaiseEvent ErrorMessagePosted("Error: Invalid StopBits") : Exit Sub
		If Not Integer.TryParse(dBits, DataBits) Then RaiseEvent ErrorMessagePosted("Error: Invalid DataBits") : Return
		If Not SerialPort.GetPortNames().Contains(name) Then
			RaiseEvent ErrorMessagePosted("Error: Invalid PortName")
			Return
		Else
			PortName = name
		End If
		ComPort = New SerialPort(PortName, BaudRate, Parity, DataBits, StopBits)
		AddHandler ComPort.DataReceived, AddressOf ReadData
		AddHandler ComPort.ErrorReceived, AddressOf ErrorReceived
	End Sub
#Region "RA6 Module" '........From Line 303 Test Start
	Public Async Function RA6_Command(ByVal script As TestScript, ByVal x As Integer) As Task
		Dim RA6FilesPath as string = Application.StartupPath & "\RA6Files"
		Dim CommandName As String = script.TestScriptCommands(x).Command.CommandName
		Dim Com As String() = Split(script.TestScriptCommands(x).Comm_Val, "|")
		Dim msg As String = Com(0)
		Dim delay As integer = CInt(Com(1))
		msg = msg.Replace("RA6FilesPath", RA6FilesPath)
		If InStr(msg, "rfp-cli -t E2l -d RA -if uart -s 1.5M -a -file") > 0 Then '............ Must Run virgindelay
			'Await SendMessage("VirginDelay", CInt(Regex.Replace(PortName, "[COM]", string.Empty)), ProductFamily)
			'Await Task.Delay(2000)
		End If
		RaiseEvent DataWritten(script.TestScriptCommands(x).Command.CommandDesc)
		Try
			If Not RunTest Then CurrentResponse = String.Empty
			outtime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
			Dim info As New ProcessStartInfo
			info.FileName = "C:\Windows\System32\cmd.exe"
			info.Arguments = " /c cd " & RA6ProgPath & " & " & msg & " -run"
			info.UseShellExecute = False
			info.CreateNoWindow = True
			Dim myProcess As Process = Process.Start(info)
			Threading.Thread.Sleep(delay)
			Await TaskDelay()

			Select Case CommandName '........... Command Response
				Case "RA6_ResetRelease"
					CurrentResponse = "Pass"
					RaiseEvent DataWritten("RA6 ResetRelease ENABLED")
				Case "RA6_DeployEnable"
					CurrentResponse = "Pass"
					RaiseEvent DataWritten("Deploy ENABLED")
				Case "RA6_FWUD_KEYS"
					script.TestScriptCommands(x).Comm_Val = "RA6_FWUD_KEYS"
					RaiseEvent DataWritten(script.TestScriptCommands(x).ExpectedResponse & " - Written")
			End Select
			intime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)

		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error" & ex.Message & System.Environment.NewLine &
				"WriteRA6Data(msg = " & msg & ", isTextCommand = " & IsText.ToString() & ")")
		End Try
	End Function
	Private CurrentProcessID As Integer = -1
	Public Async Function RA6Message(ByVal msg As String, ByVal ProgPath As String, ByVal delay As integer) As Task(Of String)
		Try
			Dim info As New ProcessStartInfo
			info.FileName = "C:\Windows\System32\cmd.exe"
			info.Arguments = " /c cd " & progpath & " & " & msg & " -run"
			info.UseShellExecute = False
			info.CreateNoWindow = True
			Dim myProcess As Process = Process.Start(info)
			Threading.Thread.Sleep(delay)
			Return "Pass"
		Catch ex As Exception
			Return "Fail"
		End Try
	End Function
	Private Sub BeginInvoke(methodInvoker As MethodInvoker)
		Throw New NotImplementedException()
	End Sub
	Public Async Function RA6Module() As Task(Of String)
		Dim retry As Boolean = True
		Try
			While retry = True
				retry = False
				Await RA6Commands("comDeviceFirmwareVersion" & vbCrLf)
			End While
		Catch ex As Exception
		End Try
		Return "Fail"
	End Function
	Public Async Function RA6Commands(ByVal msg As String) As Task
		Try
			If Not RunTest Then CurrentResponse = String.Empty
			outtime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
			CommLog(“TX(RA6): “ & msg.TrimEnd())
			RaiseEvent DataWritten(msg)
			Await TaskDelay()
			RaiseEvent DataReceived(CurrentResponse)
			CommLog(“RX(RA6): “ & CurrentResponse.Replace(vbCrLf, “|”).Trim())
			intime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted(“Error: “ & ex.Message & System.Environment.NewLine &
				“WriteData(msg = “ & msg & “, isTextCommand = “ & IsText.ToString() & “)”)
		End Try
	End Function
#End Region '............................. END RA6 MODULE REGION
#Region "Tests"
	''' <summary>
	''' Function to test communication with the device on the specified port with given test parameters.
	''' </summary>
	''' <param name="varParam1">
	''' A string that is a comma delimited list of parameters that must be in the order:
	''' MeterSerialNumber, MacAddress, SQLName, UtilitySerialName
	''' "63940110,001D2400010002E1,STL7130D\MSSQLSERVER2012,123456"
	''' </param>
	''' <param name="varParam2">
	''' A string that is a comma delimited list of parameters that must be in the order:
	''' CustomerID, ProductFamily, FormID, RFCarrier, PackPath, RDInstalled
	''' "31751500,SRFN-I-210+,01120,00,C:\Pack",True
	''' </param>
	''' <param name="port">The com port number.</param>
	''' <returns>Pass/Fail as a string.</returns>
	Public Async Function Team_INTEGRATIONDotNetTest(ByVal port As Integer, ByVal varParam1 As String, ByVal varParam2 As String) As Task(Of String)
		Dim errcode As String = ""
		Dim macaddr As String = ""
		Dim failreason As String = ""
		Dim firmware As String = ""
		Try
			RunTest = True
			Dim params1 = varParam1.Split(New Char() {","c}, StringSplitOptions.None) 'RemoveEmptyEntries)
			Dim params2 = varParam2.Split(New Char() {","c}, StringSplitOptions.None) 'RemoveEmptyEntries)
			If params2(2) = " " Then params2(2) = "0"
			Dim paramDef = New With
			{
			.MeterSerialNumber = params1(0),
			.MacAddress = params1(1),
			.SQLName = "",'params1(2),
			.UtilitySerialNumber = params1(3),
			.CustomerID = Trim(params2(0)),
			.ProductFamily = params2(1),
			.FormID = FormIDReConvert(params2(1), params2(2), params2(5)),
			.RFCarrier = Trim(params2(3)),
			.PackPath = params2(4),
			.RDInstalled = params2(5)
			}
			Dim mac As String = paramDef.MacAddress
			Dim met As String = paramDef.MeterSerialNumber
			Dim uti As String = paramDef.UtilitySerialNumber
			If UtilitySerialNumber = "debug" then logit = true '...........Log failures
			If Not Directory.Exists(paramDef.PackPath) Then
				Directory.CreateDirectory(paramDef.PackPath)
			End If
			PortName = "COM" & port.ToString()
			If ComPort.IsOpen Then ComPort.Close()
			Dim NewPackPath As String = paramDef.PackPath & "\" & Format(Date.Now(), "MM") & "-" & Format(Date.Now(), "yyyy")

			If Not Directory.Exists(NewPackPath) Then
				Directory.CreateDirectory(NewPackPath)
			End If
			'...................................................................... 
			If SetBaud(params2(1)) = False Then
				WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, "Unable to Set Baud")
				Return "Fail,0,Unable to Set Baud"
			End If
			If OpenPort() = False Then
				Dim portErr As String = "Unable to Open Port - " & PortName & ": " & FailMsg
				WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, portErr)
				Return "Fail,0," & portErr
			End If
			'...................................................................... check if DLL is allowed via SQL Query
			If GetDLLRevision(DLLversion) = False Then
				ClosePort()
				WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, "DLL Rev not found")
				Return "Fail,1,DLL Rev not found"
			End If
			'...................................................................... DLL is allowed
			'...................................................................... Firmware must be zero padded - 03.xx.yyzz
			'If paramDef.RFCarrier.Length = 9 then '................................ Firmware must be Length 10d with leading Zero (04.00.0076)
			'	paramDef.RFCarrier = "0" & paramDef.RFCarrier
			'	firmware = paramDef.RFCarrier
			'Else
				firmware = Await GetFirmwareVersion(params1(3))
				CurrentResponse = String.Empty
				If firmware.Contains("Fail") Then
					ClosePort()
					WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, "Read Firmware FAILED")
					Return "Fail,3,Read Firmware FAILED"
				End If
			'End If
			'...................................................................... check if SQL Insert Exists, if yes get Hash Value
			Dim sqlhash As String = SQLHashVal(params2(1), params2(0), firmware)
			'...................................................................... if sqlhash = "" then script does not exist
			If sqlhash = "" Then
				ClosePort()
				WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, "Read SQLHashVal Fail - No record found for " & params2(1) & " / " & params2(0) & " / " & firmware)
				Return "Fail,4,Read SQLHashVal Fail- No record found"
			End If
			'...................................................................... Verify - XML exists AND xmlHash = sqlHash
			If Not xmlHashVal(params2(1), params2(0), firmware) = sqlhash Then '... if NOT, delete xml and retrieve from SQL
				If GetSQLCustomerScript(params2(1), params2(0), firmware, sqlhash) = False Then
					ClosePort()
					WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, "Local Script & SQL Insert not found for " & firmware & " / " & params2(1) & " / " & params2(0))
					Return "Fail,5,Local Script & SQL Insert not found"
				End If
			End If
			Dim filename As String = firmware & "-" & params2(1) & "-" & params2(0) & ".xml"
			Threading.Thread.Sleep(100)
			PortName = "COM" & port.ToString()
			If ComPort.IsOpen Then ComPort.Close()
			If OpenPort() Then
				Dim script = ReadTestScriptFile(filename)
				script.SetMappedReadCommandResponse("comDeviceMACAddress", paramDef.MacAddress)
				script.SetMappedReadCommandResponse("edUtilitySerialNumber", paramDef.UtilitySerialNumber)
				script.SetMappedReadCommandResponse("edMfgSerialNumber", paramDef.MeterSerialNumber)
				script.SetMappedReadCommandResponse("edInfo", paramDef.FormID)

				script.SetMappedWriteCommand("edInfo", paramDef.FormID)
				script.SetMappedWriteCommand("edUtilitySerialNumber", paramDef.UtilitySerialNumber)
				script.SetMappedWriteCommand("edMfgSerialNumber", paramDef.MeterSerialNumber)

				script = Await ExecuteTestScript(script)
				Dim y As Integer = script.TestScriptCommands.Count
				Dim result As New StringBuilder()
				If paramDef.MacAddress = "" Then paramDef.MacAddress = MacAddress
				Dim packFile = paramDef.ProductFamily & "_" & script.CustomerName & " " & paramDef.MacAddress & ".txt"
				Dim resultFile = "SRFN_Results_" & paramDef.ProductFamily & "_" & Format(Date.Now(), "MMM") & "-" & Format(Date.Now(), "yyyy") & ".txt"

				Dim testResults = New StringBuilder()
				testResults.Append(vbCrLf & vbCrLf & "============= NEW TEST =============" & vbCrLf)
				testResults.Append("Test Date:    " & Now() & " " & vbCrLf &
			   "Test File:    " & Mid(script.TestFilePath, InStrRev(script.TestFilePath, "\") + 1) & vbCrLf &
			   "Drawing:      " & script.Drawing & vbCrLf &
			   "Hash Value:   " & script.HashValue & vbCrLf &
			   "Product:      " & script.ProductFamily & vbCrLf &
			   "Meter Form:   " & paramDef.FormID & vbCrLf &
			   "Customer:     " & script.CustomerName & vbCrLf &
			   "CustomerID:   " & paramDef.CustomerID & vbCrLf &
			   "Meter Serial: " & paramDef.MeterSerialNumber & vbCrLf &
			   "MACID:        " & paramDef.MacAddress & vbCrLf &
			   "DLLVer:       " & DLLVersion & vbCrLf &
			   "TestPC:       " & Environment.MachineName & vbCrLf)
				macaddr = paramDef.MacAddress
				'refnum = script.TestScriptCommands.Where(Function(x) x.IsSuccess).ToList().Count
				'If script.TestSuccessful = True Then refnum = refnum - 1
				For x As Integer = 0 To script.LastStep
				Next

				For x As Integer = 0 To script.LastStep 'refnum
					errcode = script.TestScriptCommands(x).ErrCode.ToString
					testResults.Append("-------------------------------------------" & vbCrLf)
					testResults.Append("Step " & script.TestScriptCommands(x).RefNum.ToString() & "......." & vbCrLf &
					   "Desc........." & script.TestScriptCommands(x).Command.CommandDesc & vbCrLf &
					   "Outbnd......." & script.TestScriptCommands(x).Command.CommandName & vbCrLf &
					   "Inbnd........" & script.TestScriptCommands(x).ActualResponse & vbCrLf &
					   "Exp.........." & script.TestScriptCommands(x).ExpectedResponse & vbCrLf &
					   "OutbTime....." & script.TestScriptCommands(x).OutbTime & vbCrLf &
					   "InbTime......" & script.TestScriptCommands(x).InbTime & vbCrLf &
					   "Result......." & IIf(script.TestScriptCommands(x).IsSuccess, "PASS", "FAIL").ToString() & vbCrLf)

					If script.TestScriptCommands(x).IsSuccess = False Then
						failreason = script.TestScriptCommands(x).Command.CommandName & vbCrLf & "Actual......" & script.TestScriptCommands(x).ActualResponse & vbCrLf & "Expected...." & script.TestScriptCommands(x).ExpectedResponse
					End If
				Next

				testResults.Append("============= END TEST =============" & vbCrLf & vbCrLf)
				If mac = "" Then mac = MacAddress
				SQLWriteTestResults(script.ProductFamily, IIf(script.TestSuccessful, "Pass", "Fail").ToString(), mac, met, uti, script.CustomerName, script.CustomerID, filename, script.HashValue, paramDef.FormID, script.TestScriptCommands(script.LastStep).RefNum.ToString(), script.TestScriptCommands(script.LastStep).ExpectedResponse, script.TestScriptCommands(script.LastStep).ActualResponse, DLLVersion)
				ComPort.Close()
				ComPort.Dispose()
				If macaddr = "" Then macaddr = MacAddress
				result.Append("Result:" & IIf(script.TestSuccessful, "Pass", "Fail").ToString() &
				"; Product:" & script.ProductFamily & "; TestDate:" & Now() & "; MacID:" & macaddr & "; MeterID:" & paramDef.MeterSerialNumber &
				"; UtilityID:" & paramDef.UtilitySerialNumber & "; Customer:" & script.CustomerName & "; CustomerID:" & script.CustomerID &
				"; TestFile:" & filename & "; HashVal:" & script.HashValue & "; Form:" & paramDef.FormID & "; RefNum:" & script.TestScriptCommands(script.LastStep).RefNum.ToString() &
				"; ExpResp:" & script.TestScriptCommands(script.LastStep).ExpectedResponse & "; ActResp:" & script.TestScriptCommands(script.LastStep).ActualResponse & "; Drawing:" & script.Drawing & "; DLLVer:" & DLLVersion & "; TestPC:" & Environment.MachineName & vbCrLf)

				My.Computer.FileSystem.WriteAllText(Path.Combine(paramDef.PackPath, resultFile), result.ToString, True)
				If script.TestSuccessful = True Then '....................... TEST PASS
					My.Computer.FileSystem.WriteAllText(Path.Combine(NewPackPath, packFile), testResults.ToString(), True)
					testResults.Clear()
					Return "Pass" & "," & script.TestScriptCommands(script.LastStep).RefNum.ToString() & "," & macaddr
				Else  '...................................................... TEST FAIL
					My.Computer.FileSystem.WriteAllText(Path.Combine(NewPackPath, packFile), testResults.ToString(), True)
					testResults.Clear()
					Return "Fail" & "," & script.TestScriptCommands(script.LastStep).RefNum.ToString() & "," & script.TestScriptCommands(script.LastStep).Command.CommandName & vbCrLf & " Act: " & script.TestScriptCommands(script.LastStep).ActualResponse & vbCrLf & " Exp: " & script.TestScriptCommands(script.LastStep).ExpectedResponse
				End If
			Else
				ComPort.Close()
				ComPort.Dispose()
				Dim portErr6 As String = "Unable to Open Port - " & PortName & ": " & FailMsg
				WriteFailToPackFile(paramDef.PackPath, NewPackPath, paramDef.ProductFamily, paramDef.MacAddress, met, portErr6)
				Return "Fail,6," & portErr6
			End If
		Catch ex As Exception
			If ComPort.IsOpen Then ComPort.Close()
			RaiseEvent ErrorMessagePosted("Error:  " & ex.Message)
			Return "Fail,0,Exception: " & ex.Message
		End Try
		'Return "Pass" & "," & "" & "," & macaddr
		ComPort.Close()
		ComPort.Dispose()
	End Function
	Private Sub WriteFailToPackFile(packPath As String, newPackPath As String, productFamily As String, macAddress As String, meterSerial As String, stepDesc As String)
		Try
			Dim resultFile As String = "SRFN_Results_" & productFamily & "_" & Format(Date.Now(), "MMM") & "-" & Format(Date.Now(), "yyyy") & ".txt"
			Dim packFile As String = productFamily & "_" & If(macAddress <> "", macAddress, "UNKNOWN") & ".txt"
			Dim entry As String =
				vbCrLf & "============= NEW TEST =============" & vbCrLf &
				"Test Date:    " & Now() & vbCrLf &
				"Product:      " & productFamily & vbCrLf &
				"Meter Serial: " & meterSerial & vbCrLf &
				"MACID:        " & macAddress & vbCrLf &
				"DLLVer:       " & DLLVersion & vbCrLf &
				"TestPC:       " & Environment.MachineName & vbCrLf &
				"Result:       FAIL - " & stepDesc & vbCrLf &
				"============= END TEST =============" & vbCrLf
			Dim resultEntry As String =
				"Result:Fail; Product:" & productFamily & "; TestDate:" & Now() &
				"; MacID:" & macAddress & "; MeterID:" & meterSerial &
				"; Error:" & stepDesc & "; DLLVer:" & DLLVersion &
				"; TestPC:" & Environment.MachineName & vbCrLf
			If Directory.Exists(packPath) Then
				My.Computer.FileSystem.WriteAllText(Path.Combine(packPath, resultFile), resultEntry, True)
			End If
			If Directory.Exists(newPackPath) Then
				My.Computer.FileSystem.WriteAllText(Path.Combine(newPackPath, packFile), entry, True)
			End If
		Catch
		End Try
	End Sub
	Private Function FormIDReConvert(byval ProdFamily As String, byval FrmID As String, byval RDInstalled As String) As String
		FormIDReConvert = ""
		Dim Prodtmp As String = ProdFamily
		If Prodtmp = "SRFN-I-210+c" Then Prodtmp = "SRFN-I-210C"
		If Prodtmp = "AclaraRF3-I210+c" Then Prodtmp = "AclaraRF3-I210C"
		Dim doc As New XmlDocument()
		Dim xmldoc As String = Application.StartupPath & "\FormList.xml"
		doc.Load(xmldoc)
		Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/" & Replace(Prodtmp, "+", ""))
		For Each node As XmlNode In nodes
			If FrmID = node.SelectSingleNode("FormID").InnerText Then
				FormIDReConvert = node.SelectSingleNode("MeterType").InnerText
				If RDInstalled = "True" Then
					FormIDReConvert = FormIDReConvert.Replace("S", "SRD")
					Return FormIDReConvert
				else
					Return FormIDReConvert
				End If
			End If
		Next
	End Function
	Private Function TestFailEarly(ByVal path As String) As Boolean

		TestFailEarly = True
	End Function
	Private Async Function CheckQuietModeEnabled() As Task
		While True
			If Not SelfTestReceived AndAlso RunTest Then
				Await Task.Delay(1000)
				RaiseEvent WaitingForEnteringquietmodeReceived()
			Else
				Return
			End If
		End While
	End Function
	Public Async Function QuietModeEnabled() As Task(Of Boolean)
		Try
			RaiseEvent EnablingQuietMode("Verifying" & vbCrLf & "QuietMode Enabled", 250)
			WriteWaitTime = 250
			Await WriteData("quietMode" & vbCrLf)
			If _currentResponse.Contains("quietMode 1") Then
				'_currentResponse = String.Empty
				ClosePort()
				Return True
			Else
				'_currentResponse = String.Empty
				ClosePort()
				Return False
			End If
		Catch ex As Exception
		End Try
		Return False
	End Function
	Public Async Function EnableQuietMode() As Task(Of Boolean)
		Try
			OpenPort()
			RaiseEvent EnablingQuietMode("QuietMode NOT Enabled" & vbCrLf & "Enabling QuietMode NOW", 1500)
			Await WriteData("quietMode 1" & vbCrLf)
			Threading.Thread.Sleep(500)
			If _currentResponse.Contains("Entering quiet mode") Then
				Threading.Thread.Sleep(6000)
				ClosePort()
				_currentResponse = String.Empty
				Return True
			End If
			ClosePort()
			_currentResponse = String.Empty
			Return False
		Catch ex As Exception
		End Try
		Return False
	End Function
	Public Async Function DisableQuietMode() As Task(Of Boolean)
		Try
			OpenPort()
			RaiseEvent EnablingQuietMode("Disabling QuietMode" & vbCrLf, 250)
			WriteWaitTime = 250
			Await WriteData("quietMode 0" & vbCrLf)
			If Await WaitForMicroResetSelfTestReceived() = True Then
			End If
			'Return True 
		Catch ex As Exception
		End Try
		Return True
	End Function
	Private Async Function WaitForMicroResetSelfTestReceived() As Task(Of Boolean)
		Dim cnt As Integer = 0
		While True
			If Not SelfTestReceived AndAlso RunTest Then
				If cnt > 20 Then
					ClosePort()
					_currentResponse = String.Empty
					Return False
				End If
				Await Task.Delay(500)
				RaiseEvent WaitingForSelfTestCommandReceived()
				cnt += 1
			Else
				ClosePort()
				_currentResponse = String.Empty
				Return True
			End If
		End While
		ClosePort()
		Return False
	End Function
	Private Async Function WaitForQuietModeEnabled() As Task(Of Boolean)
		Dim cnt As Integer = 0
		While True
			If Not EnteringQuietModeReceived AndAlso RunTest Then
				If cnt > 10 Then
					ClosePort()
					Return False
				End If
				Await Task.Delay(1000)
				RaiseEvent WaitingForEnteringquietmodeReceived()
				cnt += 1
			Else
				ClosePort()
				Return True
			End If
		End While
		ClosePort()
		Return False
	End Function
Public Async Function GetFirmwareVersion(Optional ByVal UtilSerial As String = "") As Task(Of String)

        Try
            WriteWaitTime = 500
            For i = 0 To 3
				If i > 0 Then Await Task.Delay(500)
				If ComPort.IsOpen Then ClosePort()
				OpenPort()
				CurrentResponse = ""
				Await WriteData("comDeviceFirmwareVersion" & vbCrLf)
				ClosePort()
				If UtilSerial = "DEBUG" Then MsgBox("GetFirmwareVersion:" & vblf & "'" & CurrentResponse & "'" & vblf & "Raw Response")
				CurrentResponse = CurrentResponse.Replace(vbCrLf, "").Replace("comDeviceFirmwareVersion", "").Replace(" ", "")
				If UtilSerial = "DEBUG" Then MsgBox("GetFirmwareVersion: " & vblf & "'" & CurrentResponse & "'" & vblf & "Parsed Response")
				If VerifyFirmware(CurrentResponse) = True Then Return CurrentResponse
            Next
        Catch ex As Exception
            If ComPort.IsOpen Then ComPort.Close()
        End Try
        Return "Fail"
    End Function
	Private Function VerifyFirmware(ByVal fw As String) As Boolean
		Try
			If fw.Length < 8 Then Return False
			For i As Integer = 1 To fw.Length
				If Asc(Mid(fw, i, 1)) > 57 OrElse Asc(Mid(fw, i, 1)) < 46 Then
					Return False
				End If
			Next
			Return True
		Catch ex As Exception
		End Try
		Return False
	End Function
	Public Async Function ExecuteTestScript(ByVal script As TestScript) As Task(Of TestScript)
		Try
			'If SelfTestReceived = True Then
			If Not ComPort.IsOpen AndAlso Not OpenPort() Then
				script.TestSuccessful = False
				Return script
			End If
			'End If
			RunTest = True
			'Await WaitForSelfTestCommandReceived()

			For x As Integer = 0 To script.TestScriptCommands.Count - 1
				script.TestScriptCommands(x).Attempts = 1
				If Not RunTest Then
					script.TestSuccessful = False
					Return script
				End If
				If StopAtStep > 0 AndAlso script.TestScriptCommands(x).RefNum > StopAtStep Then
					script.TestSuccessful = True
					RaiseEvent TestProgressCompleted(script)
					Return script
				End If
				script.LastStep = x
				CommLog("Step " & script.TestScriptCommands(x).RefNum.ToString() & " start: '" & script.TestScriptCommands(x).Command.CommandName & "' (loop x=" & x.ToString() & ")")
				WriteWaitTime = script.TestScriptCommands(x).Sleep
				If InStr(script.TestScriptCommands(x).Command.CommandName, "comDeviceFirmwareVersion") > 0 Then
					FallbackFirmwareVersion = script.TestScriptCommands(x).ExpectedResponse
					BatchLog("  > FallbackFirmwareVersion captured: '" & FallbackFirmwareVersion & "'")
				End If

				'.... add support for optical logon 9/22/17
				If script.TestScriptCommands(x).Command.CommandName.ToString = "OpticalLogon" Then
					Dim SetExtEncoding As Object = Encoding
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					Await WriteData("î" & ChrW(128) & vbNullChar & vbNullChar & vbNullChar & ChrW(14) & "P" & vbNullChar & ChrW(2) & "Aclara    " & vbCr & "FÜ" & vbLf)
				ElseIf Instr(script.TestScriptCommands(x).Command.CommandName, "RA6_VerifyFirmwareVersion") > 0 Then '........ RA6 COMMAND
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					If Await GetFirmwareVersion() = script.TestScriptCommands(x).ExpectedResponse Then '................. F/W READ matches EXPECTED F/W - Skip to stated test step Comm_val(0)
						Dim skipto As String() = Split(script.TestScriptCommands(x).Command.CommandName, "|")
						x = CInt(skipto(1))
						GoTo Jump :
					End If
				ElseIf Mid(script.TestScriptCommands(x).Command.CommandName.ToString, 1, 4) = “RA6_” Then '........ RA6 COMMAND
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					Await RA6_Command(script, x)
				ElseIf script.TestScriptCommands(x).Command.CommandName = “MassErase_RA6” Then
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					CurrentResponse = String.Empty
					RaiseEvent DataWritten(“MassErase_RA6”)
					Await Task.Delay(500)
					Await WriteData(“virgindelay” & vbCrLf)
					Dim vdStart As DateTime = DateTime.Now
					While Not CurrentResponse.Contains(“Signature erased”) AndAlso (DateTime.Now - vdStart).TotalSeconds < 10
						Await Task.Delay(100)
					End While
					Await Task.Delay(500)
					Dim eraseOk As Boolean = False
					If MassEraseAction IsNot Nothing Then eraseOk = Await MassEraseAction()
					script.TestScriptCommands(x).InbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					If eraseOk Then
						script.TestScriptCommands(x).ActualResponse = “OK”
						script.TestScriptCommands(x).ExpectedResponse = “OK”
						RaiseEvent TestCommandSucceeded(script.TestScriptCommands(x))
						RaiseEvent TestProgressUpdated(CInt(((x + 1) / script.TestScriptCommands.Count) * 100))
						If script.TestScriptCommands(x).Sleep > 0 Then Await Task.Delay(script.TestScriptCommands(x).Sleep)
					Else
						script.TestScriptCommands(x).ActualResponse = “FAIL”
						RaiseEvent TestCommandFailed(script.TestScriptCommands(x))
						Dim erase_cmd As TestScriptCommand = script.TestScriptCommands(x)
						If erase_cmd.Retries > 0 AndAlso erase_cmd.RetriesUsed < erase_cmd.Retries Then
							CommLog(“RETRY(MassErase) RetriesUsed=” & erase_cmd.RetriesUsed.ToString() & “/” & erase_cmd.Retries.ToString() & “ -> jump RefNum=” & erase_cmd.RetryJumpToStep.ToString())
							erase_cmd.RetriesUsed += 1
							BatchLog(“  > Retry MassErase (“ & erase_cmd.RetriesUsed.ToString() & “/” & erase_cmd.Retries.ToString() & “)”)
							If erase_cmd.RetryAction = “USBToggle” AndAlso ToggleUSBAction IsNot Nothing Then
								BatchLog(“  > USB Toggle (MassErase retry)”)
								Await ToggleUSBAction(erase_cmd.RetryDelayMs)
							End If
							If erase_cmd.RetryJumpToStep >= 0 Then
								Dim eraseJumpIdx As Integer = script.TestScriptCommands.FindIndex(Function(c) c.RefNum = erase_cmd.RetryJumpToStep)
								If eraseJumpIdx >= 0 Then
									x = eraseJumpIdx - 1
									GoTo Jump
								End If
							End If
						End If
						RunTest = False
						script.TestSuccessful = False
						RaiseEvent TestProgressCompleted(script)
						Return script
					End If
					GoTo Jump
				ElseIf script.TestScriptCommands(x).Command.CommandName = “Update Firmware” Then
					' Safety: if capture from comDeviceFirmwareVersion didn't fire, search the script now
					If FallbackFirmwareVersion = “” Then
						Dim fvStep = script.TestScriptCommands.Find(Function(c) InStr(c.Command.CommandName, “comDeviceFirmwareVersion”) > 0)
						If fvStep IsNot Nothing Then
							FallbackFirmwareVersion = fvStep.ExpectedResponse
							BatchLog(“  > FallbackFirmwareVersion (late capture): '” & FallbackFirmwareVersion & “'”)
						End If
					End If
					BatchLog(“  > FallbackFirmwareVersion at Update Firmware: '” & FallbackFirmwareVersion & “'”)
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					CurrentResponse = String.Empty
					RaiseEvent DataWritten(“Update Firmware “ & script.TestScriptCommands(x).Comm_Val)
					Await Task.Delay(500)
					Dim fwOk As Boolean = False
					If UpdateFirmwareAction IsNot Nothing Then fwOk = Await UpdateFirmwareAction(script.TestScriptCommands(x).Comm_Val)
					script.TestScriptCommands(x).InbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					If fwOk Then
						script.TestScriptCommands(x).ActualResponse = “OK”
						script.TestScriptCommands(x).ExpectedResponse = “OK”
						RaiseEvent TestCommandSucceeded(script.TestScriptCommands(x))
						RaiseEvent TestProgressUpdated(CInt(((x + 1) / script.TestScriptCommands.Count) * 100))
						If script.TestScriptCommands(x).Sleep > 0 Then Await Task.Delay(script.TestScriptCommands(x).Sleep)
					Else
						script.TestScriptCommands(x).ActualResponse = “FAIL”
						RaiseEvent TestCommandFailed(script.TestScriptCommands(x))
						Dim fw_cmd As TestScriptCommand = script.TestScriptCommands(x)
						If fw_cmd.Retries > 0 AndAlso fw_cmd.RetriesUsed < fw_cmd.Retries Then
							fw_cmd.RetriesUsed += 1
							BatchLog(“  > UpdateFirmware failed — reflashing fallback: “ & FallbackFirmwareVersion & “ (“ & fw_cmd.RetriesUsed.ToString() & “/” & fw_cmd.Retries.ToString() & “)”)
							If ColorAction IsNot Nothing Then ColorAction(Color.Red)
							If fw_cmd.RetryAction = “USBToggle” AndAlso ToggleUSBAction IsNot Nothing Then
								BatchLog(“  > USB Toggle (UpdateFirmware retry)”)
								Await ToggleUSBAction(fw_cmd.RetryDelayMs)
							End If
							Dim fallbackHex As String = If(FallbackFirmwareVersion <> “”, DownloadFirmwareHexByVersion(FallbackFirmwareVersion, CurrentProductFamily), “”)
							Dim fallbackOk As Boolean = fallbackHex <> “” AndAlso Await FlashHex(fallbackHex)
							If fallbackOk Then
								BatchLog(“  > Fallback reflash OK — retrying firmware update”)
								If ColorAction IsNot Nothing Then ColorAction(Color.Yellow)
								If fw_cmd.RetryJumpToStep >= 0 Then
									Dim fwJumpIdx As Integer = script.TestScriptCommands.FindIndex(Function(c) c.RefNum = fw_cmd.RetryJumpToStep)
									If fwJumpIdx >= 0 Then
										x = fwJumpIdx - 1
										GoTo Jump
									End If
								End If
							Else
								BatchLog(“  > Fallback reflash FAILED — test fails”)
							End If
						End If
						RunTest = False
						script.TestSuccessful = False
						RaiseEvent TestProgressCompleted(script)
						Return script
					End If
					GoTo Jump
				ElseIf script.TestScriptCommands(x).Command.CommandName = “VerifyUSBComm” Then
					' USB Closed → 500ms → Reset Release → 1000ms → read comDeviceMACAddress
					Dim vcm As TestScriptCommand = script.TestScriptCommands(x)
					vcm.OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					Dim maxVcmInPlace As Integer = If(vcm.RetryJumpToStep >= 0, 1, If(vcm.Retries > 0, vcm.Retries + 1, 1))
					Dim vcmPassed As Boolean = False
					For attempt As Integer = 1 To maxVcmInPlace
						vcm.Attempts = attempt
						BatchLog(“  > VerifyUSBComm attempt “ & attempt & “/” & maxVcmInPlace)
						If VerifyUSBCommAction IsNot Nothing Then Await VerifyUSBCommAction()
						CurrentResponse = String.Empty
						Await WriteData(“comDeviceMACAddress “ & vcm.Comm_Val & vbCrLf)
						Dim expmsgVcm As String = vcm.ExpectedResponse
						If Regex.Split(CurrentResponse, expmsgVcm).Length - 1 = 2 Then CurrentResponse = expmsgVcm
						If Regex.Matches(CurrentResponse, Regex.Escape(vbCrLf)).Count > 1 Then
							CurrentResponse = Mid(CurrentResponse, InStr(CurrentResponse, vbCrLf) + 2)
							If InStr(CurrentResponse, “comDeviceMACAddress”) > 0 AndAlso MacAddress = “” Then
								MacAddress = CurrentResponse.Replace(“comDeviceMACAddress”, “”).Replace(“ “, “”).Replace(vbCr, “”).Replace(vbLf, “”).Replace(vbCrLf, “”)
							End If
							CurrentResponse = CurrentResponse.TrimStart(New Char() {Chr(13), Chr(10)})
							Dim firstCrLfVcm As Integer = InStr(CurrentResponse, vbCrLf)
							If firstCrLfVcm > 0 Then CurrentResponse = Mid(CurrentResponse, 1, firstCrLfVcm - 1)
						End If
						vcm.ActualResponse = Trim(CurrentResponse)
						CurrentResponse = String.Empty
						vcm.InbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
						If vcm.IsSuccess Then
							CommLog(“Step “ & vcm.RefNum.ToString() & “ VerifyUSBComm PASS (“ & attempt & “/” & maxVcmInPlace & “): '” & vcm.ActualResponse & “'”)
							RaiseEvent TestCommandSucceeded(vcm)
							vcmPassed = True
							Exit For
						Else
							CommLog(“Step “ & vcm.RefNum.ToString() & “ VerifyUSBComm FAIL (“ & attempt & “/” & maxVcmInPlace & “): actual='” & vcm.ActualResponse & “' expected='” & vcm.ExpectedResponse & “'”)
							BatchLog(“  > VerifyUSBComm FAIL (“ & attempt & “/” & maxVcmInPlace & “)” & If(attempt < maxVcmInPlace, “ retrying...”, “”))
							RaiseEvent TestCommandFailed(vcm)
						End If
					Next
					If Not vcmPassed Then
						If vcm.RetryJumpToStep >= 0 AndAlso vcm.RetriesUsed < vcm.Retries Then
							vcm.RetriesUsed += 1
							CommLog(“RETRY(VerifyUSBComm) RetriesUsed=” & vcm.RetriesUsed.ToString() & “/” & vcm.Retries.ToString() & “ -> jump RefNum=” & vcm.RetryJumpToStep.ToString())
							BatchLog(“  > Retry VerifyUSBComm (“ & vcm.RetriesUsed.ToString() & “/” & vcm.Retries.ToString() & “)”)
							If vcm.RetryAction = “USBToggle” AndAlso ToggleUSBAction IsNot Nothing Then
								BatchLog(“  > USB Toggle (VerifyUSBComm retry)”)
								Await ToggleUSBAction(vcm.RetryDelayMs)
							End If
							Dim vcmJumpIdx As Integer = script.TestScriptCommands.FindIndex(Function(c) c.RefNum = vcm.RetryJumpToStep)
							If vcmJumpIdx >= 0 Then
								x = vcmJumpIdx - 1
								GoTo Jump
							End If
						End If
						RunTest = False
						script.TestSuccessful = False
						RaiseEvent TestCommandFailed(vcm)
						RaiseEvent TestProgressCompleted(script)
						Return script
					End If
					If vcm.Sleep > 0 Then Await Task.Delay(vcm.Sleep)
					RaiseEvent TestProgressUpdated(CInt(((x + 1) / script.TestScriptCommands.Count) * 100))
					GoTo Jump
				ElseIf script.TestScriptCommands(x).Command.CommandName.StartsWith(“Verify-”) Then
					' Soft probe: send command, retry up to Retries+1 times, never fails the test
					Dim vc As TestScriptCommand = script.TestScriptCommands(x)
					Dim realCmd As String = vc.Command.CommandName.Substring(7)
					Dim maxAttempts As Integer = If(vc.Retries > 0, vc.Retries + 1, 1)
					Dim verifyPassed As Boolean = False
					vc.OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					For attempt As Integer = 1 To maxAttempts
						vc.Attempts = attempt
						CurrentResponse = String.Empty
						Await WriteData(realCmd & “ “ & vc.Comm_Val & vbCrLf)
						Dim expmsgV As String = vc.ExpectedResponse
						If Regex.Split(CurrentResponse, expmsgV).Length - 1 = 2 Then CurrentResponse = expmsgV
						If Regex.Matches(CurrentResponse, Regex.Escape(vbCrLf)).Count > 1 Then
							CurrentResponse = Mid(CurrentResponse, InStr(CurrentResponse, vbCrLf) + 2)
							If InStr(CurrentResponse, “comDeviceMACAddress”) > 0 AndAlso MacAddress = “” Then
								MacAddress = CurrentResponse.Replace(“comDeviceMACAddress”, “”).Replace(“ “, “”).Replace(vbCr, “”).Replace(vbLf, “”).Replace(vbCrLf, “”)
							End If
							CurrentResponse = CurrentResponse.TrimStart(New Char() {Chr(13), Chr(10)})
							Dim firstCrLfV As Integer = InStr(CurrentResponse, vbCrLf)
							If firstCrLfV > 0 Then CurrentResponse = Mid(CurrentResponse, 1, firstCrLfV - 1)
						End If
						vc.ActualResponse = Trim(CurrentResponse)
						CurrentResponse = String.Empty
						vc.InbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
						If vc.IsSuccess Then
							CommLog(“Step “ & vc.RefNum.ToString() & “ Verify PASS (“ & attempt & “/” & maxAttempts & “): '” & vc.ActualResponse & “'”)
							RaiseEvent TestCommandSucceeded(vc)
							verifyPassed = True
							Exit For
						Else
							CommLog(“Step “ & vc.RefNum.ToString() & “ Verify attempt “ & attempt & “/” & maxAttempts & “ no match: actual='” & vc.ActualResponse & “' expected='” & vc.ExpectedResponse & “'”)
							BatchLog(“  > Verify “ & realCmd & “ (“ & attempt & “/” & maxAttempts & “) - “ & If(attempt < maxAttempts, “retrying...”, “exhausted, continuing”))
							RaiseEvent TestCommandFailed(vc)
						End If
					Next
					If Not verifyPassed Then
						CommLog(“Step “ & vc.RefNum.ToString() & “ Verify EXHAUSTED - no test failure, continuing”)
					End If
					If vc.Sleep > 0 Then Await Task.Delay(vc.Sleep)
					RaiseEvent TestProgressUpdated(CInt(((x + 1) / script.TestScriptCommands.Count) * 100))
					GoTo Jump
				Else
					script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
					CurrentResponse = String.Empty
					Await WriteData(script.TestScriptCommands(x).Command.CommandName & “ “ & script.TestScriptCommands(x).Comm_Val & vbCrLf)
				End If
				Dim expmsg as string = script.TestScriptCommands(x).ExpectedResponse
				if Regex.Split(CurrentResponse, expmsg).Length - 1 = 2 then '..... remove first entry
					CurrentResponse = expmsg
				End If
				If Regex.Matches(CurrentResponse, Regex.Escape(vbCrLf)).Count > 1 Then
					CurrentResponse = Mid(CurrentResponse, InStr(CurrentResponse, vbCrLf) + 2)
					If InStr(CurrentResponse, "comDeviceMACAddress") > 0 AndAlso MacAddress = "" Then
						MacAddress = CurrentResponse.Replace("comDeviceMACAddress", "").Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "").Replace(vbCrLf, "")
					End If
					CurrentResponse = CurrentResponse.TrimStart(New Char() {Chr(13), Chr(10)})
					Dim firstCrLf As Integer = InStr(CurrentResponse, vbCrLf)
					If firstCrLf > 0 Then
						CurrentResponse = Mid(CurrentResponse, 1, firstCrLf - 1)
					End If
				End If
				'.......................
				script.TestScriptCommands(x).ActualResponse = Trim(CurrentResponse)
				CurrentResponse = String.Empty
				'script.TestScriptCommands(x).OutbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
				script.TestScriptCommands(x).InbTime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
				If script.TestScriptCommands(x).IsSuccess Then
					CommLog("Step " & script.TestScriptCommands(x).RefNum.ToString() & " PASS: actual='" & script.TestScriptCommands(x).ActualResponse & "'")
					RaiseEvent TestCommandSucceeded(script.TestScriptCommands(x))
				Else
					CommLog("Step " & script.TestScriptCommands(x).RefNum.ToString() & " FAIL: actual='" & script.TestScriptCommands(x).ActualResponse & "' expected='" & script.TestScriptCommands(x).ExpectedResponse & "' compare=" & script.TestScriptCommands(x).Compare.ToString())
					If script.TestScriptCommands(x).Retries > 0 Then
						Dim sc As TestScriptCommand = script.TestScriptCommands(x)
						If sc.RetryJumpToStep >= 0 Then
							' Jump-back retry: toggle USB (with delay) and restart from target step
							If sc.RetriesUsed < sc.Retries Then
							CommLog("RETRY(jump-back) Step " & sc.RefNum.ToString() & " '" & sc.Command.CommandName & "' actual='" & sc.ActualResponse & "' expected='" & sc.ExpectedResponse & "' RetriesUsed=" & sc.RetriesUsed.ToString() & "/" & sc.Retries.ToString() & " -> RefNum=" & sc.RetryJumpToStep.ToString())
								sc.RetriesUsed += 1
								BatchLog("  > Retry Step " & sc.RefNum.ToString() & " '" & sc.Command.CommandName & "' (" & sc.RetriesUsed.ToString() & "/" & sc.Retries.ToString() & ")")
								If sc.RetryAction = "USBToggle" AndAlso ToggleUSBAction IsNot Nothing Then
									BatchLog("  > USB Toggle (Step " & sc.RefNum.ToString() & " retry)")
									Await ToggleUSBAction(sc.RetryDelayMs)
								End If
								Dim serialJumpIdx As Integer = script.TestScriptCommands.FindIndex(Function(c) c.RefNum = sc.RetryJumpToStep)
								If serialJumpIdx >= 0 Then
									x = serialJumpIdx - 1
									GoTo Jump
								End If
							End If
							RunTest = False
							script.TestSuccessful = False
							RaiseEvent TestCommandFailed(sc)
							RaiseEvent TestProgressCompleted(script)
							Return script
						Else
							' In-place retry (legacy integer Retries field)
							Dim temp As Integer = sc.Retries
							While temp > 0
								Await WriteData(sc.Command.CommandName & " " & sc.Comm_Val & vbCrLf)
								If CommandDelay > sc.Sleep Then Await TaskDelay(CommandDelay - sc.Sleep)
								sc.ActualResponse = CurrentResponse
								CurrentResponse = String.Empty
								If sc.IsSuccess Then
									CommLog("Step " & sc.RefNum.ToString() & " PASS (retry): actual='" & sc.ActualResponse & "'")
									RaiseEvent TestCommandSucceeded(sc)
									Exit While
								Else
									temp -= 1
									sc.Attempts += 1
									RaiseEvent TestCommandFailed(sc)
								End If
							End While
							If Not sc.IsSuccess Then
								RunTest = False
								script.TestSuccessful = False
								RaiseEvent TestCommandFailed(sc)
								RaiseEvent TestProgressCompleted(script)
								Return script
							End If
						End If
					Else
						RunTest = False
						script.TestSuccessful = False
						RaiseEvent TestCommandFailed(script.TestScriptCommands(x))
						RaiseEvent TestProgressCompleted(script)
						Return script
					End If

				End If
				If script.TestScriptCommands(x).Sleep > 0 Then Await Task.Delay(script.TestScriptCommands(x).Sleep)
				Dim progress As Integer
				progress = CInt((((x + 1) / script.TestScriptCommands.Count)) * 100)
				RaiseEvent TestProgressUpdated(progress)
Jump:
			Next

			RunTest = False
			ClosePort()
			If script.TestScriptCommands.Count <> script.TestScriptCommands.Where(Function(x) x.IsSuccess).Count() Then
				script.TestSuccessful = False
				Return script
			End If
			script.TestSuccessful = True
			RaiseEvent TestProgressCompleted(script)
			Return script
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error" & ex.Message)
			RunTest = False
			ClosePort()
			script.TestSuccessful = False
			Return script
		End Try
	End Function

#End Region

#Region "Properties"

	'settings props
	Public Property CommandList As List(Of String) = New List(Of String)
	'comm props
	Private ComPort As SerialPort
	Private DebugPort As SerialPort
	Public Property PortName As String
	Public Property BaudRate As Integer
	Public Property Parity As Parity = Ports.Parity.None
	Public Property StopBits As StopBits = Ports.StopBits.One
	Public Property DataBits As Integer = 8
	'Public Property AsciiEncoding As Integer
	'Public Property Encoding As Encoding
	Public Property HandShake As Handshake = HandShake.None
	Public Property IsHashing As Boolean = True
	Public Property BarCode As String
	Public Property RtsEnable As Boolean = False
	Public Property IsText As Boolean = True

	'srfn variables
	Public Property ProductFamily As String
	Public Property CustomerName As String
	Public Property TestFile As String
	Public Property MeterForm As String
	Public Property CustomerId As String
	Public Property HashValue As String
	Public Property UtilitySerialNumber As String
	Public Property MeterSerialNumber As String
	Public Property MacAddress As String
	Public Property SqlName As String
	Public Property DateFormatString As String = "MM/dd/yyyy HH
    mm
    ss"
	Public Property BaudRateValues As List(Of String) = New List(Of String)() From {"2400", "4800", "9600", "38400", "57600", "115200"}
	Public Property DataBitValues As List(Of String) = New List(Of String)() From {"7", "8", "9"}
	Public ReadOnly Property Encoding() As Object
		Get
			comPort.Encoding = System.Text.Encoding.GetEncoding(28605)
			Return System.Text.Encoding.GetEncoding(28605)
		End Get
	End Property
	Public ReadOnly Property PortNameValues() As List(Of String)
		Get
			Return SerialPort.GetPortNames.ToList()
		End Get
	End Property
	Public ReadOnly Property ParityValues() As List(Of String)
		Get
			Dim values As New List(Of String)
			For Each value In [Enum].GetNames(GetType(Ports.Parity))
				values.Add(value)
			Next
			Return values
		End Get
	End Property
	Public ReadOnly Property StopBitValues() As List(Of String)
		Get
			Dim values As New List(Of String)
			For Each value In [Enum].GetNames(GetType(Ports.StopBits))
				values.Add(value)
			Next
			Return values
		End Get
	End Property
	Public Property CommandDelay As Integer = My.Settings.CommandDelay
	Public ReadOnly DefaultBaudRate As String = "38400"
	Public ReadOnly DefaultParity As String = "None"
	Public ReadOnly DefaultStopBits As String = "One"
	Public ReadOnly DefaultDataBits As String = "8"
	Public ReadOnly DefaultBarCode As String = "13"
	Public Property WriteWaitTime As Integer = 500
	Public TestFileDirectory As String = "Test Files"
	Public RunTest As Boolean
	Public TestProgress As Integer = 0 '...1 = SelfTest Pass, 2 = get Firmware, 3 = Run Test
	Private _currentResponse As String = String.Empty
	Private Property SelfTestReceived As Boolean = False
	Private Property EnteringQuietModeReceived As Boolean = False
	Private Property CurrentResponse As String
		Get
			Return _currentResponse
		End Get
		Set(value As String)
			_currentResponse = value
			If _currentResponse.Contains("SelfTest 0000") Then
				_WaitToStart = 0
				SelfTestReceived = True
				RaiseEvent SelfTestCommandReceived()
			End If
			If _currentResponse.Contains("OK") Then
				_WaitToStart = 0
			End If
		End Set
	End Property
#End Region
#Region "Serial Communication"
	Public Function SetBaud(ByVal ProductFamily As string) As Boolean
		Try
			With ComPort
				.BaudRate = 38400
				Select Case ProductFamily
					Case "SRFN-KV2c", "AclaraRF3-KV2c"
						.BaudRate = 9600
				End Select
			End with
		Catch ex As Exception
		End Try
		Return True
	End Function
	Public Function SetDebugBaud() As Boolean
		Try
			With ComPort
				.BaudRate = 115400
			End with
		Catch ex As Exception
		End Try
		Return True
	End Function
#Region "SerialPort"
	Public Function OpenPort() As Boolean
		Try
			If ComPort.IsOpen() Then
				ComPort.Close()
			End If
			With ComPort
				'.BaudRate = Cint(GetBaudRate())
				'.BaudRate = GetBaudRate()
				.DataBits = DataBits
				.StopBits = StopBits
				.Parity = Parity
				.Handshake = HandShake
				.RtsEnable = RtsEnable
				.PortName = PortName
				.Open()
			End With
			'RaiseEvent StatusMessagePosted("Port opened at " & DateTime.Now.ToString(DateFormatString))
			Return True
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("An error occurred opening the port. - " & ex.Message)
			FailMsg = ex.Message
			Return False
		End Try
	End Function
	Public Sub ClosePort()
		Try
			If ComPort.IsOpen Then
				ComPort.Close()
				'RaiseEvent StatusMessagePosted("Port closed at " & DateTime.Now.ToString(DateFormatString))
			End If
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error
     " & ex.Message & System.Environment.NewLine & "ClosePort()")
		End Try
	End Sub
	Public Async Function WriteData(ByVal msg As String) As Task '.....updated1/4/2023, added delay till timeout >> msg|timeout
		Try '.......................................................... updated 1/12/2023 add Discard Out Buffer
			Dim delay As Integer = 0
			Dim msglen As Integer = 0
			If Not RunTest Then CurrentResponse = String.Empty
			If Instr(msg, "|") > 0 Then
				Dim tmp As String() = Split(msg, "|")
				msg = tmp(0)
				msglen = msg.Length + 5
				delay = Cint(tmp(1))
			End if
			outtime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”)
			If Not ComPort.IsOpen Then ComPort.Open()
			If IsText Then
				if Not InStr(msg, "vbLf") > 0 Then msg += vbLf
				ComPort.Write(msg)
				RaiseEvent DataWritten(msg)
			Else
				Dim byteMsg = HexToByte(msg)
				ComPort.Write(byteMsg, 0, byteMsg.Length)
				RaiseEvent DataWritten(msg)
			End If
			CommLog("TX: " & msg.TrimEnd())

			If delay > 0 Then
				While delay > 0
					Await TaskDelay(100)
					delay -= 100
					If CurrentResponse.Length > msglen Then Exit While
				End While
			Else
				Await TaskDelay(delay)
			End If
			CurrentResponse = CurrentResponse.Replace(vbLf, "").Replace(vbCr, vbCrLf)
			RaiseEvent DataReceived(CurrentResponse)
			CommLog("RX: " & CurrentResponse.Replace(vbCrLf, "|").Trim())
			ComPort.DiscardOutBuffer()
			intime = Now.ToString(“MM/dd/yyyy HH:mm:ss.fff”) '.ToString("yyyy-MM-dd HH:mm:ss")
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error" & ex.Message & System.Environment.NewLine &
			"WriteData(msg = " & msg & ", isTextCommand = " & IsText.ToString() & ")")
		End Try
	End Function
	Public Sub ReadData(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
		Dim response As String = String.Empty '..................................................... updated 1/12/2023 add Discard In Buffer
		Try
			If IsText Then
				response = ComPort.ReadExisting()
			Else
				Dim numberOfBytes = ComPort.BytesToRead
				Dim bytes = New Byte(numberOfBytes - 1) {}
				ComPort.Read(bytes, 0, numberOfBytes)
				response = ByteToHex(bytes)
			End If
			CurrentResponse += response
			If CurrentResponse.Length > 5 Then
				CurrentResponse = CurrentResponse
				'ComPort.DiscardInBuffer() 
			End If
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error
     " & ex.Message)
		End Try
	End Sub
	Private Sub ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)
		RaiseEvent ErrorMessagePosted("Serial communication reported an error.")
	End Sub
#End Region
#Region "Debug Port"
	Public Async Function SendDebugMessage(ByVal msg As String, ByVal teststep As string, ByVal MacAddress As string, ByVal MeterSerial As string, ByVal CustomerID As string, ByVal firmware As string) As Task(Of String)
		Dim temp As String = String.Empty
		PortName = "COM" & GetDebugPort().ToString()
		With DebugPort
			.BaudRate = Cint("115200")
			.DataBits = DataBits
			.StopBits = StopBits
			.Parity = Parity
			.Handshake = HandShake
			.RtsEnable = RtsEnable
			.PortName = PortName
		End With
		DebugPort.Open
		If OpenDebugPort(PortName) = False Then
			Return "Fail,0,Unable to Open Debug Port"
		Else
			Await WriteDebugData(msg & System.Environment.NewLine)
		End If

		Threading.Thread.Sleep(2000)
		DebugPort.DiscardInBuffer()
		DebugPort.Close()
		DebugPort.Dispose()


		Try
			Dim file6 As String = Mid(CurrentResponse, 17 + InStr(CurrentResponse, "337,     "), 16)
			'Threading.Thread.Sleep(500)
			Dim debugresult As String = "Fail"
			For Each c As Char In file6
				If c <> "0" Then
					debugresult = "Pass"
					Exit For
				End If
			Next
			Dim response As String = System.Security.SecurityElement.Escape(CurrentResponse)
			SQLWriteDebugResults(debugresult, teststep, MacAddress, MeterSerialNumber, CustomerId, firmware, response)
		Catch ex As Exception
		End Try

		Return CurrentResponse
	End Function
	Private Function GetDebugPort() As Integer
		Dim doc As New XmlDocument()
		Dim xmldoc As String = Application.StartupPath & "\FormValues.xml"
		doc.Load(xmldoc)
		Dim formnodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/CurrentForm")
		For Each node As XmlNode In formnodes
			GetDebugPort = CInt(node.SelectSingleNode("debugport").InnerText)
		Next
		Return GetDebugPort
	End Function
	Public Shared Function RemoveIllegalFileNameChars(input As String, Optional replacement As String = "") As String
		Dim regexSearch = New String(Path.GetInvalidFileNameChars()) & New String(Path.GetInvalidPathChars())
		Dim r = New Regex(String.Format("[{0}]", Regex.Escape(regexSearch)))
		Return r.Replace(input, replacement)
	End Function
	Public Function SQLWriteDebugResults(ByVal DebugResult As String, ByVal TestStep As String, ByVal MacAddress As String, ByVal MeterSerialNumber As String, ByVal CustomerId As String, ByVal firmware As String, ByVal Xmldebug As String) As Boolean
		SQLWriteDebugResults = False
		Dim sp As String = "','"
		Dim query As String = "INSERT INTO [Aclara_TestResults].[dbo].[SRFN_DebugResults] Values("
		query = query & "'" & DebugResult & sp & TestStep & sp & Now() & sp & MacAddress & sp & MeterSerialNumber & sp & CustomerId & sp & firmware & sp & Xmldebug & "')"
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_TestResults"))
			rsData = dcnDB.Execute(query)
			'rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			_TestFail = True
			CommLog("SQLWriteDebugResults Write Error: " & ex.ToString)
			'Exit Function
		End Try
	End Function
	Public Function OpenDebugPort(ByVal port As string) As Boolean
		Try
			If DebugPort.IsOpen() Then
				DebugPort.Close()
			End If
			With DebugPort
				.DataBits = DataBits
				.StopBits = StopBits
				.Parity = Parity
				.Handshake = HandShake
				.RtsEnable = RtsEnable
				.PortName = port
				.Open()
			End With
			Return True
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("An error occurred opening the debugport. - " & ex.Message)
			FailMsg = ex.Message
			Return False
		End Try
	End Function
	Public Async Function WriteDebugData(ByVal msg As String) As Task
		Try
			CurrentResponse = String.Empty
			If Not DebugPort.IsOpen Then DebugPort.Open()
			If IsText Then
				if Not InStr(msg, "vbLf") > 0 Then msg += vbLf
				DebugPort.Write(msg)
			End If

			Await TaskDelay(1000)
			CurrentResponse = CurrentResponse.Replace(vbLf, "").Replace(vbCr, vbCrLf)
			RaiseEvent DataReceived(CurrentResponse)

		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Write DebugPort Error: " & ex.Message & System.Environment.NewLine &
			"WriteData(msg = " & msg & ", isTextCommand = " & IsText.ToString() & ")")
		End Try
	End Function
	Public Sub ReadDebugData(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
		Dim response As String = String.Empty
		Dim file6 as String = String.Empty
		Try
			If IsText Then
				response = DebugPort.ReadExisting()
			Else
				'Dim numberOfBytes = DebugPort.BytesToRead
				Dim numberOfBytes = 264
				Dim bytes = New Byte(numberOfBytes - 1) {}
				DebugPort.Read(bytes, 0, numberOfBytes)
				response = ByteToHex(bytes)
			End If
			CurrentResponse += response
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Read Debug Port Error: " & ex.Message)
		End Try
	End Sub
#End Region
	Private Sub SetCommProperties()
		With ComPort
			.BaudRate = BaudRate
			.Parity = Parity
			.StopBits = StopBits
			.Handshake = HandShake
			.DataBits = DataBits
			.RtsEnable = RtsEnable
			.PortName = PortName
		End With
	End Sub
#End Region

#Region "Support Functions"
	Shared ReadOnly r As New Regex("^[0-9A-F]+$")
	Public Shared Function IsValidHex(data As String) As Boolean
		Return r.Match(data).Success
	End Function

	Public Shared Function GenerateHash(ByVal SourceText As String) As String
		Dim Ue As New UnicodeEncoding
		Dim ByteSourceText As Byte() = Ue.GetBytes(SourceText)
		Dim Md5 As New Security.Cryptography.MD5CryptoServiceProvider
		Dim ByteHash As Byte() = Md5.ComputeHash(ByteSourceText)
		Return Convert.ToBase64String(ByteHash)
	End Function
	Public Function HexToByte(ByVal data As String) As Byte()
		data = data.Replace(" ", "")
		If data.Length Mod 2 = 0 Then
			Dim numberChars As Integer = data.Length
			Dim bytes As Byte() = New Byte(CInt(numberChars / 2 - 1)) {}

			For i As Integer = 0 To numberChars - 1 Step 2
				bytes(CInt(i / 2)) = Convert.ToByte(data.Substring(i, 2), 16)
			Next
			Return bytes
		End If
		RaiseEvent ErrorMessagePosted("Error: Invalid Hex data." & System.Environment.NewLine & "HexToByte(data=" & data & ")")
		Return Nothing
	End Function

	Public Shared Function ByteToHex(ByVal byteArray As Byte()) As String
		Dim builder As New StringBuilder(byteArray.Length * 3)
		For Each data As Byte In byteArray
			builder.Append(Convert.ToString(data, 16).PadLeft(2, "0"c).PadRight(3, " "c))
		Next

		Return builder.ToString().ToUpper()
	End Function
	Public Async Function SendMessage(ByVal msg As String, ByVal port As Integer, ByVal productfamily As string, Optional ByVal optval As String = "") As Task(Of String)
		Dim temp As String = String.Empty
		Dim i As Integer = 0
		PortName = "COM" & port.ToString()
		If ComPort.IsOpen Then ComPort.Close()
		SetBaud(productfamily)
		If OpenPort() = False Then
			Return "Fail,0,Unable to Open Port"
		Else
			Select Case msg
				Case "OpenPort"
					Return "Pass,0,Port Opened"
				Case "quietMode 0"
					RunTest = True
					If Await DisableQuietMode() = False Then
						RunTest = False
						Return "Fail,0,DisableQuietMode FAILED"
					Else
						RunTest = False
						Return "Pass,0,QuietMode Disabled"
					End If
				Case "quietMode 1"
					RunTest = True
					If Await EnableQuietMode() = False Then
						RunTest = False
						Return "Fail,0,EnableQuietMode FAILED"
					Else
						Threading.Thread.Sleep(2000)
						Return "Pass,0,QuietMode Enabled"
					End If
				Case "quietMode"
					If Await QuietModeEnabled() = True Then
						Return "Pass,0,QuietMode Enabled"
					Else
						Return "Pass,0,QuietMode Not Enabled"
					End If
				Case "ReadEnergy"
					Await WriteData("0.0.0.1.1.1.12.0.0.0.0.0.0.0.0.3.72.0|5000" & vbLf)
					While Instr(1, CurrentResponse, vbCrLf) < 1 '.........'wait till response <> null or up to 10s
						Threading.Thread.Sleep(100)
					End While
				Case "edfwversion"
					Await WriteData("edfwversion" & vbLf)
					While Instr(1, CurrentResponse, vbCrLf) < 1 '.........'wait till response <> null or up to 10s
						Threading.Thread.Sleep(100)
					End While
				Case "edInfo"
					Await WriteData("edInfo" & vbLf)
					While Instr(1, CurrentResponse, vbCrLf) < 1 '.........'wait till response <> null or up to 10s
						Threading.Thread.Sleep(100)
					End While
				Case "virgindelay"
					CurrentResponse = String.Empty
					Await WriteData("virgindelay" & vbLf)
					Dim vdDeadline As DateTime = DateTime.Now.AddSeconds(10)
					While Not CurrentResponse.Contains("Signature erased") AndAlso DateTime.Now < vdDeadline
						Threading.Thread.Sleep(100)
					End While
					If Not CurrentResponse.Contains("Signature erased") Then CurrentResponse = "virgindelay error - check reset release"
					Threading.Thread.Sleep(500)
				Case "edMfgSerialNumber"
					Await WriteData("edMfgSerialNumber" & vbLf)
					While Instr(1, CurrentResponse, vbCrLf) < 1 '.........'wait till response <> null or up to 10s
						Threading.Thread.Sleep(100)
					End While
				Case "OpticalLogon"
					Dim SetExtEncoding As Object = Encoding
					Await WriteData("î" & ChrW(128) & vbNullChar & vbNullChar & vbNullChar & ChrW(14) & "P" & vbNullChar & ChrW(2) & "Aclara    " & vbCr & "FÜ" & vbLf)
				Case Else
					Await WriteData(msg & System.Environment.NewLine)
					'If Mid(msg,1,6) = "0.0.0." Then Threading.Thread.Sleep(1000)
			End Select
		End If
		Threading.Thread.Sleep(100)

		ClosePort()
		RunTest = False
		temp = CurrentResponse.Replace(vbLf, "").Replace(vbCr, "").Replace(vbCrLf, "")
		If InStr(temp, "is not a valid command!") > 0 Then
			_WaitToStart = 1 '........Quiet Mode ON
		End If
		If InStr(temp, "Entering quiet mode") > 0 Then
			_WaitToStart = 1 '........Quiet Mode ON
			Threading.Thread.Sleep(4000)
		End If
		If InStr(temp, "Exiting quiet mode") > 0 Then
			If _WaitToStart = 1 Then
				RaiseEvent SelfTestCommandReceived()
			End If
			_WaitToStart = 0 '........Reset Flag
			Threading.Thread.Sleep(5000)
		End If
		ComPort.Close()
		ComPort.Dispose()
		Return CurrentResponse
	End Function
	Private Async Function TaskDelay(Optional ByVal CustomDelayTime As Integer = 0) As Task
		If CustomDelayTime = 0 Then
			Await Task.Delay(WriteWaitTime)
		Else
			Await Task.Delay(CustomDelayTime)
		End If
	End Function

	Private Async Function WaitForSelfTestCommandReceived() As Task
		While True
			If Not EnteringQuietModeReceived AndAlso RunTest Then
				Await Task.Delay(1000)
				RaiseEvent WaitingForSelfTestCommandReceived()
			Else
				Exit While
			End If
		End While
	End Function
#End Region

#Region "Read Files"

	'Public Function ReadCommandList() As List(Of SerialCommand)
	'Dim filePath = Application.StartupPath & "\" & CommandsListFileName
	'Dim lines = File.ReadAllLines(filePath)
	'Dim commands = New List(Of SerialCommand)
	'Try
	'    For X As Integer = 0 To lines.Count - 1
	'        Dim commandDesc = lines(X).Split(New Char() {"="c}, StringSplitOptions.RemoveEmptyEntries)(1)
	'        Dim commandName = lines(X + 1).Split(New Char() {"="c}, StringSplitOptions.RemoveEmptyEntries)(1)
	'        commands.Add(New SerialCommand() With {.CommandName = commandName, .CommandDesc = commandDesc})

	'        X += 1
	'    Next
	'Catch ex As Exception
	'    RaiseEvent ErrorMessagePosted(ex.Message)
	'End Try
	'Return commands
	'    End Function
	Public Sub SetSetOptions()
		'Dim filePath = Application.StartupPath & "\" & SetOptionsFileName
		Dim setOptionVals = "cboPort=" & PortName & vbCrLf &
			"cboBaud=" & BaudRate.ToString() & vbCrLf &
			"cboParity=" & Parity.ToString() & vbCrLf &
			"cboStop=" & StopBits.ToString() & vbCrLf &
			"cboData=" & DataBits.ToString() & vbCrLf &
			"ChkHash=" & IIf(IsHashing, "1", "0").ToString() & vbCrLf &
			"rdoText=" & IIf(IsText, "1", "0").ToString()
		'File.WriteAllText(filePath, setOptionVals)
	End Sub
	Public Function ReadTestFiles() As String()
		Dim files = Directory.GetFiles(Application.StartupPath & "\" & TestFileDirectory & "\")
		Dim fileNames = New List(Of String)
		For Each file As String In files
			fileNames.Add(file.Substring(file.LastIndexOf("\") + 1))
		Next
		Return fileNames.ToArray
	End Function
	'Public Function ReadManualScriptFile(fileName As String) As TestScript
	'Return ReadTestFile(Path.Combine(Application.StartupPath, ManualScriptDirectory, fileName))
	'End Function
	Public Function ReadTestScriptFile(fileName As String) As TestScript
		Return ReadTestFile(Path.Combine(Application.StartupPath, TestFileDirectory, fileName))
	End Function
	Public Function ReadTestFile(ByVal filePath As String) As TestScript
		Try
			Dim ds As New DataSet()
			Dim script As New TestScript
			Dim temp As String = ""
			ds.ReadXml(filePath)
			Dim strTemp As String = String.Empty
			Dim strTemp1 As String = String.Empty

			For i = 0 To ds.Tables("Table").Rows.Count - 1
				For y = 0 To ds.Tables("Table").Columns.Count - 1
					If IsDBNull(ds.Tables("Table").Rows(i).Item(y)) = False Then
						strTemp = CStr(ds.Tables("Table").Rows(i).Item(y))
					End If
					strTemp1 = strTemp1 & strTemp
				Next
			Next

			With script
				.TestFilePath = filePath
				.ProductFamily = CStr(ds.Tables("HashTable").Rows(0).Item(0))
				.Drawing = CStr(ds.Tables("HashTable").Rows(0).Item(1))
				.CustomerName = CStr(ds.Tables("HashTable").Rows(0).Item(2))
				.CustomerID = CStr(ds.Tables("HashTable").Rows(0).Item(3))
				.HashValue = CStr(ds.Tables("HashTable").Rows(0).Item(4))
				.ProgressMax = CInt(ds.Tables("HashTable").Rows(0).Item(5))
				.CalculatedHash = GenerateHash(strTemp1)
				If .HashValue <> .CalculatedHash Then
					RaiseEvent ErrorMessagePosted("Error: " & filePath.Substring(filePath.LastIndexOf("\") + 1) & " - Hash Value doesn't match.")
				End If
			End With

			For i = 0 To ds.Tables("Table").Rows.Count - 1
				If CStr(ds.Tables("Table").Rows(i).Item(1)) = "" Then
				End If
				Dim scriptCmd As New TestScriptCommand() With {
					.RefNum = CInt(ds.Tables("Table").Rows(i).Item(0)),
					.Command = New SerialCommand() With {
						.CommandDesc = CStr(ds.Tables("Table").Rows(i).Item(1)),
						.CommandName = CStr(ds.Tables("Table").Rows(i).Item(2))
					},
					.Comm_Val = CStr(ds.Tables("Table").Rows(i).Item(3)),
					.ExpectedResponse = CStr(ds.Tables("Table").Rows(i).Item(4)),
					.Sleep = CInt(ds.Tables("Table").Rows(i).Item(5)),
					.Compare = CInt(ds.Tables("Table").Rows(i).Item(6)),
					.ErrCode = CStr(ds.Tables("Table").Rows(i).Item(8))
				}
				scriptCmd.ParseRetriesField(CStr(ds.Tables("Table").Rows(i).Item(7)))
				script.TestScriptCommands.Add(scriptCmd)

			Next
			'For i = 0 To script.TestScriptCommands.Count
			'    If script.TestScriptCommands(i).Command.CommandName = "comDeviceMACAddress" Then
			'        script.TestScriptCommands(i).ExpectedResponse = MacAddress
			'    End If
			'Next

			Return script
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error: " & ex.Message)
		End Try

		Return Nothing
	End Function

	Public Function ReadFormList(Optional ByVal filePath As String = Nothing) As FormList
		Try
			Dim ds As New DataSet()
			Dim forms As New FormList()
			If filePath Is Nothing Then filePath = Application.StartupPath & "\FormList.xml"

			ds.ReadXml(filePath)

			forms.HashValue = ds.Tables("HashTable").Rows(0)(0).ToString()

			For i = 0 To ds.Tables("Table").Rows.Count - 1
				forms.FormListProducts.Add(New FormListProduct() _
											With
											{
											.MeterType = CStr(ds.Tables("Table").Rows(i).Item(0)),
											.Product = CStr(ds.Tables("Table").Rows(i).Item(1))
										   })
			Next
			Return forms
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error: " & ex.Message)
		End Try
		Return Nothing
	End Function

#Region "IDisposable Support"
	Private disposedValue As Boolean ' To detect redundant calls

	' IDisposable
	Protected Overridable Sub Dispose(disposing As Boolean)
		If Not disposedValue Then
			If disposing Then
				ComPort.Dispose()
			End If

			' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			' TODO: set large fields to null.
		End If
		disposedValue = True
	End Sub

	' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
	'Protected Overrides Sub Finalize()
	'    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
	'    Dispose(False)
	'    MyBase.Finalize()
	'End Sub

	' This code added by Visual Basic to correctly implement the disposable pattern.
	Public Sub Dispose() Implements IDisposable.Dispose
		' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
		Dispose(True)
		' TODO: uncomment the following line if Finalize() is overridden above.
		' GC.SuppressFinalize(Me)
	End Sub
#End Region

#End Region
#Region "SQL Commands"
	'============================================== Get SQL HashValue and Check against xml
	Private Function xmlHashVal(ByVal ProductFamily As String, ByVal CustomerID As String, ByVal Firmware As String) As String
		xmlHashVal = ""
		Dim inifile As String = Application.StartupPath & "\Test Files\" & Firmware & "-" & ProductFamily & "-" & CustomerID & ".xml"
		Try
			If File.Exists(inifile) Then
				Dim ds As New DataSet()
				ds.ReadXml(inifile)
				Dim strTemp As String = String.Empty
				Dim strTemp1 As String = String.Empty
				For i = 0 To ds.Tables("Table").Rows.Count - 1
					For y = 0 To ds.Tables("Table").Columns.Count - 1
						If Not IsDBNull(ds.Tables("Table").Rows(i).Item(y)) Then
							strTemp = CStr(ds.Tables("Table").Rows(i).Item(y))
						End If
						strTemp1 = strTemp1 & strTemp
					Next
				Next
				xmlHashVal = GenerateHash(strTemp1)
			End If
		Catch ex As Exception
			RaiseEvent ErrorMessagePosted("Error: " & ex.Message)
		End Try
	End Function
	Public Function SQLWriteTestResults(ByVal ProductFamily As String, ByVal Result As String, ByVal MacAddress As String, ByVal MeterSerialNumber As String, ByVal UtilitySerialNumber As String, ByVal CustomerName As String, ByVal CustomerId As String, ByVal TestFile As String, ByVal HashValue As String, ByVal MeterForm As String, ByVal RefNumber As String, ByVal Exp_Resp As String, ByVal Act_Resp As String, ByVal DLL_Ver As String) As Boolean
		SQLWriteTestResults = False
		Dim query As String = "INSERT INTO [Aclara_TestResults].[dbo].[SRFN_TestResults] Values("
		query = query & " '" & ProductFamily & "', '" & Result & "', '" & Now() & "', '" & MacAddress & "', '" & MeterSerialNumber & "', '" & UtilitySerialNumber & "', '" & _
		CustomerName & "', '" & CustomerId & "', '" & TestFile & "', '" & HashValue & "', '" & MeterForm & "', '" & RefNumber & "', '" & Exp_Resp & "', '" & Act_Resp & "', '" & DLL_Ver & "', '" & Environment.MachineName & "')"
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_TestResults"))
			rsData = dcnDB.Execute(query)
			'rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			_TestFail = True
			CommLog("CheckHashValue/WriteTestResults Error: " & ex.ToString)
			'Exit Function
		End Try
	End Function
	Private Function GetConfig(ByVal ProductFamily As String, ByVal CustomerID As String, ByVal Firmware As String) As String
		GetConfig = ""
		Dim query As String = "Select * FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Configs] Where "
		query = query & "Product = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "'"
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			rsData = dcnDB.Execute(query)
			GetConfig = RTrim(CStr(rsData.Fields(0).Value)).ToString
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			_TestFail = True
			CommLog("GetConfig Error: " & ex.ToString)
		End Try
	End Function

	Private Function SQLHashVal(ByVal ProductFamily As String, ByVal CustomerID As String, ByVal Firmware As String) As String
		SQLHashVal = ""
		Dim query As String = "Select HashVal FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] Where "
		query = query & "ProductFamily = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "' and Firmware = '" & Firmware & "'"
		Dim attempt As Integer = 0
		Do While attempt < 2
			Try
				CheckSQLState()
				dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
				rsData = dcnDB.Execute(query)
				SQLHashVal = RTrim(CStr(rsData.Fields(0).Value)).ToString
				rsData.Close()
				rsData = Nothing
				Return SQLHashVal
			Catch ex As Exception
				CommLog("SQLHashVal Error (attempt " & (attempt + 1) & "): " & ex.ToString)
				attempt += 1
				If attempt < 2 Then Threading.Thread.Sleep(500)
			End Try
		Loop
		_TestFail = True
	End Function
	'============================================== Get Customer Script & Write XML File
	Private Function GetDLLRevision(ByVal DLLver As String) As Boolean
		CheckSQLState()
		'DLLver = DLLversion
		Try
			dcnDB.Open(GetSQLConnection("DLL_Revision"))
			rsData = dcnDB.Execute("SELECT [DLL_Rev] FROM [Aclara_CustomerSpecific].[dbo].[DLL_Revision] where DLL_Rev = '" & DLLver & "'")
			If Not rsData.EOF Then Return True
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			CommLog("DLL Check Error: " & ex.ToString)
		End Try
		Return False
	End Function
	'============================================== Get Customer Name
	Public Function GetSQLCustomerName(ByVal ProductFamily As String, ByVal CustomerID As String) As String
		GetSQLCustomerName = ""
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			Dim query As String = "Select Count(*) FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] where "
			query = query & "ProductFamily = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "'"
			rsData = dcnDB.Execute(query)
			Dim cnt As Integer = CInt(rsData.Fields(0).Value)
			rsData.Close()
			rsData = Nothing
			If cnt = 0 Then Return "Customer Not Found"
			' Select by known safe column names only — avoids "invalid column name" if schema differs
			query = "Select Top 1 ProductFamily, Drawing, CustomerID, Firmware, HashVal FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] where "
			query = query & "ProductFamily = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "'"
			rsData = dcnDB.Execute(query)
			' Fields: 0=ProductFamily, 1=Drawing, 2=CustomerID, 3=Firmware, 4=HashVal
			GetSQLCustomerName = RTrim(CStr(rsData.Fields(2).Value)) & "  " & RTrim(CStr(rsData.Fields(3).Value)) & "  " & RTrim(CStr(rsData.Fields(1).Value))
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			Dim cs As String = CustomerValuesConnStr
			If cs = "" Then cs = "(empty — not configured)"
			CommLog("GetCustomerName FAILED" & vbCrLf &
			        "  ConnStr: " & cs & vbCrLf &
			        "  Error:   " & ex.Message)
			MsgBox("CustomerValues SQL connection failed:" & vbCrLf & vbCrLf &
			       "ConnStr: " & cs & vbCrLf & vbCrLf &
			       "Error: " & ex.Message,
			       MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, "SQL Error")
		End Try
	End Function
	'============================================== Get All Customers for Product Family
	Public Function GetSQLLikeCustomers(ByVal ProductFamily As String, ByVal CustomerID As String) As String
		GetSQLLikeCustomers = ""
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			' Search CustomerID and Customer columns using entered value as LIKE pattern
			Dim term As String = CustomerID.Replace("'", "''")
			Dim query As String = "SELECT TOP 10 ProductFamily, Drawing, Customer, CustomerID, Firmware FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] WHERE " &
			                      "ProductFamily = '" & ProductFamily & "' AND (CustomerID LIKE '%" & term & "%' OR Customer LIKE '%" & term & "%') ORDER BY CustomerID"
			rsData = dcnDB.Execute(query)
			Dim sb As New System.Text.StringBuilder()
			Do While Not rsData.EOF
				' Fields: 0=ProductFamily, 1=Drawing, 2=Customer, 3=CustomerID, 4=Firmware
				sb.AppendLine(RTrim(CStr(rsData.Fields(0).Value)) & "  " & RTrim(CStr(rsData.Fields(1).Value)) & "  " &
				              RTrim(CStr(rsData.Fields(2).Value)) & "  " & RTrim(CStr(rsData.Fields(3).Value)) & "  " &
				              RTrim(CStr(rsData.Fields(4).Value)))
				rsData.MoveNext()
			Loop
			rsData.Close()
			rsData = Nothing
			GetSQLLikeCustomers = sb.ToString().TrimEnd()
		Catch ex As Exception
			Dim cs As String = CustomerValuesConnStr
			If cs = "" Then cs = "(empty — not configured)"
			CommLog("GetSQLLikeCustomers FAILED" & vbCrLf &
			        "  ConnStr: " & cs & vbCrLf &
			        "  Error:   " & ex.Message)
		End Try
	End Function
	'============================================== Get Customer Config, Template, Write SQL INSERT
	Private Function GetCustomerConfig(ByVal ProductFamily As String, ByVal CustomerID As String) As String
		'Private Function GetCustomerConfig_OLD(ByVal ProductFamily As String, ByVal CustomerID As String, ByVal Firmware As String) As String
		GetCustomerConfig = "Fail"
		Dim i As Integer = 0
		Dim j As Integer = 0
		Dim CustomerName As String = ""
		Dim Drawing As string = ""
		Dim ScriptName As String = ""
		Dim ProgressMax As integer
		Dim configtmp(100) as String
		Dim xmlconfig(100, 1) as String
		Dim xmltemplate(200) as String
		Dim Firmware As String = ""
		'============================================== Get CustomerConfig (If Exists)
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			Dim query As String = "Select * FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Configs] where "
			query = query & "ProductFamily = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "'"
			rsData = dcnDB.Execute(query)
			CustomerName = RTrim(CStr(rsData.Fields(1).Value))
			Drawing = RTrim(CStr(rsData.Fields(3).Value))
			ScriptName = RTrim(CStr(rsData.Fields(4).Value))
			configtmp = Split(RTrim(CStr(rsData.Fields(7).Value)), "><")
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			ComPort.Close()
			ComPort.Dispose()
			Return "Fail,4,No Customer Config-" & ProductFamily & "-" & CustomerID
		End Try
		Dim configtmp2(100, 1) As String
		Dim grt As Integer = 0
		Dim les As Integer = 0
		For i = 1 To UBound(configtmp)
			grt = InStr(configtmp(i), ">")
			les = InStr(configtmp(i), "<")
			If les > 0 Then
				configtmp2(i, 0) = Mid(configtmp(i), 1, grt - 1)
				configtmp2(i, 1) = Mid(configtmp(i), grt + 1, les - grt - 1)
			End If
		Next
		'============================================== GET Template
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			Dim query As String = "Select * FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Script_Templates] where "
			'OLD query = query & "ProductFamily = '" & ProductFamily & "' and Firmware = '" & Firmware & "'"
			query = query & "ProductFamily = '" & ProductFamily & "'"
			rsData = dcnDB.Execute(query)
			ProgressMax = Cint(rsData.Fields(5).Value)
			xmltemplate = Split(RTrim(CStr(rsData.Fields(7).Value)), "<Table>")
			rsData = Nothing
		Catch ex As Exception
			rsData.Close()
			Return "Fail,4,No Template-" & ProductFamily
		End Try
		'============================================== Replace all Values
		'Logic:
		'<Comm_Val>val</Comm_Val> - swap
		'<Exp_Resp>val</Exp_Resp> - swap
		'otherwise ignore

		'integrationSetupDate >> current date
		Dim tmpIntgSetDate As String = Replace(Now().ToString, ".", "")
		'integrationProgramVersion >> DLL minus "."
		Dim tmpProgver As String = Replace(DLLVersion, ".", "")

		'Find start of customer specifics >> j = "START CUSTOMER SPECIFIC CONFIGURATION"
		for j = 1 To UBound(xmltemplate)
			If InStr(xmltemplate(j), "START CUSTOMER") > 0 Then Exit For
		Next
		Dim xmlstrt As Integer = j + 1

		For i = 1 To UBound(configtmp)
			for j = xmlstrt To UBound(xmltemplate)
				If configtmp2(i, 0) IsNot Nothing Then
					Dim tmp As String = configtmp2(i, 0)
					Dim tmp2 As String = xmltemplate(j)
					Dim tmp3 As string = xmltemplate(j)
					if InStr(1, xmltemplate(j).ToLower, configtmp2(i, 0).ToLower) > 0 'found, now replace all val with value
						xmltemplate(j) = xmltemplate(j).Replace(">val<", ">" & configtmp2(i, 1) & "<")
					End If
				End If
			Next
		Next
		'============================================== Create & INSERT Template
		Dim xmlstr As New StringBuilder
		For i = 1 To UBound(xmltemplate)
			Dim temp As String = xmltemplate(i)
			xmlstr.Append("<Table>" & xmltemplate(i))
		Next
		Dim hashval As String = GenerateHash(xmlstr.ToString)
		Try
			Dim query As String = "DELETE FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] WHERE ProductFamily = '" & _
			ProductFamily & "' and CustomerID ='" & CustomerID & "' and Firmware = '" & Firmware & "'"

			'OLD query = query & " INSERT INTO [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] VALUES ('" & _
			'ProductFamily & "','" & Drawing & "','" & CustomerName & "','" & CustomerID & "','" & Firmware & "','"  & _
			'Hashval & "','" & ProgressMax & "','" & xmlstr.ToString & "')"
			query = query & " INSERT INTO [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] VALUES ('" & _
			ProductFamily & "','" & Drawing & "','" & CustomerName & "','" & CustomerID & "','" & Firmware & "','" & _
			Hashval & "','" & ProgressMax & "','" & xmlstr.ToString & "')"
			rsData = dcnDB.Execute(query)
			rsData = Nothing
			Return "Pass-Database Updated"
		Catch ex As Exception
			rsData.Close()
			Return "Fail,4,Delete/Insert Statement After GET Template"
		End Try
	End Function
	Private Function GetSQLCustomerScript(ByVal ProductFamily As String, ByVal CustomerID As String, ByVal Firmware As String, ByVal HashVal As String) As Boolean
		GetSQLCustomerScript = False
		Dim i As Integer = 0
		Dim Draw As String = ""
		Dim Name As String = ""
		Dim Hash As String = ""
		Dim PMax As String = ""
		Dim xmlscript As String = ""
		'============================================== Ensure Test Files directory exists
		Dim testFilesDir As String = IO.Path.Combine(Application.StartupPath, "Test Files")
		Dim inifile As String = IO.Path.Combine(testFilesDir, Firmware & "-" & ProductFamily & "-" & CustomerID & ".xml")
		Try
			IO.Directory.CreateDirectory(testFilesDir)
		Catch ex As Exception
			CommLog("CreateDirectory failed [" & testFilesDir & "]: " & ex.ToString)
		End Try
		'============================================== Delete Existing File (If Exists)
		Try
			If System.IO.File.Exists(inifile) = True Then
				Dim FileInfo as fileInfo = new FileInfo(inifile)
				fileInfo.IsReadOnly = false
				File.Delete(inifile)
			End If
		Catch ex As Exception
			_TestFail = True
			CommLog("GetSQLCustomerScript() XML Delete Error: " & ex.ToString)
			Return False
		End Try
		'============================================== Now Get The Correct File
		Try
			CheckSQLState()
			dcnDB.Open(GetSQLConnection("SRFN_CustomerValues"))
			Dim query As String = "Select * FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] where "
			query = query & "ProductFamily = '" & ProductFamily & "' and CustomerID = '" & CustomerID & "' and HashVal  like '" & HashVal & "'"
			rsData = dcnDB.Execute(query)
			'ProductFamily = RTrim(rsData.Fields(0).Value)
			Draw = RTrim(CStr(rsData.Fields(2).Value))
			'firmware = RTrim(CStr(rsData.Fields(2).Value))
			Name = RTrim(CStr(rsData.Fields(3).Value))
			'CustomerID = RTrim(rsData.Fields(4).Value)
			Hash = RTrim(CStr(rsData.Fields(6).Value))
			PMax = RTrim(CStr(rsData.Fields(7).Value))
			xmlscript = RTrim(CStr(rsData.Fields(8).Value))
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			CommLog("GetSQLCustomerScript() SQL Retrieve Error: " & ex.ToString)
			_TestFail = True
			Return False
		End Try
		'============================================== Write XML to new Text-XML File
		Dim setoptionvals As String = ""
		setoptionvals = "<?xml version=""1.0"" standalone=""yes""?>" & vbCrLf
		setoptionvals += "<NewDataSet>" & vbCrLf
		setoptionvals += "<HashTable>" & vbCrLf
		setoptionvals += "<ProductFamily>" & ProductFamily & "</ProductFamily>" & vbCrLf
		setoptionvals += "<Drawing>" & Draw & "</Drawing>" & vbCrLf
		setoptionvals += "<CustomerName>" & Name & "</CustomerName>" & vbCrLf
		setoptionvals += "<CustomerID>" & CustomerID & "</CustomerID>" & vbCrLf
		setoptionvals += "<HashValue>" & Hash & "</HashValue>" & vbCrLf
		setoptionvals += "<ProgressMax>" & PMax & "</ProgressMax>" & vbCrLf
		setoptionvals += "</HashTable>" & vbCrLf
		setoptionvals += xmlscript.Replace("><", ">" & Environment.NewLine & "<")
		setoptionvals += Environment.NewLine & "</NewDataSet>"
		'============================================== Attempt the Write
		CommLog("Writing script file: " & inifile)
		Try
			If File.Exists(inifile) Then SetAttr(inifile, vbNormal)
			System.IO.File.WriteAllText(inifile, setoptionvals)
			SetAttr(inifile, vbReadOnly)
			GetSQLCustomerScript = True
			CommLog("Script file written OK")
		Catch ex As Exception
			_TestFail = True
			CommLog("GetSQLCustomerScript() XML Write Error [" & inifile & "]: " & ex.ToString)
		End Try
	End Function
	'============================================== Check SQL State Close if Open
	Private Sub CheckSQLState()
		Try
			If dcnDB IsNot Nothing Then
				If dcnDB.State > 0 Then dcnDB.Close()
			End If
		Catch ex As Exception
		End Try
	End Sub
	'============================================== Get Connection Strings
	Private Function GetSQLConnection(ByVal TableName As String) As String
		Dim cs As String = ""
		Select Case TableName
			Case "SRFN_CustomerValues" : cs = CustomerValuesConnStr
			Case "SRFN_TestResults"    : cs = TestResultsConnStr
			Case "DLL_Revision"        : cs = DLLRevisionConnStr
		End Select
		If cs <> "" Then
			CommLog("GetSQLConnection [" & TableName & "] using in-memory string: " & cs.Substring(0, Math.Min(80, cs.Length)) & "...")
			Return cs
		End If
		Try
			Dim doc As New XmlDocument()
			doc.Load(Application.StartupPath & "\SQLValues.xml")
			cs = doc.SelectSingleNode("Data/Database/" & TableName).InnerText
			CommLog("GetSQLConnection [" & TableName & "] using XML fallback: " & cs.Substring(0, Math.Min(80, cs.Length)) & "...")
			Return cs
		Catch ex As Exception
		End Try
		CommLog("GetSQLConnection [" & TableName & "] ERROR: no connection string found (in-memory empty, XML fallback failed)")
		Return ""
	End Function
#End Region

#Region "RA6 Self-Contained Implementations"
	'--- Process runner: used by ExecMassErase, ExecUpdateFirmware, ExecToggleUSB
	Public Shared Async Function RunRA6Command(cmdPath As String, args As String) As Task(Of Boolean)
		Dim fullCmd As String = "/c cd """ & cmdPath & """ & " & args & " -run"
		BatchLog("  > rfp-cli: " & args)
		Return Await Task.Run(Of Boolean)(Function() RunRA6Sync(fullCmd))
	End Function
	Private Shared Function RunRA6Sync(fullCmd As String) As Boolean
		Dim success As Boolean = False
		Try
			Dim psi As New ProcessStartInfo() With {
				.FileName = "C:\Windows\System32\cmd.exe",
				.Arguments = fullCmd,
				.RedirectStandardOutput = True,
				.RedirectStandardError = True,
				.UseShellExecute = False,
				.CreateNoWindow = True
			}
			Using p As New Process()
				p.StartInfo = psi
				p.Start()
				Dim errTask As Task = Task.Run(Sub()
					Dim errLine As String = p.StandardError.ReadLine()
					Do While errLine IsNot Nothing
						CommLog("rfp-cli err: " & errLine)
						errLine = p.StandardError.ReadLine()
					Loop
				End Sub)
				Dim outLine As String = p.StandardOutput.ReadLine()
				Do While outLine IsNot Nothing
					CommLog("rfp-cli: " & outLine)
					If outLine.Trim().Equals("Operation successful", StringComparison.OrdinalIgnoreCase) Then success = True
					outLine = p.StandardOutput.ReadLine()
				Loop
				errTask.Wait()
				p.WaitForExit()
			End Using
		Catch ex As Exception
			CommLog("RunRA6Sync error: " & ex.Message)
		End Try
		Return success
	End Function
	Private Shared Function FindKeyFile(ra6Dir As String, pattern As String) As String
		Try
			Dim files As String() = IO.Directory.GetFiles(ra6Dir, pattern, IO.SearchOption.TopDirectoryOnly)
			If files.Length > 0 Then Return files(0)
		Catch
		End Try
		Return ""
	End Function

	Private Shared Async Function FlashHex(hexPath As String) As Task(Of Boolean)
		Dim ra6Dir As String = IO.Path.GetDirectoryName(hexPath)
		Dim secKey As String = FindKeyFile(ra6Dir, "dlm-ssd-*.rkey")
		Dim nonsecKey As String = FindKeyFile(ra6Dir, "dlm-nsecsd-*.rkey")
		Dim args As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -a -file " & Chr(34) & hexPath & Chr(34)
		If secKey <> "" AndAlso nonsecKey <> "" Then
			args &= " -fo seckey " & Chr(34) & secKey & Chr(34) & " -fo nonseckey " & Chr(34) & nonsecKey & Chr(34)
		End If
		Return Await RunRA6Command(RA6ProgPath, args)
	End Function

	'--- Firmware SQL operations (ADODB — same protocol as Browse/Check) -----------

	Private Shared Function OpenFirmwareConn() As ADODB.Connection
		Dim conn As New ADODB.Connection()
		conn.ConnectionTimeout = 30
		Dim cs As String = FirmwareConnStr
		If cs = "" Then cs = "Provider=SQLOLEDB;Data Source=" & SqlServerName & ";Integrated Security=SSPI;Initial Catalog=Aclara_CustomerSpecific;"
		conn.Open(cs)
		Return conn
	End Function

	Public Shared Function FetchFirmwareList(productFamily As String) As List(Of String)
		Dim items As New List(Of String)
		Dim conn As ADODB.Connection = Nothing
		Try
			conn = OpenFirmwareConn()
			Dim rs As ADODB.Recordset = conn.Execute(
				"SELECT FileName FROM [dbo].[Aclara_Firmware] WHERE ProductFamily = '" &
				productFamily.Replace("'", "''") & "' ORDER BY FileName")
			Do While Not rs.EOF
				items.Add(rs.Fields("FileName").Value.ToString().Trim())
				rs.MoveNext()
			Loop
			rs.Close()
		Catch ex As Exception
			CommLog("FetchFirmwareList error: " & ex.Message)
		Finally
			Try
				If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
			Catch
			End Try
		End Try
		Return items
	End Function

	Private Shared Sub EnsureFirmwareTableExists()
		Dim conn As ADODB.Connection = Nothing
		Try
			conn = OpenFirmwareConn()
			conn.Execute(
				"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Aclara_Firmware') " &
				"CREATE TABLE [dbo].[Aclara_Firmware] (" &
				"    [Id] INT IDENTITY(1,1) PRIMARY KEY," &
				"    [ProductFamily] NVARCHAR(50) NOT NULL," &
				"    [FileName] NVARCHAR(255) NOT NULL," &
				"    [FileData] VARBINARY(MAX) NOT NULL," &
				"    [UploadDate] DATETIME NOT NULL DEFAULT GETDATE()" &
				")")
		Catch ex As Exception
			CommLog("EnsureFirmwareTableExists error: " & ex.Message)
		Finally
			Try
				If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
			Catch
			End Try
		End Try
	End Sub

	Public Shared Function UploadFirmwareToSQL(productFamily As String, fileName As String, data As Byte()) As Boolean
		Try
			EnsureFirmwareTableExists()
			Dim conn As ADODB.Connection = Nothing
			Try
				conn = OpenFirmwareConn()
				Dim rsCheck As ADODB.Recordset = conn.Execute(
					"SELECT COUNT(1) FROM [dbo].[Aclara_Firmware] WHERE ProductFamily='" &
					productFamily.Replace("'", "''") & "' AND FileName='" &
					fileName.Replace("'", "''") & "'")
				Dim exists As Boolean = (CInt(rsCheck.Fields(0).Value) > 0)
				rsCheck.Close()

				Dim cmd As New ADODB.Command()
				cmd.ActiveConnection = conn
				cmd.CommandTimeout = 120

				If exists Then
					cmd.CommandText = "UPDATE [dbo].[Aclara_Firmware] SET FileData=?, UploadDate=GETDATE() WHERE ProductFamily=? AND FileName=?"
					Dim pData As ADODB.Parameter = cmd.CreateParameter("", ADODB.DataTypeEnum.adLongVarBinary, ADODB.ParameterDirectionEnum.adParamInput, data.Length)
					cmd.Parameters.Append(pData)
					pData.AppendChunk(data)
					cmd.Parameters.Append(cmd.CreateParameter("", ADODB.DataTypeEnum.adVarWChar, ADODB.ParameterDirectionEnum.adParamInput, 50, productFamily))
					cmd.Parameters.Append(cmd.CreateParameter("", ADODB.DataTypeEnum.adVarWChar, ADODB.ParameterDirectionEnum.adParamInput, 255, fileName))
				Else
					cmd.CommandText = "INSERT INTO [dbo].[Aclara_Firmware] (ProductFamily,FileName,FileData,UploadDate) VALUES (?,?,?,GETDATE())"
					cmd.Parameters.Append(cmd.CreateParameter("", ADODB.DataTypeEnum.adVarWChar, ADODB.ParameterDirectionEnum.adParamInput, 50, productFamily))
					cmd.Parameters.Append(cmd.CreateParameter("", ADODB.DataTypeEnum.adVarWChar, ADODB.ParameterDirectionEnum.adParamInput, 255, fileName))
					Dim pData As ADODB.Parameter = cmd.CreateParameter("", ADODB.DataTypeEnum.adLongVarBinary, ADODB.ParameterDirectionEnum.adParamInput, data.Length)
					cmd.Parameters.Append(pData)
					pData.AppendChunk(data)
				End If
				cmd.Execute()
				Return True
			Finally
				Try
					If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
				Catch
				End Try
			End Try
		Catch ex As Exception
			CommLog("UploadFirmwareToSQL error: " & ex.ToString)
			Return False
		End Try
	End Function

	Public Shared Function DeleteFirmwareFromSQL(productFamily As String, fileName As String) As Boolean
		Dim conn As ADODB.Connection = Nothing
		Try
			conn = OpenFirmwareConn()
			conn.Execute("DELETE FROM [dbo].[Aclara_Firmware] WHERE ProductFamily='" &
				productFamily.Replace("'", "''") & "' AND FileName='" &
				fileName.Replace("'", "''") & "'")
			Return True
		Catch ex As Exception
			CommLog("DeleteFirmwareFromSQL error: " & ex.ToString)
			Return False
		Finally
			Try
				If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
			Catch
			End Try
		End Try
	End Function

	'--- Downloads firmware hex from Aclara_Firmware SQL table to local RA6Files\ by exact filename
	Public Shared Function DownloadFirmwareHexByName(fileName As String, productFamily As String) As String
		Dim conn As ADODB.Connection = Nothing
		Try
			conn = OpenFirmwareConn()
			Dim rs As ADODB.Recordset = conn.Execute(
				"SELECT FileName, FileData FROM [dbo].[Aclara_Firmware] WHERE ProductFamily = '" &
				productFamily.Replace("'", "''") & "' AND FileName = '" &
				fileName.Replace("'", "''") & "'")
			If rs.EOF Then Return ""
			Dim fn As String = rs.Fields("FileName").Value.ToString()
			Dim bytes As Byte() = CType(rs.Fields("FileData").Value, Byte())
			rs.Close()
			Dim ra6Dir As String = IO.Path.Combine(Application.StartupPath, "RA6Files")
			IO.Directory.CreateDirectory(ra6Dir)
			Dim localPath As String = IO.Path.Combine(ra6Dir, fn)
			IO.File.WriteAllBytes(localPath, bytes)
			Return localPath
		Catch ex As Exception
			CommLog("DownloadFirmwareHexByName error: " & ex.ToString)
			Return ""
		Finally
			Try
				If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
			Catch
			End Try
		End Try
	End Function

	'--- Downloads firmware hex from Aclara_Firmware SQL table to local RA6Files\
	Private Shared Function DownloadFirmwareHexByVersion(version As String, productFamily As String) As String
		Dim conn As ADODB.Connection = Nothing
		Try
			conn = OpenFirmwareConn()
			Dim rs As ADODB.Recordset = conn.Execute(
				"SELECT TOP 1 FileName, FileData FROM [dbo].[Aclara_Firmware] WHERE ProductFamily = '" &
				productFamily.Replace("'", "''") & "' AND FileName LIKE '%" &
				version.TrimStart("0"c).Replace("'", "''") & "%'")
			If rs.EOF Then Return ""
			Dim fn As String = rs.Fields("FileName").Value.ToString()
			Dim bytes As Byte() = CType(rs.Fields("FileData").Value, Byte())
			rs.Close()
			Dim ra6Dir As String = IO.Path.Combine(Application.StartupPath, "RA6Files")
			IO.Directory.CreateDirectory(ra6Dir)
			Dim localPath As String = IO.Path.Combine(ra6Dir, fn)
			IO.File.WriteAllBytes(localPath, bytes)
			Return localPath
		Catch ex As Exception
			CommLog("DownloadFirmwareHexByVersion error: " & ex.ToString)
			Return ""
		Finally
			Try
				If conn IsNot Nothing AndAlso conn.State > 0 Then conn.Close()
			Catch
			End Try
		End Try
	End Function
	'--- Default MassEraseAction implementation
	Public Shared Async Function ExecMassErase() As Task(Of Boolean)
		Try
			BatchLog("  > MassErase: rfp-cli -erase-chip")
			Dim ok As Boolean = Await RunRA6Command(RA6ProgPath, "rfp-cli -t E2l -d RA -if uart -s 1.5M -erase-chip")
			BatchLog("  > MassErase: " & If(ok, "OK", "FAILED"))
			Return ok
		Catch ex As Exception
			CommLog("ExecMassErase error: " & ex.Message)
			Return False
		End Try
	End Function
	'--- Default UpdateFirmwareAction implementation
	Public Shared Async Function ExecUpdateFirmware(version As String) As Task(Of Boolean)
		Try
			BatchLog("  > UpdateFirmware: version=" & version)
			Dim hexPath As String = DownloadFirmwareHexByVersion(version, CurrentProductFamily)
			If hexPath = "" Then
				BatchLog("  > UpdateFirmware: hex not found in SQL for version=" & version)
				Return False
			End If
			Dim ok As Boolean = Await FlashHex(hexPath)
			BatchLog("  > UpdateFirmware: " & If(ok, "OK", "FAILED"))
			Return ok
		Catch ex As Exception
			CommLog("ExecUpdateFirmware error: " & ex.Message)
			Return False
		End Try
	End Function
	'--- Default ToggleUSBAction implementation
	Public Shared Async Function ExecToggleUSB(delayMs As Integer) As Task
		Try
			If RelayPort = "" Then
				BatchLog("  > ToggleUSB: no relay port configured")
				Return
			End If
			_relay.Open(RelayPort)
			_relay.SetChannel(1, False)
			Dim offState As String = _relay.GetChannel(1)
			UsbIsOn = (offState = "1")
			If offState = "0" Then
				BatchLog("  > USB OFF (Open) confirmed")
			Else
				BatchLog("  > USB OFF FAILED state=" & If(offState IsNot Nothing, offState, "none"))
			End If
			Await Task.Delay(5000)
			_relay.SetChannel(1, True)
			Dim onState As String = _relay.GetChannel(1)
			UsbIsOn = (onState = "1")
			If onState = "1" Then
				BatchLog("  > USB ON (Closed) confirmed")
			Else
				BatchLog("  > USB ON FAILED state=" & If(onState IsNot Nothing, onState, "none"))
			End If
			Await Task.Delay(500)
			BatchLog("  > Reset Release")
			Await RunRA6Command(RA6ProgPath, "rfp-cli -t E2l -d RA -if uart -s 1.5M")
			Await Task.Delay(1000)
			BatchLog("  > USB toggle complete")
		Catch ex As Exception
			CommLog("ExecToggleUSB error: " & ex.Message)
		End Try
	End Function
	'--- Default VerifyUSBCommAction implementation
	Public Shared Async Function ExecVerifyUSBComm() As Task
		Await ExecToggleUSB(0)
		If Not UsbIsOn Then
			BatchLog("  > VerifyUSBComm: USB is OFF after cycle, toggling ON")
			Await ExecToggleUSB(0)
		End If
		BatchLog("  > VerifyUSBComm: USB " & If(UsbIsOn, "ON (Closed) - reading MAC", "still OFF - MAC read may fail"))
	End Function
	'--- Relay helper for Form1 button handlers (set channel, verify, return state string)
	Public Shared Function RelaySetAndGet(channel As Integer, state As Boolean) As String
		Try
			If RelayPort = "" Then Return Nothing
			_relay.Open(RelayPort)
			_relay.SetChannel(channel, state)
			Dim result As String = _relay.GetChannel(channel)
			UsbIsOn = (result = "1")
			Return result
		Catch ex As Exception
			CommLog("RelaySetAndGet error: " & ex.Message)
			Return Nothing
		End Try
	End Function
#End Region

End Class
