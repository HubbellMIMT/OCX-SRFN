<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDebugTerminal
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
        Me.lblPortInfo = New System.Windows.Forms.Label()
        Me.rtbDisplay = New System.Windows.Forms.RichTextBox()
        Me.txtSend = New System.Windows.Forms.TextBox()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblPortInfo
        '
        Me.lblPortInfo.AutoSize = True
        Me.lblPortInfo.Location = New System.Drawing.Point(8, 6)
        Me.lblPortInfo.Text = "Debug Terminal"
        '
        'rtbDisplay
        '
        Me.rtbDisplay.Anchor = CType(AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right, AnchorStyles)
        Me.rtbDisplay.BackColor = System.Drawing.Color.Black
        Me.rtbDisplay.ForeColor = System.Drawing.Color.Lime
        Me.rtbDisplay.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
        Me.rtbDisplay.Location = New System.Drawing.Point(8, 26)
        Me.rtbDisplay.ReadOnly = True
        Me.rtbDisplay.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.rtbDisplay.Size = New System.Drawing.Size(660, 308)
        Me.rtbDisplay.TabIndex = 0
        Me.rtbDisplay.Text = ""
        '
        'txtSend
        '
        Me.txtSend.Anchor = CType(AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right, AnchorStyles)
        Me.txtSend.Location = New System.Drawing.Point(8, 344)
        Me.txtSend.Size = New System.Drawing.Size(496, 23)
        Me.txtSend.TabIndex = 1
        '
        'btnSend
        '
        Me.btnSend.Anchor = CType(AnchorStyles.Bottom Or AnchorStyles.Right, AnchorStyles)
        Me.btnSend.Location = New System.Drawing.Point(509, 342)
        Me.btnSend.Size = New System.Drawing.Size(50, 25)
        Me.btnSend.TabIndex = 2
        Me.btnSend.Text = "Send"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType(AnchorStyles.Bottom Or AnchorStyles.Right, AnchorStyles)
        Me.btnClear.Location = New System.Drawing.Point(564, 342)
        Me.btnClear.Size = New System.Drawing.Size(50, 25)
        Me.btnClear.TabIndex = 3
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType(AnchorStyles.Bottom Or AnchorStyles.Right, AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(619, 342)
        Me.btnClose.Size = New System.Drawing.Size(50, 25)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmDebugTerminal
        '
        Me.ClientSize = New System.Drawing.Size(676, 375)
        Me.Controls.Add(Me.lblPortInfo)
        Me.Controls.Add(Me.rtbDisplay)
        Me.Controls.Add(Me.txtSend)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MinimumSize = New System.Drawing.Size(520, 300)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Debug Terminal"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblPortInfo As Label
    Friend WithEvents rtbDisplay As RichTextBox
    Friend WithEvents txtSend As TextBox
    Friend WithEvents btnSend As Button
    Friend WithEvents btnClear As Button
    Friend WithEvents btnClose As Button
End Class
