<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PlaniGrid
    Inherits System.Windows.Forms.UserControl

    'UserControl reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PlaniGrid))
        Dim TextShadower2 As gCursorLib.TextShadower = New gCursorLib.TextShadower
        Me.BotoMes = New System.Windows.Forms.Button
        Me.BotoMenys = New System.Windows.Forms.Button
        Me.GridPanel = New PlaniSoftwareControlLibrary.PlaniGrid.DoubleBufferedPanel
        Me.PlaniGridDragCursor = New gCursorLib.gCursor(Me.components)
        Me.SuspendLayout()
        '
        'BotoMes
        '
        Me.BotoMes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BotoMes.Location = New System.Drawing.Point(34, 3)
        Me.BotoMes.Name = "BotoMes"
        Me.BotoMes.Size = New System.Drawing.Size(36, 23)
        Me.BotoMes.TabIndex = 2
        Me.BotoMes.TabStop = False
        Me.BotoMes.Text = "+"
        Me.BotoMes.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.BotoMes.UseVisualStyleBackColor = True
        '
        'BotoMenys
        '
        Me.BotoMenys.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BotoMenys.Location = New System.Drawing.Point(3, 3)
        Me.BotoMenys.Name = "BotoMenys"
        Me.BotoMenys.Size = New System.Drawing.Size(31, 23)
        Me.BotoMenys.TabIndex = 1
        Me.BotoMenys.TabStop = False
        Me.BotoMenys.Text = "-"
        Me.BotoMenys.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.BotoMenys.UseVisualStyleBackColor = True
        '
        'GridPanel
        '
        Me.GridPanel.AllowDrop = True
        Me.GridPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GridPanel.AutoScroll = True
        Me.GridPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GridPanel.BackColor = System.Drawing.SystemColors.Window
        Me.GridPanel.Location = New System.Drawing.Point(74, 37)
        Me.GridPanel.Name = "GridPanel"
        Me.GridPanel.Size = New System.Drawing.Size(235, 171)
        Me.GridPanel.TabIndex = 0
        Me.GridPanel.TabStop = True
        '
        'PlaniGridDragCursor
        '
        Me.PlaniGridDragCursor.gBlackBitBack = False
        Me.PlaniGridDragCursor.gBoxShadow = True
        Me.PlaniGridDragCursor.gCursorImage = CType(resources.GetObject("PlaniGridDragCursor.gCursorImage"), System.Drawing.Bitmap)
        Me.PlaniGridDragCursor.gEffect = gCursorLib.gCursor.eEffect.No
        Me.PlaniGridDragCursor.gFont = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PlaniGridDragCursor.gHotSpot = System.Drawing.ContentAlignment.TopCenter
        Me.PlaniGridDragCursor.gIBTransp = 80
        Me.PlaniGridDragCursor.gImage = CType(resources.GetObject("PlaniGridDragCursor.gImage"), System.Drawing.Bitmap)
        Me.PlaniGridDragCursor.gImageBorderColor = System.Drawing.Color.Black
        Me.PlaniGridDragCursor.gImageBox = New System.Drawing.Size(75, 56)
        Me.PlaniGridDragCursor.gImageBoxColor = System.Drawing.Color.White
        Me.PlaniGridDragCursor.gITransp = 0
        Me.PlaniGridDragCursor.gScrolling = gCursorLib.gCursor.eScrolling.No
        Me.PlaniGridDragCursor.gShowImageBox = False
        Me.PlaniGridDragCursor.gShowTextBox = True
        Me.PlaniGridDragCursor.gTBTransp = 80
        Me.PlaniGridDragCursor.gText = "Example Text" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Second Line" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Third Line"
        Me.PlaniGridDragCursor.gTextAlignment = System.Drawing.ContentAlignment.TopLeft
        Me.PlaniGridDragCursor.gTextAutoFit = gCursorLib.gCursor.eTextAutoFit.None
        Me.PlaniGridDragCursor.gTextBorderColor = System.Drawing.Color.Black
        Me.PlaniGridDragCursor.gTextBox = New System.Drawing.Size(100, 30)
        Me.PlaniGridDragCursor.gTextBoxColor = System.Drawing.Color.White
        Me.PlaniGridDragCursor.gTextColor = System.Drawing.Color.Black
        Me.PlaniGridDragCursor.gTextFade = gCursorLib.gCursor.eTextFade.Solid
        Me.PlaniGridDragCursor.gTextMultiline = False
        Me.PlaniGridDragCursor.gTextShadow = False
        Me.PlaniGridDragCursor.gTextShadowColor = System.Drawing.Color.Black
        TextShadower2.Alignment = System.Drawing.ContentAlignment.MiddleCenter
        TextShadower2.Blur = 2.0!
        TextShadower2.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TextShadower2.Offset = CType(resources.GetObject("TextShadower2.Offset"), System.Drawing.PointF)
        TextShadower2.Padding = New System.Windows.Forms.Padding(0)
        TextShadower2.ShadowColor = System.Drawing.Color.Black
        TextShadower2.ShadowTransp = 128
        TextShadower2.Text = "Drop Shadow"
        TextShadower2.TextColor = System.Drawing.Color.Blue
        Me.PlaniGridDragCursor.gTextShadower = TextShadower2
        Me.PlaniGridDragCursor.gTTransp = 0
        Me.PlaniGridDragCursor.gType = gCursorLib.gCursor.eType.Text
        '
        'PlaniGrid
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.GridPanel)
        Me.Controls.Add(Me.BotoMes)
        Me.Controls.Add(Me.BotoMenys)
        Me.DoubleBuffered = True
        Me.Name = "PlaniGrid"
        Me.Size = New System.Drawing.Size(309, 208)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GridPanel As DoubleBufferedPanel
    Friend WithEvents PlaniGridDragCursor As gCursorLib.gCursor
    Friend WithEvents BotoMes As System.Windows.Forms.Button
    Friend WithEvents BotoMenys As System.Windows.Forms.Button

End Class
