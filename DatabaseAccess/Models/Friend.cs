using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(FriendConfiguration))]
public partial class Friend : System.Object
{
    public int Friendid { get; set; } = default;
    public DateOnly Starttime { get; set; } = default;

    public int? Datingtypeid { get; set; } = default;
    public int Contactid1 { get; set; } = default;
    public int Contactid2 { get; set; } = default;

    public virtual Contact Contactid1Navigation { get; set; } = null!;
    public virtual Contact Contactid2Navigation { get; set; } = null!;

    public virtual Datingtype? Datingtype { get; set; } = default;
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
