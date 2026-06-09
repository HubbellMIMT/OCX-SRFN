Public Class frmSQLConfig

    Private _main As Form1

    Public Sub New(mainForm As Form1)
        InitializeComponent()
        _main = mainForm
    End Sub

    Private Sub frmSQLConfig_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtDLLRevision.Text = _main.DLL_Revision.Text
        txtCustomerValues.Text = _main.SRFN_CustomerValues.Text
        txtTestResults.Text = _main.SRFN_TestResults.Text
        txtFirmware.Text = _main.Aclara_FirmwareConnStr
        txtServer.Text = _main.txtSQLServer.Text
        chkShowForm.Checked = _main.chkShowForm.Checked
        txtRA6Path.Text = _main.RA6ProgPath.Text
    End Sub

    Private Sub CopyToMain()
        _main.DLL_Revision.Text = txtDLLRevision.Text
        _main.SRFN_CustomerValues.Text = txtCustomerValues.Text
        _main.SRFN_TestResults.Text = txtTestResults.Text
        _main.Aclara_FirmwareConnStr = txtFirmware.Text
        _main.txtSQLServer.Text = txtServer.Text
        _main.SQLServer = txtServer.Text
        _main.chkShowForm.Checked = chkShowForm.Checked
        _main.RA6ProgPath.Text = txtRA6Path.Text
        SRFN.Communication.CommManager2.CustomerValuesConnStr = txtCustomerValues.Text.Trim()
        SRFN.Communication.CommManager2.TestResultsConnStr = txtTestResults.Text.Trim()
        SRFN.Communication.CommManager2.DLLRevisionConnStr = txtDLLRevision.Text.Trim()
        SRFN.Communication.CommManager2.FirmwareConnStr = txtFirmware.Text.Trim()
    End Sub

    ' ── Check buttons ────────────────────────────────────────────────────────

    Private Sub btnChkDLL_Click(sender As Object, e As EventArgs) Handles btnChkDLL.Click
        btnChkDLL.BackColor = Color.Yellow
        Me.Refresh()
        btnChkDLL.BackColor = If(TestSQLConnection(txtDLLRevision.Text, "DLL_Revision", "DLL Rev"), Color.LightGreen, Color.Red)
    End Sub

    Private Sub btnChkCustomer_Click(sender As Object, e As EventArgs) Handles btnChkCustomer.Click
        btnChkCustomer.BackColor = Color.Yellow
        Me.Refresh()
        btnChkCustomer.BackColor = If(TestSQLConnection(txtCustomerValues.Text, "SRFN_Customer_Values", "Customer Values"), Color.LightGreen, Color.Red)
    End Sub

    Private Sub btnChkResults_Click(sender As Object, e As EventArgs) Handles btnChkResults.Click
        btnChkResults.BackColor = Color.Yellow
        Me.Refresh()
        btnChkResults.BackColor = If(TestSQLConnection(txtTestResults.Text, "Integration_TestResults", "Test Results"), Color.LightGreen, Color.Red)
    End Sub

    Private Sub btnChkFirmware_Click(sender As Object, e As EventArgs) Handles btnChkFirmware.Click
        btnChkFirmware.BackColor = Color.Yellow
        Me.Refresh()
        btnChkFirmware.BackColor = If(TestSQLConnection(txtFirmware.Text, "Aclara_Firmware", "Firmware"), Color.LightGreen, Color.Red)
    End Sub

    Private Function TestSQLConnection(adoConnStr As String, tableName As String, label As String) As Boolean
        Dim log As New System.Text.StringBuilder()
        log.AppendLine("── " & label & " ──────────────────────────")
        log.AppendLine("Connection String:")
        log.AppendLine("  " & If(adoConnStr.Trim() = "", "(empty)", adoConnStr))

        If adoConnStr.Trim() = "" Then
            log.AppendLine("RESULT: FAILED — connection string is empty")
            txtConnLog.Text = log.ToString()
            txtConnLog.SelectionStart = txtConnLog.Text.Length
            txtConnLog.ScrollToCaret()
            Return False
        End If

        log.AppendLine("Table:  [dbo].[" & tableName & "]")
        log.AppendLine("Step 1: Opening ADODB connection...")
        txtConnLog.Text = log.ToString()
        Me.Refresh()

        Dim adoConn As New ADODB.Connection()
        Try
            adoConn.Open(adoConnStr)
            log.AppendLine("Step 2: Connection opened OK")
            log.AppendLine("Step 3: Executing SELECT TOP 1 1 FROM [dbo].[" & tableName & "]...")
            txtConnLog.Text = log.ToString()
            Me.Refresh()

            adoConn.Execute("SELECT TOP 1 1 FROM [dbo].[" & tableName & "]")
            log.AppendLine("RESULT: SUCCESS")
            txtConnLog.Text = log.ToString()
            txtConnLog.SelectionStart = txtConnLog.Text.Length
            txtConnLog.ScrollToCaret()
            Return True
        Catch ex As Exception
            log.AppendLine("RESULT: FAILED")
            log.AppendLine("Error:  " & ex.Message)
            txtConnLog.Text = log.ToString()
            txtConnLog.SelectionStart = txtConnLog.Text.Length
            txtConnLog.ScrollToCaret()
            Return False
        Finally
            Try
                If adoConn.State > 0 Then adoConn.Close()
            Catch
            End Try
        End Try
    End Function

    ' ── Browse buttons ───────────────────────────────────────────────────────

    Private Sub btnBrowseDLL_Click(sender As Object, e As EventArgs) Handles btnBrowseDLL.Click
        txtDLLRevision.Text = ""
        ResetConnectState()
        Dim cs As String = BrowseSQL(txtDLLRevision.Text)
        If cs <> "" Then
            txtDLLRevision.Text = cs
            Save()
        End If
    End Sub

    Private Sub btnBrowseCustomer_Click(sender As Object, e As EventArgs) Handles btnBrowseCustomer.Click
        txtCustomerValues.Text = ""
        ResetConnectState()
        Dim cs As String = BrowseSQL(txtCustomerValues.Text)
        If cs <> "" Then
            txtCustomerValues.Text = cs
            Save()
        End If
    End Sub

    Private Sub btnBrowseResults_Click(sender As Object, e As EventArgs) Handles btnBrowseResults.Click
        txtTestResults.Text = ""
        ResetConnectState()
        Dim cs As String = BrowseSQL(txtTestResults.Text)
        If cs <> "" Then
            txtTestResults.Text = cs
            Save()
        End If
    End Sub

    Private Sub btnBrowseFirmware_Click(sender As Object, e As EventArgs) Handles btnBrowseFirmware.Click
        txtFirmware.Text = ""
        ResetConnectState()
        Dim cs As String = BrowseSQL(txtFirmware.Text)
        If cs <> "" Then
            txtFirmware.Text = cs
            Save()
        End If
    End Sub

    Private Sub ResetConnectState()
        txtConnLog.Text = ""
    End Sub

    Private Function BrowseSQL(Optional existingConnStr As String = "") As String
        ' Use server from existing string (preserves instance name), fall back to txtServer
        Dim server As String = ParseConnStrKey(existingConnStr, "Data Source")
        If server = "" Then server = txtServer.Text.Trim()
        If server = "" Then
            MsgBox("Enter a SQL Server name first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return ""
        End If
        ' Always open with blank credentials — user enters them fresh in the Browse dialog
        Using frm As New frmSQLBrowser(server)
            If frm.ShowDialog(Me) = DialogResult.OK Then Return frm.SelectedConnectionString
        End Using
        Return ""
    End Function

    Private Function ParseConnStrKey(connStr As String, key As String) As String
        For Each part As String In connStr.Split(";"c)
            Dim eq As Integer = part.IndexOf("="c)
            If eq > 0 Then
                If String.Equals(part.Substring(0, eq).Trim(), key, StringComparison.OrdinalIgnoreCase) Then
                    Return part.Substring(eq + 1).Trim()
                End If
            End If
        Next
        Return ""
    End Function

    ' ── RA6 path check ───────────────────────────────────────────────────────

    Private Sub btnChkRA6Path_Click(sender As Object, e As EventArgs) Handles btnChkRA6Path.Click
        btnChkRA6Path.BackColor = SystemColors.Control
        Me.Refresh()

        Dim current As String = txtRA6Path.Text.Trim()
        If current <> "" AndAlso IO.File.Exists(IO.Path.Combine(current, "rfp-cli.exe")) Then
            btnChkRA6Path.BackColor = Color.LightGreen
            _main.RA6ProgPath.Text = current
            Return
        End If

        Dim searchRoots As String() = {
            "C:\Program Files (x86)\Renesas Electronics\Programming Tools",
            "C:\Program Files\Renesas Electronics\Programming Tools"
        }
        For Each root As String In searchRoots
            If IO.Directory.Exists(root) Then
                Dim found As String() = IO.Directory.GetFiles(root, "rfp-cli.exe", IO.SearchOption.AllDirectories)
                If found.Length > 0 Then
                    txtRA6Path.Text = IO.Path.GetDirectoryName(found(0))
                    btnChkRA6Path.BackColor = Color.LightGreen
                    _main.RA6ProgPath.Text = txtRA6Path.Text
                    Save()
                    Return
                End If
            End If
        Next

        btnChkRA6Path.BackColor = Color.Red
        MsgBox("rfp-cli.exe not found. Install Renesas Flash Programmer or set the path manually.",
               MsgBoxStyle.Exclamation Or MsgBoxStyle.SystemModal)
    End Sub

    ' ── Load / Save ConnStr.txt ──────────────────────────────────────────────

    Private Function GetConnStrPath() As String
        Return IO.Path.Combine(Application.StartupPath, "ConnStr.txt")
    End Function

    Private Sub btnLoadConnStr_Click(sender As Object, e As EventArgs) Handles btnLoadConnStr.Click
        Dim path As String = GetConnStrPath()
        If Not IO.File.Exists(path) Then
            MsgBox("ConnStr.txt not found: " & path, MsgBoxStyle.Exclamation Or MsgBoxStyle.SystemModal)
            Return
        End If
        Try
            txtDLLRevision.Text = ""
            txtCustomerValues.Text = ""
            txtTestResults.Text = ""
            txtFirmware.Text = ""
            txtServer.Text = ""

            For Each line As String In IO.File.ReadAllLines(path)
                Dim t As String = line.Trim()
                If t = "" OrElse t.StartsWith(";") Then Continue For
                Dim eq As Integer = t.IndexOf("="c)
                If eq < 1 Then Continue For
                Dim key As String = t.Substring(0, eq).Trim().ToLower()
                Dim val As String = t.Substring(eq + 1).Trim()
                Select Case key
                    Case "dllrevision", "dll_revision", "dll_rev"
                        txtDLLRevision.Text = val
                    Case "customervalues", "srfn_customervalues", "customer"
                        txtCustomerValues.Text = val
                    Case "testresults", "srfn_testresults"
                        txtTestResults.Text = val
                    Case "firmware", "firmwareconn", "aclara_firmwareconn"
                        txtFirmware.Text = val
                    Case "sqlserver", "txtsqlserver", "sql_server"
                        txtServer.Text = val
                End Select
            Next

            If txtCustomerValues.Text = "" AndAlso txtDLLRevision.Text <> "" Then
                txtCustomerValues.Text = txtDLLRevision.Text
            End If
            If txtFirmware.Text = "" AndAlso txtDLLRevision.Text <> "" Then
                txtFirmware.Text = txtDLLRevision.Text
            End If
        Catch ex As Exception
            MsgBox("Error reading ConnStr.txt: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
        End Try
    End Sub

    ' ── Save All / Close ─────────────────────────────────────────────────────

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Save()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmSQLConfig_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Save()
    End Sub

    Private Sub Save()
        _main.txtPassword.Text = ""
        CopyToMain()

        ' Save connection strings to ConnStr.txt
        Try
            Dim path As String = GetConnStrPath()
            Dim lines As New List(Of String) From {
                "SQLServer=" & txtServer.Text,
                "DLLRevision=" & txtDLLRevision.Text,
                "CustomerValues=" & txtCustomerValues.Text,
                "TestResults=" & txtTestResults.Text,
                "Firmware=" & txtFirmware.Text
            }
            IO.File.WriteAllLines(path, lines.ToArray())
        Catch ex As Exception
            MsgBox("Save to ConnStr.txt failed: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
        End Try

        ' Save to SQLValues.xml for app startup
        Try
            Dim path As String = IO.Path.Combine(Application.StartupPath, "SQLValues.xml")
            Dim s As New Xml.XmlWriterSettings() With {.Indent = True}
            Using w As Xml.XmlWriter = Xml.XmlWriter.Create(path, s)
                w.WriteStartDocument()
                w.WriteStartElement("Data")
                w.WriteStartElement("Database")
                w.WriteElementString("DLL_Revision", txtDLLRevision.Text)
                w.WriteElementString("SRFN_CustomerValues", txtCustomerValues.Text)
                w.WriteElementString("SRFN_TestResults", txtTestResults.Text)
                w.WriteElementString("Aclara_FirmwareConn", txtFirmware.Text)
                w.WriteElementString("txtSQLServer", txtServer.Text)
                w.WriteElementString("ShowForm", If(chkShowForm.Checked, "True", "False"))
                w.WriteElementString("debugtest", "")
                w.WriteEndElement()
                w.WriteEndElement()
                w.WriteEndDocument()
            End Using
        Catch ex As Exception
            MsgBox("Save to SQLValues.xml failed: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
        End Try
    End Sub

End Class
