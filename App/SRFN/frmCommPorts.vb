Public Class frmCommPorts

    Private _main As IMainForm
    Private _knownPorts As New HashSet(Of String)
    Private _discoverTimer As New System.Windows.Forms.Timer() With {.Interval = 500}
    Private _debugForm As frmDebugTerminal = Nothing

    Public Sub New(mainForm As IMainForm)
        InitializeComponent()
        _main = mainForm
    End Sub

    Private Sub frmCommPorts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPorts()
    End Sub

    Private Sub LoadPorts()
        cboCommPort.Items.Clear()
        cboDebugPort.Items.Clear()
        cboRelayPort.Items.Clear()
        cboOpticalPort.Items.Clear()
        For Each sp As String In My.Computer.Ports.SerialPortNames
            Dim portNum As String = sp.Replace("COM", "")
            cboCommPort.Items.Add(portNum)
            cboDebugPort.Items.Add(portNum)
            cboRelayPort.Items.Add(portNum)
            cboOpticalPort.Items.Add(portNum)
        Next
        cboCommPort.Text = _main.CommPortText
        cboDebugPort.Text = _main.DebugPortText
        cboRelayPort.Text = _main.RelayPortText
        cboOpticalPort.Text = _main.OpticalPortText
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        LoadPorts()
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Dim comm As String = cboCommPort.Text
        Dim debug As String = cboDebugPort.Text
        Dim relay As String = cboRelayPort.Text
        Dim optical As String = cboOpticalPort.Text
        _main.ShowPorts()
        _main.CommPortText = comm
        _main.DebugPortText = debug
        _main.RelayPortText = relay
        _main.OpticalPortText = optical
        _main.SetFormXml()
        _main.UpdatePortStatus()
        Me.Close()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmCommPorts_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _discoverTimer.Stop()
        _main.PasswordText = ""
        ' Debug terminal stays open independently — do not close it here
    End Sub

    ' ── Debug Window ────────────────────────────────────────────────────────────

    Private Sub chkDebugWindow_CheckedChanged(sender As Object, e As EventArgs) Handles chkDebugWindow.CheckedChanged
        If chkDebugWindow.Checked Then
            If _debugForm Is Nothing OrElse _debugForm.IsDisposed Then
                _debugForm = New frmDebugTerminal(Me, cboDebugPort.Text.Trim())
                _debugForm.Show()
            End If
            Me.Close()
        Else
            CloseDebugForm()
        End If
    End Sub

    Private Sub CloseDebugForm()
        If _debugForm IsNot Nothing AndAlso Not _debugForm.IsDisposed Then
            _debugForm.Close()
        End If
        _debugForm = Nothing
    End Sub

    ' ── Port Discovery ───────────────────────────────────────────────────────────

    Private Sub btnDiscover_Click(sender As Object, e As EventArgs) Handles btnDiscover.Click
        _knownPorts = New HashSet(Of String)(My.Computer.Ports.SerialPortNames)
        lblDiscoverStatus.Text = "Plug in device..."
        btnDiscover.Enabled = False
        AddHandler _discoverTimer.Tick, AddressOf DiscoverTimer_Tick
        _discoverTimer.Start()
    End Sub

    Private Sub DiscoverTimer_Tick(sender As Object, e As EventArgs)
        Dim current = My.Computer.Ports.SerialPortNames
        Dim newPorts = current.Where(Function(p) Not _knownPorts.Contains(p)).ToList()
        If newPorts.Count = 0 Then Return
        _discoverTimer.Stop()
        RemoveHandler _discoverTimer.Tick, AddressOf DiscoverTimer_Tick
        Dim portName As String = newPorts(0)
        Dim portNum As String = portName.Replace("COM", "")
        lblDiscoverStatus.Text = "Found: " & portName
        Dim selected As String = AskPortType(portName)
        Select Case selected
            Case "Mfg"
                If Not cboCommPort.Items.Contains(portNum) Then cboCommPort.Items.Add(portNum)
                cboCommPort.Text = portNum
            Case "Debug"
                If Not cboDebugPort.Items.Contains(portNum) Then cboDebugPort.Items.Add(portNum)
                cboDebugPort.Text = portNum
            Case "Relay"
                If Not cboRelayPort.Items.Contains(portNum) Then cboRelayPort.Items.Add(portNum)
                cboRelayPort.Text = portNum
            Case "Optical"
                If Not cboOpticalPort.Items.Contains(portNum) Then cboOpticalPort.Items.Add(portNum)
                cboOpticalPort.Text = portNum
        End Select
        lblDiscoverStatus.Text = If(selected = "", "", portName & " assigned")
        btnDiscover.Enabled = True
    End Sub

    Private Function AskPortType(portName As String) As String
        Dim f As New Form()
        f.Text = "Device Found"
        f.ClientSize = New System.Drawing.Size(210, 170)
        f.FormBorderStyle = FormBorderStyle.FixedDialog
        f.StartPosition = FormStartPosition.CenterParent
        f.MaximizeBox = False
        f.MinimizeBox = False

        Dim lbl As New Label()
        lbl.Text = portName & " detected. Assign as:"
        lbl.Location = New System.Drawing.Point(10, 12)
        lbl.Size = New System.Drawing.Size(190, 20)
        f.Controls.Add(lbl)

        Dim rbMfg As New RadioButton()
        rbMfg.Text = "Mfg Port"
        rbMfg.Location = New System.Drawing.Point(20, 40)
        rbMfg.Checked = True
        f.Controls.Add(rbMfg)

        Dim rbDebug As New RadioButton()
        rbDebug.Text = "Debug Port"
        rbDebug.Location = New System.Drawing.Point(20, 65)
        f.Controls.Add(rbDebug)

        Dim rbRelay As New RadioButton()
        rbRelay.Text = "USB Relay"
        rbRelay.Location = New System.Drawing.Point(20, 90)
        f.Controls.Add(rbRelay)

        Dim rbOptical As New RadioButton()
        rbOptical.Text = "Optical USB Port"
        rbOptical.Location = New System.Drawing.Point(20, 115)
        f.Controls.Add(rbOptical)

        f.ClientSize = New System.Drawing.Size(210, 195)
        Dim btnAssign As New Button()
        btnAssign.Text = "Assign"
        btnAssign.Location = New System.Drawing.Point(55, 158)
        btnAssign.Size = New System.Drawing.Size(75, 25)
        btnAssign.DialogResult = DialogResult.OK
        f.Controls.Add(btnAssign)
        f.AcceptButton = btnAssign

        Dim btnCancel As New Button()
        btnCancel.Text = "Cancel"
        btnCancel.Location = New System.Drawing.Point(138, 158)
        btnCancel.Size = New System.Drawing.Size(60, 25)
        btnCancel.DialogResult = DialogResult.Cancel
        f.Controls.Add(btnCancel)
        f.CancelButton = btnCancel

        If f.ShowDialog(Me) = DialogResult.OK Then
            If rbMfg.Checked Then Return "Mfg"
            If rbDebug.Checked Then Return "Debug"
            If rbRelay.Checked Then Return "Relay"
            If rbOptical.Checked Then Return "Optical"
        End If
        Return ""
    End Function

End Class
