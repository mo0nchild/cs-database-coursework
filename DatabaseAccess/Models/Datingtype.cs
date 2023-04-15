using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(DatingTypeConfiguration))]
public partial class Datingtype : System.Object
{
    public int Datingtypeid { get; set; } = default;
    public string Typeofdating { get; set; } = null!;

    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}
