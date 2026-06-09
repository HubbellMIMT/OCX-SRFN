Option Strict On
Option Explicit On
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml
Imports TWRN.Communication.TWRN
Public Class frmOCX
    Inherits System.Windows.Forms.Form
    'Dim lgmodule As New TWRN.Communication.TWRN.lgmoduleINT
    Private WithEvents lgmodule As TWRN.Communication.TWRN.lgmoduleINT
    'Dim xmlsql As New TWRN.Communication.TWRN.XML_Results
    Public GenIntCollection As New Collection
    Public sqlsvr As string
    Public customerConnStr As string
    Public integratConnStr As string
    Public resultssConnStr As string
    Public gTestResults_PCName As String
    Public gTestResults_DBAName As String
    Public gstrCnn_TestResults As String = ""
    Public ProdName(20) As String
    Public ProdType(20) As String
    Public gIntegrationDBA(20) As String
    Public gCustomerTestDBA(20) As String
    Public gProduct As String
    Public gIntDBA As String
    Public gCustDBA As String
    Public gRate9600 As String
    Public gRateMax As String
    Public Rate9600(20) As String
    Public RateMax(20) As String
    Public Prod(20) As String
    Public MeterType(50) As String
    Public Form_ID(50) As String
    Public ProductFamily As String 'Replaced gProductType As String 1-4-2010
    Public gPackPath As String = "C:\Pack\Aclara PLS"
    Public gHash As String
    Public ShowWeco As Boolean = False
    Public errcode As Integer = 0
    Public NonStandard As Boolean = False
    Friend WithEvents pu As RichTextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtMfg As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtcomOptical As TextBox
    Friend WithEvents chkRDMeter As CheckBox
    Friend WithEvents Pass As TextBox
    Friend WithEvents Label5 As Label
