Public Interface IMainForm
    ' SQL Config
    Property DLLRevisionText As String
    Property CustomerValuesText As String
    Property TestResultsText As String
    Property FirmwareConnStr As String
    Property SQLServerText As String
    Property SQLServer As String
    Property ShowFormChecked As Boolean
    Property RA6ProgPathText As String
    WriteOnly Property PasswordText As String
    ' Comm Ports
    Property CommPortText As String
    Property DebugPortText As String
    Property RelayPortText As String
    Property OpticalPortText As String
    Sub ShowPorts()
    Sub SetFormXml()
    Sub UpdatePortStatus()
    ' Firmware
    Property ProductFamilyText As String
    Sub WriteFirmwareToNIC(firmwareName As String)
    Function VirginDelay() As Task(Of Boolean)
End Interface
