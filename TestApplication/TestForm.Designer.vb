<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestForm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TestForm))
        Dim TextShadower1 As gCursorLib.TextShadower = New gCursorLib.TextShadower
        Me.ListBox3 = New System.Windows.Forms.ListBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.ListBox2 = New System.Windows.Forms.ListBox
        Me.aag = New System.Windows.Forms.CheckBox
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.DragCursor = New gCursorLib.gCursor(Me.components)
        Me.PlaniMonthView1 = New PlaniSoftwareControlLibrary.PlaniMonthView
        Me.Button2 = New System.Windows.Forms.Button
        Me.PlaniGrid1 = New PlaniSoftwareControlLibrary.PlaniGrid
        Me.SuspendLayout()
        '
        'ListBox3
        '
        Me.ListBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListBox3.FormattingEnabled = True
        Me.ListBox3.Location = New System.Drawing.Point(12, 442)
        Me.ListBox3.Name = "ListBox3"
        Me.ListBox3.Size = New System.Drawing.Size(234, 108)
        Me.ListBox3.TabIndex = 20
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(57, 165)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 19
        Me.Button1.Text = "Clear"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ListBox2
        '
        Me.ListBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.Location = New System.Drawing.Point(12, 328)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple
        Me.ListBox2.Size = New System.Drawing.Size(234, 108)
        Me.ListBox2.TabIndex = 18
        '
        'aag
        '
        Me.aag.AutoSize = True
        Me.aag.Location = New System.Drawing.Point(12, 169)
        Me.aag.Name = "aag"
        Me.aag.Size = New System.Drawing.Size(44, 17)
        Me.aag.TabIndex = 17
        Me.aag.Text = "aag"
        Me.aag.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.AllowDrop = True
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(12, 194)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(234, 121)
        Me.ListBox1.TabIndex = 16
        '
        'DragCursor
        '
        Me.DragCursor.gBlackBitBack = False
        Me.DragCursor.gBoxShadow = True
        Me.DragCursor.gCursorImage = CType(resources.GetObject("DragCursor.gCursorImage"), System.Drawing.Bitmap)
        Me.DragCursor.gEffect = gCursorLib.gCursor.eEffect.No
        Me.DragCursor.gFont = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DragCursor.gHotSpot = System.Drawing.ContentAlignment.MiddleCenter
        Me.DragCursor.gIBTransp = 80
        Me.DragCursor.gImage = CType(resources.GetObject("DragCursor.gImage"), System.Drawing.Bitmap)
        Me.DragCursor.gImageBorderColor = System.Drawing.Color.Black
        Me.DragCursor.gImageBox = New System.Drawing.Size(75, 56)
        Me.DragCursor.gImageBoxColor = System.Drawing.Color.White
        Me.DragCursor.gITransp = 0
        Me.DragCursor.gScrolling = gCursorLib.gCursor.eScrolling.No
        Me.DragCursor.gShowImageBox = False
        Me.DragCursor.gShowTextBox = True
        Me.DragCursor.gTBTransp = 80
        Me.DragCursor.gText = "Example Text" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Second Line" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Third Line"
        Me.DragCursor.gTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.DragCursor.gTextAutoFit = gCursorLib.gCursor.eTextAutoFit.All
        Me.DragCursor.gTextBorderColor = System.Drawing.Color.Black
        Me.DragCursor.gTextBox = New System.Drawing.Size(100, 30)
        Me.DragCursor.gTextBoxColor = System.Drawing.Color.White
        Me.DragCursor.gTextColor = System.Drawing.Color.Black
        Me.DragCursor.gTextFade = gCursorLib.gCursor.eTextFade.Solid
        Me.DragCursor.gTextMultiline = False
        Me.DragCursor.gTextShadow = False
        Me.DragCursor.gTextShadowColor = System.Drawing.Color.Black
        TextShadower1.Alignment = System.Drawing.ContentAlignment.MiddleCenter
        TextShadower1.Blur = 2.0!
        TextShadower1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TextShadower1.Offset = CType(resources.GetObject("TextShadower1.Offset"), System.Drawing.PointF)
        TextShadower1.Padding = New System.Windows.Forms.Padding(0)
        TextShadower1.ShadowColor = System.Drawing.Color.Black
        TextShadower1.ShadowTransp = 128
        TextShadower1.Text = "Drop Shadow"
        TextShadower1.TextColor = System.Drawing.Color.Blue
        Me.DragCursor.gTextShadower = TextShadower1
        Me.DragCursor.gTTransp = 0
        Me.DragCursor.gType = gCursorLib.gCursor.eType.Text
        '
        'PlaniMonthView1
        '
        Me.PlaniMonthView1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.PlaniMonthView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PlaniMonthView1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.PlaniMonthView1.GridBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.PlaniMonthView1.Language = PlaniSoftwareControlLibrary.PlaniMonthView.Idiom.Català
        Me.PlaniMonthView1.Location = New System.Drawing.Point(12, 9)
        Me.PlaniMonthView1.MaximumSize = New System.Drawing.Size(234, 150)
        Me.PlaniMonthView1.MinimumSize = New System.Drawing.Size(234, 150)
        Me.PlaniMonthView1.Name = "PlaniMonthView1"
        Me.PlaniMonthView1.SelectedDate = New Date(2016, 7, 10, 0, 0, 0, 0)
        Me.PlaniMonthView1.Size = New System.Drawing.Size(234, 150)
        Me.PlaniMonthView1.TabIndex = 22
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(149, 165)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 23
        Me.Button2.Text = "Print"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'PlaniGrid1
        '
        Me.PlaniGrid1.AllowMultiDayElements = True
        Me.PlaniGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PlaniGrid1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.PlaniGrid1.BackColor = System.Drawing.SystemColors.ControlDark
        Me.PlaniGrid1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PlaniGrid1.DisplayDate = New Date(2016, 7, 10, 0, 0, 0, 0)
        Me.PlaniGrid1.EndTimeOfDay = "22:00"
        Me.PlaniGrid1.ExtResourceOverlap = True
        Me.PlaniGrid1.GridBackColor = System.Drawing.SystemColors.Window
        Me.PlaniGrid1.HeaderBackColor = System.Drawing.SystemColors.ControlDark
        Me.PlaniGrid1.HeaderFontColor = System.Drawing.SystemColors.Window
        Me.PlaniGrid1.HLinesColor = System.Drawing.SystemColors.Control
        Me.PlaniGrid1.Language = PlaniSoftwareControlLibrary.PlaniGrid.PGIdioma.English
        Me.PlaniGrid1.LastWorkingDayOfWeek = System.DayOfWeek.Saturday
        Me.PlaniGrid1.Location = New System.Drawing.Point(252, 9)
        Me.PlaniGrid1.LunchTimeEnd = "15:00"
        Me.PlaniGrid1.LunchTimeStart = "13:00"
        Me.PlaniGrid1.MarkDoneElements = PlaniSoftwareControlLibrary.PlaniGrid.PGVistaFetes.Underline
        Me.PlaniGrid1.MinColsWidth = 100
        Me.PlaniGrid1.MinRowsHeight = 13
        Me.PlaniGrid1.MinutesGap = 30
        Me.PlaniGrid1.Name = "PlaniGrid1"
        Me.PlaniGrid1.ScrollPosition = New System.Drawing.Point(0, 0)
        Me.PlaniGrid1.Size = New System.Drawing.Size(651, 541)
        Me.PlaniGrid1.SnapToGrid = True
        Me.PlaniGrid1.StartTimeOfDay = "07:00"
        Me.PlaniGrid1.TabIndex = 21
        Me.PlaniGrid1.VisualizationMode = PlaniSoftwareControlLibrary.PlaniGrid.PGMode.Week
        Me.PlaniGrid1.VLinesColor = System.Drawing.SystemColors.ButtonShadow
        '
        'TestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(908, 562)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.PlaniMonthView1)
        Me.Controls.Add(Me.PlaniGrid1)
        Me.Controls.Add(Me.ListBox3)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ListBox2)
        Me.Controls.Add(Me.aag)
        Me.Controls.Add(Me.ListBox1)
        Me.Name = "TestForm"
        Me.Text = "TestApplication"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListBox3 As System.Windows.Forms.ListBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents aag As System.Windows.Forms.CheckBox
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents PlaniGrid1 As PlaniSoftwareControlLibrary.PlaniGrid
    Friend WithEvents DragCursor As gCursorLib.gCursor
    Friend WithEvents PlaniMonthView1 As PlaniSoftwareControlLibrary.PlaniMonthView
    Friend WithEvents Button2 As System.Windows.Forms.Button

End Class
