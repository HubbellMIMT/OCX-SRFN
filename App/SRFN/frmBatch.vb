Public Class frmBatch

    Private _main As Form1
    Private _running As Boolean = False
    Private _totalCount As Integer = 0

    Public Sub New(mainForm As Form1)
        InitializeComponent()
        _main = mainForm
    End Sub

    Private Sub chkStopAtStep_CheckedChanged(sender As Object, e As EventArgs) Handles chkStopAtStep.CheckedChanged
        nudStopStep.Enabled = chkStopAtStep.Checked
    End Sub

    Private Sub chkWaitMs_CheckedChanged(sender As Object, e As EventArgs) Handles chkWaitMs.CheckedChanged
        nudWaitMs.Enabled = chkWaitMs.Checked
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        _totalCount = CInt(nudCount.Value)
        _running = True
        nudCount.Enabled = False
        nudStopStep.Enabled = False
        nudWaitMs.Enabled = False
        chkStopAtStep.Enabled = False
        chkWaitMs.Enabled = False
        btnStart.Enabled = False
        btnStop.Enabled = True
        rtbLog.Clear()
        RunBatch()
    End Sub

    Private Async Sub RunBatch()
        Dim passCount As Integer = 0
        Dim failCount As Integer = 0
        Dim runTimes As New List(Of Double)
        Dim failStepCounts As New Dictionary(Of String, Integer)

        Try
            Dim remaining As Integer = _totalCount
            Do While remaining > 0 AndAlso _running
                If Me.IsDisposed Then Return
                Dim attempt As Integer = _totalCount - remaining + 1

                SRFN.Communication.CommManager2.StopAtStep = If(chkStopAtStep.Checked, CInt(nudStopStep.Value), -1)

                rtbLog.AppendText("[" & attempt & "/" & _totalCount & "] Running...")
                rtbLog.ScrollToCaret()
                lblStatus.Text = "Running attempt " & attempt & " of " & _totalCount & "..."
                Me.Refresh()

                SRFN.Communication.CommManager2.BatchLogAction = AddressOf AppendBatchEvent
                Dim sw As New System.Diagnostics.Stopwatch()
                sw.Restart()
                Dim result As String = Await _main.RunTestRaw()
                sw.Stop()
                SRFN.Communication.CommManager2.BatchLogAction = Nothing
                If Me.IsDisposed Then Return
                SRFN.Communication.CommManager2.StopAtStep = -1

                Dim elapsedSec As Double = sw.Elapsed.TotalSeconds
                Dim passed As Boolean = Not result.StartsWith("Fail")
                Dim tag As String = If(passed, "PASS", "FAIL")

                Dim parts() As String = result.Split(New Char() {","c}, 3)
                Dim stepNum As String = If(parts.Length >= 2, parts(1).Trim(), "?")
                Dim stepCmd As String = ""
                If Not passed AndAlso parts.Length >= 3 Then
                    Dim cmdLines() As String = parts(2).Split(New Char() {Chr(13), Chr(10)}, StringSplitOptions.RemoveEmptyEntries)
                    If cmdLines.Length > 0 Then stepCmd = cmdLines(0).Trim()
                End If

                Dim lineDetail As String = " " & tag & "  step:" & stepNum & "  " & FormatSec(elapsedSec)
                If Not passed AndAlso stepCmd <> "" Then lineDetail &= "  " & stepCmd
                rtbLog.AppendText(lineDetail & Environment.NewLine)
                rtbLog.ScrollToCaret()

                If passed Then
                    passCount += 1
                Else
                    failCount += 1
                    Dim failKey As String = "step:" & stepNum & If(stepCmd <> "", " " & stepCmd, "")
                    If failStepCounts.ContainsKey(failKey) Then
                        failStepCounts(failKey) += 1
                    Else
                        failStepCounts(failKey) = 1
                    End If
                End If
                runTimes.Add(elapsedSec)

                remaining -= 1

                If remaining > 0 AndAlso _running Then
                    Dim waitMs As Integer = If(chkWaitMs.Checked, CInt(nudWaitMs.Value), 10000)
                    Dim waitSecs As Integer = Math.Max(1, CInt(Math.Round(waitMs / 1000.0)))
                    For i As Integer = waitSecs To 1 Step -1
                        If Not _running OrElse Me.IsDisposed Then Exit For
                        lblStatus.Text = "Attempt " & attempt & "/" & _totalCount & " " & tag & " — next in " & i & "s"
                        Me.Refresh()
                        Await Task.Delay(1000)
                        If Me.IsDisposed Then Return
                    Next
                End If
            Loop

            Dim completedRuns As Integer = _totalCount - remaining
            If completedRuns > 0 Then
                Dim sep As String = New String("─"c, 45)
                rtbLog.AppendText(Environment.NewLine & sep & Environment.NewLine)
                rtbLog.AppendText("SUMMARY  " & completedRuns & " runs:  " & passCount & " PASS  " & failCount & " FAIL" & Environment.NewLine)
                If runTimes.Count > 0 Then
                    Dim total As Double = 0
                    Dim mn As Double = Double.MaxValue
                    Dim mx As Double = Double.MinValue
                    For Each t As Double In runTimes
                        total += t
                        If t < mn Then mn = t
                        If t > mx Then mx = t
                    Next
                    rtbLog.AppendText("Time:   avg " & FormatSec(total / runTimes.Count) & "  min " & FormatSec(mn) & "  max " & FormatSec(mx) & Environment.NewLine)
                End If
                If failStepCounts.Count > 0 Then
                    rtbLog.AppendText("Fail steps:" & Environment.NewLine)
                    For Each kv As KeyValuePair(Of String, Integer) In failStepCounts
                        rtbLog.AppendText("  x" & kv.Value & "  " & kv.Key & Environment.NewLine)
                    Next
                End If
                rtbLog.ScrollToCaret()
                lblStatus.Text = If(_running, "Complete: ", "Stopped: ") & completedRuns & " runs  " & passCount & " PASS  " & failCount & " FAIL"
            Else
                lblStatus.Text = If(_running, "Complete — 0 runs", "Stopped.")
            End If

        Catch ex As Exception
            SRFN.Communication.CommManager2.StopAtStep = -1
            If Not Me.IsDisposed Then
                rtbLog.AppendText("ERROR: " & ex.Message & Environment.NewLine)
                lblStatus.Text = "Error: " & ex.Message
            End If
        Finally
            _running = False
            If Not Me.IsDisposed Then
                nudCount.Enabled = True
                nudStopStep.Enabled = chkStopAtStep.Checked
                nudWaitMs.Enabled = chkWaitMs.Checked
                chkStopAtStep.Enabled = True
                chkWaitMs.Enabled = True
                btnStart.Enabled = True
                btnStop.Enabled = False
            End If
        End Try
    End Sub

    Private Shared Function FormatSec(s As Double) As String
        Return s.ToString("0.0") & "s"
    End Function

    Private Sub AppendBatchEvent(msg As String)
        If Me.IsDisposed Then Return
        If rtbLog.InvokeRequired Then
            rtbLog.Invoke(Sub() AppendBatchEvent(msg))
        Else
            rtbLog.AppendText(msg & Environment.NewLine)
            rtbLog.ScrollToCaret()
        End If
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        _running = False
        lblStatus.Text = "Stopping after current run..."
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        _running = False
        SRFN.Communication.CommManager2.StopAtStep = -1
        Me.Close()
    End Sub

    Private Sub frmBatch_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _running = False
        SRFN.Communication.CommManager2.StopAtStep = -1
    End Sub

End Class
