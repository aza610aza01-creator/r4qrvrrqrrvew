using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PremiumLivingFurnitureWinForms;

public class MaterialManagementForm : UserControl
{
    readonly TextBox search = new() { Width = 280 };
    readonly DataGridView grid = new();
    readonly Color bg = Color.FromArgb(7, 11, 27), card = Color.FromArgb(19, 23, 48), border = Color.FromArgb(92, 100, 132);

    public MaterialManagementForm()
    {
        Dock = DockStyle.Fill;
        BackColor = bg;
        Font = new Font("Segoe UI", 10);
        Build();
        try { Database.EnsureMaterialsTable(); LoadData(); }
        catch (Exception ex) { MessageBox.Show("Materials table error: " + ex.Message); }
    }

    void Build()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, BackColor = bg, Padding = new Padding(24) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(new Label
        {
            Text = "Material Management\r\nManage materials and connect each material to supplier/product tables.",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            BackColor = bg,
            Font = new Font("Segoe UI", 20, FontStyle.Bold)
        }, 0, 0);

        var toolbarCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14), Margin = new Padding(0, 0, 0, 12) };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.White, WrapContents = false };
        toolbar.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 10, 6, 0), ForeColor = Theme.Muted, BackColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
        search.Margin = new Padding(0, 6, 14, 6);
        search.TextChanged += (_, _) => LoadData();
        toolbar.Controls.Add(search);

        var add = Theme.PrimaryButton("Add New", 115);
        var edit = Theme.SecondaryButton("Modify Row", 125);
        var delete = Theme.SecondaryButton("Delete", 95);
        var refresh = Theme.SecondaryButton("Refresh", 105);
        add.Click += (_, _) => AddMaterial();
        edit.Click += (_, _) => EditMaterial();
        delete.Click += (_, _) => DeleteMaterial();
        refresh.Click += (_, _) => LoadData();
        toolbar.Controls.Add(add);
        toolbar.Controls.Add(edit);
        toolbar.Controls.Add(delete);
        toolbar.Controls.Add(refresh);
        toolbarCard.Controls.Add(toolbar);
        root.Controls.Add(toolbarCard, 0, 1);

        var tableCard = new RoundedPanel { Dock = DockStyle.Fill, BackColor = card, BorderColor = border, Radius = 20, Padding = new Padding(18) };
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = card;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 42;
        grid.RowTemplate.Height = 46;
        grid.GridColor = border;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersDefaultCellStyle.BackColor = card;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        grid.DefaultCellStyle.BackColor = card;
        grid.DefaultCellStyle.ForeColor = Color.White;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(32, 38, 73);
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditMaterial(); };
        tableCard.Controls.Add(grid);
        root.Controls.Add(tableCard, 0, 2);
        Controls.Add(root);
    }

    void LoadData()
    {
        try
        {
            grid.DataSource = Database.GetMaterials(search.Text.Trim());
            HideTechnicalColumns();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Material Management"); }
    }

    void HideTechnicalColumns()
    {
        if (grid.Columns.Contains("SupplierId")) grid.Columns["SupplierId"].Visible = false;
        if (grid.Columns.Contains("ProductId")) grid.Columns["ProductId"].Visible = false;
        if (grid.Columns.Contains("PhysicalQuantity")) grid.Columns["PhysicalQuantity"].HeaderText = "Physical Quantity";
        if (grid.Columns.Contains("SupplierName")) grid.Columns["SupplierName"].HeaderText = "Supplier Name";
        if (grid.Columns.Contains("ProductName")) grid.Columns["ProductName"].HeaderText = "Product Name";
        grid.ClearSelection();
    }

    void AddMaterial()
    {
        using var form = new MaterialEditForm();
        if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
        Database.AddMaterial(form.MaterialName, form.PhysicalQuantity, form.Unit, form.SupplierId, form.ProductId);
        LoadData();
    }

    void EditMaterial()
    {
        if (grid.CurrentRow == null || !grid.Columns.Contains("Id")) { MessageBox.Show("Please select one material row first.", "Modify Row"); return; }
        using var form = new MaterialEditForm(grid.CurrentRow);
        if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
        Database.UpdateMaterial(form.MaterialId, form.MaterialName, form.PhysicalQuantity, form.Unit, form.SupplierId, form.ProductId);
        LoadData();
    }

    void DeleteMaterial()
    {
        if (grid.CurrentRow == null || !grid.Columns.Contains("Id")) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);
        if (MessageBox.Show("Delete selected material row?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        Database.DeleteRecord("Materials", id);
        LoadData();
    }
}

public class MaterialEditForm : Form
{
    readonly TextBox materialName = new();
    readonly NumericUpDown qty = new() { Minimum = 0, Maximum = 999999 };
    readonly TextBox unit = new();
    readonly ComboBox supplier = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    readonly ComboBox product = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    public int MaterialId { get; private set; }
    public string MaterialName => materialName.Text.Trim();
    public int PhysicalQuantity => (int)qty.Value;
    public string Unit => unit.Text.Trim();
    public int SupplierId => supplier.SelectedItem is LookupItem s ? s.Id : 0;
    public int ProductId => product.SelectedItem is LookupItem p ? p.Id : 0;

    public MaterialEditForm(DataGridViewRow? row = null)
    {
        MaterialId = row == null ? 0 : Convert.ToInt32(row.Cells["Id"].Value);
        Text = MaterialId == 0 ? "Add Material" : "Modify Material Row";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(820, 520);
        MinimumSize = new Size(760, 500);
        Font = Theme.DefaultFont;
        BackColor = Theme.AppBg;
        Build();
        LoadLookups();
        if (row != null) LoadExisting(row);
    }

    void Build()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(26), AutoScroll = true, BackColor = Theme.AppBg };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(root, "Material Name *", materialName);
        AddRow(root, "Physical Quantity", qty);
        AddRow(root, "Unit *", unit);
        AddRow(root, "Supplier Relationship", supplier);
        AddRow(root, "Product Relationship", product);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, BackColor = Theme.AppBg };
        var save = Theme.PrimaryButton(MaterialId == 0 ? "Add Material" : "Save Changes", 140);
        var cancel = Theme.SecondaryButton("Cancel", 110);
        save.Click += (_, _) => Save();
        cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(save);
        buttons.Controls.Add(cancel);
        int row = root.RowCount++;
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        root.SetColumnSpan(buttons, 2);
        root.Controls.Add(buttons, 0, row);
        Controls.Add(root);
    }

    static void AddRow(TableLayoutPanel root, string label, Control input)
    {
        int row = root.RowCount++;
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, row);
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 8, 0, 8);
        root.Controls.Add(input, 1, row);
    }

    void LoadLookups()
    {
        supplier.Items.Clear(); supplier.Items.Add(new LookupItem(0, "0 - No supplier link")); foreach (var item in Database.GetLookup("Suppliers")) supplier.Items.Add(item); supplier.SelectedIndex = 0;
        product.Items.Clear(); product.Items.Add(new LookupItem(0, "0 - No product link")); foreach (var item in Database.GetLookup("Products")) product.Items.Add(item); product.SelectedIndex = 0;
    }

    void LoadExisting(DataGridViewRow row)
    {
        materialName.Text = Cell(row, "MaterialName");
        unit.Text = Cell(row, "Unit");
        if (int.TryParse(Cell(row, "PhysicalQuantity"), out int q) && q >= qty.Minimum && q <= qty.Maximum) qty.Value = q;
        SelectLookup(supplier, Cell(row, "SupplierId"));
        SelectLookup(product, Cell(row, "ProductId"));
    }

    void Save()
    {
        if (string.IsNullOrWhiteSpace(materialName.Text)) { MessageBox.Show("Please enter material name."); return; }
        if (string.IsNullOrWhiteSpace(unit.Text)) { MessageBox.Show("Please enter unit."); return; }
        DialogResult = DialogResult.OK;
    }

    static string Cell(DataGridViewRow row, string column)
    {
        if (!row.DataGridView.Columns.Contains(column)) return "";
        object? value = row.Cells[column].Value;
        return value == null || value == DBNull.Value ? "" : Convert.ToString(value) ?? "";
    }

    static void SelectLookup(ComboBox combo, string idText)
    {
        if (!int.TryParse(idText, out int id)) return;
        for (int i = 0; i < combo.Items.Count; i++) if (combo.Items[i] is LookupItem item && item.Id == id) { combo.SelectedIndex = i; return; }
    }
}

public class RoundedPanel : Panel
{
    public Color BorderColor { get; set; } = Color.Gray;
    public int Radius { get; set; } = 20;
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = new GraphicsPath();
        int r = Radius;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        path.AddArc(rect.X, rect.Y, r, r, 180, 90);
        path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
        path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
        path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
        path.CloseFigure();
        using var pen = new Pen(BorderColor, 1);
        e.Graphics.DrawPath(pen, path);
    }
}
