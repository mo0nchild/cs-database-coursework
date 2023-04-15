using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(PostConfiguration))]
public partial class Post : System.Object
{
    public int Postid { get; set; } = default;
    public string Postname { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