#Region " Windows Form Designer generated code "
    Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call
    End Sub
    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents btnIntegrationTest As System.Windows.Forms.Button
    Friend WithEvents lblLG_SN As System.Windows.Forms.Label
    Friend WithEvents txtLGSerialNum As System.Windows.Forms.TextBox
    Friend WithEvents txtCustomerID As System.Windows.Forms.TextBox
    Friend WithEvents txtDCSISerialNum As System.Windows.Forms.TextBox
    Friend WithEvents lblDCSI_SN As System.Windows.Forms.Label
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents lblTestStatus As System.Windows.Forms.Label
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents btnEEProm As System.Windows.Forms.Button
    Friend WithEvents txtpass As System.Windows.Forms.TextBox
    Friend WithEvents tbxFailMsg As System.Windows.Forms.RichTextBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents lbxForms As System.Windows.Forms.ComboBox
    Friend WithEvents lbxProduct As System.Windows.Forms.ComboBox
    Friend WithEvents lblListbox As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnServerConnect As System.Windows.Forms.Button
    Friend WithEvents txtcom As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents lblFailMsgLabel As System.Windows.Forms.Label
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents bxUMT As System.Windows.Forms.GroupBox
    Friend WithEvents chkNonStandard As System.Windows.Forms.CheckBox
    Protected WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents pnlFCT As System.Windows.Forms.Panel
    Friend WithEvents lbxProductFCT As System.Windows.Forms.ListBox
    Friend WithEvents btnCustomerID As System.Windows.Forms.Button
    Protected WithEvents lblcustomer As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.lblcustomer = New System.Windows.Forms.Label()
        Me.btnCustomerID = New System.Windows.Forms.Button()
        Me.btnIntegrationTest = New System.Windows.Forms.Button()
        Me.lblLG_SN = New System.Windows.Forms.Label()
        Me.txtLGSerialNum = New System.Windows.Forms.TextBox()
        Me.txtCustomerID = New System.Windows.Forms.TextBox()
        Me.txtDCSISerialNum = New System.Windows.Forms.TextBox()
        Me.lblDCSI_SN = New System.Windows.Forms.Label()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.lblTestStatus = New System.Windows.Forms.Label()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Pass = New System.Windows.Forms.TextBox()
        Me.btnEEProm = New System.Windows.Forms.Button()
        Me.txtpass = New System.Windows.Forms.TextBox()
        Me.tbxFailMsg = New System.Windows.Forms.RichTextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.chkNonStandard = New System.Windows.Forms.CheckBox()
        Me.chkRDMeter = New System.Windows.Forms.CheckBox()
        Me.lblProduct = New System.Windows.Forms.Label()
        Me.lbxForms = New System.Windows.Forms.ComboBox()
        Me.lbxProduct = New System.Windows.Forms.ComboBox()
        Me.lblListbox = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtcomOptical = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtMfg = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnServerConnect = New System.Windows.Forms.Button()
        Me.txtcom = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.bxUMT = New System.Windows.Forms.GroupBox()
        Me.lblFailMsgLabel = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.pnlFCT = New System.Windows.Forms.Panel()
        Me.lbxProductFCT = New System.Windows.Forms.ListBox()
        Me.pu = New System.Windows.Forms.RichTextBox()
        Me.GroupBox5.SuspendLayout
        Me.GroupBox7.SuspendLayout
        Me.GroupBox6.SuspendLayout
        Me.GroupBox4.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.pnlFCT.SuspendLayout
        Me.SuspendLayout
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.lblcustomer)
        Me.GroupBox5.Controls.Add(Me.btnCustomerID)
        Me.GroupBox5.Controls.Add(Me.btnIntegrationTest)
        Me.GroupBox5.Controls.Add(Me.lblLG_SN)
        Me.GroupBox5.Controls.Add(Me.txtLGSerialNum)
        Me.GroupBox5.Controls.Add(Me.txtCustomerID)
        Me.GroupBox5.Controls.Add(Me.txtDCSISerialNum)
        Me.GroupBox5.Controls.Add(Me.lblDCSI_SN)
        Me.GroupBox5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.GroupBox5.Location = New System.Drawing.Point(8, 256)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(160, 256)
        Me.GroupBox5.TabIndex = 114
        Me.GroupBox5.TabStop = false
        '
        'lblcustomer
        '
        Me.lblcustomer.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblcustomer.Location = New System.Drawing.Point(8, 16)
        Me.lblcustomer.Name = "lblcustomer"
        Me.lblcustomer.Size = New System.Drawing.Size(144, 16)
        Me.lblcustomer.TabIndex = 135
        Me.lblcustomer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnCustomerID
        '
        Me.btnCustomerID.Enabled = false
        Me.btnCustomerID.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnCustomerID.Location = New System.Drawing.Point(24, 40)
        Me.btnCustomerID.Name = "btnCustomerID"
        Me.btnCustomerID.Size = New System.Drawing.Size(112, 24)
        Me.btnCustomerID.TabIndex = 134
        Me.btnCustomerID.Text = "CustomerID"
        '
        'btnIntegrationTest
        '
        Me.btnIntegrationTest.Enabled = false
        Me.btnIntegrationTest.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnIntegrationTest.Location = New System.Drawing.Point(8, 208)
        Me.btnIntegrationTest.Name = "btnIntegrationTest"
        Me.btnIntegrationTest.Size = New System.Drawing.Size(136, 40)
        Me.btnIntegrationTest.TabIndex = 7
        Me.btnIntegrationTest.Text = "Integration Test"
        '
        'lblLG_SN
        '
        Me.lblLG_SN.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblLG_SN.Location = New System.Drawing.Point(8, 104)
        Me.lblLG_SN.Name = "lblLG_SN"
        Me.lblLG_SN.Size = New System.Drawing.Size(136, 16)
        Me.lblLG_SN.TabIndex = 76
        Me.lblLG_SN.Text = "Meter Serial Number"
        Me.lblLG_SN.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'txtLGSerialNum
        '
        Me.txtLGSerialNum.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtLGSerialNum.Location = New System.Drawing.Point(8, 120)
        Me.txtLGSerialNum.MaxLength = 10
        Me.txtLGSerialNum.Name = "txtLGSerialNum"
        Me.txtLGSerialNum.Size = New System.Drawing.Size(136, 22)
        Me.txtLGSerialNum.TabIndex = 5
        Me.txtLGSerialNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtCustomerID
        '
        Me.txtCustomerID.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtCustomerID.Location = New System.Drawing.Point(8, 72)
        Me.txtCustomerID.MaxLength = 8
        Me.txtCustomerID.Name = "txtCustomerID"
        Me.txtCustomerID.Size = New System.Drawing.Size(136, 22)
        Me.txtCustomerID.TabIndex = 4
        Me.txtCustomerID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtDCSISerialNum
        '
        Me.txtDCSISerialNum.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtDCSISerialNum.Location = New System.Drawing.Point(8, 176)
        Me.txtDCSISerialNum.MaxLength = 10
        Me.txtDCSISerialNum.Name = "txtDCSISerialNum"
        Me.txtDCSISerialNum.Size = New System.Drawing.Size(136, 22)
        Me.txtDCSISerialNum.TabIndex = 6
        Me.txtDCSISerialNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblDCSI_SN
        '
        Me.lblDCSI_SN.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblDCSI_SN.Location = New System.Drawing.Point(8, 152)
        Me.lblDCSI_SN.Name = "lblDCSI_SN"
        Me.lblDCSI_SN.Size = New System.Drawing.Size(144, 23)
        Me.lblDCSI_SN.TabIndex = 73
        Me.lblDCSI_SN.Text = "Aclara Serial Number"
        Me.lblDCSI_SN.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.lblTestStatus)
        Me.GroupBox7.Location = New System.Drawing.Point(176, 203)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(680, 117)
        Me.GroupBox7.TabIndex = 2
        Me.GroupBox7.TabStop = false
        '
        'lblTestStatus
        '
        Me.lblTestStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTestStatus.Location = New System.Drawing.Point(8, 8)
        Me.lblTestStatus.Name = "lblTestStatus"
        Me.lblTestStatus.Size = New System.Drawing.Size(666, 104)
        Me.lblTestStatus.TabIndex = 112
        Me.lblTestStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnExit
        '
        Me.btnExit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnExit.Location = New System.Drawing.Point(16, 520)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(144, 32)
        Me.btnExit.TabIndex = 111
        Me.btnExit.TabStop = false
        Me.btnExit.Text = "Exit"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Pass)
        Me.GroupBox6.Controls.Add(Me.btnEEProm)
        Me.GroupBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.GroupBox6.Location = New System.Drawing.Point(8, 203)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(160, 53)
        Me.GroupBox6.TabIndex = 115
        Me.GroupBox6.TabStop = false
        '
        'Pass
        '
        Me.Pass.BackColor = System.Drawing.SystemColors.Window
        Me.Pass.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Pass.Location = New System.Drawing.Point(86, 17)
        Me.Pass.MaxLength = 6
        Me.Pass.Name = "Pass"
        Me.Pass.Size = New System.Drawing.Size(68, 22)
        Me.Pass.TabIndex = 118
        Me.Pass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnEEProm
        '
        Me.btnEEProm.Enabled = false
        Me.btnEEProm.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnEEProm.Location = New System.Drawing.Point(8, 16)
        Me.btnEEProm.Name = "btnEEProm"
        Me.btnEEProm.Size = New System.Drawing.Size(72, 24)
        Me.btnEEProm.TabIndex = 113
        Me.btnEEProm.Text = "Run Script"
        '
        'txtpass
        '
        Me.txtpass.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtpass.Location = New System.Drawing.Point(307, 31)
        Me.txtpass.MaxLength = 15
        Me.txtpass.Name = "txtpass"
        Me.txtpass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtpass.Size = New System.Drawing.Size(49, 22)
        Me.txtpass.TabIndex = 112
        Me.txtpass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'tbxFailMsg
        '
        Me.tbxFailMsg.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.tbxFailMsg.Location = New System.Drawing.Point(176, 352)
        Me.tbxFailMsg.Name = "tbxFailMsg"
        Me.tbxFailMsg.ReadOnly = true
        Me.tbxFailMsg.Size = New System.Drawing.Size(680, 200)
        Me.tbxFailMsg.TabIndex = 116
        Me.tbxFailMsg.Text = ""
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.GroupBox3)
        Me.GroupBox4.Controls.Add(Me.GroupBox1)
        Me.GroupBox4.Controls.Add(Me.bxUMT)
        Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(8, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(848, 200)
        Me.GroupBox4.TabIndex = 113
        Me.GroupBox4.TabStop = false
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.chkNonStandard)
        Me.GroupBox3.Controls.Add(Me.chkRDMeter)
        Me.GroupBox3.Controls.Add(Me.lblProduct)
        Me.GroupBox3.Controls.Add(Me.lbxForms)
        Me.GroupBox3.Controls.Add(Me.lbxProduct)
        Me.GroupBox3.Controls.Add(Me.lblListbox)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(8, 16)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(216, 176)
        Me.GroupBox3.TabIndex = 72
        Me.GroupBox3.TabStop = false
        '
        'chkNonStandard
        '
        Me.chkNonStandard.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.chkNonStandard.Location = New System.Drawing.Point(31, 146)
        Me.chkNonStandard.Name = "chkNonStandard"
        Me.chkNonStandard.Size = New System.Drawing.Size(121, 24)
        Me.chkNonStandard.TabIndex = 0
        Me.chkNonStandard.Text = "Non-Standard"
        '
        'chkRDMeter
        '
        Me.chkRDMeter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.chkRDMeter.Location = New System.Drawing.Point(31, 125)
        Me.chkRDMeter.Name = "chkRDMeter"
        Me.chkRDMeter.Size = New System.Drawing.Size(88, 17)
        Me.chkRDMeter.TabIndex = 64
        Me.chkRDMeter.Text = "RD Meter"
        Me.chkRDMeter.Visible = false
        '
        'lblProduct
        '
        Me.lblProduct.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblProduct.Location = New System.Drawing.Point(8, 16)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(160, 16)
        Me.lblProduct.TabIndex = 62
        Me.lblProduct.Text = "Product Family"
        '
        'lbxForms
        '
        Me.lbxForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.lbxForms.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lbxForms.ItemHeight = 16
        Me.lbxForms.Location = New System.Drawing.Point(8, 80)
        Me.lbxForms.MaxDropDownItems = 9
        Me.lbxForms.Name = "lbxForms"
        Me.lbxForms.Size = New System.Drawing.Size(200, 24)
        Me.lbxForms.TabIndex = 2
        '
        'lbxProduct
        '
        Me.lbxProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.lbxProduct.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lbxProduct.ItemHeight = 16
        Me.lbxProduct.Location = New System.Drawing.Point(8, 32)
        Me.lbxProduct.Name = "lbxProduct"
        Me.lbxProduct.Size = New System.Drawing.Size(200, 24)
        Me.lbxProduct.TabIndex = 1
        '
        'lblListbox
        '
        Me.lblListbox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblListbox.Location = New System.Drawing.Point(8, 64)
        Me.lblListbox.Name = "lblListbox"
        Me.lblListbox.Size = New System.Drawing.Size(192, 16)
        Me.lblListbox.TabIndex = 57
        Me.lblListbox.Text = "Form   Voltage  Class  US/Can"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtpass)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.txtcomOptical)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.txtMfg)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.btnServerConnect)
        Me.GroupBox1.Controls.Add(Me.txtcom)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.Button3)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(481, 16)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(361, 176)
        Me.GroupBox1.TabIndex = 57
        Me.GroupBox1.TabStop = false
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label5.Location = New System.Drawing.Point(263, 13)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 16)
        Me.Label5.TabIndex = 117
        Me.Label5.Text = "Mfg"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label10.Location = New System.Drawing.Point(141, 13)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(50, 16)
        Me.Label10.TabIndex = 115
        Me.Label10.Text = "Wired"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label9
        '
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label9.Location = New System.Drawing.Point(193, 13)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(50, 16)
        Me.Label9.TabIndex = 114
        Me.Label9.Text = "Optical"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtcomOptical
        '
        Me.txtcomOptical.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtcomOptical.Location = New System.Drawing.Point(206, 30)
        Me.txtcomOptical.Name = "txtcomOptical"
        Me.txtcomOptical.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtcomOptical.Size = New System.Drawing.Size(24, 22)
        Me.txtcomOptical.TabIndex = 113
        Me.txtcomOptical.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label8.Location = New System.Drawing.Point(315, 13)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(41, 16)
        Me.Label8.TabIndex = 112
        Me.Label8.Text = "Pass"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtMfg
        '
        Me.txtMfg.BackColor = System.Drawing.SystemColors.Window
        Me.txtMfg.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtMfg.Location = New System.Drawing.Point(260, 30)
        Me.txtMfg.MaxLength = 6
        Me.txtMfg.Name = "txtMfg"
        Me.txtMfg.ReadOnly = true
        Me.txtMfg.Size = New System.Drawing.Size(41, 22)
        Me.txtMfg.TabIndex = 111
        Me.txtMfg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label4.Location = New System.Drawing.Point(70, 20)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 104
        Me.Label4.Text = "COM Port:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnServerConnect
        '
        Me.btnServerConnect.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btnServerConnect.Location = New System.Drawing.Point(196, 65)
        Me.btnServerConnect.Name = "btnServerConnect"
        Me.btnServerConnect.Size = New System.Drawing.Size(140, 52)
        Me.btnServerConnect.TabIndex = 0
        Me.btnServerConnect.Text = "Connect to SQL Server"
        '
        'txtcom
        '
        Me.txtcom.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.txtcom.Location = New System.Drawing.Point(154, 30)
        Me.txtcom.Name = "txtcom"
        Me.txtcom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtcom.Size = New System.Drawing.Size(24, 22)
        Me.txtcom.TabIndex = 102
        Me.txtcom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label15
        '
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 12!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label15.Location = New System.Drawing.Point(728, 168)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(72, 23)
        Me.Label15.TabIndex = 9
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(728, 128)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 1
        Me.Button3.Text = "Write XML"
        '
        'bxUMT
        '
        Me.bxUMT.Location = New System.Drawing.Point(232, 104)
        Me.bxUMT.Name = "bxUMT"
        Me.bxUMT.Size = New System.Drawing.Size(240, 88)
        Me.bxUMT.TabIndex = 73
        Me.bxUMT.TabStop = false
        Me.bxUMT.Visible = false
        '
        'lblFailMsgLabel
        '
        Me.lblFailMsgLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblFailMsgLabel.Location = New System.Drawing.Point(176, 328)
        Me.lblFailMsgLabel.Name = "lblFailMsgLabel"
        Me.lblFailMsgLabel.Size = New System.Drawing.Size(152, 23)
        Me.lblFailMsgLabel.TabIndex = 112
        Me.lblFailMsgLabel.Text = "Failure Message:"
        '
        'pnlFCT
        '
        Me.pnlFCT.Controls.Add(Me.lbxProductFCT)
        Me.pnlFCT.Location = New System.Drawing.Point(16, 296)
        Me.pnlFCT.Name = "pnlFCT"
        Me.pnlFCT.Size = New System.Drawing.Size(144, 152)
        Me.pnlFCT.TabIndex = 132
        Me.pnlFCT.Visible = false
        '
        'lbxProductFCT
        '
        Me.lbxProductFCT.Location = New System.Drawing.Point(8, 8)
        Me.lbxProductFCT.Name = "lbxProductFCT"
        Me.lbxProductFCT.Size = New System.Drawing.Size(128, 134)
        Me.lbxProductFCT.TabIndex = 109
        Me.lbxProductFCT.Visible = false
        '
        'pu
        '
        Me.pu.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.pu.Location = New System.Drawing.Point(176, 352)
        Me.pu.Name = "pu"
        Me.pu.ReadOnly = true
        Me.pu.Size = New System.Drawing.Size(680, 200)
        Me.pu.TabIndex = 133
        Me.pu.Text = ""
        '
        'frmOCX
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(864, 558)
        Me.ControlBox = false
        Me.Controls.Add(Me.pu)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.GroupBox6)
        Me.Controls.Add(Me.tbxFailMsg)
        Me.Controls.Add(Me.lblFailMsgLabel)
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.pnlFCT)
        Me.Name = "frmOCX"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.GroupBox5.ResumeLayout(false)
        Me.GroupBox5.PerformLayout
        Me.GroupBox7.ResumeLayout(false)
        Me.GroupBox6.ResumeLayout(false)
        Me.GroupBox6.PerformLayout
        Me.GroupBox4.ResumeLayout(false)
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.pnlFCT.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub

