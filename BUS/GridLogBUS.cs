using DAL;
using DTO;
namespace BUS;
public class GridLogBUS
{
    private readonly GridLogDAL _dal = new();
    public void Add(GridLog log) => _dal.Add(log);
    public List<GridLog> GetAll() => _dal.GetAll();
}
