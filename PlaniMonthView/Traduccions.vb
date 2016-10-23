'***********************************
'TRADUCCIO
'***********************************
Friend Module Traduccions

    'per traduir textos
    Friend vpEN As New Dictionary(Of String, String) 'al anglès
    Friend vpES As New Dictionary(Of String, String) 'al castellà

    Sub CarregaT()
        'omplim les taules de traduccio
        vpEN.Clear()
        vpES.Clear()

        vpEN.Add("Abril", "April")
        vpEN.Add("Agost", "August")
        vpEN.Add("dc", "Wed")
        vpEN.Add("Desembre", "December")
        vpEN.Add("dg", "Sun")
        vpEN.Add("dj", "Thu")
        vpEN.Add("dl", "Mon")
        vpEN.Add("dm", "Tue")
        vpEN.Add("ds", "Sat")
        vpEN.Add("dv", "Fri")
        vpEN.Add("Febrer", "February")
        vpEN.Add("Gener", "January")
        vpEN.Add("Juliol", "July")
        vpEN.Add("Juny", "June")
        vpEN.Add("Maig", "May")
        vpEN.Add("Març", "March")
        vpEN.Add("Novembre", "November")
        vpEN.Add("Octubre", "October")
        vpEN.Add("Setembre", "September")

        vpES.Add("Agost", "Agosto")
        vpES.Add("dc", "mié")
        vpES.Add("Desembre", "Diciembre")
        vpES.Add("dg", "dom")
        vpES.Add("dj", "jue")
        vpES.Add("dl", "lun")
        vpES.Add("dm", "mar")
        vpES.Add("ds", "sáb")
        vpES.Add("dv", "vie")
        vpES.Add("Febrer", "Febrero")
        vpES.Add("Gener", "Enero")
        vpES.Add("Juliol", "Julio")
        vpES.Add("Juny", "Junio")
        vpES.Add("Maig", "Mayo")
        vpES.Add("Març", "Marzo")
        vpES.Add("Novembre", "Noviembre")
        vpES.Add("Setembre", "Septiembre")

    End Sub

End Module


