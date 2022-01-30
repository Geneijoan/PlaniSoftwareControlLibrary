Public Class PlaniCalendar

    Public Event DateChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DateRangeEventArgs)
    ReadOnly mesos() As String = {"Gener", "Febrer", "Març", "Abril", "Maig", "Juny", "Juliol", "Agost", "Setembre", "Octubre", "Novembre", "Desembre"}
    ReadOnly meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
    ReadOnly months() As String = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"}

    ReadOnly dies() As String = {"dl", "dm", "dc", "dj", "dv", "ds", "dg"}
    ReadOnly dias() As String = {"lun", "mar", "mié", "jue", "vie", "sáb", "dom"}
    ReadOnly days() As String = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"}

    Dim prLanguage As Idioma = Idioma.Català

    Enum Idioma
        Català
        Castellano
        English
    End Enum

    '*****************************************
    ' PROPIETATS
    '*****************************************

    Public Property Language() As Idioma
        Get
            Return prLanguage
        End Get
        Set(ByVal value As Idioma)
            prLanguage = value
            ActualitzaLabelMes(MonthCalendar1.SelectionStart)
            ActualitzaLabelsDia()
        End Set
    End Property

    Public Property DateSelected() As Date
        Get
            Return MonthCalendar1.SelectionStart
        End Get
        Set(ByVal value As Date)
            If MonthCalendar1.SelectionStart <> value Then
                MonthCalendar1.SelectionStart = value
                MonthCalendar1.SelectionEnd = value
            End If
        End Set
    End Property

    <BrowsableAttribute(False), DefaultValue(2)> _
    Public Overloads Property BorderStyle() As BorderStyle
        Get
            Return BorderStyle.Fixed3D
        End Get
        Set(ByVal value As BorderStyle)

        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property AutoScroll() As Boolean
        Get
            Return MyBase.AutoScroll
        End Get
        Set(ByVal value As Boolean)
            MyBase.AutoScroll = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property AutoScrollOffset() As System.Drawing.Point
        Get
            Return MyBase.AutoScrollOffset
        End Get
        Set(ByVal value As System.Drawing.Point)
            MyBase.AutoScrollOffset = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overloads Property AutoScrollMargin() As Point
        Get

        End Get
        Set(ByVal value As Point)

        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overloads Property AutoScrollMinSize() As Size
        Get

        End Get
        Set(ByVal value As Size)

        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackColor() As System.Drawing.Color
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            MyBase.BackColor = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackGroundImage() As System.Drawing.Image
        Get
            Return MyBase.BackgroundImage
        End Get
        Set(ByVal value As System.Drawing.Image)
            MyBase.BackgroundImage = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackGroundImageLayout() As ImageLayout
        Get
            Return MyBase.BackgroundImageLayout
        End Get
        Set(ByVal value As ImageLayout)
            MyBase.BackgroundImageLayout = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property Font() As System.Drawing.Font
        Get
            Return MyBase.Font
        End Get
        Set(ByVal value As System.Drawing.Font)
            If Not MyBase.Font.Equals(value) Then
                MyBase.Font = value
            End If
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property MaximumSize() As Size
        Get
            Return MyBase.MaximumSize
        End Get
        Set(ByVal value As Size)
            MyBase.MaximumSize = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property MinimumSize() As Size
        Get
            Return MyBase.MinimumSize
        End Get
        Set(ByVal value As Size)
            MyBase.MinimumSize = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overloads Property Size() As Size
        Get
            Return MyBase.Size
        End Get
        Set(ByVal value As Size)
            MyBase.Size = value
        End Set
    End Property


    '*****************************************
    ' FUNCIONS I PROCEDIMENTS
    '*****************************************

    Private Sub MonthCalendar1_DateChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        ActualitzaLabelMes(e.Start)
        RaiseEvent DateChanged(sender, e)
    End Sub

    Private Sub PlaniCalendar_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MyBase.BorderStyle = Windows.Forms.BorderStyle.Fixed3D
        MyBase.BackColor = MonthCalendar1.BackColor
        ActualitzaLabelMes(MonthCalendar1.SelectionRange.Start)
        ActualitzaLabelsDia()
    End Sub

    Private Sub ActualitzaLabelMes(ByVal pDate As Date)

        Select Case prLanguage

            Case Idioma.English
                LabelMes.Text = months(pDate.Month - 1) & " " & pDate.Year

            Case Idioma.Castellano
                LabelMes.Text = meses(pDate.Month - 1) & " de " & pDate.Year

            Case Else
                LabelMes.Text = mesos(pDate.Month - 1) & " de " & pDate.Year

        End Select
    End Sub

    Private Sub ActualitzaLabelsDia()
        Select Case prLanguage

            Case Idioma.English
                Label1.Text = days(0)
                Label2.Text = days(1)
                Label3.Text = days(2)
                Label4.Text = days(3)
                Label5.Text = days(4)
                Label6.Text = days(5)
                Label7.Text = days(6)

            Case Idioma.Castellano
                Label1.Text = dias(0)
                Label2.Text = dias(1)
                Label3.Text = dias(2)
                Label4.Text = dias(3)
                Label5.Text = dias(4)
                Label6.Text = dias(5)
                Label7.Text = dias(6)

            Case Else
                Label1.Text = dies(0)
                Label2.Text = dies(1)
                Label3.Text = dies(2)
                Label4.Text = dies(3)
                Label5.Text = dies(4)
                Label6.Text = dies(5)
                Label7.Text = dies(6)

        End Select
    End Sub

End Class
