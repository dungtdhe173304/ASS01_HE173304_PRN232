namespace Repositories;

public interface IRepositoryFactory
{
    IMemberRepository GetMemberRepository();
    ICategoryRepository GetCategoryRepository();
    IProductRepository GetProductRepository();
    IOrderRepository GetOrderRepository();
    IOrderDetailRepository GetOrderDetailRepository();
}