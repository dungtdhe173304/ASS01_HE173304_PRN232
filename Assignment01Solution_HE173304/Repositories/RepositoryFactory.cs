using DataAccessObject;

namespace Repositories;

public class RepositoryFactory : IRepositoryFactory
{
    public IMemberRepository GetMemberRepository()
    {
        return new MemberRepository(new MemberDAO());
    }

    public ICategoryRepository GetCategoryRepository()
    {
        return new CategoryRepository(new CategoryDAO());
    }

    public IProductRepository GetProductRepository()
    {
        return new ProductRepository(new ProductDAO());
    }

    public IOrderRepository GetOrderRepository()
    {
        return new OrderRepository(new OrderDAO());
    }

    public IOrderDetailRepository GetOrderDetailRepository()
    {
        return new OrderDetailRepository(new OrderDetailDAO());
    }
}