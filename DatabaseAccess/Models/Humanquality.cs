using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(HumanQualityConfiguration))]
public partial class Humanquality : System.Object
{
    public int Humanqualityid { get; set; } = default;

    public string Qualityname { get; set; } = null!;
    public string Qualitytype { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
