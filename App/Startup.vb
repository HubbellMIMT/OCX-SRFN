Imports System.Windows.Forms
Imports System.IO

Module Startup

    Private _trayIcon As NotifyIcon
    Private _currentForm As Form

    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim mode As String
        Using dlg As New frmModeSwitch()
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            mode = dlg.SelectedMode
        End Using

        SetupTrayIcon(mode)

        If mode = "OCX" Then
            _currentForm = New frmOCX()
            AddHandler _currentForm.FormClosed, AddressOf OnFormClosed
            _currentForm.Show()
        End If

        Application.Run()
        _trayIcon.Visible = False
        _trayIcon.Dispose()
    End Sub

    Private Sub SetupTrayIcon(mode As String)
        _trayIcon = New NotifyIcon()
        _trayIcon.Text = If(mode = "OCX", "TWACS", "SRFN")
        _trayIcon.Icon = System.Drawing.SystemIcons.Application
        _trayIcon.Visible = True

        Dim menu As New ContextMenu()

        If mode = "SRFN" Then
            Dim itemSQL As New MenuItem("Set SQL...")
            AddHandler itemSQL.Click, AddressOf SetSQL_Click
            menu.MenuItems.Add(itemSQL)
            menu.MenuItems.Add("-")
        End If

        Dim itemSwitch As New MenuItem("Switch Mode...")
        AddHandler itemSwitch.Click, AddressOf SwitchMode_Click
        menu.MenuItems.Add(itemSwitch)
        menu.MenuItems.Add("-")

        Dim itemExit As New MenuItem("Exit")
        AddHandler itemExit.Click, AddressOf Exit_Click
        menu.MenuItems.Add(itemExit)

        _trayIcon.ContextMenu = menu
    End Sub

    Private Sub SetSQL_Click(sender As Object, e As EventArgs)
        Dim current As String = LoadSQLServerName()
        Dim input As String = InputBox("Enter SQL server name:", "Set SQL", current)
        If input.Trim() <> "" Then
            SRFN.Communication.CommManager2.SqlServerName = input.Trim()
            SaveSQLServerName(input.Trim())
        End If
    End Sub

    Private Sub SwitchMode_Click(sender As Object, e As EventArgs)
        Dim mode As String
        Using dlg As New frmModeSwitch()
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            mode = dlg.SelectedMode
        End Using

        If _currentForm IsNot Nothing Then
            _currentForm.Close()
            _currentForm.Dispose()
            _currentForm = Nothing
        End If

        _trayIcon.Visible = False
        _trayIcon.Dispose()

        SetupTrayIcon(mode)

        If mode = "OCX" Then
            _currentForm = New frmOCX()
            AddHandler _currentForm.FormClosed, AddressOf OnFormClosed
            _currentForm.Show()
        End If
    End Sub

    Private Sub OnFormClosed(sender As Object, e As FormClosedEventArgs)
        Application.Exit()
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

    Private Function LoadSQLServerName() As String
        Try
            Dim filePath As String = Path.Combine(Application.StartupPath, "SQLValues.xml")
            Dim doc As New Xml.XmlDocument()
            doc.Load(filePath)
            Dim n = doc.SelectSingleNode("/Data/Database/txtSQLServer")
            If n IsNot Nothing Then Return n.InnerText.Trim()
        Catch
        End Try
        Return ""
    End Function

    Private Sub SaveSQLServerName(serverName As String)
        Try
            Dim filePath As String = Path.Combine(Application.StartupPath, "SQLValues.xml")
            Dim xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>" & Environment.NewLine &
                                "<Data><Database>" & Environment.NewLine &
                                "  <txtSQLServer>" & serverName & "</txtSQLServer>" & Environment.NewLine &
                                "</Database></Data>"
            File.WriteAllText(filePath, xml, System.Text.Encoding.UTF8)
        Catch
        End Try
    End Sub

End Module
