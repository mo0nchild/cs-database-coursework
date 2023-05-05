using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(LocationConfiguration))]
public partial class Location : System.Object
{
    public int Locationid { get; set; } = default!;
    public string? Street { get; set; } = default!;

    public int Cityid { get; set; } = default!;
    public virtual City City { get; set; } = null!;
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
