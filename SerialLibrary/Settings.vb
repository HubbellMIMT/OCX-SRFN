Imports System.CodeDom.Compiler
Imports System.Configuration
Imports System.IO.Ports
Imports System.Runtime.CompilerServices
Namespace My
	<GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0"), CompilerGenerated>
	Friend NotInheritable Class Settings
		Inherits ApplicationSettingsBase
		Private Shared defaultInstance As Settings = CType(SettingsBase.Synchronized(New Settings()), Settings)
		Public Shared ReadOnly Property [Default]() As Settings
			Get
				Return defaultInstance
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("8"), DebuggerNonUserCode>
		Public ReadOnly Property DataBits() As Integer
			Get
				Return DirectCast(Me("DataBits"), Integer)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("One"), DebuggerNonUserCode>
		Public ReadOnly Property StopBits() As StopBits
			Get
				Return DirectCast(Me("StopBits"), StopBits)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("None"), DebuggerNonUserCode>
		Public ReadOnly Property Parity() As Parity
			Get
				Return DirectCast(Me("Parity"), Parity)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("ECDHE-ECDSA-AES256-CCM-8"), DebuggerNonUserCode>
		Public ReadOnly Property CipherSuiteList() As String
			Get
				Return DirectCast(Me("CipherSuiteList"), String)
			End Get
		End Property
		<ApplicationScopedSetting, DefaultSettingValue("C:\Development\SRFN Projects\SFN DTLS Projects\EP Integration Test v3.3.12\SRFN.TestApp\bin\Debug\CertKeys\dtlsMfgRootCA.pem"), DebuggerNonUserCode>
		Public ReadOnly Property CertificateAuthorityFile() As String '
			Get
				Return DirectCast(Me("CertificateAuthorityFile"), String)
			End Get
		End Property


		<ApplicationScopedSetting, DefaultSettingValue("C:\Development\SRFN Projects\SFN DTLS Projects\EP Integration Test v3.3.12\SRFN.TestApp\bin\Debug\CertKeys\MFG-PrivKey.pem"), DebuggerNonUserCode>
		Public ReadOnly Property PrivateKeyFile() As String
			Get
				Return DirectCast(Me("PrivateKeyFile"), String)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("C:\Development\SRFN Projects\SFN DTLS Projects\EP Integration Test v3.3.12\SRFN.TestApp\bin\Debug\CertKeys\MFGChain.pem"), DebuggerNonUserCode>
		Public ReadOnly Property CertFile() As String
			Get
				Return DirectCast(Me("CertFile"), String)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("10"), DebuggerNonUserCode>
		Public ReadOnly Property TransportTimeoutSeconds() As UInteger
			Get
				Return DirectCast(Me("TransportTimeoutSeconds"), UInteger)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("0"), DebuggerNonUserCode>
		Public ReadOnly Property DelayMilliseconds() As Integer
			Get
				Return DirectCast(Me("DelayMilliseconds"), Integer)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("False"), DebuggerNonUserCode>
		Public ReadOnly Property Debug() As Boolean
			Get
				Return DirectCast(Me("Debug"), Boolean)
			End Get
		End Property

		<ApplicationScopedSetting, DefaultSettingValue("True"), DebuggerNonUserCode>
		Public ReadOnly Property GroupMessages() As Boolean
			Get
				Return DirectCast(Me("GroupMessages"), Boolean)
			End Get
		End Property
	End Class
End Namespace
