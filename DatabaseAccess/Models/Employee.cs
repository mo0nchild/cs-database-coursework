using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(EmployeeConfiguration))]
public partial class Employee : System.Object
{
    public int Employeeid { get; set; } = default;

    public string Companyname { get; set; } = null!;
    public string? Status { get; set; } = default;

    public int? Postid { get; set; } = default;
    public int Contactid { get; set; } = default;

    public virtual Contact Contact { get; set; } = null!;
    public virtual Post? Post { get; set; }
}
