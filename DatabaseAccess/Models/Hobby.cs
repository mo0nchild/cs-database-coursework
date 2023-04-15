using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(HobbyConfiguration))]
public partial class Hobby : System.Object
{
    public int Hobbyid { get; set; } = default;

    public string Hobbyname { get; set; } = null!;
    public string? Hobbytype { get; set; } = default;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
