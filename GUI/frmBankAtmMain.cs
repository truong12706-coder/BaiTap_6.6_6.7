using System.Drawing;
namespace GUI;
public class frmBankAtmMain : Form
{
    private readonly ToolStripStatusLabel lblUser = new("Giao dịch viên: Admin");
    private readonly ToolStripStatusLabel lblCore = new("Core Banking: Đang kết nối (mô phỏng)");
    private readonly ToolStripProgressBar progress = new() { Minimum=0, Maximum=100, Value=0, Width=180 };
    public frmBankAtmMain()
    {
        Text="HỆ THỐNG ĐIỀU HÀNH MẠNG LƯỚI ATM & GIÁM SÁT LƯỚI ĐIỆN";
        WindowState=FormWindowState.Maximized; IsMdiContainer=true; BackColor=Color.FromArgb(235,240,247);
        var menu=new MenuStrip();
        var sys=new ToolStripMenuItem("Hệ thống");
        sys.DropDownItems.Add("Đăng xuất",null,(_,_)=>{ if(MessageBox.Show("Đăng xuất?","Xác nhận",MessageBoxButtons.YesNo)==DialogResult.Yes) Close(); });
        sys.DropDownItems.Add("Khóa màn hình",null,(_,_)=>MessageBox.Show("Chức năng khóa màn hình mô phỏng."));
        sys.DropDownItems.Add("Thoát",null,(_,_)=>Close());
        var atm=new ToolStripMenuItem("Quản lý ATM");
        atm.DropDownItems.Add("Danh sách Trạm ATM & Tồn quỹ",null,(_,_)=>OpenChild<frmAtmStations>());
        atm.DropDownItems.Add("Đối soát Giao dịch Cuối ngày",null,(_,_)=>OpenChild<frmAtmReconciliation>());
        var grid=new ToolStripMenuItem("Giám sát lưới điện");
        grid.DropDownItems.Add("Trung tâm SCADA",null,(_,_)=>OpenChild<frmPowerGridMonitor>());
        var windows=new ToolStripMenuItem("Cửa sổ");
        windows.DropDownItems.Add("Xếp tầng",null,(_,_)=>LayoutMdi(MdiLayout.Cascade));
        windows.DropDownItems.Add("Xếp ngang",null,(_,_)=>LayoutMdi(MdiLayout.TileHorizontal));
        windows.DropDownItems.Add("Xếp dọc",null,(_,_)=>LayoutMdi(MdiLayout.TileVertical));
        windows.DropDownItems.Add("Đóng tất cả",null,(_,_)=>{ foreach(Form f in MdiChildren.ToArray()) f.Close(); });
        menu.Items.AddRange(new ToolStripItem[]{sys,atm,grid,windows}); MainMenuStrip=menu; Controls.Add(menu);
        var status=new StatusStrip(); status.Items.Add(lblUser); status.Items.Add(new ToolStripStatusLabel(" | ")); status.Items.Add(lblCore); status.Items.Add(new ToolStripStatusLabel(" | Tiến độ đối soát:")); status.Items.Add(progress); Controls.Add(status);
    }
    private void OpenChild<T>() where T:Form,new()
    {
        foreach(Form child in MdiChildren)
            if(child is T) { child.Activate(); return; }
        var form=new T { MdiParent=this }; form.Show();
    }
    public void SetReconciliationProgress(int value)
    {
        if(InvokeRequired) { BeginInvoke(new Action(()=>SetReconciliationProgress(value))); return; }
        progress.Value=Math.Clamp(value,0,100);
    }
}
