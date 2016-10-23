Option Explicit On

Public Class PlaniGrid

#Region "Definició"
    'DESCRIPCIO DEL CONTROL:
    'Permet planificar PGElements en el temps de forma gràfica

    'Per cada element es defineix:
    ' - identificador (unic)
    ' - nom (per mostrar a la graella)
    ' - data/hora d'inici
    ' - data/hora de final
    ' - durada no compartible (independent del nro de recursos assignats)
    ' - durada compartible (repartida entre el nro de recursos assignats) 
    ' - color (si no informat, es calcula dels recursos)
    ' - coleccio de Recursos assignats 
    '       - nom del recurs (id)
    '       - color del recurs (per visualització diaria i per recurs)
    ' - fet (si l'element està processat o no)

    'Valida el solapament de recursos  

    'Permet diversos modos de visualitzacio:
    ' - mensual
    ' - setmanal
    ' - diaria
    ' - ocupació per recurs

    '* FUNCIONS PENDENTS
    '- marcar festius (fixe x dia/es de la setmana) 
    '- marcar festius a ma en modo mes o per pgm des de fora (ojo persistencia)
    '- boto dret per:
    '     . regular zoom (alt files o periodeminim?) amb roda mouse
    '     . veure o no periodes no hàbils (hores)
    '- hora inici activitat setmanal (primer dia) y hora fi activitat setmanal (ultim dia)

#End Region

#Region "Tipos de dades i classes"

    'CLASSE DoubleBufferedPanel (panel de graella de PlaniGrid)
    Class DoubleBufferedPanel
        Inherits Panel

        Public Sub New()
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            SetStyle(ControlStyles.UserPaint, True)
            UpdateStyles()
        End Sub

    End Class

    'CLASSE Recurs
    Public Class PGResource

        Private prNom As String
        Private prColor As Color

        Public Sub New(ByVal pName As String, ByVal pColor As Color)
            prNom = pName
            prColor = pColor
        End Sub

        Public Property Name() As String
            Get
                Return prNom
            End Get
            Set(ByVal value As String)
                prNom = value
            End Set
        End Property

        Public Property Color() As Color
            Get
                Return prColor
            End Get
            Set(ByVal value As Color)
                prColor = value
            End Set
        End Property

    End Class

    'CLASSE Element
    Public Class PGElement
        Private prId As String              'identificador unic en la grid
        Private prNom As String             'a mostrar en la grid
        Private prInici As DateTime
        Private prFinal As DateTime
        Private prDuradaNoCompartible As TimeSpan
        Private prDuradaCompartible As TimeSpan
        Private prColor As Color            'opcional. si no s'informa (color.empty) es calcula segons recursos
        Private prRecursos As Dictionary(Of String, Color)
        Private prFet As Boolean            'si el element està fet (tancat)

        'per crear un element sense inici ni recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pNonSharedDuration As TimeSpan, ByVal pSharedDuration As TimeSpan, ByVal pColor As Color, Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = Date.MinValue
            prFinal = Date.MinValue
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaCompartible = IIf(pSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pSharedDuration)
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            prFet = pDone
        End Sub

        'per crear un element sense inici pero amb recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pNonSharedDuration As TimeSpan, ByVal pSharedDuration As TimeSpan, ByVal pColor As Color, ByVal pResources As Dictionary(Of String, Color), Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = Date.MinValue
            prFinal = Date.MinValue
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaCompartible = IIf(pSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pSharedDuration)
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            For Each e In pResources
                prRecursos.Add(e.Key, e.Value)
            Next
            prFet = pDone
        End Sub

        'per crear un element amb inici i sense recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pStart As Date, ByVal pNonSharedDuration As TimeSpan, ByVal pSharedDuration As TimeSpan, ByVal pColor As Color, Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = pStart
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaCompartible = IIf(pSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pSharedDuration)
            prFinal = IIf(prInici = Date.MinValue, Date.MinValue, prInici + prDuradaCompartible + prDuradaNoCompartible)
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            prFet = pDone
        End Sub

        'per crear un element amb inici i amb recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pStart As Date, ByVal pNonSharedDuration As TimeSpan, ByVal pSharedDuration As TimeSpan, ByVal pColor As Color, ByVal pResources As Dictionary(Of String, Color), Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = pStart
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaCompartible = IIf(pSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pSharedDuration)
            prFinal = IIf(prInici = Date.MinValue, Date.MinValue, prInici + prDuradaNoCompartible + New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(pResources.Count > 0, pResources.Count, 1), 0))
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            For Each e In pResources
                prRecursos.Add(e.Key, e.Value)
            Next
            prFet = pDone
        End Sub

        'per crear un element amb inici i final, sense recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pStart As Date, ByVal pFinal As Date, ByVal pNonSharedDuration As TimeSpan, ByVal pColor As Color, Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = pStart
            prFinal = IIf(prInici = Date.MinValue, Date.MinValue, IIf(pFinal > pStart, pFinal, pStart))
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaNoCompartible = IIf(prDuradaNoCompartible < (prFinal - prInici), prDuradaNoCompartible, (prFinal - prInici))
            prDuradaCompartible = (prFinal - prInici) - prDuradaNoCompartible
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            prFet = pDone
        End Sub

        'per crear un element amb inici i final, amb recursos assignats
        Public Sub New(ByVal pId As String, ByVal pName As String, ByVal pStart As Date, ByVal pFinal As Date, ByVal pNonSharedDuration As TimeSpan, ByVal pColor As Color, ByVal pResources As Dictionary(Of String, Color), Optional ByVal pDone As Boolean = False)
            prId = pId
            prNom = pName
            prInici = pStart
            prFinal = IIf(prInici = Date.MinValue, Date.MinValue, IIf(pFinal > pStart, pFinal, pStart))
            prDuradaNoCompartible = IIf(pNonSharedDuration < TimeSpan.Zero, TimeSpan.Zero, pNonSharedDuration)
            prDuradaNoCompartible = IIf(prDuradaNoCompartible < (prFinal - prInici), prDuradaNoCompartible, (prFinal - prInici))
            prDuradaCompartible = New TimeSpan(0, ((prFinal - prInici) - prDuradaNoCompartible).TotalMinutes * IIf(pResources.Count > 0, pResources.Count, 1), 0)
            prColor = pColor
            prRecursos = New Dictionary(Of String, Color)
            For Each e In pResources
                prRecursos.Add(e.Key, e.Value)
            Next
            prFet = pDone
        End Sub

        'per clonar un element
        Public Sub New(ByVal pPGElement As PGElement)
            Dim auxTs As TimeSpan

            prId = pPGElement.prId
            prNom = pPGElement.prNom
            If pPGElement.prInici = Date.MinValue Then
                prInici = Date.MinValue
                prFinal = Date.MinValue
                prDuradaNoCompartible = IIf(pPGElement.prDuradaNoCompartible > TimeSpan.Zero, pPGElement.prDuradaNoCompartible, TimeSpan.Zero)
                prDuradaCompartible = IIf(pPGElement.prDuradaCompartible > TimeSpan.Zero, pPGElement.prDuradaCompartible, TimeSpan.Zero)
            Else
                prInici = pPGElement.prInici
                prFinal = IIf(pPGElement.prFinal > pPGElement.prInici, pPGElement.prFinal, pPGElement.prInici)
                prDuradaNoCompartible = IIf(pPGElement.prDuradaNoCompartible > TimeSpan.Zero, pPGElement.prDuradaNoCompartible, TimeSpan.Zero)
                prDuradaNoCompartible = IIf(prDuradaNoCompartible < (prFinal - prInici), prDuradaNoCompartible, (prFinal - prInici))
                prDuradaCompartible = IIf(pPGElement.prDuradaCompartible > TimeSpan.Zero, pPGElement.prDuradaCompartible, TimeSpan.Zero)
                auxTs = New TimeSpan(0, ((prFinal - prInici) - prDuradaNoCompartible).TotalMinutes * IIf(pPGElement.prRecursos.Count > 0, pPGElement.prRecursos.Count, 1), 0)
                'la durada compartible entre el nro. de recursos no pot ser superior a la durada de l'element menys la durada no compartible
                If prDuradaCompartible > auxTs Then prDuradaCompartible = auxTs
            End If
            prColor = pPGElement.prColor
            prRecursos = New Dictionary(Of String, Color)
            For Each e In pPGElement.prRecursos
                prRecursos.Add(e.Key, e.Value)
            Next
            prFet = pPGElement.prFet
        End Sub

        'PROPIETATS
        Public Property Id() As String
            Get
                Return prId
            End Get
            Set(ByVal value As String)
                prId = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return prNom
            End Get
            Set(ByVal value As String)
                prNom = value
            End Set
        End Property

        Public Property Starts() As DateTime
            Get
                Return prInici
            End Get
            Set(ByVal value As DateTime)
                Dim auxTs As TimeSpan
                If value = Date.MinValue Then
                    prInici = value
                    prFinal = value
                Else
                    If value <> prInici Then
                        prFinal = IIf(prFinal = Date.MinValue, prInici, prFinal - (prInici - value))
                        prInici = value
                        auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                        If prFinal < (prInici + prDuradaNoCompartible + auxTs) Then
                            prFinal = prInici + prDuradaNoCompartible + auxTs
                        End If
                    End If
                End If
            End Set
        End Property

        Public Property Ends() As DateTime
            Get
                Return prFinal
            End Get
            Set(ByVal value As DateTime)
                Dim auxTs As TimeSpan
                If value = Date.MinValue Then
                    prInici = Date.MinValue
                    prFinal = Date.MinValue
                Else
                    prFinal = IIf(value > prInici, value, prInici)
                    auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                    If prInici <> Date.MinValue And prFinal <> Date.MinValue And prFinal < (prInici + prDuradaNoCompartible + auxTs) Then
                        prFinal = prInici + prDuradaNoCompartible + auxTs
                    End If
                End If
            End Set
        End Property

        Public Property NonSharedDuration() As TimeSpan
            Get
                Return prDuradaNoCompartible
            End Get
            Set(ByVal value As TimeSpan)
                Dim auxTs As TimeSpan
                If value >= TimeSpan.Zero Then
                    prDuradaNoCompartible = value
                Else
                    prDuradaNoCompartible = TimeSpan.Zero
                End If
                auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                If prInici <> Date.MinValue And prFinal <> Date.MinValue And prFinal < (prInici + prDuradaNoCompartible + auxTs) Then
                    prFinal = prInici + prDuradaNoCompartible + auxTs
                End If
            End Set
        End Property

        Public Property SharedDuration() As TimeSpan
            Get
                Return prDuradaCompartible
            End Get
            Set(ByVal value As TimeSpan)
                Dim auxTs As TimeSpan
                If value >= TimeSpan.Zero Then
                    prDuradaCompartible = value
                Else
                    prDuradaCompartible = TimeSpan.Zero
                End If
                auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                If prFinal < (prInici + prDuradaNoCompartible + auxTs) Then
                    prFinal = prInici + prDuradaNoCompartible + auxTs
                End If
            End Set
        End Property

        Public Property Color() As Color
            Get
                Return prColor
            End Get
            Set(ByVal value As Color)
                prColor = value
            End Set
        End Property

        Public Property Resources() As Dictionary(Of String, Color)
            Get
                Return prRecursos
            End Get
            Set(ByVal value As Dictionary(Of String, Color))
                Dim auxTs As TimeSpan
                prRecursos = value
                If prInici <> Date.MinValue Then
                    auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                    If prFinal < (prInici + prDuradaNoCompartible + auxTs) Then
                        prFinal = prInici + prDuradaNoCompartible + auxTs
                    End If
                End If
            End Set
        End Property

        Public Property Done() As Boolean
            Get
                Return prFet
            End Get
            Set(ByVal value As Boolean)
                prFet = value
            End Set
        End Property

        Public Function AddUpdateResource(ByVal pPGResource As PGResource) As Boolean
            If prRecursos.ContainsKey(pPGResource.Name) Then
                prRecursos(pPGResource.Name) = pPGResource.Color
            Else
                prRecursos.Add(pPGResource.Name, pPGResource.Color)
            End If
            AddUpdateResource = True
        End Function

        Public Function DeleteResource(ByVal pResource As String) As Boolean
            Dim auxTs As TimeSpan

            DeleteResource = prRecursos.Remove(pResource)
            'recalcular prFinal
            If DeleteResource Then
                auxTs = New TimeSpan(0, prDuradaCompartible.TotalMinutes \ IIf(prRecursos.Count > 0, prRecursos.Count, 1), 0)
                If prFinal < prInici + prDuradaNoCompartible + auxTs Then
                    prFinal = prInici + prDuradaNoCompartible + auxTs
                End If
            End If
        End Function

    End Class

#End Region

#Region "Enumeracions i estructures"

    'modes de visualització de la grid
    Enum PGMode
        Day
        Week
        Month
        Resource
    End Enum

    'idiomes del control
    Enum PGIdioma
        Català
        Castellano
        English
    End Enum

    'codis de retorn d'accions sobre PGElement
    Enum PGReturnCode
        PGError
        PGAdded
        PGUpdated
        PGDeleted
        PGSelected
    End Enum

    'estructura per dibuixar N elements en una cel·la
    Structure ElementsCella
        Dim tamany As Size
        Dim elements As List(Of String)
        Dim fets As List(Of Boolean)
    End Structure

    'visualitzacio fetes
    Enum PGVistaFetes
        Bold
        Underline
    End Enum

#End Region

