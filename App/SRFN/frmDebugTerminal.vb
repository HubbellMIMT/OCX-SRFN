Public Class frmDebugTerminal

    Private _port As New System.IO.Ports.SerialPort()
    Private _portNum As String
    Private _owner As frmCommPorts

    Public Sub New(owner As frmCommPorts, portNum As String)
        InitializeComponent()
        _owner = owner
        _portNum = portNum
    End Sub

    Private Sub frmDebugTerminal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Position just to the right of the owner form
        If _owner IsNot Nothing Then
            Me.Location = New System.Drawing.Point(_owner.Right + 5, _owner.Top)
        End If
        OpenPort()
    End Sub

    Private Sub OpenPort()
        If _portNum = "" Then
            AppendText("[No debug port selected — set Debug Port in Comm Ports dialog]" & vbCrLf)
            Return
        End If
        Try
            If _port.IsOpen Then _port.Close()
            _port.PortName = "COM" & _portNum
            _port.BaudRate = 115200
            _port.DataBits = 8
            _port.Parity = System.IO.Ports.Parity.None
            _port.StopBits = System.IO.Ports.StopBits.One
            AddHandler _port.DataReceived, AddressOf Port_DataReceived
            _port.Open()
            Me.Text = "Debug Terminal  —  COM" & _portNum & "  115200"
            lblPortInfo.Text = "COM" & _portNum & "  |  115200 baud"
            AppendText("[COM" & _portNum & " opened]" & vbCrLf)
        Catch ex As Exception
            Me.Text = "Debug Terminal  —  COM" & _portNum & "  [ERROR]"
            lblPortInfo.Text = "COM" & _portNum & "  |  error"
            AppendText("[Error opening port: " & ex.Message & "]" & vbCrLf)
        End Try
    End Sub

    Private Sub ClosePort()
        Try
            RemoveHandler _port.DataReceived, AddressOf Port_DataReceived
            If _port.IsOpen Then _port.Close()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Port_DataReceived(sender As Object, e As System.IO.Ports.SerialDataReceivedEventArgs)
        Dim data As String = ""
        Try
            data = _port.ReadExisting()
        Catch ex As Exception
            data = "[read error: " & ex.Message & "]"
        End Try
        Me.BeginInvoke(New Action(Sub() AppendText(data)))
    End Sub

    Private Sub AppendText(text As String)
        rtbDisplay.AppendText(text)
        rtbDisplay.ScrollToCaret()
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        If Not _port.IsOpen Then
            AppendText("[Port not open]" & vbCrLf)
            Return
        End If
        Try
            Dim cmd As String = txtSend.Text
            _port.Write(cmd & vbCrLf)
            AppendText("> " & cmd & vbCrLf)
            txtSend.Clear()
        Catch ex As Exception
            AppendText("[Send error: " & ex.Message & "]" & vbCrLf)
        End Try
    End Sub

    Private Sub txtSend_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSend.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnSend.PerformClick()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        rtbDisplay.Clear()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmDebugTerminal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ClosePort()
        _port.Dispose()
        ' Uncheck the checkbox without re-triggering an open
        If _owner IsNot Nothing AndAlso Not _owner.IsDisposed Then
            _owner.chkDebugWindow.Checked = False
        End If
    End Sub

End Class