#End Region
Private Sub OCXDemo_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
Try
KeyPreview = True
If lgmodule Is Nothing Then
    lgmodule = New TWRN.Communication.TWRN.lgmoduleINT
End If
Me.Text = "Aclara Module Integration " & lgmodule.DLL
    lblTestStatus.Text = "Connect To SQL Server"
    lblTestStatus.BackColor = Color.LightBlue
    btnServerConnect.BackColor = Color.LightBlue
    'Me.TopMost = True
    lblTestStatus.BringToFront()
    bxUMT.SendToBack()
    Dim frm As New frmOCX
    frm.TopMost = True
    'frm.Show()
Catch ex As Exception
    MsgBox("Error OCXDemo_Load")
End Try
End Sub
'Private Sub btnIntegrationTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIntegrationTest.Click
'    IntegrationTest()
'    Me.Show()
'    Me.BringToFront()
'    Me.Visible = True
'End Sub
Private Async Sub btnIntegrationTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIntegrationTest.Click
'Private Sub IntegrationTest()
Try
Dim strTestresult As String
Dim varParampassed1 As String
Dim varParampassed2 As String
Dim MeterSerialNumber As String
Dim DCSISerialNumber As String
Dim SQLName As String
Dim CustomerID As String
Dim FormID As String
Dim RFCarrier As String = ""
Dim TestDescrip As String
Dim TestResult As String
Dim SerialRead As String
Dim SerialScanned As String
Dim meterfirmware As String = ""
If gPackPath = "" Then GetCommValues()
Dim RDInstalled As Boolean = chkRDMeter.Checked
Me.Hide()
lblTestStatus.BackColor = Color.White
lbxProduct.BackColor = Color.White
lbxForms.BackColor = Color.White
Me.Refresh()

