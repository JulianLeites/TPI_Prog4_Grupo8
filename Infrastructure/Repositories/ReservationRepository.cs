using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context) : base(context)
    {
    }
}