Option Strict On
Imports System.Drawing
Imports System.IO.Ports
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
'*****************************************************************************************
'               Team SRFN TEST PROCESSING SOFTWARE
'*****************************************************************************************
'*****************************************************************************************
Public Delegate Sub UpdateDisplayTextEventHandler(ByVal msg As String, ByVal msgColor As Color)
Public Delegate Sub SelectAllDisplayTextEventHandler()

''' <summary>
''' This is the original class for serial communications.
''' </summary>
''' <remarks></remarks>
Public Class CommManager
	Public Event UpdateDisplayText(ByVal msg As String, ByVal msgColor As Color)
	'Public Event UpdateDisplayText(ByVal msg As String, ByVal msgColor As Color)
	Public Event SelectAllDisplayText()

	Public Function Team_INTEGRATIONDotNetTest(ByVal port As Integer, ByVal varParam1 As String, ByVal varParam2 As String) As String
		Try
			comPort.PortName = "COM" & port.ToString()
			Dim params1 = varParam1.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
			Dim params2 = varParam2.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

			Dim paramDef = New With
				{
					.MeterSerialNumber = params1(0),
					.MacAddress = params1(1),
					.SQLName = params1(2),
					.UtilitySerialNumber = params1(3),
					.CustomerID = params2(0),
					.ProductFamily = params2(1),
					.FormID = params2(2),
					.RFCarrier = params2(3),
					.PackPath = params2(4),
					.RDInstalled = params2(5)
				}

			If Me.OpenPort() Then
				Me.WriteData("")
			End If
		Catch ex As Exception
			Return "Fail"
		End Try

		Return "Pass"
	End Function

#Region "Manager Enums"
	''' <summary>
	''' enumeration to hold our transmission types
	''' </summary>
	Public Enum TransmissionType
		Text
		Hex
	End Enum

	''' <summary>
	''' enumeration to hold our message types
	''' </summary>
	Public Enum MessageType
		Incoming
		Outgoing
		Normal
		Warning
		[Error]
	End Enum
#End Region

#Region "Manager Variables"
	'property variables
	Private _dateFormat As String = "MM/dd/yyyy HH:mm:ss"
	Private _baudRate As String = String.Empty
	Private _parity As String = String.Empty
	Private _stopBits As String = String.Empty
	Private _dataBits As String = String.Empty
	Private _portName As String = String.Empty
	Private _handshake As String = String.Empty
	Private _transType As TransmissionType
	'Private _displayWindow As RichTextBox
	Private _msg As String
	Private _type As MessageType
	Public inbMessage As String
	Public TestFail As Boolean = True
	'global manager variables
	Private MessageColor As Color() = {Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red}
	Private comPort As New SerialPort()
	Private write As Boolean = True