lblTestStatus.Text = ""
lblFailMsgLabel.Visible = False
lblFailMsgLabel.Update()
pu.Text = ""
pu.Update()

ContinueTest:
If txtcom.Text = "" Then txtcom.Text = "1"
If txtLGSerialNum.Text = "" Then txtLGSerialNum.Text = ""
If txtDCSISerialNum.Text = "" Then txtDCSISerialNum.Text = "0"
Dim comport As Integer = CInt(txtcom.Text)
If Mid(lbxProduct.text,1,5) = "UMT-C-KV2" Then
    comport = CInt(txtcomOptical.text)
End If
MeterSerialNumber = txtLGSerialNum.Text
DCSISerialNumber = txtDCSISerialNum.Text
        'SQLName = txtServerName.Text
CustomerID = txtCustomerID.Text

If lbxForms.SelectedItem Is "" Then
    lblTestStatus.Text = "Select Form"
    lblTestStatus.BackColor = Color.Yellow
    lbxForms.Focus()
    'Return False
End If
FormID = Form_ID(lbxForms.SelectedIndex)
SQLName = sqlsvr

        '................NonStandard UMT
        If chkNonStandard.Checked Then
            'Dim ModelID As String = Mid(FormID, 1, 2)
            FormID = Hex(Val("&H" & Mid(FormID, 1, 2)) + 100) & Mid(FormID, 3, 3)
        End If

        If CustomerID = "" Then
            lblTestStatus.Text = "Customer ID Required"
            lblTestStatus.BackColor = Color.Yellow
            txtCustomerID.Focus()
            'Return False
        End If
        lblTestStatus.Text = ""
        pu.Text = ""
        If MeterSerialNumber = "" Then MeterSerialNumber = "0"
        ProductFamily = lbxProduct.SelectedItem.ToString
        lblTestStatus.BackColor = Color.Yellow
        lblTestStatus.Text = "Testing Unit"
        lblTestStatus.Update()
        'If lgmodule Is Nothing Then
        '    lgmodule = New TWRN.Communication.TWRN.lgmoduleINT
        'End If
        varParampassed1 = MeterSerialNumber & "," & DCSISerialNumber & "," & SQLName & "," & meterfirmware' & "," & chkNonStandard.Checked
        varParampassed2 = CustomerID & "," & ProductFamily & "," & FormID & "," & RFCarrier & "," & gPackPath & "," & RDInstalled
        '......................DO NOT MODIFY NEXT LINE or will break connection to WECO software
        'ORIGINAL = LG HACKED strTestresult = lgmodule.ACLARA_INTEGRATIONDotNetTest(nCommPort, varParampassed1, varParampassed2)
        'ORIG 8-23 = strTestresult = TWRN.Communication.TWRN.lgmoduleINT.ACLARA_INTEGRATIONDotNetTest(comport, varParampassed1, varParampassed2)
        strTestresult = Await lgmodule.ACLARA_INTEGRATIONDotNetTest(comport, varParampassed1, varParampassed2)
        gPackPath = lgmodule.Pack_Path
        SerialRead = lgmodule.SerialRead
        SerialScanned = lgmodule.SerialScanned
        TestDescrip = lgmodule.TestDescrip
        pu.Text = ""
        Me.Show()
        Me.BringToFront()
        Me.Visible = True
        If Mid(strTestresult, 1, 6) = "Passed" Then
            TestResult = "Pass" '..........................................MODULE PASSES
            lblFailMsgLabel.Visible = False
            lblTestStatus.Text = "* MODULE PASS *"
            lblTestStatus.BackColor = Color.LawnGreen
            pu.Text = TestDescrip & " PASSES INTEGRATION"
            pu.Text = "Pack Path = " & gPackPath
            'If ShowWeco = True Then pu.Text = pu.Text & vbLf & vbLf & "WECO Result=" & strTestresult
            'If lgmodule.ResetBaud() = True Then
            'End If

            'Return False
        Else
            TestResult = "Fail" '.............................................................MODULE FAILS
        End If
        If lgmodule.BadCommunication Then
            lblTestStatus.BackColor = Color.Red
            lblFailMsgLabel.Visible = True
            pu.Visible = True
            lblTestStatus.Text = "BAD COMMUNICATION"
            pu.Text = "Pack Path = " & gPackPath
            If ShowWeco Then pu.Text = pu.Text & vbLf & vbLf & "WECO Result=" & strTestresult
            pu.Text = pu.Text & vbLf & vbLf & lgmodule.CommandName & vbLf & vbLf & TestDescrip & " FAILS INTEGRATION"
            'If lgmodule.ResetBaud() = True Then
            'End If
            'Me.Show()
            'Me.BringToFront()
            'Me.Visible = True
            'Return False
        End If
        If TestResult = "Fail" Then '....................MODULE Fails
            lblTestStatus.BackColor = Color.Red
            lblFailMsgLabel.Visible = True

            If SerialRead <> SerialScanned Then
                lblTestStatus.Text = "SERIAL NUMBER MISMATCH" & vbLf & "      Serial Read: " & SerialRead & vbLf & "Serial Scanned: " & SerialScanned
            Else
                lblTestStatus.Text = "* MODULE FAIL *" & vblf & lgmodule.TestFailure
            End If

            pu.Text = "Pack Path = " & gPackPath
            pu.Text = pu.Text & vbLf & vbLf & "FAILS INTEGRATION" & vbLf & lgmodule.FailMessage & vbLf &  "Desc: " & TestDescrip
        End If
            'Me.Show()
            'Me.BringToFront()
            'Me.Visible = True
