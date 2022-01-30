Public Class PlaniMonthView

    Public Event DateChanged(ByVal pDate As Date)

#Region "Tipos i constants"

    'idiomes del control
    Public Enum Idiom
        Català
        Castellano
        English
    End Enum

    ReadOnly DiesSetmanaCurts() As String = {"dg", "dl", "dm", "dc", "dj", "dv", "ds"}
    ReadOnly Mesos() As String = {"Gener", "Febrer", "Març", "Abril", "Maig", "Juny", "Juliol", "Agost", "Setembre", "Octubre", "Novembre", "Desembre"}

#End Region

#Region "Variables Globals"

    Dim prIdioma As Idiom = Idiom.Català
    Dim prGridBackColor As System.Drawing.Color = Color.FromName(System.Drawing.KnownColor.Window.ToString)

    Dim prDataSel As Date = Today
    Dim PrimeraData As Date = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek > 0, DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek - 7, DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek), DateSerial(Year(Today), Month(Today), 1))
    Dim UltimaData As Date = DateAdd(DateInterval.Day, 41, DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek > 0, DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek - 7, DayOfWeek.Monday - DateSerial(Year(Today), Month(Today), 1).DayOfWeek), DateSerial(Year(Today), Month(Today), 1)))

#End Region

#Region "Propietats"

    <BrowsableAttribute(False)> _
    Public Overrides Property Font() As System.Drawing.Font
        Get
            Return MyBase.Font
        End Get
        Set(ByVal value As System.Drawing.Font)
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property AutoScroll() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property AutoSize() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    Public Overrides Property BackColor() As System.Drawing.Color
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            MyBase.BackColor = value
            AnyMes.BackColor = value
            MonthGrid.DefaultCellStyle.SelectionBackColor = value
            CarregaGrid()
        End Set
    End Property

    Public Overrides Property ForeColor() As System.Drawing.Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            MyBase.ForeColor = value
            CarregaGrid()
        End Set
    End Property

    Public Property GridBackColor() As System.Drawing.Color
        Get
            Return prGridBackColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            prGridBackColor = value
            AnyMes.ForeColor = prGridBackColor
            MonthGrid.GridColor = prGridBackColor
            MonthGrid.BackgroundColor = prGridBackColor
            MonthGrid.DefaultCellStyle.BackColor = prGridBackColor
            CarregaGrid()
        End Set
    End Property

    Public Property SelectedDate() As Date
        Get
            Return prDataSel
        End Get
        Set(ByVal value As Date)
            Dim auxdate As Date = prDataSel.Date
            prDataSel = value.Date
            'calculem la primera i la ultima data de visualització 
            PrimeraData = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DateSerial(Year(prDataSel), Month(prDataSel), 1).DayOfWeek > 0, DayOfWeek.Monday - DateSerial(Year(prDataSel), Month(prDataSel), 1).DayOfWeek - 7, DayOfWeek.Monday - DateSerial(Year(prDataSel), Month(prDataSel), 1).DayOfWeek), DateSerial(Year(prDataSel), Month(prDataSel), 1))
            UltimaData = DateAdd(DateInterval.Day, 41, PrimeraData)
            CarregaGrid()
            If value.Date <> auxdate.Date Then
                RaiseEvent DateChanged(prDataSel)
            End If
        End Set
    End Property

    Public Property Language() As Idiom
        Get
            Return prIdioma
        End Get
        Set(ByVal value As Idiom)
            prIdioma = value
            CarregaGrid()
        End Set
    End Property

#End Region

