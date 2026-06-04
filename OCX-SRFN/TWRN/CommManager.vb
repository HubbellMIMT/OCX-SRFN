Option Strict On
Imports System
Imports System.Text
Imports System.Drawing
Imports System.IO.Ports
Imports System.Windows.Forms
Imports System.Threading
Imports System.Threading.Tasks

Namespace TWRN
Public Delegate Sub DataWrittenEventHandler(ByVal msg As String)
Public Delegate Sub DataReceivedEventHandler(ByVal msg As String)
'*****************************************************************************************
'               ACLARA TWACS TEST PROCESSING SOFTWARE
'*****************************************************************************************
Public Class CommManager
'    Public Delegate Sub DataWrittenEventHandler(ByVal msg As String)
'    Public Delegate Sub DataReceivedEventHandler(ByVal msg As String)
    Public Event DataWritten(ByVal msg As String)
    Public Event DataReceived(ByVal msg As String)
    Private WithEvents serial As New IO.Ports.SerialPort
    Private oLockObject as new Object
    Private isDone As Boolean
    Dim BufferSize As Integer
    Dim TotalByteLastRec As Integer
    Dim response As String = String.Empty
#Region "Manager Variables"
    Private _inbLen As String = String.Empty
    Private _baudRate As String = String.Empty
    Private _outbmsg As String = String.Empty
    Private _opcode As String = String.Empty
    Private _parity As String = String.Empty
    Private _stopBits As String = String.Empty
    Private _dataBits As String = String.Empty
    Private _portName As String = String.Empty
    Private _outbDesc As String = String.Empty
    Private _currentResponse As String = String.Empty
    Private _displayWindow As RichTextBox
    Private _msg As String
    Public ComPort As New SerialPort()
#End Region
#Region "Manager Properties"
    Public Property outbDesc() As String
        Get
            Return _outbDesc
        End Get
        Set(ByVal value As String)
            _outbDesc = value
        End Set
    End Property

    Public Property inbLen() As String
        Get
            Return _inbLen
        End Get
        Set(ByVal value As String)
            _inbLen = value
        End Set
    End Property
    Public Property opcode() As String
        Get
            Return _opcode
        End Get
        Set(ByVal value As String)
            _opcode = value
        End Set
    End Property
    Public Property outbmsg() As String
        Get
            Return _outbmsg
        End Get
        Set(ByVal value As String)
            _outbmsg = value
        End Set
    End Property
    Public Property BaudRate() As String
        Get
            Return _baudRate
        End Get
        Set(ByVal value As String)
            _baudRate = value
        End Set
    End Property
    Public Property Parity() As String
        Get
            Return _parity
        End Get
        Set(ByVal value As String)
            _parity = value
        End Set
    End Property

    Public Property StopBits() As String
        Get
            Return _stopBits
        End Get
        Set(ByVal value As String)
            _stopBits = value
        End Set
    End Property
    Public Property DataBits() As String
        Get
            Return _dataBits
        End Get
        Set(ByVal value As String)
            _dataBits = value
        End Set
    End Property
    Public Property PortName() As String
        Get
            Return _portName
        End Get
        Set(ByVal value As String)
            _portName = value
        End Set
    End Property
    Public Property Message() As String
        Get
            Return _msg
        End Get
        Set(ByVal value As String)
            _msg = value
        End Set
    End Property
    Private Property CurrentResponse As String
        Get
            Return _currentResponse
        End Get
        Set(value As String)
            _currentResponse = value
        End Set
    End Property
#End Region
#Region "Manager Constructors"
Public Sub New()
    _inbLen = String.Empty
    _outbmsg = String.Empty
    _opcode = String.Empty
    _baudRate = String.Empty
    _parity = String.Empty
    _stopBits = String.Empty
    _dataBits = String.Empty
    _portName = "COM1"
    AddHandler ComPort.DataReceived, AddressOf ReadData
End Sub
#End Region
#Region "Write Read Serial Port" '========================== Updated 11/8/23
Public Async Function WriteData(ByVal msg As String) As Task
Dim outtime As DateTime = Now()
Dim cnt As Integer = 0
Try
    CurrentResponse = String.Empty
    stractual = String.Empty
    InbStatus = String.Empty
    Dim intime As DateTime
    isDone = False 
    msg = BuildPacket(Opcode, msg)
    msg = ChrW(1) & msg & calcksm(msg) & ChrW(4)
    ComPort.PortName = PortName 
    ComPort.BaudRate = CInt(gBaudRate)
    If Not ComPort.IsOpen Then ComPort.Open()
    ComPort.Write(msg)
        
    do'...................................................... NEW
        Threading.Thread.Sleep(100)
	    SyncLock oLockObject
	    End SyncLock
        If isDone Then
            intime = Now()
            ClosePort() 
            stractual = CurrentResponse 
        End If
        cnt += 1
        If cnt > 50 Then
                ClosePort() 
                Exit Do 
        End If
    loop until isDone
    '...................................................... 
    Exit Function 
