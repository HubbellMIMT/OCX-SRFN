Imports System.Windows.Forms

Module Startup

    Private _trayIcon As NotifyIcon
    Private _currentForm As Form

    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        SetupTrayIcon()
        LaunchMode(ModeSettings.GetMode())
        Application.Run()
        _trayIcon.Visible = False
        _trayIcon.Dispose()
    End Sub

    Private Sub SetupTrayIcon()
        _trayIcon = New NotifyIcon()
        _trayIcon.Text = "OCX-SRFN"
        _trayIcon.Icon = System.Drawing.SystemIcons.Application
        _trayIcon.Visible = True

        Dim menu As New ContextMenu()
        Dim itemSwitch As New MenuItem("Switch Mode...")
        AddHandler itemSwitch.Click, AddressOf SwitchMode_Click
        Dim itemExit As New MenuItem("Exit")
        AddHandler itemExit.Click, AddressOf Exit_Click
        menu.MenuItems.Add(itemSwitch)
        menu.MenuItems.Add("-")
        menu.MenuItems.Add(itemExit)
        _trayIcon.ContextMenu = menu
    End Sub

    Private Sub LaunchMode(mode As String)
        If _currentForm IsNot Nothing Then
            _currentForm.Close()
            _currentForm.Dispose()
        End If
        If mode = "OCX" Then
            _currentForm = New frmOCX()
        Else
            _currentForm = New SRFN.Communication.frmManual()
        End If
        AddHandler _currentForm.FormClosed, AddressOf OnFormClosed
        _currentForm.Show()
    End Sub

    Private Sub OnFormClosed(sender As Object, e As FormClosedEventArgs)
        ' If main form is closed via X, exit the app
        Application.Exit()
    End Sub

    Private Sub SwitchMode_Click(sender As Object, e As EventArgs)
        Using dlg As New frmModeSwitch()
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim newMode As String = ModeSettings.GetMode()
                LaunchMode(newMode)
            End If
        End Using
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

End Module