#Region "Constants"

    Dim DiesSetmanaCurts() As String = {"dg", "dl", "dm", "dc", "dj", "dv", "ds"}
    Dim DiesSetmana() As String = {"diumenge", "dilluns", "dimarts", "dimecres", "dijous", "divendres", "dissabte"}
    Dim Mesos() As String = {"Gener", "Febrer", "Març", "Abril", "Maig", "Juny", "Juliol", "Agost", "Setembre", "Octubre", "Novembre", "Desembre"}
    Dim MesosCurts() As String = {"Gen", "Feb", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Des"}

#End Region

#Region "Variables globals"

    'variables propies de PlaniGrid
    Dim vpNroColumnes As Integer
    Dim vpNroFiles As Integer

    Dim vpAmpleColumna As Integer
    Dim vpMinAmpleColumna As Integer   'determina l'ample minim de les columnes

    Dim vpAltFila As Integer
    Dim vpAltFilaMes As Integer

    Dim vpLlistaElements As Dictionary(Of String, PGElement)        'per emmagatzemar els PGElements del control
    Dim vpLlistaElementsData As SortedList(Of String, String)       'per ordenar els PGElements (String clau = DataHoraInici format yyyyMMddHHmmss)
    Dim vpLlistaRecursosDia As SortedList(Of String, PGResource)    'per emmagatzemar els recursos del dia

    'variables auxiliars
    Dim vaScrollPos As Point    'coordenades del rectangle del scroll pel control en DragOver
    Dim vaPuntDragAnt As Point  'ultim punt del drag per calcular el sentit del moviment
    Dim vaElementDrop As String    'per saber si tenim un element valid per fer el drop
    Dim vaRecursosDrop As String    'per saber si tenim recursos valids per fer el drop
    Dim vaDataHoraIniPer As DateTime    'data/hora inici de periode (a les 00:00h.)
    Dim vaDataHoraFiPer As DateTime     'data/hora fi de periode (a les 23:59h.)

    Dim vaElementSel As String = ""     'id del PGElement seleccionat
    Dim vaDataMouse As Date = Date.MinValue      'data on tenim el cursor
    Dim vaRecursSel As String = ""    'recurs de visualització modo recurs
    Dim vaColorSel As Color = Color.Empty   'color del recurs de visualització modo recurs

    Dim vaToolTip1 As New ToolTip   'per mostrar missatges sobre el control
    Dim vaReformateja As Boolean    'per determinar si cal reformatejar el control abans de Paint
    Dim vaPuntClick As Point        'per determinar on hem fet mousedown en coordenades del control
    Dim vaAjustBorder As Integer = 0  'per ajustar les coordenades dels punts del control segons el borderstyle

    Dim vaNElementsCella As Dictionary(Of Point, ElementsCella) 'estructura per controlar quants elements hi ha abans en una mateixa cel·la del mes

    Dim vaFontDone As Font = New Font(Font, FontStyle.Bold)     'per marcar feines fetes
    Dim vaFontPast As Font = New Font(Font, FontStyle.Italic)    'per marcar feines passades
    Dim vaFontDonePast As Font = New Font(Font, FontStyle.Italic Or FontStyle.Bold) 'per marcar feines fetes passades

    'emmagatzemament / inicialitzacio de valors de les propietats 
    Dim prColorLiniesH As Color = SystemColors.Control
    Dim prColorLiniesV As Color = SystemColors.ButtonShadow
    Dim prColorLletresCap As Color = SystemColors.Window
    Dim prVistaFetes As PGVistaFetes = PGVistaFetes.Bold

    Dim prModeVisualitzacio As PGMode = PGMode.Week   'mode de visualitzacio
    Dim prSolapaRecursExt As Boolean    'permet solapament recursos en alta externa d'elements (deixa recurs a blanc)
    Dim prDataActual As DateTime = Today   'determina la data base per la visualització de la graella
    Dim prPrimeraData As Date   'primera data de visualització mes
    Dim prUltimaData As Date    'ultima data de visualització mes

    Dim prMinAmpleColumna As Integer = 100
    Dim prMinAltFila As Integer = Font.Height

    Dim prPeriodeMinimMinuts As Integer = 30                        'per visualitzacio coordenada Y de la graella

    Dim prHoraIniciActivitat As TimeSpan = New TimeSpan(0, 0, 0)    'per definir la finestra d'hores a tractar
    Dim prHoraFiActivitat As TimeSpan = New TimeSpan(24, 0, 0)

    Dim prHoraIniciDinar As TimeSpan = New TimeSpan(13, 0, 0)       'per marcar la finestra d'hores del dinar
    Dim prHoraFiDinar As TimeSpan = New TimeSpan(15, 0, 0)

    Dim prUltimDiaLaborable As System.DayOfWeek = System.DayOfWeek.Saturday  'de la setmana

    Dim prAjustarAGrid As Boolean = True                'per ajustar els elements a la graella o no

    Dim prPermetreElementsEntreDies As Boolean = False  'per ajustar els elements en la graella d'un dia o no

    Friend prIdioma As PGIdioma = PGIdioma.English      'idioma per defecte del control

#End Region

#Region "Events cap al formulari"

    Public Event PGMessage(ByVal pTMessage As String, ByVal pMessage As String)
    Public Event PGElement_Clicked(ByVal pPGElement As PGElement)
    Public Event PGElement_DoubleClicked(ByVal pPGElement As PGElement)
    Public Event PGElement_Added(ByVal pPGElement As PGElement)
    Public Event PGElement_Updated(ByVal pPGElement As PGElement)
    Public Event PGElement_Deleted(ByVal pPGElement As PGElement)
    Public Event PGElement_KeyDown(ByVal pPGElement As PGElement, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs)
    Public Event PGPeriod_Changed(ByVal pMode As PGMode, ByVal pCurrentDate As Date)
    Public Event PGMode_Changed(ByVal pMode As PGMode, ByVal pCurrentDate As Date, ByVal pResource As String)
    Public Event PGElements_Unselected()
    Public Event PGSizeChanged(ByVal pColumnsWidth As Integer, ByVal pRowsHeight As Integer)
    Public Event PGHScroll(ByVal pOldValue As Integer, ByVal pNewValue As Integer)
    Public Event PGVScroll(ByVal pOldValue As Integer, ByVal pNewValue As Integer)

#End Region

#Region "Propietats"
    'Changing How Properties Are Displayed

    'To change how some properties are displayed, you can apply different attributes to the properties. Attributes are declarative tags used to annotate programming elements such as types, fields, methods, and properties that can be retrieved at run time using reflection. Here is a partial list:

    'DescriptionAttribute. Sets the text for the property that is displayed in the description help pane below the properties. This is a useful way to provide help text for the active property (the property that has focus). Apply this attribute to the MaxRepeatRate property.
    'CategoryAttribute. Sets the category that the property is under in the grid. This is useful when you want a property grouped by a category name. If a property does not have a category specified, then it will be assigned to the Misc category. Apply this attribute to all properties.
    'BrowsableAttribute – Indicates whether the property is shown in the grid. This is useful when you want to hide a property from the grid. By default, a public property is always shown in the grid. Apply this attribute to the SettingsChanged property.
    'ReadOnlyAttribute – Indicates whether the property is read-only. This is useful when you want to keep a property from being editable in the grid. By default, a public property with get and set accessor functions is editable in the grid. Apply this attribute to the AppVersion property.
    'DefaultValueAttribute – Identifies the property's default value. This is useful when you want to provide a default value for a property and later determine if the property's value is different than the default. Apply this attribute to all properties.
    'DefaultPropertyAttribute – Identifies the default property for the class. The default property for a class gets the focus first when the class is selected in the grid. Apply this attribute to the AppSettings class.

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Grid horizontal lines color.")> _
    Public Property HLinesColor() As Color
        Get
            Return prColorLiniesH
        End Get
        Set(ByVal value As Color)
            If prColorLiniesH <> value Then
                prColorLiniesH = value
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Grid vertical lines color.")> _
    Public Property VLinesColor() As Color
        Get
            Return prColorLiniesV
        End Get
        Set(ByVal value As Color)
            If prColorLiniesV <> value Then
                prColorLiniesV = value
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Grid backcolor.")> _
    Public Property GridBackColor() As Color
        Get
            Return GridPanel.BackColor
        End Get
        Set(ByVal value As Color)
            If GridPanel.BackColor <> value Then
                GridPanel.BackColor = value
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Headers backcolor.")> _
    Public Property HeaderBackColor() As Color
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As Color)
            If MyBase.BackColor <> value Then
                MyBase.BackColor = value
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Text color in grid headers.")> _
    Public Property HeaderFontColor() As Color
        Get
            Return prColorLletresCap
        End Get
        Set(ByVal value As Color)
            If prColorLletresCap <> value Then
                prColorLletresCap = value
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Mark done elements.")> _
    Public Property MarkDoneElements() As PGVistaFetes
        Get
            Return prVistaFetes
        End Get
        Set(ByVal value As PGVistaFetes)
            If prVistaFetes <> value Then
                prVistaFetes = value

                'creem les fonts pels elements passats i fets
                Try
                    If prVistaFetes = PGVistaFetes.Bold Then
                        vaFontDone = New Font(Font, FontStyle.Bold)
                    Else
                        vaFontDone = New Font(Font, FontStyle.Underline)
                    End If
                Catch ex As Exception
                    vaFontDone = Font
                End Try

                Try
                    If prVistaFetes = PGVistaFetes.Bold Then
                        vaFontDonePast = New Font(Font, FontStyle.Italic Or FontStyle.Bold)
                    Else
                        vaFontDonePast = New Font(Font, FontStyle.Italic Or FontStyle.Underline)
                    End If
                Catch ex As Exception
                    vaFontDonePast = vaFontPast
                End Try

                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), DescriptionAttribute("Controls visualization mode.")> _
    Public Property VisualizationMode() As PGMode
        Get
            Return prModeVisualitzacio
        End Get
        Set(ByVal value As PGMode)
            If VisualizationMode <> value Then
                prModeVisualitzacio = value
                CalculaDatesPeriode()
                vaReformateja = True
                Invalidate()
                RaiseEvent PGMode_Changed(VisualizationMode, DisplayDate, IIf(VisualizationMode = PGMode.Resource, vaRecursSel, ""))
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), DescriptionAttribute("Allows external resource overlap. Clears resource.")> _
    Public Property ExtResourceOverlap() As Boolean
        Get
            Return prSolapaRecursExt
        End Get
        Set(ByVal value As Boolean)
            prSolapaRecursExt = value
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Property DisplayDate() As Date
        Get
            Return prDataActual
        End Get
        Set(ByVal value As Date)
            If value <> DisplayDate Then
                prDataActual = value
                CalculaDatesPeriode()
                vaReformateja = True
                Invalidate()
                RaiseEvent PGPeriod_Changed(VisualizationMode, DisplayDate)
            End If
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public ReadOnly Property DisplayFirstDate() As Date
        Get
            Return prPrimeraData
        End Get
    End Property

    <BrowsableAttribute(False)> _
    Public ReadOnly Property DisplayLastDate() As Date
        Get
            Return prUltimaData
        End Get
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Minimum allowed width of control columns.")> _
    Public Property MinColsWidth() As Integer
        Get
            Return prMinAmpleColumna
        End Get
        Set(ByVal value As Integer)
            If value < vpMinAmpleColumna Then value = vpMinAmpleColumna 'limitem el ample minim de les columnes
            If prMinAmpleColumna <> value Then
                prMinAmpleColumna = value
                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Minimum allowed height of control rows.")> _
    Public Property MinRowsHeight() As Integer
        Get
            Return prMinAltFila
        End Get
        Set(ByVal value As Integer)
            If value < Font.Height Then value = Font.Height 'limitem l'alt minim de les files
            If prMinAltFila <> value Then
                prMinAltFila = value
                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All), _
        DescriptionAttribute("Minutes that one grid row represents.")> _
    Public Property MinutesGap() As Integer
        Get
            Return prPeriodeMinimMinuts
        End Get
        Set(ByVal value As Integer)

            If value < 1 Then value = 1 'minim 1 minut

            If value > DuradaJornada.TotalMinutes Then value = DuradaJornada.TotalMinutes 'maxim 1 jornada

            If prPeriodeMinimMinuts <> value Then
                prPeriodeMinimMinuts = value

                ActualitzaPropietatsPeriode()

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All), _
        DescriptionAttribute("Daily start time of activity.")> _
    Public Property StartTimeOfDay() As String
        Get
            Return Microsoft.VisualBasic.Left(prHoraIniciActivitat.ToString, 5)
        End Get
        Set(ByVal value As String)
            Dim auxTs As TimeSpan

            If value = "24:00" Then
                auxTs = New TimeSpan(24, 0, 0)
            Else
                Try
                    auxTs = TimeSpan.Parse(value)
                Catch ex As Exception
                    auxTs = New TimeSpan(0, 0, 0)
                End Try
            End If

            'no ha de ser negatiu ni superior a 1 dia
            If auxTs.TotalMinutes < 0 Or auxTs.TotalMinutes > 1440 Then
                auxTs = New TimeSpan(0, 0, 0)
            End If

            If prHoraIniciActivitat <> auxTs Then
                prHoraIniciActivitat = auxTs

                ActualitzaPropietatsPeriode()

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All), _
        DescriptionAttribute("Daily end time of activity.")> _
    Public Property EndTimeOfDay() As String
        Get
            'mostrem 24:00 si 1 dia + 00:00h
            Return IIf(prHoraFiActivitat.Days > 0, "24:00", Microsoft.VisualBasic.Left(prHoraFiActivitat.ToString, 5))
        End Get
        Set(ByVal value As String)
            Dim auxTs As TimeSpan

            If value = "24:00" Then
                auxTs = New TimeSpan(24, 0, 0)
            Else
                Try
                    auxTs = TimeSpan.Parse(value)
                Catch ex As Exception
                    auxTs = New TimeSpan(24, 0, 0)
                End Try
            End If

            'no ha de ser negatiu ni superior a 1 dia
            If auxTs.TotalMinutes < 0 Or auxTs.TotalMinutes > 1440 Then
                auxTs = New TimeSpan(24, 0, 0)
            End If

            If prHoraFiActivitat <> auxTs Then
                prHoraFiActivitat = auxTs

                ActualitzaPropietatsPeriode()

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All), _
        DescriptionAttribute("Daily lunch start time.")> _
    Public Property LunchTimeStart() As String
        Get
            Return Microsoft.VisualBasic.Left(prHoraIniciDinar.ToString, 5)
        End Get
        Set(ByVal value As String)
            Dim auxTs As TimeSpan

            If value = "24:00" Then
                auxTs = New TimeSpan(24, 0, 0)
            Else
                Try
                    auxTs = TimeSpan.Parse(value)
                Catch ex As Exception
                    Exit Property
                End Try
            End If

            'no ha de ser negatiu ni superior a 1 dia
            If auxTs.TotalMinutes < 0 Or auxTs.TotalMinutes > 1440 Then
                auxTs = New TimeSpan(13, 0, 0)
            End If

            If prHoraIniciDinar <> auxTs Then
                prHoraIniciDinar = auxTs

                ActualitzaPropietatsPeriode()

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All), _
        DescriptionAttribute("Daily lunch end time.")> _
    Public Property LunchTimeEnd() As String
        Get
            'mostrem 24:00 si 1 dia + 00:00h
            Return IIf(prHoraFiDinar.Days > 0, "24:00", Microsoft.VisualBasic.Left(prHoraFiDinar.ToString, 5))
        End Get
        Set(ByVal value As String)
            Dim auxTs As TimeSpan

            If value = "24:00" Then
                auxTs = New TimeSpan(24, 0, 0)
            Else
                Try
                    auxTs = TimeSpan.Parse(value)
                Catch ex As Exception
                    Exit Property
                End Try
            End If

            'no ha de ser negatiu ni superior a 1 dia
            If auxTs.TotalMinutes < 0 Or auxTs.TotalMinutes > 1440 Then
                auxTs = New TimeSpan(15, 0, 0)
            End If

            If prHoraFiDinar <> auxTs Then
                prHoraFiDinar = auxTs

                ActualitzaPropietatsPeriode()

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Last working day of week.")> _
    Public Property LastWorkingDayOfWeek() As System.DayOfWeek
        Get
            Return prUltimDiaLaborable
        End Get
        Set(ByVal value As System.DayOfWeek)
            If value <> prUltimDiaLaborable Then
                prUltimDiaLaborable = value
                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), DescriptionAttribute("Determines the default adjustement of the elements in the grid.")> _
    Public Property SnapToGrid() As Boolean
        Get
            Return prAjustarAGrid
        End Get
        Set(ByVal value As Boolean)
            prAjustarAGrid = value
            vaReformateja = True
            Invalidate()
        End Set
    End Property

    <System.ComponentModel.Category("Comportamiento"), DescriptionAttribute("Determines if an element can exceed a day.")> _
    Public Property AllowMultiDayElements() As Boolean
        Get
            Return prPermetreElementsEntreDies
        End Get
        Set(ByVal value As Boolean)
            If value <> prPermetreElementsEntreDies Then
                prPermetreElementsEntreDies = value
                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <System.ComponentModel.Category("Apariencia"), DescriptionAttribute("Control language.")> _
    Public Property Language() As PGIdioma
        Get
            Return prIdioma
        End Get
        Set(ByVal value As PGIdioma)
            prIdioma = value
            vaReformateja = True
            Invalidate()
        End Set
    End Property

    <DefaultValue(True)> _
    Public Overrides Property AllowDrop() As Boolean
        Get
            Return GridPanel.AllowDrop
        End Get
        Set(ByVal value As Boolean)
            GridPanel.AllowDrop = value
        End Set
    End Property

    <DefaultValue(BorderStyle.Fixed3D)> _
    Public Shadows Property BorderStyle() As System.Windows.Forms.BorderStyle
        Get
            Return MyBase.BorderStyle
        End Get
        Set(ByVal value As System.Windows.Forms.BorderStyle)
            MyBase.BorderStyle = value
            GridPanel.BorderStyle = value
        End Set
    End Property

    Public Overrides Property Font() As System.Drawing.Font
        Get
            Return MyBase.Font
        End Get
        Set(ByVal value As System.Drawing.Font)
            If Not MyBase.Font.Equals(value) Then
                MyBase.Font = value
                If MinRowsHeight < MyBase.Font.Height Then MinRowsHeight = MyBase.Font.Height

                'creem les fonts pels elements passats i fets
                Try
                    If prVistaFetes = PGVistaFetes.Bold Then
                        vaFontDone = New Font(Font, FontStyle.Bold)
                    Else
                        vaFontDone = New Font(Font, FontStyle.Underline)
                    End If
                Catch ex As Exception
                    vaFontDone = Font
                End Try

                Try
                    vaFontPast = New Font(Font, FontStyle.Italic)
                Catch ex As Exception
                    vaFontPast = Font
                End Try

                Try
                    If prVistaFetes = PGVistaFetes.Bold Then
                        vaFontDonePast = New Font(Font, FontStyle.Italic Or FontStyle.Bold)
                    Else
                        vaFontDonePast = New Font(Font, FontStyle.Italic Or FontStyle.Underline)
                    End If
                Catch ex As Exception
                    vaFontDonePast = vaFontPast
                End Try

                vaReformateja = True
                Invalidate()
            End If
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackgroundImage() As System.Drawing.Image
        Get
            Return MyBase.BackgroundImage
        End Get
        Set(ByVal value As System.Drawing.Image)
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackgroundImageLayout() As System.Windows.Forms.ImageLayout
        Get
            Return MyBase.BackgroundImageLayout
        End Get
        Set(ByVal value As System.Windows.Forms.ImageLayout)
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Overrides Property BackColor() As System.Drawing.Color
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As System.Drawing.Color)
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
    Public Overrides Property AutoScrollOffset() As System.Drawing.Point
        Get
            Return New System.Drawing.Point(0, 0)
        End Get
        Set(ByVal value As System.Drawing.Point)
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public Property ScrollPosition() As System.Drawing.Point
        Get
            Return GridPanel.AutoScrollPosition
        End Get
        Set(ByVal value As System.Drawing.Point)
            If value <> GridPanel.AutoScrollPosition Then
                GridPanel.AutoScrollPosition = value
                Invalidate()
            End If
        End Set
    End Property

    <BrowsableAttribute(False)> _
    Public ReadOnly Property ColumnWidth() As Integer
        Get
            Return vpAmpleColumna
        End Get
    End Property

    <BrowsableAttribute(False)> _
    Public ReadOnly Property RowHeight() As Integer
        Get
            Return vpAltFila
        End Get
    End Property
#End Region

#Region "Overrides"

    'SOBREESCRIPTURA DE FUNCIONS BASE PER TRACATAMENTS ESPECIALS

    Protected Overrides Function IsInputKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If keyData = Keys.Up Or keyData = Keys.Down Or keyData = Keys.Right Or keyData = Keys.Left Then
            Return True
        Else
            Return MyBase.IsInputKey(keyData)
        End If
        Debug.WriteLine("IsInputKey " & keyData.ToString)
    End Function

#End Region

#Region "Tracatament d'events"

    Private Sub PlaniGrid_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FontChanged
        vaReformateja = True
        Invalidate()
    End Sub

    Private Sub PlaniGrid_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        vpLlistaElements = New Dictionary(Of String, PGElement)     'llista d'elements
        vpLlistaElementsData = New SortedList(Of String, String)    'llista d'elements ordenada per data
        vpLlistaRecursosDia = New SortedList(Of String, PGResource) 'llista de recursos del dia actual

        'carrega taules de traduccio
        CarregaT()

        'variables auxiliars
        vaScrollPos = New Point(0, 0)
        vaPuntDragAnt = New Point(0, 0)
        vaElementDrop = ""
        vaElementSel = ""
        vaRecursosDrop = ""

        DisplayDate = Today

        vaToolTip1.Active = False

        vaReformateja = True
        Invalidate()

    End Sub

    Private Sub PlaniGrid_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDoubleClick
        Dim auxData As DateTime
        Dim idxrecurs As Integer

        If vpAmpleColumna <= 0 Then Exit Sub

        idxrecurs = ((vaPuntClick.X - GridPanel.Left - GridPanel.AutoScrollPosition.X) \ vpAmpleColumna) '- 1

        Select Case VisualizationMode

            Case PGMode.Day
                If vaPuntClick.X >= GridPanel.Left Then
                    If idxrecurs < 0 Or idxrecurs > vpLlistaRecursosDia.Count - 1 Then Exit Sub
                    vaRecursSel = vpLlistaRecursosDia.ElementAt(idxrecurs).Value.Name
                    vaColorSel = vpLlistaRecursosDia.ElementAt(idxrecurs).Value.Color
                    VisualizationMode = PGMode.Resource
                Else
                    VisualizationMode = PGMode.Week
                End If

            Case PGMode.Week
                auxData = DePosicioADataHora(vaPuntClick, False)
                If auxData.Date <> DateTime.MinValue Then
                    DisplayDate = auxData.Date
                    VisualizationMode = PGMode.Day
                Else
                    VisualizationMode = PGMode.Month
                End If

            Case PGMode.Month
                auxData = DePosicioADataHora(vaPuntClick, False)
                If auxData.Date <> DateTime.MinValue Then
                    DisplayDate = auxData.Date
                    VisualizationMode = PGMode.Week
                End If

            Case PGMode.Resource
                auxData = DePosicioADataHora(vaPuntClick, False)
                If auxData.Date <> DateTime.MinValue Then
                    DisplayDate = auxData.Date
                    VisualizationMode = PGMode.Day
                Else
                    VisualizationMode = PGMode.Week
                End If

        End Select

        If VisualizationMode = PGMode.Day Then vaReformateja = True
        GridPanel.AutoScrollPosition = New Point(0, 0)
        Invalidate()

    End Sub

    Private Sub PlaniGrid_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        vaPuntClick = New Point(e.X, e.Y)  'X,Y coordenades de PlaniGrid
    End Sub

    Private Sub PlaniGrid_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
        vaToolTip1.Active = True
    End Sub

    Private Sub PlaniGrid_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
        vaToolTip1.Active = False
    End Sub

    Private Sub PlaniGrid_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        Static Xant As Integer
        Static Yant As Integer
        Dim idxrecurs As Integer

        If vpAmpleColumna <= 0 Then Exit Sub

        idxrecurs = ((e.X - GridPanel.Left - GridPanel.AutoScrollPosition.X) \ vpAmpleColumna)

        If e.X <> Xant Or e.Y <> Yant Then  'en coordenades de PlaniGrid

            If e.X < GridPanel.Left And e.Y > GridPanel.Top Then
                'sobre la capçalera de files
                Select Case VisualizationMode

                    Case PGMode.Day
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode setmana"))
                        vaToolTip1.Active = True

                    Case PGMode.Month
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode setmana"))
                        vaToolTip1.Active = True

                    Case PGMode.Week
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode mes"))
                        vaToolTip1.Active = True

                    Case PGMode.Resource
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode setmana"))
                        vaToolTip1.Active = True

                End Select

            ElseIf e.X > GridPanel.Left And e.Y < GridPanel.Top Then
                'sobre la capçalera de columnes

                Select Case VisualizationMode

                    Case PGMode.Day
                        If idxrecurs >= 0 And idxrecurs < vpLlistaRecursosDia.Count Then
                            vaToolTip1.SetToolTip(Me, T("DobleClick per mode recurs") & " " & IIf(vpLlistaRecursosDia.ElementAt(idxrecurs).Value.Name = "", Chr(216), "'" & vpLlistaRecursosDia.ElementAt(idxrecurs).Value.Name & "'"))
                            vaToolTip1.Active = True
                        Else
                            vaToolTip1.Active = False
                        End If

                    Case PGMode.Month
                        vaToolTip1.Active = False

                    Case PGMode.Week
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode dia"))
                        vaToolTip1.Active = True

                    Case PGMode.Resource
                        vaToolTip1.SetToolTip(Me, T("DobleClick per mode dia"))
                        vaToolTip1.Active = True

                End Select

            Else
                'en altres llocs
                vaToolTip1.Active = False
            End If

        End If

        Xant = e.X
        Yant = e.Y

    End Sub

    Private Sub PlaniGrid_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        If vaReformateja Then ReformatejaControl()

        'pintem columnes
        DibuixarCapçaleraCols(e.Graphics)

        'pintem files
        DibuixarCapçaleraFiles(e.Graphics)

        'dibuixem GridPanel
        GridPanel.Invalidate()

    End Sub

    Private Sub BotoMenys_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles BotoMenys.GotFocus
        GridPanel.Focus()
    End Sub

    Private Sub BotoMenys_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BotoMenys.MouseDown
        Dim auxPeriode As Integer

        Select Case VisualizationMode

            Case PGMode.Day
                auxPeriode = -1
                vaReformateja = True

            Case PGMode.Week
                auxPeriode = -7

            Case PGMode.Resource
                auxPeriode = -7

            Case PGMode.Month
                auxPeriode = DateDiff(DateInterval.Day, DisplayDate, DateAdd(DateInterval.Month, -1, DisplayDate))

        End Select

        DisplayDate = DateAdd(DateInterval.Day, auxPeriode, DisplayDate)

        Invalidate()
        GridPanel.Focus()
    End Sub

    Private Sub BotoMes_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles BotoMes.GotFocus
        GridPanel.Focus()
    End Sub

    Private Sub BotoMes_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BotoMes.MouseDown
        Dim auxPeriode As Integer

        Select Case VisualizationMode

            Case PGMode.Day
                auxPeriode = 1
                vaReformateja = True

            Case PGMode.Week
                auxPeriode = 7

            Case PGMode.Resource
                auxPeriode = 7

            Case PGMode.Month
                auxPeriode = DateDiff(DateInterval.Day, DisplayDate, DateAdd(DateInterval.Month, 1, DisplayDate))

        End Select

        DisplayDate = DateAdd(DateInterval.Day, auxPeriode, DisplayDate)

        Invalidate()
        GridPanel.Focus()

    End Sub

    Private Sub GridPanel_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles GridPanel.DragDrop
        Dim auxPGElement As PGElement
        Dim auxPGRecursos As Dictionary(Of String, Color)
        Dim auxStr As String
        Dim UpPoint As Point
        Dim minTimeSpan As TimeSpan = New TimeSpan(0, MinutesGap, 0)
        Dim auxPGRecurs As PGResource
        Dim auxReturn As PGReturnCode

        'X,Y coordenades de pantalla
        UpPoint = PointToClient(New Point(e.X - vaAjustBorder, e.Y - vaAjustBorder))

        'activem el drop dins la graella només si hem mogut el mouse des del click
        If UpPoint <> vaPuntClick Then

            'recuperem l'objecte del drag si es PGElement
            If vaElementDrop <> "" Then
                auxPGElement = CType(e.Data.GetData(GetType(PGElement)), PGElement)

                'si estem en mode dia, considerem el recurs del drop
                If VisualizationMode = PGMode.Day Then
                    auxPGRecurs = ObtenirRecursSeleccio(UpPoint)

                    If auxPGRecurs.Name = "" Then
                        If vaRecursSel = "" Then
                            'element de no recursos a no recursos
                            'no fer res
                        Else
                            'element de vaRecursSel a no recursos
                            'eliminem vaRecursSel del element
                            auxPGElement.DeleteResource(vaRecursSel)
                        End If
                    Else
                        If vaRecursSel = "" Then
                            'element de no recursos a auxPGRecurs
                            'afegim auxPGRecurs al element
                            auxPGElement.AddUpdateResource(auxPGRecurs)
                        Else
                            'element de vaRecursSel a auxPGRecurs
                            If e.Effect = DragDropEffects.Move Then
                                'substituim recursos de l'element
                                'eliminem vaRecursSel del element
                                auxPGElement.DeleteResource(vaRecursSel)
                                'afegim auxPGRecurs al element
                                auxPGElement.AddUpdateResource(auxPGRecurs)
                            Else
                                'afegim recursos a l'element
                                'afegim auxPGRecurs al element
                                auxPGElement.AddUpdateResource(auxPGRecurs)
                            End If
                        End If
                    End If
                End If

                'si estem en mode recurs 
                If VisualizationMode = PGMode.Resource Then
                    'si no estem en recurs sense assignar, assegurem que l'element té el recurs
                    If vaRecursSel <> "" Then
                        auxPGRecurs = New PGResource(vaRecursSel, vaColorSel)
                        auxPGElement.AddUpdateResource(auxPGRecurs)
                    Else
                        'si estem en recurs sense assignar
                        'si l'element te recursos anem a mode del primer recurs
                        If auxPGElement.Resources.Count > 0 Then
                            vaRecursSel = auxPGElement.Resources.Keys().First
                            vaColorSel = auxPGElement.Resources.Values().First
                        End If
                    End If
                End If

                'per qualsevol modo, fixem l'inici de l'element
                auxPGElement.Starts = DePosicioADataHora(UpPoint, SnapToGrid)
                'si estem en mode mes, ajustem l'hora a inici activitat
                If VisualizationMode = PGMode.Month Then
                    auxPGElement.Starts = auxPGElement.Starts.Date + prHoraIniciActivitat
                End If

                auxReturn = PGGridElementAddUpdate(auxPGElement)
                If auxReturn <> PGReturnCode.PGError Then
                    If auxReturn = PGReturnCode.PGAdded Then
                        RaiseEvent PGElement_Added(auxPGElement)
                    Else
                        RaiseEvent PGElement_Updated(auxPGElement)
                    End If
                    vaElementSel = auxPGElement.Id
                    DisplayDate = auxPGElement.Starts.Date
                Else
                    'RaiseEvent PGMessage(T("ERROR"), T("Error al actualitzar '") & auxPGElement.Name & "' (#" & auxPGElement.Id & ")")
                    GoTo sortir
                End If
            End If

            'recuperem l'objecte del drag si es Recursos
            If vaRecursosDrop <> "" Then
                auxPGRecursos = CType(e.Data.GetData(GetType(Dictionary(Of String, Color))), Dictionary(Of String, Color))

                If VisualizationMode = PGMode.Day Then
                    auxPGRecurs = ObtenirRecursSeleccio(UpPoint)
                    auxStr = ObtenirSeleccioPerRecurs(UpPoint, auxPGRecurs.Name)
                Else
                    auxStr = ObtenirSeleccio(UpPoint)
                End If

                If auxStr <> "" Then
                    'creem un nou element per no afectar al de la llista si no passa les validacions
                    auxPGElement = New PGElement(vpLlistaElements.Item(auxStr))
                    If e.Effect = DragDropEffects.Move Then
                        'substituim recursos de l'element
                        auxPGElement.Resources = auxPGRecursos
                    Else
                        'afegim recursos a l'element
                        For Each r In auxPGRecursos
                            auxPGElement.AddUpdateResource(New PGResource(r.Key, r.Value))
                        Next
                    End If

                    auxReturn = PGGridElementAddUpdate(auxPGElement)
                    If auxReturn <> PGReturnCode.PGError Then
                        If auxReturn = PGReturnCode.PGAdded Then
                            RaiseEvent PGElement_Added(auxPGElement)
                        Else
                            RaiseEvent PGElement_Updated(auxPGElement)
                        End If
                        vaElementSel = auxPGElement.Id
                    Else
                        'RaiseEvent PGMessage(T("ERROR"), T("Error al actualitzar '") & auxPGElement.Name & "' (#" & auxPGElement.Id & ")")
                        GoTo sortir
                    End If

                End If
            End If
        End If

