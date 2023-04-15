using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(MessageConfiguration))]
public partial class Message : System.Object
{
    public int Messageid { get; set; } = default;

    public string Messagebody { get; set; } = null!;
    public DateTime Sendtime { get; set; }

    public int Friendid { get; set; } = default;
    public int Contactid { get; set; } = default;

    public virtual Contact Contact { get; set; } = null!;
    public virtual Friend Friend { get; set; } = null!;
}
