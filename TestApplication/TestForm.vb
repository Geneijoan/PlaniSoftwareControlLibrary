Friend Class TestForm
    ReadOnly rand As New Random
    Dim randomcolor As Color
    ReadOnly colorsrecursos As New List(Of Color)
    Dim comptaIds As Long = 1
    Private Source As Control

    Private Sub FormProves_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim i As Integer

        For i = 0 To 30
            ListBox1.Items.Add("Element" + Format(i, "00"))
            ListBox2.Items.Add("Recurs" + Format(i, "00"))
            randomcolor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))
            If i = 0 Then
                colorsrecursos.Add(Color.Empty)
            Else
                colorsrecursos.Add(randomcolor)
            End If
        Next

        aag.Checked = PlaniGrid1.SnapToGrid
        PlaniGrid1.VisualizationMode = PlaniSoftwareControlLibrary.PlaniGrid.PGMode.Month
        'For i = 0 To 255
        '    Debug.WriteLine(i & " " & Chr(i))
        'Next

    End Sub

    Private Sub ListBox1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles ListBox1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            DragCursor.gEffect = gCursorLib.gCursor.eEffect.Copy
        Else
            DragCursor.gEffect = gCursorLib.gCursor.eEffect.No
        End If
        Cursor.Current = DragCursor.gCursor
    End Sub

    Private Sub ListBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDown
        Dim auxPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement
        Dim auxPGRecursos As Dictionary(Of String, Color)
        Dim returnvalue As System.Windows.Forms.DragDropEffects

        'obtenim els recursos seleccionats i els afegim a l'element del drag
        auxPGRecursos = New Dictionary(Of String, Color)
        For Each r In ListBox2.SelectedIndices
            'auxPGRecurs = New PlaniGrid.PlaniGrid.PGResource(ListBox2.Items(r), colorsrecursos.Item(r))
            If Not auxPGRecursos.ContainsKey(ListBox2.Items(r)) Then auxPGRecursos.Add(ListBox2.Items(r), colorsrecursos.Item(r))
        Next

        'prova element buit
        'auxPGElement = New PlaniGrid.PGElement(" ", " ", TimeSpan.Zero, TimeSpan.Zero, Color.Empty, New PlaniGrid.PGResources)

        'If ListBox1.SelectedIndex Mod 2 = 0 And PlaniGrid1.PGResourcesColor(auxPGRecursos) = Color.Empty Then
        'auxPGElement = New PlaniGrid.PGElement(CStr(ListBox1.SelectedIndex), ListBox1.SelectedItem, TimeSpan.Zero, New TimeSpan(ListBox1.SelectedIndex, 0, 0), Color.Yellow, auxPGRecursos)
        'Else

        'auxPGElement = New PlaniGrid.PlaniGrid.PGElement(CStr(ListBox1.SelectedIndex), ListBox1.SelectedItem, TimeSpan.Zero, New TimeSpan(ListBox1.SelectedIndex, 0, 0), PlaniGrid1.PGResourcesColor(auxPGRecursos), auxPGRecursos)
        'auxPGElement = New PlaniGrid.PlaniGrid.PGElement(comptaIds.ToString, ListBox1.SelectedItem, TimeSpan.Zero, New TimeSpan(ListBox1.SelectedIndex, 0, 0), PlaniGrid1.PGResourcesColor(auxPGRecursos), auxPGRecursos)
        auxPGElement = New PlaniSoftwareControlLibrary.PlaniGrid.PGElement(comptaIds.ToString, ListBox1.SelectedItem, TimeSpan.Zero, New TimeSpan(ListBox1.SelectedIndex, 0, 0), Color.Empty, auxPGRecursos)
        'End If

        DragCursor.gText = ListBox1.SelectedItem.ToString
        DragCursor.gFont = Me.Font
        DragCursor.gTextBoxColor = IIf(auxPGRecursos.Count > 0, PlaniGrid1.PGResourcesColor(auxPGRecursos), ListBox1.BackColor)
        DragCursor.gTBTransp = 0
        DragCursor.gTextColor = PlaniGrid1.ContrastedColor(DragCursor.gTextBoxColor)
        DragCursor.MakeCursor()
        Source = CType(sender, Control)

        returnvalue = ListBox1.DoDragDrop(auxPGElement, DragDropEffects.Copy)
        If (returnvalue And DragDropEffects.Copy) = DragDropEffects.Copy Then
            For i = 0 To ListBox2.Items.Count - 1
                ListBox2.SetSelected(i, False)
            Next
            ListBox2.Refresh()
        End If

        'Debug.WriteLine(returnValue.ToString)

    End Sub

    Private Sub Aag_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles aag.CheckedChanged
        PlaniGrid1.SnapToGrid = aag.Checked
    End Sub

    Private Sub ListBox2_DrawItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles ListBox2.DrawItem
        Dim auxbrush As System.Drawing.Brush

        randomcolor = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))
        auxbrush = New System.Drawing.SolidBrush(colorsrecursos.Item(e.Index))

        e.DrawBackground()
        e.Graphics.FillRectangle(auxbrush, e.Bounds)
        Using b As New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(ListBox2.GetItemText(ListBox2.Items(e.Index)), e.Font, b, e.Bounds)
        End Using
        e.DrawFocusRectangle()

    End Sub

    Private Sub ListBox2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox2.MouseDown
        Dim returnValue As DragDropEffects
        'Dim auxPGRecurs As PlaniGrid.PlaniGrid.PGResource
        Dim auxPGRecursos As Dictionary(Of String, Color)

        auxPGRecursos = New Dictionary(Of String, Color)
        For Each r In ListBox2.SelectedIndices
            'auxPGRecurs = New PlaniGrid.PlaniGrid.PGResource(ListBox2.Items(r), colorsrecursos.Item(r))
            If Not auxPGRecursos.ContainsKey(ListBox2.Items(r)) Then auxPGRecursos.Add(ListBox2.Items(r), colorsrecursos.Item(r))
        Next

        If ListBox2.SelectedItems.Count > 0 Then
            returnValue = DoDragDrop(auxPGRecursos, DragDropEffects.Copy Or DragDropEffects.Move)
            If returnValue = DragDropEffects.Move Or returnValue = DragDropEffects.Copy Then
                For i = 0 To ListBox2.Items.Count - 1
                    ListBox2.SetSelected(i, False)
                Next
                ListBox2.Refresh()
            End If
        End If

    End Sub

    Private Sub PlaniGridUserControl1_PGElement_Added(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement) Handles PlaniGrid1.PGElement_Added
        Debug.WriteLine("RAISED PGElement_Added: " & pPGElement.Name & " " & pPGElement.Id)
        comptaIds += 1
        'If pPGElement.Id = pPGElement.NewId Then
        '    comptaIds += 1
        '    PlaniGrid1.PGElementChangeId(pPGElement.Id, comptaIds.ToString)
        '    Debug.WriteLine("PGElement Id Changed: " & pPGElement.Id & " -> " & comptaIds.ToString)
        'End If
        'MsgBox("RAISED PGElement_Added: " & pPGElement.Name)
    End Sub

    Private Sub PlaniGridUserControl1_PGElement_Clicked(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement) Handles PlaniGrid1.PGElement_Clicked
        'MsgBox("RAISED PGElement_Clicked: " & pPGElement.Name)
        Debug.WriteLine("RAISED PGElement_Clicked: " & pPGElement.Name)

        'DragCursor.gText = pPGElement.Name
        'DragCursor.gFont = Me.Font
        'DragCursor.gTextBoxColor = IIf(pPGElement.Resources.Count > 0, PlaniGrid1.PGResourcesColor(pPGElement.Resources), Me.BackColor)
        'DragCursor.gTBTransp = 0
        'DragCursor.MakeCursor()
        ''Source = CType(sender, Control)

        'If (ListBox1.DoDragDrop(pPGElement, DragDropEffects.Move) And DragDropEffects.Move) = DragDropEffects.Move Then
        '    Debug.WriteLine("Element " & pPGElement.Id & " mogut")
        'End If

    End Sub

    Private Sub PlaniGridUserControl1_PGElement_Deleted(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement) Handles PlaniGrid1.PGElement_Deleted
        'MsgBox("RAISED PGElement_Deleted: " & pPGElement.Name)
        Debug.WriteLine("RAISED PGElement_Deleted: " & pPGElement.Name)
    End Sub

    Private Sub PlaniGridUserControl1_PGElement_DoubleClicked(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement) Handles PlaniGrid1.PGElement_DoubleClicked
        'MsgBox("RAISED PGElement_DoubleClicked: " & pPGElement.Name)
        Debug.WriteLine("RAISED PGElement_DoubleClicked: " & pPGElement.Name)
    End Sub

    Private Sub PlaniGridUserControl1_PGElement_KeyDown(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles PlaniGrid1.PGElement_KeyDown
        Debug.WriteLine("RAISED PGElement_KeyDown: " & pPGElement.Name & " " & e.KeyData.ToString)
    End Sub

    Private Sub PlaniGridUserControl1_PGElement_Updated(ByVal pPGElement As PlaniSoftwareControlLibrary.PlaniGrid.PGElement) Handles PlaniGrid1.PGElement_Updated
        Debug.WriteLine("RAISED PGElement_Updated: " & pPGElement.Name)
        'MsgBox("RAISED PGElement_Updated: " & pPGElement.Name)
    End Sub

    Private Sub PlaniGrid1_PGElements_Unselected() Handles PlaniGrid1.PGElements_Unselected
        Debug.WriteLine("RAISED PGElements_Unselected ")
    End Sub

    Private Sub PlaniGridUserControl1_PGMessage(ByVal pTMessage As String, ByVal pMessage As String) Handles PlaniGrid1.PGMessage
        'Debug.WriteLine("RAISED Message: " & pTMessage & " " & pMessage)
        MsgBox("RAISED Message: " & pMessage, MsgBoxStyle.OkOnly, pTMessage)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        PlaniGrid1.PGElementsClearList()
        PlaniGrid1.RefreshGrid()
    End Sub

    Private Sub ListBox3_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox3.DoubleClick
        Dim auxList As List(Of PlaniSoftwareControlLibrary.PlaniGrid.PGElement)

        auxList = PlaniGrid1.PGElementsGetList

        ListBox3.Items.Clear()
        For Each element In auxList
            ListBox3.Items.Add(element.Name)
        Next
    End Sub

    Private Sub PlaniGridUserControl1_PGPeriod_Changed(ByVal pMode As PlaniSoftwareControlLibrary.PlaniGrid.PGMode, ByVal pCurrentDate As Date) Handles PlaniGrid1.PGPeriod_Changed
        Debug.WriteLine("RAISED PGPeriod_Changed: " & pMode.ToString & " " & pCurrentDate)
        'PlaniCalendar1.DateSelected = pCurrentDate
        PlaniMonthView1.SelectedDate = pCurrentDate
    End Sub

    Private Sub PlaniMonthView1_DateChanged(ByVal pDate As Date) Handles PlaniMonthView1.DateChanged
        PlaniGrid1.DisplayDate = pDate
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PrintForm.Show()
        PrintForm.CrystalReportViewer1.ReportSource = PlaniGrid1.PGPrint("Text al peu")
        PrintForm.CrystalReportViewer1.Refresh()
    End Sub

End Class
