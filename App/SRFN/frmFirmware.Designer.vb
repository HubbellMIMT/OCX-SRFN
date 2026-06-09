<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFirmware
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblProdFamily = New System.Windows.Forms.Label()
        Me.lblProdFamilyVal = New System.Windows.Forms.Label()
        Me.lblFirmware = New System.Windows.Forms.Label()
        Me.cboFirmware = New System.Windows.Forms.ComboBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnUpload = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnWrite = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnMassErase = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblProdFamily
        '
        Me.lblProdFamily.AutoSize = True
        Me.lblProdFamily.Location = New System.Drawing.Point(12, 15)
        Me.lblProdFamily.Text = "Product Family:"
        '
        'lblProdFamilyVal
        '
        Me.lblProdFamilyVal.AutoSize = True
        Me.lblProdFamilyVal.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblProdFamilyVal.Location = New System.Drawing.Point(120, 15)
        Me.lblProdFamilyVal.Text = ""
        '
        'lblFirmware
        '
        Me.lblFirmware.AutoSize = True
        Me.lblFirmware.Location = New System.Drawing.Point(12, 50)
        Me.lblFirmware.Text = "Write Firmware to NIC:"
        '
        'cboFirmware
        '
        Me.cboFirmware.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFirmware.FormattingEnabled = True
        Me.cboFirmware.Location = New System.Drawing.Point(12, 68)
        Me.cboFirmware.Size = New System.Drawing.Size(290, 21)
        Me.cboFirmware.TabIndex = 0
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(310, 67)
        Me.btnRefresh.Size = New System.Drawing.Size(62, 23)
        Me.btnRefresh.TabIndex = 1
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnWrite
        '
        Me.btnWrite.Location = New System.Drawing.Point(12, 103)
        Me.btnWrite.Size = New System.Drawing.Size(130, 25)
        Me.btnWrite.TabIndex = 2
        Me.btnWrite.Text = "Flash NIC Firmware"
        Me.btnWrite.UseVisualStyleBackColor = True
        '
        'btnMassErase
        '
        Me.btnMassErase.Location = New System.Drawing.Point(150, 103)
        Me.btnMassErase.Name = "btnMassErase"
        Me.btnMassErase.Size = New System.Drawing.Size(100, 25)
        Me.btnMassErase.TabIndex = 3
        Me.btnMassErase.Text = "Mass Erase"
        Me.btnMassErase.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(388, 103)
        Me.btnClose.Size = New System.Drawing.Size(70, 25)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnUpload
        '
        Me.btnUpload.Location = New System.Drawing.Point(100, 138)
        Me.btnUpload.Size = New System.Drawing.Size(176, 25)
        Me.btnUpload.TabIndex = 5
        Me.btnUpload.Text = "Upload Firmware to SQL Table"
        Me.btnUpload.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.MistyRose
        Me.btnDelete.Location = New System.Drawing.Point(284, 138)
        Me.btnDelete.Size = New System.Drawing.Size(174, 25)
        Me.btnDelete.TabIndex = 6
        Me.btnDelete.Text = "Delete selected Firmware"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'frmFirmware
        '
        Me.ClientSize = New System.Drawing.Size(470, 175)
        Me.Controls.Add(Me.lblProdFamily)
        Me.Controls.Add(Me.lblProdFamilyVal)
        Me.Controls.Add(Me.lblFirmware)
        Me.Controls.Add(Me.cboFirmware)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.btnUpload)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnWrite)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnMassErase)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Firmware"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblProdFamily As Label
    Friend WithEvents lblProdFamilyVal As Label
    Friend WithEvents lblFirmware As Label
    Friend WithEvents cboFirmware As ComboBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents btnUpload As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnWrite As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnMassErase As Button
End Class
