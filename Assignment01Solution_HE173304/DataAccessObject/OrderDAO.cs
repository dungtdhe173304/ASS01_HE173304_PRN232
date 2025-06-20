using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class OrderDAO : GenericDAO<Order>, IOrderDAO
{
    public IEnumerable<Order> GetOrdersByMember(int memberId)
    {
        return _context.Orders
            .Where(o => o.MemberId == memberId)
            .Include(o => o.OrderDetails)
            .ToList();
    }

    public IEnumerable<Order> GetOrdersByDateRange(DateOnly startDate, DateOnly endDate)
    {
        return _context.Orders
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .Include(o => o.OrderDetails)
            .ToList();
    }
    
    // Override GetById for Order entity
    public new Order? GetById(object id)
    {
        if (id is int orderId)
        {
            return _context.Orders
                .Include(o => o.Member)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);
        }
        return null;
    }
}