Catch ex As Exception
    MsgBox("IntegrationTest Form Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    Application.Exit()
    Me.Dispose()
End Try
End Sub
Public Function GetSQLValues() As object
GetSQLValues = Nothing
'Dim intparam As String = "SetIntegrationParameters" & "_" & txtMfg.Text & ".xml"
Dim dsEEParams As New System.Data.DataSet
Dim file_name As String
Try
    file_name = Application.StartupPath & "\Aclara_TestParameters\SetIntegrationParameters.xml"
    dsEEParams.ReadXml(file_name)
    sqlsvr = CType(dsEEParams.Tables("SQLValues").Rows(0).Item("sqlsvr"), String)
    customerConnStr = CType(dsEEParams.Tables("SQLValues").Rows(0).Item("customerConnStr"), String)
    integratConnStr = CType(dsEEParams.Tables("SQLValues").Rows(0).Item("integratConnStr"),String)
    resultssConnStr = CType(dsEEParams.Tables("SQLValues").Rows(0).Item("resultssConnStr"),String)
    dsEEParams.Dispose()
Catch ex As Exception
    MsgBox("SetIntegrationParameters GetSQLValues Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Function
Private Sub GetPortValues()
Try
    Dim doc As New XmlDocument()
    Dim xmldoc As String = Application.StartupPath & "\ApplicationValues\PortValues.xml"
    doc.Load(xmldoc)
    Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/PortValues")
    For Each node As XmlNode In nodes
    txtcom.Text = node.SelectSingleNode("serialportwired").InnerText
    txtcomOptical.Text = node.SelectSingleNode("serialportoptic").InnerText
    Next
Catch ex As Exception
    MsgBox("Error Reading GetPortValues.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Sub
Sub GetSerialPortNames()
    ' Show all available COM ports.
    Dim bdr As New System.Text.StringBuilder
    pu.Text = "Connected Serial Ports" & vbCr
    For Each sp As String In My.Computer.Ports.SerialPortNames
        bdr = bdr.Append(sp & vbCrLf)
    Next
    pu.Text = pu.Text & bdr.ToString
End Sub
Private Sub SetPortValues()
Try
    Dim stream_writer As New IO.StreamWriter(Application.StartupPath & "\ApplicationValues\PortValues.xml")
Try
    stream_writer.Write("<?xml version=" & """" & "1.0" & """" & " encoding=" & """" & "utf-8" & """" & "?>" & vbCr)
    stream_writer.Write("<PortValues>" & vbCr)
    stream_writer.Write("    <serialportwired>" & txtcom.Text & "</serialportwired>" & vbCr)
    stream_writer.Write("    <serialportoptic>" & txtcomOptical.Text & "</serialportoptic>" & vbCr)
    stream_writer.Write("</PortValues>" & vbCr)
     Finally
    stream_writer.Close()
End Try
Catch ex As Exception
    MsgBox("Error SetPortValues.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Sub
Private Sub GetCommValues()
pu.Clear
ShowWeco = True
'gHash = False
NonStandard = False
Try
'Dim showform As String = ""
txtpass.Text = ""
ProductFamily = ""
'gHash = False
NonStandard = False
ShowWeco = False
'gHash = False
GetPortValues()
Dim doc As New XmlDocument()
Dim xmldoc As String = Application.StartupPath & "\ApplicationValues\CommValues.xml"
doc.Load(xmldoc)
    Dim nodes As XmlNodeList = doc.DocumentElement.SelectNodes("/CommValues")
    For Each node As XmlNode In nodes
        'txtcom.Text = node.SelectSingleNode("serialportwired").InnerText
        'txtcomOptical.Text = node.SelectSingleNode("serialportoptic").InnerText
        txtMfg.Text = node.SelectSingleNode("SetIntegrationParameters").InnerText
        If node.SelectSingleNode("ShowWeco").InnerText = "True" Then ShowWeco = True
        if node.SelectSingleNode("Hash").InnerText = "Show" Then gHash = "True"
        if node.SelectSingleNode("NonStandard").InnerText = "True" Then NonStandard = True
        If node.SelectSingleNode("LockScreen").InnerText = "True" Then
            GroupBox3.Enabled = False
            GroupBox5.Enabled = False
        End If
        gPackPath = node.SelectSingleNode("Pack").InnerText
        'txtcom.Text = node.SelectSingleNode("ShowProgress").InnerText
        'txtcom.Text = node.SelectSingleNode("MAXBaud240").InnerText
        txtDCSISerialNum.Text = node.SelectSingleNode("TWACSID").InnerText
        txtCustomerID.Text = node.SelectSingleNode("CustomerID").InnerText
        txtpass.Text = node.SelectSingleNode("Script").InnerText
    Next
Catch ex As Exception
    MsgBox("Error Reading CommValues.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
End Try
End Sub
    Private Sub lbxProduct_TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbxProduct.SelectedValueChanged
        ProductChanged()
    End Sub
    Private Sub ProductChanged()
        lblTestStatus.Text = "Select Form"
        lblTestStatus.BackColor = Color.LightBlue
        lbxProduct.BackColor = Color.White
        lbxForms.BackColor = Color.LightBlue
        lbxForms.Focus()
        btnEEProm.Enabled = True
        lbxForms.Items.Clear()
        ProductFamily = CStr(lbxProduct.SelectedItem)
        bxUMT.BringToFront()
        FillFormsList(ProductFamily)
        'GetProductList()
        'GetFormList()
    End Sub
    Public Function GetProductList() As Boolean
        Dim strTemp1 As new stringbuilder
        Dim strTemp As String
        Dim MaxRef As Integer
        Dim i As integer
        Dim y As Integer
        Dim strHash As String
        Dim dsEEParams As New System.Data.DataSet
        Dim file_name As String
        Try
            If GenIntCollection.Count > 0 Then
                For y = 1 To GenIntCollection.Count
                    GenIntCollection.Remove(1)
                Next
            End If
            file_name = Application.StartupPath & "\Aclara_TestParameters\SetIntegrationParameters.xml"
            dsEEParams.ReadXml(file_name)
            strTemp = CStr(dsEEParams.Tables("HashTable").Rows(0).Item(0))
            For i = 0 To dsEEParams.Tables("Table").Rows.Count - 1
                For y = 0 To dsEEParams.Tables("Table").Columns.Count - 1
                    strTemp1.Append(dsEEParams.Tables("Table").Rows(i).Item(y))
                Next
            Next
            strHash = GenerateHash(strTemp1.ToString)
            If strHash <> strTemp Then
                If gHash = "True" Then
                    pu.Text = "Hash Error = " & strHash
                Else
                    pu.Text = "Hash Error has Value Error - Contact Aclara"
                End If
                MsgBox("GetProductList Hash Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
                Return False
            End If

SkipHash:
            MaxRef = dsEEParams.Tables("Table").Rows.Count - 1
            i = 0
            While (i <= MaxRef)
                ProdName(i) = CStr(dsEEParams.Tables("Table").Rows(i).Item("ProductName"))
                lbxProduct.Items.Add(ProdName(i))
                lbxProductFCT.Items.Add(ProdName(i))
                Prod(i) = CStr(dsEEParams.Tables("Table").Rows(i).Item("gProduct"))
                gProduct = Prod(i)
                gIntegrationDBA(i) = CStr(dsEEParams.Tables("Table").Rows(i).Item("gIntegrationDBA"))
                gIntDBA = gIntegrationDBA(i)
                gCustomerTestDBA(i) = CStr(dsEEParams.Tables("Table").Rows(i).Item("gCustomerTestDBA"))
                gCustDBA = gCustomerTestDBA(i)
                Rate9600(i) = CStr(dsEEParams.Tables("Table").Rows(i).Item("Rate9600"))
                gRate9600 = Rate9600(i)
                i = i + 1
            End While

            dsEEParams.Dispose()
            Return True
        Catch ex As Exception
            errcode = 2 'LG Process Error
            MsgBox("GetProductList Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End Try
    End Function
Private Function FillFormsList(ByVal SelectedProduct As String) as String
FillFormsList = Nothing
Dim strTemp1 As New Stringbuilder
Dim strTemp As String
Dim MaxRef As Integer
Dim i As Integer
Dim j As Integer
Dim y As Integer
Dim strHash As String
Dim dsEEParams As New System.Data.DataSet
Dim file_name As String
Dim ProdFound As Boolean = False
    Try
        If GenIntCollection.Count > 0 Then
            For y = 1 To GenIntCollection.Count
                GenIntCollection.Remove(1)
            Next
        End If
        file_name = Application.StartupPath & "\Aclara_TestParameters\FormList.xml"
        dsEEParams.ReadXml(file_name)

        strTemp = CStr(dsEEParams.Tables("HashTable").Rows(0).Item(0))
        For i = 0 To dsEEParams.Tables("Table").Rows.Count - 1
            For y = 0 To dsEEParams.Tables("Table").Columns.Count - 1
                strTemp1.Append(dsEEParams.Tables("Table").Rows(i).Item(y))
            Next
        Next
        strHash = GenerateHash(strTemp1.ToString)
        If strHash <> strTemp Then
            If gHash = "True" Then
                pu.Text = "FormList Hash = " & strHash
            Else
                pu.Text = "FormList.xml HashValue Error - Contact Aclara"
            End If
            Return Nothing
        End If

        MaxRef = dsEEParams.Tables("Table").Rows.Count - 1
        i = 0
        j = 0
        While (i <= MaxRef)
            strHash = CStr(dsEEParams.Tables("Table").Rows(i).Item("Product"))
            If SelectedProduct = CStr(dsEEParams.Tables("Table").Rows(i).Item("Product")) Then
                MeterType(j) = CStr(dsEEParams.Tables("Table").Rows(i).Item("MeterType"))
                Form_ID(j) = CStr(dsEEParams.Tables("Table").Rows(i).Item("FormID"))
                lbxForms.Items.Add(MeterType(j))
                ProdFound = True
                j = j + 1
            End If
            i = i + 1
        End While
        If ProdFound = False Then
            MsgBox("Product Not Found in FormsList.xml", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
        End If
        dsEEParams.Dispose()
    Catch ex As Exception
        errcode = 2 'LG Process Error
        MsgBox("FormsList Error", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information Or MsgBoxStyle.SystemModal)
    End Try
End Function
    Private Sub lbxForms_TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbxForms.SelectedValueChanged
        lblTestStatus.BackColor = Color.LightBlue
        lbxProduct.BackColor = Color.White
        lbxForms.BackColor = Color.White
    End Sub
    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Application.Exit()
        Me.Dispose()
    End Sub
    Private Sub txtCustomerID_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCustomerID.TextChanged
        If txtCustomerID.Text.Length > 5 Then
            lblTestStatus.Text = "Enter Meter Serial Number"
        End If
    End Sub
    Private Sub txtLGSerialNum_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLGSerialNum.TextChanged
        If txtLGSerialNum.Text.Length > 5 Then
            lblTestStatus.Text = "Enter TWACS Serial Number"
        End If
    End Sub
    Public Function GenerateHash(ByVal SourceText As String) As String
        Dim Ue As New UnicodeEncoding
        Dim ByteSourceText As Byte() = Ue.GetBytes(SourceText)
        Dim Md5 As New MD5CryptoServiceProvider
        Dim ByteHash As Byte() = Md5.ComputeHash(ByteSourceText)
        Return Convert.ToBase64String(ByteHash)
    End Function
Private Sub txtpass_TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtpass.TextChanged
'..................Check password in  txtpass
If txtpass.Text = "GET" Then
    GetSerialPortNames
    txtpass.Text = ""
    Return
End If
If txtpass.Text = "SET" Then
    SetPortValues
    txtpass.Text = ""
    Return
End If
If txtpass.Text = "CLR" Then
    ClearResults()
    txtpass.Text = ""
    Return
End If
        If txtpass.Text = "Pass3" Then
            pnlFCT.BringToFront()
            lbxProduct.Visible = False
            lblProduct.Visible = False
            btnEEProm.Enabled = False
            txtCustomerID.Visible = False
            txtLGSerialNum.Visible = False
            txtDCSISerialNum.Visible = False
            btnCustomerID.Visible = False
            lblLG_SN.Visible = False
            lblDCSI_SN.Visible = False
            lbxProductFCT.Visible = True
            'btnLoadFCTList.Visible = True
            pnlFCT.Visible = True
            lblTestStatus.Text = "Choose Product"
            lblTestStatus.BackColor = Color.LightBlue
            btnIntegrationTest.Text = "Initialize"
            lbxProductFCT.BackColor = Color.LightBlue
        Else
            pnlFCT.SendToBack()
            lbxProduct.Visible = True
            lblProduct.Visible = True
            txtDCSISerialNum.Visible = True
            lblDCSI_SN.Visible = True
            GroupBox1.Visible = True
            pnlFCT.Visible = False
            txtCustomerID.Visible = True
            txtLGSerialNum.Visible = True
            btnCustomerID.Visible = True
            lblLG_SN.Visible = True
            btnEEProm.Enabled = True
            GroupBox7.SendToBack()
            lblTestStatus.Visible = True
            lbxProductFCT.Visible = False
            btnIntegrationTest.Text = "Integration Test"
            lbxProduct.Visible = True
        End If

    End Sub
    Private Sub ClearResults()
        Try
            Dim file_name As String = Application.StartupPath & "\Aclara_TestParameters\Results.ini"
            Dim stream_writer As New IO.StreamWriter(file_name) ', False)
            Try
                stream_writer.Write("Pass=0" & vbLf & "Fail=0")
            Finally
                stream_writer.Close()
            End Try
            tbxFailMsg.Text = "Results.ini Cleared"
            txtpass.Text = ""
        Catch exc As Exception ' Report all errors.
            MsgBox(exc.Message, MsgBoxStyle.Exclamation, "cmtpassword.ini Error")
        End Try
    End Sub
    Private Sub btnCustomerID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomerID.Click
        Dim Customer As String = ""
        If txtCustomerID.Text = "" OrElse Mid(CStr(lbxProduct.SelectedItem), 1, 50).Trim = "" Then
            Exit Sub
        Else
            'Customer = lgmodule.GetCustomer(txtCustomerID.Text, Mid(lbxProduct.SelectedItem, 1, 50).Trim)
        End If

        If Customer = "" Then
            lblTestStatus.Text = "customer Not found"
        Else
            'lblTestStatus.Text = "Customer= " & Customer
            lblcustomer.Text = Customer
        End If
    End Sub
Private Sub btnServerConnect2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    'if ConnectSQL() = False Then
    '    Exit
    'End If
    'If lbxProduct.SelectedIndex = -1 Then Return
    lbxProduct.SelectedIndex = 0
    lbxForms.SelectedIndex = 0

    lbxProduct.SelectedIndex = 0
    lbxForms.SelectedIndex = 0
    txtCustomerID.Text = ""
    txtDCSISerialNum.text = ""
    btnIntegrationTest.Select

End Sub
    Private Sub btnServerConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnServerConnect.Click
    'Private Function FunctionConnectSQL() As Boolean
        lbxProductFCT.Items.Clear()
        lbxProduct.Items.Clear()
        lbxForms.Items.Clear()
        btnServerConnect.Text = "*CONNECTING*"
        btnServerConnect.BackColor = Color.Yellow
        btnServerConnect.Update()
        lbxProduct.BackColor = Color.White
        lbxForms.BackColor = Color.White
        GetCommValues()
        Try
            pu.Text = ""
            If lgmodule.SetSQLServer() = True Then
            btnServerConnect.Text = "*CONNECTED*"
            btnServerConnect.BackColor = Color.LawnGreen
            btnIntegrationTest.Enabled = True
            lblTestStatus.Text = "Select Product"
            If GroupBox3.Enabled = False Then lblTestStatus.Text = "Locked"
            lblTestStatus.BackColor = Color.LightBlue
            lbxProduct.Focus()
            txtpass.Enabled = True

            if GetProductList() = False Then
                btnServerConnect.Text = "*Not CONNECTED*"
                btnServerConnect.BackColor = Color.Red
                btnIntegrationTest.Enabled = False
            End If
            btnCustomerID.Enabled = False
            Else
                btnServerConnect.Text = "*Not CONNECTED*"
                btnServerConnect.BackColor = Color.Red
                btnIntegrationTest.Enabled = False
                pu.Text = lgmodule.FailMessage()
                txtpass.Enabled = False
                btnCustomerID.Enabled = False
            End If
        Me.BringToFront
        Startup()
        Catch ex As Exception
            MsgBox("ERR #" & Err.Number & " - " & Err.Description & vbCrLf & "Error Line #" & Err.Erl & ", Error Source: " & Err.Source, MsgBoxStyle.OkOnly, "Program Error - " & _
               "Routine: btnserver_Click")
        End Try
    End Sub
Private Sub Startup()
lbxProduct.SelectedIndex = -1 '-1.......... 3 .......AXe
lbxForms.SelectedIndex = -1 '-1........... 1 ........2S 240V
txtCustomerID.Text = "" '................. "" .......123456
txtDCSISerialNum.text = "" '.............. "" .......11111111
Pass.Text = ""
btnIntegrationTest.Select
End Sub
Private Async Sub btnEEProm_Click(sender As Object, e As EventArgs) Handles btnEEProm.Click
Dim ProductFamily As String = lbxProduct.SelectedItem.ToString
Dim comport As Integer = CInt(txtcom.Text)
If Pass.Text = "Dump" Then
            lblTestStatus.Text = "* Reading " & ProductFamily & " EEProm *"
            lblTestStatus.BackColor = Color.Yellow
    Dim result as string = ""
    result = Await lgmodule.DumpEEProm(comport, ProductFamily, txtDCSISerialNum.text)

    if Result = "True" Then
            lblFailMsgLabel.Visible = False
            lblTestStatus.Text = "* EEProm Dump Complete*"
            lblTestStatus.BackColor = Color.LawnGreen
    Else
            lblFailMsgLabel.Visible = True
            'Dim tmp as String = Await lgmodule.DumpEEProm(comport, ProductFamily, txtDCSISerialNum.text)
            lblTestStatus.Text = "* Fail EEProm Dump *" & vbLf & ProductFamily
            lblTestStatus.BackColor = Color.Yellow
    End if
            Me.Show()
            Pass.Text = ""
            Me.BringToFront()
            Me.Visible = True
    End If
End Sub
End Class
