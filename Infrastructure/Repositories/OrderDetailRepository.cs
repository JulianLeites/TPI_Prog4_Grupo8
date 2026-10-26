using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories;

public class OrderDetailRepository : GenericRepository<OrderDetail> , IOrderDetailRepository
{   
    public OrderDetailRepository (ApplicationDbContext context) : base(context)
    {
        
    }
} 