Imports System.Windows.Forms
Imports System.Drawing

Public Class frmModeSwitch
    Inherits Form

    Private txtPassword As New TextBox()
    Private btnOK As New Button()
    Private btnCancel As New Button()
    Private lblError As New Label()

    Public Sub New()
        Text = "Select Protocol Mode"
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        MinimizeBox = False
        ClientSize = New Size(300, 150)
        ShowInTaskbar = False

        Dim lblInstr As New Label() With {
            .Text = "Enter password to select startup mode:" & Environment.NewLine &
                    "  SRFN  — SRFN protocol" & Environment.NewLine &
                    "  TWACS — OCX / TWRN protocol",
            .Location = New Point(12, 12),
            .Size = New Size(270, 52)
        }

        txtPassword.Location = New Point(12, 72)
        txtPassword.Width = 270
        txtPassword.UseSystemPasswordChar = True

        lblError.Location = New Point(12, 96)
        lblError.Size = New Size(270, 16)
        lblError.ForeColor = Color.Red

        btnOK.Text = "OK"
        btnOK.Location = New Point(132, 116)
        btnOK.Width = 72
        AddHandler btnOK.Click, AddressOf btnOK_Click

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(210, 116)
        btnCancel.Width = 72
        btnCancel.DialogResult = DialogResult.Cancel

        Controls.AddRange({lblInstr, txtPassword, lblError, btnOK, btnCancel})
        AcceptButton = btnOK
        CancelButton = btnCancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs)
        If ModeSettings.SetModeByPassword(txtPassword.Text) Then
            DialogResult = DialogResult.OK
            Close()
        Else
            lblError.Text = "Enter 'SRFN' or 'TWACS'."
            txtPassword.Clear()
            txtPassword.Focus()
        End If
    End Sub

End Class