#End Region
#Region "Manager Properties"
	''' <summary>
	''' Property to hold the BaudRate
	''' of our manager class
	''' </summary>
	Public Property BaudRate() As String
		Get
			Return _baudRate
		End Get
		Set(ByVal value As String)
			_baudRate = value
		End Set
	End Property

	''' <summary>
	''' property to hold the Parity
	''' of our manager class
	''' </summary>
	Public Property Parity() As String
		Get
			Return _parity
		End Get
		Set(ByVal value As String)
			_parity = value
		End Set
	End Property

	''' <summary>
	''' property to hold the StopBits
	''' of our manager class
	''' </summary>
	Public Property StopBits() As String
		Get
			Return _stopBits
		End Get
		Set(ByVal value As String)
			_stopBits = value
		End Set
	End Property

	''' <summary>
	''' property to hold the DataBits
	''' of our manager class
	''' </summary>
	Public Property DataBits() As String
		Get
			Return _dataBits
		End Get
		Set(ByVal value As String)
			_dataBits = value
		End Set
	End Property

	''' <summary>
	''' property to hold the PortName
	''' of our manager class
	''' </summary>
	Public Property PortName() As String
		Get
			Return _portName
		End Get
		Set(ByVal value As String)
			_portName = value
		End Set
	End Property

	''' <summary>
	''' property to hold our TransmissionType
	''' of our manager class
	''' </summary>
	Public Property CurrentTransmissionType() As TransmissionType
		Get
			Return _transType
		End Get
		Set(ByVal value As TransmissionType)
			_transType = value
		End Set
	End Property

	''' <summary>
	''' Property to hold the message being sent
	''' through the serial port
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Message() As String
		Get
			Return _msg
		End Get
		Set(ByVal value As String)
			_msg = value
		End Set
	End Property

	''' <summary>
	''' Message to hold the transmission type
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Type() As MessageType
		Get
			Return _type
		End Get
		Set(ByVal value As MessageType)
			_type = value
		End Set
	End Property
#End Region
#Region "Manager Constructors"
	''' <summary>
	''' Constructor to set the properties of our Manager Class
	''' </summary>
	''' <param name="baud">Desired BaudRate</param>
	''' <param name="par">Desired Parity</param>
	''' <param name="sBits">Desired StopBits</param>
	''' <param name="dBits">Desired DataBits</param>
	''' <param name="name">Desired PortName</param>
	Public Sub New(ByVal baud As String, ByVal par As String, ByVal sBits As String, ByVal dBits As String, ByVal name As String)
		_baudRate = baud
		_parity = par
		_stopBits = sBits
		_dataBits = dBits
		_portName = name
		'now add an event handler
		AddHandler comPort.DataReceived, AddressOf comPort_DataReceived
	End Sub

	''' <summary>
	''' Comstructor to set the properties of our
	''' serial port communicator to nothing
	''' </summary>
	Public Sub New()
		_baudRate = String.Empty
		_parity = String.Empty
		_stopBits = String.Empty
		_dataBits = String.Empty
		_portName = "COM1"
		'add event handler
		AddHandler comPort.DataReceived, AddressOf comPort_DataReceived
	End Sub
#End Region

#Region "WriteData"
	Public Sub WriteData(ByVal msg As String)
		Select Case CurrentTransmissionType
			Case TransmissionType.Text
				'first make sure the port is open
				'if its not open then open it
				Try
					If Not (comPort.IsOpen = True) Then
						comPort.Open()
					End If
				Catch ex As Exception
					MsgBox("Port is Closed")
					Return
				End Try
				'send the message to the port
				comPort.Write(msg)
				'display the message
				_type = MessageType.Outgoing
				_msg = msg & "" & Environment.NewLine & ""
				DisplayData(_type, _msg)
				Exit Select
			Case TransmissionType.Hex
				Try
					'convert the message to byte array
					Dim newMsg As Byte() = HexToByte(msg)
					If Not write Then
						DisplayData(_type, _msg)
						Return
					End If
					'send the message to the port
					comPort.Write(newMsg, 0, newMsg.Length)
					'convert back to hex and display
					_type = MessageType.Outgoing
					_msg = ByteToHex(newMsg) & "" & Environment.NewLine & ""
					DisplayData(_type, _msg)
				Catch ex As FormatException
					'display error message
					_type = MessageType.Error
					_msg = ex.Message & "" & Environment.NewLine & ""
					DisplayData(_type, _msg)
				Finally
					RaiseEvent SelectAllDisplayText()
				End Try
				Exit Select
			Case Else
				'first make sure the port is open
				'if its not open then open it
				If Not (comPort.IsOpen = True) Then
					comPort.Open()
				End If
				'send the message to the port
				comPort.Write(msg)
				'display the message
				_type = MessageType.Outgoing
				_msg = msg & "" & Environment.NewLine & ""
				DisplayData(MessageType.Outgoing, msg & "" & Environment.NewLine & "")
				Exit Select
		End Select
	End Sub
#End Region

#Region "HexToByte"
	''' <summary>
	''' method to convert hex string into a byte array
	''' </summary>
	''' <param name="msg">string to convert</param>
	''' <returns>a byte array</returns>
	Private Function HexToByte(ByVal msg As String) As Byte()
		If msg.Length Mod 2 = 0 Then
			_msg = msg
			_msg = msg.Replace(" ", "")
			Dim comBuffer As Byte() = New Byte(CInt(_msg.Length / 2 - 1)) {}
			For i As Integer = 0 To _msg.Length - 1 Step 2
				comBuffer(CInt(i / 2)) = CByte(Convert.ToByte(_msg.Substring(i, 2), 16))
			Next
			write = True
			Return comBuffer
		Else
			_msg = "Invalid format"
			_type = MessageType.Error
			write = False
			Return Nothing
		End If
	End Function
