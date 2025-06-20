using BusinessObject.Models;
using DataAccessObject;

namespace Repositories;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    private readonly IOrderDetailDAO _orderDetailDAO;

    public OrderDetailRepository(IOrderDetailDAO orderDetailDAO) : base(orderDetailDAO)
    {
        _orderDetailDAO = orderDetailDAO;
    }

    public IEnumerable<OrderDetail> GetOrderDetailsByOrder(int orderId)
    {
        return _orderDetailDAO.GetOrderDetailsByOrder(orderId);
    }

    public IEnumerable<OrderDetail> GetOrderDetailsByProduct(int productId)
    {
        return _orderDetailDAO.GetOrderDetailsByProduct(productId);
    }
}