#Region "Events"

    Private Sub PlaniMonthView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MonthGrid.Rows.Add(6)
        CarregaT()
        SelectedDate = Today
    End Sub

    Private Sub MonthGrid_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles MonthGrid.CellMouseClick
        Dim auxdate As Date = CoordToDate(New Point(e.ColumnIndex, e.RowIndex))
        If e.ColumnIndex > -1 And e.RowIndex > -1 Then SelectedDate = auxdate
    End Sub

    Private Sub MonthGrid_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MonthGrid.KeyDown
        e.Handled = True
    End Sub

    Private Sub MonthGrid_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MonthGrid.KeyPress
        e.Handled = True
    End Sub

    Private Sub MonthGrid_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MonthGrid.KeyUp
        e.Handled = True
    End Sub

    Private Sub MonthGrid_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles MonthGrid.PreviewKeyDown

        Select Case e.KeyValue
            Case 37 'left
                SelectedDate = DateAdd(DateInterval.Day, -1, SelectedDate)

            Case 38 'up
                SelectedDate = DateAdd(DateInterval.Day, -7, SelectedDate)

            Case 39 'right
                SelectedDate = DateAdd(DateInterval.Day, 1, SelectedDate)

            Case 40 'down
                SelectedDate = DateAdd(DateInterval.Day, 7, SelectedDate)
        End Select

        e.IsInputKey = False
    End Sub

    Private Sub MesAnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MesAnt.Click
        SelectedDate = DateAdd(DateInterval.Month, -1, SelectedDate)
    End Sub

    Private Sub MesPos_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MesPos.Click
        SelectedDate = DateAdd(DateInterval.Month, 1, SelectedDate)
    End Sub

#End Region

#Region "Subrutines i funcions"

    'per traduir textos a l'idioma seleccionat del control
    Function T(ByVal pTxt As String) As String
        Select Case Language
            Case Idiom.English
                If vpEN.ContainsKey(pTxt) Then
                    Return vpEN(pTxt)
                Else
                    Return pTxt
                End If

            Case Idiom.Castellano
                If vpES.ContainsKey(pTxt) Then
                    Return vpES(pTxt)
                Else
                    Return pTxt
                End If
            Case Else
                Return pTxt
        End Select

    End Function

    Private Sub CarregaGrid()
        Dim auxdata As Date
        Dim k As Integer = 0

        AnyMes.Text = T(Mesos(prDataSel.Month - 1)) & " " & prDataSel.Year

        For i = 1 To MonthGrid.Rows.Count
            For j = 1 To MonthGrid.Columns.Count

                'escrivim les capçaleres de columna
                If i = 1 Then
                    MonthGrid.Columns(j - 1).HeaderText = T(DiesSetmanaCurts(j Mod 7))
                End If

                auxdata = DateAdd(DateInterval.Day, k, PrimeraData)
                'escrivim el dia del mes
                MonthGrid.Item(j - 1, i - 1).Value = auxdata.Day

                'els dies de una altre mes en gris
                If auxdata.Month <> prDataSel.Month Then
                    MonthGrid.Item(j - 1, i - 1).Style.ForeColor = Me.BackColor
                Else
                    MonthGrid.Item(j - 1, i - 1).Style.ForeColor = Me.ForeColor
                End If

                'seleccionem la cel·la de la data seleccionada
                If auxdata = prDataSel Then
                    MonthGrid.Item(j - 1, i - 1).Selected = True
                Else
                    MonthGrid.Item(j - 1, i - 1).Selected = False
                End If

                'marquem el dia actual
                If auxdata = Today Then
                    MonthGrid.Item(j - 1, i - 1).Style.Font = New Font(Me.Font, FontStyle.Underline)
                Else
                    MonthGrid.Item(j - 1, i - 1).Style.Font = New Font(Me.Font, FontStyle.Regular)
                End If

                k += 1
            Next
        Next

    End Sub

    Private Function CoordToDate(ByVal pCoord As Point) As Date
        Return DateAdd(DateInterval.Day, pCoord.Y * 7 + pCoord.X, PrimeraData)
    End Function

    Private Function DateToCoord(ByVal pDate As Date) As Point
        Dim auxint As Integer = DateDiff(DateInterval.Day, PrimeraData, pDate)

        If auxint < 0 Then
            Return New Point(-1, -1)
        Else
            If auxint > 41 Then
                Return New Point(7, 6)
            Else
                Return New Point(auxint Mod 7, auxint \ 7)
            End If
        End If

    End Function

#End Region

End Class
