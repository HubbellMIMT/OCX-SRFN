Public Class frmFirmware

    Private _main As IMainForm

    Public Sub New(mainForm As IMainForm)
        InitializeComponent()
        _main = mainForm
    End Sub

    Private Sub frmFirmware_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblProdFamilyVal.Text = _main.ProductFamilyText
        LoadFirmwareList()
    End Sub

    Private Sub LoadFirmwareList()
        cboFirmware.Items.Clear()
        Dim productFamily As String = _main.ProductFamilyText
        If _main.SQLServer = "" OrElse productFamily = "" Then Return
        Dim fcs As String = _main.FirmwareConnStr
        If fcs = "" Then fcs = _main.DLLRevisionText
        If fcs <> "" Then SRFN.Communication.CommManager2.FirmwareConnStr = fcs
        Dim items As List(Of String) = SRFN.Communication.CommManager2.FetchFirmwareList(productFamily)
        For Each fn In items
            cboFirmware.Items.Add(fn)
        Next
        If cboFirmware.Items.Count > 0 Then cboFirmware.SelectedIndex = 0
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadFirmwareList()
    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        If _main.SQLServer = "" Then
            MsgBox("Connect to SQL Server first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return
        End If
        Dim productFamily As String = _main.ProductFamilyText
        If productFamily = "" Then
            MsgBox("Select a Product Family first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return
        End If
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select Firmware Hex File"
            ofd.Filter = "Hex Files (*.hex)|*.hex"
            ofd.InitialDirectory = IO.Path.Combine(Application.StartupPath, "RA6Files")
            If ofd.ShowDialog() <> DialogResult.OK Then Return
            Dim fileName As String = IO.Path.GetFileName(ofd.FileName)
            Dim data As Byte() = IO.File.ReadAllBytes(ofd.FileName)
            If SRFN.Communication.CommManager2.UploadFirmwareToSQL(productFamily, fileName, data) Then
                MsgBox(fileName & " uploaded for " & productFamily & ".", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
                If Not cboFirmware.Items.Contains(fileName) Then cboFirmware.Items.Add(fileName)
                cboFirmware.Text = fileName
            Else
                MsgBox("Upload failed — check the log for details.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If cboFirmware.SelectedItem Is Nothing Then
            MsgBox("Select a firmware file to delete.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return
        End If
        Dim fileName As String = cboFirmware.SelectedItem.ToString()
        Dim productFamily As String = _main.ProductFamilyText
        If MsgBox("Delete " & fileName & " (" & productFamily & ") from SQL?",
                  MsgBoxStyle.YesNo Or MsgBoxStyle.Question Or MsgBoxStyle.SystemModal, "Confirm Delete") <> MsgBoxResult.Yes Then Return
        If SRFN.Communication.CommManager2.DeleteFirmwareFromSQL(productFamily, fileName) Then
            cboFirmware.Items.Remove(cboFirmware.SelectedItem)
            If cboFirmware.Items.Count > 0 Then cboFirmware.SelectedIndex = 0
        Else
            MsgBox("Delete failed — check the log for details.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
        End If
    End Sub

    Private Sub btnWrite_Click(sender As Object, e As EventArgs) Handles btnWrite.Click
        If cboFirmware.SelectedItem Is Nothing Then
            MsgBox("Select a firmware version first.", MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
            Return
        End If
        _main.WriteFirmwareToNIC(cboFirmware.SelectedItem.ToString())
        Me.Close()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmFirmware_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _main.PasswordText = ""
    End Sub

    Private Async Sub btnMassErase_Click(sender As Object, e As EventArgs) Handles btnMassErase.Click
        Dim ok As Boolean = Await _main.VirginDelay()
        If Not ok Then
            MsgBox("VirginDelay failed — 'Signature erased' not received.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal)
            Return
        End If
        Await SRFN.Communication.CommManager2.ExecMassErase()
    End Sub

End Class
