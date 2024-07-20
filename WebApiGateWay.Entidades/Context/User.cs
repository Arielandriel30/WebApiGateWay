using System;
using System.Collections.Generic;

namespace WebApiGateWay.Entidades.Context;

public partial class User
{
    public ulong UserId { get; set; }

    public ulong CustomerId { get; set; }

    public string Device { get; set; } = null!;

    public bool? Active { get; set; }

    public DateTime LastConnection { get; set; }

    public DateTime? TokensValidSince { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
