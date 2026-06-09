Imports System.IO

Public Module AppConfig

    Private ReadOnly IniPath As String = Path.Combine(Application.StartupPath, "app.ini")

    Public Function ReadKey(key As String) As String
        Try
            For Each line As String In File.ReadAllLines(IniPath)
                Dim parts = line.Split({"="c}, 2)
                If parts.Length = 2 AndAlso parts(0).Trim().ToLower() = key.ToLower() Then
                    Return parts(1).Trim()
                End If
            Next
        Catch
        End Try
        Return ""
    End Function

    Public Sub WriteKey(key As String, value As String)
        Dim lines As New List(Of String)
        Dim found As Boolean = False
        Try
            lines.AddRange(File.ReadAllLines(IniPath))
        Catch
        End Try
        For i As Integer = 0 To lines.Count - 1
            Dim parts = lines(i).Split({"="c}, 2)
            If parts.Length >= 1 AndAlso parts(0).Trim().ToLower() = key.ToLower() Then
                lines(i) = key & "=" & value
                found = True
                Exit For
            End If
        Next
        If Not found Then lines.Add(key & "=" & value)
        File.WriteAllLines(IniPath, lines.ToArray(), System.Text.Encoding.UTF8)
    End Sub

End Module
