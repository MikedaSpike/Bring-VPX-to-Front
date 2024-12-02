Imports System.ComponentModel
Imports System.Management
Imports System.Runtime.InteropServices

Public Class Form1
    Private Shared ReadOnly synchLock As Object = New Object()
    Const PollingInterval As Double = 2.0 'Seconds.
    Const ProgramName As String = "Bring Visual Pinball to Front"
    Dim bVisualpinball = False
    Dim WithEvents ProcessStartWatcher As New ManagementEventWatcher(New WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN " & PollingInterval & " WHERE TargetInstance ISA 'Win32_Process'"))
    Dim WithEvents ProcessStopWatcher As New ManagementEventWatcher(New WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN " & PollingInterval & " WHERE TargetInstance ISA 'Win32_Process'"))

#Region "Form Events"
    ''' <summary>
    ''' Initializes the form on load.
    ''' </summary>
    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load
        Me.Text = ProgramName
        UpdateLog("BVPTF (" & ProgramName & ") Version :" & Application.ProductVersion, Color.Blue)
        UpdateLog("Starting program")
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        ProcessStartWatcher.Start()
        ProcessStopWatcher.Start()
    End Sub

    ''' <summary>
    ''' Handles form closing event.
    ''' </summary>
    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs)
        ProcessStartWatcher.Stop()
        ProcessStopWatcher.Stop()
    End Sub
#End Region

#Region "Process Watchers"
    ''' <summary>
    ''' Handles the start of processes.
    ''' </summary>
    Private Sub ProcessStartWatcher_EventArrived(sender As Object, e As System.Management.EventArrivedEventArgs) Handles ProcessStartWatcher.EventArrived
        Dim ProcessName As String = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("Name")
        Dim PID As Integer = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("ProcessId")

        Select Case ProcessName.ToLower
            Case "VPinballX.exe".ToLower, "VPinballX64.exe".ToLower, "VPinballX_GL.exe".ToLower, "VPinballX_GL64.exe".ToLower, "VPinballX62.exe".ToLower, "VPinball995.exe".ToLower, "VPinball921.exe".ToLower, "VPinball99_PhysMod5_Updated.exe".ToLower, "VPinball8.exe".ToLower
                UpdateLog(String.Format("Found process start of ""{0}"" with ID {1}.", ProcessName, PID))
                bVisualpinball = True
                Delay(3000).ContinueWith(Sub() ActivateWindow(ProcessName, PID, "Visual Pinball Player"))
            Case "VPinballX_GL.exe".ToLower, "VPinballX_GL64.exe".ToLower
                UpdateLog(String.Format("Found process start of ""{0}"" with ID {1}.", ProcessName, PID))
                bVisualpinball = True
                Delay(3000).ContinueWith(Sub() ActivateWindow(ProcessName, PID, "Visual Pinball Player SDL"))
        End Select
    End Sub

    ''' <summary>
    ''' Handles the stop of processes.
    ''' </summary>
    Private Sub ProcessStopWatcher_EventArrived(sender As Object, e As System.Management.EventArrivedEventArgs) Handles ProcessStopWatcher.EventArrived
        Dim ProcessName As String = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("Name")
        Dim PID As Integer = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("ProcessId")
        Select Case ProcessName.ToLower
            Case "VPinballX.exe".ToLower, "VPinballX_GL.exe".ToLower, "VPinballX64.exe".ToLower, "VPinballX_GL64.exe".ToLower
                bVisualpinball = False
        End Select
    End Sub
#End Region

#Region "Utility Functions"
    ''' <summary>
    ''' Creates an asynchronous delay.
    ''' </summary>
    Public Shared Function Delay(ByVal milliseconds As Double) As Threading.Tasks.Task
        Dim tcs = New Threading.Tasks.TaskCompletionSource(Of Boolean)()
        Dim timer As System.Timers.Timer = New System.Timers.Timer()
        AddHandler timer.Elapsed, Sub(obj, args)
                                      tcs.TrySetResult(True)
                                  End Sub
        timer.Interval = milliseconds
        timer.AutoReset = False
        timer.Start()
        Return tcs.Task
    End Function

    ''' <summary>
    ''' Activates a window by its title.
    ''' </summary>
    Private Sub ActivateWindow(ProcessName As String, PID As Integer, WindowToactivate As String)
        Try
            Dim stopwatch As Stopwatch = New Stopwatch()
            Dim WindowFound As Boolean = False
            Dim exitDoWhile As Boolean = False
            UpdateLog("Waiting for window: [" & WindowToactivate & "] for 60 seconds")
            stopwatch.Start()
            Do While exitDoWhile = False AndAlso WindowFound = False AndAlso bVisualpinball = True
                Try
                    AppActivate(WindowToactivate)
                    UpdateLog("Window [" & WindowToactivate & "] found and set to front")
                    WindowFound = True
                Catch ex As Exception
                End Try
                Threading.Thread.Sleep(500)
                If stopwatch.ElapsedMilliseconds > 60000 Then exitDoWhile = True
            Loop
            stopwatch.Stop()
            stopwatch.Reset()
            If exitDoWhile = False Then
                UpdateLog("Monitoring window: [" & WindowToactivate & "] for 30 seconds and bring to front")
                stopwatch.Start()
                Do While stopwatch.ElapsedMilliseconds < 30000
                    If bVisualpinball = False Then UpdateLog("Process : [" & ProcessName & "] stopped. Not monitoring anymore") : Exit Sub
                    If ProcessHelper.GetActiveProcess().MainWindowTitle.ToString.ToLower <> WindowToactivate.ToLower Then
                        Try
                            AppActivate(WindowToactivate)
                        Catch ex As Exception
                            UpdateLog("Not found : [" & WindowToactivate & "]. Stop monitoring", Color.Red)
                            Exit Sub
                        End Try
                        Threading.Thread.Sleep(500)
                    End If
                Loop
                stopwatch.Stop()
                UpdateLog("Quit monitoring window [" & WindowToactivate & "]")
            Else
                UpdateLog("Timeout to find window [" & WindowToactivate & "]", Color.Red)
            End If
        Catch ex As Exception
            LogAnError(System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
        End Try
    End Sub
#End Region

#Region "Form Behavior"
    ''' <summary>
    ''' Handles form resize events.
    ''' </summary>
    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            TrayIcon.Visible = True
            TrayIcon.Icon = Me.Icon
            Me.Hide()
            TrayIcon.BalloonTipIcon = ToolTipIcon.Info
            TrayIcon.Text = "Open '" & ProgramName & "' log file"
        End If
    End Sub

    ''' <summary>
    ''' Handles form closing.
    ''' </summary>
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        TrayIcon.Visible = False
        Me.ShowInTaskbar = False
    End Sub

    ''' <summary>
    ''' Handles system tray icon double-click.
    ''' </summary>
    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrayIcon.DoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        TrayIcon.Visible = False
    End Sub

    ''' <summary>
    ''' Handles system tray icon click.
    ''' </summary>
    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrayIcon.Click
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        TrayIcon.Visible = False
        Me.ShowInTaskbar = True
    End Sub
#End Region

#Region "Logging"
    ''' <summary>
    ''' Logs errors.
    ''' </summary>
    Public Sub LogAnError(ByVal ErrorSubName As String, ErrorMessage As String)
        Try
            UpdateLog(ErrorSubName & vbTab & ErrorMessage, Color.Red)
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Logs actions.
    ''' </summary>
    Private Sub UpdateLog(ActionText As String, Optional Textcolor As Color = Nothing)
        Try
            Dim unused = BeginInvoke(New Action(Function()
                                                    SyncLock synchLock
                                                        If Textcolor = Nothing Then
                                                            rtbActionsLog.SelectionColor = Color.Black
                                                        Else
                                                            rtbActionsLog.SelectionColor = Textcolor
                                                        End If
                                                        rtbActionsLog.AppendText(DateTime.Now & vbTab & ":" & vbTab & ActionText & vbCrLf)
                                                    End SyncLock
                                                End Function))
        Catch ex As Exception
        End Try
    End Sub
#End Region
End Class

Public NotInheritable Class ProcessHelper
    Private Sub New() 'Make no instances of this class.
    End Sub

    ''' <summary>
    ''' Retrieves the handle of the foreground window.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    ''' <summary>
    ''' Retrieves the process ID of the specified window.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, ByRef lpdwProcessId As UInteger) As Integer
    End Function

    ''' <summary>
    ''' Gets the process associated with the current foreground window.
    ''' </summary>
    Public Shared Function GetActiveProcess() As Process
        Dim FocusedWindow As IntPtr = GetForegroundWindow()
        If FocusedWindow = IntPtr.Zero Then Return Nothing

        Dim FocusedWindowProcessId As UInteger = 0
        GetWindowThreadProcessId(FocusedWindow, FocusedWindowProcessId)

        If FocusedWindowProcessId = 0 Then Return Nothing
        Return Process.GetProcessById(CType(FocusedWindowProcessId, Integer))
    End Function
End Class
