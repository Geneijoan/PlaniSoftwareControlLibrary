<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PlaniMonthView
    Inherits System.Windows.Forms.UserControl

    'UserControl1 reemplaza a Dispose para limpiar la lista de componentes.
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.MonthGrid = New System.Windows.Forms.DataGridView
        Me.dl = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.dm = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.dc = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.dj = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.dv = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ds = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.dg = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.AnyMes = New System.Windows.Forms.Label
        Me.MesAnt = New System.Windows.Forms.Button
        Me.MesPos = New System.Windows.Forms.Button
        CType(Me.MonthGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MonthGrid
        '
        Me.MonthGrid.AllowUserToAddRows = False
        Me.MonthGrid.AllowUserToDeleteRows = False
        Me.MonthGrid.AllowUserToResizeColumns = False
        Me.MonthGrid.AllowUserToResizeRows = False
        Me.MonthGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.MonthGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.MonthGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.MonthGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.MonthGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.MonthGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.MonthGrid.ColumnHeadersHeight = 22
        Me.MonthGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.MonthGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.dl, Me.dm, Me.dc, Me.dj, Me.dv, Me.ds, Me.dg})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.MonthGrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.MonthGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.MonthGrid.GridColor = System.Drawing.SystemColors.Window
        Me.MonthGrid.Location = New System.Drawing.Point(0, 20)
        Me.MonthGrid.MultiSelect = False
        Me.MonthGrid.Name = "MonthGrid"
        Me.MonthGrid.ReadOnly = True
        Me.MonthGrid.RowHeadersVisible = False
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.MonthGrid.RowsDefaultCellStyle = DataGridViewCellStyle3
        Me.MonthGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.MonthGrid.RowTemplate.Height = 17
        Me.MonthGrid.RowTemplate.ReadOnly = True
        Me.MonthGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.MonthGrid.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.MonthGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.MonthGrid.ShowCellErrors = False
        Me.MonthGrid.ShowCellToolTips = False
        Me.MonthGrid.ShowEditingIcon = False
        Me.MonthGrid.ShowRowErrors = False
        Me.MonthGrid.Size = New System.Drawing.Size(232, 136)
        Me.MonthGrid.TabIndex = 0
        '
        'dl
        '
        Me.dl.Frozen = True
        Me.dl.HeaderText = "dl"
        Me.dl.MaxInputLength = 2
        Me.dl.Name = "dl"
        Me.dl.ReadOnly = True
        Me.dl.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dl.Width = 33
        '
        'dm
        '
        Me.dm.Frozen = True
        Me.dm.HeaderText = "dm"
        Me.dm.MaxInputLength = 2
        Me.dm.Name = "dm"
        Me.dm.ReadOnly = True
        Me.dm.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dm.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dm.Width = 33
        '
        'dc
        '
        Me.dc.Frozen = True
        Me.dc.HeaderText = "dc"
        Me.dc.MaxInputLength = 2
        Me.dc.Name = "dc"
        Me.dc.ReadOnly = True
        Me.dc.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dc.Width = 33
        '
        'dj
        '
        Me.dj.Frozen = True
        Me.dj.HeaderText = "dj"
        Me.dj.MaxInputLength = 2
        Me.dj.Name = "dj"
        Me.dj.ReadOnly = True
        Me.dj.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dj.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dj.Width = 33
        '
        'dv
        '
        Me.dv.Frozen = True
        Me.dv.HeaderText = "dv"
        Me.dv.MaxInputLength = 2
        Me.dv.Name = "dv"
        Me.dv.ReadOnly = True
        Me.dv.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dv.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dv.Width = 33
        '
        'ds
        '
        Me.ds.Frozen = True
        Me.ds.HeaderText = "ds"
        Me.ds.MaxInputLength = 2
        Me.ds.Name = "ds"
        Me.ds.ReadOnly = True
        Me.ds.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ds.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ds.Width = 33
        '
        'dg
        '
        Me.dg.Frozen = True
        Me.dg.HeaderText = "dg"
        Me.dg.MaxInputLength = 2
        Me.dg.Name = "dg"
        Me.dg.ReadOnly = True
        Me.dg.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dg.Width = 33
        '
        'AnyMes
        '
        Me.AnyMes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AnyMes.ForeColor = System.Drawing.SystemColors.Window
        Me.AnyMes.Location = New System.Drawing.Point(-2, 0)
        Me.AnyMes.Name = "AnyMes"
        Me.AnyMes.Size = New System.Drawing.Size(234, 21)
        Me.AnyMes.TabIndex = 1
        Me.AnyMes.Text = "Any i Mes"
        Me.AnyMes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'MesAnt
        '
        Me.MesAnt.Location = New System.Drawing.Point(1, 0)
        Me.MesAnt.Name = "MesAnt"
        Me.MesAnt.Size = New System.Drawing.Size(30, 20)
        Me.MesAnt.TabIndex = 2
        Me.MesAnt.Text = "<"
        Me.MesAnt.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.MesAnt.UseVisualStyleBackColor = True
        '
        'MesPos
        '
        Me.MesPos.Location = New System.Drawing.Point(199, 0)
        Me.MesPos.Name = "MesPos"
        Me.MesPos.Size = New System.Drawing.Size(30, 20)
        Me.MesPos.TabIndex = 3
        Me.MesPos.Text = ">"
        Me.MesPos.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.MesPos.UseVisualStyleBackColor = True
        '
        'PlaniMonthView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.SystemColors.ControlDark
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.MesPos)
        Me.Controls.Add(Me.MesAnt)
        Me.Controls.Add(Me.AnyMes)
        Me.Controls.Add(Me.MonthGrid)
        Me.MaximumSize = New System.Drawing.Size(234, 150)
        Me.MinimumSize = New System.Drawing.Size(234, 150)
        Me.Name = "PlaniMonthView"
        Me.Size = New System.Drawing.Size(230, 146)
        CType(Me.MonthGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MonthGrid As System.Windows.Forms.DataGridView
    Friend WithEvents AnyMes As System.Windows.Forms.Label
    Friend WithEvents MesAnt As System.Windows.Forms.Button
    Friend WithEvents MesPos As System.Windows.Forms.Button
    Friend WithEvents dl As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dm As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dc As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dj As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dv As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ds As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dg As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
