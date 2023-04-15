using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using DatabaseAccess.Configurations;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(AuthorizationConfiguration))]
public partial class Authorization : System.Object
{
    public int Authorizationid { get; set; } = default;

    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool Isadmin { get; set; } = default;
    public int Contactid { get; set; } = default;

    public virtual Contact Contact { get; set; } = null!;
}
