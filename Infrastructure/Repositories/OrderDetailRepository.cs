using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories;

public class OrderDetailRepository : GenericRepository<OrderDetail> , IOrderDetailRepository
{   
    private readonly ApplicationDbContext _orderDetailContext; 
    public OrderDetailRepository (ApplicationDbContext context) : base(context)
    {
        
    }
} 