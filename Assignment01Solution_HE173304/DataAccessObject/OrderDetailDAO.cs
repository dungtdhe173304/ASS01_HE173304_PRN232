using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class OrderDetailDAO : GenericDAO<OrderDetail>, IOrderDetailDAO
{
    public IEnumerable<OrderDetail> GetOrderDetailsByOrder(int orderId)
    {
        return _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .Include(od => od.Product)
            .ToList();
    }

    public IEnumerable<OrderDetail> GetOrderDetailsByProduct(int productId)
    {
        return _context.OrderDetails
            .Where(od => od.ProductId == productId)
            .Include(od => od.Order)
            .ToList();
    }
    
    // Override GetById for OrderDetail entity with composite key
    public new OrderDetail? GetById(object id)
    {
        if (id is object[] keys && keys.Length == 2 && keys[0] is int orderId && keys[1] is int productId)
        {
            return _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);
        }
        return null;
    }
}