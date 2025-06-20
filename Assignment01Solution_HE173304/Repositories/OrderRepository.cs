using BusinessObject.Models;
using DataAccessObject;

namespace Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly IOrderDAO _orderDAO;

    public OrderRepository(IOrderDAO orderDAO) : base(orderDAO)
    {
        _orderDAO = orderDAO;
    }

    public IEnumerable<Order> GetOrdersByMember(int memberId)
    {
        return _orderDAO.GetOrdersByMember(memberId);
    }

    public IEnumerable<Order> GetOrdersByDateRange(DateOnly startDate, DateOnly endDate)
    {
        return _orderDAO.GetOrdersByDateRange(startDate, endDate);
    }
}