Imports System.IO

Namespace TWRN
Public Class Progress
    Inherits System.Windows.Forms.Form
    Dim lgmodule As New lgmoduleINT
#Region " Windows Form Designer generated code "
    Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add()
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)

        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents lblcustomername As System.Windows.Forms.ListBox
    Friend WithEvents lstTWACS As System.Windows.Forms.ListBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.lblcustomername = New System.Windows.Forms.ListBox()
        Me.lstTWACS = New System.Windows.Forms.ListBox()
        Me.SuspendLayout
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(8, 440)
        Me.ProgressBar1.Maximum = 12
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(456, 32)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.TabIndex = 0
        '
        'lblcustomername
        '
        Me.lblcustomername.Location = New System.Drawing.Point(8, 8)
        Me.lblcustomername.Name = "lblcustomername"
        Me.lblcustomername.Size = New System.Drawing.Size(456, 355)
        Me.lblcustomername.TabIndex = 1
        '
        'lstTWACS
        '
        Me.lstTWACS.BackColor = System.Drawing.SystemColors.WindowText
        Me.lstTWACS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lstTWACS.ForeColor = System.Drawing.Color.Yellow
        Me.lstTWACS.ItemHeight = 16
        Me.lstTWACS.Location = New System.Drawing.Point(8, 368)
        Me.lstTWACS.Name = "lstTWACS"
        Me.lstTWACS.Size = New System.Drawing.Size(456, 68)
        Me.lstTWACS.TabIndex = 11
        '
        'Progress
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(472, 481)
        Me.ControlBox = false
        Me.Controls.Add(Me.lstTWACS)
        Me.Controls.Add(Me.lblcustomername)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Name = "Progress"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Aclara Module Integration Status"
        Me.TopMost = true
        Me.ResumeLayout(false)

End Sub
#End Region
    Private Sub Add()
        'Dim Pass As String
        'Dim Fail As String
        'Dim subT As Integer
        'Dim Total As String
        'Dim Percent As String
        'Dim Temp As Integer
        'If txtTotal.Text = "" Then
        '    Total = lgmodule.Results
        '    Temp = InStr(Total, ",")
        '    Pass = Mid(Total, 1, Temp - 1)
        '    Fail = Mid(Total, Temp + 1)
        '    txtPass.Text = Pass
        '    txtFail.Text = Fail
        '    subT = CInt(Pass) + CInt(Fail)
        '    Total = CStr(subT)
        '    txtTotal.Text = Total
        '    Percent = CSTR(100 * CInt(Pass) / CInt(Total))
        '    If Percent.Length > 6 Then Percent = Mid(Percent, 1, 5)
        '    txtPercent.Text = Percent & "%"
        '    txtSQLFail.Text = SQLWriteFail
        'End If
    End Sub
End Class
End Namespace
