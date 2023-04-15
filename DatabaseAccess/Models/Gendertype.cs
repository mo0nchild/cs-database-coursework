using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(GenderTypeConfiguration))]
public partial class Gendertype : System.Object
{
    public int Gendertypeid { get; set; } = default;

    public string Gendertypename { get; set; } = null!;
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
