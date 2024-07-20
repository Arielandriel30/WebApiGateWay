using System;
using System.Collections.Generic;

namespace WebApiGateWay.Entidades.Context;

public partial class Customer
{
    public ulong Id { get; set; }

    public string Name { get; set; } = null!;

    public string Tag { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
