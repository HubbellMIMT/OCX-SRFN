Imports System.Collections.Concurrent
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports Aclara.Hardware.Interfaces
Imports Aclara.Hardware.Security
Imports Aclara.Hardware.Serial
Imports SerialLibrary.My
Public Class SerialComm
	Implements IDisposable
	Private _syncObject As New Object()
	Private _datagramSerialPort As DatagramSerialPort
	Private _dtlsServer As DTLSServer
	Private _session As DTLSSession
	Private _sessionTask As Task
	Private _logText As New StringBuilder()
	Private _logBlockingCollection As New BlockingCollection(Of String)()
	Private _logCancellationTokenSource As New CancellationTokenSource()
	Public Function Send(cmdText As String) As Boolean
		Send = True
		Dim data As Byte() = Encoding.ASCII.GetBytes(cmdText)
		If Me._session Is Nothing Then
			MessageBox.Show("session not initialized")
			Return False
		End If
		If Send <> False Then
			Me._session.Write(data)
			If Settings.Default.Debug Then
				Me.AppendLogMessageWithNewLine(String.Format("TX - {0} [{1}]", Encoding.ASCII.GetString(data, 0, data.Length), BitConverter.ToString(data).Replace("-", String.Empty)))
			End If
		End If
	End Function
	Public Sub Close()
		Try
			Task.Factory.StartNew(Sub()
									  SyncLock Me._syncObject
										  If Me._session IsNot Nothing Then
											  Me._session.Shutdown()
											  Me._session = Nothing
										  End If
										  If Me._sessionTask IsNot Nothing AndAlso Not Me._sessionTask.IsCompleted Then
											  Me._sessionTask.Wait(2000)
										  End If
										  Me._sessionTask = Nothing
										  If Me._dtlsServer IsNot Nothing Then
											  Me._dtlsServer.Dispose()
											  RemoveHandler _dtlsServer.LogMessage, AddressOf DTLSServerLogMessage
											  RemoveHandler _dtlsServer.ClientConnected, AddressOf ClientConnected
											  Me._dtlsServer = Nothing
										  End If
										  Me._datagramSerialPort = Nothing
										  Me.StateChanged()
									  End SyncLock
								  End Sub)
		Catch ex As Exception
			Me.AppendLogMessageWithNewLine(ex.ToString())
		End Try
	End Sub
	Private Sub LoggingThread(ByVal cancellationToken As CancellationToken)
		Try
			Do While Not cancellationToken.IsCancellationRequested
				Dim message As String = Nothing
				If Me._logBlockingCollection.TryTake(message, 2147483647, cancellationToken) Then
					Dim allMessages As New StringBuilder(message)
					Do While Me._logBlockingCollection.TryTake(message, 25)
						allMessages.Append(message)
					Loop
					Me._logText.Append(allMessages)
				End If
			Loop
		Catch e1 As OperationCanceledException
		End Try
	End Sub
	Public Sub Dispose() Implements IDisposable.Dispose
		Me._logCancellationTokenSource.Cancel(False)
	End Sub
	Public Sub New()
		Task.Factory.StartNew(Sub()
								  Me.LoggingThread(Me._logCancellationTokenSource.Token)
							  End Sub)
	End Sub
	Private ReadOnly Property SessionEstablished() As Boolean
		Get
			Return Me._session IsNot Nothing
		End Get
	End Property
	Public Delegate Sub ConnectionSuccessful()
	Public Event ClientConnectedFromSerialComm As ConnectionSuccessful
	Public Delegate Sub DatagramTransported(msg As String)
	Public Event DatgramMessageTransported As DatagramTransported
