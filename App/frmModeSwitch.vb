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
        AddHandler btnSRFN.Click, AddressOf btnSRFN_Click

        Dim btnTWACS As New Button() With {
            .Text = "TWACS",
            .Location = New Point(118, 24),
            .Size = New Size(90, 34),
            .Font = New Font(Font, FontStyle.Bold)
        }
        AddHandler btnTWACS.Click, AddressOf btnTWACS_Click

        Controls.AddRange({btnSRFN, btnTWACS})
    End Sub

    Private Sub btnSRFN_Click(sender As Object, e As EventArgs)
        SelectedMode = "SRFN"
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnTWACS_Click(sender As Object, e As EventArgs)
        SelectedMode = "OCX"
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
