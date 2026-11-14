using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IDolarService
{
    Task<decimal> GetDolarOficialVentaAsync();
}
