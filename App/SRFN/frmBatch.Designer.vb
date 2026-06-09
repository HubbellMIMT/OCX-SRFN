<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmBatch
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
        Me.lblRepeat = New System.Windows.Forms.Label()
        Me.nudCount = New System.Windows.Forms.NumericUpDown()
        Me.lblTimes = New System.Windows.Forms.Label()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.chkStopAtStep = New System.Windows.Forms.CheckBox()
        Me.nudStopStep = New System.Windows.Forms.NumericUpDown()
        Me.chkWaitMs = New System.Windows.Forms.CheckBox()
        Me.nudWaitMs = New System.Windows.Forms.NumericUpDown()
        Me.lblMs = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.rtbLog = New System.Windows.Forms.RichTextBox()
        CType(Me.nudCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudStopStep, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudWaitMs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblRepeat
        '
        Me.lblRepeat.AutoSize = True
        Me.lblRepeat.Location = New System.Drawing.Point(12, 16)
        Me.lblRepeat.Text = "Repeat test:"
        '
        'nudCount
        '
        Me.nudCount.Location = New System.Drawing.Point(100, 13)
        Me.nudCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudCount.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudCount.Value = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nudCount.Size = New System.Drawing.Size(55, 22)
        Me.nudCount.TabIndex = 0
        '
        'lblTimes
        '
        Me.lblTimes.AutoSize = True
        Me.lblTimes.Location = New System.Drawing.Point(160, 16)
        Me.lblTimes.Text = "times"
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(225, 11)
        Me.btnStart.Size = New System.Drawing.Size(65, 25)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "Set"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Enabled = False
        Me.btnStop.Location = New System.Drawing.Point(296, 11)
        Me.btnStop.Size = New System.Drawing.Size(65, 25)
        Me.btnStop.TabIndex = 2
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(376, 11)
        Me.btnClose.Size = New System.Drawing.Size(60, 25)
        Me.btnClose.TabIndex = 3
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'chkStopAtStep
        '
        Me.chkStopAtStep.AutoSize = True
        Me.chkStopAtStep.Location = New System.Drawing.Point(12, 43)
        Me.chkStopAtStep.TabIndex = 5
        Me.chkStopAtStep.Text = "stop at step #"
        Me.chkStopAtStep.UseVisualStyleBackColor = True
        '
        'nudStopStep
        '
        Me.nudStopStep.Enabled = False
        Me.nudStopStep.Location = New System.Drawing.Point(115, 41)
        Me.nudStopStep.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudStopStep.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudStopStep.Value = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudStopStep.Size = New System.Drawing.Size(48, 22)
        Me.nudStopStep.TabIndex = 6
        '
        'chkWaitMs
        '
        Me.chkWaitMs.AutoSize = True
        Me.chkWaitMs.Location = New System.Drawing.Point(195, 43)
        Me.chkWaitMs.TabIndex = 7
        Me.chkWaitMs.Text = "wait"
        Me.chkWaitMs.UseVisualStyleBackColor = True
        '
        'nudWaitMs
        '
        Me.nudWaitMs.Enabled = False
        Me.nudWaitMs.Location = New System.Drawing.Point(232, 41)
        Me.nudWaitMs.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.nudWaitMs.Maximum = New Decimal(New Integer() {300000, 0, 0, 0})
        Me.nudWaitMs.Value = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudWaitMs.Size = New System.Drawing.Size(72, 22)
        Me.nudWaitMs.TabIndex = 8
        '
        'lblMs
        '
        Me.lblMs.AutoSize = True
        Me.lblMs.Location = New System.Drawing.Point(309, 43)
        Me.lblMs.Text = "ms"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = False
        Me.lblStatus.Location = New System.Drawing.Point(12, 72)
        Me.lblStatus.Size = New System.Drawing.Size(424, 15)
        Me.lblStatus.Text = "Ready."
        '
        'rtbLog
        '
        Me.rtbLog.Anchor = CType(AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right, AnchorStyles)
        Me.rtbLog.BackColor = System.Drawing.Color.Black
        Me.rtbLog.ForeColor = System.Drawing.Color.White
        Me.rtbLog.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
        Me.rtbLog.Location = New System.Drawing.Point(12, 92)
        Me.rtbLog.ReadOnly = True
        Me.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.rtbLog.Size = New System.Drawing.Size(424, 263)
        Me.rtbLog.TabIndex = 4
        Me.rtbLog.Text = ""
        '
        'frmBatch
        '
        Me.ClientSize = New System.Drawing.Size(448, 368)
        Me.Controls.Add(Me.lblRepeat)
        Me.Controls.Add(Me.nudCount)
        Me.Controls.Add(Me.lblTimes)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.chkStopAtStep)
        Me.Controls.Add(Me.nudStopStep)
        Me.Controls.Add(Me.chkWaitMs)
        Me.Controls.Add(Me.nudWaitMs)
        Me.Controls.Add(Me.lblMs)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.rtbLog)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MinimumSize = New System.Drawing.Size(350, 300)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Batch Test"
        CType(Me.nudCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudStopStep, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudWaitMs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblRepeat As Label
    Friend WithEvents nudCount As NumericUpDown
    Friend WithEvents lblTimes As Label
    Friend WithEvents btnStart As Button
    Friend WithEvents btnStop As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents chkStopAtStep As CheckBox
    Friend WithEvents nudStopStep As NumericUpDown
    Friend WithEvents chkWaitMs As CheckBox
    Friend WithEvents nudWaitMs As NumericUpDown
    Friend WithEvents lblMs As Label
    Friend WithEvents lblStatus As Label
    Friend WithEvents rtbLog As RichTextBox
End Class
