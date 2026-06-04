Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Windows.Forms

Public Module ModeSettings

    Private ReadOnly SettingsFile As String = Path.Combine(Application.StartupPath, "mode.xml")

    Public Function GetMode() As String
        Try
            Dim doc As New XmlDocument()
            doc.Load(SettingsFile)
            Dim n = doc.SelectSingleNode("/Settings/Mode")
            If n IsNot Nothing Then
                Dim m = n.InnerText.Trim().ToUpper()
                If m = "SRFN" OrElse m = "OCX" Then Return m
            End If
        Catch
        End Try
        Return ""
    End Function

    ''' <summary>
    ''' Accepts "SRFN" or "TWACS" (case-insensitive).
    ''' Saves the matching mode and returns True; returns False for any other value.
    ''' </summary>
    Public Function SetModeByPassword(password As String) As Boolean
        Select Case password.Trim().ToUpper()
            Case "SRFN"
                SaveMode("SRFN")
                Return True
            Case "TWACS"
                SaveMode("OCX")
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Sub SaveMode(mode As String)
        Dim xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>" & Environment.NewLine &
                            "<Settings>" & Environment.NewLine &
                            "  <Mode>" & mode & "</Mode>" & Environment.NewLine &
                            "</Settings>"
        File.WriteAllText(SettingsFile, xml, Encoding.UTF8)
    End Sub

    Public Sub ClearMode()
        Try
            File.Delete(SettingsFile)
        Catch
        End Try
    End Sub

End Module
