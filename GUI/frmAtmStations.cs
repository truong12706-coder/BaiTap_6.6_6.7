using BUS;
using DTO;
using System.Drawing;
namespace GUI;
public class frmAtmStations : Form
{
    private readonly AtmStationBUS bus=new();
    private readonly TextBox txtId=new(), txtAddress=new(), txtCash=new(), txtImage=new();
    private readonly ComboBox cboStatus=new() { DropDownStyle=ComboBoxStyle.DropDownList };
    private readonly DataGridView grid=new() { Dock=DockStyle.Fill, ReadOnly=true, AutoGenerateColumns=true, SelectionMode=DataGridViewSelectionMode.FullRowSelect, MultiSelect=false };
    public frmAtmStations()
    {
        Text="Quản lý danh sách trạm ATM"; Width=1000; Height=620;
        cboStatus.Items.AddRange(new object[]{"Hoạt động","Bảo trì","Ngừng hoạt động"}); cboStatus.SelectedIndex=0;
        var top=new TableLayoutPanel { Dock=DockStyle.Top, Height=175, ColumnCount=4, RowCount=4, Padding=new Padding(10) };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,120)); top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,120)); top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
        AddField(top,"Mã trạm",txtId,0,0); AddField(top,"Địa chỉ",txtAddress,1,0); AddField(top,"Tồn quỹ (VNĐ)",txtCash,0,1); AddField(top,"Trạng thái",cboStatus,1,1);
        txtImage.ReadOnly=true; AddField(top,"Ảnh hiện trường",txtImage,0,2);
        var btnImage=new Button{Text="Chọn ảnh..."}; top.Controls.Add(btnImage,2,2); top.SetColumnSpan(btnImage,2);
        btnImage.Click+=(_,_)=>{ using var dlg=new OpenFileDialog{Filter="Ảnh|*.png;*.jpg;*.jpeg;*.bmp"}; if(dlg.ShowDialog()==DialogResult.OK) txtImage.Text=dlg.FileName; };
        var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill}; var add=new Button{Text="Thêm"}; var update=new Button{Text="Sửa"}; var del=new Button{Text="Xóa"}; var clear=new Button{Text="Làm mới"};
        buttons.Controls.AddRange(new Control[]{add,update,del,clear}); top.Controls.Add(buttons,0,3); top.SetColumnSpan(buttons,4);
        add.Click+=(_,_)=>Run(()=>{bus.Add(ReadForm()); RefreshGrid();});
        update.Click+=(_,_)=>Run(()=>{bus.Update(ReadForm()); RefreshGrid();});
        del.Click+=(_,_)=>Run(()=>{bus.Delete(txtId.Text.Trim()); RefreshGrid();});
        clear.Click+=(_,_)=>ClearForm();
        grid.SelectionChanged+=(_,_)=>{ if(grid.CurrentRow?.DataBoundItem is AtmStation s){txtId.Text=s.MaTram;txtAddress.Text=s.DiaChi;txtCash.Text=s.TonQuy.ToString();cboStatus.Text=s.TrangThai;txtImage.Text=s.AnhHienTruong??"";} };
        Controls.Add(grid); Controls.Add(top); RefreshGrid();
    }
    private static void AddField(TableLayoutPanel p,string label,Control c,int col,int row){p.Controls.Add(new Label{Text=label,AutoSize=true,Anchor=AnchorStyles.Left},col*2,row);p.Controls.Add(c,col*2+1,row);c.Dock=DockStyle.Fill;}
    private AtmStation ReadForm()=>new(){MaTram=txtId.Text.Trim(),DiaChi=txtAddress.Text.Trim(),TonQuy=decimal.TryParse(txtCash.Text,out var n)?n:-1,TrangThai=cboStatus.Text,AnhHienTruong=txtImage.Text};
    private void RefreshGrid()=>grid.DataSource=bus.GetAll();
    private void Run(Action action){try{action();MessageBox.Show("Thao tác thành công.");}catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
    private void ClearForm(){txtId.Clear();txtAddress.Clear();txtCash.Clear();txtImage.Clear();cboStatus.SelectedIndex=0;grid.ClearSelection();}
}
