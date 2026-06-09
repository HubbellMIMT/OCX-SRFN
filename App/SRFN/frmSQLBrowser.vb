Public Class frmSQLBrowser

    Private _server As String
    Private _initUserId As String
    Private _initPassword As String
    Public SelectedConnectionString As String = ""

    Public Sub New(server As String, Optional userId As String = "", Optional password As String = "")
        InitializeComponent()
        _server = server
        _initUserId = userId
        _initPassword = password
    End Sub

    Private Sub frmSQLBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblServer.Text = "Server:  " & _server
        txtUserId.Text = ""
        txtBrowsePwd.Text = ""
        btnRefresh.BackColor = Color.Yellow
        lblHint.Text = "Enter credentials and click Connect / Refresh"
        lblHint.ForeColor = System.Drawing.SystemColors.GrayText
    End Sub

    Private Function BuildAdoConnStr() As String
        Dim uid As String = txtUserId.Text.Trim()
        If uid <> "" Then
            Return "Provider=SQLOLEDB;Data Source=" & _server &
                   ";User ID=" & uid & ";Password=" & txtBrowsePwd.Text & ";"
        Else
            Return "Provider=SQLOLEDB;Data Source=" & _server & ";Integrated Security=SSPI;"
        End If
    End Function

    Private Sub LoadDatabases()
        cboDatabase.Items.Clear()
        cboTable.Items.Clear()
        txtPreview.Text = ""
        btnRefresh.BackColor = Color.Yellow
        Me.Refresh()
        Dim adoConn As New ADODB.Connection()
        Try
            adoConn.Open(BuildAdoConnStr())
            Dim rs As ADODB.Recordset = adoConn.Execute(
                "SELECT name FROM sys.databases " &
                "WHERE name NOT IN ('master','tempdb','model','msdb') " &
                "ORDER BY name")
            Do While Not rs.EOF
                cboDatabase.Items.Add(rs.Fields("name").Value.ToString())
                rs.MoveNext()
            Loop
            rs.Close()
            btnRefresh.BackColor = Color.LightGreen
            lblHint.Text = "(blank = Windows auth)"
            lblHint.ForeColor = System.Drawing.SystemColors.GrayText
        Catch ex As Exception
            btnRefresh.BackColor = Color.Red
            Dim msg As String = ex.Message
            If msg.Length > 80 Then msg = msg.Substring(0, 80) & "..."
            lblHint.Text = "Failed: " & msg
            lblHint.ForeColor = System.Drawing.Color.OrangeRed
        Finally
            Try
                If adoConn.State > 0 Then adoConn.Close()
            Catch
            End Try
        End Try
    End Sub

    Private Sub cboDatabase_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDatabase.SelectedIndexChanged
        cboTable.Items.Clear()
        UpdatePreview()
        If cboDatabase.SelectedItem Is Nothing Then Return
        Dim db As String = cboDatabase.SelectedItem.ToString()
        Dim cs As String = BuildAdoConnStr()
        If Not cs.ToLower.Contains("initial catalog=") Then
            cs = cs.TrimEnd(";"c) & ";Initial Catalog=" & db & ";"
        End If
        Dim adoConn As New ADODB.Connection()
        Try
            adoConn.Open(cs)
            Dim rs As ADODB.Recordset = adoConn.Execute(
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " &
                "WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME")
            Do While Not rs.EOF
                cboTable.Items.Add(rs.Fields("TABLE_NAME").Value.ToString())
                rs.MoveNext()
            Loop
            rs.Close()
        Catch ex As Exception
            MsgBox("Error loading tables: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
        Finally
            Try
                If adoConn.State > 0 Then adoConn.Close()
            Catch
            End Try
        End Try
    End Sub

    Private Sub cboTable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTable.SelectedIndexChanged
        UpdatePreview()
    End Sub

    Private Sub UpdatePreview()
        If cboDatabase.SelectedItem Is Nothing Then
            txtPreview.Text = ""
            Return
        End If
        Dim uid As String = txtUserId.Text.Trim()
        Dim db As String = cboDatabase.SelectedItem.ToString()
        If uid <> "" Then
            txtPreview.Text = "Provider=SQLOLEDB;Data Source=" & _server &
                              ";Initial Catalog=" & db &
                              ";User ID=" & uid & ";Password=" & txtBrowsePwd.Text & ";"
        Else
            txtPreview.Text = "Provider=SQLOLEDB;Data Source=" & _server &
                              ";Initial Catalog=" & db & ";Integrated Security=SSPI;"
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadDatabases()
    End Sub

    Private Sub btnUse_Click(sender As Object, e As EventArgs) Handles btnUse.Click
        If txtPreview.Text = "" Then
            MsgBox("Select a database first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return
        End If
        SelectedConnectionString = txtPreview.Text
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
