using BusinessObject.Models;

namespace Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    IEnumerable<Order> GetOrdersByMember(int memberId);
    IEnumerable<Order> GetOrdersByDateRange(DateOnly startDate, DateOnly endDate);
}