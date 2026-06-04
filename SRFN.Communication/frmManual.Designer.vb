<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmManual
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.txtSetSQL = New System.Windows.Forms.TextBox()
        Me.btnSetSQL = New System.Windows.Forms.Button()
        Me.lblSetSQL = New System.Windows.Forms.Label()
        Me.txtHashVal = New System.Windows.Forms.TextBox()
        Me.chkManualScript = New System.Windows.Forms.CheckBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtUtilitySerial = New System.Windows.Forms.TextBox()
        Me.chkLock = New System.Windows.Forms.CheckBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.btnRunTest = New System.Windows.Forms.Button()
        Me.btnQuietON = New System.Windows.Forms.Button()
        Me.lblTestProgress = New System.Windows.Forms.Label()
        Me.rtbResult = New System.Windows.Forms.RichTextBox()
        Me.btnReadFirmwareManual = New System.Windows.Forms.Button()
        Me.btnReadMacAddress = New System.Windows.Forms.Button()
        Me.lblTestCommand = New System.Windows.Forms.Label()
        Me.lblMacScan = New System.Windows.Forms.Label()
        Me.cboMeterForm = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.groupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtBarcode = New System.Windows.Forms.TextBox()
        Me.rdoText = New System.Windows.Forms.RadioButton()
        Me.rdoHex = New System.Windows.Forms.RadioButton()
        Me.cboData = New System.Windows.Forms.ComboBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.cboStop = New System.Windows.Forms.ComboBox()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.cboParity = New System.Windows.Forms.ComboBox()
        Me.ChkHash = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboBaud = New System.Windows.Forms.ComboBox()
        Me.cboPort = New System.Windows.Forms.ComboBox()
        Me.txtProductFamily = New System.Windows.Forms.TextBox()
        Me.txtCustomerName = New System.Windows.Forms.TextBox()
        Me.txtCustomerID = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtMeterSerial = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtMacAddress = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtSend = New System.Windows.Forms.TextBox()
        Me.rtbDisplay = New System.Windows.Forms.RichTextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmdSend = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.prgTestProgress = New System.Windows.Forms.ProgressBar()
        Me.btnLoadTestScript = New System.Windows.Forms.Button()
        Me.cboTestFile = New System.Windows.Forms.ComboBox()
        Me.lblTestFile = New System.Windows.Forms.Label()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.grpRawOutput = New System.Windows.Forms.GroupBox()
        Me.grpTestResults = New System.Windows.Forms.GroupBox()
        Me.rtbTestResults = New System.Windows.Forms.RichTextBox()
        Me.btnResetTest = New System.Windows.Forms.Button()
        Me.btnClosePort = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.cmdReset = New System.Windows.Forms.Button()
        Me.groupBox2.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.grpRawOutput.SuspendLayout
        Me.grpTestResults.SuspendLayout
        Me.SuspendLayout
        '
        'txtHashVal
        '
        Me.txtHashVal.Location = New System.Drawing.Point(6, 166)
        Me.txtHashVal.Name = "txtHashVal"
        Me.txtHashVal.Size = New System.Drawing.Size(164, 20)
        Me.txtHashVal.TabIndex = 36
        '
        'chkManualScript
        '
        Me.chkManualScript.AutoSize = true
        Me.chkManualScript.Location = New System.Drawing.Point(213, 27)
        Me.chkManualScript.Name = "chkManualScript"
        Me.chkManualScript.Size = New System.Drawing.Size(114, 17)
        Me.chkManualScript.TabIndex = 34
        Me.chkManualScript.Text = "Run Manual Script"
        Me.chkManualScript.UseVisualStyleBackColor = true
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(5, 192)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(101, 13)
        Me.Label18.TabIndex = 33
        Me.Label18.Text = "Utility Serial Number"
        '
        'txtUtilitySerial
        '
        Me.txtUtilitySerial.Location = New System.Drawing.Point(6, 206)
        Me.txtUtilitySerial.Name = "txtUtilitySerial"
        Me.txtUtilitySerial.Size = New System.Drawing.Size(164, 20)
        Me.txtUtilitySerial.TabIndex = 32
        '
        'chkLock
        '
        Me.chkLock.AutoSize = true
        Me.chkLock.Location = New System.Drawing.Point(277, 236)
        Me.chkLock.Name = "chkLock"
        Me.chkLock.Size = New System.Drawing.Size(97, 17)
        Me.chkLock.TabIndex = 31
        Me.chkLock.Text = "Lock Selection"
        Me.chkLock.UseVisualStyleBackColor = true
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(4, 152)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(62, 13)
        Me.Label19.TabIndex = 35
        Me.Label19.Text = "Hash Value"
        '
        'btnRunTest
        '
        Me.btnRunTest.Enabled = false
        Me.btnRunTest.Location = New System.Drawing.Point(968, 185)
        Me.btnRunTest.Name = "btnRunTest"
        Me.btnRunTest.Size = New System.Drawing.Size(86, 23)
        Me.btnRunTest.TabIndex = 63
        Me.btnRunTest.Text = "Run Test"
        Me.btnRunTest.UseVisualStyleBackColor = true
        '
        'btnQuietON
        '
        Me.btnQuietON.Enabled = false
        Me.btnQuietON.Location = New System.Drawing.Point(968, 29)
        Me.btnQuietON.Name = "btnQuietON"
        Me.btnQuietON.Size = New System.Drawing.Size(86, 23)
        Me.btnQuietON.TabIndex = 62
        Me.btnQuietON.Text = "Quiet ON"
        Me.btnQuietON.UseVisualStyleBackColor = true
        '
        'lblTestProgress
        '
        Me.lblTestProgress.AutoSize = true
        Me.lblTestProgress.Location = New System.Drawing.Point(457, 270)
        Me.lblTestProgress.Name = "lblTestProgress"
        Me.lblTestProgress.Size = New System.Drawing.Size(0, 13)
        Me.lblTestProgress.TabIndex = 57
        '
        'rtbResult
        '
        Me.rtbResult.Location = New System.Drawing.Point(7, 17)
        Me.rtbResult.Name = "rtbResult"
        Me.rtbResult.Size = New System.Drawing.Size(267, 109)
        Me.rtbResult.TabIndex = 55
        Me.rtbResult.Text = ""
        Me.rtbResult.WordWrap = false
        '
        'btnReadFirmwareManual
        '
        Me.btnReadFirmwareManual.Enabled = false
        Me.btnReadFirmwareManual.Location = New System.Drawing.Point(968, 87)
        Me.btnReadFirmwareManual.Name = "btnReadFirmwareManual"
        Me.btnReadFirmwareManual.Size = New System.Drawing.Size(86, 23)
        Me.btnReadFirmwareManual.TabIndex = 48
        Me.btnReadFirmwareManual.Text = "Read Firmware"
        Me.btnReadFirmwareManual.UseVisualStyleBackColor = true
        '
        'btnReadMacAddress
        '
        Me.btnReadMacAddress.Enabled = false
        Me.btnReadMacAddress.Location = New System.Drawing.Point(968, 58)
        Me.btnReadMacAddress.Name = "btnReadMacAddress"
        Me.btnReadMacAddress.Size = New System.Drawing.Size(86, 23)
        Me.btnReadMacAddress.TabIndex = 51
        Me.btnReadMacAddress.Text = "Read MAC"
        Me.btnReadMacAddress.UseVisualStyleBackColor = true
        '
        'lblTestCommand
        '
        Me.lblTestCommand.AutoSize = true
        Me.lblTestCommand.Location = New System.Drawing.Point(239, 223)
        Me.lblTestCommand.Name = "lblTestCommand"
        Me.lblTestCommand.Size = New System.Drawing.Size(0, 13)
        Me.lblTestCommand.TabIndex = 59
        '
        'lblMacScan
        '
        Me.lblMacScan.AutoSize = true
        Me.lblMacScan.Location = New System.Drawing.Point(17, 200)
        Me.lblMacScan.Name = "lblMacScan"
        Me.lblMacScan.Size = New System.Drawing.Size(0, 13)
        Me.lblMacScan.TabIndex = 54
        '
        'cboMeterForm
        '
        Me.cboMeterForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMeterForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboMeterForm.FormattingEnabled = true
        Me.cboMeterForm.Location = New System.Drawing.Point(191, 91)
        Me.cboMeterForm.Name = "cboMeterForm"
        Me.cboMeterForm.Size = New System.Drawing.Size(103, 21)
        Me.cboMeterForm.TabIndex = 30
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(7, 150)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(51, 13)
        Me.Label14.TabIndex = 28
        Me.Label14.Text = "Bar Code"
        '
        'label5
        '
        Me.label5.AutoSize = true
        Me.label5.Location = New System.Drawing.Point(7, 102)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(50, 13)
        Me.label5.TabIndex = 19
        Me.label5.Text = "Data Bits"
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.Label14)
        Me.groupBox2.Controls.Add(Me.txtBarcode)
        Me.groupBox2.Controls.Add(Me.rdoText)
        Me.groupBox2.Controls.Add(Me.label5)
        Me.groupBox2.Controls.Add(Me.rdoHex)
        Me.groupBox2.Controls.Add(Me.cboData)
        Me.groupBox2.Controls.Add(Me.label4)
        Me.groupBox2.Controls.Add(Me.cboStop)
        Me.groupBox2.Controls.Add(Me.label3)
        Me.groupBox2.Controls.Add(Me.label2)
        Me.groupBox2.Controls.Add(Me.cboParity)
        Me.groupBox2.Controls.Add(Me.ChkHash)
        Me.groupBox2.Controls.Add(Me.Label1)
        Me.groupBox2.Controls.Add(Me.cboBaud)
        Me.groupBox2.Controls.Add(Me.lblMacScan)
        Me.groupBox2.Controls.Add(Me.cboPort)
        Me.groupBox2.Location = New System.Drawing.Point(388, 16)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(182, 266)
        Me.groupBox2.TabIndex = 47
        Me.groupBox2.TabStop = false
        Me.groupBox2.Text = "Serial Options"
        '
        'txtBarcode
        '
        Me.txtBarcode.Location = New System.Drawing.Point(10, 166)
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.Size = New System.Drawing.Size(37, 20)
        Me.txtBarcode.TabIndex = 27
        Me.txtBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'rdoText
        '
        Me.rdoText.AutoSize = true
        Me.rdoText.Location = New System.Drawing.Point(102, 169)
        Me.rdoText.Name = "rdoText"
        Me.rdoText.Size = New System.Drawing.Size(46, 17)
        Me.rdoText.TabIndex = 1
        Me.rdoText.TabStop = true
        Me.rdoText.Text = "Text"
        Me.rdoText.UseVisualStyleBackColor = true
        '
        'rdoHex
        '
        Me.rdoHex.AutoSize = true
        Me.rdoHex.Location = New System.Drawing.Point(102, 147)
        Me.rdoHex.Name = "rdoHex"
        Me.rdoHex.Size = New System.Drawing.Size(44, 17)
        Me.rdoHex.TabIndex = 0
        Me.rdoHex.TabStop = true
        Me.rdoHex.Text = "Hex"
        Me.rdoHex.UseVisualStyleBackColor = true
        '
        'cboData
        '
        Me.cboData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboData.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboData.FormattingEnabled = true
        Me.cboData.Items.AddRange(New Object() {"7", "8", "9"})
        Me.cboData.Location = New System.Drawing.Point(10, 118)
        Me.cboData.Name = "cboData"
        Me.cboData.Size = New System.Drawing.Size(76, 21)
        Me.cboData.TabIndex = 14
        '
        'label4
        '
        Me.label4.AutoSize = true
        Me.label4.Location = New System.Drawing.Point(94, 62)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(49, 13)
        Me.label4.TabIndex = 18
        Me.label4.Text = "Stop Bits"
        '
        'cboStop
        '
        Me.cboStop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboStop.FormattingEnabled = true
        Me.cboStop.Location = New System.Drawing.Point(96, 78)
        Me.cboStop.Name = "cboStop"
        Me.cboStop.Size = New System.Drawing.Size(76, 21)
        Me.cboStop.TabIndex = 13
        '
        'label3
        '
        Me.label3.AutoSize = true
        Me.label3.Location = New System.Drawing.Point(6, 61)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(33, 13)
        Me.label3.TabIndex = 17
        Me.label3.Text = "Parity"
        '
        'label2
        '
        Me.label2.AutoSize = true
        Me.label2.Location = New System.Drawing.Point(93, 18)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(58, 13)
        Me.label2.TabIndex = 16
        Me.label2.Text = "Baud Rate"
        '
        'cboParity
        '
        Me.cboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboParity.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboParity.FormattingEnabled = true
        Me.cboParity.Location = New System.Drawing.Point(9, 77)
        Me.cboParity.Name = "cboParity"
        Me.cboParity.Size = New System.Drawing.Size(76, 21)
        Me.cboParity.TabIndex = 12
        '
        'ChkHash
        '
        Me.ChkHash.AutoSize = true
        Me.ChkHash.Checked = true
        Me.ChkHash.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkHash.Location = New System.Drawing.Point(103, 122)
        Me.ChkHash.Name = "ChkHash"
        Me.ChkHash.Size = New System.Drawing.Size(65, 17)
        Me.ChkHash.TabIndex = 23
        Me.ChkHash.Text = "Hashing"
        Me.ChkHash.UseVisualStyleBackColor = true
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(6, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(26, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Port"
        '
        'cboBaud
        '
        Me.cboBaud.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboBaud.FormattingEnabled = true
        Me.cboBaud.Items.AddRange(New Object() {"2400", "4800", "9600", "38400", "57600", "115200"})
        Me.cboBaud.Location = New System.Drawing.Point(96, 34)
        Me.cboBaud.Name = "cboBaud"
        Me.cboBaud.Size = New System.Drawing.Size(76, 21)
        Me.cboBaud.TabIndex = 11
        '
        'cboPort
        '
        Me.cboPort.BackColor = System.Drawing.Color.White
        Me.cboPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboPort.FormattingEnabled = true
        Me.cboPort.Location = New System.Drawing.Point(9, 34)
        Me.cboPort.Name = "cboPort"
        Me.cboPort.Size = New System.Drawing.Size(76, 21)
        Me.cboPort.TabIndex = 10
        '
        'txtProductFamily
        '
        Me.txtProductFamily.Location = New System.Drawing.Point(7, 92)
        Me.txtProductFamily.Name = "txtProductFamily"
        Me.txtProductFamily.Size = New System.Drawing.Size(164, 20)
        Me.txtProductFamily.TabIndex = 28
        '
        'txtCustomerName
        '
        Me.txtCustomerName.Location = New System.Drawing.Point(6, 129)
        Me.txtCustomerName.Name = "txtCustomerName"
        Me.txtCustomerName.Size = New System.Drawing.Size(164, 20)
        Me.txtCustomerName.TabIndex = 27
        '
        'txtCustomerID
        '
        Me.txtCustomerID.Location = New System.Drawing.Point(191, 129)
        Me.txtCustomerID.Name = "txtCustomerID"
        Me.txtCustomerID.Size = New System.Drawing.Size(96, 20)
        Me.txtCustomerID.TabIndex = 25
        '
        'Label12
        '
        Me.Label12.AutoSize = true
        Me.Label12.Location = New System.Drawing.Point(188, 152)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(103, 13)
        Me.Label12.TabIndex = 23
        Me.Label12.Text = "Meter Serial Number"
        '
        'txtMeterSerial
        '
        Me.txtMeterSerial.Location = New System.Drawing.Point(190, 166)
        Me.txtMeterSerial.Name = "txtMeterSerial"
        Me.txtMeterSerial.Size = New System.Drawing.Size(164, 20)
        Me.txtMeterSerial.TabIndex = 19
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(187, 192)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(71, 13)
        Me.Label11.TabIndex = 20
        Me.Label11.Text = "MAC Address"
        '
        'txtMacAddress
        '
        Me.txtMacAddress.Location = New System.Drawing.Point(190, 206)
        Me.txtMacAddress.Name = "txtMacAddress"
        Me.txtMacAddress.Size = New System.Drawing.Size(164, 20)
        Me.txtMacAddress.TabIndex = 20
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(190, 115)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(65, 13)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Customer ID"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(4, 115)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(82, 13)
        Me.Label8.TabIndex = 17
        Me.Label8.Text = "Customer Name"
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(190, 76)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(60, 13)
        Me.Label9.TabIndex = 16
        Me.Label9.Text = "Meter Form"
        '
        'txtSend
        '
        Me.txtSend.Location = New System.Drawing.Point(425, 303)
        Me.txtSend.Name = "txtSend"
        Me.txtSend.Size = New System.Drawing.Size(191, 20)
        Me.txtSend.TabIndex = 4
        '
        'rtbDisplay
        '
        Me.rtbDisplay.Location = New System.Drawing.Point(7, 17)
        Me.rtbDisplay.Name = "rtbDisplay"
        Me.rtbDisplay.Size = New System.Drawing.Size(267, 109)
        Me.rtbDisplay.TabIndex = 3
        Me.rtbDisplay.Text = ""
        Me.rtbDisplay.WordWrap = false
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rtbDisplay)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(280, 135)
        Me.GroupBox1.TabIndex = 46
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Serial Port Communication"
        '
        'cmdSend
        '
        Me.cmdSend.Enabled = false
        Me.cmdSend.Location = New System.Drawing.Point(622, 303)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(70, 23)
        Me.cmdSend.TabIndex = 5
        Me.cmdSend.Text = "Send"
        Me.cmdSend.UseVisualStyleBackColor = true
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(5, 77)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(76, 13)
        Me.Label10.TabIndex = 15
        Me.Label10.Text = "Product Family"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.prgTestProgress)
        Me.GroupBox3.Controls.Add(Me.btnLoadTestScript)
        Me.GroupBox3.Controls.Add(Me.cboTestFile)
        Me.GroupBox3.Controls.Add(Me.lblTestFile)
        Me.GroupBox3.Controls.Add(Me.cboMeterForm)
        Me.GroupBox3.Controls.Add(Me.txtHashVal)
        Me.GroupBox3.Controls.Add(Me.Label19)
        Me.GroupBox3.Controls.Add(Me.chkManualScript)
        Me.GroupBox3.Controls.Add(Me.Label18)
        Me.GroupBox3.Controls.Add(Me.txtUtilitySerial)
        Me.GroupBox3.Controls.Add(Me.chkLock)
        Me.GroupBox3.Controls.Add(Me.txtProductFamily)
        Me.GroupBox3.Controls.Add(Me.txtCustomerName)
        Me.GroupBox3.Controls.Add(Me.txtCustomerID)
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Controls.Add(Me.txtMeterSerial)
        Me.GroupBox3.Controls.Add(Me.Label11)
        Me.GroupBox3.Controls.Add(Me.txtMacAddress)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Location = New System.Drawing.Point(575, 16)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(385, 266)
        Me.GroupBox3.TabIndex = 49
        Me.GroupBox3.TabStop = false
        Me.GroupBox3.Text = "Test Parameters"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(4, 240)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(72, 13)
        Me.Label6.TabIndex = 41
        Me.Label6.Text = "Test Progress"
        '
        'prgTestProgress
        '
        Me.prgTestProgress.Location = New System.Drawing.Point(82, 236)
        Me.prgTestProgress.Name = "prgTestProgress"
        Me.prgTestProgress.Size = New System.Drawing.Size(153, 23)
        Me.prgTestProgress.TabIndex = 40
        '
        'btnLoadTestScript
        '
        Me.btnLoadTestScript.Location = New System.Drawing.Point(330, 45)
        Me.btnLoadTestScript.Name = "btnLoadTestScript"
        Me.btnLoadTestScript.Size = New System.Drawing.Size(49, 23)
        Me.btnLoadTestScript.TabIndex = 39
        Me.btnLoadTestScript.Text = "Load"
        Me.btnLoadTestScript.UseVisualStyleBackColor = true
        '
        'cboTestFile
        '
        Me.cboTestFile.FormattingEnabled = true
        Me.cboTestFile.Location = New System.Drawing.Point(6, 46)
        Me.cboTestFile.Name = "cboTestFile"
        Me.cboTestFile.Size = New System.Drawing.Size(321, 21)
        Me.cboTestFile.TabIndex = 38
        '
        'lblTestFile
        '
        Me.lblTestFile.AutoSize = true
        Me.lblTestFile.Location = New System.Drawing.Point(6, 31)
        Me.lblTestFile.Name = "lblTestFile"
        Me.lblTestFile.Size = New System.Drawing.Size(47, 13)
        Me.lblTestFile.TabIndex = 37
        Me.lblTestFile.Text = "Test File"
        '
        'btnOpen
        '
        Me.btnOpen.Location = New System.Drawing.Point(968, 259)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(88, 23)
        Me.btnOpen.TabIndex = 44
        Me.btnOpen.Text = "Open Port"
        Me.btnOpen.UseVisualStyleBackColor = true
        '
        'grpRawOutput
        '
        Me.grpRawOutput.Controls.Add(Me.rtbResult)
        Me.grpRawOutput.Location = New System.Drawing.Point(8, 145)
        Me.grpRawOutput.Name = "grpRawOutput"
        Me.grpRawOutput.Size = New System.Drawing.Size(280, 135)
        Me.grpRawOutput.TabIndex = 64
        Me.grpRawOutput.TabStop = false
        Me.grpRawOutput.Text = "Raw Output"
        '
        'grpTestResults
        '
        Me.grpTestResults.Controls.Add(Me.rtbTestResults)
        Me.grpTestResults.Location = New System.Drawing.Point(8, 283)
        Me.grpTestResults.Name = "grpTestResults"
        Me.grpTestResults.Size = New System.Drawing.Size(280, 177)
        Me.grpTestResults.TabIndex = 65
        Me.grpTestResults.TabStop = false
        Me.grpTestResults.Text = "Test Results"
        '
        'rtbTestResults
        '
        Me.rtbTestResults.AccessibleName = "tstresult"
        Me.rtbTestResults.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.rtbTestResults.Location = New System.Drawing.Point(9, 20)
        Me.rtbTestResults.Name = "rtbTestResults"
        Me.rtbTestResults.Size = New System.Drawing.Size(267, 151)
        Me.rtbTestResults.TabIndex = 43
        Me.rtbTestResults.Text = ""
        Me.rtbTestResults.WordWrap = false
        '
        'btnResetTest
        '
        Me.btnResetTest.Enabled = false
        Me.btnResetTest.Location = New System.Drawing.Point(968, 156)
        Me.btnResetTest.Name = "btnResetTest"
        Me.btnResetTest.Size = New System.Drawing.Size(86, 23)
        Me.btnResetTest.TabIndex = 66
        Me.btnResetTest.Text = "Reset Test"
        Me.btnResetTest.UseVisualStyleBackColor = true
        '
        'btnClosePort
        '
        Me.btnClosePort.Enabled = false
        Me.btnClosePort.Location = New System.Drawing.Point(968, 230)
        Me.btnClosePort.Name = "btnClosePort"
        Me.btnClosePort.Size = New System.Drawing.Size(88, 23)
        Me.btnClosePort.TabIndex = 67
        Me.btnClosePort.Text = "Close Port"
        Me.btnClosePort.UseVisualStyleBackColor = true
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(966, 471)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(86, 23)
        Me.btnStart.TabIndex = 68
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = true
        Me.btnStart.Visible = false
        '
        'cmdReset
        '
        Me.cmdReset.Enabled = false
        Me.cmdReset.Location = New System.Drawing.Point(968, 347)
        Me.cmdReset.Name = "cmdReset"
        Me.cmdReset.Size = New System.Drawing.Size(88, 58)
        Me.cmdReset.TabIndex = 69
        Me.cmdReset.Text = "Reset"
        Me.cmdReset.UseVisualStyleBackColor = true
        '
        'lblSetSQL
        '
        Me.lblSetSQL.AutoSize = True
        Me.lblSetSQL.Location = New System.Drawing.Point(8, 468)
        Me.lblSetSQL.Name = "lblSetSQL"
        Me.lblSetSQL.Size = New System.Drawing.Size(49, 13)
        Me.lblSetSQL.TabIndex = 80
        Me.lblSetSQL.Text = "Set SQL:"
        '
        'txtSetSQL
        '
        Me.txtSetSQL.Location = New System.Drawing.Point(8, 484)
        Me.txtSetSQL.Name = "txtSetSQL"
        Me.txtSetSQL.Size = New System.Drawing.Size(180, 20)
        Me.txtSetSQL.TabIndex = 81
        Me.txtSetSQL.UseSystemPasswordChar = True
        '
        'btnSetSQL
        '
        Me.btnSetSQL.Location = New System.Drawing.Point(194, 483)
        Me.btnSetSQL.Name = "btnSetSQL"
        Me.btnSetSQL.Size = New System.Drawing.Size(84, 23)
        Me.btnSetSQL.TabIndex = 82
        Me.btnSetSQL.Text = "Set"
        Me.btnSetSQL.UseVisualStyleBackColor = True
        '
        'frmManual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1064, 516)
        Me.Controls.Add(Me.lblSetSQL)
        Me.Controls.Add(Me.txtSetSQL)
        Me.Controls.Add(Me.btnSetSQL)
        Me.Controls.Add(Me.cmdReset)
        Me.Controls.Add(Me.cmdSend)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.txtSend)
        Me.Controls.Add(Me.btnClosePort)
        Me.Controls.Add(Me.btnResetTest)
        Me.Controls.Add(Me.grpTestResults)
        Me.Controls.Add(Me.grpRawOutput)
        Me.Controls.Add(Me.btnRunTest)
        Me.Controls.Add(Me.btnQuietON)
        Me.Controls.Add(Me.lblTestProgress)
        Me.Controls.Add(Me.btnReadFirmwareManual)
        Me.Controls.Add(Me.btnReadMacAddress)
        Me.Controls.Add(Me.lblTestCommand)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.btnOpen)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "frmManual"
        Me.Text = "SRFN"
        Me.groupBox2.ResumeLayout(false)
        Me.groupBox2.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox3.PerformLayout
        Me.grpRawOutput.ResumeLayout(false)
        Me.grpTestResults.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Private WithEvents txtHashVal As System.Windows.Forms.TextBox
    Friend WithEvents chkManualScript As System.Windows.Forms.CheckBox
    Private WithEvents Label18 As System.Windows.Forms.Label
    Private WithEvents txtUtilitySerial As System.Windows.Forms.TextBox
    Friend WithEvents chkLock As System.Windows.Forms.CheckBox
    Private WithEvents Label19 As System.Windows.Forms.Label
    Private WithEvents btnRunTest As System.Windows.Forms.Button
    Private WithEvents btnQuietON As System.Windows.Forms.Button
    Private WithEvents lblTestProgress As System.Windows.Forms.Label
    Private WithEvents rtbResult As System.Windows.Forms.RichTextBox
    Private WithEvents btnReadFirmwareManual As System.Windows.Forms.Button
    Private WithEvents btnReadMacAddress As System.Windows.Forms.Button
    Private WithEvents lblTestCommand As System.Windows.Forms.Label
    Private WithEvents lblMacScan As System.Windows.Forms.Label
    Private WithEvents cboMeterForm As System.Windows.Forms.ComboBox
    Private WithEvents Label14 As System.Windows.Forms.Label
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
    Private WithEvents txtBarcode As System.Windows.Forms.TextBox
    Private WithEvents rdoText As System.Windows.Forms.RadioButton
    Private WithEvents rdoHex As System.Windows.Forms.RadioButton
    Private WithEvents cboData As System.Windows.Forms.ComboBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents cboStop As System.Windows.Forms.ComboBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents cboParity As System.Windows.Forms.ComboBox
    Friend WithEvents ChkHash As System.Windows.Forms.CheckBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents cboBaud As System.Windows.Forms.ComboBox
    Private WithEvents cboPort As System.Windows.Forms.ComboBox
    Private WithEvents txtProductFamily As System.Windows.Forms.TextBox
    Private WithEvents txtCustomerName As System.Windows.Forms.TextBox
    Private WithEvents txtCustomerID As System.Windows.Forms.TextBox
    Private WithEvents Label12 As System.Windows.Forms.Label
    Private WithEvents txtMeterSerial As System.Windows.Forms.TextBox
    Private WithEvents Label11 As System.Windows.Forms.Label
    Private WithEvents txtMacAddress As System.Windows.Forms.TextBox
    Private WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents Label9 As System.Windows.Forms.Label
    Private WithEvents txtSend As System.Windows.Forms.TextBox
    Private WithEvents rtbDisplay As System.Windows.Forms.RichTextBox
    Private WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents cmdSend As System.Windows.Forms.Button
    Private WithEvents Label10 As System.Windows.Forms.Label
    Private WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Private WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents grpRawOutput As System.Windows.Forms.GroupBox
    Friend WithEvents grpTestResults As System.Windows.Forms.GroupBox
    Friend WithEvents rtbTestResults As System.Windows.Forms.RichTextBox
    Private WithEvents btnResetTest As System.Windows.Forms.Button
    Private WithEvents btnClosePort As System.Windows.Forms.Button
    Private WithEvents cboTestFile As System.Windows.Forms.ComboBox
    Private WithEvents lblTestFile As System.Windows.Forms.Label
    Friend WithEvents btnLoadTestScript As System.Windows.Forms.Button
    Friend WithEvents prgTestProgress As System.Windows.Forms.ProgressBar
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents btnStart As System.Windows.Forms.Button
    Private WithEvents cmdReset As System.Windows.Forms.Button
    Private WithEvents lblSetSQL As System.Windows.Forms.Label
    Friend WithEvents txtSetSQL As System.Windows.Forms.TextBox
    Private WithEvents btnSetSQL As System.Windows.Forms.Button
End Class
