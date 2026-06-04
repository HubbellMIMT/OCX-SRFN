Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO

Public Class frmSRFN
    Inherits Form

    Public Event ModeSwitchRequested(mode As String)

    Private txtSQL As New TextBox()
    Private lblSQLValue As New Label()

    Public Sub New()
        Text = "SRFN"
        FormBorderStyle = FormBorderStyle.FixedSingle
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        ClientSize = New Size(360, 110)

        Dim grp As New GroupBox() With {
            .Text = "Configuration",
            .Location = New Point(12, 8),
            .Size = New Size(336, 90)
        }

        Dim lblSQL As New Label() With {
            .Text = "Set SQL:",
            .Location = New Point(10, 22),
            .AutoSize = True
        }

        txtSQL.Location = New Point(70, 19)
        txtSQL.Width = 180

        Dim btnSet As New Button() With {
            .Text = "Set",
            .Location = New Point(258, 18),
            .Width = 68
        }
        AddHandler btnSet.Click, AddressOf btnSet_Click

        lblSQLValue.Location = New Point(10, 55)
        lblSQLValue.Size = New Size(316, 16)
        lblSQLValue.ForeColor = Color.DimGray
        lblSQLValue.Text = LoadSQLServerName()

        grp.Controls.AddRange({lblSQL, txtSQL, btnSet, lblSQLValue})
        Controls.Add(grp)
        AcceptButton = btnSet
    End Sub

    Private Sub btnSet_Click(sender As Object, e As EventArgs)
        Dim entry As String = txtSQL.Text.Trim()
        Select Case entry.ToUpper()
            Case "TWACS"
                RaiseEvent ModeSwitchRequested("OCX")
            Case "SRFN"
                RaiseEvent ModeSwitchRequested("SRFN")
            Case ""
            Case Else
                SRFN.Communication.CommManager2.SqlServerName = entry
                SaveSQLServerName(entry)
                lblSQLValue.Text = "SQL: " & entry
                txtSQL.Clear()
        End Select
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
