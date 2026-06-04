Option Strict On
Imports System.IO.Ports
Imports System.Text

Public Class USBRelay
    Implements IDisposable

    Private _port As SerialPort
    Private _disposed As Boolean = False

    Public ReadOnly Property PortName As String
        Get
            Return If(_port IsNot Nothing, _port.PortName, "")
        End Get
    End Property

    Public ReadOnly Property IsOpen As Boolean
        Get
            Return _port IsNot Nothing AndAlso _port.IsOpen
        End Get
    End Property

    Public Sub Open(ByVal comPortNum As String)
        Close()
        _port = New SerialPort("COM" & comPortNum, 9600, Parity.None, 8, StopBits.One)
        _port.ReadTimeout = 500
        _port.WriteTimeout = 500
        _port.Open()
    End Sub

    Public Sub Close()
        If _port IsNot Nothing Then
            If _port.IsOpen Then _port.Close()
            _port.Dispose()
            _port = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Sets relay channel state. channel=1, state=True closes relay, False opens it.
    ''' Returns the confirmed state from the device response, or Nothing on failure.
    ''' </summary>
    Public Function SetChannel(ByVal channel As Integer, ByVal closed As Boolean) As String
        Dim cmd As String = "AT+CH" & channel & "=" & If(closed, "1", "0")
        Return SendCommand(cmd)
    End Function

    ''' <summary>
    ''' Queries relay channel state. Returns "0" (open) or "1" (closed), or Nothing on failure.
    ''' </summary>
    Public Function GetChannel(ByVal channel As Integer) As String
        Dim cmd As String = "AT+CH" & channel & "=?"
        Dim response As String = SendCommand(cmd)
        If response IsNot Nothing AndAlso response.Contains("=") Then
            Return response.Substring(response.LastIndexOf("="c) + 1).Trim()
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Sends AT ping. Returns True if device responds OK.
    ''' </summary>
    Public Function Ping() As Boolean
        Return SendCommand("AT") IsNot Nothing
    End Function

    ''' <summary>
    ''' Sends a raw AT command and returns the response string, or Nothing on error/no response.
    ''' </summary>
    Public Function SendCommand(ByVal cmd As String) As String
        If Not IsOpen Then Return Nothing
        Try
            Dim bytes As Byte() = Encoding.ASCII.GetBytes(cmd)
            _port.Write(bytes, 0, bytes.Length)
            Threading.Thread.Sleep(200)
            Dim response As String = _port.ReadExisting().Trim()
            If response.StartsWith("OK") Then Return response
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not _disposed Then
            Close()
            _disposed = True
        End If
    End Sub

End Class
