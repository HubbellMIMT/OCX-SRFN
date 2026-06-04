Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml
Imports System.Windows.Forms

Public Module ModeSettings

    Private ReadOnly SettingsFile As String = Path.Combine(Application.StartupPath, "mode.xml")
    ' Default password is "switch" — change by calling SavePasswordHash
    Private Const DefaultHash As String = "EF797C8118F02DFB649607DD5D3F8C7623048C9C063D532CC95C5ED7A898A64F"

    Public Function GetMode() As String
        Try
            Dim doc As New XmlDocument()
            doc.Load(SettingsFile)
            Dim n = doc.SelectSingleNode("/Settings/Mode")
            If n IsNot Nothing Then Return n.InnerText.Trim()
        Catch
        End Try
        Return "SRFN"
    End Function

    Public Sub SaveMode(mode As String)
        Dim hash As String = DefaultHash
        Try
            Dim doc As New XmlDocument()
            doc.Load(SettingsFile)
            Dim h = doc.SelectSingleNode("/Settings/PasswordHash")
            If h IsNot Nothing Then hash = h.InnerText
        Catch
        End Try
        Dim xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>" & Environment.NewLine &
                            "<Settings>" & Environment.NewLine &
                            "  <Mode>" & mode & "</Mode>" & Environment.NewLine &
                            "  <PasswordHash>" & hash & "</PasswordHash>" & Environment.NewLine &
                            "</Settings>"
        File.WriteAllText(SettingsFile, xml, Encoding.UTF8)
    End Sub

    Public Function VerifyPassword(password As String) As Boolean
        Return Hash(password) = GetStoredHash()
    End Function

    Private Function GetStoredHash() As String
        Try
            Dim doc As New XmlDocument()
            doc.Load(SettingsFile)
            Dim n = doc.SelectSingleNode("/Settings/PasswordHash")
            If n IsNot Nothing Then Return n.InnerText.Trim()
        Catch
        End Try
        Return DefaultHash
    End Function

    Public Function Hash(value As String) As String
        Using sha As SHA256 = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "")
        End Using
    End Function

End Module
