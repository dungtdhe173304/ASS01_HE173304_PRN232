using BusinessObject.Models;

namespace Repositories;

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
    IEnumerable<OrderDetail> GetOrderDetailsByOrder(int orderId);
    IEnumerable<OrderDetail> GetOrderDetailsByProduct(int productId);
}