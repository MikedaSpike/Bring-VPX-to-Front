Imports System.ComponentModel
Imports System.Management
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography


'Module ProcessExtensions
'    <Extension()>
'    Function GetChildProcesses(ByVal process As Process) As IList(Of Process)
'        Return New ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={process.Id}").[Get]().Cast(Of ManagementObject)().[Select](Function(mo) Process.GetProcessById(Convert.ToInt32(mo("ProcessID")))).ToList()
'    End Function
'End Module


Public Class Form1
    Private Shared ReadOnly synchLock As Object = New Object()
    Const PollingInterval As Double = 2.0 'Seconds.
    Const ProgramName As String = "Bring Visual Pinball to Front"
    Dim bVisualpinball = False
    Dim WithEvents ProcessStartWatcher As New ManagementEventWatcher(New WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN " & PollingInterval & " WHERE TargetInstance ISA 'Win32_Process'"))
    Dim WithEvents ProcessStopWatcher As New ManagementEventWatcher(New WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN " & PollingInterval & " WHERE TargetInstance ISA 'Win32_Process'"))
    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load
        Me.Text = ProgramName
        UpdateLog("BVPTF (" & ProgramName & ") Version :" & Application.ProductVersion, Color.Blue)
        UpdateLog("Starting program")
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        ProcessStartWatcher.Start()
        ProcessStopWatcher.Start()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs)
        ProcessStartWatcher.Stop()
        ProcessStopWatcher.Stop()
    End Sub

    Private Sub ProcessStartWatcher_EventArrived(sender As Object, e As System.Management.EventArrivedEventArgs) Handles ProcessStartWatcher.EventArrived
        Dim ProcessName As String = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("Name")
        Dim PID As Integer = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("ProcessId")

        Select Case ProcessName.ToLower
            Case "VPinballX.exe".ToLower, "VPinballX64.exe".ToLower, "VPinballX_GL.exe".ToLower, "VPinballX_GL64.exe".ToLower, "VPinballX62.exe".ToLower, "VPinball995.exe".ToLower, "VPinball921.exe".ToLower, "VPinball99_PhysMod5_Updated.exe".ToLower, "VPinball8.exe".ToLower
                UpdateLog(String.Format("Found process start of ""{0}"" with ID {1}.", ProcessName, PID))
                'Application.DoEvents()
                bVisualpinball = True
                Delay(3000).ContinueWith(Sub() ActivateWindow(ProcessName, PID, "Visual Pinball Player"))
            Case "VPinballX_GL.exe".ToLower, "VPinballX_GL64.exe".ToLower
                UpdateLog(String.Format("Found process start of ""{0}"" with ID {1}.", ProcessName, PID))
                'Application.DoEvents()
                bVisualpinball = True
                Delay(3000).ContinueWith(Sub() ActivateWindow(ProcessName, PID, "Visual Pinball Player SDL"))

        End Select

    End Sub

    Private Sub ProcessStopWatcher_EventArrived(sender As Object, e As System.Management.EventArrivedEventArgs) Handles ProcessStopWatcher.EventArrived
        Dim ProcessName As String = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("Name")
        Dim PID As Integer = CType(e.NewEvent.Properties("TargetInstance").Value, ManagementBaseObject)("ProcessId")
        Select Case ProcessName.ToLower
            Case "VPinballX.exe".ToLower, "VPinballX_GL.exe".ToLower, "VPinballX64.exe".ToLower, "VPinballX_GL64.exe".ToLower
                bVisualpinball = False
        End Select
        'MessageBox.Show(String.Format("Process ""{0}"" with ID {1} stopped.", ProcessName, PID))
    End Sub

    Public Shared Function Delay(ByVal milliseconds As Double) As Threading.Tasks.Task
        'asynchronous delay. 
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


    'Private Sub ActivateWindow(ProcessName As String, PID As Integer, Optional WindowToactivate As String = "")
    '    Try
    '        Debug.WriteLine("ActivateWindow [" & ProcessName, "] PID [" & PID & "] WindowToactivate [" & WindowToactivate & "]")
    '        If WindowToactivate = "" Then
    '            Dim p As Process
    '            Dim window_name As String = Process.GetProcessById(PID).MainWindowTitle.ToString
    '            AppActivate(window_name)
    '            For Each p In Process.GetProcessesByName(PID)
    '                window_name = p.MainWindowTitle.ToString
    '                AppActivate(window_name)
    '                UpdateLog("Activated window name [" & window_name & "]")
    '            Next
    '        Else
    '            Dim stopwatch As Stopwatch = New Stopwatch()
    '            stopwatch.Start()
    '            Dim stopDoWhile As Boolean = False
    '            'Need specific window to activate
    '            Do While Process.GetProcessById(PID).MainWindowTitle.ToString.ToLower <> WindowToactivate.ToLower And stopDoWhile = False
    '                Threading.Thread.Sleep(500)
    '                If stopwatch.ElapsedMilliseconds > 15000 Then stopDoWhile = True
    '                Application.DoEvents()
    '            Loop
    '            stopwatch.Stop()
    '            'Threading.Thread.Sleep(1000)
    '            Debug.WriteLine("Active window " & ProcessHelper.GetActiveProcess().MainWindowTitle.ToString)
    '            If stopDoWhile = False Then
    '                AppActivate(WindowToactivate)
    '                UpdateLog("brought to front : [" & WindowToactivate & "]")
    '                'start timer for 30 seconde
    '                stopwatch.Start()
    '                Do While stopwatch.ElapsedMilliseconds < 30000
    '                    If ProcessHelper.GetActiveProcess().MainWindowTitle.ToString.ToLower <> WindowToactivate.ToLower Then
    '                        AppActivate(WindowToactivate)
    '                        UpdateLog("Brought to front again : [" & WindowToactivate & "]", Color.OrangeRed)
    '                        Threading.Thread.Sleep(500)
    '                    End If
    '                Loop
    '            Else
    '                UpdateLog("Timeout to find window [" & WindowToactivate & "]", Color.Red)
    '                Dim p As Process
    '                For Each p In GetChildProcesses(Process.GetProcessById(PID))
    '                    UpdateLog("found process: [" & p.MainWindowTitle.ToString & "]")
    '                Next

    '            End If
    '        End If
    '    Catch ex As Exception
    '        LogAnError(System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)
    '    End Try
    'End Sub

    Private Sub ActivateWindow(ProcessName As String, PID As Integer, WindowToactivate As String)
        Try
            Dim stopwatch As Stopwatch = New Stopwatch()
            Dim WindowFound As Boolean = False
            Dim exitDoWhile As Boolean = False
            UpdateLog("Waiting for window: [" & WindowToactivate & "] for 60 seconds")
            stopwatch.Start()
            'Need specific window to activate
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
            'Threading.Thread.Sleep(1000)
            Debug.WriteLine("Active window " & ProcessHelper.GetActiveProcess().MainWindowTitle.ToString)
            If exitDoWhile = False Then
                UpdateLog("Monitoring window: [" & WindowToactivate & "] for 30 seconds and bring to front")
                stopwatch.Start()
                Do While stopwatch.ElapsedMilliseconds < 30000
                    If bVisualpinball = False Then UpdateLog("Process : [" & ProcessName & "] stopped. Not monitoring anymore") : Exit Sub
                    If ProcessHelper.GetActiveProcess().MainWindowTitle.ToString.ToLower <> WindowToactivate.ToLower Then
                        Try
                            'updateLog("Active window " & ProcessHelper.GetActiveProcess().MainWindowTitle.ToString)
                            AppActivate(WindowToactivate)
                            'UpdateLog("Brought to front again : [" & WindowToactivate & "]", Color.OrangeRed)

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

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            TrayIcon.Visible = True
            TrayIcon.Icon = Me.Icon
            Me.Hide()
            TrayIcon.BalloonTipIcon = ToolTipIcon.Info
            TrayIcon.Text = "Open '" & ProgramName & "' log file"
            'TrayIcon.BalloonTipText = "Open 'Bring Visual Pinball To Top' log file"
            'TrayIcon.BalloonTipTitle = "BVPTT"
            'TrayIcon.ShowBalloonTip(500)

        End If
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        TrayIcon.Visible = False
        Me.ShowInTaskbar = False
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrayIcon.DoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        TrayIcon.Visible = False
    End Sub

    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TrayIcon.Click
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        TrayIcon.Visible = False
        Me.ShowInTaskbar = True
    End Sub


    '*********************************************************************************************************
    ' Function : LogAnError
    ' Purpose  : Logs an error to logfile, optionally also adds so action log window
    '*********************************************************************************************************
    Public Sub LogAnError(ByVal ErrorSubName As String, ErrorMessage As String)
        Try
            UpdateLog(ErrorSubName & vbTab & ErrorMessage, Color.Red)
        Catch
        End Try
    End Sub

    '*********************************************************************************************************
    ' Function : UpdateActionLog
    ' Purpose  : Add text to the action window
    '*********************************************************************************************************
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
            'LogAnError(System.Reflection.MethodBase.GetCurrentMethod.Name, ex.Message)

        End Try
    End Sub

End Class





Public NotInheritable Class ProcessHelper
    Private Sub New() 'Make no instances of this class.
    End Sub

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, ByRef lpdwProcessId As UInteger) As Integer
    End Function

    Public Shared Function GetActiveProcess() As Process
        Dim FocusedWindow As IntPtr = GetForegroundWindow()
        If FocusedWindow = IntPtr.Zero Then Return Nothing

        Dim FocusedWindowProcessId As UInteger = 0
        GetWindowThreadProcessId(FocusedWindow, FocusedWindowProcessId)

        If FocusedWindowProcessId = 0 Then Return Nothing
        Return Process.GetProcessById(CType(FocusedWindowProcessId, Integer))
    End Function
End Class
