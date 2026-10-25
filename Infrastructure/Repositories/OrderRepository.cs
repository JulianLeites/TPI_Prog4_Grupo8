using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
   public OrderRepository(ApplicationDbContext context) : base(context)
    {
        
    }
    
}