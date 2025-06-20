using BusinessObject.Models;

namespace DataAccessObject;

public interface IOrderDetailDAO : IGenericDAO<OrderDetail>
{
    IEnumerable<OrderDetail> GetOrderDetailsByOrder(int orderId);
    IEnumerable<OrderDetail> GetOrderDetailsByProduct(int productId);
}