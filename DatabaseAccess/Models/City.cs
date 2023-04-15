using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(CityConfiguration))]
public partial class City : System.Object
{
    public int Cityid { get; set; } = default;

    public string Cityname { get; set; } = null!;
    public string Country { get; set; } = null!;

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
