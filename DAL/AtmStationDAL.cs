using DTO;
namespace DAL;
public class AtmStationDAL
{
    private static readonly List<AtmStation> _stations = new()
    {
        new() { MaTram="ATM001", DiaChi="12 Nguyễn Trãi, Hà Nội", TonQuy=250000000, TrangThai="Hoạt động" },
        new() { MaTram="ATM002", DiaChi="45 Trần Duy Hưng, Hà Nội", TonQuy=120000000, TrangThai="Bảo trì" }
    };
    public List<AtmStation> GetAll() => _stations.ToList();
    public void Add(AtmStation item)
    {
        if (_stations.Any(x => x.MaTram.Equals(item.MaTram, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã trạm đã tồn tại.");
        _stations.Add(item);
    }
    public void Update(AtmStation item)
    {
        var old = _stations.FirstOrDefault(x => x.MaTram == item.MaTram)
            ?? throw new InvalidOperationException("Không tìm thấy trạm ATM.");
        old.DiaChi=item.DiaChi; old.TonQuy=item.TonQuy; old.TrangThai=item.TrangThai; old.AnhHienTruong=item.AnhHienTruong;
    }
    public void Delete(string id)
    {
        var item=_stations.FirstOrDefault(x=>x.MaTram==id);
        if(item!=null) _stations.Remove(item);
    }
}
