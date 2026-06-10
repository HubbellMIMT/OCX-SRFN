<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCommPorts
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
        Me.lblCommPort = New System.Windows.Forms.Label()
        Me.cboCommPort = New System.Windows.Forms.ComboBox()
        Me.lblDebugPort = New System.Windows.Forms.Label()
        Me.cboDebugPort = New System.Windows.Forms.ComboBox()
        Me.lblRelayPort = New System.Windows.Forms.Label()
        Me.cboRelayPort = New System.Windows.Forms.ComboBox()
        Me.lblOpticalPort = New System.Windows.Forms.Label()
        Me.cboOpticalPort = New System.Windows.Forms.ComboBox()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnApply = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnDiscover = New System.Windows.Forms.Button()
        Me.lblDiscoverStatus = New System.Windows.Forms.Label()
        Me.chkDebugWindow = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblCommPort
        '
        Me.lblCommPort.AutoSize = True
        Me.lblCommPort.Location = New System.Drawing.Point(12, 15)
        Me.lblCommPort.Text = "Comm Port"
        '
        'cboCommPort
        '
        Me.cboCommPort.FormattingEnabled = True
        Me.cboCommPort.Location = New System.Drawing.Point(110, 12)
        Me.cboCommPort.Size = New System.Drawing.Size(60, 21)
        Me.cboCommPort.TabIndex = 0
        '
        'lblDebugPort
        '
        Me.lblDebugPort.AutoSize = True
        Me.lblDebugPort.Location = New System.Drawing.Point(12, 45)
        Me.lblDebugPort.Text = "Debug Port"
        '
        'cboDebugPort
        '
        Me.cboDebugPort.FormattingEnabled = True
        Me.cboDebugPort.Location = New System.Drawing.Point(110, 42)
        Me.cboDebugPort.Size = New System.Drawing.Size(60, 21)
        Me.cboDebugPort.TabIndex = 1
        '
        'lblRelayPort
        '
        Me.lblRelayPort.AutoSize = True
        Me.lblRelayPort.Location = New System.Drawing.Point(12, 75)
        Me.lblRelayPort.Text = "Relay Port"
        '
        'cboRelayPort
        '
        Me.cboRelayPort.FormattingEnabled = True
        Me.cboRelayPort.Location = New System.Drawing.Point(110, 72)
        Me.cboRelayPort.Size = New System.Drawing.Size(60, 21)
        Me.cboRelayPort.TabIndex = 2
        '
        'lblOpticalPort
        '
        Me.lblOpticalPort.AutoSize = True
        Me.lblOpticalPort.Location = New System.Drawing.Point(12, 105)
        Me.lblOpticalPort.Text = "Optical USB Port"
        '
        'cboOpticalPort
        '
        Me.cboOpticalPort.FormattingEnabled = True
        Me.cboOpticalPort.Location = New System.Drawing.Point(110, 102)
        Me.cboOpticalPort.Size = New System.Drawing.Size(60, 21)
        Me.cboOpticalPort.TabIndex = 3
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(12, 138)
        Me.btnUpdate.Size = New System.Drawing.Size(80, 25)
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "Update Ports"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(100, 138)
        Me.btnApply.Size = New System.Drawing.Size(60, 25)
        Me.btnApply.TabIndex = 5
        Me.btnApply.Text = "Apply"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(168, 138)
        Me.btnClose.Size = New System.Drawing.Size(60, 25)
        Me.btnClose.TabIndex = 6
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnDiscover
        '
        Me.btnDiscover.Location = New System.Drawing.Point(12, 173)
        Me.btnDiscover.Size = New System.Drawing.Size(95, 25)
        Me.btnDiscover.TabIndex = 7
        Me.btnDiscover.Text = "Discover Port"
        Me.btnDiscover.UseVisualStyleBackColor = True
        '
        'lblDiscoverStatus
        '
        Me.lblDiscoverStatus.AutoSize = True
        Me.lblDiscoverStatus.Location = New System.Drawing.Point(115, 179)
        Me.lblDiscoverStatus.Text = ""
        '
        'chkDebugWindow
        '
        Me.chkDebugWindow.AutoSize = True
        Me.chkDebugWindow.Location = New System.Drawing.Point(12, 208)
        Me.chkDebugWindow.TabIndex = 8
        Me.chkDebugWindow.Text = "Debug Window"
        Me.chkDebugWindow.UseVisualStyleBackColor = True
        '
        'frmCommPorts
        '
        Me.ClientSize = New System.Drawing.Size(240, 235)
        Me.Controls.Add(Me.lblCommPort)
        Me.Controls.Add(Me.cboCommPort)
        Me.Controls.Add(Me.lblDebugPort)
        Me.Controls.Add(Me.cboDebugPort)
        Me.Controls.Add(Me.lblRelayPort)
        Me.Controls.Add(Me.cboRelayPort)
        Me.Controls.Add(Me.lblOpticalPort)
        Me.Controls.Add(Me.cboOpticalPort)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnDiscover)
        Me.Controls.Add(Me.lblDiscoverStatus)
        Me.Controls.Add(Me.chkDebugWindow)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Comm Ports"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblCommPort As Label
    Friend WithEvents cboCommPort As ComboBox
    Friend WithEvents lblDebugPort As Label
    Friend WithEvents cboDebugPort As ComboBox
    Friend WithEvents lblRelayPort As Label
    Friend WithEvents cboRelayPort As ComboBox
    Friend WithEvents lblOpticalPort As Label
    Friend WithEvents cboOpticalPort As ComboBox
    Friend WithEvents btnUpdate As Button
    Friend WithEvents btnApply As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnDiscover As Button
    Friend WithEvents lblDiscoverStatus As Label
    Friend WithEvents chkDebugWindow As CheckBox
End Class
