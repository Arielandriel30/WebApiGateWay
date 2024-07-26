using WebApiGateWay.Entidades.Context;
using Microsoft.EntityFrameworkCore;
namespace WebApiGateWay.Services;

public class UserService
{
    private readonly UltimaMilla2Context _context;

    public UserService( UltimaMilla2Context context)
    {
        _context = context;
    }

    public async Task<User?> GetUserCustomer(Dictionary<string, string> claims)
    {
        var userId = ulong.Parse(claims["user_id"]);
        var customerTag = claims["customer_tag"];
        var device = claims["device"];

        var user = await _context.Users
            .Include(u => u.Customer)
            .Where(u => u.UserId == userId && u.Customer.Tag == customerTag && u.Device == device && u.Active == true)
            .FirstOrDefaultAsync();

        return user;
    }
}