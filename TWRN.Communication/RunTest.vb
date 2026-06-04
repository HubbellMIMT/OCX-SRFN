Option Strict Off
Option Explicit On
Imports System.IO
Imports System.IO.Ports
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml
Imports System.Threading.Tasks

Namespace TWRN
Module Module1
    Public GenIntCollection As New Collection
    'Public WithEvents TWACS As TWACSliteGroup.TWACSprot
    Public dcnDB As New ADODB.Connection
    Public rsData As New ADODB.Recordset
    Public log As New StringBuilder
    Public FastBaudEnabled As Boolean = False 
    Public RegDatRecvd As String
    Public InbStatus As String
    Public InbData As String
    Public CustVals As String(,)
    Public Varparams(25) As String
    Public commandresult(100) As String
    Public commands As String(,)
    Public strexpected As String
    Public stractual As String
    'Public inbmessage As string
    Public last_fail As String
    Public ErrString As String
    Public strComparison As String
    Public blnFAIL As Boolean
    Public intDelayIndex As Short
    Public Compare As String
    Public blnnotdelay As Boolean
    Public gRegOp220Data As String
    Public gReg1850 As String
    Public gReg2247 As String
    'Public ErrReportLong As Boolean
    Public NonStandard As Boolean
    'Public gMVCCode As String
    'Public CustomerInfo As String
    Public Drawing As String
    Public gcalibration As String
    Public striedRSSI As String
    Public ghexRegIED As String
    Public retries As Integer
    Public intMaxRef As Integer
    Public gReg90 As String
    Public iedserfill As Integer
    Public TestMode As Boolean
    Public EETest As Boolean
    Public iedtest As Boolean
    Public gEMTRFW As String
    Public HANConfig As String
    Public RetryOnFail As Boolean
    Public Val(25) As String
    Public NewVal(25) As String
    Public SQL(50) As String
    Public gHash As Boolean
    Public gReg24 As String
    Public gReg48 As String
    Public gReg51 As String
    Public AutoRetry As Integer = 0
    Public ProductType As String
    Public gMeterSerialNumber As String
    Public DLLver As String
    Public gProduct As String
    Public ShowProgress As Boolean
    'Public gCommandParameters As String
    Public gShowHash As Boolean
    Public gBaudRate As Integer
    Public gEEMap As String
    Public Integrationvalues As Boolean
    'Public Customervalues As Boolean
    Public Testvalues As Boolean
    Public Command As String
    Public TestResult As String = "Fail"
    Public DateTime As String
    Public Customer As String
    Private TimeSet As Boolean
    Public EECapture As Boolean
    Public EEdump As String
    Public gSerialNumber As String
    Public gProdTemp As String
    Public gIEDInstalled As Boolean
    Public gType As String
    Public gTypeFW As String
    Public EERead As String
    Public gRegEERead As String
    Public gReg As String
    Public LoadEE1 As Boolean
    Public BadCommunicationRoutineAttempt As Boolean
    Public gProgressMax As Integer
    Public gFCTFile As String
    Public FCTTest As Boolean
    Public EEFailCommand As String
    Public TstPass As String
    Public TstFail As String
    Public SQLWriteFail As String
    Public DecSerialRead As String
    Public HexSerialRead As String
    'Public DecSerialScanned As String
    Public TestCommand As String
    '.............EMTR
    Public gEMTRMapped As Boolean
    Public gEMTRSelected As Boolean
    Public gEMTRTest As Boolean
    Public gEMTR_Tested As Boolean
    Public gEMTRPass As Boolean
    Public gEMTRPresent As Boolean
    '.............Read EEPROM
    'Public TWACSOpcode As String
    Public InbStr As String
    Public EECommand As String
    Public gFCTProduct As String
    Public gReg1730 As String
    Public gReg229 As String
    Public gReg239 As String
    Public gRFboard As String
    Public ModuleComm As Boolean
    'Public BaudRate9600 As Boolean
    Public pwrupretry As Integer
    'Public gModulePass As Boolean
    Public errcode As Integer = 0
    Public WriteSteps As Boolean = False
    Public gMeterFW As String = ""
    Public q As Integer = 0
    '...............................................SQL Database Names
    Public gIntegration As String '.................Integration TABLE (SetIntegration Parameters)
    Public gCustomerTest As String '................Script TABLE
    Public gTestResults As String '.................Results TABLE

    Public gIntegration_PCName As String '..........Integration PC (comm.ini)
    Public gIntegration_DBAName As String '.........Integration Database (comm.ini)
    Public gstrCnn_Integration As String '.........Integration Database Connection String
    Public gCustomerTest_PCName As String '.........Customer Script PC
    Public gCustomerTest_DBAName As String '........Customer Script Database
    Public gstrCnn_Customer As String '.........Customer Database Connection String
    Public gTestResults_PCName As String '..........Results PC
    Public gTestResults_DBAName As String '.........Results Database
    Public gstrCnn_TestResults As String '.........Integration Database Connection String
    '========================================= NEW PORT VARIABLES
    'Public inbLen As integer
    'Public comPort As New SerialPort()
    Public gTDelay As Integer
    Public InbLen As Integer
    Public icnt As Integer
    Private comm As New CommManager()
    Sub New()
        comm.Parity = "None"
        comm.StopBits = "One"
        comm.DataBits = "8"
        comm.BaudRate = "2400"
    End Sub
Public Function RemoveInbHeader(ByVal inbounbdmsg As string) As String
    inbounbdmsg = inbounbdmsg.Replace(ChrW(1),"").Replace(ChrW(4),"").Replace(ChrW(6),"").Replace("?","").Replace(vbNullChar,"")
    If inbounbdmsg.Length > 6 then inbounbdmsg = inbounbdmsg.Substring(0, inbounbdmsg.Length - 2)
    stractual = string.Empty 
    stractual = inbounbdmsg
    Select Case comm.opcode
    Case "3E"
            InbStatus = Mid(stractual,9,4)
            Inbdata = Mid(stractual,13)
    Case "3F"
            InbStatus = Mid(stractual,9,4)
            Inbdata = Mid(stractual,13)
    Case "45"
            InbStatus = Mid(stractual,9,8)
            Inbdata = Mid(stractual,17)
    Case "38"
            InbStatus = Mid(stractual,9,2)
            'Inbdata = Mid(stractual,17)
    Case "DC"
            InbStatus = Mid(stractual,9,2)
    Case "5F"
            InbData = Mid(stractual,9)

    End Select
    Return stractual
End Function
Public Async Function ModuleWakeup(progress As Progress, CommPort As Integer, ProdFamily As String, ScannedTWACSID As String) As Task(Of String)
Try
    comm.PortName = "COM" & CommPort
    gBaudRate = 2400
    comm.BaudRate = Cint(gBaudRate)
    comm.inbLen = "9"
    comm.opcode = "3E"
    comm.outbmsg = "0184"
    GetPortValues '.................... Get global Task Delay Value "gTDelay"

    For j As Integer = 0 to pwrupretry +1
    Await comm.WriteData(comm.outbmsg)
    stractual = RemoveInbHeader(stractual)
    Select Case InbData.Length 
        Case 0
            Threading.Thread.Sleep(100)
            gstrcommandname = "No Communication"    
            'If RetryBox(icnt) = False Or j > pwrupretry Then
            'If RetryBox(icnt) = True Then
            '    gstrcommandname = "Wakeup Failed Reset Baud = 2400"
            '    UpdateUpdate(progress)
            '    if Await SetBaud(progress, 2400) = "Pass" Then
            '        j = 0
            '    else
            '        TestFail = True 
            '        gstrcommandname = "Wakeup Failure No Comm"
            '        Return "Fail"
            '    End If
            'Else
            '    gstrcommandname = "Wakeup Failure No Comm"
            '    Return "Fail"
            'End If
        Case > 0
            If ScannedTWACSID = Convert.ToInt32(InbData,16) Then
                DecSerialRead = ScannedTWACSID
                HexSerialRead = InbData 
                gReg24 = HexSerialRead
                TestFail = False
                Return "Pass"
            Else
                gstrcommandname = "Serial Number Mismatch"
                'If RetryBox(icnt) = False Or j > pwrupretry Then
                TestFail = True 
                'End If
                Return "Fail"
            End If
    End Select
Next

