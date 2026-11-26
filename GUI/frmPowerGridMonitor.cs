using BUS;
using DTO;
using System.Drawing;
namespace GUI;
public class frmPowerGridMonitor : Form
{
    private readonly ComboBox cboStation=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=220};
    private readonly Button btnStart=new(){Text="Bắt đầu Giám sát SCADA",AutoSize=true};
    private readonly Button btnStop=new(){Text="Ngắt Kết nối",AutoSize=true,Enabled=false};
    private readonly Label lblVoltage=new(){Text="Điện áp: -- kV",AutoSize=true,Font=new Font("Segoe UI",16,FontStyle.Bold)};
    private readonly Label lblLoadPercent=new(){Text="Mức mang tải: -- %",AutoSize=true,Font=new Font("Segoe UI",16,FontStyle.Bold)};
    private readonly Label lblWarning=new(){Text="Trạng thái: Chưa kết nối",AutoSize=true};
    private readonly ToolStripStatusLabel statusTime=new("Gói tin cuối: --:--:--");
    private readonly ToolStripStatusLabel statusCount=new("Chu kỳ: 0");
    private readonly DataGridView dgvGridLogs=new(){Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AutoGenerateColumns=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect};
    private readonly List<GridLog> logs=new();
    private readonly GridLogBUS logBus=new();
    private CancellationTokenSource? cts;
    private int cycles=0;
    public frmPowerGridMonitor()
    {
        Text="Trung tâm điều độ & giám sát lưới điện thời gian thực";Width=950;Height=620;
        cboStation.Items.AddRange(new object[]{"Trạm 110kV Hà Đông","Trạm 220kV Chèm"});cboStation.SelectedIndex=0;
        var controls=new FlowLayoutPanel{Dock=DockStyle.Top,Height=55,Padding=new Padding(10)};
        controls.Controls.AddRange(new Control[]{new Label{Text="Trạm biến áp:",AutoSize=true,Padding=new Padding(0,8,0,0)},cboStation,btnStart,btnStop});
        var dashboard=new FlowLayoutPanel{Dock=DockStyle.Top,Height=105,Padding=new Padding(18)};
        dashboard.Controls.AddRange(new Control[]{lblVoltage,lblLoadPercent,lblWarning});
        dgvGridLogs.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Thời gian",DataPropertyName="ThoiGian",Width=170});
        dgvGridLogs.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Trạm",DataPropertyName="Tram",Width=220});
        dgvGridLogs.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Điện áp (kV)",DataPropertyName="DienAp",Width=130});
        dgvGridLogs.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Mức tải (%)",DataPropertyName="MucTai",Width=120});
        dgvGridLogs.Columns.Add(new DataGridViewTextBoxColumn{HeaderText="Cảnh báo",DataPropertyName="CanhBao",AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill});
        var status=new StatusStrip();status.Items.Add(statusTime);status.Items.Add(new ToolStripStatusLabel(" | "));status.Items.Add(statusCount);
        Controls.Add(dgvGridLogs);Controls.Add(dashboard);Controls.Add(controls);Controls.Add(status);
        btnStart.Click+=async(_,_)=>await StartMonitoringAsync();btnStop.Click+=(_,_)=>cts?.Cancel();
        FormClosing+=(_,_)=>cts?.Cancel();
    }
    private async Task StartMonitoringAsync()
    {
        if(cts!=null)return;
        cts=new CancellationTokenSource();btnStart.Enabled=false;btnStop.Enabled=true;cboStation.Enabled=false;
        var token=cts.Token;string station=cboStation.Text;
        try
        {
            await Task.Run(async()=>{
                var random=new Random();
                while(!token.IsCancellationRequested)
                {
                    double voltage=Math.Round(108+random.NextDouble()*7,2);
                    int load=random.Next(50,99);
                    // Mọi thao tác đọc/ghi Control đều được chuyển về UI Thread bằng Invoke.
                    if(InvokeRequired)
                    {
                        Invoke(new Action(()=>{
                            if(IsDisposed)return;
                            cycles++;
                            lblVoltage.Text=$"Điện áp: {voltage} kV";
                            lblLoadPercent.Text=$"Mức mang tải: {load}%";
                            lblLoadPercent.ForeColor=load>85?Color.Red:Color.Green;
                            lblWarning.Text=load>85?"CẢNH BÁO: TRẠM QUÁ TẢI!":"Trạng thái: Ổn định";
                            statusTime.Text=$"Gói tin cuối: {DateTime.Now:HH:mm:ss}";
                            statusCount.Text=$"Chu kỳ: {cycles}";
                            if(load>85)
                            {
                                var log=new GridLog{ThoiGian=DateTime.Now,Tram=station,DienAp=voltage,MucTai=load,CanhBao="Quá tải (>85%)"};
                                logs.Insert(0,log);logBus.Add(log);dgvGridLogs.DataSource=null;dgvGridLogs.DataSource=logs.ToList();
                            }
                        }));
                    }
                    await Task.Delay(2000,token);
                }
            },token);
        }
        catch(OperationCanceledException){/* Người dùng đã ngắt kết nối. */}
        catch(ObjectDisposedException){/* Form đã đóng. */}
        finally
        {
            if(!IsDisposed)
            {
                btnStart.Enabled=true;btnStop.Enabled=false;cboStation.Enabled=true;
                lblWarning.Text="Trạng thái: Đã ngắt kết nối";
            }
            cts?.Dispose();cts=null;
        }
    }
}
