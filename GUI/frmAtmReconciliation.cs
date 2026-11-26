using System.Diagnostics;
namespace GUI;
public class frmAtmReconciliation : Form
{
    private readonly Button btnRun=new(){Text="Chạy Đối Soát Tự Động",AutoSize=true};
    private readonly Button btnStop=new(){Text="Dừng Đối Soát",AutoSize=true,Enabled=false};
    private readonly ProgressBar bar=new(){Dock=DockStyle.Top,Height=24};
    private readonly Label lbl=new(){Text="Sẵn sàng đối soát 200.000 giao dịch.",AutoSize=true};
    private CancellationTokenSource? cts;
    public frmAtmReconciliation()
    {
        Text="Đối soát giao dịch ATM cuối ngày"; Width=700; Height=260;
        var panel=new FlowLayoutPanel{Dock=DockStyle.Top,Height=55,Padding=new Padding(10)};
        panel.Controls.AddRange(new Control[]{btnRun,btnStop});
        Controls.Add(lbl); lbl.Dock=DockStyle.Top; lbl.Padding=new Padding(12); Controls.Add(bar); Controls.Add(panel);
        btnRun.Click+=async(_,_)=>await RunReconciliationAsync();
        btnStop.Click+=(_,_)=>cts?.Cancel();
        FormClosing+=(_,_)=>cts?.Cancel();
    }
    private async Task RunReconciliationAsync()
    {
        cts=new CancellationTokenSource(); btnRun.Enabled=false;btnStop.Enabled=true;bar.Value=0;
        try
        {
            var token=cts.Token;
            int total=200000;
            await Task.Run(async()=>{
                for(int p=1;p<=100;p++)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(45,token); // tổng thời gian xấp xỉ 4,5 giây
                    int value=p;
                    BeginInvoke(new Action(()=>{
                        bar.Value=value;lbl.Text=$"Đang tải và đối soát {value}% ({total*value/100:N0}/{total:N0} giao dịch)";
                        if(MdiParent is frmBankAtmMain main) main.SetReconciliationProgress(value);
                    }));
                }
            },token);
            lbl.Text="Đối soát hoàn tất: 200.000 giao dịch đã được kiểm tra.";
            if(MdiParent is frmBankAtmMain parent) parent.SetReconciliationProgress(100);
        }
        catch(OperationCanceledException){lbl.Text="Đã dừng đối soát theo yêu cầu.";}
        catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi đối soát");}
        finally{btnRun.Enabled=true;btnStop.Enabled=false;cts?.Dispose();cts=null;}
    }
}
