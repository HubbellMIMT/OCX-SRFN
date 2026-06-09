<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSQLConfig
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblDLLRev = New System.Windows.Forms.Label()
        Me.txtDLLRevision = New System.Windows.Forms.TextBox()
        Me.btnChkDLL = New System.Windows.Forms.Button()
        Me.btnBrowseDLL = New System.Windows.Forms.Button()
        Me.lblCustomer = New System.Windows.Forms.Label()
        Me.txtCustomerValues = New System.Windows.Forms.TextBox()
        Me.btnChkCustomer = New System.Windows.Forms.Button()
        Me.btnBrowseCustomer = New System.Windows.Forms.Button()
        Me.lblTestResults = New System.Windows.Forms.Label()
        Me.txtTestResults = New System.Windows.Forms.TextBox()
        Me.btnChkResults = New System.Windows.Forms.Button()
        Me.btnBrowseResults = New System.Windows.Forms.Button()
        Me.lblFirmware = New System.Windows.Forms.Label()
        Me.txtFirmware = New System.Windows.Forms.TextBox()
        Me.btnChkFirmware = New System.Windows.Forms.Button()
        Me.btnBrowseFirmware = New System.Windows.Forms.Button()
        Me.lblSQLServer = New System.Windows.Forms.Label()
        Me.txtServer = New System.Windows.Forms.TextBox()
        Me.chkShowForm = New System.Windows.Forms.CheckBox()
        Me.lblRA6Path = New System.Windows.Forms.Label()
        Me.txtRA6Path = New System.Windows.Forms.TextBox()
        Me.btnChkRA6Path = New System.Windows.Forms.Button()
        Me.lblConnLog = New System.Windows.Forms.Label()
        Me.txtConnLog = New System.Windows.Forms.TextBox()
        Me.btnLoadConnStr = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblDLLRev
        '
        Me.lblDLLRev.AutoSize = True
        Me.lblDLLRev.Location = New System.Drawing.Point(10, 12)
        Me.lblDLLRev.Text = "DLL Rev SQL Connection String"
        '
        'txtDLLRevision
        '
        Me.txtDLLRevision.Location = New System.Drawing.Point(10, 28)
        Me.txtDLLRevision.Size = New System.Drawing.Size(215, 20)
        Me.txtDLLRevision.TabIndex = 0
        '
        'btnChkDLL
        '
        Me.btnChkDLL.Location = New System.Drawing.Point(232, 26)
        Me.btnChkDLL.Size = New System.Drawing.Size(55, 23)
        Me.btnChkDLL.TabIndex = 1
        Me.btnChkDLL.Text = "Check"
        Me.btnChkDLL.UseVisualStyleBackColor = True
        '
        'btnBrowseDLL
        '
        Me.btnBrowseDLL.Location = New System.Drawing.Point(292, 26)
        Me.btnBrowseDLL.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseDLL.TabIndex = 2
        Me.btnBrowseDLL.Text = "Browse..."
        Me.btnBrowseDLL.UseVisualStyleBackColor = True
        '
        'lblCustomer
        '
        Me.lblCustomer.AutoSize = True
        Me.lblCustomer.Location = New System.Drawing.Point(10, 57)
        Me.lblCustomer.Text = "Customer Values SQL Connection String"
        '
        'txtCustomerValues
        '
        Me.txtCustomerValues.Location = New System.Drawing.Point(10, 73)
        Me.txtCustomerValues.Size = New System.Drawing.Size(215, 20)
        Me.txtCustomerValues.TabIndex = 3
        '
        'btnChkCustomer
        '
        Me.btnChkCustomer.Location = New System.Drawing.Point(232, 71)
        Me.btnChkCustomer.Size = New System.Drawing.Size(55, 23)
        Me.btnChkCustomer.TabIndex = 4
        Me.btnChkCustomer.Text = "Check"
        Me.btnChkCustomer.UseVisualStyleBackColor = True
        '
        'btnBrowseCustomer
        '
        Me.btnBrowseCustomer.Location = New System.Drawing.Point(292, 71)
        Me.btnBrowseCustomer.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseCustomer.TabIndex = 5
        Me.btnBrowseCustomer.Text = "Browse..."
        Me.btnBrowseCustomer.UseVisualStyleBackColor = True
        '
        'lblTestResults
        '
        Me.lblTestResults.AutoSize = True
        Me.lblTestResults.Location = New System.Drawing.Point(10, 102)
        Me.lblTestResults.Text = "Test Results SQL Connection String"
        '
        'txtTestResults
        '
        Me.txtTestResults.Location = New System.Drawing.Point(10, 118)
        Me.txtTestResults.Size = New System.Drawing.Size(215, 20)
        Me.txtTestResults.TabIndex = 6
        '
        'btnChkResults
        '
        Me.btnChkResults.Location = New System.Drawing.Point(232, 116)
        Me.btnChkResults.Size = New System.Drawing.Size(55, 23)
        Me.btnChkResults.TabIndex = 7
        Me.btnChkResults.Text = "Check"
        Me.btnChkResults.UseVisualStyleBackColor = True
        '
        'btnBrowseResults
        '
        Me.btnBrowseResults.Location = New System.Drawing.Point(292, 116)
        Me.btnBrowseResults.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseResults.TabIndex = 8
        Me.btnBrowseResults.Text = "Browse..."
        Me.btnBrowseResults.UseVisualStyleBackColor = True
        '
        'lblFirmware
        '
        Me.lblFirmware.AutoSize = True
        Me.lblFirmware.Location = New System.Drawing.Point(10, 147)
        Me.lblFirmware.Text = "Firmware SQL Connection String"
        '
        'txtFirmware
        '
        Me.txtFirmware.Location = New System.Drawing.Point(10, 163)
        Me.txtFirmware.Size = New System.Drawing.Size(215, 20)
        Me.txtFirmware.TabIndex = 9
        '
        'btnChkFirmware
        '
        Me.btnChkFirmware.Location = New System.Drawing.Point(232, 161)
        Me.btnChkFirmware.Size = New System.Drawing.Size(55, 23)
        Me.btnChkFirmware.TabIndex = 10
        Me.btnChkFirmware.Text = "Check"
        Me.btnChkFirmware.UseVisualStyleBackColor = True
        '
        'btnBrowseFirmware
        '
        Me.btnBrowseFirmware.Location = New System.Drawing.Point(292, 161)
        Me.btnBrowseFirmware.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseFirmware.TabIndex = 11
        Me.btnBrowseFirmware.Text = "Browse..."
        Me.btnBrowseFirmware.UseVisualStyleBackColor = True
        '
        'lblSQLServer
        '
        Me.lblSQLServer.AutoSize = True
        Me.lblSQLServer.Location = New System.Drawing.Point(10, 192)
        Me.lblSQLServer.Text = "SQL Server"
        '
        'txtServer
        '
        Me.txtServer.Location = New System.Drawing.Point(10, 208)
        Me.txtServer.Size = New System.Drawing.Size(220, 20)
        Me.txtServer.TabIndex = 12
        '
        'chkShowForm
        '
        Me.chkShowForm.AutoSize = True
        Me.chkShowForm.Location = New System.Drawing.Point(10, 238)
        Me.chkShowForm.Text = "Show Form While Running"
        Me.chkShowForm.TabIndex = 14
        Me.chkShowForm.UseVisualStyleBackColor = True
        '
        'lblRA6Path
        '
        Me.lblRA6Path.AutoSize = True
        Me.lblRA6Path.Location = New System.Drawing.Point(10, 265)
        Me.lblRA6Path.Text = "RA6 Programmer File Path"
        '
        'txtRA6Path
        '
        Me.txtRA6Path.Location = New System.Drawing.Point(10, 281)
        Me.txtRA6Path.Size = New System.Drawing.Size(320, 20)
        Me.txtRA6Path.TabIndex = 15
        '
        'btnChkRA6Path
        '
        Me.btnChkRA6Path.Location = New System.Drawing.Point(338, 279)
        Me.btnChkRA6Path.Size = New System.Drawing.Size(58, 23)
        Me.btnChkRA6Path.TabIndex = 16
        Me.btnChkRA6Path.Text = "Check"
        Me.btnChkRA6Path.UseVisualStyleBackColor = True
        '
        'lblConnLog
        '
        Me.lblConnLog.AutoSize = True
        Me.lblConnLog.Location = New System.Drawing.Point(10, 313)
        Me.lblConnLog.Text = "Connection Log:"
        '
        'txtConnLog
        '
        Me.txtConnLog.BackColor = System.Drawing.Color.Black
        Me.txtConnLog.ForeColor = System.Drawing.Color.Lime
        Me.txtConnLog.Font = New System.Drawing.Font("Courier New", 8.0!)
        Me.txtConnLog.Location = New System.Drawing.Point(10, 329)
        Me.txtConnLog.Multiline = True
        Me.txtConnLog.ReadOnly = True
        Me.txtConnLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtConnLog.Size = New System.Drawing.Size(386, 110)
        Me.txtConnLog.TabIndex = 17
        Me.txtConnLog.TabStop = False
        '
        'btnLoadConnStr
        '
        Me.btnLoadConnStr.Location = New System.Drawing.Point(10, 450)
        Me.btnLoadConnStr.Size = New System.Drawing.Size(120, 30)
        Me.btnLoadConnStr.TabIndex = 18
        Me.btnLoadConnStr.Text = "Load ConnStr"
        Me.btnLoadConnStr.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(145, 450)
        Me.btnSave.Size = New System.Drawing.Size(90, 30)
        Me.btnSave.TabIndex = 19
        Me.btnSave.Text = "Save All"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(316, 450)
        Me.btnClose.Size = New System.Drawing.Size(80, 30)
        Me.btnClose.TabIndex = 20
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmSQLConfig
        '
        Me.ClientSize = New System.Drawing.Size(410, 495)
        Me.Controls.Add(Me.lblDLLRev)
        Me.Controls.Add(Me.txtDLLRevision)
        Me.Controls.Add(Me.btnChkDLL)
        Me.Controls.Add(Me.btnBrowseDLL)
        Me.Controls.Add(Me.lblCustomer)
        Me.Controls.Add(Me.txtCustomerValues)
        Me.Controls.Add(Me.btnChkCustomer)
        Me.Controls.Add(Me.btnBrowseCustomer)
        Me.Controls.Add(Me.lblTestResults)
        Me.Controls.Add(Me.txtTestResults)
        Me.Controls.Add(Me.btnChkResults)
        Me.Controls.Add(Me.btnBrowseResults)
        Me.Controls.Add(Me.lblFirmware)
        Me.Controls.Add(Me.txtFirmware)
        Me.Controls.Add(Me.btnChkFirmware)
        Me.Controls.Add(Me.btnBrowseFirmware)
        Me.Controls.Add(Me.lblSQLServer)
        Me.Controls.Add(Me.txtServer)
        Me.Controls.Add(Me.chkShowForm)
        Me.Controls.Add(Me.lblRA6Path)
        Me.Controls.Add(Me.txtRA6Path)
        Me.Controls.Add(Me.btnChkRA6Path)
        Me.Controls.Add(Me.lblConnLog)
        Me.Controls.Add(Me.txtConnLog)
        Me.Controls.Add(Me.btnLoadConnStr)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "SQL Configuration"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblDLLRev As Label
    Friend WithEvents txtDLLRevision As TextBox
    Friend WithEvents btnChkDLL As Button
    Friend WithEvents btnBrowseDLL As Button
    Friend WithEvents lblCustomer As Label
    Friend WithEvents txtCustomerValues As TextBox
    Friend WithEvents btnChkCustomer As Button
    Friend WithEvents btnBrowseCustomer As Button
    Friend WithEvents lblTestResults As Label
    Friend WithEvents txtTestResults As TextBox
    Friend WithEvents btnChkResults As Button
    Friend WithEvents btnBrowseResults As Button
    Friend WithEvents lblFirmware As Label
    Friend WithEvents txtFirmware As TextBox
    Friend WithEvents btnChkFirmware As Button
    Friend WithEvents btnBrowseFirmware As Button
    Friend WithEvents lblSQLServer As Label
    Friend WithEvents txtServer As TextBox
    Friend WithEvents chkShowForm As CheckBox
    Friend WithEvents lblRA6Path As Label
    Friend WithEvents txtRA6Path As TextBox
    Friend WithEvents btnChkRA6Path As Button
    Friend WithEvents lblConnLog As Label
    Friend WithEvents txtConnLog As TextBox
    Friend WithEvents btnLoadConnStr As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnClose As Button
End Class
