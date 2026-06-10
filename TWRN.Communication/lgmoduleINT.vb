Option Strict On
Option Explicit On
Imports VB = Microsoft.VisualBasic
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Threading.Tasks

Namespace TWRN
Public Class lgmoduleINT
    Private _testContext As TestContext
    
    Public Sub New()
        Try
            ' Use the global test context by default for backward compatibility
            _testContext = DMTP1.TestContext
            ' Initialize TestFail through the property to ensure proper initialization
            DMTP1.TestFail = True
        Catch ex As Exception
        End Try
    End Sub
    
    ' Constructor for dependency injection (future use)
    Public Sub New(testContext As TestContext)
        _testContext = testContext
        TestFail = True
    End Sub
    
    ' Property to access the test context
    Public ReadOnly Property Context As TestContext
        Get
            Return _testContext
        End Get
    End Property
#Region "Run Test"
Public Async Function ReadTWACSID(CommPort As Integer) As Task(Of String)
    Return Await Module1.ReadTWACSID(CommPort)
End Function
Public Async Function ACLARA_INTEGRATIONDotNetTest(ByVal nComPort As Integer, ByVal varParam1 As String, ByVal varParam2 As String) As Task(Of String)
        Try
            ' Reset test state for new test run
            ResetTestState()
            
            Dim progress As New Progress
            
            ' Initialize test parameters
            If Not InitializeTestParameters(varParam1, varParam2) Then
                Return "FAIL - Parameter initialization failed"
            End If
            
            gintRef = 0
            logstart
            ' Perform pre-test validation
            Dim preTestResult As String = Await PerformPreTestValidation(progress, Module1.Varparams(1), Module1.Varparams(5))
            If preTestResult <> "Pass" Then
                Return preTestResult
            End If

            ' Perform communication setup and firmware reading
            Dim commSetupResult As String = Await PerformCommunicationSetup(progress, nComPort, Module1.Varparams(5), Module1.Varparams(1))
            If commSetupResult <> "Pass" Then
                TestFail = True
                GoTo FailTest
            End If

            ' Load configuration and parameters
            Dim configResult As String = Await LoadTestConfiguration(progress)
            If configResult <> "Pass" Then
                TestFail = True
                errcode = 2
                GoTo StopTest
            End If

            ' Execute the main test
            gintRef += 1
            gstrcommandname = "STARTING TEST"
            UpdateUpdate(progress)
            UpdatePass(progress)
            If TestFail = False Then
                Dim run As String = Await RunTest(progress)
            End If

StopTest: '...........................................
            If gblnFAIL_EEPROM = True Then TestFail = True
            If TestFail = True Then
                TestResult = "Fail"
            Else
                TestResult = "Pass"
                errcode = 0     'PASSED: Clear errcode
            End If

EndTest:
            If gBaudRate = 9600 Then 
                gintRef += 1
                'gstrcommandname = "Resetting Baud = 2400"
                UpdateUpdate(progress)
                If Await SetBaud(progress,2400) = "Pass" then
                    UpdateUpdate(progress)
                Else
                    'FastBaudEnabled = False
                    'UpdateFail(progress)
                End If
            End If
            

