using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(UserPictureConfiguration))]
public partial class Userpicture : System.Object
{
    public int Userpictureid { get; set; } = default;

    public string Filepath { get; set; } = null!;
    public string Picturename { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
