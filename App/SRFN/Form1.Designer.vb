<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Me.btnReset = New System.Windows.Forms.Button()
		Me.mnuMain = New System.Windows.Forms.MenuStrip()
		Me.mnuConfigure = New System.Windows.Forms.ToolStripMenuItem()
		Me.mnuSQLConfig = New System.Windows.Forms.ToolStripMenuItem()
		Me.mnuCommPorts = New System.Windows.Forms.ToolStripMenuItem()
		Me.mnuUploadFirmware = New System.Windows.Forms.ToolStripMenuItem()
		Me.lblPortStatus = New System.Windows.Forms.ToolStripLabel()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.txtRFCarrier = New System.Windows.Forms.ComboBox()
		Me.txtRelayPort = New System.Windows.Forms.ComboBox()
		Me.lblRelayPort = New System.Windows.Forms.Label()
		Me.txtDebugPort = New System.Windows.Forms.ComboBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.btnUpdatePort = New System.Windows.Forms.Button()
		Me.txtCommPort = New System.Windows.Forms.ComboBox()
		Me.btnLockForm = New System.Windows.Forms.Button()
		Me.txtFormID = New System.Windows.Forms.ComboBox()
		Me.txtProductFamily = New System.Windows.Forms.ComboBox()
		Me.chkRDInstalled = New System.Windows.Forms.CheckBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtPackPath = New System.Windows.Forms.TextBox()
		Me.btnGetCustomerName = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label18 = New System.Windows.Forms.Label()
		Me.txtUtilitySerial = New System.Windows.Forms.TextBox()
		Me.txtCustomerID = New System.Windows.Forms.TextBox()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.txtMeterSerial = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.txtMacAddress = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.FormID = New System.Windows.Forms.Label()
		Me.btnSave = New System.Windows.Forms.Button()
		Me.lblDLLVer = New System.Windows.Forms.Label()
		Me.txtPassword = New System.Windows.Forms.TextBox()
		Me.btnReadMacAddress = New System.Windows.Forms.Button()
		Me.btnVerifyComm = New System.Windows.Forms.Button()
		Me.txtResults = New System.Windows.Forms.TextBox()
		Me.btnReadFirmware = New System.Windows.Forms.Button()
		Me.btnquietON = New System.Windows.Forms.Button()
		Me.btnquietOFF = New System.Windows.Forms.Button()
		Me.btnSendcommand = New System.Windows.Forms.Button()
		Me.txtcommand = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.btnGetHash = New System.Windows.Forms.Button()
		Me.btnquietStatus = New System.Windows.Forms.Button()
		Me.btnChkPort = New System.Windows.Forms.Button()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.btnOpticalLogon = New System.Windows.Forms.Button()
		Me.btnOpticalLogoff = New System.Windows.Forms.Button()
		Me.btnReadEnergy = New System.Windows.Forms.Button()
		Me.btnEnableDTLS = New System.Windows.Forms.Button()
		Me.btnDisableDTLS = New System.Windows.Forms.Button()
		Me.btnSendDebug = New System.Windows.Forms.Button()
		Me.btn_edInfo = New System.Windows.Forms.Button()
		Me.btn_edfw = New System.Windows.Forms.Button()
		Me.btnResetRelease = New System.Windows.Forms.Button()
		Me.btnEnableDebug = New System.Windows.Forms.Button()
		Me.btn_DeployEnable = New System.Windows.Forms.Button()
		Me.btnBatch = New System.Windows.Forms.Button()
		Me.Label17 = New System.Windows.Forms.Label()
		Me.btn_CheckDeploy = New System.Windows.Forms.Button()
		Me.btn_ToggleUSB = New System.Windows.Forms.Button()
		Me.btnOpenLog = New System.Windows.Forms.Button()
		Me.DLL_Revision = New System.Windows.Forms.TextBox()
		Me.SRFN_CustomerValues = New System.Windows.Forms.TextBox()
		Me.SRFN_TestResults = New System.Windows.Forms.TextBox()
		Me.txtSQLServer = New System.Windows.Forms.TextBox()
		Me.chkShowForm = New System.Windows.Forms.CheckBox()
		Me.RA6ProgPath = New System.Windows.Forms.TextBox()
		Me.btnConnect = New System.Windows.Forms.Button()
		Me.btnCheckRA6ProgPath = New System.Windows.Forms.Button()
		Me.btnErase = New System.Windows.Forms.Button()
		Me.btnUpdateFW = New System.Windows.Forms.Button()
		Me.txtdebug = New System.Windows.Forms.TextBox()
		Me.btnCheckDLLString = New System.Windows.Forms.Button()
		Me.btnCheckCustomerString = New System.Windows.Forms.Button()
		Me.btnCheckResultString = New System.Windows.Forms.Button()
		Me.BTN_RA6ConfigVals = New System.Windows.Forms.Button()
		Me.btnSaveSQL = New System.Windows.Forms.Button()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.mnuMain.SuspendLayout
		Me.Panel1.SuspendLayout
		Me.SuspendLayout
		'
		'btnReset
		'
		Me.btnReset.Enabled = false
		Me.btnReset.Location = New System.Drawing.Point(496, 58)
		Me.btnReset.Name = "btnReset"
		Me.btnReset.Size = New System.Drawing.Size(90, 23)
		Me.btnReset.TabIndex = 3
		Me.btnReset.Text = "Send VarParam"
		Me.btnReset.UseVisualStyleBackColor = true
		'
		'mnuMain
		'
		Me.mnuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuConfigure, Me.lblPortStatus})
		Me.mnuMain.Location = New System.Drawing.Point(0, 0)
		Me.mnuMain.Name = "mnuMain"
		Me.mnuMain.Size = New System.Drawing.Size(616, 24)
		Me.mnuMain.TabIndex = 250
		'
		'mnuConfigure
		'
		Me.mnuConfigure.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSQLConfig, Me.mnuCommPorts, Me.mnuUploadFirmware})
		Me.mnuConfigure.Name = "mnuConfigure"
		Me.mnuConfigure.Size = New System.Drawing.Size(72, 20)
		Me.mnuConfigure.Text = "Configure"
		'
		'mnuSQLConfig
		'
		Me.mnuSQLConfig.Name = "mnuSQLConfig"
		Me.mnuSQLConfig.Size = New System.Drawing.Size(141, 22)
		Me.mnuSQLConfig.Text = "SQL Config"
		'
		'mnuCommPorts
		'
		Me.mnuCommPorts.Name = "mnuCommPorts"
		Me.mnuCommPorts.Size = New System.Drawing.Size(141, 22)
		Me.mnuCommPorts.Text = "Comm Ports"
		'
		'mnuUploadFirmware
		'
		Me.mnuUploadFirmware.Name = "mnuUploadFirmware"
		Me.mnuUploadFirmware.Size = New System.Drawing.Size(141, 22)
		Me.mnuUploadFirmware.Text = "Firmware"
		'
		'lblPortStatus
		'
		Me.lblPortStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
		Me.lblPortStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Bold)
		Me.lblPortStatus.Margin = New System.Windows.Forms.Padding(0, 1, 31, 2)
		Me.lblPortStatus.Name = "lblPortStatus"
		Me.lblPortStatus.Size = New System.Drawing.Size(183, 17)
		Me.lblPortStatus.Text = "Comm: -- | Debug: -- | Relay: -- "
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.txtRFCarrier)
		Me.Panel1.Controls.Add(Me.txtRelayPort)
		Me.Panel1.Controls.Add(Me.lblRelayPort)
		Me.Panel1.Controls.Add(Me.txtDebugPort)
		Me.Panel1.Controls.Add(Me.Label13)
		Me.Panel1.Controls.Add(Me.btnUpdatePort)
		Me.Panel1.Controls.Add(Me.txtCommPort)
		Me.Panel1.Controls.Add(Me.btnLockForm)
		Me.Panel1.Controls.Add(Me.txtFormID)
		Me.Panel1.Controls.Add(Me.txtProductFamily)
		Me.Panel1.Controls.Add(Me.chkRDInstalled)
		Me.Panel1.Controls.Add(Me.Label2)
		Me.Panel1.Controls.Add(Me.txtPackPath)
		Me.Panel1.Controls.Add(Me.btnGetCustomerName)
		Me.Panel1.Controls.Add(Me.Label1)
		Me.Panel1.Controls.Add(Me.Label18)
		Me.Panel1.Controls.Add(Me.txtUtilitySerial)
		Me.Panel1.Controls.Add(Me.txtCustomerID)
		Me.Panel1.Controls.Add(Me.Label12)
		Me.Panel1.Controls.Add(Me.txtMeterSerial)
		Me.Panel1.Controls.Add(Me.btnReset)
		Me.Panel1.Controls.Add(Me.Label11)
		Me.Panel1.Controls.Add(Me.txtMacAddress)
		Me.Panel1.Controls.Add(Me.Label7)
		Me.Panel1.Controls.Add(Me.Label8)
		Me.Panel1.Controls.Add(Me.Label10)
		Me.Panel1.Controls.Add(Me.FormID)
		Me.Panel1.Controls.Add(Me.btnSave)
		Me.Panel1.Location = New System.Drawing.Point(12, 484)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(592, 186)
		Me.Panel1.TabIndex = 58
		'
		'txtRFCarrier
		'
		Me.txtRFCarrier.FormattingEnabled = true
		Me.txtRFCarrier.Location = New System.Drawing.Point(235, 57)
		Me.txtRFCarrier.Name = "txtRFCarrier"
		Me.txtRFCarrier.Size = New System.Drawing.Size(122, 21)
		Me.txtRFCarrier.TabIndex = 115
		'
		'txtRelayPort
		'
		Me.txtRelayPort.FormattingEnabled = true
		Me.txtRelayPort.Location = New System.Drawing.Point(539, 18)
		Me.txtRelayPort.Name = "txtRelayPort"
		Me.txtRelayPort.Size = New System.Drawing.Size(47, 21)
		Me.txtRelayPort.TabIndex = 248
		Me.txtRelayPort.Visible = false
		'
		'lblRelayPort
		'
		Me.lblRelayPort.AutoSize = true
		Me.lblRelayPort.BackColor = System.Drawing.Color.FromArgb(CType(CType(128,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
		Me.lblRelayPort.Location = New System.Drawing.Point(533, 4)
		Me.lblRelayPort.Name = "lblRelayPort"
		Me.lblRelayPort.Size = New System.Drawing.Size(56, 13)
		Me.lblRelayPort.TabIndex = 249
		Me.lblRelayPort.Text = "Relay Port"
		Me.lblRelayPort.Visible = false
		'
		'txtDebugPort
		'
		Me.txtDebugPort.Enabled = false
		Me.txtDebugPort.FormattingEnabled = true
		Me.txtDebugPort.Location = New System.Drawing.Point(477, 18)
		Me.txtDebugPort.Name = "txtDebugPort"
		Me.txtDebugPort.Size = New System.Drawing.Size(47, 21)
		Me.txtDebugPort.TabIndex = 103
		Me.txtDebugPort.Visible = false
		'
		'Label13
		'
		Me.Label13.AutoSize = true
		Me.Label13.BackColor = System.Drawing.Color.FromArgb(CType(CType(128,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
		Me.Label13.Location = New System.Drawing.Point(471, 4)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(61, 13)
		Me.Label13.TabIndex = 102
		Me.Label13.Text = "Debug Port"
		Me.Label13.Visible = false
		'
		'btnUpdatePort
		'
		Me.btnUpdatePort.Location = New System.Drawing.Point(357, 16)
		Me.btnUpdatePort.Name = "btnUpdatePort"
		Me.btnUpdatePort.Size = New System.Drawing.Size(52, 23)
		Me.btnUpdatePort.TabIndex = 101
		Me.btnUpdatePort.Text = "Update"
		Me.btnUpdatePort.UseVisualStyleBackColor = true
		Me.btnUpdatePort.Visible = false
		'
		'txtCommPort
		'
		Me.txtCommPort.FormattingEnabled = true
		Me.txtCommPort.Location = New System.Drawing.Point(415, 16)
		Me.txtCommPort.Name = "txtCommPort"
		Me.txtCommPort.Size = New System.Drawing.Size(47, 21)
		Me.txtCommPort.TabIndex = 100
		Me.txtCommPort.Visible = false
		'
		'btnLockForm
		'
		Me.btnLockForm.BackColor = System.Drawing.Color.LightGreen
		Me.btnLockForm.Location = New System.Drawing.Point(496, 87)
		Me.btnLockForm.Name = "btnLockForm"
		Me.btnLockForm.Size = New System.Drawing.Size(90, 23)
		Me.btnLockForm.TabIndex = 99
		Me.btnLockForm.Text = "Lock Form"
		Me.btnLockForm.UseVisualStyleBackColor = false
		'
		'txtFormID
		'
		Me.txtFormID.Enabled = false
		Me.txtFormID.FormattingEnabled = true
		Me.txtFormID.Items.AddRange(New Object() {"SRFN-I-210+", "SRFN-KV2c"})
		Me.txtFormID.Location = New System.Drawing.Point(132, 93)
		Me.txtFormID.Name = "txtFormID"
		Me.txtFormID.Size = New System.Drawing.Size(157, 21)
		Me.txtFormID.TabIndex = 98
		'
		'txtProductFamily
		'
		Me.txtProductFamily.FormattingEnabled = true
		Me.txtProductFamily.Location = New System.Drawing.Point(132, 18)
		Me.txtProductFamily.Name = "txtProductFamily"
		Me.txtProductFamily.Size = New System.Drawing.Size(141, 21)
		Me.txtProductFamily.TabIndex = 94
		'
		'chkRDInstalled
		'
		Me.chkRDInstalled.AutoSize = true
		Me.chkRDInstalled.Location = New System.Drawing.Point(361, 96)
		Me.chkRDInstalled.Name = "chkRDInstalled"
		Me.chkRDInstalled.Size = New System.Drawing.Size(84, 17)
		Me.chkRDInstalled.TabIndex = 79
		Me.chkRDInstalled.Text = "RD Installed"
		Me.chkRDInstalled.UseVisualStyleBackColor = true
		'
		'Label2
		'
		Me.Label2.AutoSize = true
		Me.Label2.BackColor = System.Drawing.Color.FromArgb(CType(CType(128,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
		Me.Label2.Location = New System.Drawing.Point(409, 2)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(58, 13)
		Me.Label2.TabIndex = 75
		Me.Label2.Text = "Comm Port"
		Me.Label2.Visible = false
		'
		'txtPackPath
		'
		Me.txtPackPath.Location = New System.Drawing.Point(6, 134)
		Me.txtPackPath.Name = "txtPackPath"
		Me.txtPackPath.Size = New System.Drawing.Size(183, 20)
		Me.txtPackPath.TabIndex = 74
		Me.txtPackPath.Text = "c:\pack"
		'
		'btnGetCustomerName
		'
		Me.btnGetCustomerName.BackColor = System.Drawing.SystemColors.Control
		Me.btnGetCustomerName.Location = New System.Drawing.Point(496, 143)
		Me.btnGetCustomerName.Name = "btnGetCustomerName"
		Me.btnGetCustomerName.Size = New System.Drawing.Size(90, 23)
		Me.btnGetCustomerName.TabIndex = 96
		Me.btnGetCustomerName.Text = "Check Script"
		Me.btnGetCustomerName.UseVisualStyleBackColor = false
		'
		'Label1
		'
		Me.Label1.AutoSize = true
		Me.Label1.Location = New System.Drawing.Point(4, 120)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(54, 13)
		Me.Label1.TabIndex = 73
		Me.Label1.Text = "PackPath"
		'
		'Label18
		'
		Me.Label18.AutoSize = true
		Me.Label18.Location = New System.Drawing.Point(5, 80)
		Me.Label18.Name = "Label18"
		Me.Label18.Size = New System.Drawing.Size(101, 13)
		Me.Label18.TabIndex = 70
		Me.Label18.Text = "Utility Serial Number"
		'
		'txtUtilitySerial
		'
		Me.txtUtilitySerial.Location = New System.Drawing.Point(6, 94)
		Me.txtUtilitySerial.Name = "txtUtilitySerial"
		Me.txtUtilitySerial.Size = New System.Drawing.Size(122, 20)
		Me.txtUtilitySerial.TabIndex = 69
		'
		'txtCustomerID
		'
		Me.txtCustomerID.Location = New System.Drawing.Point(133, 58)
		Me.txtCustomerID.Name = "txtCustomerID"
		Me.txtCustomerID.Size = New System.Drawing.Size(96, 20)
		Me.txtCustomerID.TabIndex = 66
		'
		'Label12
		'
		Me.Label12.AutoSize = true
		Me.Label12.Location = New System.Drawing.Point(4, 4)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(103, 13)
		Me.Label12.TabIndex = 65
		Me.Label12.Text = "Meter Serial Number"
		'
		'txtMeterSerial
		'
		Me.txtMeterSerial.Location = New System.Drawing.Point(6, 18)
		Me.txtMeterSerial.Name = "txtMeterSerial"
		Me.txtMeterSerial.Size = New System.Drawing.Size(122, 20)
		Me.txtMeterSerial.TabIndex = 62
		'
		'Label11
		'
		Me.Label11.AutoSize = true
		Me.Label11.Location = New System.Drawing.Point(3, 44)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(71, 13)
		Me.Label11.TabIndex = 63
		Me.Label11.Text = "MAC Address"
		'
		'txtMacAddress
		'
		Me.txtMacAddress.Location = New System.Drawing.Point(6, 58)
		Me.txtMacAddress.Name = "txtMacAddress"
		Me.txtMacAddress.Size = New System.Drawing.Size(122, 20)
		Me.txtMacAddress.TabIndex = 64
		'
		'Label7
		'
		Me.Label7.AutoSize = true
		Me.Label7.Location = New System.Drawing.Point(132, 44)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(65, 13)
		Me.Label7.TabIndex = 61
		Me.Label7.Text = "Customer ID"
		'
		'Label8
		'
		Me.Label8.AutoSize = true
		Me.Label8.Location = New System.Drawing.Point(232, 44)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(54, 13)
		Me.Label8.TabIndex = 60
		Me.Label8.Text = "RF Carrier"
		'
		'Label10
		'
		Me.Label10.AutoSize = true
		Me.Label10.Location = New System.Drawing.Point(130, 4)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(76, 13)
		Me.Label10.TabIndex = 58
		Me.Label10.Text = "Product Family"
		'
		'FormID
		'
		Me.FormID.AutoSize = true
		Me.FormID.Location = New System.Drawing.Point(130, 79)
		Me.FormID.Name = "FormID"
		Me.FormID.Size = New System.Drawing.Size(44, 13)
		Me.FormID.TabIndex = 59
		Me.FormID.Text = "Form ID"
		'
		'btnSave
		'
		Me.btnSave.Location = New System.Drawing.Point(496, 116)
		Me.btnSave.Name = "btnSave"
		Me.btnSave.Size = New System.Drawing.Size(90, 23)
		Me.btnSave.TabIndex = 59
		Me.btnSave.Text = "Save All"
		Me.btnSave.UseVisualStyleBackColor = true
		'
		'lblDLLVer
		'
		Me.lblDLLVer.AutoSize = true
		Me.lblDLLVer.Location = New System.Drawing.Point(577, 32)
		Me.lblDLLVer.Name = "lblDLLVer"
		Me.lblDLLVer.Size = New System.Drawing.Size(7, 13)
		Me.lblDLLVer.TabIndex = 76
		Me.lblDLLVer.Text = ""&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)
		Me.lblDLLVer.Visible = false
		'
		'txtPassword
		'
		Me.txtPassword.Location = New System.Drawing.Point(560, 44)
		Me.txtPassword.Name = "txtPassword"
		Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
		Me.txtPassword.Size = New System.Drawing.Size(44, 20)
		Me.txtPassword.TabIndex = 77
		Me.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
		'
		'btnReadMacAddress
		'
		Me.btnReadMacAddress.Location = New System.Drawing.Point(12, 309)
		Me.btnReadMacAddress.Name = "btnReadMacAddress"
		Me.btnReadMacAddress.Size = New System.Drawing.Size(90, 23)
		Me.btnReadMacAddress.TabIndex = 80
		Me.btnReadMacAddress.Text = "Read Mac"
		Me.btnReadMacAddress.UseVisualStyleBackColor = true
		'
		'btnVerifyComm
		'
		Me.btnVerifyComm.Location = New System.Drawing.Point(12, 367)
		Me.btnVerifyComm.Name = "btnVerifyComm"
		Me.btnVerifyComm.Size = New System.Drawing.Size(90, 23)
		Me.btnVerifyComm.TabIndex = 247
		Me.btnVerifyComm.Text = "Verify Comm"
		Me.btnVerifyComm.UseVisualStyleBackColor = True
		'
		'txtResults
		'
		Me.txtResults.Location = New System.Drawing.Point(12, 78)
		Me.txtResults.Multiline = true
		Me.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.txtResults.Name = "txtResults"
		Me.txtResults.Size = New System.Drawing.Size(592, 229)
		Me.txtResults.TabIndex = 81
		'
		'btnReadFirmware
		'
		Me.btnReadFirmware.Location = New System.Drawing.Point(12, 338)
		Me.btnReadFirmware.Name = "btnReadFirmware"
		Me.btnReadFirmware.Size = New System.Drawing.Size(90, 23)
		Me.btnReadFirmware.TabIndex = 82
		Me.btnReadFirmware.Text = "Read Firmware"
		Me.btnReadFirmware.UseVisualStyleBackColor = true
		'
		'btnquietON
		'
		Me.btnquietON.Location = New System.Drawing.Point(302, 313)
		Me.btnquietON.Name = "btnquietON"
		Me.btnquietON.Size = New System.Drawing.Size(90, 23)
		Me.btnquietON.TabIndex = 83
		Me.btnquietON.Text = "Quiet ON"
		Me.btnquietON.UseVisualStyleBackColor = true
		'
		'btnquietOFF
		'
		Me.btnquietOFF.Location = New System.Drawing.Point(302, 342)
		Me.btnquietOFF.Name = "btnquietOFF"
		Me.btnquietOFF.Size = New System.Drawing.Size(90, 23)
		Me.btnquietOFF.TabIndex = 84
		Me.btnquietOFF.Text = "Quiet OFF"
		Me.btnquietOFF.UseVisualStyleBackColor = true
		'
		'btnSendcommand
		'
		Me.btnSendcommand.Location = New System.Drawing.Point(12, 37)
		Me.btnSendcommand.Name = "btnSendcommand"
		Me.btnSendcommand.Size = New System.Drawing.Size(90, 23)
		Me.btnSendcommand.TabIndex = 85
		Me.btnSendcommand.Text = "Send Command"
		Me.btnSendcommand.UseVisualStyleBackColor = true
		'
		'txtcommand
		'
		Me.txtcommand.Enabled = false
		Me.txtcommand.Location = New System.Drawing.Point(116, 40)
		Me.txtcommand.Name = "txtcommand"
		Me.txtcommand.Size = New System.Drawing.Size(266, 20)
		Me.txtcommand.TabIndex = 86
		Me.txtcommand.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
		'
		'Label5
		'
		Me.Label5.AutoSize = true
		Me.Label5.Location = New System.Drawing.Point(113, 26)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(148, 13)
		Me.Label5.TabIndex = 87
		Me.Label5.Text = "Send Manual Command (msg)"
		'
		'btnGetHash
		'
		Me.btnGetHash.Location = New System.Drawing.Point(508, 41)
		Me.btnGetHash.Name = "btnGetHash"
		Me.btnGetHash.Size = New System.Drawing.Size(46, 23)
		Me.btnGetHash.TabIndex = 88
		Me.btnGetHash.Text = "Hash"
		Me.btnGetHash.UseVisualStyleBackColor = true
		Me.btnGetHash.Visible = false
		'
		'btnquietStatus
		'
		Me.btnquietStatus.Location = New System.Drawing.Point(302, 371)
		Me.btnquietStatus.Name = "btnquietStatus"
		Me.btnquietStatus.Size = New System.Drawing.Size(90, 23)
		Me.btnquietStatus.TabIndex = 89
		Me.btnquietStatus.Text = "Chk QuietMode"
		Me.btnquietStatus.UseVisualStyleBackColor = true
		'
		'btnChkPort
		'
		Me.btnChkPort.Location = New System.Drawing.Point(206, 313)
		Me.btnChkPort.Name = "btnChkPort"
		Me.btnChkPort.Size = New System.Drawing.Size(90, 23)
		Me.btnChkPort.TabIndex = 90
		Me.btnChkPort.Text = "Check Port"
		Me.btnChkPort.UseVisualStyleBackColor = true
		'
		'Label9
		'
		Me.Label9.AutoSize = true
		Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		Me.Label9.Location = New System.Drawing.Point(113, 63)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(183, 12)
		Me.Label9.TabIndex = 91
		Me.Label9.Text = "CommManager2.SendMessage(msg, port)"
		'
		'btnOpticalLogon
		'
		Me.btnOpticalLogon.Enabled = false
		Me.btnOpticalLogon.Location = New System.Drawing.Point(108, 311)
		Me.btnOpticalLogon.Name = "btnOpticalLogon"
		Me.btnOpticalLogon.Size = New System.Drawing.Size(90, 23)
		Me.btnOpticalLogon.TabIndex = 92
		Me.btnOpticalLogon.Text = "Optical Logon"
		Me.btnOpticalLogon.UseVisualStyleBackColor = true
		'
		'btnOpticalLogoff
		'
		Me.btnOpticalLogoff.Enabled = false
		Me.btnOpticalLogoff.Location = New System.Drawing.Point(108, 339)
		Me.btnOpticalLogoff.Name = "btnOpticalLogoff"
		Me.btnOpticalLogoff.Size = New System.Drawing.Size(90, 23)
		Me.btnOpticalLogoff.TabIndex = 93
		Me.btnOpticalLogoff.Text = "Optical Logoff"
		Me.btnOpticalLogoff.UseVisualStyleBackColor = true
		'
		'btnReadEnergy
		'
		Me.btnReadEnergy.Location = New System.Drawing.Point(12, 423)
		Me.btnReadEnergy.Name = "btnReadEnergy"
		Me.btnReadEnergy.Size = New System.Drawing.Size(90, 23)
		Me.btnReadEnergy.TabIndex = 94
		Me.btnReadEnergy.Text = "Read Energy"
		Me.btnReadEnergy.UseVisualStyleBackColor = true
		'
		'btnEnableDTLS
		'
		Me.btnEnableDTLS.Enabled = false
		Me.btnEnableDTLS.Location = New System.Drawing.Point(108, 394)
		Me.btnEnableDTLS.Name = "btnEnableDTLS"
		Me.btnEnableDTLS.Size = New System.Drawing.Size(90, 23)
		Me.btnEnableDTLS.TabIndex = 97
		Me.btnEnableDTLS.Text = "Enable DTLS"
		Me.btnEnableDTLS.UseVisualStyleBackColor = true
		'
		'btnDisableDTLS
		'
		Me.btnDisableDTLS.Location = New System.Drawing.Point(108, 423)
		Me.btnDisableDTLS.Name = "btnDisableDTLS"
		Me.btnDisableDTLS.Size = New System.Drawing.Size(90, 23)
		Me.btnDisableDTLS.TabIndex = 98
		Me.btnDisableDTLS.Text = "Disable DTLS"
		Me.btnDisableDTLS.UseVisualStyleBackColor = true
		'
		'btnSendDebug
		'
		Me.btnSendDebug.Location = New System.Drawing.Point(447, 41)
		Me.btnSendDebug.Name = "btnSendDebug"
		Me.btnSendDebug.Size = New System.Drawing.Size(55, 22)
		Me.btnSendDebug.TabIndex = 101
		Me.btnSendDebug.Text = "Debug"
		Me.btnSendDebug.UseVisualStyleBackColor = true
		Me.btnSendDebug.Visible = false
		'
		'btn_edInfo
		'
		Me.btn_edInfo.Location = New System.Drawing.Point(12, 452)
		Me.btn_edInfo.Name = "btn_edInfo"
		Me.btn_edInfo.Size = New System.Drawing.Size(90, 23)
		Me.btn_edInfo.TabIndex = 102
		Me.btn_edInfo.Text = "edInfo/Form"
		Me.btn_edInfo.UseVisualStyleBackColor = true
		'
		'btn_edfw
		'
		Me.btn_edfw.Location = New System.Drawing.Point(12, 394)
		Me.btn_edfw.Name = "btn_edfw"
		Me.btn_edfw.Size = New System.Drawing.Size(90, 23)
		Me.btn_edfw.TabIndex = 103
		Me.btn_edfw.Text = "edfwversion"
		Me.btn_edfw.UseVisualStyleBackColor = true
		'
		'btnResetRelease
		'
		Me.btnResetRelease.Location = New System.Drawing.Point(417, 399)
		Me.btnResetRelease.Name = "btnResetRelease"
		Me.btnResetRelease.Size = New System.Drawing.Size(90, 23)
		Me.btnResetRelease.TabIndex = 104
		Me.btnResetRelease.Text = "Reset Release"
		Me.btnResetRelease.UseVisualStyleBackColor = true
		'
		'btnEnableDebug
		'
		Me.btnEnableDebug.Location = New System.Drawing.Point(417, 429)
		Me.btnEnableDebug.Name = "btnEnableDebug"
		Me.btnEnableDebug.Size = New System.Drawing.Size(90, 23)
		Me.btnEnableDebug.TabIndex = 106
		Me.btnEnableDebug.Text = "Enable Debug"
		Me.btnEnableDebug.UseVisualStyleBackColor = true
		'
		'btn_DeployEnable
		'
		Me.btn_DeployEnable.Location = New System.Drawing.Point(514, 400)
		Me.btn_DeployEnable.Name = "btn_DeployEnable"
		Me.btn_DeployEnable.Size = New System.Drawing.Size(90, 23)
		Me.btn_DeployEnable.TabIndex = 109
		Me.btn_DeployEnable.Text = "Deploy Enable"
		Me.btn_DeployEnable.UseVisualStyleBackColor = true
		'
		'btnBatch
		'
		Me.btnBatch.Enabled = false
		Me.btnBatch.Location = New System.Drawing.Point(514, 429)
		Me.btnBatch.Name = "btnBatch"
		Me.btnBatch.Size = New System.Drawing.Size(90, 23)
		Me.btnBatch.TabIndex = 114
		Me.btnBatch.Text = "Batch"
		Me.btnBatch.UseVisualStyleBackColor = true
		'
		'Label17
		'
		Me.Label17.AutoSize = true
		Me.Label17.Location = New System.Drawing.Point(280, 26)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New System.Drawing.Size(122, 13)
		Me.Label17.TabIndex = 244
		Me.Label17.Text = "Optional:  | delay timeout"
		'
		'btn_CheckDeploy
		'
		Me.btn_CheckDeploy.Location = New System.Drawing.Point(514, 371)
		Me.btn_CheckDeploy.Name = "btn_CheckDeploy"
		Me.btn_CheckDeploy.Size = New System.Drawing.Size(90, 23)
		Me.btn_CheckDeploy.TabIndex = 245
		Me.btn_CheckDeploy.Text = "Check Deploy"
		Me.btn_CheckDeploy.UseVisualStyleBackColor = true
		'
		'btn_ToggleUSB
		'
		Me.btn_ToggleUSB.BackColor = System.Drawing.Color.Yellow
		Me.btn_ToggleUSB.Location = New System.Drawing.Point(206, 342)
		Me.btn_ToggleUSB.Name = "btn_ToggleUSB"
		Me.btn_ToggleUSB.Size = New System.Drawing.Size(90, 23)
		Me.btn_ToggleUSB.TabIndex = 246
		Me.btn_ToggleUSB.Text = "USB OFF"
		Me.btn_ToggleUSB.UseVisualStyleBackColor = false
		'
		'btnOpenLog
		'
		Me.btnOpenLog.BackColor = System.Drawing.Color.LightYellow
		Me.btnOpenLog.Location = New System.Drawing.Point(514, 455)
		Me.btnOpenLog.Name = "btnOpenLog"
		Me.btnOpenLog.Size = New System.Drawing.Size(90, 23)
		Me.btnOpenLog.TabIndex = 251
		Me.btnOpenLog.Text = "Log Folder"
		Me.btnOpenLog.UseVisualStyleBackColor = false
		Me.btnOpenLog.Visible = false
		'
		'DLL_Revision
		'
		Me.DLL_Revision.Location = New System.Drawing.Point(0, 0)
		Me.DLL_Revision.Name = "DLL_Revision"
		Me.DLL_Revision.Size = New System.Drawing.Size(100, 20)
		Me.DLL_Revision.TabIndex = 252
		Me.DLL_Revision.Visible = false
		'
		'SRFN_CustomerValues
		'
		Me.SRFN_CustomerValues.Location = New System.Drawing.Point(0, 0)
		Me.SRFN_CustomerValues.Name = "SRFN_CustomerValues"
		Me.SRFN_CustomerValues.Size = New System.Drawing.Size(100, 20)
		Me.SRFN_CustomerValues.TabIndex = 253
		Me.SRFN_CustomerValues.Visible = false
		'
		'SRFN_TestResults
		'
		Me.SRFN_TestResults.Location = New System.Drawing.Point(0, 0)
		Me.SRFN_TestResults.Name = "SRFN_TestResults"
		Me.SRFN_TestResults.Size = New System.Drawing.Size(100, 20)
		Me.SRFN_TestResults.TabIndex = 254
		Me.SRFN_TestResults.Visible = false
		'
		'txtSQLServer
		'
		Me.txtSQLServer.Location = New System.Drawing.Point(0, 0)
		Me.txtSQLServer.Name = "txtSQLServer"
		Me.txtSQLServer.Size = New System.Drawing.Size(100, 20)
		Me.txtSQLServer.TabIndex = 255
		Me.txtSQLServer.Visible = false
		'
		'chkShowForm
		'
		Me.chkShowForm.Location = New System.Drawing.Point(0, 4)
		Me.chkShowForm.Name = "chkShowForm"
		Me.chkShowForm.Size = New System.Drawing.Size(104, 24)
		Me.chkShowForm.TabIndex = 256
		Me.chkShowForm.Visible = false
		'
		'RA6ProgPath
		'
		Me.RA6ProgPath.Location = New System.Drawing.Point(0, 0)
		Me.RA6ProgPath.Name = "RA6ProgPath"
		Me.RA6ProgPath.Size = New System.Drawing.Size(100, 20)
		Me.RA6ProgPath.TabIndex = 257
		Me.RA6ProgPath.Visible = false
		'
		'btnConnect
		'
		Me.btnConnect.Location = New System.Drawing.Point(0, 0)
		Me.btnConnect.Name = "btnConnect"
		Me.btnConnect.Size = New System.Drawing.Size(75, 23)
		Me.btnConnect.TabIndex = 258
		Me.btnConnect.Visible = false
		'
		'btnCheckRA6ProgPath
		'
		Me.btnCheckRA6ProgPath.Location = New System.Drawing.Point(0, 0)
		Me.btnCheckRA6ProgPath.Name = "btnCheckRA6ProgPath"
		Me.btnCheckRA6ProgPath.Size = New System.Drawing.Size(75, 23)
		Me.btnCheckRA6ProgPath.TabIndex = 259
		Me.btnCheckRA6ProgPath.Visible = false
		'
		'btnErase
		'
		Me.btnErase.Location = New System.Drawing.Point(0, 0)
		Me.btnErase.Name = "btnErase"
		Me.btnErase.Size = New System.Drawing.Size(75, 23)
		Me.btnErase.TabIndex = 260
		Me.btnErase.Visible = false
		'
		'btnUpdateFW
		'
		Me.btnUpdateFW.Location = New System.Drawing.Point(0, 0)
		Me.btnUpdateFW.Name = "btnUpdateFW"
		Me.btnUpdateFW.Size = New System.Drawing.Size(75, 23)
		Me.btnUpdateFW.TabIndex = 261
		Me.btnUpdateFW.Visible = false
		'
		'txtdebug
		'
		Me.txtdebug.Location = New System.Drawing.Point(0, 0)
		Me.txtdebug.Name = "txtdebug"
		Me.txtdebug.Size = New System.Drawing.Size(100, 20)
		Me.txtdebug.TabIndex = 262
		Me.txtdebug.Visible = false
		'
		'btnCheckDLLString
		'
		Me.btnCheckDLLString.Location = New System.Drawing.Point(0, 0)
		Me.btnCheckDLLString.Name = "btnCheckDLLString"
		Me.btnCheckDLLString.Size = New System.Drawing.Size(75, 23)
		Me.btnCheckDLLString.TabIndex = 263
		Me.btnCheckDLLString.Visible = false
		'
		'btnCheckCustomerString
		'
		Me.btnCheckCustomerString.Location = New System.Drawing.Point(0, 0)
		Me.btnCheckCustomerString.Name = "btnCheckCustomerString"
		Me.btnCheckCustomerString.Size = New System.Drawing.Size(75, 23)
		Me.btnCheckCustomerString.TabIndex = 264
		Me.btnCheckCustomerString.Visible = false
		'
		'btnCheckResultString
		'
		Me.btnCheckResultString.Location = New System.Drawing.Point(0, 0)
		Me.btnCheckResultString.Name = "btnCheckResultString"
		Me.btnCheckResultString.Size = New System.Drawing.Size(75, 23)
		Me.btnCheckResultString.TabIndex = 265
		Me.btnCheckResultString.Visible = false
		'
		'BTN_RA6ConfigVals
		'
		Me.BTN_RA6ConfigVals.Location = New System.Drawing.Point(0, 0)
		Me.BTN_RA6ConfigVals.Name = "BTN_RA6ConfigVals"
		Me.BTN_RA6ConfigVals.Size = New System.Drawing.Size(75, 23)
		Me.BTN_RA6ConfigVals.TabIndex = 266
		Me.BTN_RA6ConfigVals.Visible = false
		'
		'btnSaveSQL
		'
		Me.btnSaveSQL.Location = New System.Drawing.Point(0, 0)
		Me.btnSaveSQL.Name = "btnSaveSQL"
		Me.btnSaveSQL.Size = New System.Drawing.Size(75, 23)
		Me.btnSaveSQL.TabIndex = 267
		Me.btnSaveSQL.Visible = false
		'
		'Label4
		'
		Me.Label4.AutoSize = true
		Me.Label4.Location = New System.Drawing.Point(557, 28)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(47, 13)
		Me.Label4.TabIndex = 78
		Me.Label4.Text = "Set SQL"
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(616, 678)
		Me.Controls.Add(Me.btn_ToggleUSB)
		Me.Controls.Add(Me.btn_CheckDeploy)
		Me.Controls.Add(Me.Label17)
		Me.Controls.Add(Me.mnuMain)

		Me.Controls.Add(Me.btnBatch)
		Me.Controls.Add(Me.btn_DeployEnable)
		Me.Controls.Add(Me.btnEnableDebug)
		Me.Controls.Add(Me.btnResetRelease)
		Me.Controls.Add(Me.btn_edfw)
		Me.Controls.Add(Me.btn_edInfo)
		Me.Controls.Add(Me.btnSendDebug)
		Me.Controls.Add(Me.btnDisableDTLS)
		Me.Controls.Add(Me.btnEnableDTLS)
		Me.Controls.Add(Me.btnReadEnergy)
		Me.Controls.Add(Me.btnOpticalLogoff)
		Me.Controls.Add(Me.btnOpticalLogon)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.btnChkPort)
		Me.Controls.Add(Me.btnquietStatus)
		Me.Controls.Add(Me.btnGetHash)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.txtcommand)
		Me.Controls.Add(Me.btnSendcommand)
		Me.Controls.Add(Me.btnquietOFF)
		Me.Controls.Add(Me.btnquietON)
		Me.Controls.Add(Me.btnReadFirmware)
		Me.Controls.Add(Me.txtResults)
		Me.Controls.Add(Me.btnOpenLog)
		Me.Controls.Add(Me.btnReadMacAddress)
		Me.Controls.Add(Me.btnVerifyComm)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.txtPassword)
		Me.Controls.Add(Me.lblDLLVer)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me.DLL_Revision)
		Me.Controls.Add(Me.SRFN_CustomerValues)
		Me.Controls.Add(Me.SRFN_TestResults)
		Me.Controls.Add(Me.txtSQLServer)
		Me.Controls.Add(Me.chkShowForm)
		Me.Controls.Add(Me.RA6ProgPath)
		Me.Controls.Add(Me.btnConnect)
		Me.Controls.Add(Me.btnCheckRA6ProgPath)
		Me.Controls.Add(Me.btnErase)
		Me.Controls.Add(Me.btnUpdateFW)
		Me.Controls.Add(Me.txtdebug)
		Me.Controls.Add(Me.btnCheckDLLString)
		Me.Controls.Add(Me.btnCheckCustomerString)
		Me.Controls.Add(Me.btnCheckResultString)
		Me.Controls.Add(Me.BTN_RA6ConfigVals)
		Me.Controls.Add(Me.btnSaveSQL)
		Me.MainMenuStrip = Me.mnuMain
		Me.Name = "Form1"
		Me.Text = "Form1"
		Me.mnuMain.ResumeLayout(false)
		Me.mnuMain.PerformLayout
		Me.Panel1.ResumeLayout(false)
		Me.Panel1.PerformLayout
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub

    Friend WithEvents mnuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents lblPortStatus As System.Windows.Forms.ToolStripLabel
    Friend WithEvents mnuConfigure As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSQLConfig As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCommPorts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUploadFirmware As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnReset As Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents txtPackPath As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents Label18 As System.Windows.Forms.Label
    Private WithEvents txtUtilitySerial As System.Windows.Forms.TextBox
    Private WithEvents txtCustomerID As System.Windows.Forms.TextBox
    Private WithEvents Label12 As System.Windows.Forms.Label
    Private WithEvents txtMeterSerial As System.Windows.Forms.TextBox
    Private WithEvents Label11 As System.Windows.Forms.Label
    Private WithEvents txtMacAddress As System.Windows.Forms.TextBox
    Private WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents FormID As System.Windows.Forms.Label
    Private WithEvents lblDLLVer As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents btnReadMacAddress As System.Windows.Forms.Button
    Friend WithEvents btnVerifyComm As System.Windows.Forms.Button
    Private WithEvents txtResults As System.Windows.Forms.TextBox
    Friend WithEvents btnReadFirmware As System.Windows.Forms.Button
    Friend WithEvents btnquietON As System.Windows.Forms.Button
    Friend WithEvents btnquietOFF As System.Windows.Forms.Button
    Friend WithEvents btnSendcommand As System.Windows.Forms.Button
    Private WithEvents txtcommand As System.Windows.Forms.TextBox
    Private WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnGetHash As System.Windows.Forms.Button
    Friend WithEvents btnquietStatus As System.Windows.Forms.Button
    Friend WithEvents btnChkPort As System.Windows.Forms.Button
    Private WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents chkRDInstalled As CheckBox
    Friend WithEvents btnOpticalLogon As Button
    Friend WithEvents btnOpticalLogoff As Button
    Friend WithEvents txtProductFamily As ComboBox
    Private WithEvents Label10 As Label
    Friend WithEvents btnGetCustomerName As Button
    Friend WithEvents btnReadEnergy As Button
    Friend WithEvents txtFormID As ComboBox
    Friend WithEvents btnLockForm As Button
    Friend WithEvents btnEnableDTLS As Button
    Friend WithEvents btnDisableDTLS As Button
    Friend WithEvents txtCommPort As ComboBox
    Friend WithEvents btnUpdatePort As Button
    Friend WithEvents txtRelayPort As ComboBox
    Friend WithEvents lblRelayPort As Label
    Friend WithEvents txtDebugPort As ComboBox
    Private WithEvents Label13 As Label
    Friend WithEvents btnSendDebug As Button
    Friend WithEvents btn_edInfo As Button
    Friend WithEvents btn_edfw As Button
    Friend WithEvents btnResetRelease As Button
    Friend WithEvents btnEnableDebug As Button
    Friend WithEvents btn_DeployEnable As Button
    Friend WithEvents txtRFCarrier As ComboBox

    Private WithEvents Label17 As Label
    Friend WithEvents btn_CheckDeploy As Button
	Friend WithEvents btn_ToggleUSB As Button
	Friend WithEvents btnOpenLog As Button
	Friend WithEvents btnBatch As Button
	Friend WithEvents DLL_Revision As System.Windows.Forms.TextBox
	Friend WithEvents SRFN_CustomerValues As System.Windows.Forms.TextBox
	Friend WithEvents SRFN_TestResults As System.Windows.Forms.TextBox
	Friend WithEvents txtSQLServer As System.Windows.Forms.TextBox
	Friend WithEvents chkShowForm As System.Windows.Forms.CheckBox
	Friend WithEvents RA6ProgPath As System.Windows.Forms.TextBox
	Friend WithEvents btnConnect As System.Windows.Forms.Button
	Friend WithEvents btnCheckRA6ProgPath As System.Windows.Forms.Button
	Friend WithEvents btnErase As System.Windows.Forms.Button
	Friend WithEvents btnUpdateFW As System.Windows.Forms.Button
	Friend WithEvents txtdebug As System.Windows.Forms.TextBox
	Friend WithEvents btnCheckDLLString As System.Windows.Forms.Button
	Friend WithEvents btnCheckCustomerString As System.Windows.Forms.Button
	Friend WithEvents btnCheckResultString As System.Windows.Forms.Button
	Friend WithEvents BTN_RA6ConfigVals As System.Windows.Forms.Button
	Friend WithEvents btnSaveSQL As System.Windows.Forms.Button
	Private WithEvents Label4 As Label
End Class