sortir:
        vaElementDrop = ""
        vaRecursosDrop = ""

        If VisualizationMode = PGMode.Day Then vaReformateja = True
        Invalidate()
        Update()

        vaToolTip1.Active = True
        GridPanel.Focus()

    End Sub

    Private Sub GridPanel_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles GridPanel.DragEnter
        Dim auxPGElement As PGElement
        Dim auxPGRecursos As Dictionary(Of String, Color)
        Dim auxData As DateTime
        Dim auxStr As String
        Dim auxRecurs As PGResource
        Dim auxPunt As Point

        vaPuntDragAnt.X = e.X   'X,Y coordenades pantalla
        vaPuntDragAnt.Y = e.Y

        vaElementDrop = ""
        vaRecursosDrop = ""

        auxPunt = PointToClient(New Point(e.X - vaAjustBorder, e.Y - vaAjustBorder))

        'obtenim la data o data/hora sobre la que estem
        auxData = DePosicioADataHora(auxPunt, SnapToGrid)

        'si tenim un PGElement pel drop 
        If (e.Data.GetDataPresent(GetType(PGElement))) Then
            auxPGElement = CType(e.Data.GetData(GetType(PGElement)), PGElement)

            'validem que l'element del drag tingui nom 
            If auxPGElement.Name <> "" Then
                vaElementDrop = auxPGElement.Name

                'si estem sobre una data del mes actual, mostrem o no el permís de drop
                If auxData.Month = DisplayDate.Month Or VisualizationMode <> PGMode.Month Then
                    If ((e.KeyState And 4) = 4 And (e.AllowedEffect And DragDropEffects.Move) = DragDropEffects.Move) Then
                        e.Effect = DragDropEffects.Move
                    Else
                        e.Effect = DragDropEffects.Copy
                    End If
                Else
                    e.Effect = DragDropEffects.None
                End If

                'guardem posicio scroll pel dragover
                vaScrollPos.X = -GridPanel.AutoScrollPosition.X
                vaScrollPos.Y = -GridPanel.AutoScrollPosition.Y
            End If
        Else
            'si tenim un PGRecursos pel drop i no estem en mode mes
            If (e.Data.GetDataPresent(GetType(Dictionary(Of String, Color)))) And VisualizationMode <> PGMode.Month Then
                auxPGRecursos = CType(e.Data.GetData(GetType(Dictionary(Of String, Color))), Dictionary(Of String, Color))

                'validem que l'element del drag tingui recursos
                If auxPGRecursos.Count > 0 Then
                    vaRecursosDrop = auxPGRecursos.Count.ToString

                    'si estem sobre un element, mostrem o no el permís de drop
                    If VisualizationMode = PGMode.Day Then
                        'obtenim el recurs sobre el que estem (mode dia)
                        auxRecurs = ObtenirRecursSeleccio(auxPunt)
                        auxStr = ObtenirSeleccioPerRecurs(auxPunt, auxRecurs.Name)
                    Else
                        auxStr = ObtenirSeleccio(auxPunt)
                    End If
                    If auxStr <> "" Then
                        If ((e.KeyState And 4) = 4 And (e.AllowedEffect And DragDropEffects.Move) = DragDropEffects.Move) Then
                            e.Effect = DragDropEffects.Move
                        Else
                            e.Effect = DragDropEffects.Copy
                        End If
                    Else
                        e.Effect = DragDropEffects.None
                    End If

                    'guardem posicio scroll pel dragover
                    vaScrollPos.X = -GridPanel.AutoScrollPosition.X
                    vaScrollPos.Y = -GridPanel.AutoScrollPosition.Y
                End If
            End If
        End If

    End Sub

    Private Sub GridPanel_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPanel.DragLeave
        vaPuntDragAnt.X = 0
        vaPuntDragAnt.Y = 0
        vaElementDrop = ""
        vaRecursosDrop = ""
    End Sub

    Private Sub GridPanel_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles GridPanel.DragOver
        Dim auxCoord As Point
        Dim auxPos As Point
        Dim auxInt As Integer
        Dim auxData As DateTime
        Dim auxStr As String
        Dim auxRecurs As PGResource
        Dim auxPunt As Point

        'nomes fem autoscroll si es pot fer drop
        If vaElementDrop = "" And vaRecursosDrop = "" Then Exit Sub

        auxPunt = PointToClient(New Point(e.X - vaAjustBorder, e.Y - vaAjustBorder))

        'obtenim la data o data/hora sobre la que estem
        auxData = DePosicioADataHora(auxPunt, SnapToGrid)

        'efecte segons si estem sobre graella (zona de drop)
        auxCoord = DePosicioACoordenades(auxPunt)

        If vaElementDrop <> "" Then
            If auxCoord.X > 0 And auxCoord.Y > 0 And (auxData.Month = DisplayDate.Month Or VisualizationMode <> PGMode.Month) Then

                If VisualizationMode = PGMode.Day Then
                    If ((e.KeyState And 4) = 4 And (e.AllowedEffect And DragDropEffects.Move) = DragDropEffects.Move) Then
                        e.Effect = DragDropEffects.Move
                    Else
                        e.Effect = DragDropEffects.Copy
                    End If
                    'obtenim el recurs sobre el que estem (mode dia)
                    auxRecurs = ObtenirRecursSeleccio(auxPunt)
                    If auxRecurs.Name = "" Then
                        auxStr = IIf(vaRecursSel = "", "", " " & T("de") & " '" & vaRecursSel & "'") & " " & T("a") & " " & Chr(216)
                    Else
                        auxStr = IIf(vaRecursSel = "", "", " " & T("de") & " '" & vaRecursSel & "'") & " " & T("a") & " '" & auxRecurs.Name & "'"
                    End If
                Else
                    e.Effect = DragDropEffects.All
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        End If

        If vaRecursosDrop <> "" Then
            If VisualizationMode = PGMode.Day Then
                'obtenim el recurs sobre el que estem (mode dia)
                auxRecurs = ObtenirRecursSeleccio(auxPunt)
                auxStr = ObtenirSeleccioPerRecurs(auxPunt, auxRecurs.Name)
            Else
                auxStr = ObtenirSeleccio(auxPunt)
            End If

            If auxCoord.X > 0 And auxCoord.Y > 0 And VisualizationMode <> PGMode.Month And auxStr <> "" Then
                If ((e.KeyState And 4) = 4 And (e.AllowedEffect And DragDropEffects.Move) = DragDropEffects.Move) Then
                    e.Effect = DragDropEffects.Move
                Else
                    e.Effect = DragDropEffects.Copy
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        End If

        'CONTROLEM EL SCROLL DEL CONTROL

        'posicio absoluta en la pantalla
        auxPos.X = e.X - vaAjustBorder
        auxPos.Y = e.Y - vaAjustBorder

        'passem a coordenades del control
        auxPos = PointToClient(auxPos)

        auxInt = auxPos.X - Width + SystemInformation.VerticalScrollBarWidth

        'scroll a la dreta nomes si venim de l'esquerra
        If e.X > vaPuntDragAnt.X And auxInt > 0 Then
            vaScrollPos.X = vaScrollPos.X + (auxInt * 3)
            If vaScrollPos.X > (vpNroColumnes * vpAmpleColumna + GridPanel.Left) - Width + SystemInformation.VerticalScrollBarWidth Then
                vaScrollPos.X = (vpNroColumnes * vpAmpleColumna + GridPanel.Left) - Width + SystemInformation.VerticalScrollBarWidth
            End If
            GridPanel.AutoScrollPosition = vaScrollPos
            Invalidate()
            Exit Sub
        End If

        auxInt = auxPos.X - (GridPanel.Left + SystemInformation.VerticalScrollBarWidth)
        'scroll a l'esquerra nomes si venim de la dreta
        If e.X < vaPuntDragAnt.X And auxInt < 0 Then
            vaScrollPos.X = vaScrollPos.X + auxInt
            If vaScrollPos.X < 0 Then vaScrollPos.X = 0
            GridPanel.AutoScrollPosition = vaScrollPos
            Invalidate()
            Exit Sub
        End If

        'scroll avall nomes si venim de dalt
        auxInt = auxPos.Y - Height + SystemInformation.HorizontalScrollBarHeight
        If e.Y > vaPuntDragAnt.Y And auxInt > 0 Then
            vaScrollPos.Y = vaScrollPos.Y + (auxInt * 3)
            If vaScrollPos.Y > (vpNroFiles * IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) + GridPanel.Top) - Height + SystemInformation.HorizontalScrollBarHeight Then
                vaScrollPos.Y = (vpNroFiles * IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) + GridPanel.Top) - Height + SystemInformation.HorizontalScrollBarHeight
            End If
            GridPanel.AutoScrollPosition = vaScrollPos
            Invalidate()
            Exit Sub
        End If

        'scroll a dalt nomes si venim de baix
        auxInt = auxPos.Y - (GridPanel.Top + SystemInformation.VerticalScrollBarWidth)
        If e.Y < vaPuntDragAnt.Y And auxInt < 0 Then
            vaScrollPos.Y = vaScrollPos.Y + auxInt
            If vaScrollPos.Y < 0 Then vaScrollPos.Y = 0
            GridPanel.AutoScrollPosition = vaScrollPos
            Invalidate()
            Exit Sub
        End If

        vaPuntDragAnt.X = e.X
        vaPuntDragAnt.Y = e.Y

    End Sub

    Private Sub GridPanel_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles GridPanel.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            PlaniGridDragCursor.gEffect = gCursorLib.gCursor.eEffect.Copy
        Else
            PlaniGridDragCursor.gEffect = gCursorLib.gCursor.eEffect.Move
        End If
        Cursor.Current = PlaniGridDragCursor.gCursor
    End Sub

    Private Sub GridPanel_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPanel.LostFocus
        If VisualizationMode <> PGMode.Resource Then
            vaRecursSel = ""
            vaColorSel = Color.Empty
        End If
    End Sub

    Private Sub GridPanel_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridPanel.MouseDoubleClick
        Dim auxData As DateTime

        Select Case VisualizationMode

            Case PGMode.Day
                If Not vpLlistaElements.ContainsKey(vaElementSel) Then vaElementSel = ""
                If vaElementSel <> "" Then
                    RaiseEvent PGElement_DoubleClicked(vpLlistaElements(vaElementSel))
                Else
                    VisualizationMode = PGMode.Resource
                    vaReformateja = True
                    GridPanel.AutoScrollPosition = New Point(0, 0)
                    Invalidate()
                End If

            Case PGMode.Week
                If Not vpLlistaElements.ContainsKey(vaElementSel) Then vaElementSel = ""
                If vaElementSel <> "" Then
                    RaiseEvent PGElement_DoubleClicked(vpLlistaElements(vaElementSel))
                Else
                    auxData = DePosicioADataHora(vaPuntClick, False)
                    If auxData.Date <> DateTime.MinValue Then
                        DisplayDate = auxData.Date
                        VisualizationMode = PGMode.Day
                        GridPanel.AutoScrollPosition = New Point(0, 0)
                        Invalidate()
                    End If
                End If

            Case PGMode.Month
                auxData = DePosicioADataHora(vaPuntClick, False)
                If auxData.Date <> DateTime.MinValue Then
                    DisplayDate = auxData.Date
                    VisualizationMode = PGMode.Day
                    GridPanel.AutoScrollPosition = New Point(0, 0)
                    Invalidate()
                End If

            Case PGMode.Resource
                If Not vpLlistaElements.ContainsKey(vaElementSel) Then vaElementSel = ""
                If vaElementSel <> "" Then
                    RaiseEvent PGElement_DoubleClicked(vpLlistaElements(vaElementSel))
                Else
                    auxData = DePosicioADataHora(vaPuntClick, False)
                    If auxData.Date <> DateTime.MinValue Then
                        DisplayDate = auxData.Date
                        VisualizationMode = PGMode.Day
                    Else
                        VisualizationMode = PGMode.Week
                    End If
                    GridPanel.AutoScrollPosition = New Point(0, 0)
                    Invalidate()
                End If

        End Select

        GridPanel.Focus()

    End Sub

    Private Sub GridPanel_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridPanel.MouseDown
        Dim auxstr As String
        Dim auxdate As Date
        Dim auxPGElement As PGElement

        vaPuntClick = New Point(e.X + GridPanel.Left, e.Y + GridPanel.Top) 'X,Y coordenades de GridPanel

        Select Case VisualizationMode

            Case PGMode.Month
                vaRecursSel = ""
                vaColorSel = Color.Empty
                vaElementSel = ""
                RaiseEvent PGElements_Unselected()
                auxstr = ObtenirSeleccio(vaPuntClick)
                If IsDate(auxstr) Then
                    auxdate = CDate(auxstr)
                    If auxdate.Month = DisplayDate.Month Then
                        DisplayDate = auxdate.Date
                        GridPanel.Invalidate()
                        GridPanel.Update()
                    End If
                End If

            Case PGMode.Day
                'obtenim el recurs/element sobre el que estem (mode dia)
                vaRecursSel = ObtenirRecursSeleccio(vaPuntClick).Name
                vaColorSel = ObtenirRecursSeleccio(vaPuntClick).Color
                vaElementSel = ObtenirSeleccioPerRecurs(vaPuntClick, vaRecursSel)
                'el ressaltem
                GridPanel.Invalidate()
                GridPanel.Update()
                If vaElementSel <> "" Then
                    auxPGElement = New PGElement(vpLlistaElements(vaElementSel))
                    RaiseEvent PGElement_Clicked(auxPGElement)
                Else
                    RaiseEvent PGElements_Unselected()
                End If

            Case PGMode.Week
                'obtenim el element sobre el que estem 
                vaRecursSel = ""
                vaColorSel = Color.Empty
                vaElementSel = ObtenirSeleccio(vaPuntClick)
                'el ressaltem
                GridPanel.Invalidate()
                GridPanel.Update()
                If vaElementSel <> "" Then
                    auxPGElement = New PGElement(vpLlistaElements(vaElementSel))
                    RaiseEvent PGElement_Clicked(auxPGElement)
                Else
                    RaiseEvent PGElements_Unselected()
                End If

            Case PGMode.Resource
                'obtenim el element sobre el que estem 
                vaElementSel = ObtenirSeleccioPerRecurs(vaPuntClick, vaRecursSel)
                'el ressaltem
                GridPanel.Invalidate()
                GridPanel.Update()
                If vaElementSel <> "" Then
                    auxPGElement = New PGElement(vpLlistaElements(vaElementSel))
                    RaiseEvent PGElement_Clicked(auxPGElement)
                Else
                    RaiseEvent PGElements_Unselected()
                End If

        End Select

        GridPanel.Focus()

    End Sub

    Private Sub GridPanel_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPanel.MouseEnter
        vaToolTip1.Active = True
    End Sub

    Private Sub GridPanel_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPanel.MouseLeave
        vaToolTip1.Active = False
        vaDataMouse = Date.MinValue
        GridPanel.Invalidate()
    End Sub

    Private Sub GridPanel_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridPanel.MouseMove
        Static Xant As Integer
        Static Yant As Integer
        Dim auxstr As String
        Dim auxdata As Date
        Dim auxint As Integer
        Dim auxPGElement As PGElement
        Dim returnValue As System.Windows.Forms.DragDropEffects
        Dim auxPoint As New Point(e.X + GridPanel.Left, e.Y + GridPanel.Top)

        If e.Button = Windows.Forms.MouseButtons.Left Then
            'estem arrossegant
            If Not vpLlistaElements.ContainsKey(vaElementSel) Then vaElementSel = ""
            'si tenim element seleccionat i no l'estem encara arrossegant
            If vaElementSel <> "" And vaElementDrop = "" Then
                auxPGElement = New PGElement(vpLlistaElements(vaElementSel))

                'montem el cursor del drop
                PlaniGridDragCursor.gText = auxPGElement.Name
                PlaniGridDragCursor.gFont = Me.Font
                PlaniGridDragCursor.gTextBoxColor = IIf(auxPGElement.Resources.Count > 0, PGResourcesColor(auxPGElement.Resources), GridPanel.BackColor)
                PlaniGridDragCursor.gTBTransp = 33
                PlaniGridDragCursor.gTextColor = ContrastedColor(PlaniGridDragCursor.gTextBoxColor)
                auxint = Math.Max(IIf(vpAltFila > 17, vpAltFila - 7, 10), (vpAltFila * (PGElementGetDuration(auxPGElement).TotalMinutes / MinutesGap)) - 7)
                PlaniGridDragCursor.gTextBox = New Size(vpAmpleColumna - 7, auxint)
                PlaniGridDragCursor.MakeCursor()

                If VisualizationMode = PGMode.Day Then
                    returnValue = GridPanel.DoDragDrop(auxPGElement, DragDropEffects.Copy Or DragDropEffects.Move)
                Else
                    returnValue = GridPanel.DoDragDrop(auxPGElement, DragDropEffects.Move)
                End If
            End If

        Else
            'no estem arrossegant
            'si ha canviat la posició

            vaPuntClick = Point.Empty   'netejem el punt del click

            If e.X <> Xant Or e.Y <> Yant Then  'en coordenades de GridPanel
                auxdata = DePosicioADataHora(auxPoint, False)
                If VisualizationMode = PGMode.Month Then
                    auxstr = auxdata.Day & " " & T(MesosCurts(auxdata.Month - 1))
                Else
                    If VisualizationMode = PGMode.Day Then
                        auxstr = ObtenirSeleccioPerRecurs(auxPoint, ObtenirRecursSeleccio(auxPoint).Name)
                    Else
                        auxstr = ObtenirSeleccio(New Point(e.X + GridPanel.Left, e.Y + GridPanel.Top))
                    End If
                    If auxstr <> "" Then
                        auxstr = "#" & auxstr & " " & vpLlistaElements(auxstr).Name & " (" & vpLlistaElements(auxstr).Starts.ToString("HH:mm") & " " & vpLlistaElements(auxstr).Ends.ToString("HH:mm") & ")"
                    Else
                        auxstr = auxdata.ToString("HH:mm")
                    End If
                End If

                vaToolTip1.SetToolTip(Me.GridPanel, auxstr)
                vaToolTip1.Active = True
            End If

            'si estem en mode mes mostrem els elements de la data on som
            If VisualizationMode = PGMode.Month Then
                auxdata = DePosicioADataHora(auxPoint, False).Date
                If auxdata <> vaDataMouse Then
                    vaDataMouse = auxdata
                    GridPanel.Invalidate()
                End If
            Else
                vaDataMouse = Date.MinValue
            End If

        End If

        Xant = e.X
        Yant = e.Y

    End Sub

    Private Sub GridPanel_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridPanel.MouseWheel
        Invalidate()
    End Sub

    Private Sub GridPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles GridPanel.Paint
        Dim llapis As System.Drawing.Pen
        Dim i, pos As Integer
        Dim auxDataDia1 As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)
        Dim auxData As Date
        Dim auxHoraIni As DateTime
        Dim auxHoraFi As DateTime
        Dim brochaInact As System.Drawing.Brush
        Dim brochaCapDe As System.Drawing.Brush
        Dim auxData2 As Date
        Dim auxData3 As Date
        Dim auxElement As PGElement

        GridPanel.SuspendLayout()

        'DIBUIXAR COLS
        'creem les eines de dibuix
        llapis = New System.Drawing.Pen(VLinesColor)
        brochaInact = New System.Drawing.SolidBrush(ColorCompost(VLinesColor, HLinesColor))
        brochaCapDe = New System.Drawing.SolidBrush(HLinesColor)

        For i = 1 To vpNroColumnes
            pos = vpAmpleColumna * (i - 1) + GridPanel.AutoScrollPosition.X '- 1
            If pos >= GridPanel.Width Then Exit For

            'pintem inactius els dies posteriors a l'ultim dia laborable de la setmana (0 diumenge, 6 dissabte)
            If VisualizationMode <> PGMode.Day And pos + vpAmpleColumna >= 0 And pos <= GridPanel.Width Then
                auxData = DePosicioADataHora(New Point(GridPanel.Left + pos + IIf(pos < 0, vpAmpleColumna - 1, 0) + 1, GridPanel.Top), False)
                If IIf(auxData.DayOfWeek = DayOfWeek.Sunday, 6, auxData.DayOfWeek - 1) > IIf(LastWorkingDayOfWeek = DayOfWeek.Sunday, 6, LastWorkingDayOfWeek - 1) Then
                    e.Graphics.FillRectangle(brochaCapDe, New Rectangle(pos, 0, vpAmpleColumna, GridPanel.Height))
                End If
            End If

            If pos > 0 Then e.Graphics.DrawLine(llapis, pos, 0, pos, GridPanel.Height)
        Next

        'DIBUIXAR FILES
        'creem les eines de dibuix
        llapis = New System.Drawing.Pen(HLinesColor)

        For i = 1 To vpNroFiles

            pos = IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) * i + GridPanel.AutoScrollPosition.Y '- 1

            If pos > GridPanel.Height Then Exit For

            e.Graphics.DrawLine(llapis, 0, pos, GridPanel.Width, pos)

            'pintem sombrejades les dates dels mesos anterior i posterior al actual
            If VisualizationMode = PGMode.Month Then
                'obtenim la data del primer dia de cada setmana 
                auxData = DateAdd(DateInterval.Day, 7 * (i - 1) + IIf(DayOfWeek.Monday - auxDataDia1.DayOfWeek > 0, DayOfWeek.Monday - auxDataDia1.DayOfWeek - 7, DayOfWeek.Monday - auxDataDia1.DayOfWeek), auxDataDia1)
                If auxData.Year < auxDataDia1.Year Or (auxData.Year = auxDataDia1.Year And auxData.Month < auxDataDia1.Month) Then
                    'dates del mes anterior
                    e.Graphics.FillRectangle(brochaInact, New Rectangle(0, vpAltFilaMes * (i - 1), vpAmpleColumna * DateDiff(DateInterval.Day, auxData, auxDataDia1) + GridPanel.AutoScrollPosition.X, vpAltFilaMes))
                Else
                    If auxData.Year > auxDataDia1.Year Or (auxData.Year = auxDataDia1.Year And auxData.Month > auxDataDia1.Month) Then
                        'dates del mes posterior (tota la setmana)
                        e.Graphics.FillRectangle(brochaInact, New Rectangle(0, vpAltFilaMes * (i - 1) + 1, vpAmpleColumna * vpNroColumnes, vpAltFilaMes))
                    Else
                        'dates del mes posterior (part de la setmana)
                        auxData2 = DateAdd(DateInterval.Day, 6, auxData)
                        If auxData2.Year > auxDataDia1.Year Or (auxData2.Year = auxDataDia1.Year And auxData2.Month > auxDataDia1.Month) Then
                            auxData3 = DateSerial(auxData2.Year, auxData2.Month, 1)
                            e.Graphics.FillRectangle(brochaInact, New Rectangle(DeDataHoraAPosicio(auxData3, True).X, vpAltFilaMes * (i - 1) + 1, vpAmpleColumna * (DateDiff(DateInterval.Day, auxData3, auxData2) + 1), vpAltFilaMes))
                        End If
                    End If
                End If
            Else
                'mode dia o setmana o recurs, marquem l'horari de dinar
                'obtenim l'hora inicial i final de la fila
                auxHoraIni = DePosicioADataHora(New Point(GridPanel.Left, GridPanel.Top + pos - vpAltFila + 1), False)
                auxHoraFi = DePosicioADataHora(New Point(GridPanel.Left, GridPanel.Top + pos), False)
                If auxHoraIni >= auxHoraIni.Date + prHoraIniciDinar And auxHoraFi <= auxHoraIni.Date + prHoraFiDinar Then
                    For j = 0 To vpNroColumnes - 1
                        e.Graphics.FillRectangle(brochaCapDe, New Rectangle(j * vpAmpleColumna + GridPanel.AutoScrollPosition.X, pos - vpAltFila + 1, vpAmpleColumna, vpAltFila - 1))
                    Next
                End If
            End If

        Next

        'DIBUIXAR ELEMENTS

        'inicialitzem la estructura per controlar quants elements hi ha abans en una mateixa cel·la del mes
        vaNElementsCella = New Dictionary(Of Point, ElementsCella)

        'dibuixem elements que intersectin el periode visualitzat (ordenats per datahorainici)
        For Each element In vpLlistaElementsData
            auxElement = New PGElement(vpLlistaElements.Item(element.Value))
            If auxElement.Ends > vaDataHoraIniPer And auxElement.Starts <= vaDataHoraFiPer Then
                DibuixarElement(e.Graphics, auxElement)
            End If
            'parem quan passem del periode de visualitzacio
            If auxElement.Starts > vaDataHoraFiPer Then
                Exit For
            End If
        Next

        'al final 
        'redibuixem l'element seleccionat (per si l'ha tapat un altre element)
        If VisualizationMode <> PGMode.Month And vpLlistaElements.ContainsKey(vaElementSel) Then
            DibuixarElement(e.Graphics, vpLlistaElements.Item(vaElementSel))
        End If
        If VisualizationMode = PGMode.Month Then
            If DisplayDate <> Date.MinValue Then DibuixarElementsDia(e.Graphics, DisplayDate)
            'redibuixem l'element sobre el que tenim el cursor en mode mes
            If vaDataMouse <> Date.MinValue And vaDataMouse <> DisplayDate Then DibuixarElementsDia(e.Graphics, vaDataMouse)
        End If

        vaNElementsCella.Clear()
        GridPanel.ResumeLayout()

    End Sub

    Private Sub GridPanel_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles GridPanel.PreviewKeyDown
        Dim auxdate As Date = Date.MinValue
        Dim auxPGElement As PGElement
        Dim auxTimeSpanV As TimeSpan
        Dim auxTimeSpanH As TimeSpan
        Dim DateTimeElem As Date
        Dim auxDateTime As Date
        Dim auxReturn As PGReturnCode

        'si mode mes 
        If VisualizationMode = PGMode.Month Then

            'actualitzem la data seleccionada
            Select Case e.KeyValue
                Case Keys.Left '37 'left
                    auxdate = DateAdd(DateInterval.Day, -1, DisplayDate)

                Case Keys.Up '38 'up
                    auxdate = DateAdd(DateInterval.Day, -7, DisplayDate)

                Case Keys.Right '39 'right
                    auxdate = DateAdd(DateInterval.Day, 1, DisplayDate)

                Case Keys.Down '40 'down
                    auxdate = DateAdd(DateInterval.Day, 7, DisplayDate)

                Case Else
                    auxdate = DisplayDate
            End Select

            If DisplayDate <> auxdate Then
                DisplayDate = auxdate
                Invalidate()
            End If

            vaDataMouse = Date.MinValue

        Else
            'mode setmana o dia o recurs
            If Not vpLlistaElements.ContainsKey(vaElementSel) Then vaElementSel = ""
            'si element seleccionat
            If vaElementSel <> "" Then
                'el movem per la grid
                auxPGElement = New PGElement(vpLlistaElements.Item(vaElementSel))

                If SnapToGrid And Not e.Shift Then
                    auxTimeSpanV = New TimeSpan(0, MinutesGap, 0)
                Else
                    auxTimeSpanV = New TimeSpan(0, 1, 0)
                End If
                auxTimeSpanH = New TimeSpan(24, 0, 0)

                Select Case e.KeyValue
                    Case Keys.Left '37 'left
                        If VisualizationMode = PGMode.Week Or VisualizationMode = PGMode.Resource Then
                            auxPGElement.Starts -= auxTimeSpanH
                            auxReturn = PGGridElementAddUpdate(auxPGElement)
                            If auxReturn <> PGReturnCode.PGError Then
                                RaiseEvent PGElement_Updated(auxPGElement)
                                DisplayDate = auxPGElement.Starts.Date
                                Invalidate()
                                Update()
                                vaToolTip1.SetToolTip(Me.GridPanel, auxPGElement.Name & " (" & auxPGElement.Starts.ToString("HH:mm") & " " & auxPGElement.Ends.ToString("HH:mm") & ")")
                                vaToolTip1.Active = True
                            End If
                        End If

                    Case Keys.Up '38 'up
                        If e.Control Then
                            'aliniem element amb el final de l'anterior o inici activitat
                            auxDateTime = auxPGElement.Starts.Date + prHoraIniciActivitat
                            For Each element In vpLlistaElementsData
                                DateTimeElem = vpLlistaElements(element.Value).Ends
                                If DateTimeElem > auxDateTime And DateTimeElem < auxPGElement.Starts Then
                                    auxDateTime = DateTimeElem
                                End If
                                If vpLlistaElements(element.Value).Starts > auxPGElement.Starts Then Exit For
                            Next
                            auxPGElement.Starts = auxDateTime
                        Else
                            auxPGElement.Starts -= auxTimeSpanV
                        End If

                        auxReturn = PGGridElementAddUpdate(auxPGElement)
                        If auxReturn <> PGReturnCode.PGError Then
                            RaiseEvent PGElement_Updated(auxPGElement)
                            DisplayDate = auxPGElement.Starts.Date
                            vaReformateja = True
                            Invalidate()
                            Update()

                            vaToolTip1.SetToolTip(Me.GridPanel, auxPGElement.Name & " (" & auxPGElement.Starts.ToString("HH:mm") & " " & auxPGElement.Ends.ToString("HH:mm") & ")")
                            vaToolTip1.Active = True
                        End If

                    Case Keys.Right '39 'right
                        If VisualizationMode = PGMode.Week Or VisualizationMode = PGMode.Resource Then
                            auxPGElement.Starts += auxTimeSpanH
                            auxReturn = PGGridElementAddUpdate(auxPGElement)
                            If auxReturn <> PGReturnCode.PGError Then
                                RaiseEvent PGElement_Updated(auxPGElement)
                                DisplayDate = auxPGElement.Starts.Date
                                Invalidate()
                                Update()

                                vaToolTip1.SetToolTip(Me.GridPanel, auxPGElement.Name & " (" & auxPGElement.Starts.ToString("HH:mm") & " " & auxPGElement.Ends.ToString("HH:mm") & ")")
                                vaToolTip1.Active = True
                            End If
                        End If

                    Case Keys.Down '40 'down
                        If e.Control Then
                            'aliniem el final de element amb l'inici del posterior o fi activitat
                            auxDateTime = auxPGElement.Starts.Date + prHoraFiActivitat
                            For Each element In vpLlistaElementsData
                                DateTimeElem = vpLlistaElements(element.Value).Starts
                                If DateTimeElem > (auxPGElement.Starts + PGElementGetDuration(auxPGElement)) And DateTimeElem < auxDateTime Then
                                    auxDateTime = DateTimeElem
                                    Exit For
                                End If
                                If DateTimeElem > auxDateTime Then Exit For
                            Next
                            auxPGElement.Starts = auxDateTime - PGElementGetDuration(auxPGElement)
                        Else
                            auxPGElement.Starts += auxTimeSpanV
                        End If

                        auxReturn = PGGridElementAddUpdate(auxPGElement)
                        If auxReturn <> PGReturnCode.PGError Then
                            RaiseEvent PGElement_Updated(auxPGElement)
                            DisplayDate = auxPGElement.Starts.Date
                            vaReformateja = True
                            Invalidate()
                            Update()

                            vaToolTip1.SetToolTip(Me.GridPanel, auxPGElement.Name & " (" & auxPGElement.Starts.ToString("HH:mm") & " " & auxPGElement.Ends.ToString("HH:mm") & ")")
                            vaToolTip1.Active = True
                        End If

                    Case Keys.Delete '46 'delete
                        If MsgBox(T("Eliminar '") & auxPGElement.Name & "' (#" & vaElementSel & ") ?", MsgBoxStyle.YesNo, T("CONFIRMACIÓ")) = MsgBoxResult.Yes Then
                            If vpLlistaElements.ContainsKey(auxPGElement.Id) Then
                                auxPGElement = vpLlistaElements(auxPGElement.Id)
                                vpLlistaElements.Remove(auxPGElement.Id)
                                If vpLlistaElementsData.ContainsValue(auxPGElement.Id) Then
                                    vpLlistaElementsData.RemoveAt(vpLlistaElementsData.IndexOfValue(auxPGElement.Id))
                                End If
                                vaElementSel = ""
                                If VisualizationMode = PGMode.Day Then
                                    vaReformateja = True
                                    Invalidate()
                                Else
                                    GridPanel.Invalidate()
                                End If
                                RaiseEvent PGElement_Deleted(auxPGElement)
                            Else
                                RaiseEvent PGMessage(T("ERROR"), T("Error al eliminar '") & auxPGElement.Name & "' (#" & vaElementSel & ")")
                            End If
                        End If

                    Case Else
                        RaiseEvent PGElement_KeyDown(auxPGElement, e)

                End Select

            End If

        End If

        GridPanel.Focus()

    End Sub

    Private Sub GridPanel_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridPanel.Resize
        vaReformateja = True
        Invalidate()
    End Sub

    Private Sub GridPanel_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles GridPanel.Scroll
        Invalidate()
        GridPanel.Focus()
        If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
            RaiseEvent PGHScroll(e.OldValue, e.NewValue)
        Else
            RaiseEvent PGVScroll(e.OldValue, e.NewValue)
        End If
    End Sub

#End Region

#Region "Subrutines i funcions"

    'per traduir textos a l'idioma seleccionat del control
    Function T(ByVal pTxt As String) As String
        Select Case Language
            Case PGIdioma.English
                If vpEN.ContainsKey(pTxt) Then
                    Return vpEN(pTxt)
                Else
                    Return pTxt
                End If

            Case PGIdioma.Castellano
                If vpES.ContainsKey(pTxt) Then
                    Return vpES(pTxt)
                Else
                    Return pTxt
                End If
            Case Else
                Return pTxt
        End Select

    End Function

    Private Function DePosicioACoordenades(ByVal pPunt As Point) As Point
        'converteix de punt coordenades de PlaniGrid a coordenades cel·la de GridPanel
        If pPunt.IsEmpty Then
            DePosicioACoordenades = New Point(0, 0)
        End If

        'coordenada X
        If pPunt.X < GridPanel.Left Then    'si estem en la 1a columna
            DePosicioACoordenades.X = 0
        Else
            If GridPanel.VerticalScroll.Visible And pPunt.X > Width - SystemInformation.VerticalScrollBarWidth Then
                DePosicioACoordenades.X = -1 'a la barra de scroll vertical
            Else
                'hi afegim el desplaçament del scroll
                pPunt.X -= GridPanel.AutoScrollPosition.X
                Try
                    DePosicioACoordenades.X = ((pPunt.X - GridPanel.Left) \ vpAmpleColumna) + 1
                Catch ex As Exception
                    DePosicioACoordenades.X = 0
                End Try
                If DePosicioACoordenades.X > vpNroColumnes Then DePosicioACoordenades.X = -1 'a la dreta de la grid
            End If
        End If

        'coordenada Y
        If pPunt.Y < GridPanel.Top Then      'si estem en la 1a fila
            DePosicioACoordenades.Y = 0
        Else
            If GridPanel.HorizontalScroll.Visible And pPunt.Y > Height - SystemInformation.HorizontalScrollBarHeight Then
                DePosicioACoordenades.Y = -1 'a la barra de scroll horitzontal
            Else
                'hi afegim el desplaçament del scroll 
                pPunt.Y -= GridPanel.AutoScrollPosition.Y
                Try
                    DePosicioACoordenades.Y = ((pPunt.Y - GridPanel.Top) \ IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila)) + 1
                Catch ex As Exception
                    DePosicioACoordenades.Y = 0
                End Try
                If DePosicioACoordenades.Y > vpNroFiles Then DePosicioACoordenades.Y = -1 'a sota de la grid
            End If
        End If

    End Function

    Private Function DeCoordenadesAPosicio(ByVal pCoords As Point) As Point
        'converteix de coordenades de cel·la de GridPanel a posició del punt superior esquerre en PlaniGrid
        DeCoordenadesAPosicio.X = IIf(pCoords.X >= 1, GridPanel.Left + (pCoords.X - 1) * vpAmpleColumna + GridPanel.AutoScrollPosition.X, 0)
        DeCoordenadesAPosicio.Y = IIf(pCoords.Y >= 1, GridPanel.Top + (pCoords.Y - 1) * IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) + GridPanel.AutoScrollPosition.Y, 0)
    End Function

    Private Function DePosicioADataHora(ByVal pPunt As Point, ByVal pSnapToGrid As Boolean) As DateTime
        'converteix del punt de coordenades de PlaniGrid a data hora
        Dim auxData As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)
        Dim auxMinuts, auxOffset As Integer

        If pPunt.IsEmpty Or vpNroFiles = 0 Then
            Return DateTime.MinValue
        End If

        Select Case VisualizationMode

            Case PGMode.Day
                auxMinuts = prHoraIniciActivitat.TotalMinutes + (pPunt.Y - GridPanel.Top - GridPanel.AutoScrollPosition.Y) / (vpNroFiles * vpAltFila) * DuradaJornada.TotalMinutes
                auxOffset = prHoraIniciActivitat.Minutes Mod MinutesGap
                If pSnapToGrid Then auxMinuts = auxOffset + ((auxMinuts - auxOffset) \ MinutesGap) * MinutesGap
                Return DateAdd(DateInterval.Minute, IIf(auxMinuts >= 0, auxMinuts, 0), DisplayDate)

            Case PGMode.Week
                auxMinuts = prHoraIniciActivitat.TotalMinutes + (pPunt.Y - GridPanel.Top - GridPanel.AutoScrollPosition.Y) / (vpNroFiles * vpAltFila) * DuradaJornada.TotalMinutes
                auxOffset = prHoraIniciActivitat.Minutes Mod MinutesGap
                If pSnapToGrid Then auxMinuts = auxOffset + ((auxMinuts - auxOffset) \ MinutesGap) * MinutesGap
                pPunt = DePosicioACoordenades(pPunt)
                If pPunt.X < 1 Then
                    Return DateAdd(DateInterval.Minute, IIf(auxMinuts >= 0, auxMinuts, 0), DateTime.MinValue)
                Else
                    Return DateAdd(DateInterval.Minute, IIf(auxMinuts >= 0, auxMinuts, 0), DateAdd(DateInterval.Day, (pPunt.X - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate))
                End If

            Case PGMode.Month
                pPunt = DePosicioACoordenades(pPunt)
                If pPunt.Y < 1 Then
                    Return DateTime.MinValue
                Else
                    If pPunt.X < 1 Then
                        Return DateAdd(DateInterval.Day, 7 * (pPunt.Y - 1) + IIf(DayOfWeek.Monday - auxData.DayOfWeek > 0, DayOfWeek.Monday - auxData.DayOfWeek - 7, DayOfWeek.Monday - auxData.DayOfWeek), auxData)
                    Else
                        Return DateAdd(DateInterval.Day, 7 * (pPunt.Y - 1) + (pPunt.X - 1) + IIf(DayOfWeek.Monday - auxData.DayOfWeek > 0, DayOfWeek.Monday - auxData.DayOfWeek - 7, DayOfWeek.Monday - auxData.DayOfWeek), auxData)
                    End If
                End If

            Case PGMode.Resource
                auxMinuts = prHoraIniciActivitat.TotalMinutes + (pPunt.Y - GridPanel.Top - GridPanel.AutoScrollPosition.Y) / (vpNroFiles * vpAltFila) * DuradaJornada.TotalMinutes
                auxOffset = prHoraIniciActivitat.Minutes Mod MinutesGap
                If pSnapToGrid Then auxMinuts = auxOffset + ((auxMinuts - auxOffset) \ MinutesGap) * MinutesGap
                pPunt = DePosicioACoordenades(pPunt)
                If pPunt.X < 1 Then
                    Return DateAdd(DateInterval.Minute, IIf(auxMinuts >= 0, auxMinuts, 0), DateTime.MinValue)
                Else
                    Return DateAdd(DateInterval.Minute, IIf(auxMinuts >= 0, auxMinuts, 0), DateAdd(DateInterval.Day, (pPunt.X - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate))
                End If

        End Select

    End Function

    Private Function DeDataHoraAPosicio(ByVal pDataHora As DateTime, ByVal pIniper As Boolean) As Point
        'converteix de data i hora a punt superior esquerre de la cel·la en coordenades de GridPanel 
        Dim auxData As DateTime
        Dim auxHora As TimeSpan
        Dim auxMinuts As Integer
        Dim auxDataIni, auxDataFi As DateTime
        Dim auxData2 As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)

        If pDataHora = DateTime.MinValue Then
            Return Point.Empty
        End If

        auxData = pDataHora.Date
        auxHora = pDataHora.TimeOfDay

        'si son les 00:00h.
        'si inici de periode --> 00:00h del dia
        'si final de periode --> 24:00h. del dia anterior
        If Not pIniper And auxHora = TimeSpan.Zero Then
            auxData = DateAdd(DateInterval.Day, -1, auxData)
            auxHora = New TimeSpan(24, 0, 0)
        End If


        Select Case VisualizationMode

            Case PGMode.Day
                DeDataHoraAPosicio.X = GridPanel.AutoScrollPosition.X '+ vaAjustBorder
                auxMinuts = auxHora.TotalMinutes - prHoraIniciActivitat.TotalMinutes
                DeDataHoraAPosicio.Y = GridPanel.AutoScrollPosition.Y + auxMinuts / DuradaJornada.TotalMinutes * vpAltFila * vpNroFiles '+ vaAjustBorder

            Case PGMode.Week
                auxDataIni = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                auxDataFi = DateAdd(DateInterval.Day, 6, auxDataIni)
                If auxData < auxDataIni Or auxData > auxDataFi Then
                    DeDataHoraAPosicio.X = 0
                Else
                    DeDataHoraAPosicio.X = GridPanel.AutoScrollPosition.X + vpAmpleColumna * DateDiff(DateInterval.Day, auxDataIni, auxData) '+ vaAjustBorder
                End If
                auxMinuts = auxHora.TotalMinutes - prHoraIniciActivitat.TotalMinutes
                DeDataHoraAPosicio.Y = GridPanel.AutoScrollPosition.Y + auxMinuts / DuradaJornada.TotalMinutes * vpAltFila * vpNroFiles '+ vaAjustBorder

            Case PGMode.Month
                auxDataIni = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - auxData2.DayOfWeek > 0, DayOfWeek.Monday - auxData2.DayOfWeek - 7, DayOfWeek.Monday - auxData2.DayOfWeek), auxData2)
                auxDataFi = DateAdd(DateInterval.Day, 41, auxDataIni)
                If auxData < auxDataIni Or auxData > auxDataFi Then
                    DeDataHoraAPosicio.X = 0
                    DeDataHoraAPosicio.Y = 0
                Else
                    DeDataHoraAPosicio.X = GridPanel.AutoScrollPosition.X + vpAmpleColumna * (DateDiff(DateInterval.Day, auxDataIni, auxData) Mod 7) '+ vaAjustBorder
                    DeDataHoraAPosicio.Y = GridPanel.AutoScrollPosition.Y + vpAltFilaMes * (DateDiff(DateInterval.Day, auxDataIni, auxData) \ 7) '+ vaAjustBorder
                End If

            Case PGMode.Resource
                auxDataIni = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                auxDataFi = DateAdd(DateInterval.Day, 6, auxDataIni)
                If auxData < auxDataIni Or auxData > auxDataFi Then
                    DeDataHoraAPosicio.X = 0
                Else
                    DeDataHoraAPosicio.X = GridPanel.AutoScrollPosition.X + vpAmpleColumna * DateDiff(DateInterval.Day, auxDataIni, auxData) '+ vaAjustBorder
                End If
                auxMinuts = auxHora.TotalMinutes - prHoraIniciActivitat.TotalMinutes
                DeDataHoraAPosicio.Y = GridPanel.AutoScrollPosition.Y + auxMinuts / DuradaJornada.TotalMinutes * vpAltFila * vpNroFiles '+ vaAjustBorder

        End Select

    End Function

    Private Function ObtenirSeleccio(ByVal pPunt As Point) As String
        Dim auxElement As PGElement
        Dim auxDataHora As DateTime = DePosicioADataHora(pPunt, False)

        ObtenirSeleccio = ""

        If VisualizationMode = PGMode.Month Then
            'obtenim la data seleccionada
            Return auxDataHora.Date.ToString
        Else
            'selecionem l'ultim element ordenat per dataini i datafi descendent que compleixi les condicions
            For Each element In vpLlistaElementsData
                auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                If auxElement.Starts <= auxDataHora And auxElement.Ends >= auxDataHora Then
                    'si estem en mode recurs, ademés l'element ha de tenir el recurs assignat
                    If VisualizationMode = PGMode.Resource Then
                        If vaRecursSel = "" Then
                            If auxElement.Resources.Count = 0 Then
                                ObtenirSeleccio = auxElement.Id
                                'si és l'element seleccionat, no busquem més
                                If ObtenirSeleccio = vaElementSel Then
                                    Return ObtenirSeleccio
                                End If
                            End If
                        Else
                            If auxElement.Resources.ContainsKey(vaRecursSel) Then
                                ObtenirSeleccio = auxElement.Id
                                'si és l'element seleccionat, no busquem més
                                If ObtenirSeleccio = vaElementSel Then
                                    Return ObtenirSeleccio
                                End If
                            End If
                        End If
                    Else
                        ObtenirSeleccio = auxElement.Id
                        'si és l'element seleccionat, no busquem més
                        If ObtenirSeleccio = vaElementSel Then
                            Return ObtenirSeleccio
                        End If
                    End If
                End If
                'si passem de l'hora, no busquem més
                If auxElement.Starts > auxDataHora Then Return ObtenirSeleccio
            Next
        End If

        Return ObtenirSeleccio

    End Function

    Private Function ObtenirSeleccioPerRecurs(ByVal pPunt As Point, ByVal pRecurs As String) As String
        'només val per mode dia
        Dim auxElement As PGElement
        Dim auxDataHora As DateTime = DePosicioADataHora(pPunt, False)

        ObtenirSeleccioPerRecurs = ""

        If VisualizationMode = PGMode.Day Or VisualizationMode = PGMode.Resource Then
            'seleccionem l'ultim element ordenat per dataini que compleixi les condicions
            For Each element In vpLlistaElementsData
                auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                If auxElement.Starts <= auxDataHora And auxElement.Ends >= auxDataHora Then
                    If auxElement.Resources.Count = 0 Then
                        If pRecurs = "" Then
                            ObtenirSeleccioPerRecurs = auxElement.Id
                            'si element selecionat, no busquem mes
                            If ObtenirSeleccioPerRecurs = vaElementSel Then
                                Return ObtenirSeleccioPerRecurs
                            End If
                        End If
                    Else
                        For Each recurs In auxElement.Resources
                            If pRecurs = recurs.Key Then
                                ObtenirSeleccioPerRecurs = auxElement.Id
                                'si element seleccionat, no busquem mes
                                If ObtenirSeleccioPerRecurs = vaElementSel Then
                                    Return ObtenirSeleccioPerRecurs
                                End If
                                Exit For
                            End If
                        Next
                    End If
                End If
                'si passem de l'hora, no busquem mes
                If auxElement.Starts > auxDataHora Then Return ObtenirSeleccioPerRecurs
            Next
        End If

        Return ObtenirSeleccioPerRecurs

    End Function

    Private Function ObtenirRecursSeleccio(ByVal pPunt As Point) As PGResource
        Dim idxrecurs As Integer

        ObtenirRecursSeleccio = New PGResource("", Color.Empty)

        If vpAmpleColumna <= 0 Then Exit Function

        idxrecurs = ((pPunt.X - GridPanel.Left - GridPanel.AutoScrollPosition.X) \ vpAmpleColumna)

        If VisualizationMode = PGMode.Day And pPunt.X >= GridPanel.Left Then
            If idxrecurs < 0 Or idxrecurs > vpLlistaRecursosDia.Count - 1 Then Exit Function
            ObtenirRecursSeleccio = vpLlistaRecursosDia.ElementAt(idxrecurs).Value
        End If

    End Function

    Public Function PGResourcesColor(ByVal pPGResources As Dictionary(Of String, Color)) As Color
        Dim primer As Boolean = True
        Dim auxCol As Color

        'determinem el color compost dels recursos 
        If pPGResources.Count > 0 Then
            For Each r In pPGResources
                If primer Then
                    auxCol = r.Value
                    primer = False
                Else
                    auxCol = ColorCompost(auxCol, r.Value)
                End If
            Next
            Return auxCol
        Else
            Return Color.Empty
        End If

    End Function

    Private Function ColorCompost(ByVal pColor1 As Color, ByVal pColor2 As Color) As Color
        Dim auxR, auxG, auxB As Integer

        If pColor1 = Color.Empty Then
            If pColor2 = Color.Empty Then
                Return GridPanel.BackColor
            Else
                Return pColor2
            End If
        Else
            If pColor2 = Color.Empty Then
                Return pColor1
            Else
                auxR = (CInt(pColor1.R) + CInt(pColor2.R)) \ 2
                auxG = (CInt(pColor1.G) + CInt(pColor2.G)) \ 2
                auxB = (CInt(pColor1.B) + CInt(pColor2.B)) \ 2
                Return Color.FromArgb(auxR, auxG, auxB)
            End If
        End If

    End Function

    Public Function ContrastedColor(ByVal pColor As Color) As Color
        Dim distanciaanegre As Long
        Dim distanciaablanc As Long

        If pColor = Color.Empty Then
            Return GridPanel.ForeColor
        Else
            distanciaanegre = Math.Sqrt(pColor.R ^ 2 + pColor.G ^ 2 + pColor.B ^ 2)
            distanciaablanc = Math.Sqrt((255 - pColor.R) ^ 2 + (255 - pColor.G) ^ 2 + (255 - pColor.B) ^ 2)

            If distanciaablanc > distanciaanegre Then
                Return Color.White
            Else
                Return Color.Black
            End If
        End If

    End Function

    Private Sub DibuixarElement(ByRef pGraphics As Graphics, ByVal pElement As PGElement)
        Dim auxRectElem, auxRectDibuix As Rectangle
        Dim auxPuntIni, auxPuntFi As Point
        Dim auxGraphics As Graphics
        Dim auxBm As Bitmap
        Dim auxTamElem As Size
        Dim auxColorElem As Color
        Dim auxDataHoraInici, auxDataHoraFinal As DateTime
        Dim auxIntervalDies As Integer
        Dim auxPuntDibuix As Point
        Dim auxwidth As Integer
        Dim auxheight As Integer
        Dim auxstr As String
        Dim auxelementcella As ElementsCella
        'Dim auxFontBold As Font
        'Dim auxFontItalic As Font
        'Dim auxFontBoldItalic As Font

        'Try
        '    auxFontBold = New Font(Font, FontStyle.Bold)
        'Catch ex As Exception
        '    auxFontBold = Font
        'End Try

        'Try
        '    auxFontItalic = New Font(Font, FontStyle.Italic)
        'Catch ex As Exception
        '    auxFontItalic = Font
        'End Try

        'Try
        '    auxFontBoldItalic = New Font(Font, FontStyle.Italic Or FontStyle.Bold)
        'Catch ex As Exception
        '    auxFontBoldItalic = auxFontItalic
        'End Try

        'calculem els salts de dia de l'interval (-1" a final per si és les 00:00h. del dia seguent)
        auxIntervalDies = DateDiff(DateInterval.Day, pElement.Starts.Date, DateAdd(DateInterval.Second, -1, pElement.Ends))

        'tractament d'elements que abarquen varis dies
        For i = 0 To auxIntervalDies

            If i = 0 Then
                'primer dia
                auxDataHoraInici = pElement.Starts
            Else
                'no primer dia
                auxDataHoraInici = DateAdd(DateInterval.Day, i, pElement.Starts.Date + prHoraIniciActivitat)
            End If

            If i = auxIntervalDies Then
                'ultim dia
                auxDataHoraFinal = pElement.Ends
            Else
                'no ultim dia
                auxDataHoraFinal = auxDataHoraInici.Date + prHoraFiActivitat
            End If

            'per evitar possibles errors
            If auxDataHoraInici <= auxDataHoraFinal Then

                'calculem les coordenades de l'element en la graella
                auxPuntIni = DeDataHoraAPosicio(auxDataHoraInici, True)
                auxPuntFi = DeDataHoraAPosicio(auxDataHoraFinal, False)

                'només pintem els visibles
                If auxDataHoraFinal > vaDataHoraIniPer And auxDataHoraInici <= vaDataHoraFiPer Then

                    Select Case VisualizationMode

                        Case PGMode.Month
                            'mode mes només llistem els elements que caben a la cel·la (sense color)
                            'texte a escriure
                            'auxstr = pElement.Name & " (" & auxDataHoraInici.ToString("HH:mm") & " " & IIf(auxDataHoraFinal.ToString("HH:mm") = "00:00", "24:00", auxDataHoraFinal.ToString("HH:mm")) & ")"
                            'auxstr = "(" & auxDataHoraInici.ToString("HH:mm") & " " & IIf(auxDataHoraFinal.ToString("HH:mm") = "00:00", "24:00", auxDataHoraFinal.ToString("HH:mm")) & ") " & pElement.Name
                            auxstr = auxDataHoraInici.ToString("HH:mm") & " " & pElement.Name

                            'calculem el tamany de l'element 
                            'auxTamElem = pGraphics.MeasureString(auxstr, IIf(pElement.Done, IIf(pElement.Ends < Now, auxFontBoldItalic, auxFontBold), IIf(pElement.Ends < Now, auxFontItalic, Font))).ToSize
                            auxTamElem = pGraphics.MeasureString(auxstr, IIf(pElement.Done, IIf(pElement.Ends < Now, vaFontDonePast, vaFontDone), IIf(pElement.Ends < Now, vaFontPast, Font))).ToSize
                            auxTamElem.Width = Math.Max(auxTamElem.Width + 1, vpAmpleColumna - 1)

                            'obtenim quants elements hi ha ja a la cel·la i actualitzem amb l'actual
                            If vaNElementsCella.ContainsKey(auxPuntIni) Then
                                auxwidth = Math.Max(vaNElementsCella.Item(auxPuntIni).tamany.Width, auxTamElem.Width)
                                auxheight = vaNElementsCella.Item(auxPuntIni).tamany.Height + Font.Height
                                auxelementcella.tamany.Width = auxwidth
                                auxelementcella.tamany.Height = auxheight
                                auxelementcella.elements = vaNElementsCella.Item(auxPuntIni).elements
                                auxelementcella.elements.Add(auxstr)
                                auxelementcella.fets = vaNElementsCella.Item(auxPuntIni).fets
                                auxelementcella.fets.Add(pElement.Done)
                                vaNElementsCella.Item(auxPuntIni) = auxelementcella
                            Else
                                auxwidth = Math.Max(vpAmpleColumna - 1, auxTamElem.Width)
                                auxheight = Font.Height
                                auxelementcella.tamany.Width = auxwidth
                                auxelementcella.tamany.Height = auxheight
                                auxelementcella.elements = New List(Of String)
                                auxelementcella.elements.Add(auxstr)
                                auxelementcella.fets = New List(Of Boolean)
                                auxelementcella.fets.Add(pElement.Done)
                                vaNElementsCella.Add(auxPuntIni, auxelementcella)
                            End If

                            'rectangle amb l'element sobre el control
                            'rectifiquem coord Y segons el nro. de PGElements previs
                            auxPuntDibuix = auxPuntIni
                            auxPuntDibuix.Y = auxPuntDibuix.Y + auxheight - Font.Height
                            auxRectElem = New Rectangle(auxPuntDibuix, New Size(auxTamElem.Width, auxTamElem.Height))
                            'l'intersectem amb la cel·la
                            auxRectElem = Rectangle.Intersect(auxRectElem, New Rectangle(auxPuntIni.X, auxPuntIni.Y, vpAmpleColumna, vpAltFilaMes))
                            'l'intersectem amb la graella del control
                            auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, vpNroColumnes * vpAmpleColumna, vpNroFiles * vpAltFilaMes))

                            If Not auxRectDibuix.IsEmpty Then

                                'obtenim el color de l'element 
                                If auxDataHoraInici.Month = DisplayDate.Month Then
                                    If IIf(auxDataHoraInici.DayOfWeek = DayOfWeek.Sunday, 6, auxDataHoraInici.DayOfWeek - 1) > IIf(LastWorkingDayOfWeek = DayOfWeek.Sunday, 6, LastWorkingDayOfWeek - 1) Then
                                        'fons cap de setmana
                                        auxColorElem = HLinesColor
                                    Else
                                        'fons normal
                                        auxColorElem = GridPanel.BackColor
                                    End If
                                Else
                                    'fons dia no del mes
                                    auxColorElem = ColorCompost(VLinesColor, HLinesColor)
                                End If

                                'rectangle amb tamany de l'element 
                                auxRectElem = New Rectangle(0, 0, auxTamElem.Width, auxTamElem.Height)

                                'dibuixem l'element en grafics auxiliar
                                auxBm = New Bitmap(auxTamElem.Width, auxTamElem.Height)
                                auxGraphics = Graphics.FromImage(auxBm)
                                auxGraphics.Clear(auxColorElem)
                                'auxGraphics.DrawString(auxstr, IIf(pElement.Done, auxFontBold, Font), New System.Drawing.SolidBrush(Me.ForeColor), auxRectElem)
                                'auxGraphics.DrawString(auxstr, IIf(pElement.Done, IIf(pElement.Ends < Now, auxFontBoldItalic, auxFontBold), IIf(pElement.Ends < Now, auxFontItalic, Font)), New System.Drawing.SolidBrush(Me.ForeColor), auxRectElem)
                                auxGraphics.DrawString(auxstr, IIf(pElement.Done, IIf(pElement.Ends < Now, vaFontDonePast, vaFontDone), IIf(pElement.Ends < Now, vaFontPast, Font)), New System.Drawing.SolidBrush(Me.ForeColor), auxRectElem)

                                'dibuixem el troç de l'element (posX+1 per no xafar les linies de la graella)
                                pGraphics.DrawImage(auxBm, auxRectDibuix.X + 1, auxRectDibuix.Y + 1, New Rectangle(auxRectDibuix.X - auxPuntIni.X, 0, auxRectDibuix.Width - 1, auxRectDibuix.Height), GraphicsUnit.Pixel)
                            End If

                        Case PGMode.Week
                            'calculem el tamany i obtenim el color de l'element (tamany-1 per no xafar les linies de la graella)
                            auxTamElem = New Size(vpAmpleColumna - 1, IIf(auxPuntIni.Y = auxPuntFi.Y, 1, auxPuntFi.Y - auxPuntIni.Y)) 'per evitar elements de tamany 0
                            auxColorElem = IIf(pElement.Color = Color.Empty, IIf(PGResourcesColor(pElement.Resources) = Color.Empty, GridPanel.BackColor, PGResourcesColor(pElement.Resources)), pElement.Color)

                            'rectangle amb tamany de l'element 
                            auxRectElem = New Rectangle(0, 0, auxTamElem.Width, auxTamElem.Height)

                            'dibuixem l'element en grafics auxiliar
                            auxBm = New Bitmap(auxTamElem.Width, auxTamElem.Height)
                            auxGraphics = Graphics.FromImage(auxBm)
                            auxGraphics.Clear(auxColorElem)
                            auxGraphics.DrawString(pElement.Name, IIf(pElement.Done, IIf(pElement.Ends < Now, vaFontDonePast, vaFontDone), IIf(pElement.Ends < Now, vaFontPast, Font)), New System.Drawing.SolidBrush(ContrastedColor(auxColorElem)), auxRectElem)

                            'rectangle amb l'element sobre el control
                            auxRectElem = New Rectangle(auxPuntIni, auxTamElem)
                            'l'intersectem amb la graella del control
                            auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, vpNroColumnes * vpAmpleColumna, vpNroFiles * vpAltFila))
                            'si hi ha interseccio
                            If Not auxRectDibuix.IsEmpty Then
                                'dibuixem el troç de l'element 
                                pGraphics.DrawImage(auxBm, auxRectDibuix.X + 1, auxRectDibuix.Y, New Rectangle(auxRectElem.Width - auxRectDibuix.Width, 0, auxRectDibuix.Width, auxRectDibuix.Height), GraphicsUnit.Pixel)
                                'si es l'element seleccionat el ressaltem 
                                If pElement.Id = vaElementSel Then
                                    pGraphics.DrawRectangle(New System.Drawing.Pen(Me.ForeColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
                                Else
                                    pGraphics.DrawRectangle(New System.Drawing.Pen(VLinesColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
                                End If
                            End If

                        Case PGMode.Day
                            'pintem l'element per cadascun dels seus recursos (si no en té, només al recurs "")
                            'si no té recursos el pintem a la columna "" sense color
                            If pElement.Resources.Count <= 0 Then
                                DibuixarElementRecurs(pGraphics, pElement, "", auxPuntIni, auxPuntFi)
                            Else
                                'si té recursos els pintem a les columnes corresponents amb els seus colors
                                For Each r In pElement.Resources
                                    DibuixarElementRecurs(pGraphics, pElement, r.Key, auxPuntIni, auxPuntFi)
                                Next
                            End If

                        Case PGMode.Resource
                            If (vaRecursSel = "" And pElement.Resources.Count = 0) Or pElement.Resources.ContainsKey(vaRecursSel) Then
                                'calculem el tamany i obtenim el color de l'element (tamany-1 per no xafar les linies de la graella)
                                auxTamElem = New Size(vpAmpleColumna - 1, IIf(auxPuntIni.Y = auxPuntFi.Y, 1, auxPuntFi.Y - auxPuntIni.Y)) 'per evitar elements de tamany 0
                                If vaRecursSel = "" Then
                                    auxColorElem = GridPanel.BackColor
                                Else
                                    auxColorElem = IIf(pElement.Resources(vaRecursSel) = Color.Empty, GridPanel.BackColor, pElement.Resources(vaRecursSel))
                                End If

                                'rectangle amb tamany de l'element 
                                auxRectElem = New Rectangle(0, 0, auxTamElem.Width, auxTamElem.Height)

                                'dibuixem l'element en grafics auxiliar
                                auxBm = New Bitmap(auxTamElem.Width, auxTamElem.Height)
                                auxGraphics = Graphics.FromImage(auxBm)
                                auxGraphics.Clear(auxColorElem)
                                auxGraphics.DrawString(pElement.Name, IIf(pElement.Done, IIf(pElement.Ends < Now, vaFontDonePast, vaFontDone), IIf(pElement.Ends < Now, vaFontPast, Font)), New System.Drawing.SolidBrush(ContrastedColor(auxColorElem)), auxRectElem)

                                'rectangle amb l'element sobre el control
                                auxRectElem = New Rectangle(auxPuntIni, auxTamElem)
                                'l'intersectem amb la graella del control
                                auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, vpNroColumnes * vpAmpleColumna, vpNroFiles * vpAltFila))
                                'si hi ha interseccio
                                If Not auxRectDibuix.IsEmpty Then
                                    'dibuixem el troç de l'element 
                                    pGraphics.DrawImage(auxBm, auxRectDibuix.X + 1, auxRectDibuix.Y, New Rectangle(auxRectElem.Width - auxRectDibuix.Width, 0, auxRectDibuix.Width, auxRectDibuix.Height), GraphicsUnit.Pixel)
                                    'si es l'element seleccionat el ressaltem 
                                    If pElement.Id = vaElementSel Then
                                        pGraphics.DrawRectangle(New System.Drawing.Pen(Me.ForeColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
                                    Else
                                        pGraphics.DrawRectangle(New System.Drawing.Pen(VLinesColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
                                    End If
                                End If
                            End If

                    End Select

                End If
            End If
        Next

    End Sub

    'pinta l'element per un dels seus recursos
    Private Sub DibuixarElementRecurs(ByRef pGraphics As Graphics, ByVal pElement As PGElement, ByVal pNomRecurs As String, ByVal pPuntIni As Point, ByVal pPuntFi As Point)
        Dim auxRectElem, auxRectDibuix As Rectangle
        Dim auxGraphics As Graphics
        Dim auxBm As Bitmap
        Dim auxTamElem As Size
        Dim auxColorElem As Color
        Dim auxint As Integer
        'Dim auxFontBold As Font

        'Try
        '    auxFontBold = New Font(Font, FontStyle.Bold)
        'Catch ex As Exception
        '    auxFontBold = Font
        'End Try

        'obtenim l'index del recurs en la taula de recursos del dia
        auxint = vpLlistaRecursosDia.IndexOfKey(pNomRecurs)

        If auxint = -1 Then
            RaiseEvent PGMessage(T("ERROR"), "'" & pNomRecurs & "' " & T("no trobat a llista de recursos dia"))
            Exit Sub
        End If

        'encolumnem l'element/recurs segons el recurs
        pPuntIni.X += auxint * vpAmpleColumna
        pPuntFi.X += auxint * vpAmpleColumna

        'calculem el tamany i obtenim el color de l'element (tamany-1 per no xafar les linies de la graella)
        auxTamElem = New Size(vpAmpleColumna - 1, IIf(pPuntIni.Y = pPuntFi.Y, 1, pPuntFi.Y - pPuntIni.Y)) 'per evitar elements de tamany 0
        auxColorElem = If(pNomRecurs = "", GridPanel.BackColor, IIf(pElement.Resources(pNomRecurs) = Color.Empty, GridPanel.BackColor, pElement.Resources(pNomRecurs)))

        'rectangle amb tamany de l'element 
        auxRectElem = New Rectangle(0, 0, auxTamElem.Width, auxTamElem.Height)

        'dibuixem l'element en grafics auxiliar
        auxBm = New Bitmap(auxTamElem.Width, auxTamElem.Height)
        auxGraphics = Graphics.FromImage(auxBm)
        auxGraphics.Clear(auxColorElem)
        auxGraphics.DrawString(pElement.Name, IIf(pElement.Done, vaFontDone, Font), New System.Drawing.SolidBrush(ContrastedColor(auxColorElem)), auxRectElem)

        'rectangle amb l'element sobre el control
        auxRectElem = New Rectangle(pPuntIni, auxTamElem)
        'l'intersectem amb la graella del control
        auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, vpNroColumnes * vpAmpleColumna, vpNroFiles * vpAltFila))
        'si hi ha interseccio
        If Not auxRectDibuix.IsEmpty Then
            'dibuixem el troç de l'element 
            pGraphics.DrawImage(auxBm, auxRectDibuix.X + 1, auxRectDibuix.Y, New Rectangle(auxRectElem.Width - auxRectDibuix.Width, 0, auxRectDibuix.Width, auxRectDibuix.Height), GraphicsUnit.Pixel)
            'si es l'element seleccionat el ressaltem 
            If pElement.Id = vaElementSel Then
                pGraphics.DrawRectangle(New System.Drawing.Pen(Me.ForeColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
            Else
                pGraphics.DrawRectangle(New System.Drawing.Pen(VLinesColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width, auxRectDibuix.Height - 1)
            End If
        End If

    End Sub

    Private Sub DibuixarElementsDia(ByRef pGraphics As Graphics, ByVal pData As Date)
        Dim auxRectElem, auxRectDibuix As Rectangle
        Dim auxPuntIni As Point
        Dim auxGraphics As Graphics
        Dim auxBm As Bitmap
        Dim auxPuntDibuix As Point
        Dim auxelementcella As ElementsCella
        Dim auxwidth As Integer
        Dim auxheight As Integer
        Dim auxcolor As Color
        Dim auxintH, auxintW As Integer
        'Dim auxFontBold As Font

        'Try
        '    auxFontBold = New Font(Font, FontStyle.Bold)
        'Catch ex As Exception
        '    auxFontBold = Font
        'End Try

        'calculem les coordenades de l'element en la graella 
        auxPuntIni = DeDataHoraAPosicio(pData, True)

        'nomes dibuixem l'element si està en l'area visible
        If auxPuntIni.X > GridPanel.Width Or auxPuntIni.X + vpAmpleColumna < 0 Or auxPuntIni.Y > GridPanel.Height Then Exit Sub

        'obtenim la informació per pintar la cel·la (tamany i elements)
        If vaNElementsCella.ContainsKey(auxPuntIni) Then
            auxelementcella = vaNElementsCella(auxPuntIni)
            auxwidth = Math.Max(auxelementcella.tamany.Width + 4, vpAmpleColumna)
            auxheight = Math.Max(auxelementcella.tamany.Height + 4, vpAltFilaMes)

            'dibuixem els elements en grafics auxiliar
            auxBm = New Bitmap(auxwidth, auxheight)
            auxGraphics = Graphics.FromImage(auxBm)
            If pData.Month = DisplayDate.Month Then
                If IIf(pData.DayOfWeek = DayOfWeek.Sunday, 6, pData.DayOfWeek - 1) > IIf(LastWorkingDayOfWeek = DayOfWeek.Sunday, 6, LastWorkingDayOfWeek - 1) Then
                    'fons cap de setmana
                    auxcolor = HLinesColor
                Else
                    'fons normal
                    auxcolor = GridPanel.BackColor
                End If
            Else
                'fons dies d'altre mes
                auxcolor = ColorCompost(VLinesColor, HLinesColor)
            End If
            auxGraphics.Clear(auxcolor)

            auxPuntDibuix = New Point(0, 0)
            For i = 0 To auxelementcella.elements.Count - 1
                auxRectElem = New Rectangle(auxPuntDibuix.X, auxPuntDibuix.Y, auxelementcella.tamany.Width, Font.Height)
                auxGraphics.DrawString(auxelementcella.elements(i), IIf(auxelementcella.fets(i), vaFontDone, Font), New System.Drawing.SolidBrush(Me.ForeColor), auxRectElem)
                auxPuntDibuix.Y += Font.Height
            Next

            auxintW = GridPanel.Width - IIf(GridPanel.VerticalScroll.Visible, SystemInformation.VerticalScrollBarWidth, 0)
            auxintH = GridPanel.Height - IIf(GridPanel.HorizontalScroll.Visible, SystemInformation.HorizontalScrollBarHeight, 0)
            'si el rectangle aliniat a la esquerra superior de la cel·la surt per sota de la graella
            'l'aliniem a l'inferior
            If (auxPuntIni.Y + auxelementcella.tamany.Height) > auxintH Then
                'si el rectangle surt per la dreta de la graella
                'l'aliniem a la dreta 
                If auxPuntIni.X + auxelementcella.tamany.Width > auxintW Then
                    auxRectElem = New Rectangle((auxPuntIni.X + vpAmpleColumna - auxelementcella.tamany.Width), (auxintH - auxelementcella.tamany.Height), auxwidth, auxheight)
                Else
                    auxRectElem = New Rectangle(auxPuntIni.X, (auxintH - auxelementcella.tamany.Height), auxwidth, auxheight)
                End If
            Else
                'si el rectangle surt per la dreta de la graella
                'l'aliniem a la dreta 
                If auxPuntIni.X + auxelementcella.tamany.Width > auxintW Then
                    auxRectElem = New Rectangle((auxPuntIni.X + vpAmpleColumna - auxelementcella.tamany.Width), auxPuntIni.Y, auxwidth, auxheight)
                Else
                    auxRectElem = New Rectangle(auxPuntIni.X, auxPuntIni.Y, auxwidth, auxheight)
                End If
            End If

            'intersectem l'element amb el panel
            auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, GridPanel.Width, GridPanel.Height))
            'dibuixem el rectangle amb tots els elements
            pGraphics.DrawImage(auxBm, auxRectDibuix.X + 1, auxRectDibuix.Y + 1, New Rectangle(IIf(auxPuntIni.X < 0, auxRectElem.Width - auxRectDibuix.Width, 0), 0, auxRectDibuix.Width - 2, auxRectDibuix.Height - 1), GraphicsUnit.Pixel)

            'si és el dia seleccionat, l'enmarquem amb el color del text
            If pData = DisplayDate Then
                pGraphics.DrawRectangle(New System.Drawing.Pen(Me.ForeColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width - 1, auxRectDibuix.Height - 1)
            ElseIf pData = vaDataMouse Then
                'si estem sobre el dia, l'enmarquem amb el color de les linies verticals/horitzontals
                If auxcolor = VLinesColor Then
                    auxcolor = HLinesColor
                Else
                    auxcolor = VLinesColor
                End If
                pGraphics.DrawRectangle(New System.Drawing.Pen(auxcolor), auxRectDibuix.X - 1, auxRectDibuix.Y + 1, auxRectDibuix.Width - 1, auxRectDibuix.Height - 1)
                pGraphics.DrawRectangle(New System.Drawing.Pen(auxcolor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width - 1, auxRectDibuix.Height - 1)
            End If
        Else
            'si no hi ha elements a la cel·la pintem només el rectangle de seleccio
            If pData = DisplayDate Then
                auxRectElem = New Rectangle(auxPuntIni.X, auxPuntIni.Y, vpAmpleColumna, vpAltFilaMes)
                auxRectDibuix = Rectangle.Intersect(auxRectElem, New Rectangle(0, 0, GridPanel.Width, GridPanel.Height))
                pGraphics.DrawRectangle(New System.Drawing.Pen(Me.ForeColor), auxRectDibuix.X, auxRectDibuix.Y, auxRectDibuix.Width - 1, auxRectDibuix.Height - 1)
            End If
        End If

    End Sub

    Private Sub ReformatejaControl()
        Dim altCap As Integer
        Dim ampleColCap As Integer
        Dim panelwidth As Integer
        Dim panelheight As Integer
        Dim amplecolant As Integer = vpAmpleColumna
        Dim altfilaant As Integer = vpAltFila

        altCap = CInt(FontHeight * 2)
        ampleColCap = CInt(CreateGraphics().MeasureString(" 00:00 ", Font).Width)

        'dimensionem i posicionem el panell de grid
        GridPanel.Top = altCap
        GridPanel.Left = ampleColCap
        GridPanel.BorderStyle = Me.BorderStyle

        'X,Y coordenades de pantalla; 
        'vaAjustBorder depén del gruix del borderstyle de GridPanel None=0 FixedSingle=1 Fixed3D=2
        Select Case GridPanel.BorderStyle
            Case Windows.Forms.BorderStyle.Fixed3D
                vaAjustBorder = 2
            Case Windows.Forms.BorderStyle.FixedSingle
                vaAjustBorder = 1
            Case Windows.Forms.BorderStyle.None
                vaAjustBorder = 0
        End Select

        panelwidth = Me.Width - ampleColCap - 2 * vaAjustBorder
        panelheight = Me.Height - altCap - 2 * vaAjustBorder
        'panelwidth = GridPanel.Width
        'panelheight = GridPanel.Height

        'dimensionem i posicionem els botons
        BotoMenys.Top = 0
        BotoMenys.Left = 0
        BotoMenys.Width = ampleColCap \ 2 + 2
        BotoMenys.Height = BotoMenys.Width

        BotoMes.Top = 0
        BotoMes.Left = BotoMenys.Width
        BotoMes.Width = ampleColCap \ 2 + 2
        BotoMes.Height = BotoMenys.Width

        vpMinAmpleColumna = CInt(CreateGraphics().MeasureString(Now.Day.ToString & " " & T(MesosCurts(Now.Month - 1)), Font).Width * 1.25)
        If MinColsWidth < vpMinAmpleColumna Then MinColsWidth = vpMinAmpleColumna

        'calculem nro i tamany de files i columnes 
        Select Case VisualizationMode
            Case PGMode.Day
                vpNroFiles = (DuradaJornada.TotalMinutes / MinutesGap)
                vpNroColumnes = CalculaNroColsDia()

            Case PGMode.Week
                vpNroFiles = (DuradaJornada.TotalMinutes / MinutesGap)
                vpNroColumnes = 7

            Case PGMode.Month
                vpNroFiles = 6
                vpNroColumnes = 7

            Case PGMode.Resource
                vpNroFiles = (DuradaJornada.TotalMinutes / MinutesGap)
                vpNroColumnes = 7

        End Select

        vpAmpleColumna = Math.Max(MinColsWidth, panelwidth \ vpNroColumnes)

        vpAltFila = Math.Max(MinRowsHeight, panelheight \ vpNroFiles)
        vpAltFilaMes = Math.Max(MinRowsHeight, panelheight \ 6)

        'determinem l'inici del scroll del panel quan no abarca la graella
        If vpAltFila > MinRowsHeight Then
            GridPanel.VerticalScroll.Enabled = False
            If vpAmpleColumna > MinColsWidth Then
                'cap scrollbar
                GridPanel.HorizontalScroll.Enabled = False
                GridPanel.AutoScrollMinSize = New Size(0, 0)
                'redimensionem el panell de grid segons dimensio cel·les, el espai del control i els scrollbars
                GridPanel.Width = Math.Min(panelwidth, vpAmpleColumna * vpNroColumnes)
                GridPanel.Height = Math.Min(panelheight, IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) * vpNroFiles)
            Else
                'només scrollbar horitzontal
                GridPanel.HorizontalScroll.Enabled = True
                GridPanel.AutoScrollMinSize = New Size(vpNroColumnes * vpAmpleColumna, 0)
                'redimensionem el panell de grid segons dimensio cel·les, el espai del control i els scrollbars
                GridPanel.Width = Math.Min(panelwidth, vpAmpleColumna * vpNroColumnes)
                GridPanel.Height = Math.Min(panelheight, IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) * vpNroFiles + SystemInformation.HorizontalScrollBarHeight)
            End If
        Else
            GridPanel.VerticalScroll.Enabled = True
            If vpAmpleColumna > MinColsWidth Then
                'només scrollbar vertical
                GridPanel.HorizontalScroll.Enabled = False
                GridPanel.AutoScrollMinSize = New Size(0, vpNroFiles * vpAltFila)
                'redimensionem el panell de grid segons dimensio cel·les, el espai del control i els scrollbars
                GridPanel.Width = Math.Min(panelwidth, vpAmpleColumna * vpNroColumnes + SystemInformation.VerticalScrollBarWidth)
                GridPanel.Height = Math.Min(panelheight, IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) * vpNroFiles)
            Else
                'scrollbar horitzontal i vertical
                GridPanel.HorizontalScroll.Enabled = True
                GridPanel.AutoScrollMinSize = New Size(vpNroColumnes * vpAmpleColumna, vpNroFiles * vpAltFila)
                'redimensionem el panell de grid segons dimensio cel·les, el espai del control i els scrollbars
                GridPanel.Width = Math.Min(panelwidth, vpAmpleColumna * vpNroColumnes + SystemInformation.VerticalScrollBarWidth)
                GridPanel.Height = Math.Min(panelheight, IIf(VisualizationMode = PGMode.Month, vpAltFilaMes, vpAltFila) * vpNroFiles + SystemInformation.HorizontalScrollBarHeight)
            End If
        End If

        vaReformateja = False

        If amplecolant <> vpAmpleColumna Or altfilaant <> vpAltFila Then
            RaiseEvent PGSizeChanged(vpAmpleColumna, vpAltFila)
        End If

    End Sub

    Private Sub ActualitzaPropietatsPeriode()

        'ajustem hora final activitat fins que la durada sigui divisible pel periode minim
        If prHoraFiActivitat < prHoraIniciActivitat + New TimeSpan(0, MinutesGap, 0) Then
            prHoraFiActivitat = prHoraIniciActivitat + New TimeSpan(0, MinutesGap, 0)
        End If
        If (DuradaJornada.TotalMinutes Mod MinutesGap) <> 0 Then
            Do
                prHoraFiActivitat -= New TimeSpan(0, 1, 0)
            Loop Until (DuradaJornada.TotalMinutes Mod MinutesGap) = 0
        End If

        'ajustem hora inici dinar fins que el temps des de inici activitat sigui divisible pel periode minim
        If prHoraIniciDinar < prHoraIniciActivitat Then
            prHoraIniciDinar = prHoraIniciActivitat
        End If
        If ((prHoraIniciDinar.TotalMinutes - prHoraIniciActivitat.TotalMinutes) Mod MinutesGap) <> 0 Then
            Do
                prHoraIniciDinar -= New TimeSpan(0, 1, 0)
            Loop Until ((prHoraIniciDinar.TotalMinutes - prHoraIniciActivitat.TotalMinutes) Mod MinutesGap) = 0
        End If

        'ajustem hora fi dinar fins que el temps des de inici dinar sigui divisible pel periode minim
        If prHoraFiDinar < prHoraIniciDinar Then
            prHoraFiDinar = prHoraIniciDinar
        End If
        If ((prHoraFiDinar.TotalMinutes - prHoraIniciDinar.TotalMinutes) Mod MinutesGap) <> 0 Then
            Do
                prHoraFiDinar -= New TimeSpan(0, 1, 0)
            Loop Until ((prHoraFiDinar.TotalMinutes - prHoraIniciDinar.TotalMinutes) Mod MinutesGap) = 0
        End If

    End Sub

    Private Sub DibuixarCapçaleraCols(ByRef pGraphics As Graphics)
        Dim llapisCap As System.Drawing.Pen
        Dim i, posX As Integer
        Dim textWidth As Integer
        Dim brochaTexteCap As System.Drawing.Brush
        Dim auxTexte As String
        Dim auxFontBold As Font
        Dim auxData As Date
        Dim llapisColsCap As System.Drawing.Pen

        Try
            auxFontBold = New Font(Font, FontStyle.Bold)
        Catch ex As Exception
            auxFontBold = Font
        End Try

        'creem les eines de dibuix
        llapisCap = New System.Drawing.Pen(HeaderFontColor)
        llapisColsCap = New System.Drawing.Pen(HLinesColor)
        brochaTexteCap = New System.Drawing.SolidBrush(HeaderFontColor)

        For i = 1 To vpNroColumnes
            posX = GridPanel.Left + vpAmpleColumna * i + GridPanel.AutoScrollPosition.X + vaAjustBorder

            Select Case VisualizationMode

                Case PGMode.Week
                    If i = 1 Then
                        auxTexte = T("Setmana del") & " " & vaDataHoraIniPer.ToString("d") & " " & T("al") & " " & vaDataHoraFiPer.ToString("d")
                        'pintem el titol de columna al mig de la 1a. columna
                        pGraphics.DrawString(auxTexte, auxFontBold, brochaTexteCap, GridPanel.Left + 10, 0)
                    End If
                    'obtenim la data del dia i de la setmana de la DataActual
                    auxData = DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                    auxTexte = T(DiesSetmanaCurts(auxData.DayOfWeek)) & " " & auxData.Day.ToString & " " & T(MesosCurts(auxData.Month - 1))
                    'pintem el titol de columna al mig de la columna
                    textWidth = CInt(CreateGraphics().MeasureString(auxTexte, Font).Width)
                    If (posX - ((vpAmpleColumna + textWidth) \ 2)) <= Width Then
                        pGraphics.DrawString(auxTexte, Font, brochaTexteCap, (posX - ((vpAmpleColumna + textWidth) \ 2)), FontHeight)
                    End If

                Case PGMode.Month
                    If i = 1 Then
                        auxTexte = T(Mesos(DisplayDate.Month - 1)) & " " & DisplayDate.ToString("yyyy")
                        'pintem el titol de columna al mig de la 1a. columna
                        pGraphics.DrawString(auxTexte, auxFontBold, brochaTexteCap, GridPanel.Left + 10, 0)
                    End If
                    auxTexte = T(DiesSetmana(DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate).DayOfWeek))
                    'pintem el titol de columna al mig de la columna
                    textWidth = CInt(CreateGraphics().MeasureString(auxTexte, Font).Width)
                    If (posX - ((vpAmpleColumna + textWidth) \ 2)) <= Width Then
                        pGraphics.DrawString(auxTexte, Font, brochaTexteCap, (posX - ((vpAmpleColumna + textWidth) \ 2)), FontHeight)
                    End If

                Case PGMode.Day
                    If i = 1 Then
                        auxTexte = T(DiesSetmana(DisplayDate.DayOfWeek)) & " " & DisplayDate.ToString("d")
                        'pintem el titol de columna al mig de la 1a. columna
                        pGraphics.DrawString(auxTexte, auxFontBold, brochaTexteCap, GridPanel.Left + 10, 0)
                    End If
                    'pintem els recursos al mig de la columna
                    If i <= vpLlistaRecursosDia.Count Then
                        auxTexte = vpLlistaRecursosDia.ElementAt(i - 1).Value.Name
                        If auxTexte = "" Then auxTexte = Chr(216) 'conjunt buit
                        textWidth = CInt(CreateGraphics().MeasureString(auxTexte, Font).Width)
                        If (posX - ((vpAmpleColumna + textWidth) \ 2)) <= Width Then
                            pGraphics.DrawString(auxTexte, Font, brochaTexteCap, (posX - ((vpAmpleColumna + textWidth) \ 2)), FontHeight)
                        End If
                    End If

                Case PGMode.Resource
                    If i = 1 Then
                        auxTexte = T("Recurs") & " " & IIf(vaRecursSel = "", Chr(216), "'" & vaRecursSel & "'")
                        'pintem el titol de columna al mig de la 1a. columna
                        pGraphics.DrawString(auxTexte, auxFontBold, brochaTexteCap, GridPanel.Left + 10, 0)
                    End If
                    'obtenim la data del dia i de la setmana de la DataActual
                    auxData = DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                    auxTexte = T(DiesSetmanaCurts(auxData.DayOfWeek)) & " " & auxData.Day.ToString & " " & T(MesosCurts(auxData.Month - 1))
                    'pintem el titol de columna al mig de la columna
                    textWidth = CInt(CreateGraphics().MeasureString(auxTexte, Font).Width)
                    If (posX - ((vpAmpleColumna + textWidth) \ 2)) <= Width Then
                        pGraphics.DrawString(auxTexte, Font, brochaTexteCap, (posX - ((vpAmpleColumna + textWidth) \ 2)), FontHeight)
                    End If

            End Select

            'pintem marques columnes 
            If VLinesColor <> Me.BackColor And i < vpNroColumnes Then
                If posX > GridPanel.Left Then pGraphics.DrawLine(llapisColsCap, posX, GridPanel.Top - Font.Height \ 2, posX, GridPanel.Top - 1)
            End If

            If posX > Width Then Exit For
        Next

    End Sub

    Private Sub DibuixarCapçaleraFiles(ByRef pGraphics As Graphics)
        Dim auxPunt As Point
        Dim llapisCap As System.Drawing.Pen
        Dim llapisLiniesCap As System.Drawing.Pen
        Dim i As Integer
        Dim brochaTexteCap, brochaCap As System.Drawing.Brush
        Dim auxFontBold As Font
        Dim auxTexte As String
        Dim auxDataDia1 As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)
        Dim auxData As Date
        Dim PosY As Integer

        Try
            auxFontBold = New Font(Font, FontStyle.Bold)
        Catch ex As Exception
            auxFontBold = Font
        End Try

        'creem les eines de dibuix
        llapisCap = New System.Drawing.Pen(HeaderFontColor)
        llapisLiniesCap = New System.Drawing.Pen(HLinesColor)
        brochaTexteCap = New System.Drawing.SolidBrush(HeaderFontColor)
        brochaCap = New System.Drawing.SolidBrush(Me.BackColor)

        'pintem un rectangle per tapar la part de la capçalera de columnes que invaeix la capçalera de files
        pGraphics.FillRectangle(brochaCap, New Rectangle(0, 0, GridPanel.Left, GridPanel.Top))

        For i = 0 To vpNroFiles - 1

            auxPunt.X = 0
            auxPunt.Y = i + 1
            auxPunt = DeCoordenadesAPosicio(auxPunt)

            If VisualizationMode = PGMode.Month Then
                'pintem les dates del primer dia de cada setmana al mig de la fila
                PosY = auxPunt.Y + (vpAltFilaMes - FontHeight) / 2
                If PosY >= GridPanel.Top + FontHeight / 2 Then
                    'obtenim la data del primer dia de cada setmana 
                    auxData = DateAdd(DateInterval.Day, 7 * i + IIf(DayOfWeek.Monday - auxDataDia1.DayOfWeek > 0, DayOfWeek.Monday - auxDataDia1.DayOfWeek - 7, DayOfWeek.Monday - auxDataDia1.DayOfWeek), auxDataDia1)
                    auxTexte = auxData.ToString("dd/MM")
                    pGraphics.DrawString(auxTexte, Font, brochaTexteCap, auxPunt.X, PosY)
                    'pintem marques files
                    If i > 0 Then pGraphics.DrawLine(llapisLiniesCap, GridPanel.Left - Font.Height \ 2, auxPunt.Y + vaAjustBorder, GridPanel.Left, auxPunt.Y + vaAjustBorder)
                End If

            Else
                'setmana o dia o recurs

                'fins al punt final de visualitzacio vertical
                If auxPunt.Y >= Height Then Exit For

                'pintem les hores al mig de la recta
                If auxPunt.Y >= GridPanel.Top Then
                    PosY = (auxPunt.Y - FontHeight \ 2)
                    pGraphics.DrawString(TimeSerial(0, prHoraIniciActivitat.TotalMinutes + i * MinutesGap, 0).ToString("HH:mm"), Font, brochaTexteCap, auxPunt.X, PosY)
                    'pintem marques files
                    pGraphics.DrawLine(llapisLiniesCap, GridPanel.Left - Font.Height \ 2 + 1, auxPunt.Y + vaAjustBorder, GridPanel.Left, auxPunt.Y + vaAjustBorder)
                End If
            End If

        Next

    End Sub

    Private Function CalculaNroColsDia() As Integer
        'calcula el nro de columnes necessaries segons el nro. de recursos dels elements del dia
        'minin 7 i compta 1 recurs <no assignat>
        Dim nrocols As Integer = 1
        Dim auxElement As PGElement

        vpLlistaRecursosDia.Clear()
        vpLlistaRecursosDia.Add("", New PGResource("", GridPanel.BackColor))

        For Each element In vpLlistaElementsData
            auxElement = New PGElement(vpLlistaElements.Item(element.Value))

            If auxElement.Ends > vaDataHoraIniPer And auxElement.Starts <= vaDataHoraFiPer Then
                For Each r In auxElement.Resources
                    If Not vpLlistaRecursosDia.ContainsKey(r.Key) Then
                        vpLlistaRecursosDia.Add(r.Key, New PGResource(r.Key, r.Value))
                        nrocols += 1
                    End If
                Next
            End If

            If auxElement.Starts > vaDataHoraFiPer Then Exit For
        Next

        Return Math.Max(7, nrocols)

    End Function

    Private Sub CalculaDatesPeriode()
        'calculem la primera i la ultima data de visualització modo mes
        prPrimeraData = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DateSerial(Year(DisplayDate), Month(DisplayDate), 1).DayOfWeek > 0, DayOfWeek.Monday - DateSerial(Year(DisplayDate), Month(DisplayDate), 1).DayOfWeek - 7, DayOfWeek.Monday - DateSerial(Year(DisplayDate), Month(DisplayDate), 1).DayOfWeek), DateSerial(Year(DisplayDate), Month(DisplayDate), 1))
        prUltimaData = DateAdd(DateInterval.Day, 41, prPrimeraData)

        Select Case VisualizationMode

            Case PGMode.Day
                vaDataHoraIniPer = DisplayDate
                vaDataHoraFiPer = DisplayDate

            Case PGMode.Week
                vaDataHoraIniPer = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                vaDataHoraFiPer = DateAdd(DateInterval.Day, 6, vaDataHoraIniPer)

            Case PGMode.Month
                vaDataHoraIniPer = prPrimeraData
                vaDataHoraFiPer = prUltimaData

            Case PGMode.Resource
                vaDataHoraIniPer = DateAdd(DateInterval.Day, IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                vaDataHoraFiPer = DateAdd(DateInterval.Day, 6, vaDataHoraIniPer)

        End Select

        vaDataHoraFiPer = DateAdd(DateInterval.Minute, 1439, vaDataHoraFiPer)

    End Sub

    'retorna la data i l'hora final de l'element segons la duració real i (si es permet elements entre dies) l'horari d'activitat
    Public Function PGElementGetEndTime(ByVal pPGElement As PGElement) As DateTime
        Dim auxdata As DateTime
        Dim auxtime1, auxtime2 As TimeSpan
        Dim auxint1, auxint2 As Integer
        Dim auxelement As PGElement = New PGElement(pPGElement) 'per no alterar pPGElement
        Dim auxDuradaTotal As TimeSpan = PGElementGetDuration(auxelement)

        If prPermetreElementsEntreDies Then
            'periode entre pini i hora inici activitat
            auxtime1 = prHoraIniciActivitat - auxelement.Starts.TimeOfDay
            'periode entre pini i hora fi activitat
            auxtime2 = prHoraFiActivitat - auxelement.Starts.TimeOfDay

            'posicionem la data/hora inicial a l'hora inici activitat i adaptem pdurada en consequencia
            If auxtime1 > TimeSpan.Zero Then
                'si hora es anterior a inici activitat ==> comencem a hora inici activitat
                auxelement.Starts = DateAdd(DateInterval.Minute, Fix(auxtime1.TotalMinutes), auxelement.Starts)
                auxdata = auxelement.Starts
            Else
                If auxtime2 <= TimeSpan.Zero Then
                    'si hora es posterior a fi activitat ==> comencem a hora inici activitat de l'endemà
                    auxelement.Starts = DateAdd(DateInterval.Minute, Fix(auxtime1.TotalMinutes), auxelement.Starts)
                    auxelement.Starts = DateAdd(DateInterval.Day, 1, auxelement.Starts)
                    auxdata = auxelement.Starts
                Else
                    'si hora inici en periode habil ===>
                    'posicionem la data/hora inicial a l'hora inici activitat i adaptem pdurada en consequencia
                    auxdata = DateAdd(DateInterval.Minute, Fix(auxtime1.TotalMinutes), auxelement.Starts)
                    auxDuradaTotal = auxDuradaTotal - auxtime1
                End If
            End If

            'calculem els dies sencers que cal afegir i els minuts addicionals que cal afegir
            If auxDuradaTotal > DuradaJornada() Then
                auxint1 = Fix(auxDuradaTotal.TotalMinutes) \ Fix(DuradaJornada.TotalMinutes)
                auxint2 = Fix(auxDuradaTotal.TotalMinutes) Mod Fix(DuradaJornada.TotalMinutes)
            Else
                auxint1 = 0
                auxint2 = auxDuradaTotal.TotalMinutes
            End If

            auxdata = DateAdd(DateInterval.Day, auxint1, auxdata)
            auxdata = DateAdd(DateInterval.Minute, auxint2, auxdata)

            'si final a horainiciperiode --> horafiperiode del dia anterior
            If auxdata.TimeOfDay = prHoraIniciActivitat Then
                auxdata = DateAdd(DateInterval.Day, -1, auxdata.Date)
                auxdata = auxdata + prHoraFiActivitat
            End If
        Else
            'no tenim en compte l'horari
            auxdata = pPGElement.Starts + auxDuradaTotal
        End If

        Return auxdata

    End Function

    'retorna la durada efectiva de l'element en funció dels recursos assignats i les durades compartida i no 
    Public Function PGElementGetDuration(ByVal pPGElement As PGElement) As TimeSpan
        Dim auxTimespan As TimeSpan

        'dividim la durada compartible pel nro. de recursos
        auxTimespan = pPGElement.NonSharedDuration + New TimeSpan(0, pPGElement.SharedDuration.TotalMinutes \ IIf(pPGElement.Resources.Count > 0, pPGElement.Resources.Count, 1), 0)

        Return auxTimespan

    End Function

    Private Function DuradaJornada() As TimeSpan
        Return prHoraFiActivitat - prHoraIniciActivitat
    End Function

    'per refrescar la grid des de fora
    Public Sub RefreshGrid()
        vaReformateja = True
        Invalidate()
        Update()
    End Sub

    'monta la clau de la llista d'elements ordenada per data i hora
    Private Function ClauElementsData(ByVal pPGElement As PGElement) As String
        Return pPGElement.Starts.ToString("yyyyMMddHHmmss") & (TimeSpan.MaxValue.TotalMinutes - PGElementGetDuration(pPGElement).TotalMinutes) & pPGElement.Id
    End Function

    'altes i modificacions des de la grid
    Private Function PGGridElementAddUpdate(ByRef pPGElement As PGElement) As PGReturnCode
        Dim auxTimeSpan As TimeSpan

        'actualitzem l'inici segons el periode d'activitat diaria
        auxTimeSpan = pPGElement.Starts.TimeOfDay - prHoraIniciActivitat
        If auxTimeSpan < TimeSpan.Zero Then
            pPGElement.Starts = DateAdd(DateInterval.Day, -1, pPGElement.Starts.Date) + prHoraFiActivitat + auxTimeSpan
        Else
            auxTimeSpan = pPGElement.Starts.TimeOfDay - prHoraFiActivitat
            If auxTimeSpan >= TimeSpan.Zero Then
                pPGElement.Starts = DateAdd(DateInterval.Day, 1, pPGElement.Starts.Date) + prHoraIniciActivitat + auxTimeSpan
            End If
        End If

        'actualitzem el final de l'element segons el periode d'activitat
        pPGElement.Ends = PGElementGetEndTime(pPGElement)

        'si no es permet que els elements sobrepassin un dia
        If Not prPermetreElementsEntreDies Then
            If pPGElement.Starts.TimeOfDay < prHoraIniciActivitat Or pPGElement.Ends.TimeOfDay > prHoraFiActivitat Or pPGElement.Starts.Date <> pPGElement.Ends.Date Then
                RaiseEvent PGMessage(T("INFORMACIÓ"), T("No es pot sobrepassar l'horari ") & Mid(prHoraIniciActivitat.ToString, 1, 5) & "->" & Mid(prHoraFiActivitat.ToString, 1, 5))
                Return PGReturnCode.PGError
            End If
        End If

        'valida solapament recursos
        If ValidaSolapaments(pPGElement) Then
            vaElementSel = pPGElement.Id
            If vpLlistaElements.ContainsKey(pPGElement.Id) Then
                'no es pot modificar elements fets
                If pPGElement.Done Then
                    RaiseEvent PGMessage(T("INFORMACIÓ"), T("Element #") & pPGElement.Id & T(": No es pot modificar elements fets."))
                    Return PGReturnCode.PGError
                End If
                'actualizem l'element
                vpLlistaElements.Item(pPGElement.Id) = New PGElement(pPGElement)
                vpLlistaElementsData.RemoveAt(vpLlistaElementsData.IndexOfValue(pPGElement.Id))
                'ordenem per inici de l'element ascendent i durada descendent
                'vpLlistaElementsData.Add(pPGElement.Starts.ToString("yyyyMMddHHmmss") & (TimeSpan.MaxValue.TotalMinutes - PGElementGetDuration(pPGElement).TotalMinutes) & pPGElement.Id, pPGElement.Id)
                vpLlistaElementsData.Add(ClauElementsData(pPGElement), pPGElement.Id)
                Return PGReturnCode.PGUpdated
            Else
                'creem l'element (si existeix, error)
                vpLlistaElements.Add(pPGElement.Id, New PGElement(pPGElement))
                'ordenem per inici de l'element ascendent i durada descendent
                'vpLlistaElementsData.Add(pPGElement.Starts.ToString("yyyyMMddHHmmss") & (TimeSpan.MaxValue.TotalMinutes - PGElementGetDuration(pPGElement).TotalMinutes) & pPGElement.Id, pPGElement.Id)
                vpLlistaElementsData.Add(ClauElementsData(pPGElement), pPGElement.Id)
                Return PGReturnCode.PGAdded
            End If
        Else
            Return PGReturnCode.PGError
        End If

    End Function

    'altes i modificacions des de programes externs 
    '(respectem hora inici i final encara que estiguin fora del periode d'activitat)
    ' si prSolapaRecursExt i es solapa en alta, blanquejem recurs
    Public Function PGElementAddUpdate(ByRef pPGElement As PGElement) As PGReturnCode
        Dim auxbool As Boolean = False

        'validem dades de PGElement
        If pPGElement.Starts = Date.MinValue Then
            RaiseEvent PGMessage(T("INFORMACIÓ"), T("Element #") & pPGElement.Id & T(": Cal informar la data/hora d'inici de l'element."))
            Return PGReturnCode.PGError
        End If
        If pPGElement.Ends = Date.MinValue Then
            RaiseEvent PGMessage(T("INFORMACIÓ"), T("Element #") & pPGElement.Id & T(": Cal informar la data/hora final de l'element."))
            Return PGReturnCode.PGError
        End If

        'actualitzacio
        If vpLlistaElements.ContainsKey(pPGElement.Id) Then
            'valida solapament recursos
            If Not ValidaSolapaments(pPGElement) Then
                'si hi ha solapament, error
                Return PGReturnCode.PGError
            End If

            'no es pot modificar elements fets (només l'indicador de fet)
            If vpLlistaElements.Item(pPGElement.Id).Done Then
                'comparem els recursos
                If vpLlistaElements.Item(pPGElement.Id).Resources.Count <> pPGElement.Resources.Count Then
                    auxbool = True
                Else
                    For Each k In vpLlistaElements.Item(pPGElement.Id).Resources.Keys
                        If Not pPGElement.Resources.ContainsKey(k) Then
                            auxbool = True
                        End If
                    Next
                End If

                If vpLlistaElements.Item(pPGElement.Id).Name <> pPGElement.Name _
                Or vpLlistaElements.Item(pPGElement.Id).Starts <> pPGElement.Starts _
                Or vpLlistaElements.Item(pPGElement.Id).Ends <> pPGElement.Ends _
                Or vpLlistaElements.Item(pPGElement.Id).SharedDuration <> pPGElement.SharedDuration _
                Or vpLlistaElements.Item(pPGElement.Id).NonSharedDuration <> pPGElement.NonSharedDuration _
                Or auxbool Then
                    RaiseEvent PGMessage(T("INFORMACIÓ"), T("Element #") & pPGElement.Id & T(": No es pot modificar elements fets."))
                    Return PGReturnCode.PGError
                End If
            End If

            'actualizem l'element
            vpLlistaElements.Item(pPGElement.Id) = New PGElement(pPGElement)
            vpLlistaElementsData.RemoveAt(vpLlistaElementsData.IndexOfValue(pPGElement.Id))
            'ordenem per inici de l'element ascendent i durada descendent
            vpLlistaElementsData.Add(ClauElementsData(pPGElement), pPGElement.Id)
            'deixem seleccionat el element actualitzat
            vaElementSel = pPGElement.Id
            RaiseEvent PGElement_Updated(pPGElement)
            Return PGReturnCode.PGUpdated
        Else
            'alta

            'valida solapament recursos
            If Not ValidaSolapaments(pPGElement) Then
                If prSolapaRecursExt Then
                    'si hi ha solapament i es permet en alta, netegem recursos de l'element
                    pPGElement.Resources = New Dictionary(Of String, Color)
                Else
                    'si hi ha solapament i no es permet en alta, error
                    Return PGReturnCode.PGError
                End If
            End If

            'creem l'element (si existeix, error)
            vpLlistaElements.Add(pPGElement.Id, New PGElement(pPGElement))
            'ordenem per inici de l'element ascendent i durada descendent
            vpLlistaElementsData.Add(ClauElementsData(pPGElement), pPGElement.Id)
            'deixem seleccionat el element creat
            'vaElementSel = pPGElement.Id
            RaiseEvent PGElement_Added(pPGElement)
            Return PGReturnCode.PGAdded
        End If

    End Function

    Public Function PGElementDeleteById(ByVal pId As String) As PGReturnCode
        Dim auxPGElement As PGElement

        If vpLlistaElements.ContainsKey(pId) Then
            auxPGElement = vpLlistaElements(pId)
            vpLlistaElements.Remove(pId)
            If vpLlistaElementsData.ContainsValue(pId) Then
                vpLlistaElementsData.RemoveAt(vpLlistaElementsData.IndexOfValue(pId))
            End If

            If vaElementSel = pId Then vaElementSel = ""

            If VisualizationMode = PGMode.Day Then
                vaReformateja = True
                Invalidate()
            Else
                GridPanel.Invalidate()
            End If
            RaiseEvent PGElement_Deleted(auxPGElement)
            Return PGReturnCode.PGDeleted
        Else
            Return PGReturnCode.PGError
        End If

    End Function

    Public Function PGElementSelectById(ByVal pId As String) As PGReturnCode
        Dim auxPGElement As PGElement

        If vpLlistaElements.ContainsKey(pId) Then
            vaElementSel = pId
            auxPGElement = vpLlistaElements(pId)

            If VisualizationMode = PGMode.Day Then
                vaReformateja = True
                Invalidate()
            Else
                GridPanel.Invalidate()
            End If
            RaiseEvent PGElement_Clicked(auxPGElement)
            Return PGReturnCode.PGSelected
        Else
            Return PGReturnCode.PGError
        End If

    End Function

    Public Function PGElementChangeId(ByVal pId As String, ByVal pNewId As String) As PGReturnCode
        Dim auxPGElement As PGElement

        If vpLlistaElements.ContainsKey(pId) And vpLlistaElementsData.ContainsValue(pId) Then
            auxPGElement = vpLlistaElements(pId)
            auxPGElement.Id = pNewId
            vpLlistaElements.Remove(pId)
            vpLlistaElements.Add(pNewId, auxPGElement)
            vpLlistaElementsData.RemoveAt(vpLlistaElementsData.IndexOfValue(pId))
            'ordenem per inici de l'element ascendent i durada descendent
            vpLlistaElementsData.Add(ClauElementsData(auxPGElement), auxPGElement.Id)
            If vaElementSel = pId Then vaElementSel = pNewId
            RaiseEvent PGElement_Updated(auxPGElement)
            Return PGReturnCode.PGUpdated
        Else
            Return PGReturnCode.PGError
        End If

    End Function

    Public Function PGElementReadById(ByVal pId As String) As PGElement

        If vpLlistaElements.ContainsKey(pId) Then
            Return New PGElement(vpLlistaElements(pId))
        Else
            Return Nothing
        End If

    End Function

    Public Function PGElementsGetList() As List(Of PGElement)
        Dim auxList As New List(Of PGElement)

        For Each element In vpLlistaElementsData
            If vpLlistaElements.ContainsKey(element.Value) Then
                auxList.Add(vpLlistaElements(element.Value))
            End If
        Next
        Return auxList

    End Function

    Public Sub PGElementsClearList()

        If vpLlistaElements IsNot Nothing Then vpLlistaElements.Clear()
        If vpLlistaElementsData IsNot Nothing Then vpLlistaElementsData.Clear()
        If vpLlistaRecursosDia IsNot Nothing Then vpLlistaRecursosDia.Clear()
        vaElementDrop = ""
        'vaElementSel = ""   'quan recarreguem periode pot ser que l'element encara hi sigui
        Invalidate()
        Update()

    End Sub

    Public Function PGPrint(ByVal bottomtxt As String) As CrystalDecisions.CrystalReports.Engine.ReportClass
        Dim auxRpt As New crpPlaniGrid
        Dim DataSetInformes As New dsReports

        auxRpt.SummaryInfo.ReportTitle = "PlaniGrid"
        auxRpt.SummaryInfo.ReportComments = bottomtxt
        Dim DataTableInformes As dsReports.dtGridDataTable = DataSetInformes.Tables("dtGrid")
        If prModeVisualitzacio = PGMode.Day Then
            OmplirTaulaGridDia(DataTableInformes)
        Else
            OmplirTaulaGridNoDia(DataTableInformes)
        End If
        auxRpt.SetDataSource(DataSetInformes.Tables(DataTableInformes.TableName))

        Return auxRpt

    End Function

    Private Sub OmplirTaulaGridDia(ByRef pdt As dsReports.dtGridDataTable)
        Dim dr As DataRow
        Dim auxtxt As String
        Dim auxDataDia1 As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)
        Dim auxElement As PGElement
        Dim grupsdia As Integer
        Dim dtini As Date
        Dim dtfi As Date

        pdt.Clear()

        'calcular quants grups necessitem = (nro. recursos \ 7 recursos per pagina) + 1 --> grups de 1+6,7,7,...
        grupsdia = ((vpNroColumnes - 1) \ 7) + 1

        'mentre hi hagi grups
        For n = 1 To grupsdia

            'imprimir capç cols grup n
            dr = pdt.NewRow
            For col = 1 To 8
                auxtxt = "c" & CStr(col)
                If col = 1 Then
                    'saltar pagina (si no primer). controlat per pos(c1,5)="(...)" en report
                    If n > 1 Then
                        dr("c1") = "(...) "
                    End If
                    dr("c1") = dr("c1") & T(DiesSetmana(DisplayDate.DayOfWeek)) & " " & DisplayDate.ToString("d")
                Else
                    'pintem els recursos
                    If ((n - 1) * 7 + (col - 1)) <= vpLlistaRecursosDia.Count Then
                        dr(auxtxt) = vpLlistaRecursosDia.ElementAt(((n - 1) * 7 + (col - 1)) - 1).Value.Name
                        If dr(auxtxt) = "" Then dr(auxtxt) = Chr(216) 'conjunt buit
                    End If
                End If
            Next
            pdt.Rows.Add(dr)

            'imprimir pagina grup n
            For fil = 0 To vpNroFiles - 1
                dr = pdt.NewRow

                'obtenim el periode horari de la linia
                dtini = DisplayDate.Date + (prHoraIniciActivitat + New TimeSpan(0, MinutesGap * fil, 0))
                'dtfi = DisplayDate.Date + (prHoraIniciActivitat + New TimeSpan(0, MinutesGap * (fil + 1) - 1, 0))
                dtfi = DisplayDate.Date + (prHoraIniciActivitat + New TimeSpan(0, MinutesGap * (fil + 1), 0))
                dr("c1") = Mid(dtini.TimeOfDay.ToString(), 1, 5)
                'pintem elements del tram horari
                For Each element In vpLlistaElementsData
                    auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                    'comprovem que l'inici de la feina intersecta el periode horari de la linia
                    'o és el primer periode del dia i l'inici de la feina és anterior
                    If auxElement.Starts >= dtini And auxElement.Starts < dtfi _
                    Or (fil = 0 And auxElement.Starts < dtini And auxElement.Ends > dtini) Then
                        If fil = 0 And auxElement.Starts < dtini And auxElement.Ends > dtini Then
                            If auxElement.Ends.Date > dtini.Date Then
                                'si comença un dia anterior, posem hora inici activitat
                                If auxElement.Starts.Date < dtini.Date Then
                                    auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                Else
                                    auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                End If
                            Else
                                'si comença un dia anterior, posem hora inici activitat
                                If auxElement.Starts.Date < dtini.Date Then
                                    auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                Else
                                    auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                End If
                            End If
                        Else
                            If auxElement.Ends.Date > dtini.Date Then
                                auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                            Else
                                auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                            End If
                        End If
                        'si l'element no té recursos, només el posem a columna 1 si estem en grup 1
                        If auxElement.Resources.Count = 0 Then
                            If n = 1 Then
                                dr("c2") = IIf(dr("c2") = "", auxtxt, dr("c2") & vbCrLf & auxtxt)
                            End If
                        Else 'si té recursos els posem a les columnes corresponents
                            'mirem per cada columna si cal afegir l'element
                            For col = 2 To 8
                                If ((n - 1) * 7 + (col - 1)) <= vpLlistaRecursosDia.Count Then
                                    If auxElement.Resources.ContainsKey(vpLlistaRecursosDia.ElementAt(((n - 1) * 7 + (col - 1)) - 1).Value.Name) Then
                                        dr("c" + CStr(col)) = auxtxt
                                    End If
                                End If
                            Next
                        End If
                    End If
                    'parem quan passem del periode 
                    If auxElement.Starts > vaDataHoraFiPer Then
                        Exit For
                    End If
                Next
                pdt.Rows.Add(dr)
            Next
        Next

    End Sub

    Private Sub OmplirTaulaGridNoDia(ByRef pdt As dsReports.dtGridDataTable)
        Dim dr As DataRow
        Dim auxtxt As String
        Dim auxdata As Date
        Dim dataini As Date
        Dim datafi As Date
        Dim auxDataDia1 As DateTime = DateSerial(Year(DisplayDate), Month(DisplayDate), 1)
        Dim auxElement As PGElement
        Dim auxint As Integer
        Dim auxint2 As Integer
        Dim auxts As TimeSpan
        Dim auxts2 As TimeSpan
        Dim c As Char = Chr(3)  'end of text ascii

        pdt.Clear()

        'OMPLIR TAULA COM S'OMPLE GRID

        'CAPÇALERA COLUMNES
        dr = pdt.NewRow

        For i = 0 To vpNroColumnes

            auxtxt = "c" & CStr(i + 1)

            Select Case VisualizationMode

                Case PGMode.Week
                    If i = 0 Then
                        dr("c1") = T("Setmana") & vbCrLf & T("del") & " " & vaDataHoraIniPer.ToString("d") & vbCrLf & T("al") & " " & vaDataHoraFiPer.ToString("d")
                    Else
                        'obtenim la data del dia i de la setmana de la DataActual
                        auxdata = DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                        dr(auxtxt) = vbCrLf & vbCrLf & T(DiesSetmanaCurts(auxdata.DayOfWeek)) & " " & auxdata.Day.ToString & " " & T(MesosCurts(auxdata.Month - 1)) & c
                    End If

                Case PGMode.Month
                    If i = 0 Then
                        dr("c1") = T(Mesos(DisplayDate.Month - 1)) & " " & DisplayDate.ToString("yyyy")
                    Else
                        'obtenim la data del dia i de la setmana de la DataActual
                        auxdata = DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                        dr(auxtxt) = T(DiesSetmana(DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate).DayOfWeek))
                    End If

                Case PGMode.Resource
                    If i = 0 Then
                        dr("c1") = T("Recurs") & " " & IIf(vaRecursSel = "", Chr(216), "'" & vaRecursSel & "'")
                    Else
                        'obtenim la data del dia i de la setmana de la DataActual
                        auxdata = DateAdd(DateInterval.Day, (i - 1) + IIf(DayOfWeek.Monday - DisplayDate.DayOfWeek > 0, DayOfWeek.Monday - DisplayDate.DayOfWeek - 7, DayOfWeek.Monday - DisplayDate.DayOfWeek), DisplayDate)
                        dr(auxtxt) = T(DiesSetmanaCurts(auxdata.DayOfWeek)) & " " & auxdata.Day.ToString & " " & T(MesosCurts(auxdata.Month - 1))
                    End If

            End Select

        Next
        pdt.Rows.Add(dr)

        'CAPÇALERA FILES I ELEMENTS DE CADASCUNA
        For i = 0 To vpNroFiles - 1
            dr = pdt.NewRow

            Select Case VisualizationMode
                Case PGMode.Month
                    'pintem les dates del primer dia de cada setmana al mig de la fila
                    'obtenim les dates limit de la setmana 
                    dataini = DateAdd(DateInterval.Day, 7 * i + IIf(DayOfWeek.Monday - auxDataDia1.DayOfWeek > 0, DayOfWeek.Monday - auxDataDia1.DayOfWeek - 7, DayOfWeek.Monday - auxDataDia1.DayOfWeek), auxDataDia1)
                    datafi = DateAdd(DateInterval.Day, 7, dataini)
                    dr("c1") = dataini.ToString("dd/MM")
                    'pintem elements de la setmana
                    For Each element In vpLlistaElementsData
                        auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                        If auxElement.Ends > dataini And auxElement.Starts <= datafi Then
                            auxint = DateDiff(DateInterval.Day, dataini, auxElement.Starts.Date) + 2
                            auxint2 = DateDiff(DateInterval.Day, dataini, auxElement.Ends.Date) + 2
                            For j = 2 To 8
                                If j >= auxint And j <= auxint2 Then
                                    If j = auxint Then
                                        auxtxt = auxElement.Starts.ToString("HH:mm") & " " & auxElement.Name
                                    Else
                                        auxtxt = Me.StartTimeOfDay & " " & auxElement.Name
                                    End If
                                    dr("c" + CStr(j)) = IIf(dr("c" & CStr(j)) = "", auxtxt, dr("c" & CStr(j)) & vbCrLf & auxtxt)
                                End If
                            Next
                        End If
                        'parem quan passem del periode 
                        If auxElement.Starts > datafi Then
                            Exit For
                        End If
                    Next

                Case PGMode.Week
                    'obtenim el periode horari de la linia
                    auxts = prHoraIniciActivitat + New TimeSpan(0, MinutesGap * i, 0)
                    auxts2 = auxts + New TimeSpan(0, MinutesGap, 0)
                    dr("c1") = Mid(auxts.ToString(), 1, 5)
                    'pintem elements del tram horari
                    For Each element In vpLlistaElementsData
                        auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                        'comprovem que intersecta la setmana
                        If auxElement.Ends > vaDataHoraIniPer And auxElement.Starts <= vaDataHoraFiPer Then
                            'determinem les columnes (dies) que abarca l'element
                            auxint = DateDiff(DateInterval.Day, vaDataHoraIniPer.Date, auxElement.Starts.Date) + 2
                            auxint2 = DateDiff(DateInterval.Day, vaDataHoraIniPer.Date, auxElement.Ends.Date) + 2
                            'mirem per cada columna (dia) si cal afegir l'element
                            For j = 2 To 8
                                If j >= auxint And j <= auxint2 Then
                                    'en la data en que comença l'element, hora inici element
                                    If j = auxint Then
                                        If (auxElement.Starts.TimeOfDay >= auxts Or i = 0) And auxElement.Starts.TimeOfDay < auxts2 Then
                                            If auxint = auxint2 Then
                                                'auxtxt = auxElement.Name & " (" & Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & " " & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & ")"
                                                auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                            Else
                                                'auxtxt = auxElement.Name & " (" & Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & " " & Mid(prHoraFiActivitat.ToString(), 1, 5) & ")"
                                                auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                            End If
                                            dr("c" + CStr(j)) = IIf(dr("c" & CStr(j)) = "", auxtxt, dr("c" & CStr(j)) & vbCrLf & auxtxt)
                                        End If
                                    Else
                                        'en la resta, hora inici activitat
                                        If i = 0 Then
                                            If j = auxint2 Then
                                                'auxtxt = auxElement.Name & " (" & Mid(prHoraIniciActivitat.ToString(), 1, 5) & " " & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & ")"
                                                auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                            Else
                                                'auxtxt = auxElement.Name & " (" & Mid(prHoraIniciActivitat.ToString(), 1, 5) & " " & Mid(prHoraFiActivitat.ToString(), 1, 5) & ")"
                                                auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                            End If
                                            dr("c" + CStr(j)) = IIf(dr("c" & CStr(j)) = "", auxtxt, dr("c" & CStr(j)) & vbCrLf & auxtxt)
                                        End If
                                    End If

                                End If
                            Next
                        End If
                        'parem quan passem del periode 
                        If auxElement.Starts > vaDataHoraFiPer Then
                            Exit For
                        End If
                    Next

                Case PGMode.Resource
                    'obtenim el periode horari de la linia
                    auxts = prHoraIniciActivitat + New TimeSpan(0, MinutesGap * i, 0)
                    auxts2 = auxts + New TimeSpan(0, MinutesGap, 0)
                    dr("c1") = Mid(auxts.ToString(), 1, 5)
                    'pintem elements del tram horari
                    For Each element In vpLlistaElementsData
                        auxElement = New PGElement(vpLlistaElements.Item(element.Value))
                        'comprovem que intersecta la setmana
                        If auxElement.Ends > vaDataHoraIniPer And auxElement.Starts <= vaDataHoraFiPer Then
                            'comprovem que l'element té el recurs
                            auxint = 0
                            For Each e In auxElement.Resources
                                If e.Key = vaRecursSel Then
                                    auxint = 1
                                    Exit For
                                End If
                            Next
                            If auxint = 1 Or (auxElement.Resources.Count = 0 And vaRecursSel = "") Then
                                'determinem les columnes (dies) que abarca l'element
                                auxint = DateDiff(DateInterval.Day, vaDataHoraIniPer.Date, auxElement.Starts.Date) + 2
                                auxint2 = DateDiff(DateInterval.Day, vaDataHoraIniPer.Date, auxElement.Ends.Date) + 2
                                'mirem per cada columna si cal afegir l'element
                                For j = 2 To 8
                                    If j >= auxint And j <= auxint2 Then
                                        'en la data en que comença l'element, hora inici element
                                        If j = auxint Then
                                            If (auxElement.Starts.TimeOfDay >= auxts Or i = 0) And auxElement.Starts.TimeOfDay < auxts2 Then
                                                If auxint = auxint2 Then
                                                    'auxtxt = auxElement.Name & " (" & Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & " " & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & ")"
                                                    auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                                Else
                                                    'auxtxt = auxElement.Name & " (" & Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & " " & Mid(prHoraFiActivitat.ToString(), 1, 5) & ")"
                                                    auxtxt = Mid(auxElement.Starts.TimeOfDay.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                                End If
                                                dr("c" + CStr(j)) = IIf(dr("c" & CStr(j)) = "", auxtxt, dr("c" & CStr(j)) & vbCrLf & auxtxt)
                                            End If
                                        Else
                                            'en la resta, hora inici activitat
                                            If i = 0 Then
                                                If j = auxint2 Then
                                                    'auxtxt = auxElement.Name & " (" & Mid(prHoraIniciActivitat.ToString(), 1, 5) & " " & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & ")"
                                                    auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(auxElement.Ends.TimeOfDay.ToString(), 1, 5) & " " & auxElement.Name
                                                Else
                                                    'auxtxt = auxElement.Name & " (" & Mid(prHoraIniciActivitat.ToString(), 1, 5) & " " & Mid(prHoraFiActivitat.ToString(), 1, 5) & ")"
                                                    auxtxt = Mid(prHoraIniciActivitat.ToString(), 1, 5) & "-" & Mid(prHoraFiActivitat.ToString(), 1, 5) & " " & auxElement.Name
                                                End If
                                                dr("c" + CStr(j)) = IIf(dr("c" & CStr(j)) = "", auxtxt, dr("c" & CStr(j)) & vbCrLf & auxtxt)
                                            End If
                                        End If

                                    End If
                                Next
                            End If
                        End If

                        'parem quan passem del periode 
                        If auxElement.Starts > vaDataHoraFiPer Then
                            Exit For
                        End If
                    Next

            End Select

            pdt.Rows.Add(dr)
        Next

    End Sub

    Private Function ValidaSolapaments(ByVal pPGElement As PGElement) As Boolean
        Dim auxElement As PGElement
        'Dim auxclau As String = ClauElementsData(pPGElement)

        For Each element In vpLlistaElementsData
            auxElement = New PGElement(vpLlistaElements.Item(element.Value))
            'si es solapen els elements
            If auxElement.Id <> pPGElement.Id And auxElement.Starts < pPGElement.Ends And auxElement.Ends > pPGElement.Starts Then
                'validem recursos comuns
                For Each rnou In pPGElement.Resources
                    For Each rexistent In auxElement.Resources
                        If rnou.Key <> "" And rnou.Key = rexistent.Key Then
                            RaiseEvent PGMessage(T("INFORMACIÓ"), T("Element #") & pPGElement.Id & T(": Solapament de recurs '") & rnou.Key & T("' amb Element #") & auxElement.Id)
                            Return False
                        End If
                    Next
                Next
            End If
            If auxElement.Starts > pPGElement.Ends Then Exit For
        Next

        Return True

    End Function

    Public Sub New()

        ' Llamada necesaria para el Diseñador de Windows Forms.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Me.GridPanel.AllowDrop = True
        Me.BorderStyle = Windows.Forms.BorderStyle.Fixed3D
        Me.HeaderBackColor = System.Drawing.SystemColors.ControlDark
    End Sub

#End Region

End Class
