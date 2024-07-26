using WebApiGateWay.Entidades.Context;
using Microsoft.EntityFrameworkCore;
namespace WebApiGateWay.Services;

public class CustomerService
{
    private readonly UltimaMilla2Context _context;

    public CustomerService( UltimaMilla2Context context)
    {
        _context = context;
    }
    public async Task<string?> GetCustomerUrl(Dictionary<string, string> claims)
    {
        var customerTag = claims["customer_tag"];
        var customerUrl = await _context.Customers
            .Where(c => c.Tag == customerTag)
            .Select(c => c.Url)
            .FirstOrDefaultAsync();

        return customerUrl;
    }
}