Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO

Public Class frmSRFN
    Inherits Form

    Public Event ModeSwitchRequested(mode As String)

    Private txtSetSQL As New TextBox()
    Private btnSetSQL As New Button()
    Private lblSetSQL As New Label()
    Private lblSQLStatus As New Label()

    Public Sub New()
        Text = "SRFN"
        FormBorderStyle = FormBorderStyle.FixedSingle
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        ClientSize = New Size(320, 80)

        Dim lblTitle As New Label() With {
            .Text = "SRFN Protocol",
            .Location = New Point(12, 14),
            .AutoSize = True,
            .Font = New Font(Font.FontFamily, 11, FontStyle.Bold)
        }

        lblSQLStatus.Location = New Point(12, 44)
        lblSQLStatus.Size = New Size(296, 16)
        lblSQLStatus.ForeColor = Color.DimGray
        lblSQLStatus.Text = LoadSQLServerName()

        lblSetSQL.Text = "Set SQL Server:"
        lblSetSQL.Location = New Point(12, 90)
        lblSetSQL.AutoSize = True
        lblSetSQL.Visible = False

        txtSetSQL.Location = New Point(12, 108)
        txtSetSQL.Width = 212
        txtSetSQL.Visible = False

        btnSetSQL.Text = "Set"
        btnSetSQL.Location = New Point(230, 107)
        btnSetSQL.Width = 78
        btnSetSQL.Visible = False
        AddHandler btnSetSQL.Click, AddressOf btnSetSQL_Click

        Controls.AddRange({lblTitle, lblSQLStatus, lblSetSQL, txtSetSQL, btnSetSQL})
        AcceptButton = btnSetSQL
    End Sub

    Public Sub ShowSQLEntry()
        ClientSize = New Size(320, 140)
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
                HideSQLEntry()
            Case Else
                SRFN.Communication.CommManager2.SqlServerName = entry
                SaveSQLServerName(entry)
                lblSQLStatus.Text = "SQL: " & entry
                txtSetSQL.Clear()
                HideSQLEntry()
        End Select
    End Sub

    Private Sub HideSQLEntry()
        ClientSize = New Size(320, 80)
        lblSetSQL.Visible = False
        txtSetSQL.Visible = False
        btnSetSQL.Visible = False
    End Sub

    Private Function LoadSQLServerName() As String
        Try
            Dim filePath As String = Path.Combine(Application.StartupPath, "SQLValues.xml")
            Dim doc As New Xml.XmlDocument()
            doc.Load(filePath)
            Dim n = doc.SelectSingleNode("/Data/Database/txtSQLServer")
            If n IsNot Nothing AndAlso n.InnerText.Trim() <> "" Then
                SRFN.Communication.CommManager2.SqlServerName = n.InnerText.Trim()
                Return "SQL: " & n.InnerText.Trim()
            End If
        Catch
        End Try
        Return "SQL server not configured"
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
