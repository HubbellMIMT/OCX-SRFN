Option Strict On
Imports System.IO
Imports System.IO.Ports
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Xml
Imports SerialLibrary
Imports SRFN.Communication
Public Class Form1
	Inherits Form
	Implements IMainForm

	Public Event ModeSwitchRequested(mode As String)

	Private WithEvents _commManager As CommManager2
	Private _frmCommPorts As frmCommPorts = Nothing
	Private _frmBatch As frmBatch = Nothing
	Private _sqlServer As String = ""
	Private _firmwareConnStr As String = ""
	Private _searchMode As Boolean = False
	Public dcnDB As New ADODB.Connection
	Public rsData As New ADODB.Recordset
	Public Prodnum(5, 2) As String
	Dim MeterType(6, 25) As String
	Public DLLver As String = ""
	Private Shared _instance As Form1

#Region "App Logging"
	Private Shared _logPath As String = ""

	Public Shared Sub InitLogging()
		Try
			Dim logDir As String = IO.Path.Combine(Application.StartupPath, "Logs")
			IO.Directory.CreateDirectory(logDir)
			_logPath = IO.Path.Combine(logDir, "ErrorLog_" & Now.ToString("yyyyMMdd") & ".txt")
		Catch
			_logPath = IO.Path.Combine(IO.Path.GetTempPath(), "SRFN_ErrorLog_" & Now.ToString("yyyyMMdd") & ".txt")
		End Try
		IO.File.AppendAllText(_logPath, "===== Session started " & Now.ToString("yyyy-MM-dd HH:mm:ss") & " [" & Application.StartupPath & "] =====" & Environment.NewLine)
	End Sub

	Public Shared Sub AppLog(message As String)
		Dim ts As String = Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
		Dim entry As String = "[" & ts & "] " & message
		If _logPath <> "" Then
			Try
				IO.File.AppendAllText(_logPath, entry & Environment.NewLine)
			Catch
			End Try
		End If
		If _instance IsNot Nothing AndAlso Not _instance.IsDisposed Then
			Try
				If _instance.InvokeRequired Then
					_instance.BeginInvoke(Sub() _instance.AppendLog(entry))
				Else
					_instance.AppendLog(entry)
				End If
			Catch
			End Try
		End If
	End Sub

	Private Sub AppendLog(entry As String)
		txtResults.AppendText(entry & Environment.NewLine)
		If entry.IndexOf("Error", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
		   entry.IndexOf("Fail", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
		   entry.IndexOf("Exception", StringComparison.OrdinalIgnoreCase) >= 0 Then
			btnOpenLog.Visible = True
		End If
	End Sub

	Friend Sub SetResultsBackColor(c As Color)
		If txtResults.InvokeRequired Then
			txtResults.BeginInvoke(New Action(Sub() txtResults.BackColor = c))
		Else
			txtResults.BackColor = c
		End If
	End Sub

	Private Sub btnOpenLog_Click(sender As Object, e As EventArgs) Handles btnOpenLog.Click
		Try
			Process.Start("explorer.exe", IO.Path.GetDirectoryName(_logPath))
		Catch ex As Exception
			MsgBox("Cannot open folder: " & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
		End Try
	End Sub
	Private Sub Handle_ThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
		AppLog("UNHANDLED UI EXCEPTION: " & e.Exception.ToString())
	End Sub
	Private Sub Handle_AppDomainException(sender As Object, e As UnhandledExceptionEventArgs)
		AppLog("UNHANDLED EXCEPTION: " & CType(e.ExceptionObject, Exception).ToString())
	End Sub
#End Region

	'============================================
	'Item number	Title	Variation details	Custom label (SKU)	Available quantity	Format	Currency	Start price	Auction Buy It Now price	Reserve price	Current price	Sold quantity	Views (future)	Watchers	Bids	Start date	End date	eBay category 1 name	eBay category 1 number	eBay category 2 name	eBay category 2 number	Condition	eBay Product ID(ePID)	Listing site	P:UPC
	Private Sub btnSendDebug_Click(sender As Object, e As EventArgs) Handles btnSendDebug.Click
		'Private Async Sub btnSendDebug_Click(sender As Object, e As EventArgs) Handles btnSendDebug.Click
		'Await SendDebugMsg(txtcommand.Text)
		'ReadInInv 
	End Sub
	Public Sub New()
		InitLogging()
		InitializeComponent()
		_instance = Me
		AddHandler Application.ThreadException, AddressOf Handle_ThreadException
		AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf Handle_AppDomainException
		If txtProductFamily.SelectedIndex <> -1 Then txtProductFamily.SelectedIndex = 1
	End Sub
	Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		GetFormListXml   '    GetFormValues()
		GetFormValues()
		'SetFormXml() ' = SetFormValues()
		LoadProductFamily()
			'ShowPorts()
			LoadFW() '....................................txtNicFW
			txtFormID.Items.Clear
		txtFormID.Text = ""
		txtProductFamily.Text = ""
		txtProductFamily.SelectedIndex = 0
		If txtFormID.Items.Count > 0 Then txtFormID.SelectedIndex = 0
		If txtRFCarrier.Items.Count > 0 Then txtRFCarrier.SelectedIndex = 0
		If txtProductFamily.Items.Count > 0 Then txtProductFamily.SelectedIndex = 0
		DLLver = SRFN.Communication.CommManager2.DLLVersion
		Text = "Team SRFN Integration Test w/Security " & DLLver & "  [" & Application.StartupPath & "]"
		txtPassword.Text = ""
		RA6Buttons(False)
		If txtProductFamily.Items.Count > 3 Then txtProductFamily.SelectedIndex = 3
		UpdatePortStatus()
		If RA6ProgPath.Text = "" Then FindRA6ProgPath()
		If txtSQLServer.Text <> "" Then _sqlServer = txtSQLServer.Text
		CommManager2.LogAction = AddressOf AppLog
		CommManager2.ColorAction = AddressOf SetResultsBackColor
		CommManager2.RA6ProgPath = RA6ProgPath.Text
		CommManager2.RelayPort = txtRelayPort.Text
		CommManager2.SqlServerName = _sqlServer
		CommManager2.CurrentProductFamily = txtProductFamily.Text
		CommManager2.FirmwareConnStr = BuildFirmwareConnStr()
		CommManager2.CustomerValuesConnStr = SRFN_CustomerValues.Text.Trim()
		CommManager2.TestResultsConnStr = SRFN_TestResults.Text.Trim()
		CommManager2.DLLRevisionConnStr = DLL_Revision.Text.Trim()
		AppLog("App started - log: " & _logPath)
		btnLockForm.Enabled = (txtCustomerID.Text.Length >= 7)
		Me.Refresh()
	End Sub

	Private Sub txtCustomerID_TextChanged(sender As Object, e As EventArgs) Handles txtCustomerID.TextChanged
		btnLockForm.Enabled = (txtCustomerID.Text.Length >= 7)
	End Sub

	Private Sub FindRA6ProgPath()
		Try
			Dim searchRoots As String() = {"C:\Program Files (x86)\Renesas Electronics\Programming Tools",
			                               "C:\Program Files\Renesas Electronics\Programming Tools"}
			For Each root As String In searchRoots
				If Directory.Exists(root) Then
					Dim files As String() = Directory.GetFiles(root, "rfp-cli.exe", SearchOption.AllDirectories)
					If files.Length > 0 Then
						RA6ProgPath.Text = Path.GetDirectoryName(files(0))
						SetFormXml()
						Return
					End If
				End If
			Next
		Catch ex As Exception
		End Try
	End Sub
	Private Sub LoadFW()
		RA6Buttons(False)
		Try
			Dim di As New IO.DirectoryInfo(Application.StartupPath & "\RA6Files")
			Dim aryFi As IO.FileInfo() = di.GetFiles("*.hex")
			Dim fi As IO.FileInfo
			txtRFCarrier.Items.Add("00")
			For Each fi In aryFi
				Dim str As String = Mid(fi.Name, InStr(fi.Name, "V") + 1)
				str = Mid(str, 1, InStr(str, " "))
				txtRFCarrier.Items.Add(str)
			Next
		Catch ex As Exception
		End Try
	End Sub
#Region "RA6 Module"
	Public Async Function VirginDelay() As Task(Of Boolean) Implements IMainForm.VirginDelay
		Dim response As String = Await SendMsg("virgindelay")
		Return response.Contains("Signature erased")
	End Function
	Private Sub btn_CheckDeploy_Click(sender As Object, e As EventArgs) Handles btn_CheckDeploy.Click
		Dim cmdpath As String = RA6ProgPath.Text
		Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -sig"
		StartProcess(cmdpath, msg)
	End Sub
	Private Async Sub btnRA6FW_Keys_Click(sender As Object, e As EventArgs)
		'............. Update Firmware & Set Keys
		'............. Select Chosen Firmware Version
		Await VirginDelay()
		Dim file As String = ""
		Dim di As New IO.DirectoryInfo(Application.StartupPath & "\RA6Files")
		Dim aryFi As IO.FileInfo() = di.GetFiles("*.hex")
		Dim fi As IO.FileInfo
		If SelectedFirmwareName = "" Then
			txtResults.Text = "No Firmware Selected"
			Exit Sub
		End If
		For Each fi In aryFi
			If InStr(fi.Name, SelectedFirmwareName) > 0 Then
				file = fi.Name
				Exit For
			End If
		Next
		'Dim cmdpath As String = RA6ProgPath.Text
		'Dim RA6ProgPath As string = "C:\Program Files (x86)\Renesas Electronics\Programming Tools\Renesas Flash Programmer V3.09"
		'Dim RA6FilesPath As string = "C:\Development\SRFN Projects\SFN DTLS Projects\EP Integration Test v3.5.00 - Renesas\SRFN.TestApp\bin\Debug\RA6Files"

		'Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -a -file ""RA6FilesPath\hexfile"" -fo seckey ""RA6FilesPath\dlm-ssd-test.rkey"" -fo nonseckey ""RA6FilesPath\dlm-nsecsd-test.rkey"""
		'msg = msg.Replace("hexfile",file)
		'msg = msg.Replace("RA6FilesPath",RA6FilesPath)
		'cmdpath = RA6ProgPath
		'StartProcess(cmdpath, msg)
	End Sub
	Private Async Sub btnEnableDebug_Click(sender As Object, e As EventArgs) Handles btnEnableDebug.Click
		Await SendMsg("debugportenabled 1")
	End Sub
	Private Sub ListFirmware(FirmwarePath as String)
		txtResults.Clear()
		txtResults.TextAlign = HorizontalAlignment.Left
		Me.Refresh
		Dim di As New IO.DirectoryInfo(FirmwarePath)
		Dim aryFi As IO.FileInfo() = di.GetFiles("*.hex")
		Dim fi As IO.FileInfo
		For Each fi In aryFi
			txtResults.AppendText(fi.Name & vbLf)
		Next
	End Sub
	Private Async Function StartProcess(cmdpath As String, msg As String) As Task
		txtResults.Clear()
		txtResults.BackColor = Color.Yellow
		SetRA6ButtonsEnabled(False)
		Me.Refresh
		Dim Result As Boolean = False
		Dim fullCmd As String = "/c cd """ & cmdpath & """ & " & msg & " -run"
		txtResults.AppendText("CMD: " & fullCmd & Environment.NewLine)
		Dim MyStartInfo As New ProcessStartInfo() With {
			.FileName = "C:\Windows\System32\cmd.exe",
			.Arguments = fullCmd,
			.RedirectStandardError = True,
			.RedirectStandardOutput = True,
			.UseShellExecute = False,
			.CreateNoWindow = True
			}
		Dim MyProcess As Process = New Process() With {
			.StartInfo = MyStartInfo,
			.EnableRaisingEvents = True,
			.SynchronizingObject = Me
			}

		MyProcess.Start()
		MyProcess.BeginErrorReadLine()
		MyProcess.BeginOutputReadLine()

		CurrentProcessID = MyProcess.Id
		txtResults.TextAlign = HorizontalAlignment.Left
		AddHandler MyProcess.OutputDataReceived,
				Sub(sender As Object, e As DataReceivedEventArgs)
					If e.Data IsNot Nothing Then
						BeginInvoke(New MethodInvoker(
						Sub()
							If e.Data.Length > 2 Then
								txtResults.AppendText(e.Data + Environment.NewLine)
								txtResults.ScrollToCaret()
							End if
							If e.Data.Equals("Operation successful", StringComparison.OrdinalIgnoreCase) Then
								Result = True
							End If
						End Sub))
					End If
				End Sub

		AddHandler MyProcess.ErrorDataReceived,
			Sub(sender As Object, e As DataReceivedEventArgs)
				If e.Data IsNot Nothing Then
					BeginInvoke(New MethodInvoker(
					Sub()
						txtResults.AppendText(e.Data + Environment.NewLine)
						txtResults.ScrollToCaret()
					End Sub))
				End If
			End Sub

		AddHandler MyProcess.Exited,
			Sub(source As Object, ev As EventArgs)
				MyProcess.Close()
				If MyProcess IsNot Nothing Then MyProcess.Dispose()
				BeginInvoke(New MethodInvoker(Sub()
					txtResults.BackColor = If(Result, Color.LightGreen, Color.White)
					SetRA6ButtonsEnabled(True)
					Me.Refresh()
				End Sub))
			End Sub
	End Function
	Private CurrentProcessID As Integer = -1
	Async Sub ReadFirmware()
		Await SendMsg("comDeviceFirmwareVersion")
	End Sub
#Region "Ra6 Buttons"
	Private Sub btn_DeployEnable_Click(sender As Object, e As EventArgs) Handles btn_DeployEnable.Click
		Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -dlm DPL"
		Dim cmdpath As String = RA6ProgPath.Text
		StartProcess(cmdpath, msg)
	End Sub
	'Private Async Sub btn_DeployDisable1_Click(sender As Object, e As EventArgs) Handles btn_DeployDisable1.Click
	'Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -dlm NSECSD -auth nonseckey 00112233445566778899AABBCCDDEEFF -run"
	'Dim cmdpath As String = "C:\Program Files (x86)\Renesas Electronics\Programming Tools\Renesas Flash Programmer V3.09"
	'StartProcess(cmdpath, msg)
	'Threading.Thread.Sleep(4000)
	'Dim msg2 As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -dlm SSD -auth seckey 00112233445566778899AABBCCDDEEFF -run"
	'StartProcess(cmdpath, msg)
	'End Sub
	'Private Sub btn_DeployDisable2_Click_1(sender As Object, e As EventArgs) 
	'Dim msg2 As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -dlm SSD -auth seckey 00112233445566778899AABBCCDDEEFF -run"
	'End Sub
	'============================== RA6 CommManager
	Private Async Function RA6Msg(ByVal msg As String) As Task
		Dim delay As integer = 0
		Dim dismsg As String = ""
		Dim Progpath As String = RA6ProgPath.Text
		Select Case msg
			Case "MassErase"
				msg = "rfp-cli -t E2l -d RA -if uart -s 1.5M -erase-chip"
				delay = 5000
				dismsg = "Mass Erase"
			Case "ResetRelease"
				msg = "rfp-cli -t E2l -d RA -if uart -s 1.5M"
				delay = 3000
				dismsg = "Reset Release"
		End Select

		If _commManager Is Nothing Then _commManager = New CommManager2()

		Dim response As String = Await _commManager.RA6Message(msg, ProgPath, Delay)
		If response = "Pass" Then
			txtResults.Text = dismsg & " - PASS"
		Else
			txtResults.Text = dismsg & " - FAIL"
		End If
	End Function
	Private Sub btnResetRelease_Click(sender As Object, e As EventArgs) Handles btnResetRelease.Click
		Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M"
		Dim progpath As String = RA6ProgPath.Text '"C:\Program Files (x86)\Renesas Electronics\Programming Tools\Renesas Flash Programmer V3.09"
		StartProcess(progpath, msg)
	End Sub
#End Region
	Private Sub LoadProductFamily()
		txtFormID.Items.Clear()
		For i = 0 To 6
			txtProductFamily.Items.Add(MeterType(i, 0))
		Next

	End Sub
	'.............................................SET XML VALUES
	Private Sub btnSaveSQL_Click(sender As Object, e As EventArgs)  '...... SAVE SQLVALUES.xml
		SetFormXml()
		Dim SQLValuesxml As String = Application.StartupPath & "\SQLValues.xml"
		Dim ShowForm As String = ""
		If chkShowForm.Checked = True Then
			ShowForm = "True"
		Else
			ShowForm = "False"
		End If
		Dim settings As New XmlWriterSettings()
		settings.Indent = True
		Dim FVxml As XmlWriter = XmlWriter.Create(SQLValuesxml, settings)
		With FVxml
			.WriteStartDocument()
			.WriteComment("XML Database.")
			.WriteStartElement("Data")
			.WriteStartElement("Database")

			.WriteStartElement("DLL_Revision")
			.WriteString(DLL_Revision.Text)
			.WriteEndElement()

			.WriteStartElement("SRFN_CustomerValues")
			.WriteString(SRFN_CustomerValues.Text)
			.WriteEndElement()

			.WriteStartElement("SRFN_TestResults")
			.WriteString(SRFN_TestResults.Text)
			.WriteEndElement()

			.WriteStartElement("Aclara_FirmwareConn")
			.WriteString(_firmwareConnStr)
			.WriteEndElement()

			'.WriteStartElement("SRFN_RA6_ConfigValues")
			'.WriteString(SRFN_RA6ConfigVals.Text)
			'.WriteEndElement()

			.WriteStartElement("txtSQLServer")
			.WriteString(txtSQLServer.Text)
			.WriteEndElement()

			.WriteStartElement("ShowForm")
			.WriteString(ShowForm)
			.WriteEndElement()

			.WriteStartElement("debugtest")
			.WriteString(txtdebug.Text)
			.WriteEndElement()
			.WriteEndElement()

			.WriteEndDocument()
			.Close()
		End With
		txtPassword.Text = ""
	End Sub
	Public Sub SetFormXml() Implements IMainForm.SetFormXml
		Dim FormValuesxml As String = Application.StartupPath & "\FormValues.xml"
		Select Case txtProductFamily.Text
			Case "SRFN-I-210+"
				Prodnum(0, 1) = txtCommPort.Text
			Case "SRFN-I-210+c"
				Prodnum(1, 1) = txtCommPort.Text
			Case "SRFN-KV2c", "AclaraRF3-KV2c"
				Prodnum(2, 1) = txtCommPort.Text
			Case "SRFN-I-210+c-RA6"
				Prodnum(3, 1) = txtCommPort.Text
			Case "AclaraRF3-I210+c"
				Prodnum(4, 1) = txtCommPort.Text
			Case "SRFN-EV2C-RA6"
				Prodnum(5, 1) = txtCommPort.Text
		End Select

		Dim settings As New XmlWriterSettings()
		settings.Indent = True
		Dim FVxml As XmlWriter = XmlWriter.Create(FormValuesxml, settings)
		With FVxml
			.WriteStartDocument()
			.WriteComment("XML Database.")
			.WriteStartElement("Data")

			.WriteStartElement("CurrentForm")

			.WriteStartElement("txtMeterSerial")
			.WriteString(txtMeterSerial.Text)
			.WriteEndElement()

			.WriteStartElement("txtMacAddress")
			.WriteString(txtMacAddress.Text)
			.WriteEndElement()

			.WriteStartElement("txtUtilitySerial")
			.WriteString(txtUtilitySerial.Text)
			.WriteEndElement()

			.WriteStartElement("txtCustomerID")
			.WriteString(txtCustomerID.Text)
			.WriteEndElement()

			.WriteStartElement("txtProductFamily")
			.WriteString(txtProductFamily.Text)
			.WriteEndElement()

			.WriteStartElement("txtCommPort")
			.WriteString(txtCommPort.Text)
			.WriteEndElement()

			.WriteStartElement("BaudRate")
			If txtProductFamily.Text = "SRFN-KV2c" Then
				.WriteString("9600")
			Else
				.WriteString("38400")
			End If
			.WriteEndElement()

			.WriteStartElement("txtFormID")
			.WriteString(txtFormID.Text)
			.WriteEndElement()

			.WriteStartElement("txtRFCarrier")
			.WriteString(txtRFCarrier.Text)
			.WriteEndElement()

			.WriteStartElement("txtPackPath")
			.WriteString(txtPackPath.Text)
			.WriteEndElement()

			.WriteStartElement("debugport")
			.WriteString(txtDebugPort.Text)
			.WriteEndElement()

			.WriteStartElement("relayport")
			.WriteString(txtRelayPort.Text)
			.WriteEndElement()

			.WriteStartElement("RA6ProgPath")
			.WriteString(RA6ProgPath.Text)
			.WriteEndElement()
			.WriteEndElement()

			.WriteStartElement("ProductFamilies")

			.WriteStartElement("Prod1")
			.WriteString(Prodnum(0, 0))
			.WriteEndElement()

			.WriteStartElement("Port1")
			.WriteString(Prodnum(0, 1))
			.WriteEndElement()

			.WriteStartElement("BaudRate1")
			.WriteString("9600")
			.WriteEndElement()

			.WriteStartElement("Prod2")
			.WriteString(Prodnum(1, 0))
			.WriteEndElement()

			.WriteStartElement("Port2")
			.WriteString(Prodnum(1, 1))
			.WriteEndElement()

			.WriteStartElement("BaudRate2")
			.WriteString("38400")
			.WriteEndElement()

			.WriteStartElement("Prod3")
			.WriteString(Prodnum(2, 0))
			.WriteEndElement()

			.WriteStartElement("Port3")
			.WriteString(Prodnum(2, 1))
			.WriteEndElement()

			.WriteStartElement("BaudRate3")
			.WriteString("9600")
			.WriteEndElement()

			.WriteStartElement("Prod4")
			.WriteString(Prodnum(3, 0))
			.WriteEndElement()

			.WriteStartElement("Port4")
			.WriteString(Prodnum(3, 1))
			.WriteEndElement()

			.WriteStartElement("BaudRate4")
			.WriteString("38400")

			.WriteEndElement()
			.WriteEndElement()

			.WriteEndDocument()
			.Close()
		End With
	End Sub
	Private Sub GetFormListXml()
		Try
			Dim doc As New XmlDocument()
			Dim i As Integer = 1
			Dim xmldoc As String = Application.StartupPath & "\FormList.xml"
			doc.Load(xmldoc)
			Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/SRFN-I-210")
			For Each node As XmlNode In nodes
				MeterType(0, 0) = "SRFN-I-210+"
				MeterType(0, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next

			i = 0
			nodes = doc.DocumentElement.SelectNodes("/Data/SRFN-I-210C")
			For Each node As XmlNode In nodes
				MeterType(1, 0) = "SRFN-I-210+c"
				MeterType(1, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next

			i = 0
			nodes = doc.DocumentElement.SelectNodes("/Data/SRFN-KV2c")
			For Each node As XmlNode In nodes
				MeterType(2, 0) = "SRFN-KV2c"
				MeterType(2, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next

			i = 0
			nodes = doc.DocumentElement.SelectNodes("/Data/SRFN-I-210C-RA6")
			For Each node As XmlNode In nodes
				MeterType(3, 0) = "SRFN-I-210+c-RA6"
				MeterType(3, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next

			i = 0
			nodes = doc.DocumentElement.SelectNodes("/Data/AclaraRF3-I210C")
			For Each node As XmlNode In nodes
				MeterType(4, 0) = "AclaraRF3-I210+c"
				MeterType(4, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next

			i = 0
			nodes = doc.DocumentElement.SelectNodes("/Data/AclaraRF3-KV2c")
			For Each node As XmlNode In nodes
				MeterType(5, 0) = "AclaraRF3-KV2c"
				MeterType(5, i) = node.SelectSingleNode("MeterType").InnerText & " " & node.SelectSingleNode("FormID").InnerText
				i = i + 1
			Next
				MeterType(6, 0) = "SRFN-EV2C-RA6"
				MeterType(6, i) = ""
		Catch ex As Exception
			AppLog("Error Reading FormList.xml: " & ex.Message)
			MsgBox("Error Reading FormList.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
		End Try
	End Sub
	Public Sub GetFormValues()
		Try
			Dim showform As String = ""
			Dim doc As New XmlDocument()
			Dim xmldoc As String = Application.StartupPath & "\SQLValues.xml"
			If Not IO.File.Exists(xmldoc) Then GoTo LoadFormValues
			doc.Load(xmldoc)
			Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/Database")
			For Each node As XmlNode In nodes
				DLL_Revision.Text = If(node.SelectSingleNode("DLL_Revision")?.InnerText, "")
				SRFN_CustomerValues.Text = If(node.SelectSingleNode("SRFN_CustomerValues")?.InnerText, "")
				SRFN_TestResults.Text = If(node.SelectSingleNode("SRFN_TestResults")?.InnerText, "")
				_firmwareConnStr = If(node.SelectSingleNode("Aclara_FirmwareConn")?.InnerText, "")
				txtSQLServer.Text = If(node.SelectSingleNode("txtSQLServer")?.InnerText, "")
				showform = If(node.SelectSingleNode("ShowForm")?.InnerText, "")
			Next
			If showform = "True" Then
				chkShowForm.Checked = True
			Else
				chkShowForm.Checked = False
			End If

		Catch ex As Exception
		End Try

LoadFormValues:
		Try
			Dim doc As New XmlDocument()
			Dim xmldoc As String = Application.StartupPath & "\FormValues.xml"
			If Not IO.File.Exists(xmldoc) Then Return
			doc.Load(xmldoc)
			Dim formnodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/CurrentForm")
			For Each node As XmlNode In formnodes
				txtMeterSerial.Text = node.SelectSingleNode("txtMeterSerial").InnerText
				txtMacAddress.Text = node.SelectSingleNode("txtMacAddress").InnerText
				txtUtilitySerial.Text = node.SelectSingleNode("txtUtilitySerial").InnerText
				txtCustomerID.Text = node.SelectSingleNode("txtCustomerID").InnerText
				txtProductFamily.Text = node.SelectSingleNode("txtProductFamily").InnerText
				txtCommPort.Text = node.SelectSingleNode("txtCommPort").InnerText
				txtFormID.Text = node.SelectSingleNode("txtFormID").InnerText
				txtPackPath.Text = node.SelectSingleNode("txtPackPath").InnerText
				txtDebugPort.Text = node.SelectSingleNode("debugport").InnerText
				Dim relayNode As XmlNode = node.SelectSingleNode("relayport")
				If relayNode IsNot Nothing Then txtRelayPort.Text = relayNode.InnerText
				RA6ProgPath.Text = node.SelectSingleNode("RA6ProgPath").InnerText
			Next

			Dim prodfnodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/ProductFamilies")
			For Each node As XmlNode In prodfnodes
				Prodnum(0, 0) = node.SelectSingleNode("Prod1").InnerText
				Prodnum(0, 1) = node.SelectSingleNode("Port1").InnerText
				Prodnum(1, 0) = node.SelectSingleNode("Prod2").InnerText
				Prodnum(1, 1) = node.SelectSingleNode("Port2").InnerText
				Prodnum(2, 0) = node.SelectSingleNode("Prod3").InnerText
				Prodnum(2, 1) = node.SelectSingleNode("Port3").InnerText
				Prodnum(3, 0) = node.SelectSingleNode("Prod4").InnerText
				Prodnum(3, 1) = node.SelectSingleNode("Port4").InnerText
			Next

		Catch ex As Exception
		End Try

		'Try
		'    Dim doc As New XmlDocument()
		'    Dim xmldoc As String = Application.StartupPath & "\debug.xml"
		'    doc.Load(xmldoc)
		'    Dim formnodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/Debug")
		'    For Each node As XmlNode In formnodes
		'        txtDebug.Text = node.SelectSingleNode("debugtest").InnerText
		'    Next

		'Catch ex As Exception
		'    MsgBox("Error Reading debug.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
		'End Try
	End Sub

	'.............................................END XML GET FORM VALUES
	Public Sub UpdatePortStatus() Implements IMainForm.UpdatePortStatus
		lblPortStatus.Text = "Comm: " & If(txtCommPort.Text <> "", txtCommPort.Text, "--") &
		                     "  |  Debug: " & If(txtDebugPort.Text <> "", txtDebugPort.Text, "--") &
		                     "  |  Relay: " & If(txtRelayPort.Text <> "", txtRelayPort.Text, "--")
	End Sub

	Public Sub ShowPorts() Implements IMainForm.ShowPorts
		txtCommPort.Items.Clear()
		txtCommPort.ResetText()
		txtDebugPort.Items.Clear()
		txtDebugPort.ResetText()
		txtRelayPort.Items.Clear()
		txtRelayPort.ResetText()
		Try
			For Each sp As String In My.Computer.Ports.SerialPortNames
				Dim portNum As String = Replace(sp.ToString, "COM", "")
				txtCommPort.Items.Add(portNum)
				txtDebugPort.Items.Add(portNum)
				txtRelayPort.Items.Add(portNum)
			Next
		Catch ex As Exception
		End Try
		Refresh()
	End Sub
	Private Sub mnuSQLConfig_Click(sender As Object, e As EventArgs) Handles mnuSQLConfig.Click
		Dim frm As New frmSQLConfig(Me)
		frm.ShowDialog(Me)
		txtPassword.Text = ""
	End Sub

	Private Sub mnuCommPorts_Click(sender As Object, e As EventArgs) Handles mnuCommPorts.Click
		If _frmCommPorts Is Nothing OrElse _frmCommPorts.IsDisposed Then
			_frmCommPorts = New frmCommPorts(Me)
			PlaceForm(_frmCommPorts)
			_frmCommPorts.Show(Me)
		Else
			_frmCommPorts.BringToFront()
		End If
	End Sub

	Private Sub btnRunManual_Click(sender As Object, e As EventArgs)
	End Sub
	Private Async Sub btnRunAutomated_Click(sender As Object, e As EventArgs)
		If _commManager Is Nothing Then _commManager = New CommManager2()
		Try : _commManager.ClosePort() : Catch : End Try
		txtResults.BackColor = Color.Yellow
		txtResults.Text = "Test Running"
		Dim commport As Integer = CInt(txtCommPort.Text)
		Dim MeterSerial As String = txtMeterSerial.Text
		Dim MacAddress As String = txtMacAddress.Text
		Dim UtilitySerial As String = txtUtilitySerial.Text
		Dim SQLServer As String = txtSQLServer.Text
		Dim CustomerID As String = txtCustomerID.Text
		Dim ProductFamily As String = txtProductFamily.Text
		Dim FormID As String = txtFormID.Text
		Dim RFCarrier As String = txtRFCarrier.Text
		Dim PackPath As String = txtPackPath.Text
		Dim RDInstalled As Boolean = chkRDInstalled.Checked
		Dim varparam1 As String = MeterSerial & "," & MacAddress & "," & SQLServer & "," & UtilitySerial
		Dim varparam2 As String = CustomerID & "," & ProductFamily & "," & FormID & "," & RFCarrier & "," & PackPath & "," & RDInstalled
		'Dim results = Await _commManager.Team_INTEGRATIONDotNetTest(21, "63940110,001D2400010002E1,STL7130D\MSSQLSERVER2012,123456", "31751500,SRFN-I-210+,01120,00,C:\Pack")
		Dim results = Await _commManager.Team_INTEGRATIONDotNetTest(commport, varparam1, varparam2)
		Me.BringToFront()
		If results = "Pass" Then
			txtResults.BackColor = Color.Green
			txtResults.Text = "Pass"
		Else
			txtResults.BackColor = Color.Red
			txtResults.Text = "Fail"
		End If
		Me.Refresh()
	End Sub

	Private Sub Local_TestProgressUpdated(ByVal progress As Integer)
		'ProgressBar1.Value = progress
	End Sub

	Private Sub Local_TestProgressCompleted(ByVal script As TestScript)
		'ProgressBar1.Value = 0
		'_commManager.Dispose()
		'_commManager = Nothing
	End Sub

	Private Sub CommManager_TestProgressUpdated(ByVal progress As Integer) Handles _commManager.TestProgressUpdated
		If Me.InvokeRequired Then
			Dim updateProgress As New TestProgressUpdatedEventHandler(AddressOf Local_TestProgressUpdated)
			Me.Invoke(updateProgress)
		Else
			Local_TestProgressUpdated(progress)
		End If
	End Sub

	Private Sub CommManager_TestProgressCompleted(ByVal script As TestScript) Handles _commManager.TestProgressCompleted
		If Me.InvokeRequired Then
			Dim testComplete As New TestProgressCompletedEventHandler(AddressOf Local_TestProgressCompleted)
			Me.Invoke(testComplete)
		Else
			Local_TestProgressCompleted(script)
		End If
	End Sub
	Private Function FormIDConvert(ByVal FrmID As String) As String
		FormIDConvert = ""
		Dim Prodtmp As String = txtProductFamily.Text
		If Prodtmp = "" Then Return ""
		If Prodtmp = "SRFN-I-210+c" Then Prodtmp = "SRFN-I-210C"
		If Prodtmp = "SRFN-I-210+c-RA6" Then Prodtmp = "SRFN-I-210C-RA6"
		Dim doc As New XmlDocument()
		Dim xmldoc As String = Application.StartupPath & "\FormList.xml"
		doc.Load(xmldoc)
		Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/" & Replace(Prodtmp, "+", ""))
		For Each node As XmlNode In nodes
			If FrmID = node.SelectSingleNode("MeterType").InnerText Then
				Return node.SelectSingleNode("FormID").InnerText
			End If
		Next
	End Function
	Private Async Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
		Try
			txtResults.BackColor = Color.Yellow
			txtResults.Text = "Test Running"
			'execute the proxy method to the com manager
			Dim commport As Integer
			Dim portText As String = txtCommPort.Text.Trim().ToUpper()
			If portText.StartsWith("COM") Then portText = portText.Substring(3).Trim()
			If Not Integer.TryParse(portText, commport) Then
				AppLog("Invalid COM port value: '" & txtCommPort.Text & "'")
				MsgBox("Invalid COM port: '" & txtCommPort.Text & "'" & vbCrLf & "Enter a number (e.g. 3) or COM name (e.g. COM3).", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
				Return
			End If
			Dim MeterSerial As String = txtMeterSerial.Text
			Dim MacAddress As String = txtMacAddress.Text
			Dim UtilitySerial As String = txtUtilitySerial.Text
			Dim SQLServer As String = txtSQLServer.Text
			Dim CustomerID As String = txtCustomerID.Text
			Dim ProductFamily As String = txtProductFamily.Text
			Dim FormID As String = FormIDConvert(txtFormID.Text)
			If FormID = "" Then FormID = " "
			Dim RFCarrier As String = txtRFCarrier.Text
			Dim PackPath As String = txtPackPath.Text
			Dim RDInstalled As Boolean = chkRDInstalled.Checked
			Dim varparam1 As String = MeterSerial & "," & MacAddress & "," & SQLServer & "," & UtilitySerial
			Dim varparam2 As String = CustomerID & "," & ProductFamily & "," & FormID & "," & RFCarrier & "," & PackPath & "," & RDInstalled
						AppLog("--- Test Start: CustomerID=" & CustomerID & " ProductFamily=" & ProductFamily & " MeterSerial=" & MeterSerial & " COM=" & commport & " FormID=" & txtFormID.Text & " RFCarrier=" & RFCarrier & " ---")
Dim result As String = ""
			If _commManager Is Nothing Then _commManager = New CommManager2()
			Try : _commManager.ClosePort() : Catch : End Try
			result = Await _commManager.Team_INTEGRATIONDotNetTest(commport, varparam1, varparam2)
			If Mid(result, 1, 4) = "Fail" Then
				result = Mid(result, 1 + InStr(result, ","))
				txtResults.BackColor = Color.Red
				txtResults.Text = "Fail" & vbCrLf & "Step " & Mid(result, 1, InStrRev(result, ",") - 1) & vbCrLf & Mid(result, InStrRev(result, ",") + 1)
			Else
				txtResults.BackColor = Color.Green
				txtResults.Text = result
			End If
			Me.BringToFront()
			Me.Refresh()
		Catch ex As Exception
			AppLog("btnReset_Click error: " & ex.ToString)
			MsgBox("Test error: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
		End Try
	End Sub

	Friend Async Function RunTestRaw() As Task(Of String)
		Try
			txtResults.BackColor = Color.Yellow
			txtResults.Text = "Batch Running..."
			Me.Refresh()
			Dim commport As Integer
			Dim portText As String = txtCommPort.Text.Trim().ToUpper()
			If portText.StartsWith("COM") Then portText = portText.Substring(3).Trim()
			If Not Integer.TryParse(portText, commport) Then Return "Fail,Invalid COM port"
			Dim formID As String = FormIDConvert(txtFormID.Text)
			If formID = "" Then formID = " "
			Dim varparam1 As String = txtMeterSerial.Text & "," & txtMacAddress.Text & "," & txtSQLServer.Text & "," & txtUtilitySerial.Text
			Dim varparam2 As String = txtCustomerID.Text & "," & txtProductFamily.Text & "," & formID & "," & txtRFCarrier.Text & "," & txtPackPath.Text & "," & chkRDInstalled.Checked.ToString()
			AppLog("--- Batch Test Start: COM=" & commport & " CustomerID=" & txtCustomerID.Text & " ProductFamily=" & txtProductFamily.Text & " ---")
			If _commManager Is Nothing Then _commManager = New CommManager2()
			Try : _commManager.ClosePort() : Catch : End Try
			Dim result As String = Await _commManager.Team_INTEGRATIONDotNetTest(commport, varparam1, varparam2)
			If Mid(result, 1, 4) = "Fail" Then
				Dim r As String = Mid(result, 1 + InStr(result, ","))
				txtResults.BackColor = Color.Red
				txtResults.Text = "Fail" & vbCrLf & "Step " & Mid(r, 1, InStrRev(r, ",") - 1) & vbCrLf & Mid(r, InStrRev(r, ",") + 1)
			Else
				txtResults.BackColor = Color.Green
				txtResults.Text = result
			End If
			Me.BringToFront()
			If _frmBatch IsNot Nothing AndAlso Not _frmBatch.IsDisposed Then _frmBatch.BringToFront()
			Me.Refresh()
			Return result
		Catch ex As Exception
			AppLog("RunTestRaw error: " & ex.ToString())
			Return "Fail," & ex.Message
		End Try
	End Function

	Private Sub btnBatch_Click(sender As Object, e As EventArgs) Handles btnBatch.Click
		If _frmBatch Is Nothing OrElse _frmBatch.IsDisposed Then
			_frmBatch = New frmBatch(Me)
			PlaceForm(_frmBatch)
			_frmBatch.Show(Me)
		Else
			_frmBatch.BringToFront()
		End If
	End Sub

	Private Sub PlaceForm(frm As Form)
		Dim screen As Rectangle = System.Windows.Forms.Screen.GetWorkingArea(Me)
		Dim x As Integer = Me.Right + 5
		If x + frm.Width > screen.Right Then x = Me.Left - frm.Width - 5
		x = Math.Max(screen.Left, x)
		Dim y As Integer = Me.Top
		For Each f As Form In Application.OpenForms
			If f IsNot frm AndAlso f IsNot Me AndAlso f.Visible Then
				If f.Left < x + frm.Width AndAlso f.Right > x Then
					y = Math.Max(y, f.Bottom + 5)
				End If
			End If
		Next
		If y + frm.Height > screen.Bottom Then y = Math.Max(screen.Top, screen.Bottom - frm.Height)
		frm.Location = New Point(x, y)
	End Sub

	Private Function parseval(ByVal val As String) As String
		parseval = Nothing
		Dim temp As Integer = InStr(val, "=")
		parseval = Mid(val, temp + 1)
	End Function

	Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
		Dim server As String = txtSQLServer.Text.Trim()
		If server = "" Then
			MsgBox("Enter a SQL Server name first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
			Return
		End If

		btnConnect.BackColor = Color.Yellow
		btnConnect.Text = "Connecting..."
		Me.Refresh()

		Dim connected As Boolean = False
		Dim lastError As String = ""

		' Try the stored connection string first (may carry SQL auth from Browse),
		' then fall back to Windows auth with bare server name.
		Dim fullConnStr As String = DLL_Revision.Text.Trim()
		Dim sspiStr As String = "Provider=SQLOLEDB;Data Source=" & server & ";Integrated Security=SSPI;"
		For Each connStr As String In {fullConnStr, sspiStr}
			If connStr = "" Then Continue For
			Try
				CheckSQLState()
				dcnDB.Open(connStr)
				connected = True
				Exit For
			Catch ex As Exception
				lastError = ex.Message
				AppLog("Connection attempt failed: " & ex.ToString)
				CheckSQLState()
			End Try
		Next

		If connected Then
			btnSaveSQL.PerformClick()
			btnConnect.BackColor = Color.Green
			btnConnect.Text = "Connected"
			_sqlServer = server
			CommManager2.SqlServerName = server
			CommManager2.CurrentProductFamily = txtProductFamily.Text
			CommManager2.FirmwareConnStr = BuildFirmwareConnStr()
			CommManager2.CustomerValuesConnStr = SRFN_CustomerValues.Text.Trim()
			CommManager2.TestResultsConnStr = SRFN_TestResults.Text.Trim()
			CommManager2.DLLRevisionConnStr = DLL_Revision.Text.Trim()
			CheckSQLState()
		Else
			btnConnect.BackColor = Color.Red
			btnConnect.Text = "Connect"
			MsgBox("Connection failed: " & lastError, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
		End If
	End Sub

#Region "SQLFunctions"
	'============================================== Check SQL State Close if Open
	Private Sub CheckSQLState()
		Try
			If dcnDB IsNot Nothing Then
				If dcnDB.State > 0 Then dcnDB.Close()
			End If
		Catch ex As Exception
		End Try
	End Sub
	Private Function CheckSQL(ByVal Table As String, ByVal SQLString As String, Optional ByVal WhereVal As String = "") As Boolean
		CheckSQL = False
		If SQLString.Trim() = "" Then
			MsgBox("Connection string is empty — use Browse to set it.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
			Return False
		End If
		CheckSQLState()
		Try
			dcnDB.Open(SQLString)
			rsData = dcnDB.Execute("SELECT * FROM [dbo].[" & Table & "]" & WhereVal)
			If Not rsData.EOF Then CheckSQL = True
			rsData.Close()
			rsData = Nothing
		Catch ex As Exception
			MsgBox("SQL check failed [" & Table & "]: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
		Finally
			CheckSQLState()
		End Try
	End Function
	Private Sub btnCheckDLLString_Click(sender As Object, e As EventArgs) 
		btnCheckDLLString.BackColor = Color.Yellow
		If CheckSQL("DLL_Revision", DLL_Revision.Text, "where DLL_Rev = '" & DLLver & "'") = True Then
			btnCheckDLLString.BackColor = Color.LightGreen
		Else
			btnCheckDLLString.BackColor = Color.Red
		End If
		If (btnCheckDLLString.BackColor = Color.LightGreen AndAlso btnCheckCustomerString.BackColor = Color.LightGreen AndAlso btnCheckResultString.BackColor = Color.LightGreen) Then

		End If
	End Sub '
	Private Sub btnCheckCustomerString_Click(sender As Object, e As EventArgs) 
		btnCheckCustomerString.BackColor = Color.Yellow
		If CheckSQL("SRFN_Customer_Values", SRFN_CustomerValues.Text) = True Then
			btnCheckCustomerString.BackColor = Color.LightGreen
		Else
			btnCheckCustomerString.BackColor = Color.Red
		End If
		If (btnCheckDLLString.BackColor = Color.LightGreen AndAlso btnCheckCustomerString.BackColor = Color.LightGreen AndAlso btnCheckResultString.BackColor = Color.LightGreen) Then

		End If
	End Sub
	Private Sub btnCheckResultString_Click(sender As Object, e As EventArgs) 
		btnCheckResultString.BackColor = Color.Yellow
		If CheckSQL("Integration_TestResults", SRFN_TestResults.Text) = True Then
			btnCheckResultString.BackColor = Color.LightGreen
		Else
			btnCheckResultString.BackColor = Color.Red
		End If
	End Sub
	Private Sub BTN_RA6ConfigVals_Click(sender As Object, e As EventArgs) 
		BTN_RA6ConfigVals.BackColor = Color.Yellow
		'If CheckSQL("SRFN_RA6_ConfigValues", SRFN_RA6ConfigVals.Text) = True Then
		'    BTN_RA6ConfigVals.BackColor = Color.LightGreen
		'Else
		'    BTN_RA6ConfigVals.BackColor = Color.Yellow
		'End If
	End Sub
	Private Sub btnCheckRA6ProgPath_Click(sender As Object, e As EventArgs) 
		Dim path As String = RA6ProgPath.text
		If path.Length < 5 Then Exit Sub
		Dim files As String() = Directory.GetFiles(path, "rfp-cli.exe", SearchOption.AllDirectories)
		If files.Length > 0 Then
			btnCheckRA6ProgPath.BackColor = Color.LightGreen
		Else
			btnCheckRA6ProgPath.BackColor = Color.Yellow
		End If
	End Sub
#End Region

#Region "SetPassword"
	Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged
		txtResults.BackColor = Color.White
		If txtPassword.Text = "TWACS" Then
			RaiseEvent ModeSwitchRequested("OCX")
			Return
		End If
		If txtPassword.Text = "REWORK" Then
			LockAllButtons()
		End If
		If txtPassword.Text = "HASH1" Then
			btnGetHash.Visible = True
		ElseIf txtPassword.Text = "SQL1" Then
			btnGetHash.Text = "SQL"
			btnGetHash.Visible = True
		Else
			btnGetHash.Visible = False
		End If
		If txtPassword.Text = "RA6" Then
			RA6Buttons(True)
		Else
			RA6Buttons(False)
		End If

	End Sub
Private Sub SetRA6ButtonsEnabled(ByVal enab As Boolean)
		btnResetRelease.Enabled = enab
		btn_DeployEnable.Enabled = enab
		btnEnableDebug.Enabled = enab
		btn_CheckDeploy.Enabled = enab
	End Sub

	Private Sub RA6Buttons(ByVal Enab As Boolean)
		btnResetRelease.Enabled = True
		mnuConfigure.Enabled = Enab
		btn_ToggleUSB.Enabled = Enab
		If Enab = True Then
			btn_DeployEnable.Enabled = True
			btnEnableDebug.Enabled = True
			btnEnableDTLS.Enabled = True
			btnBatch.Enabled = True
			txtcommand.Enabled = True
		Else
			btn_DeployEnable.Enabled = False
			btnEnableDebug.Enabled = False
			btnEnableDTLS.Enabled = False
			btnBatch.Enabled = False
			txtcommand.Enabled = False
		End If
	End Sub
#End Region
	Private Async Function SendMsg(ByVal msg As String) As Task(Of String)
		txtcommand.Text = msg
		Dim optval As String = ""
		If InStr(msg, "DTLS") > 0 Then optval = Application.StartupPath & "\CertKeys\"
		txtResults.Text = "Sending Command" & vbCrLf & msg
		If msg = "" Then txtResults.Text = "No Command Sent"
		txtResults.BackColor = Color.Yellow
		Me.Refresh()
		Threading.Thread.Sleep(100)

		Dim portNum As Integer = 0
		If Not Integer.TryParse(txtCommPort.Text.Trim(), portNum) OrElse portNum < 1 Then
			txtResults.Text = "Invalid COM port - select a port first"
			txtResults.BackColor = Color.Red
			Return String.Empty
		End If

		Try
			If _commManager Is Nothing Then
				_commManager = New CommManager2()
			End If
			Dim SetExtEncoding As Object = _commManager.Encoding
			Dim response As String = Await _commManager.SendMessage(msg, portNum, txtProductFamily.Text, optval)

			If Mid(response, 1, 4) = "Fail" Then
				txtResults.BackColor = Color.Red
			ElseIf Mid(response, 1, 4) = "Pass" Then
				txtResults.BackColor = Color.Green
			ElseIf response.Contains("is not a valid") Then
				txtResults.BackColor = Color.Yellow
			Else
				txtResults.BackColor = Color.White
			End If
			txtResults.Text = response
			Return response
		Catch ex As Exception
			txtResults.BackColor = Color.Orange
			txtResults.Text = "Port error: " & ex.Message
			Return String.Empty
		End Try
	End Function

	Private Async Function SendDebugMsg(ByVal msg As String) As Task
		txtcommand.Text = msg
		txtResults.Text = "Sending Debug Msg" & vbCrLf & msg
		If msg = "" Then txtResults.Text = "No Command Sent"
		txtResults.BackColor = Color.Yellow
		Me.Refresh()
		Threading.Thread.Sleep(100)
		If _commManager Is Nothing Then
			_commManager = New CommManager2()
		End If
		Dim SetExtEncoding As Object = _commManager.Encoding
		Dim response As String = Await _commManager.SendDebugMessage(msg, "demo", "", "", "", "")

		If Mid(response, 1, 4) = "Fail" Then
			txtResults.BackColor = Color.Red
		ElseIf Mid(response, 1, 4) = "Pass" Then
			txtResults.BackColor = Color.Green
		ElseIf response.Contains("is not a valid") Then
			txtResults.BackColor = Color.Yellow
		Else
			txtResults.BackColor = Color.White
		End If
		txtResults.Text = response
	End Function

#Region "Buttons"
	Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
		'..... OLD METHOD SetFormValues()
		SetFormXml()
		btnSaveSQL.PerformClick()
		txtPassword.Text = ""
	End Sub
	Private Sub btnSqtSQL_Click(sender As Object, e As EventArgs)
		txtPassword.Text = ""
	End Sub
	Private Async Sub btnReadMac_Click(sender As Object, e As EventArgs) Handles btnReadMacAddress.Click
		Await SendMsg("comDeviceMACAddress")
	End Sub

	Private Async Sub btnVerifyComm_Click(sender As Object, e As EventArgs) Handles btnVerifyComm.Click
		Try
			btnVerifyComm.Enabled = False

			' Step 1: USB relay to Closed
			Dim relayPort As String = txtRelayPort.Text.Trim()
			If relayPort <> "" Then
				txtResults.BackColor = Color.Yellow
				txtResults.Text = "Verify Comm: setting USB relay to Closed..."
				Me.Refresh()
				CommManager2.RelayPort = relayPort
				CommManager2.RelaySetAndGet(1, True)
				btn_ToggleUSB.Text = If(CommManager2.UsbIsOn, "USB ON", "USB OFF")
				btn_ToggleUSB.BackColor = If(CommManager2.UsbIsOn, Color.Green, Color.Yellow)
				If Not CommManager2.UsbIsOn Then
					txtResults.BackColor = Color.Red
					txtResults.Text = "Verify Comm: USB relay could not be set to Closed"
					Return
				End If
			End If

			' Step 2: Reset Release (500ms after Closed, then 1000ms settle)
			Await Task.Delay(500)
			txtResults.BackColor = Color.Yellow
			txtResults.Text = "Verify Comm: Reset Release..."
			Me.Refresh()
			Await CommManager2.RunRA6Command(RA6ProgPath.Text, "rfp-cli -t E2l -d RA -if uart -s 1.5M")
			Await Task.Delay(1000)

			' Step 3: Read MAC address
			txtResults.BackColor = Color.Yellow
			txtResults.Text = "Verify Comm: reading MAC address..."
			Me.Refresh()
			Dim response As String = Await SendMsg("comDeviceMACAddress")

			If Not response.StartsWith("Fail") AndAlso response.Trim().Length > 0 Then
				txtResults.BackColor = Color.LightGreen
				txtResults.Text = "End Point Mfg Port Communication Successful" & vbCrLf & response
			Else
				txtResults.BackColor = Color.Red
				txtResults.Text = "Verify Comm Failed - MAC read: " & response
			End If

		Catch ex As Exception
			txtResults.BackColor = Color.Red
			txtResults.Text = "Verify Comm error: " & ex.Message
		Finally
			btnVerifyComm.Enabled = True
		End Try
	End Sub
	Private Async Sub btn_edInfo_Click(sender As Object, e As EventArgs) Handles btn_edInfo.Click
		Await SendMsg("edInfo")
	End Sub
	Private Async Sub btn_edfw_Click(sender As Object, e As EventArgs) Handles btn_edfw.Click
		Await SendMsg("edfwversion")
	End Sub
	Private Async Sub btnReadFirmware_Click(sender As Object, e As EventArgs) Handles btnReadFirmware.Click
		Await SendMsg("comDeviceFirmwareVersion")
	End Sub
	Private Async Sub btnquietStatus_Click(sender As Object, e As EventArgs) Handles btnquietStatus.Click
		Await SendMsg("quietMode")
	End Sub
	Private Async Sub btnquietON_Click(sender As Object, e As EventArgs) Handles btnquietON.Click
		Await SendMsg("quietMode 1")
	End Sub
	Private Async Sub btnquietOFF_Click(sender As Object, e As EventArgs) Handles btnquietOFF.Click
		Await SendMsg("quietMode 0")
	End Sub
	Private Async Sub btnChkPort_Click(sender As Object, e As EventArgs) Handles btnChkPort.Click
		Await SendMsg("OpenPort")
	End Sub
	Private Async Sub btnSendcommand_Click(sender As Object, e As EventArgs) Handles btnSendcommand.Click
		Await SendMsg(txtcommand.Text)
	End Sub
	Private Async Sub btnOpticalLogon_Click(sender As Object, e As EventArgs) Handles btnOpticalLogon.Click
		Await SendMsg("OpticalLogon")
	End Sub
	Private Async Sub btnOpticalLogoff_Click(sender As Object, e As EventArgs) Handles btnOpticalLogoff.Click
		Await SendMsg("logoff")
	End Sub
	Private Async Sub btnReadEnergy_Click(sender As Object, e As EventArgs) Handles btnReadEnergy.Click
		Await SendMsg("ReadEnergy")
	End Sub
#End Region
	Private Sub btnGetHash_Click(sender As Object, e As EventArgs) Handles btnGetHash.Click
		Dim i As Integer = 0
		Dim strTemp As String = ""
		If btnGetHash.Text = "SQL" Then
			CreateSQL_Fromxml()
			Exit Sub
		End If

		Dim fbd As New OpenFileDialog With {
			.Title = "Select XML files",
			.Filter = "XML Files (*.xml)|*.xml",
			.Multiselect = True,
			.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}
		If fbd.ShowDialog() <> DialogResult.OK Then
			txtResults.Text = "Path Error"
			Exit Sub
		End If
		For i = 0 To fbd.FileNames.Count - 1
			Dim xmlFile As String = fbd.FileNames(i)
			RenumberAndFixXml(xmlFile)
			strTemp = GetHashVal(xmlFile)
			If i = 0 Then txtResults.Text = "HashValue" & vbCrLf & strTemp
			If strTemp IsNot Nothing Then GenerateSqlFromXml(xmlFile, strTemp)
		Next
	End Sub

	Private Sub RenumberAndFixXml(filename As String)
		Try
			Dim doc As New XmlDocument()
			doc.Load(filename)

			Dim tables As XmlNodeList = doc.SelectNodes("//Table")
			Dim count As Integer = 0
			For Each tbl As XmlNode In tables
				count += 1
				Dim refNode As XmlNode = tbl.SelectSingleNode("RefNum")
				If refNode IsNot Nothing Then refNode.InnerText = count.ToString()
			Next

			Dim progressNode As XmlNode = doc.SelectSingleNode("//HashTable/ProgressMax")
			If progressNode IsNot Nothing Then progressNode.InnerText = count.ToString()

			doc.Save(filename)
			AppLog("RenumberAndFixXml: " & count.ToString() & " steps, ProgressMax=" & count.ToString())
		Catch ex As Exception
			AppLog("RenumberAndFixXml error: " & ex.Message)
			MsgBox("Renumber failed: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
		End Try
	End Sub

	Private Sub GenerateSqlFromXml(filename As String, hashValue As String)
		Try
			Dim doc As New XmlDocument()
			doc.Load(filename)

			Dim productFamily As String = ""
			Dim drawing As String = ""
			Dim customerName As String = ""
			Dim customerID As String = ""
			Dim progressMax As String = ""
			Dim firmware As String = ""

			Dim n As XmlNode
			n = doc.SelectSingleNode("//HashTable/ProductFamily") : If n IsNot Nothing Then productFamily = n.InnerText
			n = doc.SelectSingleNode("//HashTable/Drawing")       : If n IsNot Nothing Then drawing = n.InnerText
			n = doc.SelectSingleNode("//HashTable/CustomerName")  : If n IsNot Nothing Then customerName = n.InnerText
			n = doc.SelectSingleNode("//HashTable/CustomerID")    : If n IsNot Nothing Then customerID = n.InnerText
			n = doc.SelectSingleNode("//HashTable/ProgressMax")   : If n IsNot Nothing Then progressMax = n.InnerText

			' Firmware version from comDeviceFirmwareVersion expected response
			For Each tbl As XmlNode In doc.SelectNodes("//Table")
				Dim cn As XmlNode = tbl.SelectSingleNode("CommName")
				If cn IsNot Nothing AndAlso cn.InnerText = "comDeviceFirmwareVersion" Then
					Dim er As XmlNode = tbl.SelectSingleNode("Exp_Resp")
					If er IsNot Nothing Then firmware = er.InnerText
					Exit For
				End If
			Next

			' Build <Table> XML content
			Dim tbls As New System.Text.StringBuilder()
			For Each tbl As XmlNode In doc.SelectNodes("//Table")
				tbls.AppendLine("   <Table>")
				For Each child As XmlNode In tbl.ChildNodes
					If child.InnerText = "" Then
						tbls.AppendLine("     <" & child.Name & "/>")
					Else
						tbls.AppendLine("     <" & child.Name & ">" & child.InnerText.Replace("'", "''") & "</" & child.Name & ">")
					End If
				Next
				tbls.AppendLine("   </Table>")
			Next

			' Build SQL
			Dim sql As New System.Text.StringBuilder()
			sql.AppendLine("DELETE FROM [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] WHERE ProductFamily = '" & productFamily.Replace("'", "''") & "' and CustomerID ='" & customerID & "' and Firmware = '" & firmware & "'")
			sql.AppendLine()
			sql.AppendLine("INSERT INTO [Aclara_CustomerSpecific].[dbo].[SRFN_Customer_Values] VALUES (")
			sql.AppendLine("'" & productFamily.Replace("'", "''") & "',")
			sql.AppendLine("'" & drawing.Replace("'", "''") & "',")
			sql.AppendLine("'" & customerName.Replace("'", "''") & "',")
			sql.AppendLine("'" & customerID & "',")
			sql.AppendLine("'" & firmware & "',")
			sql.AppendLine("'" & hashValue & "',")
			sql.AppendLine("'" & progressMax & "',")
			sql.Append("'" & vbCrLf)
			sql.Append(tbls.ToString().TrimEnd())
			sql.Append("')")

			Dim sqlFilename As String = "SRFN SQL SERIAL INSERT_" & firmware & "-" & productFamily & "-" & customerName & " _" & customerID & ".sql"
		Dim sqlPath As String = IO.Path.Combine(IO.Path.GetDirectoryName(filename), sqlFilename)
			IO.File.WriteAllText(sqlPath, sql.ToString(), System.Text.Encoding.UTF8)
			AppLog("GenerateSqlFromXml: saved " & sqlPath)
			txtResults.Text = txtResults.Text & vbCrLf & "SQL saved: " & IO.Path.GetFileName(sqlPath)
		Catch ex As Exception
			AppLog("GenerateSqlFromXml error: " & ex.Message)
			MsgBox("SQL generation failed: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
		End Try
	End Sub

	Private Sub CreateSQL_Fromxml()
		Dim fbd As New OpenFileDialog With {
			.Title = "Select XML Script file",
			.Filter = "XML Files (*.xml)|*.xml",
			.Multiselect = False,
			.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}
		If fbd.ShowDialog() <> DialogResult.OK Then
			txtResults.Text = "Path Error"
			Return
		End If
		If Not fbd.FileName.ToLower().EndsWith(".xml") Then
			MsgBox("Not an xml file", MsgBoxStyle.Exclamation Or MsgBoxStyle.SystemModal)
			Return
		End If
		RenumberAndFixXml(fbd.FileName)
		Dim hashVal As String = GetHashVal(fbd.FileName)
		If hashVal IsNot Nothing Then GenerateSqlFromXml(fbd.FileName, hashVal)
		btnGetHash.Text = "Hash"
	End Sub
	Private Function GetHashVal(ByVal filename As String) As String
		Dim dialog As New OpenFileDialog()
		txtResults.Text = ""
		txtResults.BackColor = Color.White
		Dim oldhash As String = ""
		Dim lines As String() = IO.File.ReadAllLines(filename)
		For i As Integer = 0 To lines.Length - 1
			If lines(i).Contains("<HashValue>") Then
				oldhash = Mid(lines(i), 1 + InStr(lines(i), ">"))
				oldhash = Mid(oldhash, 1, InStr(oldhash, "<") - 1)
			End If
		Next
		Dim ds As New DataSet()
		Dim script As New TestScript
		Dim temp As String = ""
		ds.ReadXml(filename)
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
		If oldhash = "" Then
			MsgBox("HASH VALUE FAILED" & vbCrLf & "No prev hash value")
			Return Nothing
		End If
		strTemp1 = CommManager2.GenerateHash(strTemp1)
		System.IO.File.WriteAllText(filename, System.IO.File.ReadAllText(filename).Replace(oldhash, strTemp1))
		Return strTemp1
	End Function
#End Region
	Private Sub txtProductFamily_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtProductFamily.SelectedIndexChanged
		txtFormID.SelectedIndex = -1
		Me.Refresh()
		Dim baud As Integer = 3
		Select Case txtProductFamily.Text
			Case Prodnum(0, 0)
				txtCommPort.Text = Prodnum(0, 1)
				'baud = 3
			Case Prodnum(1, 0)
				txtCommPort.Text = Prodnum(1, 1)
				'baud = 3
			Case Prodnum(2, 0)
				txtCommPort.Text = Prodnum(2, 1)
				'baud = 2
			Case Prodnum(3, 0)
				txtCommPort.Text = Prodnum(3, 1)
				'baud = 2
			Case Prodnum(4, 0)
				txtCommPort.Text = Prodnum(4, 1)
				'baud = 2
			Case Prodnum(5, 0)
				txtCommPort.Text = Prodnum(5, 1)
'baud = 2
End Select
GetForm(txtProductFamily.Text)
btnReset.Focus()

		If txtProductFamily.Text = "SRFN-KV2c" or txtProductFamily.Text = "AclaraRF3-KV2c" Then
			chkRDInstalled.Enabled = False
			btnOpticalLogoff.Enabled = True
			btnOpticalLogon.Enabled = True
		Else
			chkRDInstalled.Enabled = True
			btnOpticalLogoff.Enabled = False
			btnOpticalLogon.Enabled = False
		End If

		If txtProductFamily.Text = "SRFN-I-210+" Then
			txtFormID.Enabled = True
		Else
			txtFormID.Enabled = False
		End If
		CommManager2.CurrentProductFamily = txtProductFamily.Text
		SetFormXml()
	End Sub
	Private Sub txtFormID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtFormID.SelectedIndexChanged
		If txtFormID.SelectedIndex > -1 and txtProductFamily.Text = "SRFN-I-210+" Then
			txtResults.Text = txtFormID.SelectedItem.ToString
		Else
			txtResults.Text = ""
		End If
	End Sub
	Public Function GetForm(ByVal ProductFamily As String) As Object
		Try
			txtFormID.Items.Clear
			Select Case ProductFamily
				Case "SRFN-I-210+"
					ProductFamily = "SRFN-I-210"
				Case "SRFN-I-210+c"
					ProductFamily = "SRFN-I-210C"
				Case "SRFN-I-210+c-RA6"
					ProductFamily = "SRFN-I-210C-RA6"
				Case "AclaraRF3-I210+c"
					ProductFamily = "AclaraRF3-I210C"
				Case "AclaraRF3-KV2c"
					ProductFamily = "AclaraRF3-KV2c"
				Case "SRFN-KV2c"
					ProductFamily = "SRFN-KV2c"
				Case "SRFN-EV2C-RA6"
					ProductFamily = "SRFN-EV2C-RA6"
			End Select

			Dim doc As New XmlDocument()
			Dim xmldoc As String = Application.StartupPath & "\FormList.xml"
			doc.Load(xmldoc)
			Dim showform As String = ""
			Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/Data/" & ProductFamily)
			For Each node As XmlNode In nodes
				txtFormID.Items.Add(node.SelectSingleNode("MeterType").InnerText)
			Next
		Catch ex As Exception
			AppLog("Error Reading FormList.xml: " & ex.Message)
			MsgBox("Error Reading FormList.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
		End Try
		Return Nothing
	End Function
	Private Sub btnGetCustomerName_Click(sender As Object, e As EventArgs) Handles btnGetCustomerName.Click
		GetCustomerName()
	End Sub
	Private Sub GetCustomerName()
		txtResults.Text = ""
		txtResults.BackColor = Color.White
		Me.Refresh()
		If _commManager Is Nothing Then _commManager = New CommManager2()
		Dim tmp As String = _commManager.GetSQLCustomerName(txtProductFamily.Text, txtCustomerID.Text)
		txtResults.Text = tmp
	End Sub
	Private Sub btnLockForm_Click(sender As Object, e As EventArgs) Handles btnLockForm.Click
		If txtProductFamily.Enabled = True Then
			txtProductFamily.Enabled = False
			txtCustomerID.Enabled = False
			txtFormID.Enabled = False
			chkRDInstalled.Enabled = False
			txtSQLServer.Enabled = False
			txtPackPath.Enabled = False
			txtCommPort.Enabled = False
			txtRFCarrier.Enabled = False
			btnReset.Enabled = True
			btnReset.BackColor = Color.LightGreen
			btnLockForm.BackColor = Color.Transparent
			btnLockForm.Text = "Unlock Form"
			If _commManager Is Nothing Then _commManager = New CommManager2()
			Dim tmp As String = _commManager.GetSQLCustomerName(txtProductFamily.Text, txtCustomerID.Text)
			If tmp = "Customer Not Found" OrElse tmp = "" Then
				Dim records As String = _commManager.GetSQLLikeCustomers(txtProductFamily.Text, txtCustomerID.Text)
				txtResults.ReadOnly = False
				txtResults.Text = If(records <> "", records & vbCrLf, "") &
				                  "No Match for " & txtCustomerID.Text & vbCrLf &
				                  "Enter Customer Name or CustomerID To Search:" & vbCrLf
				txtResults.SelectionStart = txtResults.Text.Length
				txtResults.Focus()
				_searchMode = True
			Else
				txtResults.ReadOnly = True
				_searchMode = False
				txtResults.Text = String.Concat(Enumerable.Repeat(vbCrLf, 8)) & tmp
			End If
		Else
			txtProductFamily.Enabled = True
			txtCustomerID.Enabled = True
			txtFormID.Enabled = True
			chkRDInstalled.Enabled = True
			txtSQLServer.Enabled = True
			txtPackPath.Enabled = True
			txtCommPort.Enabled = True
			txtRFCarrier.Enabled = True
			btnReset.Enabled = False
			btnReset.BackColor = Color.Transparent
			btnLockForm.BackColor = Color.Transparent
			btnLockForm.Text = "Lock Form"
			_searchMode = False
			txtResults.ReadOnly = True
		End If
		Me.Refresh
	End Sub
	Private Sub txtResults_KeyDown(sender As Object, e As KeyEventArgs) Handles txtResults.KeyDown
		If Not _searchMode OrElse e.KeyCode <> Keys.Enter OrElse e.Shift Then Return
		e.SuppressKeyPress = True
		Dim lines() As String = txtResults.Lines
		Dim term As String = ""
		For i As Integer = lines.Length - 1 To 0 Step -1
			If lines(i).Trim() <> "" Then
				term = lines(i).Trim()
				Exit For
			End If
		Next
		If term = "" Then Return
		If _commManager Is Nothing Then _commManager = New CommManager2()
		Dim records As String = _commManager.GetSQLLikeCustomers(txtProductFamily.Text, term)
		txtResults.Text = If(records <> "", records & vbCrLf, "") &
		                  "Search: " & term & vbCrLf &
		                  "Enter Customer Name or CustomerID To Search:" & vbCrLf
		txtResults.SelectionStart = txtResults.Text.Length
		txtResults.ScrollToCaret()
	End Sub
	Private Async Sub btnEnableDTLS_Click(sender As Object, e As EventArgs) Handles btnEnableDTLS.Click
		if txtProductFamily.text.Contains("AclaraRF3") Then
			Await SendMsg("mfgpSecurityAuthMode 2")
		Else
			Await SendMsg("appSecurityAuthMode 2")
		End If
	End Sub
	Dim comm As New SerialComm
	Private Sub btnDisableDTLS_Click(sender As Object, e As EventArgs) Handles btnDisableDTLS.Click
		Dim path As String = Application.StartupPath & "\CertKeys\"
		If comm.CheckCertFilesExist(path) = False Then '.......BOOLEAN TRUE/FALSE
			txtResults.Text = "Cert File Missing"
			txtResults.BackColor = Color.Red
			Me.Refresh
		Else
			DTLSLogoff()
		End If
	End Sub
	Private Sub DTLSLogoff()
		Dim PortName As String = "COM" & txtCommPort.Text
		Dim BaudRate As Integer = 38400
		Dim dtlscommand As String = ""
		If txtProductFamily.Text.Contains("KV2c") Then BaudRate = 9600
		Dim certpath As String = Application.StartupPath & "\CertKeys\"

		txtResults.Text = "Attempting Connection"
		Me.Refresh
		If comm.DTLSConnect(PortName, BaudRate, certpath, 15000) = False Then
			txtResults.Text = "DTLS Connect Error"
			txtResults.BackColor = Color.Yellow
			Me.Refresh
			Return
		End If
'Thread.Sleep(15000)
txtResults.Text = "Logging Off"
Thread.Sleep(500)
		Me.Refresh 
		if txtProductFamily.text.Contains("AclaraRF3") Then
			dtlscommand = "mfgpSecurityAuthMode 0"
		else			
			dtlscommand = "appSecurityAuthMode 0"
		End If

		If comm.Send(dtlscommand) = False Then '.......BOOLEAN TRUE/FALSE
			txtResults.BackColor = Color.Yellow
			txtResults.Text = "Failed to Log Off"
		Else
			txtResults.BackColor = Color.White
			txtResults.Text = "Successfully Logged Off"
		End If
		Me.Refresh
		Thread.Sleep(500)
		comm.Close()
	End Sub
	Private Sub txtCommPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtCommPort.SelectedIndexChanged
		SetFormXml()
		txtcommand.Select
	End Sub
	Private Sub btnUpdatePort_Click(sender As Object, e As EventArgs) Handles btnUpdatePort.Click
		ShowPorts()
	End Sub
	Private Sub txtDebugPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtDebugPort.SelectedIndexChanged
		If txtDebugPort.SelectedIndex <> -1 Then
			btnSendDebug.Visible = True
		Else
			btnSendDebug.Visible = False
		End If
	End Sub

	Private Sub btnClose_Click(sender As Object, e As EventArgs)
		txtPassword.text = ""
	End Sub


Private Sub btn_ToggleUSB_Click(sender As Object, e As EventArgs) Handles btn_ToggleUSB.Click
	Dim portNum As String = txtRelayPort.Text.Trim()
	If portNum = "" Then
		txtResults.Text = "USB Relay: no Relay Port selected"
		txtResults.BackColor = Color.Red
		Return
	End If
	Try
		CommManager2.RelayPort = portNum
		Dim state As String = CommManager2.RelaySetAndGet(1, Not CommManager2.UsbIsOn)
		If state IsNot Nothing Then
			txtResults.Text = "Relay State: " & If(CommManager2.UsbIsOn, "ON (Closed)", "OFF (Open)") & " (verified)"
			txtResults.BackColor = If(CommManager2.UsbIsOn, Color.LightGreen, Color.Yellow)
			btn_ToggleUSB.Text = If(CommManager2.UsbIsOn, "USB ON", "USB OFF")
			btn_ToggleUSB.BackColor = If(CommManager2.UsbIsOn, Color.Green, Color.Yellow)
		Else
			txtResults.Text = "USB Relay: state unknown after command"
			txtResults.BackColor = Color.Yellow
		End If
	Catch ex As Exception
		txtResults.Text = "USB Relay error: " & ex.Message
		txtResults.BackColor = Color.Red
	End Try
End Sub
Private Function ToggleE2PowerOn As Boolean 

	Dim usbdata As String = ""
	Dim open_relay As Byte() = {&H41, &H54, &H2B, &H43, &H48, &H31, &H3D, &H30}
    Dim close_relay As Byte() = {&H41, &H54, &H2B, &H43, &H48, &H31, &H3D, &H31}
'	Dim usbport As New SerialPort(Aux.E2LITE_RELAY, 9600, Parity.None, 8, StopBits.One)
Try
'usbport.Open()
'open relay
'usbport.Write(open_relay, 0, 8)
'        Thread.Sleep(Aux.E2LITE_POWERDELAY) ' (comes from INI file, I think it's 8000ms)
'read state
'	usbdata = usbport.ReadExisting()
'	If Not usbdata.Contains("CH1=0") Then
'		Throw New Exception("Open K Expected CH1=0, Received '" & usbdata & "'")
'	End If

'close relay
'	usbport.Write(close_relay, 0, 8)
'	Thread.Sleep(Aux.E2LITE_POWERDELAY)
'read state
'	usbdata = usbport.ReadExisting()
'	If Not usbdata.Contains("CH1=1") Then
'		Throw New Exception("Close K Expected CH1=1, Received '" & usbdata & "'")
'	End If
'ok, good
Return True
    Catch ex As Exception
Return False
Finally
'If usbport.IsOpen Then
'	usbport.Close()
'End If
End Try
End Function
Private Function ToggleE2PowerOff As Boolean 
    Dim usbdata As String = ""
    Dim open_relay As Byte() = {&H41, &H54, &H2B, &H43, &H48, &H31, &H3D, &H30}
    Dim close_relay As Byte() = {&H41, &H54, &H2B, &H43, &H48, &H31, &H3D, &H31}
'    Dim usbport As New SerialPort(Aux.E2LITE_RELAY, 9600, Parity.None, 8, StopBits.One)
'    Try
'      usbport.Open()
'        Select Case _onoff
'            Case True
                'close relay
'                usbport.Write(close_relay, 0, 8)
'                Thread.Sleep(SRFNConsts.E2RELAYREADDELAY)
                'read state
'                usbdata = usbport.ReadExisting()
'                If Not usbdata.Contains("CH1=1") Then
'                    Throw New Exception("Close K Expected CH1=1, Received '" & usbdata & "'")
'                End If
'            Case False
                'open relay
'                usbport.Write(open_relay, 0, 8)
 '               Thread.Sleep(SRFNConsts.E2RELAYREADDELAY)
                'read state
 '               usbdata = usbport.ReadExisting()
'                If Not usbdata.Contains("CH1=0") Then
'                    Throw New Exception("Open K Expected CH1=0, Received '" & usbdata & "'")
'                End If
 '       End Select

        'ok, good
        Return True
'    Catch ex As Exception
'        Return False
'    Finally
'        If usbport.IsOpen Then
'            usbport.Close()
'        End If
'    End Try
End Function
Private Async Function ToggleUSB(ByVal msg As String) As Task
		txtcommand.Text = msg
		txtResults.Text = "Toggle USB Command" & vbCrLf & msg
		If msg = "" Then txtResults.Text = "No Command Sent"
		txtResults.BackColor = Color.Yellow
		Me.Refresh()
		Threading.Thread.Sleep(100)
		If _commManager Is Nothing Then
			_commManager = New CommManager2()
		End If
		Dim SetExtEncoding As Object = _commManager.Encoding
		'Dim response As String = Await _commManager.SendUSBPortMsg(msg, CInt(txtCommPort.Text), txtProductFamily.Text)

		'If Mid(response, 1, 4) = "Fail" Then
		'	txtResults.BackColor = Color.Red
		'ElseIf Mid(response, 1, 4) = "Pass" Then
		'	txtResults.BackColor = Color.Green
		'ElseIf response.Contains("is not a valid") Then
		'	txtResults.BackColor = Color.Yellow
		'Else
		'	txtResults.BackColor = Color.White
		'End If
		'txtResults.Text = response
	End Function
	'============================================================================================================
	Private Sub LockAllButtons()
		btnLockForm.PerformClick
	    For Each ctrl As Control In Me.Controls
			If TypeOf ctrl Is Button Then
	            ctrl.Enabled = False
	        End If
		Next
		txtCommPort.Enabled = False
		btnUpdatePort.Enabled = False
		txtProductFamily.Enabled = False
		txtMacAddress.Enabled = False
		txtProductFamily.Enabled = False
		txtRFCarrier.Enabled = False
		txtCustomerID.Enabled = False
		btnSave.Enabled = False
		txtMeterSerial.Enabled = False
		txtUtilitySerial.Enabled = False
		txtPackPath.Enabled = False
		btnReset.Enabled = True
		btnEnableDebug.Enabled = True
		'txtPassword.Enabled = False
		txtFormID.Enabled = False
	btnErase.Visible = True
	btnErase.Enabled = True
	btnUpdateFW.Visible = True
	btnUpdateFW.Enabled = True
	btnLockForm.Enabled = False
	btnGetCustomerName .Enabled = False
	btnReset.Size = New Size(90, 37)
	btnLockForm.Visible = False
    'btnReset.Location = New Point(423, 386)
	'btnReset.Visible = True
	'btnReset.Enabled = True
	Me.Refresh 
	End Sub

	Private Async Sub btnErase_Click(sender As Object, e As EventArgs)
		Await CommManager2.ExecMassErase()
	End Sub

	Private Sub btnUpdateFW_Click(sender As Object, e As EventArgs) 
		btnEnableDebug.PerformClick()
		btnReset.Enabled = True
	End Sub
	Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
	End Sub

#Region "Firmware SQL"
	Public SelectedFirmwareName As String = ""

	Public Property SQLServer As String Implements IMainForm.SQLServer
		Get
			Return _sqlServer
		End Get
		Set(value As String)
			_sqlServer = value
		End Set
	End Property

	Public Property FirmwareConnStr As String Implements IMainForm.FirmwareConnStr
		Get
			Return _firmwareConnStr
		End Get
		Set(value As String)
			_firmwareConnStr = value
		End Set
	End Property

	Private Function BuildFirmwareConnStr() As String
		Dim cs As String = FirmwareConnStr
		If cs = "" Then cs = DLL_Revision.Text
		If cs <> "" Then Return cs
		Return "Provider=SQLOLEDB;Data Source=" & _sqlServer & ";Initial Catalog=Aclara_CustomerSpecific;Integrated Security=SSPI;"
	End Function


	Public Sub WriteFirmwareToNIC(firmwareName As String) Implements IMainForm.WriteFirmwareToNIC
		SelectedFirmwareName = firmwareName
		txtResults.Text = "Downloading firmware from SQL..."
		Me.Refresh()
		Try
			Dim filepath As String = CommManager2.DownloadFirmwareHexByName(firmwareName.Trim(), txtProductFamily.Text)
			If filepath = "" Then
				txtResults.Text = "Row not found in Aclara_Firmware: ProductFamily=" & txtProductFamily.Text & " FileName=" & firmwareName
				Exit Sub
			End If
			Dim ra6Dir As String = IO.Path.GetDirectoryName(filepath)
			Dim secKeys As String() = If(IO.Directory.Exists(ra6Dir), IO.Directory.GetFiles(ra6Dir, "dlm-ssd-*.rkey", IO.SearchOption.TopDirectoryOnly), New String() {})
			Dim nonsecKeys As String() = If(IO.Directory.Exists(ra6Dir), IO.Directory.GetFiles(ra6Dir, "dlm-nsecsd-*.rkey", IO.SearchOption.TopDirectoryOnly), New String() {})
			Dim msg As String = "rfp-cli -t E2l -d RA -if uart -s 1.5M -a -file " & Chr(34) & filepath & Chr(34)
			If secKeys.Length > 0 AndAlso nonsecKeys.Length > 0 Then
				msg &= " -fo seckey " & Chr(34) & secKeys(0) & Chr(34) & " -fo nonseckey " & Chr(34) & nonsecKeys(0) & Chr(34)
			End If
			StartProcess(RA6ProgPath.Text, msg)
		Catch ex As Exception
			txtResults.Text = "Download failed: " & ex.Message
		End Try
	End Sub

	Private Sub mnuUploadFirmware_Click(sender As Object, e As EventArgs) Handles mnuUploadFirmware.Click
		Dim frm As New frmFirmware(Me)
		frm.ShowDialog(Me)
	End Sub

	Private Sub lblPortStatus_Click(sender As Object, e As EventArgs) Handles lblPortStatus.Click

	End Sub
#End Region

	' ── IMainForm wrappers ─────────────────────────────────────────────────
	Public Property DLLRevisionText As String Implements IMainForm.DLLRevisionText
		Get
			Return DLL_Revision.Text
		End Get
		Set(v As String)
			DLL_Revision.Text = v
		End Set
	End Property
	Public Property CustomerValuesText As String Implements IMainForm.CustomerValuesText
		Get
			Return SRFN_CustomerValues.Text
		End Get
		Set(v As String)
			SRFN_CustomerValues.Text = v
		End Set
	End Property
	Public Property TestResultsText As String Implements IMainForm.TestResultsText
		Get
			Return SRFN_TestResults.Text
		End Get
		Set(v As String)
			SRFN_TestResults.Text = v
		End Set
	End Property
	Public Property SQLServerText As String Implements IMainForm.SQLServerText
		Get
			Return txtSQLServer.Text
		End Get
		Set(v As String)
			txtSQLServer.Text = v
			SQLServer = v
		End Set
	End Property
	Public Property ShowFormChecked As Boolean Implements IMainForm.ShowFormChecked
		Get
			Return chkShowForm.Checked
		End Get
		Set(v As Boolean)
			chkShowForm.Checked = v
		End Set
	End Property
	Public Property RA6ProgPathText As String Implements IMainForm.RA6ProgPathText
		Get
			Return RA6ProgPath.Text
		End Get
		Set(v As String)
			RA6ProgPath.Text = v
		End Set
	End Property
	Public WriteOnly Property PasswordText As String Implements IMainForm.PasswordText
		Set(v As String)
			txtPassword.Text = v
		End Set
	End Property
	Public Property CommPortText As String Implements IMainForm.CommPortText
		Get
			Return txtCommPort.Text
		End Get
		Set(v As String)
			txtCommPort.Text = v
		End Set
	End Property
	Public Property DebugPortText As String Implements IMainForm.DebugPortText
		Get
			Return txtDebugPort.Text
		End Get
		Set(v As String)
			txtDebugPort.Text = v
		End Set
	End Property
	Public Property RelayPortText As String Implements IMainForm.RelayPortText
		Get
			Return txtRelayPort.Text
		End Get
		Set(v As String)
			txtRelayPort.Text = v
		End Set
	End Property
	Private _opticalPort As String = ""
	Public Property OpticalPortText As String Implements IMainForm.OpticalPortText
		Get
			Return _opticalPort
		End Get
		Set(v As String)
			_opticalPort = v
		End Set
	End Property
	Public Property ProductFamilyText As String Implements IMainForm.ProductFamilyText
		Get
			Return txtProductFamily.Text
		End Get
		Set(v As String)
			txtProductFamily.Text = v
		End Set
	End Property

End Class
