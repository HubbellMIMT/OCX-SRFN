Option Strict On 
Option Explicit On 
Imports VB = Microsoft
Imports System.Linq

Namespace TWRN

' Structured data classes to replace scattered global variables
Public Class TestConfiguration
    Public Property CustomerId As String = String.Empty
    Public Property Customer As String = String.Empty  
    Public Property ProductType As String = String.Empty
    Public Property ProductFamily As String = String.Empty
    Public Property Product As String = String.Empty
    Public Property EEMap As String = String.Empty
    Public Property PackPath As String = String.Empty
    Public Property CommandParameters As String = String.Empty
    Public Property MfrNumber As String = String.Empty
End Class

' Replacement for the Varparams array with meaningful property names
Public Class TestParameters
    Public Property MeterSerialNumber As String = String.Empty ' Varparams(0)
    Public Property DCSISerialNumber As String = String.Empty  ' Varparams(1)
    Public Property SQLName As String = String.Empty          ' Varparams(2)
    Public Property MeterFirmware As String = String.Empty    ' Varparams(3)
    Public Property CustomerId As String = String.Empty       ' Varparams(4)
    Public Property ProductFamily As String = String.Empty    ' Varparams(5)
    Public Property FormVoltage As String = String.Empty      ' Varparams(6)
    Public Property RFCarrier As String = String.Empty        ' Varparams(7)
    Public Property PackPath As String = String.Empty         ' Varparams(8)
    Public Property RDInstalled As String = String.Empty      ' Varparams(9)
    
    ' SQL Connection parameters
    Public Property DLLRevConnectionString As String = String.Empty  ' Varparams(10)
    Public Property DLLRevDatabase As String = String.Empty          ' Varparams(11)
    Public Property DLLRevSource As String = String.Empty            ' Varparams(12)
    Public Property ProductConnectionString As String = String.Empty  ' Varparams(13)
    Public Property ProductDatabase As String = String.Empty         ' Varparams(14)
    Public Property ProductSource As String = String.Empty           ' Varparams(15)
    Public Property ResultConnectionString As String = String.Empty  ' Varparams(16)
    Public Property ResultDatabase As String = String.Empty          ' Varparams(17)
    Public Property ResultSource As String = String.Empty            ' Varparams(18)
    
    ' Additional parameters
    Public Property ModuleFirmware As String = String.Empty          ' Varparams(20)
    Public Property TestResults As String = String.Empty             ' Varparams(21)
    
    ' Provide array-like access for backward compatibility
    Default Public Property Item(index As Integer) As String
        Get
            Select Case index
                Case 0 : Return MeterSerialNumber
                Case 1 : Return DCSISerialNumber
                Case 2 : Return SQLName
                Case 3 : Return MeterFirmware
                Case 4 : Return CustomerId
                Case 5 : Return ProductFamily
                Case 6 : Return FormVoltage
                Case 7 : Return RFCarrier
                Case 8 : Return PackPath
                Case 9 : Return RDInstalled
                Case 10 : Return DLLRevConnectionString
                Case 11 : Return DLLRevDatabase
                Case 12 : Return DLLRevSource
                Case 13 : Return ProductConnectionString
                Case 14 : Return ProductDatabase
                Case 15 : Return ProductSource
                Case 16 : Return ResultConnectionString
                Case 17 : Return ResultDatabase
                Case 18 : Return ResultSource
                Case 20 : Return ModuleFirmware
                Case 21 : Return TestResults
                Case Else : Return String.Empty
            End Select
        End Get
        Set(value As String)
            Select Case index
                Case 0 : MeterSerialNumber = value
                Case 1 : DCSISerialNumber = value
                Case 2 : SQLName = value
                Case 3 : MeterFirmware = value
                Case 4 : CustomerId = value
                Case 5 : ProductFamily = value
                Case 6 : FormVoltage = value
                Case 7 : RFCarrier = value
                Case 8 : PackPath = value
                Case 9 : RDInstalled = value
                Case 10 : DLLRevConnectionString = value
                Case 11 : DLLRevDatabase = value
                Case 12 : DLLRevSource = value
                Case 13 : ProductConnectionString = value
                Case 14 : ProductDatabase = value
                Case 15 : ProductSource = value
                Case 16 : ResultConnectionString = value
                Case 17 : ResultDatabase = value
                Case 18 : ResultSource = value
                Case 20 : ModuleFirmware = value
                Case 21 : TestResults = value
            End Select
        End Set
    End Property
