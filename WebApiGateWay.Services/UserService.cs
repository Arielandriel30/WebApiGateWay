using WebApiGateWay.Entidades.Context;
using Microsoft.EntityFrameworkCore;
namespace WebApiGateWay.Services;

public class UserService(UltimaMilla2Context context)
{
    public async Task<User?> GetUserCustomer(Dictionary<string, string> claims)
    {
        var userId = ulong.Parse(claims["user_id"]);
        var customerTag = claims["customer_tag"];
        var device = claims["device"];

        var user = await context.Users
            .Include(u => u.Customer)
            .Where(u => u.UserId == userId && u.Customer.Tag == customerTag && u.Device == device && u.Active == true)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<(bool, User?)> UpdateUserCustomer(Dictionary<string, string> claims)
    {
        var userId = ulong.Parse(claims["user_id"]);
        var customerTag = claims["customer_tag"];
        var device = claims["device"];
        // Buscar el usuario que cumple con los criterios especificados
        var user = await context.Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.Customer.Tag == customerTag && u.Device == device && u.Active == true);

        if (user != null)
        {
            // Actualizar la fecha y hora de tokens_valid_since
            user.TokensValidSince = DateTime.UtcNow;
        
            // Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
        
            return (true, user);
        }
        return (false, null);
    }
}