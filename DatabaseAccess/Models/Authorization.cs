using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using DatabaseAccess.Configurations;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(AuthorizationConfiguration))]
public partial class Authorization : System.Object
{
    public int Authorizationid { get; set; } = default;
    public string Login { get; set; } = default!;

    public string Password { get; set; } = default!;
    public int Contactid { get; set; } = default;
    public bool Isadmin { get; set; } = default;

    public string Referenceguid { get; set; } = null!;
    public virtual Contact Contact { get; set; } = null!;
}