End Class

Public Class TestState
    Public Property TestFail As Boolean = True
    Public Property CurrentRef As Integer = 0
    Public Property BadCommunication As Boolean = False
    Public Property EEPromFail As Boolean = False
    Public Property CurrentCommandName As String = String.Empty
    Public Property PassCount As Short = 0
    Public Property FailCount As Short = 0
End Class

Public Class CommunicationData
    Public Property OutboundString As String = String.Empty
    Public Property InboundFullAnswer As String = String.Empty
    Public Property InboundString As String = String.Empty
End Class

Public Class TestContext
    Public Property Configuration As New TestConfiguration()
    Public Property State As New TestState()
    Public Property Communication As New CommunicationData()
    Public Property Parameters As New TestParameters()
    
    Public Sub Reset()
        State.TestFail = True
        State.CurrentRef = 0
        State.BadCommunication = False
        State.EEPromFail = False
        State.CurrentCommandName = String.Empty
        Communication.OutboundString = String.Empty
        Communication.InboundFullAnswer = String.Empty
        Communication.InboundString = String.Empty
        ' Reset parameters but keep connection strings
        Parameters.MeterSerialNumber = String.Empty
        Parameters.DCSISerialNumber = String.Empty
        Parameters.MeterFirmware = String.Empty
        Parameters.ModuleFirmware = String.Empty
    End Sub
End Class

' Utility class to handle firmware-specific logic
Public Class FirmwareUtilities
    Private Shared ReadOnly SpecialFirmwareVersions As String() = {"0534", "0537", "0571"}
    
    Public Shared Function RequiresMapSwapping(firmwareVersion As String) As Boolean
        Return SpecialFirmwareVersions.Contains(firmwareVersion)
    End Function
    
    Public Shared Function IsValidFirmwareVersion(firmwareVersion As String) As Boolean
        Return Not String.IsNullOrEmpty(firmwareVersion) AndAlso firmwareVersion.Length = 4
    End Function
    
    Public Shared Function GetFirmwareDisplayName(firmwareVersion As String) As String
        If String.IsNullOrEmpty(firmwareVersion) Then Return "Unknown"
        If firmwareVersion.Length = 4 Then
            Return String.Format("v{0}.{1}", firmwareVersion.Substring(0, 2), firmwareVersion.Substring(2, 2))
        End If
        Return firmwareVersion
    End Function
    
    Public Shared Function GetProductFamilyShortName(productFamily As String) As String
        If String.IsNullOrEmpty(productFamily) Then Return String.Empty
        
        ' Extract meaningful part from product family strings like "UMTR-F-AXe_4.xx"
        Dim parts As String() = productFamily.Split("_"c)
        Return If(parts.Length > 0, parts(0), productFamily)
    End Function
    
    Public Shared Function RequiresMeterFirmwareRead(productFamily As String) As Boolean
        Return Not String.IsNullOrEmpty(productFamily) AndAlso 
               (productFamily.Contains("F-FX") OrElse productFamily.Contains("F-AX") OrElse productFamily.Contains("F-RX"))
    End Function
End Class

