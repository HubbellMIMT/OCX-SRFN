Imports System.Windows.Forms
Imports System.Drawing

Public Class frmModeSwitch
    Inherits Form

    Private txtPassword As New TextBox()
    Private rbSRFN As New RadioButton()
    Private rbOCX As New RadioButton()
    Private btnOK As New Button()
    Private btnCancel As New Button()
    Private lblError As New Label()

    Public Sub New()
        Text = "Switch Protocol Mode"
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        MinimizeBox = False
        ClientSize = New Size(300, 200)
        ShowInTaskbar = False

        Dim lblPwd As New Label() With {.Text = "Password:", .Location = New Point(12, 16), .AutoSize = True}
        txtPassword.Location = New Point(12, 36)
        txtPassword.Width = 270
        txtPassword.UseSystemPasswordChar = True

        lblError.Location = New Point(12, 62)
        lblError.Size = New Size(270, 20)
        lblError.ForeColor = Color.Red
        lblError.Text = ""

        rbSRFN.Text = "SRFN"
        rbSRFN.Location = New Point(12, 90)
        rbSRFN.AutoSize = True
        rbSRFN.Checked = (ModeSettings.GetMode() = "SRFN")

        rbOCX.Text = "OCX / TWRN"
        rbOCX.Location = New Point(12, 114)
        rbOCX.AutoSize = True
        rbOCX.Checked = (ModeSettings.GetMode() = "OCX")

        btnOK.Text = "Switch"
        btnOK.Location = New Point(132, 158)
        btnOK.Width = 72
        AddHandler btnOK.Click, AddressOf btnOK_Click

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(210, 158)
        btnCancel.Width = 72
        btnCancel.DialogResult = DialogResult.Cancel

        Controls.AddRange({lblPwd, txtPassword, lblError, rbSRFN, rbOCX, btnOK, btnCancel})
        AcceptButton = btnOK
        CancelButton = btnCancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs)
        If Not ModeSettings.VerifyPassword(txtPassword.Text) Then
            lblError.Text = "Incorrect password."
            txtPassword.Clear()
            txtPassword.Focus()
            Return
        End If
        Dim newMode As String = If(rbSRFN.Checked, "SRFN", "OCX")
        ModeSettings.SaveMode(newMode)
        DialogResult = DialogResult.OK
        Close()
    End Sub

End Class
