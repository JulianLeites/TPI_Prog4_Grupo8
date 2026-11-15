using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services;

public static class CurrencyConverter
{
    public static decimal? ToUsd(decimal amount, decimal? rate) =>
        rate is > 0 ? Math.Round(amount / rate.Value, 2) : null;
}