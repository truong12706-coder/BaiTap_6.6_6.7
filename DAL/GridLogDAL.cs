using DTO;
namespace DAL;
public class GridLogDAL
{
    private static readonly List<GridLog> _logs = new();
    public void Add(GridLog log) => _logs.Add(log);
    public List<GridLog> GetAll() => _logs.OrderByDescending(x=>x.ThoiGian).ToList();
}
