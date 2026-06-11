Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO.Ports
Imports TWRN.Communication.TWRN

Public Class frmOCX
    Inherits Form
    Implements IMainForm

    ' ── controls ─────────────────────────────────────────────────
    Private WithEvents mnuMain As MenuStrip
    Private WithEvents mnuConfigure As ToolStripMenuItem
    Private WithEvents mnuSQLConfig As ToolStripMenuItem
    Private WithEvents mnuCommPorts As ToolStripMenuItem
    Private WithEvents mnuUploadFirmware As ToolStripMenuItem
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
    Private WithEvents txtPassword As TextBox
    Private lblSetSQL As Label

    Private _lgmodule As New TWRN.Communication.TWRN.lgmoduleINT()
    Private _locked As Boolean = False
    Private _adminMode As Boolean = False
    Private _frmCommPorts As frmCommPorts = Nothing
    Private _opticalPort As String = ""

    ' ── constructor ──────────────────────────────────────────────
    Public Sub New()
        InitializeComponent()
        PopulateCommPorts()
        LoadProductFamily()
        Text = "TWACS Integration Test"
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        mnuMain = New MenuStrip()
        mnuConfigure = New ToolStripMenuItem()
        mnuSQLConfig = New ToolStripMenuItem()
        mnuCommPorts = New ToolStripMenuItem()
        mnuUploadFirmware = New ToolStripMenuItem()
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
        txtPassword = New TextBox()
        lblSetSQL = New Label()
        Dim lblCmd As New Label()

        mnuMain.SuspendLayout()
        Panel1.SuspendLayout()
        Me.SuspendLayout()

        ' ── MenuStrip ─────────────────────────────────────────────
        mnuConfigure.DropDownItems.AddRange(New ToolStripItem() {mnuSQLConfig, mnuCommPorts, mnuUploadFirmware})
        mnuConfigure.Text = "Configure"
        mnuSQLConfig.Text = "SQL Config"
        mnuCommPorts.Text = "Comm Ports"
        mnuUploadFirmware.Text = "Firmware"
        lblPortStatus.Alignment = ToolStripItemAlignment.Right
        lblPortStatus.Font = New Font("Microsoft Sans Serif", 8, FontStyle.Bold)
        lblPortStatus.Margin = New Padding(0, 1, 31, 2)
        lblPortStatus.Text = "Port: --"
        mnuMain.Items.AddRange(New ToolStripItem() {mnuConfigure, lblPortStatus})
        mnuMain.Location = New Point(0, 0)
        mnuMain.Size = New Size(616, 24)

        ' ── Set SQL (upper right) ────────────────────────────────
        lblSetSQL.AutoSize = True
        lblSetSQL.Location = New Point(557, 28)
        lblSetSQL.Text = "Set SQL"

        txtPassword.Location = New Point(560, 44)
        txtPassword.Size = New Size(44, 20)
        txtPassword.PasswordChar = CChar("*")
        txtPassword.TextAlign = HorizontalAlignment.Center

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

        btnOpticalLogon.Location = New Point(108, 311)
        btnOpticalLogon.Size = New Size(90, 23)
        btnOpticalLogon.Text = "Optical Logon"

        btnChkPort.Location = New Point(206, 313)
        btnChkPort.Size = New Size(90, 23)
        btnChkPort.Text = "Check Port"

        btnResetRelease.Location = New Point(417, 399)
        btnResetRelease.Size = New Size(90, 23)
        btnResetRelease.Text = "Reset Release"

        ' ── Button row 2 (y=338) ─────────────────────────────────
        btnReadFirmware.Location = New Point(12, 338)
        btnReadFirmware.Size = New Size(90, 23)
        btnReadFirmware.Text = "Read Firmware"

        btnOpticalLogoff.Location = New Point(108, 339)
        btnOpticalLogoff.Size = New Size(90, 23)
        btnOpticalLogoff.Text = "Optical Logoff"

        btnToggleUSB.Location = New Point(206, 342)
        btnToggleUSB.Size = New Size(90, 23)
        btnToggleUSB.Text = "USB OFF"
        btnToggleUSB.BackColor = Color.Yellow

        btnBatch.Location = New Point(514, 429)
        btnBatch.Size = New Size(90, 23)
        btnBatch.Text = "Batch"
        btnBatch.Enabled = False

        ' ── Button row 3 (y=367) ─────────────────────────────────
        btnVerifyComm.Location = New Point(12, 367)
        btnVerifyComm.Size = New Size(90, 23)
        btnVerifyComm.Text = "Verify Comm"

        btnOpenLog.Location = New Point(514, 455)
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
        lblSQLLbl.Location = New Point(232, 44)
        lblSQLLbl.Text = "SQL Server"

        txtSQLServer.Location = New Point(235, 57)
        txtSQLServer.Size = New Size(122, 20)

        txtSQLServer.ReadOnly = True
        txtFormID.Enabled = False

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
            lblSQLLbl, txtSQLServer
        })

        ' ── Form ─────────────────────────────────────────────────
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(616, 680)
        Me.MainMenuStrip = mnuMain
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.CenterScreen

        Me.Controls.AddRange(New Control() {
            mnuMain, lblCmd,
            lblSetSQL, txtPassword,
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

    Private Sub LoadProductFamily()
        txtProductFamily.Items.Clear()
        Dim xmlPath As String = IO.Path.Combine(Application.StartupPath, "Aclara_TestParameters", "SetIntegrationParameters.xml")
        If IO.File.Exists(xmlPath) Then
            Try
                Dim ds As New System.Data.DataSet()
                ds.ReadXml(xmlPath)
                Dim tbl As System.Data.DataTable = ds.Tables("Table")
                If tbl IsNot Nothing Then
                    For Each row As System.Data.DataRow In tbl.Rows
                        Dim name As String = row.Item("Productname").ToString().Trim()
                        If name <> "" AndAlso Not txtProductFamily.Items.Contains(name) Then
                            txtProductFamily.Items.Add(name)
                        End If
                    Next
                End If
            Catch
            End Try
        End If
        If txtProductFamily.Items.Count = 0 Then
            txtProductFamily.Items.AddRange(New Object() {
                "UMT-C-KV2", "UMT-C-KV2_Gen5", "UMTR-G2",
                "UMT-GCB", "UMT-GCB10_6.xx", "UMT-GCB12_6.xx"
            })
        End If
        If txtProductFamily.Items.Count > 0 Then txtProductFamily.SelectedIndex = 0
    End Sub

    Private Sub UpdatePortLabel()
        Dim sel As String = txtCommPort.Text
        If sel = "" Then
            lblPortStatus.Text = "Port: --"
        Else
            lblPortStatus.Text = "Port: COM" & sel
        End If
    End Sub

    Private Function GetCommPortNumber() As Integer
        Dim n As Integer = 1
        Integer.TryParse(txtCommPort.Text, n)
        Return n
    End Function

    Private Function IsOptical() As Boolean
        Return _opticalPort <> ""
    End Function

    ' ── menu handlers ────────────────────────────────────────────
    Private Sub mnuSQLConfig_Click(sender As Object, e As EventArgs) Handles mnuSQLConfig.Click
        Dim frm As New frmSQLConfig(Me)
        frm.ShowDialog(Me)
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

    Private Sub mnuUploadFirmware_Click(sender As Object, e As EventArgs) Handles mnuUploadFirmware.Click
        Dim frm As New frmFirmware(Me)
        frm.ShowDialog(Me)
    End Sub

    Private Sub PlaceForm(frm As Form)
        Dim screen As Rectangle = System.Windows.Forms.Screen.GetWorkingArea(Me)
        Dim x As Integer = Me.Right + 5
        If x + frm.Width > screen.Right Then x = screen.Right - frm.Width
        frm.Location = New Point(x, Me.Top)
    End Sub

    ' ── button handlers ──────────────────────────────────────────
    Private Async Sub btnReadTWACS_Click(sender As Object, e As EventArgs) Handles btnReadTWACS.Click
        Dim port As Integer = GetCommPortNumber()
        btnReadTWACS.Enabled = False
        txtResults.Text = "Reading TWACS ID on COM" & port & "..."
        Dim result As String = Await _lgmodule.ReadTWACSID(port)
        btnReadTWACS.Enabled = True
        If result <> "" AndAlso Not result.StartsWith("Error") Then
            txtTWACSAddress.Text = result
            txtResults.Text = "TWACS ID: " & result
        ElseIf result.StartsWith("Error") Then
            txtResults.Text = result
        Else
            txtResults.Text = "No response from COM" & port
        End If
    End Sub

    Private Sub btnReadFirmware_Click(sender As Object, e As EventArgs) Handles btnReadFirmware.Click
        txtResults.Text = "Read Firmware: not yet implemented"
    End Sub

    Private Async Sub btnVerifyComm_Click(sender As Object, e As EventArgs) Handles btnVerifyComm.Click
        Dim port As Integer = GetCommPortNumber()
        btnVerifyComm.Enabled = False
        Try
            If SRFN.Communication.CommManager2.RelayPort <> "" Then
                txtResults.Text = "Verify Comm: setting USB relay to Closed..."
                Me.Refresh()
                SRFN.Communication.CommManager2.RelaySetAndGet(1, True)
                btnToggleUSB.Text = If(SRFN.Communication.CommManager2.UsbIsOn, "USB ON", "USB OFF")
                btnToggleUSB.BackColor = If(SRFN.Communication.CommManager2.UsbIsOn, Color.Green, Color.Yellow)
                If Not SRFN.Communication.CommManager2.UsbIsOn Then
                    txtResults.Text = "Verify Comm: USB relay could not be set to Closed"
                    txtResults.BackColor = Color.Red
                    Return
                End If
                Await Task.Delay(500)
            End If
            txtResults.BackColor = Color.Yellow
            txtResults.Text = "Verify Comm: reading TWACS ID on COM" & port & "..."
            Me.Refresh()
            Dim result As String = Await _lgmodule.ReadTWACSID(port)
            If result <> "" AndAlso Not result.StartsWith("Error") Then
                txtTWACSAddress.Text = result
                txtResults.BackColor = Color.LightGreen
                txtResults.Text = "TWACS Communication Successful — ID: " & result
            Else
                txtResults.BackColor = Color.Red
                txtResults.Text = "Verify Comm Failed — " & If(result = "", "no response from COM" & port, result)
            End If
        Catch ex As Exception
            txtResults.BackColor = Color.Red
            txtResults.Text = "Verify Comm error: " & ex.Message
        Finally
            btnVerifyComm.Enabled = True
        End Try
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
        Dim portNum As String = SRFN.Communication.CommManager2.RelayPort.Trim()
        If portNum = "" Then
            txtResults.Text = "USB Relay: no Relay Port selected — set it via Configure > Comm Ports"
            txtResults.BackColor = Color.Red
            Return
        End If
        Try
            Dim state As String = SRFN.Communication.CommManager2.RelaySetAndGet(1, Not SRFN.Communication.CommManager2.UsbIsOn)
            If state IsNot Nothing Then
                txtResults.Text = "Relay State: " & If(SRFN.Communication.CommManager2.UsbIsOn, "ON (Closed)", "OFF (Open)") & " (verified)"
                txtResults.BackColor = If(SRFN.Communication.CommManager2.UsbIsOn, Color.LightGreen, Color.Yellow)
                btnToggleUSB.Text = If(SRFN.Communication.CommManager2.UsbIsOn, "USB ON", "USB OFF")
                btnToggleUSB.BackColor = If(SRFN.Communication.CommManager2.UsbIsOn, Color.Green, Color.Yellow)
            Else
                txtResults.Text = "USB Relay: state unknown after command"
                txtResults.BackColor = Color.Yellow
            End If
        Catch ex As Exception
            txtResults.Text = "USB Relay error: " & ex.Message
            txtResults.BackColor = Color.Red
        End Try
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

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged
        If txtPassword.Text.ToUpper() = "RA66" Then
            SetAdminMode(Not _adminMode)
            txtPassword.Text = ""
            txtResults.Text = If(_adminMode, "Admin unlocked — config accessible.", "Admin locked.")
        End If
    End Sub


    Private Sub SetAdminMode(enabled As Boolean)
        _adminMode = enabled
        txtSQLServer.ReadOnly = Not enabled
        txtFormID.Enabled = enabled
    End Sub

    Private Sub txtProductFamily_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtProductFamily.SelectedIndexChanged
        ' TODO: load txtFormID items from TWACS FormID XML filtered by product family
    End Sub

    Private Sub txtCommPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtCommPort.SelectedIndexChanged
        UpdatePortLabel()
    End Sub

    ' ── IMainForm implementation ──────────────────────────────────────────
    Public Property DLLRevisionText As String Implements IMainForm.DLLRevisionText
        Get
            Return ""
        End Get
        Set(v As String)
        End Set
    End Property
    Public Property CustomerValuesText As String Implements IMainForm.CustomerValuesText
        Get
            Return ""
        End Get
        Set(v As String)
        End Set
    End Property
    Public Property TestResultsText As String Implements IMainForm.TestResultsText
        Get
            Return ""
        End Get
        Set(v As String)
        End Set
    End Property
    Public Property FirmwareConnStr As String Implements IMainForm.FirmwareConnStr
        Get
            Return ""
        End Get
        Set(v As String)
        End Set
    End Property
    Public Property SQLServerText As String Implements IMainForm.SQLServerText
        Get
            Return txtSQLServer.Text
        End Get
        Set(v As String)
            txtSQLServer.Text = v
        End Set
    End Property
    Private _sqlServer As String = ""
    Public Property SQLServer As String Implements IMainForm.SQLServer
        Get
            Return _sqlServer
        End Get
        Set(v As String)
            _sqlServer = v
            txtSQLServer.Text = v
        End Set
    End Property
    Public Property ShowFormChecked As Boolean Implements IMainForm.ShowFormChecked
        Get
            Return False
        End Get
        Set(v As Boolean)
        End Set
    End Property
    Public Property RA6ProgPathText As String Implements IMainForm.RA6ProgPathText
        Get
            Return ""
        End Get
        Set(v As String)
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
            UpdatePortLabel()
        End Set
    End Property
    Public Property DebugPortText As String Implements IMainForm.DebugPortText
        Get
            Return ""
        End Get
        Set(v As String)
        End Set
    End Property
    Public Property RelayPortText As String Implements IMainForm.RelayPortText
        Get
            Return SRFN.Communication.CommManager2.RelayPort
        End Get
        Set(v As String)
            SRFN.Communication.CommManager2.RelayPort = v
        End Set
    End Property
    Public Property OpticalPortText As String Implements IMainForm.OpticalPortText
        Get
            Return _opticalPort
        End Get
        Set(v As String)
            _opticalPort = v
        End Set
    End Property
    Public Sub ShowPorts() Implements IMainForm.ShowPorts
        PopulateCommPorts()
    End Sub
    Public Sub SetFormXml() Implements IMainForm.SetFormXml
    End Sub
    Public Sub UpdatePortStatus() Implements IMainForm.UpdatePortStatus
        UpdatePortLabel()
    End Sub
    Public Property ProductFamilyText As String Implements IMainForm.ProductFamilyText
        Get
            Return txtProductFamily.Text
        End Get
        Set(v As String)
            txtProductFamily.Text = v
        End Set
    End Property
    Public Sub WriteFirmwareToNIC(firmwareName As String) Implements IMainForm.WriteFirmwareToNIC
        txtResults.Text = "Firmware upload not available in TWACS mode."
    End Sub
    Public Function VirginDelay() As Task(Of Boolean) Implements IMainForm.VirginDelay
        Return Task.FromResult(False)
    End Function

End Class
