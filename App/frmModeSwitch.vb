Imports System.Windows.Forms
Imports System.Drawing

Public Class frmModeSwitch
    Inherits Form

    Public SelectedMode As String = ""

    Public Sub New()
        Text = "Select Protocol"
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        MinimizeBox = False
        ClientSize = New Size(220, 80)
        ShowInTaskbar = False

        Dim btnSRFN As New Button() With {
            .Text = "SRFN",
            .Location = New Point(12, 24),
            .Size = New Size(90, 34),
            .Font = New Font(Font, FontStyle.Bold)
        }
        AddHandler btnSRFN.Click, Sub(s, e)
                                      SelectedMode = "SRFN"
                                      DialogResult = DialogResult.OK
                                      Close()
                                  End Sub

        Dim btnTWACS As New Button() With {
            .Text = "TWACS",
            .Location = New Point(118, 24),
            .Size = New Size(90, 34),
            .Font = New Font(Font, FontStyle.Bold)
        }
        AddHandler btnTWACS.Click, Sub(s, e)
                                       SelectedMode = "OCX"
                                       DialogResult = DialogResult.OK
                                       Close()
                                   End Sub

        Controls.AddRange({btnSRFN, btnTWACS})
    End Sub

End Class
