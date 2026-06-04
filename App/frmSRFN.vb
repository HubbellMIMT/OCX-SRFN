Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO

Public Class frmSRFN
    Inherits Form

    Public Event ModeSwitchRequested(mode As String)

    Private txtSetSQL As New TextBox()
    Private btnSetSQL As New Button()
    Private lblSetSQL As New Label()
    Private lblStatus As New Label()

    Public Sub New()
        Text = "SRFN"
        FormBorderStyle = FormBorderStyle.FixedSingle
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        ClientSize = New Size(300, 60)

        lblStatus.Location = New Point(12, 20)
        lblStatus.Size = New Size(276, 16)
        lblStatus.ForeColor = Color.DimGray

        Dim sqlName As String = LoadSQLServerName()
        lblStatus.Text = If(sqlName = "", "SQL server not configured", "SQL: " & sqlName)

        lblSetSQL.Text = "Set SQL:"
        lblSetSQL.Location = New Point(12, 70)
        lblSetSQL.AutoSize = True
        lblSetSQL.Visible = False

        txtSetSQL.Location = New Point(12, 88)
        txtSetSQL.Width = 194
        txtSetSQL.UseSystemPasswordChar = True
        txtSetSQL.Visible = False

        btnSetSQL.Text = "Set"
        btnSetSQL.Location = New Point(212, 87)
        btnSetSQL.Width = 76
        btnSetSQL.Visible = False
        AddHandler btnSetSQL.Click, AddressOf btnSetSQL_Click

        Controls.AddRange({lblStatus, lblSetSQL, txtSetSQL, btnSetSQL})
        AcceptButton = btnSetSQL
    End Sub

    Public Sub ShowSQLEntry()
        ClientSize = New Size(300, 120)
        lblSetSQL.Visible = True
        txtSetSQL.Visible = True
        btnSetSQL.Visible = True
        txtSetSQL.Focus()
    End Sub

    Private Sub btnSetSQL_Click(sender As Object, e As EventArgs)
        Dim entry As String = txtSetSQL.Text.Trim()
        Select Case entry.ToUpper()
            Case "TWACS"
                RaiseEvent ModeSwitchRequested("OCX")
            Case "SRFN"
                RaiseEvent ModeSwitchRequested("SRFN")
            Case ""
                ' nothing
            Case Else
                SRFN.Communication.CommManager2.SqlServerName = entry
                SaveSQLServerName(entry)
                lblStatus.Text = "SQL: " & entry
                txtSetSQL.Clear()
                ClientSize = New Size(300, 60)
                lblSetSQL.Visible = False
                txtSetSQL.Visible = False
                btnSetSQL.Visible = False
        End Select
    End Sub

    Private Function LoadSQLServerName() As String
        Try
            Dim filePath As String = Path.Combine(Application.StartupPath, "SQLValues.xml")
            Dim doc As New Xml.XmlDocument()
            doc.Load(filePath)
            Dim n = doc.SelectSingleNode("/Data/Database/txtSQLServer")
            If n IsNot Nothing Then
                SRFN.Communication.CommManager2.SqlServerName = n.InnerText.Trim()
                Return n.InnerText.Trim()
            End If
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

End Class