#Region "DTLS Logoff"
	Public Function DTLSConnect(PortName, BaudRate, certpath, Optional WaitTime = 15000) As Boolean
		DTLSConnect = False
		Try
			Settings.Default.Reload()
			Me._datagramSerialPort = New DatagramSerialPort()
			Dim serialPortConfiguration As SerialPortConfiguration = New SerialPortConfiguration With {
						.PortName = PortName,
						.BaudRate = BaudRate,
						.DataBits = Settings.Default.DataBits,
						.StopBits = Settings.Default.StopBits,
						.Parity = Settings.Default.Parity,
						.DelayMilliseconds = Settings.Default.DelayMilliseconds
					}
			Dim dtlsConfiugration As DTLSConfiguration = New DTLSConfiguration With {
						.PrivateKeyFile = certpath & "MFG-PrivKey.pem",
						.CertificateAuthorityFile = certpath & "dtlsMfgRootCA.pem",
						.CertFile = certpath & "MFGChain.pem",
						.CipherSuiteList = Settings.Default.CipherSuiteList,
						.GroupMessages = Settings.Default.GroupMessages,
						.Debug = Settings.Default.Debug,
						.TransportTimeoutSeconds = Settings.Default.TransportTimeoutSeconds
					}
			AddHandler _datagramSerialPort.DatagramTransported, AddressOf DatagramSerialPortDatagramTransported
			Me._datagramSerialPort.Open(serialPortConfiguration)
			Dim bytes As Byte() = Me._datagramSerialPort.Read(10UI)
			Do While bytes IsNot Nothing
				bytes = Me._datagramSerialPort.Read(10UI)
			Loop
			Me._dtlsServer = New DTLSServer(Me._datagramSerialPort)
			AddHandler _dtlsServer.ClientConnected, AddressOf ClientConnected
			AddHandler _dtlsServer.LogMessage, AddressOf DTLSServerLogMessage
			Me._dtlsServer.Initialize(dtlsConfiugration, True)
			Me._datagramSerialPort.WriteRaw(Encoding.ASCII.GetBytes("752732DA-47CD-49C3-B8F4-DA8F23BE0708" & vbLf & vbCr))
			DTLSConnect = True
			Thread.Sleep(WaitTime)
		Catch ex As Exception
			Me.Close()
			Me.AppendLogMessageWithNewLine(ex.ToString())
		End Try
	End Function
	Public Function CheckCertFilesExist(ByVal path As String) As Boolean
		If (My.Computer.FileSystem.FileExists(path & "MFG-PrivKey.pem") = False _
			OrElse My.Computer.FileSystem.FileExists(path & "MFGChain.pem") = False _
			OrElse My.Computer.FileSystem.FileExists(path & "dtlsMfgRootCA.pem") = False) Then
			Return False
		End If
		Return True
	End Function
#End Region
	Private Sub DTLSServerLogMessage(ByVal logLevel As Integer, ByVal message As String)
		Me.AppendLogMessageWithNewLine(message)
	End Sub
	Private Sub AppendLogMessage(ByVal message As String)
		If message.EndsWith(vbCrLf) Then
			message = message.TrimEnd(New Char() {ControlChars.Cr, ControlChars.Lf})
			message &= vbLf
		End If
		Me._logBlockingCollection.TryAdd(message)
	End Sub
	Private Sub StateChanged()
	End Sub
	Public Sub ClientConnected(ByVal session As DTLSSession)
		If session Is Nothing Then
			Me.AppendLogMessageWithNewLine("DTLS Handshake Failed.")
			Return
		End If
		RaiseEvent ClientConnectedFromSerialComm()
		Me._session = session
		Me.StateChanged()
		Me._sessionTask = Task.Factory.StartNew(Sub()
													Dim [exit] As Boolean = False
													Do While Me._session IsNot Nothing AndAlso Not [exit]
														Try
															Dim received As Byte() = Me._session.Read()
															If received IsNot Nothing Then
																If Settings.Default.Debug Then
																	Me.AppendLogMessageWithNewLine(String.Format("RX - {0} [{1}]", Encoding.ASCII.GetString(received, 0, received.Length), BitConverter.ToString(received).Replace("-", String.Empty)))
																Else
																	Me.AppendLogMessage(Encoding.ASCII.GetString(received, 0, received.Length))
																End If
															Else
																[exit] = True
															End If
														Catch ex As Exception
															Me.AppendLogMessageWithNewLine(ex.ToString())
														End Try
													Loop
												End Sub)
	End Sub
	Private Sub DatagramSerialPortDatagramTransported(ByVal datagramDirection As DatagramDirection, ByVal datagram() As Byte)
		Dim msg = BitConverter.ToString(datagram).Replace("-", String.Empty)
		RaiseEvent DatgramMessageTransported(msg)
		If Settings.Default.Debug Then
			Me.AppendLogMessageWithNewLine(String.Format("DTLS {0} ({1}) - [{2}]", datagramDirection, datagram.Length, msg))
		End If
	End Sub
	Private Sub AppendLogMessageWithNewLine(ByVal message As String)
		message = message.TrimEnd(New Char() {ControlChars.Cr, ControlChars.Lf})
		If Not String.IsNullOrWhiteSpace(message) Then
			Me._logBlockingCollection.TryAdd(String.Format("{0}: {1}{2}", Date.Now.ToString("HH:mm:ss.fff"), message, Environment.NewLine))
		End If
	End Sub

End Class
