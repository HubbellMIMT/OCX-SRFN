Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO.Ports
Imports TWRN.Communication.TWRN

Public Class frmOCX
    Inherits Form

    ' ── controls ─────────────────────────────────────────────────
    Private WithEvents mnuMain As MenuStrip
    Private WithEvents mnuConfigure As ToolStripMenuItem
    Private WithEvents mnuSQLConfig As ToolStripMenuItem
    Private WithEvents mnuCommPorts As ToolStripMenuItem
    Private lblPortStatus As ToolStripLabel

    Private txtResults As TextBox
    Private WithEvents btnSendcommand As Button
    Private WithEvents txtcommand As TextBox

    Private WithEvents btnReadTWACS As Button
    Private WithEvents btnOpticalLogon As Button
    Private WithEvents btnChkPort As Button
    Private WithEvents btnResetRelease As Button
    Private WithEvents btnReadFirmware As Button
    Private WithEvents btnOpticalLogoff As Button
    Private WithEvents btnToggleUSB As Button
    Private WithEvents btnBatch As Button
    Private WithEvents btnVerifyComm As Button
    Private WithEvents btnOpenLog As Button

    Private Panel1 As Panel
    Private WithEvents txtCommPort As ComboBox
    Private WithEvents txtMeterSerial As TextBox
    Private WithEvents txtTWACSAddress As TextBox
    Private WithEvents txtCustomerID As TextBox
    Private WithEvents txtProductFamily As ComboBox
    Private WithEvents txtFormID As ComboBox
    Private WithEvents chkRDInstalled As CheckBox
    Private WithEvents txtPackPath As TextBox
    Private WithEvents btnSendVarParam As Button
    Private WithEvents btnLockForm As Button
    Private WithEvents btnSave As Button
    Private WithEvents txtSQLServer As TextBox
    Private WithEvents btnConnect As Button
    Private WithEvents txtPassword As TextBox

    Private _usbOn As Boolean = True
    Private _locked As Boolean = False

    ' ── constructor ──────────────────────────────────────────────
    Public Sub New()
        InitializeComponent()
        PopulateCommPorts()
        Text = "TWACS Integration Test"
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        mnuMain = New MenuStrip()
        mnuConfigure = New ToolStripMenuItem()
        mnuSQLConfig = New ToolStripMenuItem()
        mnuCommPorts = New ToolStripMenuItem()
        lblPortStatus = New ToolStripLabel()
        txtResults = New TextBox()
        btnSendcommand = New Button()
        txtcommand = New TextBox()
        btnReadTWACS = New Button()
        btnOpticalLogon = New Button()
        btnChkPort = New Button()
        btnResetRelease = New Button()
        btnReadFirmware = New Button()
        btnOpticalLogoff = New Button()
        btnToggleUSB = New Button()
        btnBatch = New Button()
        btnVerifyComm = New Button()
        btnOpenLog = New Button()
        Panel1 = New Panel()
        Dim lblCommPort As New Label()
        txtCommPort = New ComboBox()
        Dim lblMeterSerial As New Label()
        txtMeterSerial = New TextBox()
        Dim lblTWACSAddr As New Label()
        txtTWACSAddress = New TextBox()
        Dim lblCustID As New Label()
        txtCustomerID = New TextBox()
        Dim lblProdFamily As New Label()
        txtProductFamily = New ComboBox()
        Dim lblFormIDLbl As New Label()
        txtFormID = New ComboBox()
        chkRDInstalled = New CheckBox()
        Dim lblPackPathLbl As New Label()
        txtPackPath = New TextBox()
        btnSendVarParam = New Button()
        btnLockForm = New Button()
        btnSave = New Button()
        Dim lblSQLLbl As New Label()
        txtSQLServer = New TextBox()
        btnConnect = New Button()
        txtPassword = New TextBox()
        Dim lblCmd As New Label()

        mnuMain.SuspendLayout()
        Panel1.SuspendLayout()
        Me.SuspendLayout()

        ' ── MenuStrip ────────────────────────────────────────────
        mnuConfigure.DropDownItems.AddRange(New ToolStripItem() {mnuSQLConfig, mnuCommPorts})
        mnuConfigure.Text = "Configure"
        mnuSQLConfig.Text = "SQL Config"
        mnuCommPorts.Text = "Comm Ports"
        lblPortStatus.Alignment = ToolStripItemAlignment.Right
        lblPortStatus.Font = New Font("Microsoft Sans Serif", 8, FontStyle.Bold)
        lblPortStatus.Margin = New Padding(0, 1, 31, 2)
        lblPortStatus.Text = "Port: --"
        mnuMain.Items.AddRange(New ToolStripItem() {mnuConfigure, lblPortStatus})
        mnuMain.Location = New Point(0, 0)
        mnuMain.Size = New Size(616, 24)

        ' ── Send Command area ────────────────────────────────────
        lblCmd.AutoSize = True
        lblCmd.Location = New Point(113, 26)
        lblCmd.Text = "Send Manual Command (msg)"

        btnSendcommand.Location = New Point(12, 37)
        btnSendcommand.Size = New Size(90, 23)
        btnSendcommand.Text = "Send Command"

        txtcommand.Location = New Point(116, 40)
        txtcommand.Size = New Size(266, 20)
        txtcommand.TextAlign = HorizontalAlignment.Center

        ' ── Results ──────────────────────────────────────────────
        txtResults.Location = New Point(12, 78)
        txtResults.Multiline = True
        txtResults.ScrollBars = ScrollBars.Vertical
        txtResults.Size = New Size(592, 229)
        txtResults.ReadOnly = True

        ' ── Button row 1 (y=309) ─────────────────────────────────
        btnReadTWACS.Location = New Point(12, 309)
        btnReadTWACS.Size = New Size(90, 23)
        btnReadTWACS.Text = "Read TWACS"

        btnOpticalLogon.Location = New Point(108, 309)
        btnOpticalLogon.Size = New Size(90, 23)
        btnOpticalLogon.Text = "Optical Logon"

        btnChkPort.Location = New Point(206, 309)
        btnChkPort.Size = New Size(90, 23)
        btnChkPort.Text = "Check Port"

        btnResetRelease.Location = New Point(302, 309)
        btnResetRelease.Size = New Size(90, 23)
        btnResetRelease.Text = "Reset Release"

        ' ── Button row 2 (y=338) ─────────────────────────────────
        btnReadFirmware.Location = New Point(12, 338)
        btnReadFirmware.Size = New Size(90, 23)
        btnReadFirmware.Text = "Read Firmware"

        btnOpticalLogoff.Location = New Point(108, 338)
        btnOpticalLogoff.Size = New Size(90, 23)
        btnOpticalLogoff.Text = "Optical Logoff"

        btnToggleUSB.Location = New Point(206, 338)
        btnToggleUSB.Size = New Size(90, 23)
        btnToggleUSB.Text = "USB OFF"
        btnToggleUSB.BackColor = Color.Yellow

        btnBatch.Location = New Point(302, 338)
        btnBatch.Size = New Size(90, 23)
        btnBatch.Text = "Batch"
        btnBatch.Enabled = False

        ' ── Button row 3 (y=367) ─────────────────────────────────
        btnVerifyComm.Location = New Point(12, 367)
        btnVerifyComm.Size = New Size(90, 23)
        btnVerifyComm.Text = "Verify Comm"

        btnOpenLog.Location = New Point(108, 367)
        btnOpenLog.Size = New Size(90, 23)
        btnOpenLog.Text = "Log Folder"
        btnOpenLog.BackColor = Color.LightYellow

        ' ── Bottom Panel ─────────────────────────────────────────
        Panel1.Location = New Point(12, 484)
        Panel1.Size = New Size(592, 186)
        Panel1.BorderStyle = BorderStyle.FixedSingle

        lblCommPort.AutoSize = True
        lblCommPort.Location = New Point(409, 2)
        lblCommPort.Text = "Comm Port"

        txtCommPort.FormattingEnabled = True
        txtCommPort.Location = New Point(415, 16)
        txtCommPort.Size = New Size(60, 21)

        lblMeterSerial.AutoSize = True
        lblMeterSerial.Location = New Point(4, 4)
        lblMeterSerial.Text = "Meter Serial Number"

        txtMeterSerial.Location = New Point(6, 18)
        txtMeterSerial.Size = New Size(122, 20)

        lblTWACSAddr.AutoSize = True
        lblTWACSAddr.Location = New Point(3, 44)
        lblTWACSAddr.Text = "TWACS Address"

        txtTWACSAddress.Location = New Point(6, 58)
        txtTWACSAddress.Size = New Size(122, 20)

        lblCustID.AutoSize = True
        lblCustID.Location = New Point(132, 44)
        lblCustID.Text = "Customer ID"

        txtCustomerID.Location = New Point(133, 58)
        txtCustomerID.Size = New Size(96, 20)

        lblProdFamily.AutoSize = True
        lblProdFamily.Location = New Point(130, 4)
        lblProdFamily.Text = "Product Family"

        txtProductFamily.FormattingEnabled = True
        txtProductFamily.Location = New Point(132, 18)
        txtProductFamily.Size = New Size(141, 21)

        lblFormIDLbl.AutoSize = True
        lblFormIDLbl.Location = New Point(130, 79)
        lblFormIDLbl.Text = "Form ID"

        txtFormID.FormattingEnabled = True
        txtFormID.Location = New Point(132, 93)
        txtFormID.Size = New Size(157, 21)

        chkRDInstalled.AutoSize = True
        chkRDInstalled.Location = New Point(361, 96)
        chkRDInstalled.Text = "RD Installed"

        lblPackPathLbl.AutoSize = True
        lblPackPathLbl.Location = New Point(4, 120)
        lblPackPathLbl.Text = "PackPath"

        txtPackPath.Location = New Point(6, 134)
        txtPackPath.Size = New Size(183, 20)
        txtPackPath.Text = "c:\pack"

        btnSendVarParam.Enabled = False
        btnSendVarParam.Location = New Point(496, 58)
        btnSendVarParam.Size = New Size(90, 23)
        btnSendVarParam.Text = "Send VarParam"

        btnLockForm.BackColor = Color.LightGreen
        btnLockForm.Location = New Point(496, 87)
        btnLockForm.Size = New Size(90, 23)
        btnLockForm.Text = "Lock Form"

        btnSave.Location = New Point(496, 116)
        btnSave.Size = New Size(90, 23)
        btnSave.Text = "Save All"

        lblSQLLbl.AutoSize = True
        lblSQLLbl.Location = New Point(4, 155)
        lblSQLLbl.Text = "SQL Server"

        txtSQLServer.Location = New Point(80, 152)
        txtSQLServer.Size = New Size(200, 20)

        btnConnect.Location = New Point(286, 150)
        btnConnect.Size = New Size(75, 23)
        btnConnect.Text = "Connect"

        txtPassword.Location = New Point(480, 152)
        txtPassword.PasswordChar = CChar("*")
        txtPassword.Size = New Size(60, 20)
        txtPassword.TextAlign = HorizontalAlignment.Center

        Panel1.Controls.AddRange(New Control() {
            lblCommPort, txtCommPort,
            lblMeterSerial, txtMeterSerial,
            lblTWACSAddr, txtTWACSAddress,
            lblCustID, txtCustomerID,
            lblProdFamily, txtProductFamily,
            lblFormIDLbl, txtFormID,
            chkRDInstalled,
            lblPackPathLbl, txtPackPath,
            btnSendVarParam, btnLockForm, btnSave,
            lblSQLLbl, txtSQLServer, btnConnect,
            txtPassword
        })

        ' ── Form ─────────────────────────────────────────────────
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(616, 680)
        Me.MainMenuStrip = mnuMain
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.CenterScreen

        Me.Controls.AddRange(New Control() {
            mnuMain, lblCmd,
            btnSendcommand, txtcommand,
            txtResults,
            btnReadTWACS, btnOpticalLogon, btnChkPort, btnResetRelease,
            btnReadFirmware, btnOpticalLogoff, btnToggleUSB, btnBatch,
            btnVerifyComm, btnOpenLog,
            Panel1
        })

        mnuMain.ResumeLayout(False)
        mnuMain.PerformLayout()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    ' ── helpers ──────────────────────────────────────────────────
    Private Sub PopulateCommPorts()
        Dim current As String = If(txtCommPort.Text, "")
        txtCommPort.Items.Clear()
        For Each port As String In SerialPort.GetPortNames()
            txtCommPort.Items.Add(port.Replace("COM", "").Replace("com", ""))
        Next
        If current <> "" Then
            Dim idx As Integer = txtCommPort.Items.IndexOf(current)
            If idx >= 0 Then
                txtCommPort.SelectedIndex = idx
            ElseIf txtCommPort.Items.Count > 0 Then
                txtCommPort.SelectedIndex = 0
            End If
        ElseIf txtCommPort.Items.Count > 0 Then
            txtCommPort.SelectedIndex = 0
        End If
        UpdatePortLabel()
    End Sub

    Private Sub UpdatePortLabel()
        lblPortStatus.Text = If(txtCommPort.Text <> "", "Port: COM" & txtCommPort.Text, "Port: --")
    End Sub

    Private Function GetCommPortNumber() As Integer
        Dim n As Integer = 1
        Integer.TryParse(txtCommPort.Text, n)
        Return n
    End Function

    ' ── menu handlers ────────────────────────────────────────────
    Private Sub mnuSQLConfig_Click(sender As Object, e As EventArgs) Handles mnuSQLConfig.Click
    End Sub

    Private Sub mnuCommPorts_Click(sender As Object, e As EventArgs) Handles mnuCommPorts.Click
        PopulateCommPorts()
    End Sub

    ' ── button handlers ──────────────────────────────────────────
    Private Async Sub btnReadTWACS_Click(sender As Object, e As EventArgs) Handles btnReadTWACS.Click
        Dim port As Integer = GetCommPortNumber()
        btnReadTWACS.Enabled = False
        txtResults.Text = "Reading TWACS ID on COM" & port & "..."
        Dim result As String = Await ReadTWACSID(port)
        btnReadTWACS.Enabled = True
        If result <> "" AndAlso Not result.StartsWith("Error") Then
            txtTWACSAddress.Text = result
            txtResults.Text = "TWACS ID: " & result & "  (hex: " & gReg24 & ")"
        ElseIf result.StartsWith("Error") Then
            txtResults.Text = result
        Else
            txtResults.Text = "No response from COM" & port
        End If
    End Sub

    Private Sub btnReadFirmware_Click(sender As Object, e As EventArgs) Handles btnReadFirmware.Click
        txtResults.Text = "Read Firmware: not yet implemented"
    End Sub

    Private Sub btnVerifyComm_Click(sender As Object, e As EventArgs) Handles btnVerifyComm.Click
        txtResults.Text = "Verify Comm: not yet implemented"
    End Sub

    Private Sub btnChkPort_Click(sender As Object, e As EventArgs) Handles btnChkPort.Click
        PopulateCommPorts()
        Dim ports As String = String.Join(", ", SerialPort.GetPortNames())
        txtResults.Text = If(ports <> "", "Available ports: " & ports, "No COM ports found")
    End Sub

    Private Sub btnOpticalLogon_Click(sender As Object, e As EventArgs) Handles btnOpticalLogon.Click
        txtResults.Text = "Optical Logon: not yet implemented"
    End Sub

    Private Sub btnOpticalLogoff_Click(sender As Object, e As EventArgs) Handles btnOpticalLogoff.Click
        txtResults.Text = "Optical Logoff: not yet implemented"
    End Sub

    Private Sub btnToggleUSB_Click(sender As Object, e As EventArgs) Handles btnToggleUSB.Click
        _usbOn = Not _usbOn
        btnToggleUSB.Text = If(_usbOn, "USB OFF", "USB ON")
        txtResults.Text = "USB toggle: not yet implemented"
    End Sub

    Private Sub btnResetRelease_Click(sender As Object, e As EventArgs) Handles btnResetRelease.Click
        txtResults.Text = "Reset Release: not yet implemented"
    End Sub

    Private Sub btnBatch_Click(sender As Object, e As EventArgs) Handles btnBatch.Click
        txtResults.Text = "Batch: not yet implemented"
    End Sub

    Private Sub btnOpenLog_Click(sender As Object, e As EventArgs) Handles btnOpenLog.Click
        Try
            Dim logDir As String = IO.Path.Combine(Application.StartupPath, "Logs")
            If Not IO.Directory.Exists(logDir) Then IO.Directory.CreateDirectory(logDir)
            Process.Start("explorer.exe", logDir)
        Catch ex As Exception
            txtResults.Text = "Log Folder: " & ex.Message
        End Try
    End Sub

    Private Sub btnSendcommand_Click(sender As Object, e As EventArgs) Handles btnSendcommand.Click
        txtResults.Text = "Send Command: not yet implemented"
    End Sub

    Private Sub btnSendVarParam_Click(sender As Object, e As EventArgs) Handles btnSendVarParam.Click
        txtResults.Text = "Send VarParam: not yet implemented"
    End Sub

    Private Sub btnLockForm_Click(sender As Object, e As EventArgs) Handles btnLockForm.Click
        _locked = Not _locked
        Panel1.Enabled = Not _locked
        btnLockForm.BackColor = If(_locked, Color.Red, Color.LightGreen)
        btnLockForm.Text = If(_locked, "Unlock Form", "Lock Form")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        txtResults.Text = "Save All: not yet implemented"
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        txtResults.Text = "Connect SQL: not yet implemented"
    End Sub

    Private Sub txtProductFamily_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtProductFamily.SelectedIndexChanged
        ' TODO: load txtFormID items from TWACS FormID XML filtered by product family
    End Sub

    Private Sub txtCommPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtCommPort.SelectedIndexChanged
        UpdatePortLabel()
    End Sub

End Class
