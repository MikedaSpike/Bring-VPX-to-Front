<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.rtbActionsLog = New System.Windows.Forms.RichTextBox()
        Me.TrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.SuspendLayout()
        '
        'rtbActionsLog
        '
        Me.rtbActionsLog.BackColor = System.Drawing.SystemColors.Window
        Me.rtbActionsLog.HideSelection = False
        Me.rtbActionsLog.Location = New System.Drawing.Point(12, 12)
        Me.rtbActionsLog.Name = "rtbActionsLog"
        Me.rtbActionsLog.ReadOnly = True
        Me.rtbActionsLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical
        Me.rtbActionsLog.Size = New System.Drawing.Size(481, 306)
        Me.rtbActionsLog.TabIndex = 151
        Me.rtbActionsLog.Text = ""
        '
        'TrayIcon
        '
        Me.TrayIcon.Icon = CType(resources.GetObject("TrayIcon.Icon"), System.Drawing.Icon)
        Me.TrayIcon.Visible = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(505, 330)
        Me.Controls.Add(Me.rtbActionsLog)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Bring VPX to Front"
        Me.ResumeLayout(False)

    End Sub

    Public WithEvents rtbActionsLog As RichTextBox
    Friend WithEvents TrayIcon As NotifyIcon
End Class
