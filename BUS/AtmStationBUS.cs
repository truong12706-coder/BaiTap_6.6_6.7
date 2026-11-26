using DAL;
using DTO;
namespace BUS;
public class AtmStationBUS
{
    private readonly AtmStationDAL _dal = new();
    public List<AtmStation> GetAll() => _dal.GetAll();
    public void Add(AtmStation item)
    {
        if(string.IsNullOrWhiteSpace(item.MaTram)) throw new ArgumentException("Mã trạm không được để trống.");
        if(string.IsNullOrWhiteSpace(item.DiaChi)) throw new ArgumentException("Địa chỉ không được để trống.");
        if(item.TonQuy < 0) throw new ArgumentException("Tồn quỹ không hợp lệ.");
        _dal.Add(item);
    }
    public void Update(AtmStation item) => _dal.Update(item);
    public void Delete(string id) => _dal.Delete(id);
}