#End Region

#Region "ByteToHex"
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

#Region "DisplayData"
	''' <summary>
	''' Method to display the data to and
	''' from the port on the screen
	''' </summary>
	''' <remarks></remarks>
	<STAThread()>
	Public Sub DisplayData(ByVal type As MessageType, ByVal msg As String)
		Try
			DoDisplay()
		Catch ex As Exception
			'MsgBox("DisplayData Error")
		End Try
	End Sub
#End Region

#Region "OpenPort"
	Public Function OpenPort() As Boolean
		Try
			'first check if the port is already open
			'if its open then close it
			If comPort.IsOpen = True Then
				comPort.Close()
			End If

			'set the properties of our SerialPort Object
			comPort.BaudRate = Integer.Parse(_baudRate)
			'BaudRate
			comPort.DataBits = Integer.Parse(_dataBits)
			'DataBits
			comPort.StopBits = DirectCast([Enum].Parse(GetType(StopBits), _stopBits), StopBits)
			'StopBits
			comPort.Parity = DirectCast([Enum].Parse(GetType(Parity), _parity), Parity)
			'Parity
			comPort.Handshake = Handshake.None
			comPort.RtsEnable = False
			comPort.PortName = _portName
			'PortName
			'now open the port
			comPort.Open()
			'display message
			_type = MessageType.Normal
			_msg = "Port opened at " & DateTime.Now.ToString(_dateFormat) & Environment.NewLine
			DisplayData(_type, _msg)
			'return true
			Return True
		Catch ex As Exception
			DisplayData(MessageType.[Error], ex.Message)
			Return False
		End Try
	End Function
#End Region

#Region "ClosePort "
	Public Sub ClosePort()
		If comPort.IsOpen Then
			_msg = "Port closed at " & DateTime.Now.ToString(_dateFormat) & "" & Environment.NewLine & ""
			_type = MessageType.Normal
			DisplayData(_type, _msg)
			comPort.Close()
		End If
	End Sub
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

#Region "comPort_DataReceived"
	''' <summary>
	''' method that will be called when theres data waiting in the buffer
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Private Sub comPort_DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)

		'determine the mode the user selected (binary/string)
		Select Case CurrentTransmissionType
			Case TransmissionType.Text
				'user chose string
				'read data waiting in the buffer
				Dim msg As String = comPort.ReadExisting()
				'display the data to the user
				_type = MessageType.Incoming
				_msg = msg
				DisplayData(MessageType.Incoming, msg & "" & Environment.NewLine & "" & vbCrLf)
				inbMessage = Trim(msg)
				If InStr(inbMessage, "SelfTest") > 0 Then TestFail = True
				Dim temp As Object = Thread.CurrentThread.IsThreadPoolThread
				Exit Select
			Case TransmissionType.Hex
				'user chose binary
				'retrieve number of bytes in the buffer
				Dim bytes As Integer = comPort.BytesToRead
				'create a byte array to hold the awaiting data
				Dim comBuffer As Byte() = New Byte(bytes - 1) {}
				'read the data and store it
				comPort.Read(comBuffer, 0, bytes)
				'display the data to the user
				_type = MessageType.Incoming
				_msg = ByteToHex(comBuffer) & "" & Environment.NewLine & ""
				DisplayData(MessageType.Incoming, ByteToHex(comBuffer) & "" & Environment.NewLine & "")
				Exit Select
			Case Else
				'read data waiting in the buffer
				Dim str As String = comPort.ReadExisting()
				'display the data to the user
				_type = MessageType.Incoming
				_msg = str & "" & Environment.NewLine & ""
				DisplayData(MessageType.Incoming, str & "" & Environment.NewLine & "")
				Exit Select
		End Select
	End Sub
#End Region

#Region "DoDisplay"
	Private Sub DoDisplay()
		RaiseEvent UpdateDisplayText(_msg, MessageColor(CType(_type, Integer)))
	End Sub
#End Region

End Class