FailTest:
            If progress.IsHandleCreated Then progress.Close()
            GetTestResults()        'Write Values to Aclara_TestResults.dbo.UMT_R_F_TestResults
            progress = Nothing      '...........MAKE SURE TEST FAILS IF NOT PASSING
            'SetResults()        'increment Results.ini file

            If TestFail = True Then
                If gblnFAIL_EEPROM = True AndAlso TestCommand.Length > 39 Then TestCommand = EECommand 'If EE Fail - insert Where Fail
                If TestCommand.Length > 40 Then TestCommand = gstrcommandname 'Shorted to fit L&G 40 Char Error Length
                log.Append("Failed" & ", Ref: " & gintRef & "; " & gstrcommandname & vbCrLf & _
                "-------------------------------------------" & vbCrLf)
                lgmodule.Print_logfile'.................... PRINT LOG FILE if Test FAILS
                Return "Failed" & ", " & lgmodule.CommandName
            Else
                log.Append("Test Pass")
                lgmodule.Print_logfile '.................... SKIP LOG FILE if Test PASSES
                Return "Passed"
            End If
            gintRef = 0
            FileClose(1)
            Return "Failed, Unknown Error-Contact Aclara"
        Catch ex As Exception
            log.Append("Failed" & ", " & errcode & ";" & TestCommand & vbLf & "Test Pass")
            lgmodule.Print_logfile
            gintRef = 0
            TestFail = True
            Return "Failed" & ", " & gstrcommandname
        End Try
    End Function

    ' Helper methods for improved code organization
    Private Sub ResetTestState()
        If _testContext IsNot Nothing Then
            _testContext.Reset()
        End If
        errcode = 0
        TestResult = String.Empty
        TestCommand = String.Empty
        EECommand = String.Empty
        EEFailCommand = String.Empty
    End Sub
    
    Private Function InitializeTestParameters(ByVal varParam1 As String, ByVal varParam2 As String) As Boolean
        Try
            Dim varPar1 As String() = Split(varParam1, ",")
            Dim varPar2 As String() = Split(varParam2, ",")
            
            If varPar1.Length < 4 OrElse varPar2.Length < 6 Then
                Return False
            End If
            
            Module1.Varparams(0) = varPar1(0) ' MeterSerialNumber
            Module1.Varparams(1) = varPar1(1) ' DCSISerialNumber
            Module1.Varparams(2) = varPar1(2) ' SQLName
            Module1.Varparams(3) = varPar1(3) ' Meter Firmware
            Module1.Varparams(4) = varPar2(0) ' CustomerID
            Module1.Varparams(5) = varPar2(1) ' ProductFamily
            Module1.Varparams(6) = varPar2(2) ' FormID
            Module1.Varparams(7) = varPar2(3) ' RFCarrier
            
            ' Clean up PackPath
            Dim PackPath As String = Trim(varPar2(4))
            If PackPath.EndsWith("\") Then
                PackPath = PackPath.Substring(0, PackPath.Length - 1)
            End If
            Module1.Varparams(8) = PackPath
            Module1.Varparams(9) = varPar2(5) ' RDInstalled
            
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Async Function PerformPreTestValidation(ByVal progress As Progress, ByVal serialNumber As String, ByVal prodFamily As String) As Task(Of String)
        Try
            ' Check SQL Connection
            gintRef += 1
            gstrcommandname = "Check SQL Connection"
            UpdateUpdate(progress)
            If lgmodule.SetSQLServer() = False Then
                errcode = 2
                UpdateFail(progress)
                Return "FAIL - DLL not supported on Database"
            End If
            UpdatePass(progress)
            progress.lblcustomername.BackColor = System.Drawing.Color.White
            progress.ProgressBar1.Maximum = 55
            progress.Show()

            ' Validate Serial Number
            gintRef += 1
            gstrcommandname = "Validate Serial Number"
            UpdateUpdate(progress)
            If lgmodule.SerialNumberVal(serialNumber) = False Then
                TestCommand = gstrcommandname
                UpdateFailTest(progress)
                TestFail = True
                Return "FAIL - Invalid Serial Number"
            End If
            UpdatePass(progress)

            ' Get Integration Parameters
            gintRef += 1
            gstrcommandname = "Get Integration Parameters"
            If GetIntegrationParameters(progress, prodFamily) = False Then
                UpdateFailTest(progress)
                Return "Fail - Get integration parameters Failure"
            End If
            UpdatePass(progress)

            Return "Pass"
        Catch ex As Exception
            Return "Fail - Pre-test validation error: " & ex.Message
        End Try
    End Function

    Private Async Function PerformCommunicationSetup(ByVal progress As Progress, ByVal comPort As Integer, ByVal prodFamily As String, ByVal serialNumber As String) As Task(Of String)
        Try
            ' Module Wakeup
            gintRef += 1
            gstrcommandname = "Module Wakeup - Initial Read Serial"
            gBaudRate = 2400
            Dim awake As String = Await ModuleWakeup(progress, comPort, prodFamily, serialNumber)
            If awake = "Fail" Then
                UpdateUpdate(progress)
                UpdateFailTest(progress)
                Return "Fail"
            Else
                UpdateUpdate(progress)
                UpdatePass(progress)
            End If

            ' Set Fast Baud Rate if applicable
            gintRef += 1
            UpdateUpdate(progress)
            If FastBaudEnabled = True Then
                If Await SetBaud(progress, 9600) = "Pass" Then
                    FastBaudEnabled = True
                    UpdatePass(progress)
                Else
                    UpdateFailTest(progress)
                    TestFail = True
                    Return "Fail"
                End If
            End If

            ' Read Module Firmware
            gintRef += 1
            gstrcommandname = "Read Module Firmware"
            Dim Firmware As String = Await ReadFWver(progress, comPort)
            If Firmware = "Fail" Then
                UpdateFailTest(progress)
                Return "Fail"
            Else
                UpdateUpdate(progress)
                UpdatePass(progress)
            End If

            ' Read Meter Firmware for applicable product families
            If FirmwareUtilities.RequiresMeterFirmwareRead(prodFamily) Then
                gintRef += 1
                gstrcommandname = "Read Meter Firmware"
                Dim meterfirmware As String = Await ReadMeterFWver(progress)
                If InStr(meterfirmware, "Fail") > 0 Then
                    Return "Fail"
                End If
                Module1.Varparams(3) = meterfirmware
            End If

            Return "Pass"
        Catch ex As Exception
            Return "Fail - Communication setup error: " & ex.Message
        End Try
    End Function

    Private Async Function LoadTestConfiguration(ByVal progress As Progress) As Task(Of String)
        Try
            ' Get Integration Values
            If Not ExecuteTestStep(progress, "Get " & Module1.Varparams(5) & "Integration Values.xml", 
                                 Function() GetIntegrationValues(progress)) Then
                errcode = 2
                Return "Fail"
            End If

            ' Get Customer Values
            If Not ExecuteTestStep(progress, "Get Customer Values", 
                                 Function() GetCustomerValues(progress)) Then
                Return "Fail"
            End If

            ' Swap Maps if needed (firmware dependent)
            If FirmwareUtilities.RequiresMapSwapping(Module1.Varparams(3)) Then
                If Not ExecuteTestStep(progress, "Swap Mapping Meter per FW " & Module1.Varparams(5) & "_" & Module1.Varparams(3) & ".xml",
                                     Function() SwapMapsbyFW(progress)) Then
                    Return "Fail"
                End If
            End If

            ' Get Command Parameters
            If Not ExecuteTestStep(progress, "Reading " & Module1.Varparams(5) & "_" & Module1.Varparams(3) & ".xml",
                                 Function() GetCommandParameters(progress)) Then
                errcode = 2
                Return "Fail"
            End If

            Return "Pass"
        Catch ex As Exception
            Return "Fail - Configuration loading error: " & ex.Message
        End Try
    End Function

    ' Common helper for executing test steps with consistent progress reporting
    Private Function ExecuteTestStep(ByVal progress As Progress, ByVal stepName As String, ByVal testFunction As Func(Of Boolean)) As Boolean
        gintRef += 1
        gstrcommandname = stepName
        UpdateUpdate(progress)
        
        Try
            Dim result As Boolean = testFunction()
            If result AndAlso Not TestFail Then
                UpdatePass(progress)
                Return True
            Else
                HandleTestFailure(progress, stepName, "Test step failed")
                Return False
            End If
        Catch ex As Exception
            HandleTestFailure(progress, stepName, ex.Message)
            Return False
        End Try
    End Function

    ' Centralized error handling and logging
    Private Sub HandleTestFailure(ByVal progress As Progress, ByVal stepName As String, ByVal errorMessage As String)
        gstrcommandname = stepName
        TestCommand = stepName
        TestFail = True
        UpdateFailTest(progress)
        
        ' Log the error details
        log.Append("FAILED - Step: " & stepName & vbCrLf & "Error: " & errorMessage & vbCrLf)
    End Sub

    ' Improved exception logging with more context
    Private Function LogException(ByVal methodName As String, ByVal ex As Exception) As String
        Dim errorMsg As String = String.Format("{0} failed: {1} (Error {2})", methodName, ex.Message, ex.HResult)
        log.Append("EXCEPTION - " & errorMsg & vbCrLf & "Stack Trace: " & ex.StackTrace & vbCrLf)
        Return errorMsg
    End Function
#End Region
#Region "Properties"
    Public Property DLLRevDate() As String
        Get
            DLLRevDate = "8/27/2025"
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property DLL() As String
        Get
            DLL = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
        Set(ByVal Value As String)
            DLLver = DLL
        End Set
    End Property
    Public Property ScriptDescrip() As String
        Get
            ScriptDescrip = gstrcommandname '& vbLf & gstrfailmessage.TrimStart
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property TestDescrip() As String
        Get
            TestDescrip = Module1.Varparams(5) & " - " & Module1.Varparams(4) '& vbLf & gstrcommandname '& vbLf & gstrfailmessage.TrimStart 'TestCommand
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property TestFailure() As String
        Get
            TestFailure = gstrcommandname
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property TestDate() As String
        Get
            TestDate = DateTime
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property SerialRead() As String
        Get
            SerialRead = DecSerialRead
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property SerialScanned() As String
        Get
            SerialScanned = Module1.Varparams(1)
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property CommandName() As String
        Get
            CommandName = "Ref: " & gintRef & " - " & gstrcommandname
            'gCommandParameters = ""
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property FailMessage() As String
        Get
            'If gblnFAIL_EEPROM = True Then
            'FailMessage = gstrfailmessage
            'Else
            FailMessage = gstrcommandname
            'End If
            'gCommandParameters = ""        'Orig gstrfailmessage = ""
            'gstrcommandname = ""
            'gstrfailmessage = ""
            Err.Clear()
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property BadCommunication() As Boolean
        Get
            'BadCommunication = gblnbadcommunication
        End Get
        Set(ByVal Value As Boolean)
            'gblnbadcommunication = BadCommunication
        End Set
    End Property
    Public Property EERead() As String
        Get
            EERead = gProduct & " - 'TWACSID: " & gSerialNumber & " EEMap: " & gEEMap & _
            vbLf & "DateTime: " & VB.Format(Now, "MM/dd/yyyy-hh:mm:ss") & _
            vbLf & vbLf & RegDatRecvd

            'EERead = gProduct & " - 'TWACSID: " & gSerialNumber & " EEMap: " & gEEMap & _
            'vbLf & "DateTime: " & VB.Format(Now, "MM/dd/yyyy-hh:mm:ss") & _
            'vbLf & vbLf & RegDataRcvd.DefInstance._RichTextBox1_1.Text
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property Pack_Path() As String ', ByVal Fail)
        Get
            Pack_Path = Module1.Varparams(8)
            gPackPath = ""
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property Results() As String ', ByVal Fail)
        Get
            GetResults()
            Results = TstPass & "," & TstFail
        End Get
        Set(ByVal Value As String)
        End Set
    End Property
    Public Property EEPromPack() As String
        Get
            'EEPromPack = gstrfailmessage 'gPackPath & gProduct & "-" & gSerialNumber & ".EEPROM"
            If EEPromPack = "" Then EEPromPack = gPackPath & gProduct & "-" & gSerialNumber & ".EEPROM"
            'If gProduct = "" Then EEPromPack = gstrfailmessage
        End Get
        Set(ByVal Value As String)
            DLLver = DLL
        End Set
    End Property
#End Region
#Region "Demo Calls"
#End Region
    Private Function GetProdFamily(ByVal ProdFamily As String) As String
        Select Case ProdFamily
            Case "UMT-C-KV2"
                ProdFamily = "UMT-C-KV2"
            Case "UMT-C-KV2_Gen5"
                ProdFamily = "UMT-C-KV2"
            Case "UMTR-G2"
                ProdFamily = "UMT-GCB"
            Case "UMT-GCB10_6.xx"
                ProdFamily = "UMT-GCB"
            Case "UMT-GCB12_6.xx"
                ProdFamily = "UMT-GCB"
        End Select
        Return ProdFamily
    End Function
    Public Function GetConnStr() As Object
        'Public Function GetConnStr(ByVal table As String, byval filename As string)
        GetConnStr = Nothing
        Dim Intxmlfile As String = Application.StartupPath & "\ApplicationValues\SQLTables.xml"
        Try
            Dim ds As New DataSet()
            ds.ReadXml(Intxmlfile)
            '.....DLL Rev
            Module1.Varparams(10) = CStr(ds.Tables("DLLRevision").Rows(0).Item(0)).ToString '..... connection string
            Module1.Varparams(11) = CStr(ds.Tables("DLLRevision").Rows(0).Item(1)).ToString '..... database
            Module1.Varparams(12) = CStr(ds.Tables("DLLRevision").Rows(0).Item(2)).ToString '..... source
            '.....Product
            Module1.Varparams(13) = CStr(ds.Tables("Product").Rows(0).Item(0)).ToString '..... connection string
            Module1.Varparams(14) = CStr(ds.Tables("Product").Rows(0).Item(1)).ToString '..... database
            Module1.Varparams(15) = CStr(ds.Tables("Product").Rows(0).Item(2)).ToString '..... source
            '.....Result
            Module1.Varparams(16) = CStr(ds.Tables("Result").Rows(0).Item(0)).ToString '..... connection string
            Module1.Varparams(17) = CStr(ds.Tables("Result").Rows(0).Item(1)).ToString '..... database
            Module1.Varparams(18) = CStr(ds.Tables("Result").Rows(0).Item(2)).ToString '..... source
            Return True
        Catch ex As Exception
            MsgBox("Error GetConnStr", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Public Function CheckConn(ByVal table As String, ByVal filename As String) As Object
        CheckConn = Nothing
        Dim Intxmlfile As String = Application.StartupPath & "\ApplicationValues\SQLTables.xml"
        Try
            Dim ds As New DataSet()
            ds.ReadXml(Intxmlfile)
            '.....DLL Rev
            Return {
            CStr(ds.Tables("DLLRevision").Rows(0).Item(0)).ToString,
            CStr(ds.Tables("DLLRevision").Rows(0).Item(1)).ToString,
            CStr(ds.Tables("DLLRevision").Rows(0).Item(2)).ToString}
            '    Return {str,dba,src}
        Catch ex As Exception
            MsgBox("Error CheckConn", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Public Shared Sub logstart()
        log.Clear
        Dim time As DateTime = Now()
        log.Append(vbCrLf & _
            "============= NEW TEST =============" & vbCrLf & _
            "Test Date:    " & time & vbCrLf & _
            "Product:      " & Module1.Varparams(5) & vbCrLf & _
            "Form Voltage: " & Module1.Varparams(6) & vbCrLf & _
            "CustomerID:   " & Module1.Varparams(4) & vbCrLf & _
            "Meter Serial: " & Module1.Varparams(0) & vbCrLf & _
            "TWACSID:      " & Module1.Varparams(1) & vbCrLf & _
            "DLLVer:       " & lgmodule.DLL & vbCrLf & _
            "Pack Path:    " & Module1.Varparams(8) & vbCrLf & _
            "TestPC:       " & System.Environment.MachineName & vbCrLf & _
            "-------------------------------------------" & vbCrLf)
    End Sub
    Private Sub Print_logfile()
        Try
            Dim NewPackPath As String = Module1.Varparams(8) & "\" & Format(Date.Now(), "MM") & "-" & Format(Date.Now(), "yyyy")
            If Not Directory.Exists(NewPackPath) Then
                Directory.CreateDirectory(NewPackPath)
            End If

            Dim logfile As String = NewPackPath & "\" & Module1.Varparams(1) & "_" & Module1.Varparams(5) & "_" & Module1.Varparams(4) & ".tst"
            Dim fs As StreamWriter
            If (Not File.Exists(logfile)) Then
                fs = File.CreateText(logfile)
                fs.WriteLine(log)
            Else
                fs = File.AppendText(logfile)
                fs.WriteLine(log)
            End If
            fs.Close

            '................... Print Integration.DAT
            Dim sw As StreamWriter
            'Result:Fail; TWACSID:11111111 Product:UMTR-F-RXe_4.xx; CustomerID: 123456; TestDate:7/7/2021 11:12:21 AM; DLL:v3.10.0.0; ScannedID:11111111 - Read Reg 90 Firing Angle
            Dim dat As String = ""
            If HexSerialRead = "" Then
                dat = "TWACSID:" & Module1.Varparams(1) & " Product:" & Module1.Varparams(5) & "; CustomerID: " & Module1.Varparams(4) & "; TestDate:" & Now() & "; DLL:v" & DLL & "; ScannedID: No Module Comm"
            Else
                If InStr(HexSerialRead, "No Comm") > 0 Then
                    dat = "TWACSID:" & Module1.Varparams(1) & " Product:" & Module1.Varparams(5) & "; CustomerID: " & Module1.Varparams(4) & "; TestDate:" & Now() & "; DLL:v" & DLL & "; ScannedID: NO COMM"
                    gstrcommandname = "NO COMM"
                Else
                    dat = "TWACSID:" & Module1.Varparams(1) & " Product:" & Module1.Varparams(5) & "; CustomerID: " & Module1.Varparams(4) & "; TestDate:" & Now() & "; DLL:v" & DLL & "; ScannedID:" & CInt("&H" & HexSerialRead)
                End If
            End If

            Dim datfile As String = Module1.Varparams(8) & "\Integration - " & Format(Date.Now(), "MM") & "-" & Format(Date.Now(), "yyyy") & ".dat"
            If (Not File.Exists(datfile)) Then
                sw = File.CreateText(datfile)
                sw.WriteLine("Start Int Log File - " & Now())
            Else
                sw = File.AppendText(datfile)
                If TestFail = True Then
                    sw.WriteLine("Result:Fail; " & dat & " - " & gstrcommandname)
                Else
                    sw.WriteLine("Result:Pass; " & dat)
                End If
            End If
            sw.Close
        Catch ex As Exception
            gstrcommandname = "Error in Printlogfile"
        End Try
    End Sub
    Public Sub CloseProgress(ByVal Progress As Progress)
        If Progress.IsHandleCreated Then Progress.Close()
        Progress = Nothing
    End Sub
Public Async Function DumpEEProm(ByVal nComPort As Integer, ByVal ProductFamily As String, ByVal DCSIScannedSerial As String) As Task(of String)
Try
Dim progress As New Progress
If ShowProgress = True Then
    'progress.ProgressBar1.Maximum = 25
    'progress.Show()
End If

WakeupModule:
    gintRef = 1
    gstrcommandname = "Module Wakeup - Inital Read Serial"
    gBaudRate = 2400
    Dim awake As string = Await ModuleWakeup(progress, nComPort, ProductFamily, DCSIScannedSerial) 
    If awake = "Fail" Then
       'gBaudRate = 2400
       ' Await SetBaud(progress, 2400) '........WHY IS THIS HERE?
        UpdateFailTest(progress)
        Return "Fail"
        'GoTo FailTest
    Else
        'UpdateUpdate(progress)
        'UpdatePass(progress)
    End If

ReadModuleFirmware: '............................RunTest.283
    gintRef += 1
    gstrcommandname = "Read Module Firmware"
    Dim Firmware As String = Await ReadFWver(progress, nComPort) 
    Firmware = Module1.Varparams(20)
    If Firmware.Length = 0 Then
        UpdateFailTest(progress)
        Return "Fail"
        'GoTo FailTest
    Else
        UpdateUpdate(progress)
        UpdatePass(progress)
    End If

    Firmware = CInt("&H" & Mid(InbData, 1, 2)) & "." & CInt("&H" & Mid(InbData, 3, 2)) 
    Dim filename As String = ProductFamily & "_" & Firmware
    gintRef += 1
    gstrcommandname = "Get " & filename & " EEProm"
    UpdateUpdate(progress)

GetEEProm:
    gstrcommandname = "Read " & gProduct & " - " & gEEMap & " EEPROM Parameters.xml"
    UpdateUpdate(progress)

    Dim resp As String = Await ReadEEProm(progress, nComPort, filename)
    progress.Close()
    progress = Nothing
    Return resp

Catch ex As Exception
    gstrcommandname = Err.Number & " " & Err.Description
    'MsgBox("Dump EEProm Error - q" & gstrfailmessage, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Function
    Private Sub GetSQLValues(ByVal ProdFamily As String)
        gBaudRate = 2400 '...default
        DLLver = lgmodule.DLL
        Dim Table As String = "wired"
        If Mid(ProdFamily, 1, 5) = "UMT-C" Then Table = "optical"
        Try
            Dim showform As String = ""
            Dim doc As New XmlDocument()
            Dim xmldoc As String = Application.StartupPath & "\ApplicationValues\SQLValues.xml"
            doc.Load(xmldoc)
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/SQLValues")
            For Each node As XmlNode In nodes
                gCustomerTest_PCName = node.SelectSingleNode("customerserver").InnerText
                gCustomerTest_DBAName = node.SelectSingleNode("customerDBA").InnerText
                gstrCnn_Customer = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" & gCustomerTest_DBAName & ";Data Source=" & gCustomerTest_PCName

                gIntegration_PCName = node.SelectSingleNode("integrationserver").InnerText
                gIntegration_DBAName = node.SelectSingleNode("customerDBA").InnerText
                gstrCnn_Integration = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" & gIntegration_DBAName & ";Data Source=" & gIntegration_PCName

                gTestResults_PCName = node.SelectSingleNode("resultsserver").InnerText
                gTestResults_DBAName = node.SelectSingleNode("resultsDBA").InnerText
                gstrCnn_TestResults = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" & gTestResults_DBAName & ";Data Source=" & gTestResults_PCName
            Next
        Catch ex As Exception
            MsgBox("Error Reading SQLValues.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Sub
    Private Sub WriteEEDump()
        Dim FilePath As String = gPackPath & gProduct & "-" & DecSerialRead & ".EEPROM"
        If File.Exists(FilePath) Then File.Delete(FilePath)
        FileOpen(1, FilePath, OpenMode.Append)
        PrintLine(1, "'TWACS ID: " & DecSerialRead & " - Customer: " & gCustomer & " - CustomerID: " & gCustomerId & _
        vbLf & "Script CreateDate: " & Drawing & " - TestDate: " & DateTime & vbLf)
        PrintLine(1, "DLLver " & DLLver)
        PrintLine(1, RegDatRecvd)
        PrintLine(1, "  " & gProduct)
        PrintLine(1, "")
    End Sub
    Public Function RunScript(ByVal ScriptName As String, ByRef progress As Progress, ByVal CommPort As Integer) As Boolean
        'gCommandParameters = ScriptName
        'ProductFamily = ProductFamily
        RunScript = False
        Try
            'gstrcommandname = gCommandParameters
            'gstrcommandname = "Verify " & gCommandParameters & " Script Present"
            ''UpdateUpdate(progress)
            gintRef += 1
            If VerifyScriptName(progress) = False Then
                ''UpdateFail(progress)
                TestFail = True
                GoTo ScriptNotFound
            Else
                ''UpdatePass(progress)
            End If
            'gstrcommandname = "Read " & gCommandParameters & " Script"
            'UpdateUpdate(progress)
            gintRef += 1
            If GetCommandParameters(progress) = True Then '....xml script read - OK\
                ''UpdatePass(progress)
                TestFail = False
                If TestFail = False Then Call RunTest(progress) 'Run the test
            Else
                ''UpdateFail(progress)
            End If
Done:
            If TestFail = True Then
                RunScript = False
            Else
                RunScript = True
            End If
            'BadCommunicationRoutine(progress)
            'GetTestResults()                'Write Values to Aclara_TestResults
ScriptNotFound:
            progress.Close()
        Catch ex As Exception
            MsgBox(ScriptName & vbLf & gstrcommandname & vbLf & ex.Message, MsgBoxStyle.Exclamation, "External Script Error")
            progress.Close()
        End Try
    End Function
    Public Function PackPath(ByRef strpackpath As String) As Boolean
        Try
            gPackPath = strpackpath
            If gPackPath = "" Then
                Return False
            End If
            Return True
        Catch ex As Exception
            TestFail = True
            gstrcommandname = Err.Number & " " & Err.Description
        End Try
    End Function
    Private Function GetCustomerDatabase(ByVal ProductFamily As String) As Boolean
        Dim strTemp1 As New StringBuilder
        Dim strTemp As String
        Dim i As Integer
        Dim y As Integer
        Dim strHash As String
        'Dim Rate_9600 As String
        'Dim Desc As String
        Dim dsIntParams As New System.Data.DataSet
        Dim file_name As String
        Try
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If
            file_name = Application.StartupPath & "\Aclara_TestParameters\SetIntegrationParameters.xml"
            dsIntParams.ReadXml(file_name)

            strTemp = CStr(dsIntParams.Tables("HashTable").Rows(0).Item(0))
            'strTemp1 = ""
            For i = 0 To dsIntParams.Tables("Table").Rows.Count - 1
                For y = 0 To dsIntParams.Tables("Table").Columns.Count - 1
                    strTemp1.Append(CStr(dsIntParams.Tables("Table").Rows(i).Item(y)))
                Next
            Next
            strHash = GenerateHash(strTemp1.ToString)
            If strHash <> strTemp Then 'Compare stored vs generated hash values
                TestFail = True
                errcode = 2 'LG Process Error
                gstrcommandname = ""
                gstrcommandname = "IntegrationParameters.xml Hash Mismatch"
                TestCommand = gstrcommandname
                MsgBox(gstrcommandname, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
                If gShowHash = True Then
                    gstrcommandname = "IntegrationParameters.xml= " & strHash
                End If
                Return True
            End If
SkipHash:
            intMaxRef = dsIntParams.Tables("Table").Rows.Count - 1
            i = 0

            While (i <= intMaxRef)
                If ProductFamily = CStr(dsIntParams.Tables("Table").Rows(i).Item("ProductName")) Then
                    gProduct = CStr(dsIntParams.Tables("Table").Rows(i).Item("gProduct"))
                    gIntegration = CStr(dsIntParams.Tables("Table").Rows(i).Item("gIntegrationDBA"))
                    gCustomerTest = CStr(dsIntParams.Tables("Table").Rows(i).Item("gCustomerTestDBA"))
                    'Exit While
                    'Rate_9600 = dsIntParams.Tables("Table").Rows(i).Item("FastDataCapable")
                    'If Rate_9600 = "T" Then
                    'FastBaudCapable = True 'Rate9600 = True
                    'Else
                    'FastBaudCapable = False 'Rate9600 = False
                    'gBaudRate = "2400"
                    'End If
                    'i = i + 1
                End If
                i = i + 1
            End While
            dsIntParams.Dispose()
            Return True
        Catch ex As Exception
            TestFail = True
            errcode = 2 'LG Process Error
            gstrcommandname = ""
            gstrcommandname = "SetIntegrationParameters.xml Error"
            TestCommand = gstrcommandname
            'MsgBox(ex, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal
            MsgBox(gstrcommandname & vbLf & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Private Sub MessageFail(ByRef Failure As String)
        MsgBox(Failure, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    End Sub
    Private Sub DLLFail(ByVal Rev As String)
        MsgBox("SQL connected, DLL " & Rev & " NOT CURRENT - UPDATE DLL to " & DLL, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    End Sub
    Private Sub SQLFail(ByVal Rev As String)
        MsgBox("SQL connected, Current DLL " & Rev & " NOT SUPPORTED - CONTACT ACLARA", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    End Sub
    Private Function SerialNumberVal(ByRef SerialNumber As String) As Boolean
        SerialNumberVal = False
        Try
            For Each Character As String In SerialNumber
                If Char.IsNumber(CChar(Character)) Then
                    SerialNumberVal = True
                ElseIf Char.IsLetter(CChar(Character)) Then
                    Return False
                ElseIf Not Char.IsWhiteSpace(CChar(Character)) Then
                    SerialNumberVal = False
                    Return False
                End If
            Next
        Catch ex As Exception
        End Try
    End Function
    Public Function SetSQLServer() As Boolean
        lgmodule.GetConnStr()
        Dim dcnDB As New ADODB.Connection
        Dim rsData As New ADODB.Recordset

        Dim Query As String = "SELECT Rev_Date FROM " & Module1.Varparams(11) & ".dbo.DLL_Revision WHERE DLL_Rev = '" & DLL() & "'"
        'gstrcommandname = ""
        Try
            EEFailCommand = " "
            dcnDB.Open(Module1.Varparams(10))
            rsData = dcnDB.Execute(Query)
            If Not rsData.EOF Then
                If RTrim(CStr(rsData.Fields(0).Value)) <> DLLRevDate() Then
                    SQLFail(DLL())
                    gstrcommandname = "DLL ERROR - UPDATE to " & DLL()
                    TestCommand = gstrcommandname
                    SetSQLServer = False
                    SQLFail(DLL())
                Else
                    'gstrcommandname = ""
                    Return True

                End If
            Else
                gstrcommandname = "Connection or DLL Version ERROR - DLL = " & DLL()
                TestCommand = gstrcommandname
                Return False
            End If
            rsData.Close()
            rsData = Nothing
        Catch ex As Exception
            errcode = 2         'Process Error
            MessageFail("SetSQLServer DLL Version ERROR - DLL = " & DLL() & Err.Number & Err.Description)
        End Try
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
End Namespace