Catch ex As Exception
    gstrcommandname = "WriteData Error: " & ex.Message
    'MsgBox("WriteData Error: " & ex.Message )
    'Return False
End Try
End Function
Public Async Function EEData(ByVal msg As String) As Task
Dim outtime As DateTime = Now()
Try
    CurrentResponse = String.Empty
    stractual = String.Empty
    InbStatus = String.Empty
    Dim intime As DateTime
    isDone = False 
    msg = BuildPacket(Opcode, msg)
    msg = ChrW(1) & msg & calcksm(msg) & ChrW(4)
    ComPort.PortName = PortName 
    ComPort.BaudRate = CInt(gBaudRate)
    If Not ComPort.IsOpen Then ComPort.Open()
    ComPort.Write(msg)
        
    do'...................................................... NEW
        Threading.Thread.Sleep(100)
	    SyncLock oLockObject
	    End SyncLock
        If isDone Then
            intime = Now()
            ClosePort() 
            stractual = CurrentResponse 
        End If
    loop until isDone
    '...................................................... 
    Exit Function 
Catch ex As Exception
    gstrcommandname = "ReadEEData Error: " & ex.Message
End Try
End Function
Public Async Sub ReadData(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
Dim response As String = String.Empty
Try
    While ComPort.BytesToRead > 0
        'Await TaskDelayShort()
        response = ComPort.ReadExisting()
        CurrentResponse += response
        if Instr(response,ChrW(4)) > 0 Then '...................... NEW 11/8/2023
            isdone = True
        End If
    End While
Catch ex As Exception
End Try
End Sub
'Private Async Function TaskDelay() As Task
    'Await Task.Delay(gTDelay)
'End Function
'Private Async Function TaskDelayShort() As Task
    'Await Task.Delay(100)
'End Function
#End Region
#Region "Build Outbound Packet"
Private Function BuildPacket(ByVal Opcode As String, ByVal Data As String) As String
Dim Mode, Addr, InbSpec, sOutbound, dummy As String
Dim Leftover As Integer, PadLen As Integer
Mode = "1"
Addr = "00000000"
InbSpec = "023F"
sOutbound = Hex2Bin(Opcode)
sOutbound &= "00000" & Right(Hex2Bin(Mode), 3)
dummy = Hex2Bin(Addr)
sOutbound &= dummy
If (Opcode <> "04") AndAlso (Opcode <> "30") AndAlso (Opcode <> "32") Then
    dummy = Right(Hex2Bin(Right("0000" & InbSpec, 4)), 14)
    sOutbound &= dummy
End If
dummy = Hex2Bin(Data) 'Convert data to binary
If Opcode = "3E" OrElse Opcode = "3F" Then dummy = "00" & dummy
sOutbound &= dummy
Leftover = (Len(sOutbound)) Mod 8
PadLen = 8 - Leftover
If Leftover = 0 Then PadLen = 0
dummy = New String(CChar("0"), PadLen)
sOutbound &= dummy
sOutbound = Bin2Hex(sOutbound) ' Convert back to hex.
Return sOutbound
End Function
Public Function calcksm(ByVal strString As String) As String
Dim lsb, i, msb, j, X As integer
Dim X1 As String = ""
i = 1 : msb = 0 : lsb = 0 : X = 0
For j = 1 To Len(strString)
If i = 1 Then X1 = Mid(strString, j, 1)
If i = -1 Then X1 = Mid(strString, j, 1)
'X = CShort(HX2DC(X1))
X = Cint("&H" & X1)
If i = 1 Then msb = msb + X
If i = -1 Then lsb = lsb + X
i = i * (-1)
Next j
Return Right("00" & Hex((msb * 16 + lsb) Mod 256), 2)
End Function
#End Region
#Region "Open Close Serial Port"
Public Function OpenPort() As Boolean
Try
If ComPort.IsOpen() Then
    ComPort.Close()
End If
    ComPort.BaudRate = Integer.Parse("2400")
    ComPort.DataBits = Integer.Parse("8")
    ComPort.StopBits = DirectCast([Enum].Parse(GetType(StopBits), "One"), StopBits)
    ComPort.Parity = DirectCast([Enum].Parse(GetType(Parity), "None"), Parity)
    ComPort.Handshake = Handshake.None
    ComPort.RtsEnable = False
    ComPort.PortName = PortName
    ComPort.Open
    Return True
Catch ex As Exception
    Return False
End Try
End Function
Public Function ClosePort() As Boolean
Try
If ComPort.IsOpen Then
    ComPort.Close()
    Return True
End If
Catch ex As Exception
Return False
End Try
End Function
#End Region
#Region "HexToByte - ByteToHex"
    ''' <summary>
    ''' method to convert hex string into a byte array
    ''' </summary>
    ''' <param name="msg">string to convert</param>
    ''' <returns>a byte array</returns>
    Private Function HexToByte(ByVal msg As String) As Byte()
        If msg.Length Mod 2 = 0 Then
            'remove any spaces from the string
            _msg = msg
            _msg = msg.Replace(" ", "")
            'create a byte array the length of the
            'divided by 2 (Hex is 2 characters in length)
            'Dim comBuffer As Byte() = New Byte(_msg.Length / 2 - 1) {}
            Dim comBuffer As Byte()
            For i As Integer = 0 To _msg.Length - 1 Step 2
                '    comBuffer(i / 2) = CByte(Convert.ToByte(_msg.Substring(i, 2), 16))
            Next
            'write = True
            'loop through the length of the provided string
            'convert each set of 2 characters to a byte
            'and add to the array
            'return the array
            Return comBuffer
        Else
            _msg = "Invalid format"
            '_type = MessageType.Error
            ' DisplayData(_Type, _msg)
            'write = False
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' method to convert a byte array into a hex string
    ''' </summary>
    ''' <param name="comByte">byte array to convert</param>
    ''' <returns>a hex string</returns>
    Private Function ByteToHex(ByVal comByte As Byte()) As String
        'create a new StringBuilder object
        Dim builder As New StringBuilder(comByte.Length * 3)
        'loop through each byte in the array
        For Each data As Byte In comByte
            builder.Append(Convert.ToString(data, 16).PadLeft(2, "0"c).PadRight(3, " "c))
            'convert the byte to a string and add to the stringbuilder
        Next
        'return the converted value
        Return builder.ToString().ToUpper()
    End Function
#End Region

#Region "SetParityValues"
    Public Sub SetParityValues(ByVal obj As Object)
        For Each str As String In [Enum].GetNames(GetType(Parity))
            DirectCast(obj, ComboBox).Items.Add(str)
        Next
    End Sub
#End Region

#Region "SetStopBitValues"
    Public Sub SetStopBitValues(ByVal obj As Object)
        For Each str As String In [Enum].GetNames(GetType(StopBits))
            DirectCast(obj, ComboBox).Items.Add(str)
        Next
    End Sub
#End Region

#Region "SetPortNameValues"
    Public Sub SetPortNameValues(ByVal obj As Object)

        For Each str As String In SerialPort.GetPortNames()
            DirectCast(obj, ComboBox).Items.Add(str)
        Next
    End Sub
#End Region
#Region "DoDisplay"
Private Class AsyncSerialPort
    Private v1 As String
    Private v2 As Integer
    Private none As Parity
    Private v3 As Integer
    Private one As StopBits

Public Sub New(v1 As String, v2 As Integer, none As Parity, v3 As Integer, one As StopBits)
    Me.v1 = v1
    Me.v2 = v2
    Me.none = none
    Me.v3 = v3
    Me.one = one
    End Sub
End Class
#End Region
#Region "Old Conversions"
Public Function Hex2Bin(ByVal Hexcode As String) As String
Dim dummy, dummy2 As String
Dim j As integer
dummy2 = ""
For j = 1 To Len(Hexcode)
    dummy = Mid(Hexcode, j, 1)
    Select Case dummy
    Case "0"
        dummy = "0000"
    Case "1"
        dummy = "0001"
    Case "2"
        dummy = "0010"
    Case "3"
        dummy = "0011"
    Case "4"
        dummy = "0100"
    Case "5"
        dummy = "0101"
    Case "6"
        dummy = "0110"
    Case "7"
        dummy = "0111"
    Case "8"
        dummy = "1000"
    Case "9"
        dummy = "1001"
    Case "A"
        dummy = "1010"
    Case "B"
        dummy = "1011"
    Case "C"
        dummy = "1100"
    Case "D"
        dummy = "1101"
    Case "E"
        dummy = "1110"
    Case "F"
        dummy = "1111"
    End Select
    dummy2 = dummy2 & dummy
Next j
Return dummy2
End Function
Public Function Bin2Hex(ByVal Binary As String) As String
Dim j As Integer
Dim dummy, dummy2 As String
dummy2 = ""
If Binary.Length Mod 4 > 0 Then
    Dim nPadChars As Int32 = 4 - (Binary.Length Mod 4)
    Dim strPad As String = "0"
    strPad = strPad.PadLeft(nPadChars, CChar("0"))
    Binary = strPad & Binary
End If

For j = 1 To Len(Binary) Step 4
    dummy = Mid(Binary, j, 4)
    Select Case dummy
    Case Is = "0000"
        dummy = "0"
    Case Is = "0001"
        dummy = "1"
    Case Is = "0010"
        dummy = "2"
    Case Is = "0011"
        dummy = "3"
    Case Is = "0100"
        dummy = "4"
    Case Is = "0101"
        dummy = "5"
    Case Is = "0110"
        dummy = "6"
    Case Is = "0111"
        dummy = "7"
    Case Is = "1000"
        dummy = "8"
    Case Is = "1001"
        dummy = "9"
    Case Is = "1010"
        dummy = "A"
    Case Is = "1011"
        dummy = "B"
    Case Is = "1100"
        dummy = "C"
    Case Is = "1101"
        dummy = "D"
    Case Is = "1110"
        dummy = "E"
    Case Is = "1111"
        dummy = "F"
    End Select
        dummy2 = dummy2 & dummy
    Next j
Return dummy2
End Function
#End Region
End Class
End Namespace