Catch ex As Exception
    TestFail = True
    UpdateFail(progress)
    gstrcommandname = "Module Wakeup Error"
    MsgBox(gstrcommandname & vbLf, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    End Try
End Function
Public Async Function ReadFWver(ByVal progress As Progress, ByVal CommPort As Integer) As Task (of String)
TestFail = True
Try
    comm.inbLen = "9"
    comm.opcode = "3E"
    comm.outbmsg = "0304"

For i As Integer = 0 to pwrupretry +1
    Await comm.WriteData(comm.outbmsg)
    stractual = RemoveInbHeader(stractual)
    Select Case InbData.Length 
        Case 0
            Threading.Thread.Sleep(100)
            gstrcommandname = "No Communication"
            UpdateFail(progress)
            'If RetryBox(icnt) = False Or i > pwrupretry Then
            '    Return "Fail"
            'End If
        Case > 0
            Varparams(20) = CInt("&H" & Mid(InbData, 1, 2)) & "." & _
            CInt("&H" & Mid(InbData, 3, 2)) & "." & _
            CInt("&H" & Mid(InbData, 5, 2)) & "." & _
            CInt("&H" & Mid(InbData, 7, 2))
            gstrcommandname = "Module Firmware = " & Varparams(20)
            'UpdatePass(progress)
            Return "Pass"
    End Select
Next

Catch ex As Exception
    TestFail = True
    UpdateFail(progress)
    gstrcommandname = "ReadFWver Error"
    MsgBox(gstrcommandname & vbLf, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Function
Public Async Function ReadMeterFWver(ByVal progress As Progress) As Task (of String)
TestFail = True
'................................ First Set Reg 290 = 12240001200E
    comm.inbLen = "7"
    comm.opcode = "3F"
    comm.outbmsg = "12240001200E"
Try 
    For icnt = 0 To 3
        gstrcommandname = "Set Reg290 to Read Meter F/W"
        UpdateUpdate(progress)
        Await comm.WriteData(comm.outbmsg)
            stractual = RemoveInbHeader(stractual)
            If InbData = HexSerialRead Then
                UpdatePass(progress)
                Exit For 
            End If
            if icnt > 2 then
                gstrcommandname = "Fail to Set 290=12240001200E"
                TestFail = True 
                Return "Fail"
        End If
    Next
Catch ex As Exception
End Try    
'................................ Now Read the Reg 305 to get Meter Firmware Version
    System.Threading.Thread.Sleep(750)
    comm.opcode = "3E"
    comm.inbLen = "9"
    comm.outbmsg = "1328"
    Dim fw As String = ""
    Dim fwdec As Integer = 0
    TryAgain:
    Try
        For icnt = 0 To 3
                gstrcommandname = "Read Meter F/W"
                UpdateUpdate(progress)
                  Await comm.WriteData(comm.outbmsg)
                stractual = RemoveInbHeader(stractual)
                fw = Mid(InbData, InbData.Length - 3, 4) '........0521 = 5.33, 0522 = 5.34, 0525 = 5.37, 0547 = 5.71
                fwdec = CInt(fw)
                Select Case fwdec
                    Case = 521, 522, 525, 547
                        UpdatePass(progress)
                        TestFail = False
                        Return Mid(fw, 1, 2) & CInt("&H" & (Mid(fw, 3, 2)))
                    Case Else
                        gstrcommandname = "Fail-Meter F/W=" & Mid(fw, 1, 2) & CInt("&H" & (Mid(fw, 3, 2)))
                        UpdateFail(progress)
                        if icnt > 3 Then Return "Fail"
                End Select
            Next
            gstrcommandname = "Fail-Meter F/W " & fwdec
            UpdateFail(progress)
            Return "Fail"

        Catch ex As Exception
            TestFail = True
            errcode = 4
            gstrcommandname = "Fail-Meter F/W " & fwdec
            UpdateFail(progress)
            Return "Fail"
        End Try
    End Function
Public Async Function RunTest(ByVal progress As Progress) As Task (of String)
Dim Temp1 As String
Dim Temp2 As String
Dim Temp3 As String
Dim intlength As Integer
Dim Retry As Boolean
Dim y As Integer
Dim strExpectTemp As String
Dim j As Integer = 0
Dim isleep As Integer
        Try
            '............................................................Replace Expected values with Customervals
            For i As Integer = 0 To commands.GetLength(0) - 1
                Dim tm As Integer = InStr(commands(i, 6), "Reg")
                If InStr(commands(i, 6), "Reg") > 0 Then
                    commands(i, 6) = SwapComands(commands(i, 6))
                End If
                If InStr(commands(i, 8), "Reg") > 0 Then
                    commands(i, 8) = SwapComands(commands(i, 8))
                End If
            Next

            gintRef += 1 'gintRef & 1   'Increments Command Reference Count (First Test Commands = 10)
            Dim gint As Integer = commands.GetUpperBound(0)
            progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
            progress.ProgressBar1.Maximum = gintRef + gint + 2
For i As Integer = 0 To commands.GetUpperBound(0)
    TestFail = True
RetryCommand:
    gstrcommandname = commands(i, 3)
    if i = 44 then
        stractual = stractual
    End If
    UpdateUpdate(progress)
    gstrcommandname = commands(i, 3) '........................Command Name
    Compare = commands(i, 7) '................................Compare
    strexpected = commands(i, 8)  '...........................Expected
    'comm.outbmsg = gstrcommandname
    comm.inbLen = CStr(Hex(commands(i, 5)))
    comm.opcode = commands(i, 4) '...........................Opcode
    comm.outbmsg = Replace(commands(i, 6), ";", "")'...............outbmessage
    Await comm.WriteData(comm.outbmsg)
    stractual = RemoveInbHeader(stractual)
    Dim intime As DateTime = Now()
    strexpected = Swap(strexpected)
    Select Case comm.opcode 'TWACS.Opcode
        Case "45" '..................................... Set Date Time
        stractual = InbStatus
        Case "3F" '..................................... Write Reg
        stractual = InbStatus
        Case "3E" '..................................... Read Reg
        stractual = InbData 
        Case "38" '..................................... Execute Pending Write
        stractual = InbStatus
        Case "DC" '..................................... 
        stractual = InbStatus
        Case "5F" '..................................... Read EEProm
        stractual = InbData
    End Select
    Select Case (Compare)'.............................. TestFail = True
        Case 0
            If stractual = "" Then '........................ Retry Command
                Exit Select
            Else
            TestFail = False
        End If
        Case 1  '....................................... Must Match
            If stractual = strexpected Then
                TestFail = False
                GoTo ignore
            Else
                intlength = Len(strexpected)
                Do Until intlength = 0
                    If Mid(strexpected, intlength, 1) = "x" OrElse Mid(strexpected, intlength, 1) = "X" Then GoTo skip
                    If Mid(strexpected, intlength, 1) <> Mid(stractual, intlength, 1) Then
                        TestFail = True
                        Exit Select
                    End If
                skip:
                    intlength = intlength - 1
                Loop
            End If
        Case 2
            If CInt("&H" & stractual) < CInt("&H" & strexpected) Then
                TestFail = False
            Else
                TestFail = True
            End If
        Case 3
            If CInt("&H" & stractual) > CInt("&H" & strexpected) Then
                TestFail = False
            Else
                TestFail = True
            End If
        Case 4
            'x = 0
            y = InStr(strexpected, "-")
            strExpectTemp = strexpected
            gblnFAIL_EEPROM = False
            TestFail = True
            Case 5
            y = InStr(strexpected, "|")
            Temp1 = Mid(strexpected, 1, y - 1)
            Temp1 = Mid(Temp1, 1, InStr(Temp1, "X") - 1)
            Temp2 = Mid(strexpected, y + 1, Temp1.Length)
            Temp3 = Mid(stractual, 1, Temp1.Length)
            If Mid(stractual, 1, Temp1.Length) = Temp1 OrElse Mid(stractual, 1, Temp1.Length) = Temp2 Then
                TestFail = False
                gblnFAIL_EEPROM = False
            Else
                TestFail = True
                'gstrcommandname = gstrcommandname & " Act=" & stractual & " Exp=" & Temp1 & "|" & Temp2
            End If
            End Select
ignore:
            If TestFail = True OrElse gblnFAIL_EEPROM = True OrElse gblnbadcommunication = True Then
                'errcode = errorcode(i)
                UpdateFail(progress)
            Else
                UpdatePass(progress)
            End If

            If isleep <> 0 Then
                System.Threading.Thread.Sleep(isleep * 1000)
            End If

            If TestFail = True Then
            If commands(i, 11) > 0 Then '................................. Retries > 0
            UpdateFail(progress)
            commands(i, 11) = commands(i, 11) - 1  '........................ Retries > 0
            System.Threading.Thread.Sleep(2 * 1000) 'Wait 2 seconds
            Retry = True
            errcode = 0
            TestFail = False  '
            gblnFAIL_EEPROM = False '
            'gblnbadcommunication = False '
            GoTo RetryCommand '........................................... Try Again
                Else
            UpdateFail(progress)
                GoTo TestFail '............................................... Failure TEST OVER
                End If
            End If

            gProgressMax = gProgressMax + j - 1
            'progress.ProgressBar1.Maximum = gProgressMax
            gintRef += 1
            Next
            Return "Passed"

TestFail:
        Return "Failed" & lgmodule.CommandName
        Catch ex As Exception
            gstrcommandname = Err.Number & " " & Err.Description
        End Try

ErrorHandler:
        TestFail = True
        progress.Close()
        errcode = 2
        'gstrcommandname = Err.Number & " " & Err.Description
        Return "Fail"
End Function
Public Async Function SetBaud(ByVal progress As Progress, ByVal Baud As integer) As Task (of String)
gintRef += 1
Dim tmpgstrcommandname as string = gstrcommandname
Try
comm.opcode = "3F" '.....Set Fast Baud 9600 - Set Reg2231 = 0x02
comm.inbLen = "9"
If Baud = 9600 then '..................... Set Baud **TO 9600**
    comm.outbmsg = "8B7102"
    gstrcommandname = "Set Fast Baud = 9600"
    gBaudRate = 2400
else
    comm.outbmsg = "8B7100"
    gstrcommandname = "Reset Baud = 2400"
    gBaudRate = 9600
End If
icnt = 0
For icnt = 0 To 3
    UpdateUpdate(progress)
    Await comm.WriteData(comm.outbmsg)
    stractual = RemoveInbHeader(stractual)
    If InbStatus = "0000" Then
        if comm.outbmsg = "8B7102" Then
            gBaudRate = 9600
        Else
            gBaudRate = 2400
        End If
        gstrcommandname = tmpgstrcommandname
        'UpdatePass(Progress)
        Return "Pass"
    Else
        gstrcommandname = "SetBaud Error"
        UpdateUpdate(progress)
        gstrcommandname = tmpgstrcommandname
        'TestFail = True 
        UpdateFail(Progress)
        'Return "Fail"
    End If
Next
Return "Fail"

Catch ex As Exception
    TestFail = True
    errcode = 1 'LG Process Error
    gstrcommandname = "SetFastRate Error"
    TestCommand = gstrcommandname
End Try
End Function

#Region "Serialized XML"
    '============================================== Check SQL State Close if Open
    Private Sub CheckSQLState()
        Try
            If dcnDB IsNot Nothing Then
                If dcnDB.State > 0 Then dcnDB.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Function EleParse(ByVal lne As String) As String
        Dim i As Integer = lne.IndexOf("<")
        EleParse = lne.Substring(i + 1, lne.IndexOf(">", i + 1) - i - 1)
    End Function
    Private Function ValParse(ByVal lne As String) As String
        Dim i As Integer = lne.IndexOf(">")
        ValParse = lne.Substring(i + 1, lne.IndexOf("</", i + 1) - i - 1)
    End Function
    Private Function ReadIntegrationxml(ProductFamily, Firmware) As Array
        ReadIntegrationxml = Nothing
        Dim Intxmlfile As String = Application.StartupPath & "\ProductValues\" & ProductFamily & "_" & Firmware & "_IntValues.xml"
        Dim IntVals(200, 1) As String
        'Dim IntVals As New List(Of List(Of String))
        Dim i As Integer
        Dim lne As String
        Try
            Using reader As StreamReader = New StreamReader(Intxmlfile)
                While Not reader.EndOfStream
                    lne = reader.ReadLine()
                    If i < 6 AndAlso InStr(lne, "</") > 0 AndAlso Mid(lne, 1, 2) <> "</" Then
                        IntVals(i, 0) = EleParse(lne)
                        IntVals(i, 1) = ValParse(lne)
                        i += 1
                    End If
                    If i > 5 AndAlso InStr(lne, "CommandName") > 0 Then
                        IntVals(i, 0) = ValParse(lne)
                        IntVals(i, 1) = ValParse(reader.ReadLine())
                        i += 1
                    End If
                    If InStr(lne, "FormVoltage") > 0 Then
                        IntVals(i, 0) = EleParse(lne)
                        IntVals(i, 1) = ValParse(lne)
                        lne = reader.ReadLine()
                        i += 1
                        'do again till see /Table
                        While InStr(lne, "/Table") = 0
                            IntVals(i, 0) = EleParse(lne)
                            IntVals(i, 1) = ValParse(lne)
                            lne = reader.ReadLine()
                            i += 1
                        End While
                    End If
                End While
                Dim tmp As String = IntVals(200, 1)
                '====================================== Return Integration Values
                Return IntVals

            End Using
            Return "Fail on ReadIntegrationxml" & ProductFamily & Firmware
        Catch ex As Exception
        End Try
    End Function
    Private Function CreatIntegrationxml(ByRef progress As Progress, ByVal gstrCnn_Integration As String, ByVal ProductFamily As String, ByVal Firmware As String) As Boolean
        '..................................................... GET PRODUCT INTEGRATION VALUES
        Dim Intxml(6) As String
        Try
            gIntegration = "Aclara_SerialIntegrationValues"
            Dim Query As String = "SELECT * FROM " & gIntegration & " WITH (NOLOCK) WHERE ProductFamily = '" & ProductFamily & "' AND Reg48 = '" & Firmware & "'"
            CheckSQLState()
            dcnDB.Open(gstrCnn_Integration)
            rsData = dcnDB.Execute(Query)
            Intxml(0) = RTrim(CStr(rsData.Fields(1).Value)) 'ProductType
            Intxml(1) = RTrim(CStr(rsData.Fields(2).Value)) 'ProductFamily
            Intxml(2) = RTrim(CStr(rsData.Fields(3).Value)) 'Product
            Intxml(3) = RTrim(CStr(rsData.Fields(4).Value)) 'Drawing
            Intxml(4) = RTrim(CStr(rsData.Fields(5).Value)) 'Reg48
            Intxml(5) = RTrim(CStr(rsData.Fields(6).Value)) 'HashVal
            Intxml(6) = RTrim(CStr(rsData.Fields(7).Value)) 'IntVals
            rsData.Close()
            rsData = Nothing
        Catch ex As Exception
            CreatIntegrationxml = False
            MsgBox("CreateIntegrationxml SQL Retrieve Error: " & ex.ToString, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try

        Try
            '============================================== Write XML to new Text-XML File
            Dim setoptionvals As String = ""
            setoptionvals = "<?xml version=""1.0"" standalone=""yes""?>" & vbCrLf
            setoptionvals += "<NewDataSet>" & vbCrLf
            setoptionvals += "<HashTable>" & vbCrLf
            setoptionvals += "	<ProductType>" & Intxml(0) & "</ProductType>" & vbCrLf
            setoptionvals += "	<ProductFamily>" & Intxml(1) & "</ProductFamily>" & vbCrLf
            setoptionvals += "	<Product>" & Intxml(2) & "</Product>" & vbCrLf
            setoptionvals += "	<Drawing>" & Intxml(3) & "</Drawing>" & vbCrLf
            setoptionvals += "	<Reg48>" & Intxml(4) & "</Reg48>" & vbCrLf
            setoptionvals += "	<HashVal>" & Intxml(5) & "</HashVal>" & vbCrLf
            setoptionvals += "</HashTable>" & vbCrLf
            setoptionvals += "	<IntValues>" & Intxml(6) & "</IntValues>"
            setoptionvals += Environment.NewLine & "</NewDataSet>"
            setoptionvals = setoptionvals.Replace("><", ">" & Environment.NewLine & "<")
            'Catch ex As Exception
            '    CreatIntegrationxml = False
            '    MsgBox("CreateIntegrationxml Create xml Error: " & ex.ToString, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            'End Try
            '============================================== Attempt the Write
            Dim Intxmlfile As String = Application.StartupPath & "\ProductValues\" & ProductFamily & "_" & Firmware & "_IntValues.xml"
            If File.Exists(Intxmlfile) Then SetAttr(Intxmlfile, vbNormal)
            System.IO.File.WriteAllText(Intxmlfile, setoptionvals)
            SetAttr(Intxmlfile, vbReadOnly)
            Return True
        Catch ex As Exception
            '_TestFail = True
            MsgBox("CreateIntegrationxml Write Error: " & ex.ToString, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
#End Region
    Private Function GetHashSQL(ByVal Progress As Progress, ByVal IntegrationTable As String, ByVal ProductFamily As String, ByVal Firmware As String) As String
        GetHashSQL = Nothing
        Try
            Dim Query As String = "SELECT * FROM [Aclara_CustomerSpecific].[dbo].[Aclara_SerialIntegrationValues] WITH (NOLOCK) WHERE ProductFamily = '" & ProductFamily & "' AND Reg48 = '" & Firmware & "'"
            CheckSQLState()
            dcnDB.Open(IntegrationTable)
            rsData = dcnDB.Execute(Query)
            Return RTrim(CStr(rsData.Fields(6).Value))
            rsData.Close()
            rsData = Nothing
        Catch ex As Exception
            MsgBox("GetSQLCustomerScript() SQL Retrieve Error: " & ex.ToString, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return "Fail"
        End Try
    End Function
    Public Function VerifyScriptName(ByVal Progress) As Boolean
        Dim file_name As String
        Dim dsCommParams As New System.Data.DataSet
        Try
            dsCommParams.ReadXml(file_name)
            VerifyScriptName = True
        Catch ex As Exception
            VerifyScriptName = False
            'MsgBox(gCommandParameters & " Script NOT FOUND - Contact Aclara", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Private Function ConvertMap(ByVal Progress As Progress, ByVal Map As String) As String
        ConvertMap = ""
        Dim Draw As String = ""
        Dim DrawRev As String = ""
        Try
            If Mid(Map, 1, 1) = "Y" OrElse Mid(Map, 1, 1) = "y" Then
                Draw = Mid(Map, 2, 5)
                DrawRev = Mid(Map, Len(Map), 1)
            End If
            Return Draw & "," & DrawRev
        Catch ex As Exception
        End Try
    End Function
    Private Function ConvertDLL(ByVal Progress As Progress, ByVal DLL As String) As String
        '...remove all values to get 3 digit number
        Dim TempDLL As String = ""
        Dim i As Integer = 1
        Try
            Do Until Len(TempDLL) > 2
                If IsNumeric(Mid(DLL, i, 1)) = True Then
                    TempDLL = TempDLL & Hex(Mid(DLL, i, 1))
                End If
                i = i + 1
                If i > 20 Then Exit Do
            Loop
            Return TempDLL
        Catch ex As Exception
            ConvertDLL = "ERR"
        End Try
    End Function
    Public Function GetCommandParameters(ByRef progress As Progress) As Boolean
        TestFail = True
        Dim i As Integer
        Dim y As Integer
        'Dim strTemp1 As String
        Dim strTemp1 As New StringBuilder
        Dim dsCommParams As New System.Data.DataSet
        Dim file_name As String
        '................. file does NOT exist >> Get SQL (productfamil = Varparams(5), firmware = Varparams(20) ** DONE
        '................. file does exist  >> Compare HashValues ** DONE
        '................. HashValues Match >> Proceed ** DONE
        '................. HashValues do NOT Match >> Delete Local File & Get SQL
        Try
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If
            '.................................If Integration xml is not local PULL FROM SQL
            file_name = Application.StartupPath & "\Aclara_TestParameters\Commands\" & Varparams(5) & "_" & Varparams(20) & "_Commands.xml"
            If Not File.Exists(file_name) Then
                If GetIntVal(progress, "Commands") = False Then
                    gstrcommandname = Varparams(5) & "_" & Varparams(20) & "_Commands Missing from Database"
                    Return False
                End If
            End If

            gstrcommandname = "Verify " & Varparams(5) & " Commands Hash Value"
            UpdatePass(progress)
            dsCommParams.ReadXml(file_name)
            For i = 0 To dsCommParams.Tables("Table").Rows.Count - 1 '............Now Validate Local Hash Value
                For y = 0 To dsCommParams.Tables("Table").Columns.Count - 1
                    strTemp1.Append(dsCommParams.Tables("Table").Rows(i).Item(y))
                Next
            Next
            '.................................Calculate Local Hash value from xml
            Dim CalculatedHash As String = GenerateHash(strTemp1.ToString)
            '.................................Get Local Hash value from xml
            Dim tmpHash As String = dsCommParams.Tables("HashTable").Rows(0).Item(0)
            '.................................Get SQL Master Hash value
            gintRef = gintRef + 1
            Dim getsqlhash As String = GetsqlHashVal(progress, "Commands")
            '.................................Compare Local to SQL Master Hash value
            If CalculatedHash <> getsqlhash Then
                File.Delete(file_name)
                gstrcommandname = Varparams(5) & " SQL IntegrationValue Hash Mismatch - Retry"
                Return False
            End If
            '.................................Compare Local to SQL Master Hash value
            If CalculatedHash <> tmpHash Then 'Compare stored vs generated hash values
                File.Delete(file_name)
                gstrcommandname = Varparams(5) & " GetCommandParameters Hash Mismatch - Retry"
                TestCommand = gstrcommandname
                TestFail = True
                Return False
            End If
            UpdatePass(progress)

SkipHash:
            'Dim gComType, Temp1, CompareType, OpData, gRegNum, gRegVal As String
            Dim j As Integer = 0
            intMaxRef = dsCommParams.Tables("Table").Rows.Count - 1
            'Dim commands(intMaxRef, 15)
            ReDim Preserve commands(intMaxRef, 15)
            i = 0
            'j = 0
            While (i <= intMaxRef)
                commands(j, 0) = dsCommParams.Tables("Table").Rows(i).Item("gRefNumber")
                commands(j, 1) = dsCommParams.Tables("Table").Rows(i).Item("gCommType")
                commands(j, 2) = dsCommParams.Tables("Table").Rows(i).Item("gParameters")
                commands(j, 3) = dsCommParams.Tables("Table").Rows(i).Item("gCommandname")
                commands(j, 4) = dsCommParams.Tables("Table").Rows(i).Item("gOpcode")
                commands(j, 5) = dsCommParams.Tables("Table").Rows(i).Item("gInboundLen")
                commands(j, 6) = dsCommParams.Tables("Table").Rows(i).Item("gOpData")
                commands(j, 7) = dsCommParams.Tables("Table").Rows(i).Item("gCompare")
                commands(j, 8) = dsCommParams.Tables("Table").Rows(i).Item("gExpected")
                commands(j, 9) = dsCommParams.Tables("Table").Rows(i).Item("gSleep")
                commands(j, 10) = dsCommParams.Tables("Table").Rows(i).Item("gCommPass")
                commands(j, 11) = dsCommParams.Tables("Table").Rows(i).Item("gRetries")
                commands(j, 12) = dsCommParams.Tables("Table").Rows(i).Item("gModstractual")
                commands(j, 13) = dsCommParams.Tables("Table").Rows(i).Item("errcode")
                commands(j, 14) = dsCommParams.Tables("Table").Rows(i).Item("gJumpToPass")
                commands(j, 15) = dsCommParams.Tables("Table").Rows(i).Item("gJumpToFail")
                j = j + 1
                i = i + 1
            End While
            'UpdateUpdate(progress)
            dsCommParams.Dispose()
            TestFail = False
            Return True

        Catch ex As Exception
            TestFail = True
            errcode = 2 'LG Process Error
            File.Delete(file_name)
            Return False
            gstrcommandname = ""
            gstrcommandname = "IntegrationParameters.xml Hash Error"
            TestCommand = gstrcommandname
            'MsgBox(ex, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Public Function GenerateHash(ByVal SourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New UnicodeEncoding
        'Retrieve a byte array based on the source text
        Dim ByteSourceText As Byte() = Ue.GetBytes(SourceText)
        'Instantiate an MD5 Provider object
        Dim Md5 As New MD5CryptoServiceProvider
        'Compute the hash value from the source
        Dim ByteHash As Byte() = Md5.ComputeHash(ByteSourceText)
        'And convert it to String format for return
        Return Convert.ToBase64String(ByteHash)
    End Function
    Public Function ReadError(ByVal actual As String, ByVal expected As String)
        gstrcommandname = " A=" & actual & "/" & "E=" & expected
        If Len(gstrcommandname & gstrcommandname) > 40 Then
            gstrcommandname = ""
        End If
    End Function

    Private Function SQLErrorMessage(ByVal SQLError) As Object
        MsgBox(gstrcommandname & vbCrLf & SQLError, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        Return Nothing
    End Function
    Public Function GetIntVal(ByRef progress As Progress, ByVal Func As String) As Boolean
        GetIntVal = False
        Dim IntVal As String = ""
        Dim filename As String = Application.StartupPath & "\Aclara_TestParameters\" & Func & "\" & Varparams(5) & "_" & Varparams(20) & "_" & Func & ".xml"
        gstrcommandname = "Reading GetIntVal for " & ProductFamily & " v" & Varparams(20)
        UpdateUpdate(progress)
        Try
            Dim Query As String = "SELECT [Config] FROM [Aclara_CustomerSpecific].[dbo].[Aclara_IntVals]" & _
            " WITH (NOLOCK) WHERE [ProductFamily] = '" & Varparams(5) & "' and [Firmware] = '" & Varparams(20) & "' and [Function] = '" & Func & "'"
            CheckSQLState()
            dcnDB.Open(Varparams(10))
            rsData = dcnDB.Execute(Query)
            If Not rsData.EOF Then
                IntVal = rsData.Fields(0).Value
            End If
            rsData.Close()
            rsData = Nothing
        Catch ex As Exception
        End Try
        '............................ NOW CREATE THE XML FILE
        Try
            IntVal = IntVal.Replace("><", ">|<")
            Dim tmp As String() = Split(IntVal, "|")
            Dim fs As New FileStream(filename, FileMode.Create, FileAccess.Write)
            Dim sa As New StreamWriter(fs)
            sa.WriteLine("<?xml version=" & Chr(34) & "1.0" & Chr(34) & " standalone=" & Chr(34) & "yes" & Chr(34) & "?>")
            For i As Integer = 0 To tmp.Length - 1
                sa.WriteLine(tmp(i))
            Next
            sa.Flush
            sa.Close()
            '............................
            Return True
        Catch ex As Exception
            Return False
            TestFail = True
            UpdateFail(progress)
            errcode = 2
            gstrcommandname = "GetIntVal sql Failed"
        End Try
    End Function
    Public Function GetsqlHashVal(ByRef progress As Progress, ByVal Func As String) As String
        GetsqlHashVal = "False"
        gstrcommandname = "Reading " & Func & " Hash Value"
        UpdateUpdate(progress)
        Try
            Dim Query As String = "SELECT [HashValue] FROM [Aclara_CustomerSpecific].[dbo].[Aclara_IntVals]" & _
            " WITH (NOLOCK) WHERE [ProductFamily] = '" & Varparams(5) & "' and [Function] = '" & Func & "' and [Firmware] = '" & Varparams(20) & "'"
            CheckSQLState()
            dcnDB.Open(Varparams(10))
            rsData = dcnDB.Execute(Query)
            If Not rsData.EOF Then
                GetsqlHashVal = RTrim(rsData.Fields(0).Value)
            End If
            rsData.Close()
            rsData = Nothing
            Return GetsqlHashVal

        Catch ex As Exception
            Return "False"
            TestFail = True
            UpdateFail(progress)
            errcode = 2
            gstrcommandname = "GetHashVal sql Failed"
        End Try
    End Function
    Public Function GetIntegrationValues(ByRef progress As Progress) As Boolean
        Dim i As Integer
        Dim y As Integer
        'Dim strTemp1 As String
        Dim strTemp1 As New StringBuilder
        Dim dsIntParams As New System.Data.DataSet
        Dim file_name As String
        '................. file does NOT exist >> Get SQL (productfamil = Varparams(5), firmware = Varparams(20) ** DONE
        '................. file does exist  >> Compare HashValues ** DONE
        '................. HashValues Match >> Proceed ** DONE
        '................. HashValues do NOT Match >> Delete Local File & Get SQL
        Try
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If
            '.................................If Integration xml is not local PULL FROM SQL
            file_name = Application.StartupPath & "\Aclara_TestParameters\IntegrationValues\" & Varparams(5) & "_" & Varparams(20) & "_IntegrationValues.xml"
            If Not File.Exists(file_name) Then
                If GetIntVal(progress, "IntegrationValues") = False Then
                    gstrcommandname = Varparams(5) & "_" & Varparams(20) & "_IntegrationValues Missing from Database"
                    Return False
                End If
            End If
            'UpdatePass(progress)

            gstrcommandname = "Verify " & Varparams(5) & " IntegrationValues Hash Value"
            gintRef += 1

            dsIntParams.ReadXml(file_name)
            For i = 0 To dsIntParams.Tables("Table").Rows.Count - 1 '............Now Validate Local Hash Value
                For y = 0 To dsIntParams.Tables("Table").Columns.Count - 1
                    strTemp1.Append(dsIntParams.Tables("Table").Rows(i).Item(y))
                Next
            Next
            '.................................Calculate Local Hash value from xml
            Dim CalculatedHash As String = GenerateHash(strTemp1.ToString)
            '.................................Get Local Hash value from xml
            Dim tmpHash As String = dsIntParams.Tables("HashTable").Rows(0).Item(0)
            '.................................Get SQL Master Hash value
            Dim getsqlhash As String = GetsqlHashVal(progress, "IntegrationValues")
            '.................................Compare Local to SQL Master Hash value
            If tmpHash <> getsqlhash Then
                File.Delete(file_name)
                gstrcommandname = Varparams(5) & " SQL IntegrationValue Hash Mismatch - Retry"
                TestFail = True
                Return False
            End If
            '.................................Compare Local to SQL Master Hash value
            If CalculatedHash <> tmpHash Then 'Compare stored vs generated hash values
                File.Delete(file_name)
                TestFail = True
                gstrcommandname = Varparams(5) & " IntegrationValue Mismatch - Retry"
                TestCommand = gstrcommandname
                Return False
            End If
            UpdatePass(progress)

SkipHash:
            '.................................Verify Integration Values
            If Varparams(5) <> dsIntParams.Tables("HashTable").Rows(0).Item(1) Or  '..............HashValue
                Varparams(20) <> dsIntParams.Tables("HashTable").Rows(0).Item(2) Then  '..........HashValue
                Return False
            End If

            For i = 0 To dsIntParams.Tables("Table").Rows.Count
                If Varparams(6) = dsIntParams.Tables("Table").Rows(i).Item("FormVoltage") OrElse
                    Varparams(6) = dsIntParams.Tables("Table").Rows(i).Item("NSFormVolta") Then
                    gProductType = dsIntParams.Tables("Table").Rows(i).Item("ProductType")
                    gReg90 = dsIntParams.Tables("Table").Rows(i).Item("Reg90")
                    dsIntParams.Dispose()
                    Return True
                End If
            Next

            If gIntegration = "" Then
                TestFail = True
                errcode = 2
                UpdateFail(progress)
                dsIntParams.Dispose()
                Return False
            End If

            UpdatePass(progress)
            dsIntParams.Dispose()

        Catch ex As Exception
            TestFail = True
            errcode = 2 'LG Process Error
            File.Delete(file_name)
            Return False
            gstrcommandname = ""
            gstrcommandname = "IntegrationParameters.xml Hash Error"
            TestCommand = gstrcommandname
            MsgBox(ex, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Public Function GetIntegrationParameters(ByRef progress As Progress, ByVal ProductFamily As String) As Boolean
        Dim strTemp1 As String
        Dim strTemp As String
        Dim i As Integer
        Dim y As Integer
        Dim strHash As String
        Dim Rate_9600 As String
        'Dim Desc As String
        Dim dsIntParams As New System.Data.DataSet
        Dim file_name As String
        gstrcommandname = "Get Integration Parameters.xml"
        UpdateUpdate(progress)
        Try
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If
            file_name = Application.StartupPath & "\Aclara_TestParameters\SetIntegrationParameters.xml"
            dsIntParams.ReadXml(file_name)

            strTemp = dsIntParams.Tables("HashTable").Rows(0).Item(0)
            strTemp1 = ""
            For i = 0 To dsIntParams.Tables("Table").Rows.Count - 1
                For y = 0 To dsIntParams.Tables("Table").Columns.Count - 1
                    strTemp1 = strTemp1 & dsIntParams.Tables("Table").Rows(i).Item(y)
                Next
            Next
            strHash = GenerateHash(strTemp1)
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
                Return False
            End If
SkipHash:
            intMaxRef = dsIntParams.Tables("Table").Rows.Count - 1
            i = 0
            While (i <= intMaxRef)
                If ProductFamily = dsIntParams.Tables("Table").Rows(i).Item("Productname") Then
                    gIntegration = dsIntParams.Tables("Table").Rows(i).Item("gIntegrationDBA")
                    gCustomerTest = dsIntParams.Tables("Table").Rows(i).Item("gCustomerTestDBA")
                    gTestResults = dsIntParams.Tables("Table").Rows(i).Item("gTestResultsDBA")
                    Varparams(21) = gTestResults
                    Rate_9600 = dsIntParams.Tables("Table").Rows(i).Item("Rate9600")
                    If Rate_9600 = "T" Then
                        FastBaudEnabled = True 'Rate9600 = True
                        gBaudRate = dsIntParams.Tables("Table").Rows(i).Item("BaudRate")
                    Else
                        FastBaudEnabled = False 'Rate9600 = False
                        gBaudRate = "2400"
                    End If
                    'If dsIntParams.Tables("Table").Rows(i).Item("SerialComm") = "O" Then OptiComm = True
                    dsIntParams.Dispose()
                    Return True
                End If
                i = i + 1
            End While
            If gIntegration = "" Then
                TestFail = True
                errcode = 2
                UpdateFail(progress)
                dsIntParams.Dispose()
                Return False
            End If
            UpdatePass(progress)
            dsIntParams.Dispose()
            Return True
        Catch ex As Exception
            TestFail = True
            errcode = 2 'LG Process Error
            GetIntegrationParameters = False
            gstrcommandname = ""
            gstrcommandname = "IntegrationParameters.xml Hash Error"
            TestCommand = gstrcommandname
            MsgBox(ex, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
'    Private Function RetryBox(ByRef RetCount As String) As Boolean
        'Dim F As New RetrySerial
'        Try
            'F.FormBorderStyle = FormBorderStyle.FixedDialog
            'F.FormBorderStyle = FormBorderStyle.FixedDialog
            'F.Size = New Size(336, 393)
            'F.Location = New Point(0, 0)
 '           F.StartPosition = FormStartPosition.CenterParent
            'F.Focus()
            'F.lblRetryTest.Focus()
            'RetrySerial.ActiveForm.Focus()
            'F.txtRetries.Text = "Retries = " & RetCount
 '           If RetCount = 0 Then
 '               F.lblRetryTest.BackColor = Color.Yellow
 '           End If

 '           F.ShowDialog()

 '           If RetryOnFail Then
 '               RetryBox = True     '.....retry = YES
 '               RetCount += 1
 '           Else
 '               RetryBox = False
 '           End If
 '       Catch ex As Exception
 '           MsgBox("RetryBox PopupBox Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
 '       End Try
'End Function
Public Async Function ReadEEProm(progress As Progress, CommPort As Integer, filename As String) As Task(Of String)
        '..................................... GET EEProm
        Dim strTemp1 As New StringBuilder
        Dim dsCommParams As New System.Data.DataSet
        Dim y As Integer = 0
        Dim tmpHash As String = ""
        Dim readlen As String = ""
        Try
            '.................................Read xml doc
            If Not File.Exists(Application.StartupPath & "\Aclara_TestParameters\EEMaps\" & filename & ".xml") Then
                'gstrfailmessage = filename &" EEprom xml Missing"
                MsgBox(filename & " EEprom xml Missing", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
                Return False
            End If
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If

            gstrcommandname = "Read " & filename & " EE Map"
            'UpdateUpdate(progress)
            dsCommParams.ReadXml(Application.StartupPath & "\Aclara_TestParameters\EEMaps\" & filename & ".xml")
            tmpHash = dsCommParams.Tables("HashTable").Rows(0).Item(0)
            readlen = dsCommParams.Tables("HashTable").Rows(0).Item(1)
            Dim tblcnt As Integer = dsCommParams.Tables("Table").Rows.Count
            Dim EETable(tblcnt - 1, 3) As String
            For i As Integer = 0 To tblcnt - 1
                EETable(i, 0) = dsCommParams.Tables("Table").Rows(i).Item("arrloc") '.................... loc
                EETable(i, 1) = dsCommParams.Tables("Table").Rows(i).Item("arrlength") '................. len
                EETable(i, 2) = dsCommParams.Tables("Table").Rows(i).Item("arrregID") '.................. Reg
                EETable(i, 3) = dsCommParams.Tables("Table").Rows(i).Item("arrdesc") '................... Des
                If EETable(i, 2).Length > 1 Then
                    EETable(i, 3) = EETable(i, 3) & " - Reg " & EETable(i, 2)
                End If
            Next
            comm.inbLen = "2"
            comm.opcode = "5F"
            gTDelay = 10000
            comm.outbmsg = readlen
            Await comm.EEData(comm.outbmsg)
            stractual = RemoveInbHeader(stractual)
            If InStr(stractual, ChrW(1)) > 0 Then
                stractual = Replace(stractual, ChrW(1), "")
            End If
            stractual = Mid(stractual, 9)
            If stractual = "" Then
                Return False
            End If
            '.................................Parse Response

            log.Clear
            log.Append(filename & " EEProm Dump" & vbCrLf)
            log.Append(Now() & vbCrLf)
            log.Append("E-Square Size = " & CInt("&H" & readlen) & " Bytes" & vbCrLf)
            log.Append("0xLoc  Len   Description                                                  Value Read" & vbCrLf)
            Dim tmp(tblcnt) As String
            For i As Integer = 0 To tblcnt - 1
                log.Append(
                EETable(i, 0) & "   " & StrDup(3 - Len(EETable(i, 1)), "0") & EETable(i, 1) & "   " &
                EETable(i, 3) & StrDup(60 - Len(EETable(i, 3)), ".") & " " & Mid(stractual, 1, CInt(EETable(i, 1)) * 2) & vbCrLf)
                stractual = Mid(stractual, 2 * CInt(EETable(i, 1)) + 1)
            Next
            '................................ Print EEProm
            Dim eeprom As String = ""
            'Dim eeprom As String = Application.StartupPath & "\Aclara_TestParameters\EEMaps\" & filename & ".tst"
            If Varparams(8) = "" Then
                eeprom = "C:\Pack\Aclara_PLS\" & filename & "_dump.tst"
            Else
                eeprom = Varparams(8) & "\" & filename & "_dump.tst"
            End If

            Dim fs As New FileStream(eeprom, FileMode.Create, FileAccess.Write)
            Dim sa As New StreamWriter(fs)
            sa.WriteLine(log)
            sa.Close()
            dsCommParams.Dispose()

            'System.Diagnostics.Process.Start(eeprom)

            Return True

        Catch ex As Exception
            MsgBox(gstrcommandname & " - " & gstrcommandname, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            TestFail = True
            progress.Close()
        End Try
    End Function
    Public Function GetTestResults() As Boolean
        'Dim xml_results As New ACLARA_INTEGRATION.XML_Results
        'Dim DBA As String = "Aclara_TestResults"
        'Dim Table As String = "Integration_TestResults"
        Dim Query As String = ""
        Dim VerifySQL As Integer = 0
        Dim SQLWritten As Boolean = False
        Dim Archived As String = "0"
        'Dim dcnDB As Object = ""
        'Dim rsData As Object = ""

        If gblnbadcommunication = True Then TestFail = True
        If gblnFAIL_EEPROM = True Then TestFail = True
        '.......................................ERROR Code Assign
        If TestFail = True Then
            If gblnbadcommunication = True Then '.........COMM FAILURE
                If errcode = 0 Then errcode = 4
            End If
            If gblnFAIL_EEPROM = True Then
                errcode = 1 '.............................EE Read Failure
                If LoadEE1 = True Then errcode = 9 '..................FCT Error
            End If

            If TestCommand = "" Then
                'If TWACSOpcode = "3F" Then
                TestCommand = "EE Read Failure"
                'Else
                TestCommand = Trim(gstrcommandname & " A=" & stractual & "/" & "E=" & strexpected)
                'End If
                If Len(TestCommand) > 40 Then TestCommand = gstrcommandname
            End If
            '...........................................If Errcode = 0 then errorcode NOT assigned yet
            If errcode = 0 Then
                'If TWACSOpcode = "3E" Then errcode = 1 '.............Read Failure
                'If TWACSOpcode = "3F" Then errcode = 7 '.............Write Failure
                'If TWACSOpcode = "DC" Then errcode = 7 '.............Write Failure
                If InbStr = "1338" Then errcode = 6 '..................Meter Comm Failure
            End If
        End If
        '.......................................END - ERROR Code Assign
        'OLD......EEdump = "ErrCode: " & errcode & ErrString
        'If ErrReportLong = True Then
        '    EEdump = "ErrCode: " & errcode & ";" & ErrString
        'Else
        EEdump = "ErrCode: " & errcode
        'End If
        TestCommand = Replace(TestCommand, "'", "")            '...Remove
        TestCommand = Replace(TestCommand, ",", " ")            '...Remove commas
        TestCommand = Replace(TestCommand, ";", " ")            '...Remove semicommas
        TestCommand = Replace(TestCommand, ":", " ")            '...Remove colons
        TestCommand = Replace(TestCommand, "  ", " ")            '...Remove spaces
        TestCommand = Replace(TestCommand, "   ", " ")            '...Remove spaces

        TestCommand = Replace(TestCommand, "<", " ")            '...Remove less than
        TestCommand = Replace(TestCommand, "&", " ")            '...Remove less than
        TestCommand = Replace(TestCommand, "%", " ")            '...Remove less than
        TestCommand = Replace(TestCommand, ">", " ")            '...Remove less than
        TestCommand = Replace(TestCommand, """", " ")            '...Remove less than
        '...........Write results to MONTHLY xml file
        'If writexml = True Then
        'xml_results.WriteResults()
        'End If

        'If writesql = False Then
        '    Exit Function
        'End If

RetrySQLInsert:
        Try
            Query = "INSERT INTO " & Varparams(17) & ".[dbo].[Integration_TestResults]" &
               "(Product, Result, [DateTime], TWACSID, MeterID, Customer, CustomerID, TypeModel, EEMap, RCE_FW, RF_FW, RefID, Command, DLL_Ver, TestPC, EEdump, Archived)" &
               " VALUES ('" & Varparams(5) & "', '" & TestResult & "', '" & Now() & "', '" & DecSerialRead & "', '" & Varparams(0) & "', '" & gCustomer & "', '" & Varparams(4) &
               "', '" & Varparams(6) & "', '" & gEEMap & "', '" & Varparams(20) & "', '" & gEMTRFW & "', '" & gintRef & "', '" & TestCommand & "', '" & lgmodule.DLL & "', '" & System.Environment.MachineName & "', '" & EEdump _
               & "', '" & Archived & "')" ''
            CheckSQLState()
            dcnDB.Open(Varparams(16))
            rsData = dcnDB.Execute(Query)
        Catch ex As Exception
            If AutoRetry > 0 Then '... autoretry SQL indert ONLY ONCE
                AutoRetry = AutoRetry - 1
                System.Threading.Thread.Sleep(1 * 1000)
                GoTo RetrySQLInsert
            End If
            SQLWriteFail = SQLWriteFail & 1
        End Try

        Try
SKIPRETRY:
            If gblnFAIL_EEPROM = True AndAlso TestFail = True Then      'Setup for WECO if EE Fail > 40 Chars
                If Len(TestCommand) > 40 Then TestCommand = "EE " & EECommand
            End If

        Catch ex As Exception
            MsgBox("XML Test Result Insertion Error:" & vbLf & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Private Function VerifySQL_ResultWritten() As Boolean
        Dim Query As String
        Dim Count As Integer
        Dim Table As String = "Integration_TestResults"
        VerifySQL_ResultWritten = False

        Try
            Query = "Select COUNT(TWACSID) From " & Table & " With (NoLock) Where DateTime = '" & DateTime & "' AND TWACSID = '" & DecSerialRead & "'"
            Dim cn As New ADODB.Connection
            cn.ConnectionString = gstrCnn_TestResults
            cn.Open()
            Dim rs As New ADODB.Recordset
            rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient
            rs.CursorType = ADODB.CursorTypeEnum.adOpenStatic
            rs.LockType = ADODB.LockTypeEnum.adLockBatchOptimistic
            rs.Open(Query, cn)
            Count = Val(rs.Fields(0).Value)
            If Count > 0 Then
                VerifySQL_ResultWritten = True
            Else
                VerifySQL_ResultWritten = False
                SQLWriteFail = SQLWriteFail & 1
            End If

            rs.ActiveConnection = Nothing
            cn.Close()
        Catch ex As Exception
            'MsgBox("Unable to Verify SQL Result:" & vbLf & ex.Message, MsgBoxStyle.OKOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
    Private Function GetFormID() As String
        GetFormID = Nothing
        Dim tmpdec As String = Varparams(6)
        tmpdec = Convert.ToInt32(Mid(Varparams(6), 1, 2), 16)
        Select Case Varparams(5)
            Case "UMTR-F-RXe_4.xx", "UMTR-F-AXe_4.xx"
                If tmpdec > 100 Then
                    Return Hex(tmpdec - 120)
                Else
                    Return Hex(tmpdec - 20)
                End If
            Case "UMTR-F-AL_4.xx", "UMTR-F-FX_4.xx", "UMT-C-FXP_6.xx"
                If tmpdec > 100 Then
                    Return Hex(tmpdec - 100)
                Else
                    Return Hex(tmpdec)
                End If
        End Select
    End Function
    Public Function GetCustomerValues(ByRef progress As Progress) As Boolean
        TestFail = True
        Dim Query As String = ""
        Dim i As Integer = 0
        Dim formID As String = ""
        Dim columns As String()
        gstrcommandname = "Reading " & Varparams(5) & " Customer Values"
        UpdateUpdate(progress)
        Try
            '.......................................UMTR-RX
            'Aclara_CustomerSpecific.dbo.UMT_RF_CustomerValues
            formID = GetFormID()
            If formID.Length < 2 Then formID = "0" & formID
            Select Case Varparams(5)
'.......................................UMTR-AXe & RXe
                Case "UMTR-F-AXe_4.xx", "UMTR-F-RXe_4.xx"
                    Query = "SELECT Drawing,Customer,Op220MVC" & formID & ",Reg1850MVC" & formID & ",Reg1851MVC" & formID & ",Reg1852MVC" & formID & ",Reg1853MVC" & formID & _
                    ",Reg290,Reg291,Reg292,Reg293,Reg294,Reg295,Reg296,Reg297,Reg298,Reg299,Reg300,Reg301,Reg302,Reg303,Reg304,Reg305,Reg1800,Reg1801,Reg1802,Reg1803" & _
                    ",Reg1804,Reg1805,Reg1806,Reg1807,Reg1808,Reg1809,Reg1810,Reg1811,Reg1812,Reg1813,Reg1814,Reg1815,Reg703,Reg192 FROM " & Varparams(14) & _
                    ".dbo.UMT_RX_CustomerValues WITH (NOLOCK) WHERE CustomerID = '" & Varparams(4) & "'" & " AND ProductFamily = " & "'" & Varparams(5) & "'"
                    ReDim Preserve CustVals(1, 43)
                    columns = {"Drawing", "Customer", "Reg220", "Reg1850", "Reg1851", "Reg1852", "Reg1853", "Reg290", "Reg291", "Reg292", "Reg293", "Reg294", "Reg295", "Reg296", "Reg297", "Reg298", "Reg299", "Reg300", "Reg301", "Reg302", "Reg303", "Reg304", "Reg305", "Reg1800", "Reg1801", "Reg1802", "Reg1803", "Reg1804", "Reg1805", "Reg1806", "Reg1807", "Reg1808", "Reg1809", "Reg1810", "Reg1811", "Reg1812", "Reg1813", "Reg1814", "Reg1815", "Reg702", "Reg192", "Reg51", "Reg90", "Reg24"}
'.......................................UMTR-F-AL -F-AX
                Case "UMTR-F-AL_4.xx", "UMTR-F-FX_4.xx"
                    Query = "SELECT CreateDate,Customer,Op220MVC" & formID & ",Reg1850MVC" & formID & ",Reg1851MVC" & formID & _
                    ",Reg290,Reg291,Reg292,Reg293,Reg294,Reg295,Reg296,Reg297,Reg298,Reg299,Reg300,Reg301,Reg302,Reg303,Reg304,Reg305,Reg1800,Reg1801,Reg1802,Reg1803" & _
                    ",Reg1804,Reg1805,Reg1806,Reg1807,Reg1808,Reg1809,Reg1810,Reg1811,Reg1812,Reg1813,Reg1814,Reg1815,Reg703,Reg192,EMTR FROM " & Varparams(14) & _
                    ".dbo.UMT_RF_CustomerValues WITH (NOLOCK) WHERE CustomerID = '" & Varparams(4) & "'" & " AND ProductFamily = " & "'" & Varparams(5) & "'"
                    ReDim Preserve CustVals(1, 43)
                    columns = {"CreateDate", "Customer", "Reg220", "Reg1850", "Reg1851", "Reg290", "Reg291", "Reg292", "Reg293", "Reg294", "Reg295", "Reg296", "Reg297", "Reg298", "Reg299", "Reg300", "Reg301", "Reg302", "Reg303", "Reg304", "Reg305", "Reg1800", "Reg1801", "Reg1802", "Reg1803", "Reg1804", "Reg1805", "Reg1806", "Reg1807", "Reg1808", "Reg1809", "Reg1810", "Reg1811", "Reg1812", "Reg1813", "Reg1814", "Reg1815", "Reg703", "Reg192", "RegEMTR", "Reg51", "Reg90", "Reg24"}
'.......................................UMT-C-FXP
                Case "UMT-C-FXP_6.xx"
                    Query = "SELECT Drawing,Customer,Op220MVC" & formID & ",Reg1850MVC" & formID & ",Reg1851MVC" & formID & ",Reg1852MVC" & formID & ",Reg1853MVC" & formID & _
                    ",Reg290,Reg291,Reg292,Reg293,Reg294,Reg295,Reg296,Reg297,Reg298,Reg299,Reg300,Reg301,Reg302,Reg303,Reg304,Reg305,Reg1800,Reg1801,Reg1802,Reg1803" & _
                    ",Reg1804,Reg1805,Reg1806,Reg1807,Reg1808,Reg1809,Reg1810,Reg1811,Reg1812,Reg1813,Reg1814,Reg1815,Reg2269,Reg2270,Reg2271,Reg2272,Reg2273,Reg2274" & _
                    ",Reg2275,Reg2276,Reg2277,Reg2278,Reg2279,Reg2280,Reg2281,Reg2282,Reg2283,Reg2284,Reg2461,Reg2462,Reg2463,Reg2464,Reg2465,Reg2466,Reg2467,Reg2468" & _
                    ",Reg2469,Reg2470,Reg2471,Reg2472,Reg2473,Reg2474,Reg2475,Reg2476,EMTR FROM " & Varparams(14) & ".dbo.UMT_C_FXP_CustomerValues " & _
                    " WITH (NOLOCK) WHERE CustomerID = '" & Varparams(4) & "'" & " AND ProductFamily = " & "'" & Varparams(5) & "'"
                    ReDim Preserve CustVals(1, 75)
                    columns = {"Drawing", "Customer", "Reg220", "Reg1850", "Reg1851", "Reg1852", "Reg1853", "Reg290", "Reg291", "Reg292", "Reg293", "Reg294", "Reg295", "Reg296", "Reg297", "Reg298", "Reg299", "Reg300", "Reg301", "Reg302", "Reg303", "Reg304", "Reg305", "Reg1800", "Reg1801", "Reg1802", "Reg1803", "Reg1804", "Reg1805", "Reg1806", "Reg1807", "Reg1808", "Reg1809", "Reg1810", "Reg1811", "Reg1812", "Reg1813", "Reg1814", "Reg1815", "Reg2269", "Reg2270", "Reg2271", "Reg2272", "Reg2273", "Reg2274", "Reg2275", "Reg2276", "Reg2277", "Reg2278", "Reg2279", "Reg2280", "Reg2281", "Reg2282", "Reg2283", "Reg2284", "Reg2461", "Reg2462", "Reg2463", "Reg2464", "Reg2465", "Reg2466", "Reg2467", "Reg2468", "Reg2469", "Reg2470", "Reg2471", "Reg2472", "Reg2473", "Reg2474", "Reg2475", "Reg2476", "RegEMTR", "Reg51", "Reg90", "Reg24"}
            End Select
            CheckSQLState()
            dcnDB.Open(Varparams(10))
            rsData = dcnDB.Execute(Query)
            If Not rsData.EOF Then
                For i = 0 To rsData.Fields.Count - 1
                    CustVals(1, i) = RTrim(rsData.Fields(i).Value)
                Next
                For i = 0 To columns.GetUpperBound(0)
                    CustVals(0, i) = columns(i)
                    If columns(i) = "Reg24" Then CustVals(1, i) = gReg24
                    If columns(i) = "Reg90" Then CustVals(1, i) = gReg90
                    If columns(i) = "Reg51" Then CustVals(1, i) = gProductType & Mid(Varparams(6), 1, 2)
                    If columns(i) = "Customer" Then gCustomer = CustVals(1, i)
                Next
                rsData.Close()
                rsData = Nothing
                TestFail = False
                Return True
            Else
                If i < 10 Then
                    TestFail = True
                    gstrcommandname = "No Customer Record for ID " & Varparams(4) & " - " & Varparams(5)
                    TestCommand = gstrcommandname
                    UpdateFail(progress)
                    errcode = 2
                    Return False
                End If
            End If
        Catch ex As Exception
            UpdateFail(progress)
            errcode = 2
            gstrcommandname = "SQL Error CustID - " & gCustomerId
            Return False
        End Try
    End Function
    Public Function SwapMapsbyFW(ByRef progress As Progress) As Boolean
        Try
            TestFail = True
            'gstrcommandname = "Verifying MeterFW for Mapping"
            UpdateUpdate(progress)
            Dim meterfw As String = Varparams(3)
            Dim dsEEParams As New System.Data.DataSet
            dsEEParams.ReadXml(Application.StartupPath & "\ApplicationValues\MappedFWValues.xml")
            Dim i, y, j, fwcnt, mapcnt As Integer
            fwcnt = dsEEParams.Tables("FW").Rows.Count
            mapcnt = CInt(dsEEParams.Tables("Map534").Rows.Count)
            Dim fwmap(fwcnt, mapcnt) As String
            Dim map534(mapcnt) As String
            Dim map537(mapcnt) As String
            Dim map571(mapcnt) As String
            For i = 0 To fwcnt - 1
                fwmap(i, 0) = dsEEParams.Tables("FW").Rows(i).Item(1).ToString
                For y = 1 To mapcnt
                    If fwmap(i, 0) = "0534" Then
                        map534(y) = dsEEParams.Tables("Map534").Rows(y - 1).Item(0).ToString
                    ElseIf fwmap(i, 0) = "0537" Then
                        map537(y) = dsEEParams.Tables("Map537").Rows(y - 1).Item(0).ToString
                    ElseIf fwmap(i, 0) = "0571" Then
                        map571(y) = dsEEParams.Tables("Map571").Rows(y - 1).Item(0).ToString
                    Else
                        gstrcommandname = "Unsupported MeterFW=" & meterfw
                        Return False '............. unknown dirmward
                    End If
                Next
            Next
            '................. Now Compare Map Script (CustVals(1, i)) to fwmap Values using the Meter Firmware (Varparams(3)
            '..... ............Run each Map Script Value down fwmapping list - choose value by meter firmware version

            i = 0
            While CustVals(0, i) <> "Reg290" '......................Find "Reg290" for the start index
                i = i + 1
            End While

            j = i + 32 '............................................Run thru all 32 map values
            Do While i < j
                For y = 0 To mapcnt
                    If CustVals(1, i) = map534(y) Then '......................When map value = mapfw verify by meterfw - swap if needed
                        If meterfw = "0537" Then CustVals(1, i) = map537(y)
                        If meterfw = "0571" Then CustVals(1, i) = map571(y)
                    ElseIf CustVals(1, i) = map537(y) Then
                        If meterfw = "0534" Then CustVals(1, i) = map534(y)
                        If meterfw = "0571" Then CustVals(1, i) = map571(y)
                    ElseIf CustVals(1, i) = map571(y) Then
                        If meterfw = "0534" Then CustVals(1, i) = map534(y)
                        If meterfw = "0537" Then CustVals(1, i) = map537(y)
                    End If
                Next
                i += 1
            Loop

            TestFail = False
            Return True

        Catch ex As Exception
            gstrcommandname = "Error in SwapMapsbyFW"
            Return False
        End Try
    End Function
    Private Function SwapComands(ByVal opdata As String) As String
        SwapComands = ""
        Dim odata As StringBuilder = New StringBuilder
        Try
            Dim tmp As String() = opdata.Split(";") 'Run all "Regxxx" against CustVals
            For i As Integer = 0 To tmp.GetUpperBound(0) - 1
                If InStr(tmp(i), "Reg") > 0 Then
                    For j As Integer = 0 To CustVals.GetUpperBound(1)
                        If tmp(i) = CustVals(0, j) Then
                            tmp(i) = CustVals(1, j)
                            Exit For
                        End If
                    Next
                End If
                odata.Append(tmp(i))
            Next
            Return odata.ToString
        Catch ex As Exception
        End Try
    End Function
    Private Function Swap(ByVal exp As String)
        Swap = ""
        Try
            Dim tmp2 As StringBuilder = New StringBuilder
            Dim tmp As String() = exp.Split(New Char() {";"})
            For i As Integer = 0 To tmp.GetUpperBound(0)
                Select Case (tmp(i))
                    Case "Reg24"
                        tmp(i) = HexSerialRead
                End Select
                tmp2.Append(tmp(i))
            Next
            Return tmp2.ToString
        Catch ex As Exception
        End Try
    End Function
    Private Function Invalid_Character(ByVal actresponse As String, ByRef Progress As Object) As Boolean
        Try
            For Each Character As Char In actresponse
                If Char.IsNumber(Character) Then
                    Return False
                ElseIf Char.IsLetter(Character) Then
                    Return False
                ElseIf Not Char.IsWhiteSpace(Character) Then
                    Return True
                End If
            Next
        Catch ex As Exception
        End Try
    End Function
    Private Sub WriteEEResults()
        PrintLine(1, gstrcommandname & "   Interval Data Channel 1 Reg 1850.................." & stractual)
    End Sub
    Public Sub BadCommunicationRoutine(ByRef Progress As Progress)
        BadCommunicationRoutineAttempt = True
        Dim TstResult As String = ""
        DLLver = lgmodule.DLL
        'If rework = True Then DLLver = DLLver & " Rework"
        If gPackPath = "" Then gPackPath = "C:\Pack\Aclara_PLS\" 'Default
        If Not System.IO.Directory.Exists(gPackPath) Then System.IO.Directory.CreateDirectory(gPackPath)
        If Not System.IO.File.Exists(gPackPath & "Integration.DAT") Then
            System.IO.File.Exists(gPackPath & "Integration.DAT")
        End If

        Try
            If TestFail = True OrElse gblnFAIL_EEPROM OrElse gblnbadcommunication = True Then
                TestFail = True
                TestResult = "Fail"
                'If gstrfailmessage = "" Then gstrfailmessage = "FAIL: " & gstrcommandname
                Progress.lblcustomername.Items.Add("***** TEST FAILURE *****")
                Progress.lblcustomername.SelectedIndex = Progress.lblcustomername.Items.Count - 1
                Progress.Update()
                'sleep(5)
            Else
                TestFail = False
                TestResult = "Pass"
            End If

            If gEMTRPresent = True Then
                If Len(gcalibration) < 4 Then gcalibration = New String("0", 4 - (Len(gcalibration))) & gcalibration
                If Len(striedRSSI) < 2 Then striedRSSI = New String("0", 2 - (Len(striedRSSI))) & striedRSSI
            Else
                gcalibration = "NA"
                striedRSSI = "NA"
            End If
            If FCTTest = True Then gProduct = "FCT_Test"
            If gProduct = "" Then gProduct = ProductFamily '   Product        =    gProduct
            If TestResult = "" Then TestResult = "Fail" '         Result        =    TestResult
            If DateTime = "" Then DateTime = "" '             DateTime    =    DateTime
            If DecSerialRead = "" Then DecSerialRead = "NA"
            'If DecSerialScanned = "" Then DecSerialScanned = "NOT SCANNED"
            If gMeterSerialNumber = "" Then gMeterSerialNumber = "NA"
            If gCustomer = "" Then gCustomer = "NA" '  Customer    =    CustomerInfo
            If gCustomerId = "" Then gCustomerId = "NA" '    CustomerID    =    gCustomerId
            If gReg51 = "" Then gReg51 = "NA" '              TypeModel    =    TypeModel
            If gEEMap = "" Then gEEMap = "NA" '              EEMap       =   gEEMap
            If gReg48 = "" Then gReg48 = "NA" '              RCE_FW        =    gFWver  gReg48
            If gEMTRFW = "" Then gEMTRFW = "NA" '              RF_FW        =    emtrfw  gReg1607
            'If gintRef = "" Then gintRef = "NA" '            RefID        =    gintRef
            If DLLver = "" Then DLLver = "NA" '              DLL_Ver     =    DLLver
            'If TestPC = "" Then TestPC = "NA" '              TestPC        =    TestPC
            If Drawing = "" Then Drawing = "NA" '
            'If gCommandParameters = "" Then gCommandParameters = "NA"
            If EEdump = "" Then EEdump = "NA" '              EEdump        =    EEdump
            'EEdump = ErrString
            If Command = "" Then Command = "NA"

WriteData:
            FileOpen(1, gPackPath & "Integration.DAT", OpenMode.Append)
            PrintLine(1, TstResult)
            FileClose(1)
            '..................................CLOSE Integration.DAT
            If TestFail = True Then '.........TEST FAILED - WRITE TO ERROR FILE
                '........................Create File
                'If Not File.Exists(gPackPath & gProduct & "-" & DecSerialRead & ".ERR") Then File.Create(gPackPath & gProduct & "-" & DecSerialRead & ".ERR")
                '........................Write To File
                FileOpen(1, gPackPath & gProduct & "-" & DecSerialRead & ".ERR", OpenMode.Append)
                PrintLine(1, "")
                'VB.Format(Now, "MM/dd/yyyy-hh:mm:ss")
                If gEMTRPresent = False Then
                    PrintLine(1, RegDatRecvd & " Ref:" & gintRef & " - " & gstrcommandname & vbLf _
                     & "TWACS ID:" & DecSerialRead & "; Meter ID:" & gMeterSerialNumber & vbLf _
                     & "CustomerID:" & gCustomerId & "; Customer:" & gCustomer & "; ScriptCreateDate:" & Drawing & vbLf _
                     & "Product:" & gProduct & "; Type/Model:" & gReg51 & "; & RCE F/W:" & gReg48 & vbLf _
                     & "TestDate: " & DateTime & vbLf _
                     & "EEMap:" & gEEMap & "; DLLver:" & DLLver & "; Test PC:" & System.Environment.MachineName)
                Else
                    PrintLine(1, RegDatRecvd & " Ref:" & gintRef & " - " & gstrcommandname & vbLf _
                     & "TWACS ID:" & DecSerialRead & "; Meter ID:" & gMeterSerialNumber & vbLf _
                     & "CustomerID:" & gCustomerId & "; Customer:" & gCustomer & "; ScriptCreateDate:" & Drawing & vbLf _
                     & "Product:" & gProduct & "; Type/Model:" & gReg51 & "; & RCE F/W:" & gReg48 & vbLf _
                     & "TestDate: " & DateTime & vbLf _
                     & "EEMap:" & gEEMap & "; DLLver:" & DLLver & "; Test PC:" & System.Environment.MachineName & vbLf _
                     & "EMTR Pass:" & gEMTRPass & "; EMTR F/W: " & gEMTRFW & "; Calib:" & gcalibration & "; IED RSSI:" & striedRSSI)
                End If
                PrintLine(1, "")
                PrintLine(1, RegDatRecvd)
                If gblnbadcommunication = True Then
                    PrintLine(1, "*BAD COMMUNICATION*")
                End If
                System.Threading.Thread.Sleep(1 * 100)
            End If

            FileClose(1)
            RegDatRecvd = ""
            iedtest = False     'reset emtr test indicator
            blnnotdelay = True
Done:
        Catch ex As Exception
            If Err.Number = 0 Then Exit Sub
            gstrcommandname = Err.Number & " " & Err.Description
            MsgBox(ex.Message & "Error File Write error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Sub
    Public Sub UpdateTWACS(ByRef progress As Progress)
        progress.lstTWACS.Items.Clear()
        progress.lstTWACS.Items.Add("Outbnd:" & Chr(9) & gsOutbound_STR)
        progress.lstTWACS.Items.Add("Inbnd:" & Chr(9) & gsInbound_STR)
        progress.lstTWACS.Items.Add("Act:" & Chr(9) & stractual)
        progress.lstTWACS.Items.Add("Exp:" & Chr(9) & strexpected)
    End Sub
    Public Sub UpdateUpdate(ByRef progress As Progress)
        If gintRef > progress.ProgressBar1.Maximum Then gintRef = gintRef - 1
        progress.ProgressBar1.Value = gintRef
        progress.lblcustomername.Items.Add("Ref: " & gintRef & " - " & gstrcommandname & " - Attempt # " & icnt + 1)
        progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
        progress.Update()
    End Sub
    Public Sub UpdateOnly(ByRef progress As Progress)
        progress.lblcustomername.Items.Add(gstrcommandname)
        progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
        progress.Update()
    End Sub
    Public Sub UpdatePass(ByRef progress As Progress)
        TestFail = False
        logtest(progress)
        progress.lblcustomername.Items.Add("PASS")
        progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
        progress.Update()
        UpdateTWACS(progress)
        ErrString = ""
        stractual = ""
        progress.lblcustomername.BackColor = progress.BackColor.White
        progress.Update()
    End Sub
    Public Sub UpdateFail(ByRef progress As Progress)
        TestFail = True
        logtest(progress)
        progress.lblcustomername.BackColor = progress.BackColor.Yellow
        progress.lblcustomername.Items.Add("FAIL")
        progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
        progress.Update()
        UpdateTWACS(progress)
        ErrString = gsOutbound_STR & ";" & gsInbound_STR & ";" & stractual & ";" & strexpected
        If ErrString.Length > 480 Then ErrString = Mid(ErrString, 1, 480)
    End Sub
    Public Sub logtest(ByRef progress As Progress)
        Dim time As DateTime = Now()
        Dim pad As String = StrDup(8 - Len(gintRef.ToString), ".")
        Try
            log.Append(
                "Step " & gintRef & StrDup(8 - Len(gintRef.ToString), ".") & time & vbCrLf &
                "Desc........." & gstrcommandname & vbCrLf &
                "Outbound....." & gsOutbound_STR & vbCrLf &
                "Inbound......" & gInbFullAnswer & vbCrLf &
                "Act.........." & stractual & vbCrLf &
                "Exp.........." & strexpected & vbCrLf)
            gsOutbound_STR = ""
            gInbFullAnswer = ""
            If TestFail = True Then
                log.Append("Result.......FAIL: " & gstrcommandname & vbCrLf)
            Else
                log.Append("Result.......PASS" & vbCrLf)
            End If
            log.Append("-------------------------------------------" & vbCrLf)
        Catch ex As Exception
            gstrcommandname = "logtest.tst error - nonfatal"
        End Try
    End Sub
    Public Sub UpdateFailTest(ByRef progress As Progress)
        TestFail = True
        progress.lblcustomername.BackColor = progress.BackColor.Yellow
        progress.lblcustomername.Items.Add("FAIL TEST")
        progress.lblcustomername.SelectedIndex = progress.lblcustomername.Items.Count - 1
        progress.Update()
        UpdateTWACS(progress)
        ErrString = gsOutbound_STR & ";" & gsInbound_STR & ";" & stractual & ";" & strexpected
        If ErrString.Length > 480 Then ErrString = Mid(ErrString, 1, 480)
        lgmodule.CloseProgress(progress)
    End Sub
    Public Sub CreateXMLFiles(ByVal file_name)
        Try
            Dim fs As New FileStream(file_name, FileMode.Create, FileAccess.Write)
            Dim sa As New StreamWriter(fs)
            sa.WriteLine("<?xml version=" & Chr(34) & "1.0" & Chr(34) & " standalone=" & Chr(34) & "yes" & Chr(34) & "?>")
            sa.WriteLine("<NewDataSet>")
            sa.WriteLine("<HashTable>")
            sa.WriteLine("<HashValue>HASHVAL</HashValue>")
            sa.WriteLine("<TestPC>" & System.Environment.MachineName & "</TestPC>")
            sa.WriteLine("</HashTable>")
            sa.WriteLine("</NewDataSet>")
            sa.Close()
        Catch ex As Exception
            MsgBox(ex.Message, "Create XML File Error - Contact Aclara", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Sub
    Public Sub GetResults()
        Try
            Dim doc As New XmlDocument()
            Dim xmldoc As String = Application.StartupPath & "\ApplicationValues\CommValues.xml"
            doc.Load(xmldoc)
            Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/CommValues")
            For Each node As XmlNode In nodes
                TstPass = node.SelectSingleNode("Pass").InnerText
                TstFail = node.SelectSingleNode("Fail").InnerText
                SQLWriteFail = node.SelectSingleNode("WriteError").InnerText
            Next
        Catch ex As Exception
            MsgBox("GetResults.xml Read Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Sub
    Public Sub SetResults()
        If TestFail = False Then
            Convert.ToInt32(TstPass)
            TstPass = TstPass & 1
            TstPass = CStr(TstPass)
        Else
            Convert.ToInt32(TstFail)
            TstFail = TstFail & 1
            TstFail = CStr(TstFail)
        End If
        Try
            Dim file_name As String = Application.StartupPath & "\Aclara_TestParameters\Results.ini"
            Dim stream_writer As New IO.StreamWriter(file_name) ', False)
            Try
                stream_writer.Write("Pass=" & TstPass & vbLf)
                stream_writer.Write("Fail=" & TstFail & vbLf)
                stream_writer.Write("SQLWriteError=" & SQLWriteFail)
            Finally
                stream_writer.Close()
            End Try

        Catch exc As Exception ' Report all errors.
            MsgBox(exc.Message, MsgBoxStyle.Exclamation, "cmtpassword.ini Error")
        End Try
    End Sub
    Public Sub Results(ByVal Total, ByVal Pass, ByVal Fail)
        'Dim oWrite As System.IO.StreamWriter
        'Dim objStreamWriter As StreamWriter
        Try
            Dim file_name As String = Application.StartupPath & "\Aclara_TestParameters\Results.ini"
            Dim stream_writer As New IO.StreamWriter(file_name) ', False)
            Try
                stream_writer.Write("ZeroPassword=True")
            Finally
                stream_writer.Close()
            End Try
            'lblcmt.Text = "Password = Zero"
        Catch exc As Exception ' Report all errors.
            MsgBox(exc.Message, MsgBoxStyle.Exclamation, "Results.ini Error")
        End Try
    End Sub
    Public Function WriteBaudRate(ByVal Baud As String) As Object
        WriteBaudRate = ""
        Try
            Dim file_name As String = Application.StartupPath & "\ApplicationValues\CurrentBaudRate.ini"
            Dim stream_writer As New IO.StreamWriter(file_name)
            Try
                stream_writer.Write(Baud)
            Finally
                stream_writer.Close()
            End Try
        Catch exc As Exception
            MsgBox(exc.Message, MsgBoxStyle.Exclamation, "Write BaudRate.ini Error")
        End Try
    End Function
    Public Function ReadBaudRate() As String
        Dim file_name As String = Application.StartupPath & "\ApplicationValues\CurrentBaudRate.ini"
        Dim stream_reader As New IO.StreamReader(file_name)
        Try
            ReadBaudRate = stream_reader.ReadLine()
            stream_reader.Close()
        Catch exc As Exception
            MsgBox(exc.Message, MsgBoxStyle.Exclamation, "Read BaudRate.ini Error")
        End Try
    End Function
    Public Function ReadSQL(ByVal Progress, ByVal ProductFamily) As Boolean
        Dim strTemp1 As String
        Dim strTemp As String
        Dim i As Long
        Dim y As Integer
        Dim strHash As String
        Dim intMaxRef As Integer = 100
        Dim file_name As String
        Dim nameoffile As String
        Dim dsCommParams As New System.Data.DataSet

        If GenIntCollection.Count > 0 Then
            For y = 1 To GenIntCollection.Count
                GenIntCollection.Remove(1)
            Next
        End If
        file_name = Application.StartupPath & "\Aclara_TestParameters\EEMAPS\" & ProductFamily & "_SQL.xml"
        dsCommParams.ReadXml(file_name)
        strTemp = dsCommParams.Tables("HashTable").Rows(0).Item(0)
        strTemp1 = ""

        For i = 0 To dsCommParams.Tables("Table").Rows.Count - 1
            For y = 0 To dsCommParams.Tables("Table").Columns.Count - 1
                'TODO: Use compound operator: &=
                strTemp1 = strTemp1 & dsCommParams.Tables("Table").Rows(i).Item(y)
            Next
        Next
        strHash = GenerateHash(strTemp1)
        'If gHash = False Then GoTo SkipHash
        'Compare stored vs generated hash values
        If strHash <> strTemp Then 'If fail then exit
            TestFail = True
            ReadSQL = False
            errcode = 2 'LG Process Error
            gstrcommandname = ""
            gstrcommandname = nameoffile & "-Hash Error"
            TestCommand = gstrcommandname
            MsgBox("Read SQL Structure File : " & ProductFamily & "_SQL.xml" & " - Hash Value doesn't match", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            If gShowHash = True Then
                gstrcommandname = ProductFamily & "_SQL.xml" & " hash value = " & strHash
            End If
            Exit Function
        End If

SkipHash:
        Try

            'intMaxRef = dsCommParams.Tables("Table").Rows.Count

            i = 0
            While (i <= intMaxRef)
                SQL(i) = dsCommParams.Tables("Table").Rows(0).Item("SQL" & i)
                strTemp = SQL(i)
                If SQL(i) = "END" Then
                    ReadSQL = True
                    dsCommParams.Dispose()
                    Exit Function
                End If
                'SQL(i) = AssignRegValues(Progress, SQL(i), "")    'than execute jump + maybe other commands
                i = i + 1
            End While

            ReadSQL = True
            dsCommParams.Dispose()
            Exit Function

        Catch ex As Exception
            TestFail = True
            ReadSQL = False
            errcode = 2     'LG Process Error
            gstrcommandname = ""
            gstrcommandname = "XML Error-" & nameoffile
            TestCommand = gstrcommandname
            MsgBox(ProductFamily & "_SQL.xml Not Read" & vbLf & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
Private Sub GetPortValues()
Try
    Dim doc As New XmlDocument()
    Dim xmldoc As String = Application.StartupPath & "\ApplicationValues\PortDelay.xml"
    doc.Load(xmldoc)
    Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/PortValues")
    For Each node As XmlNode In nodes
        gTDelay = node.SelectSingleNode("gTaskDelay").InnerText
    Next
Catch ex As Exception
    MsgBox("Error Reading GetPortValues.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Sub
End Module
End Namespace
