using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Services;

public class DolarService : IDolarService
{
    private readonly HttpClient _httpClient;

    public DolarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetDolarOficialVentaAsync()
    {
        try
        {
            // 💡 Llamada directa a la API del Dólar Oficial
            var result = await _httpClient.GetFromJsonAsync<DolarResponseDto>("https://dolarapi.com/v1/dolares/oficial");
            return result?.Venta ?? 0;
        }
        catch
        {
            // Si la API no responde o no hay internet, devolvemos 0 para no romper la app
            return 0;
        }
    }
}