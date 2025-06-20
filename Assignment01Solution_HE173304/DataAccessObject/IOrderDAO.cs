using BusinessObject.Models;

namespace DataAccessObject;

public interface IOrderDAO : IGenericDAO<Order>
{
    IEnumerable<Order> GetOrdersByMember(int memberId);
    IEnumerable<Order> GetOrdersByDateRange(DateOnly startDate, DateOnly endDate);
}