Module DMTP1
    Public lgmodule As New lgmoduleINT
    Private _testContext As TestContext
    
    ' Lazy initialization of TestContext to prevent null reference exceptions
    Public ReadOnly Property TestContext As TestContext
        Get
            If _testContext Is Nothing Then
                _testContext = New TestContext()
            End If
            Return _testContext
        End Get
    End Property
    
    ' Backward compatibility wrapper for Varparams array
    ' Note: Using DMTP1.TestParams to avoid conflict with Module1.Varparams
    Public ReadOnly Property TestParams As TestParameters
        Get
            Return TestContext.Parameters
        End Get
    End Property
    
    ' Legacy property wrappers for backward compatibility (to be removed gradually)
    Public Property gCustomerId As String
        Get
            Return TestContext.Configuration.CustomerId
        End Get
        Set(value As String)
            TestContext.Configuration.CustomerId = value
        End Set
    End Property
    
    Public Property gCustomer As String
        Get
            Return TestContext.Configuration.Customer
        End Get
        Set(value As String)
            TestContext.Configuration.Customer = value
        End Set
    End Property
    
    Public Property gProductType As String
        Get
            Return TestContext.Configuration.ProductType
        End Get
        Set(value As String)
            TestContext.Configuration.ProductType = value
        End Set
    End Property
    
    Public Property TestFail As Boolean
        Get
            Return TestContext.State.TestFail
        End Get
        Set(value As Boolean)
            TestContext.State.TestFail = value
        End Set
    End Property
    
    Public Property gPackPath As String
        Get
            Return TestContext.Configuration.PackPath
        End Get
        Set(value As String)
            TestContext.Configuration.PackPath = value
        End Set
    End Property
    
    Public Property gintRef As Integer
        Get
            Return TestContext.State.CurrentRef
        End Get
        Set(value As Integer)
            TestContext.State.CurrentRef = value
        End Set
    End Property
    
    Public Property gblnbadcommunication As Boolean
        Get
            Return TestContext.State.BadCommunication
        End Get
        Set(value As Boolean)
            TestContext.State.BadCommunication = value
        End Set
    End Property
    
    Public Property gPasscount As Short
        Get
            Return TestContext.State.PassCount
        End Get
        Set(value As Short)
            TestContext.State.PassCount = value
        End Set
    End Property
    
    Public Property gFailcount As Short
        Get
            Return TestContext.State.FailCount
        End Get
        Set(value As Short)
            TestContext.State.FailCount = value
        End Set
    End Property
    
    Public Property gmfrnumber As String
        Get
            Return TestContext.Configuration.MfrNumber
        End Get
        Set(value As String)
            TestContext.Configuration.MfrNumber = value
        End Set
    End Property
    
    Public Property ProductFamily As String
        Get
            Return TestContext.Configuration.ProductFamily
        End Get
        Set(value As String)
            TestContext.Configuration.ProductFamily = value
        End Set
    End Property
    
    Public Property Product As String
        Get
            Return TestContext.Configuration.Product
        End Get
        Set(value As String)
            TestContext.Configuration.Product = value
        End Set
    End Property
    
    Public Property EEMap As String
        Get
            Return TestContext.Configuration.EEMap
        End Get
        Set(value As String)
            TestContext.Configuration.EEMap = value
        End Set
    End Property
    
    Public Property CommandParameters As String
        Get
            Return TestContext.Configuration.CommandParameters
        End Get
        Set(value As String)
            TestContext.Configuration.CommandParameters = value
        End Set
    End Property
    
    Public Property gblnFAIL_EEPROM As Boolean
        Get
            Return TestContext.State.EEPromFail
        End Get
        Set(value As Boolean)
            TestContext.State.EEPromFail = value
        End Set
    End Property
    
    Public Property gstrcommandname As String
        Get
            Return TestContext.State.CurrentCommandName
        End Get
        Set(value As String)
            TestContext.State.CurrentCommandName = value
        End Set
    End Property
    
    Public Property gsOutbound_STR As String
        Get
            Return TestContext.Communication.OutboundString
        End Get
        Set(value As String)
            TestContext.Communication.OutboundString = value
        End Set
    End Property
    
    Public Property gInbFullAnswer As String
        Get
            Return TestContext.Communication.InboundFullAnswer
        End Get
        Set(value As String)
            TestContext.Communication.InboundFullAnswer = value
        End Set
    End Property
    
    Public Property gsInbound_STR As String
        Get
            Return TestContext.Communication.InboundString
        End Get
        Set(value As String)
            TestContext.Communication.InboundString = value
        End Set
    End Property
End Module
End Namespace
