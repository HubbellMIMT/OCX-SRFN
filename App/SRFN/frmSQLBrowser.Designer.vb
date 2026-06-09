<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSQLBrowser
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
        Me.lblServer = New System.Windows.Forms.Label()
        Me.lblUserId = New System.Windows.Forms.Label()
        Me.txtUserId = New System.Windows.Forms.TextBox()
        Me.lblBrowsePwd = New System.Windows.Forms.Label()
        Me.txtBrowsePwd = New System.Windows.Forms.TextBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.lblDatabase = New System.Windows.Forms.Label()
        Me.cboDatabase = New System.Windows.Forms.ComboBox()
        Me.lblTable = New System.Windows.Forms.Label()
        Me.cboTable = New System.Windows.Forms.ComboBox()
        Me.lblPreview = New System.Windows.Forms.Label()
        Me.txtPreview = New System.Windows.Forms.TextBox()
        Me.btnUse = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblServer
        '
        Me.lblServer.AutoSize = True
        Me.lblServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblServer.Location = New System.Drawing.Point(10, 12)
        Me.lblServer.Text = "Server:"
        '
        'lblUserId
        '
        Me.lblUserId.AutoSize = True
        Me.lblUserId.Location = New System.Drawing.Point(10, 40)
        Me.lblUserId.Text = "User ID:"
        '
        'txtUserId
        '
        Me.txtUserId.Location = New System.Drawing.Point(80, 37)
        Me.txtUserId.Size = New System.Drawing.Size(220, 20)
        Me.txtUserId.TabIndex = 0
        '
        'lblBrowsePwd
        '
        Me.lblBrowsePwd.AutoSize = True
        Me.lblBrowsePwd.Location = New System.Drawing.Point(10, 65)
        Me.lblBrowsePwd.Text = "Password:"
        '
        'txtBrowsePwd
        '
        Me.txtBrowsePwd.Location = New System.Drawing.Point(80, 62)
        Me.txtBrowsePwd.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtBrowsePwd.Size = New System.Drawing.Size(220, 20)
        Me.txtBrowsePwd.TabIndex = 1
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(312, 37)
        Me.btnRefresh.Size = New System.Drawing.Size(128, 23)
        Me.btnRefresh.TabIndex = 2
        Me.btnRefresh.Text = "Connect / Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'lblHint
        '
        Me.lblHint.AutoSize = False
        Me.lblHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblHint.Location = New System.Drawing.Point(10, 88)
        Me.lblHint.Size = New System.Drawing.Size(430, 17)
        Me.lblHint.Text = "(blank = Windows auth)"
        '
        'lblDatabase
        '
        Me.lblDatabase.AutoSize = True
        Me.lblDatabase.Location = New System.Drawing.Point(10, 118)
        Me.lblDatabase.Text = "Database:"
        '
        'cboDatabase
        '
        Me.cboDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDatabase.FormattingEnabled = True
        Me.cboDatabase.Location = New System.Drawing.Point(80, 115)
        Me.cboDatabase.Size = New System.Drawing.Size(360, 21)
        Me.cboDatabase.TabIndex = 3
        '
        'lblTable
        '
        Me.lblTable.AutoSize = True
        Me.lblTable.Location = New System.Drawing.Point(10, 145)
        Me.lblTable.Text = "Table:"
        '
        'cboTable
        '
        Me.cboTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTable.FormattingEnabled = True
        Me.cboTable.Location = New System.Drawing.Point(80, 142)
        Me.cboTable.Size = New System.Drawing.Size(360, 21)
        Me.cboTable.TabIndex = 4
        '
        'lblPreview
        '
        Me.lblPreview.AutoSize = True
        Me.lblPreview.Location = New System.Drawing.Point(10, 175)
        Me.lblPreview.Text = "Connection String:"
        '
        'txtPreview
        '
        Me.txtPreview.BackColor = System.Drawing.SystemColors.Control
        Me.txtPreview.Location = New System.Drawing.Point(10, 191)
        Me.txtPreview.ReadOnly = True
        Me.txtPreview.Size = New System.Drawing.Size(430, 20)
        Me.txtPreview.TabIndex = 5
        '
        'btnUse
        '
        Me.btnUse.Location = New System.Drawing.Point(10, 226)
        Me.btnUse.Size = New System.Drawing.Size(195, 28)
        Me.btnUse.TabIndex = 6
        Me.btnUse.Text = "Use This Connection String"
        Me.btnUse.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(365, 226)
        Me.btnCancel.Size = New System.Drawing.Size(75, 28)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmSQLBrowser
        '
        Me.AcceptButton = Me.btnUse
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(452, 267)
        Me.Controls.Add(Me.lblServer)
        Me.Controls.Add(Me.lblUserId)
        Me.Controls.Add(Me.txtUserId)
        Me.Controls.Add(Me.lblBrowsePwd)
        Me.Controls.Add(Me.txtBrowsePwd)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.lblHint)
        Me.Controls.Add(Me.lblDatabase)
        Me.Controls.Add(Me.cboDatabase)
        Me.Controls.Add(Me.lblTable)
        Me.Controls.Add(Me.cboTable)
        Me.Controls.Add(Me.lblPreview)
        Me.Controls.Add(Me.txtPreview)
        Me.Controls.Add(Me.btnUse)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Browse SQL Tables"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblServer As Label
    Friend WithEvents lblUserId As Label
    Friend WithEvents txtUserId As TextBox
    Friend WithEvents lblBrowsePwd As Label
    Friend WithEvents txtBrowsePwd As TextBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents lblHint As Label
    Friend WithEvents lblDatabase As Label
    Friend WithEvents cboDatabase As ComboBox
    Friend WithEvents lblTable As Label
    Friend WithEvents cboTable As ComboBox
    Friend WithEvents lblPreview As Label
    Friend WithEvents txtPreview As TextBox
    Friend WithEvents btnUse As Button
    Friend WithEvents btnCancel As Button
End Class
