using DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

[EntityTypeConfiguration(typeof(ContactConfiguration))]
public partial class Contact : System.Object
{
    public int Contactid { get; set; } = default;

    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Patronymic { get; set; } = default;

    public string? Familystatus { get; set; } = default;
    public DateTime Birthday { get; set; } = default;

    public string Emailaddress { get; set; } = null!;
    public string? Phonenumber { get; set; } = default;
    public DateTime Lastupdate { get; set; } = default;

    public int? Locationid { get; set; } = default;
    public int Gendertypeid { get; set; } = default;
    public int? Userpictureid { get; set; } = default;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Friend> FriendContactid1Navigations { get; set; } = new List<Friend>();
    public virtual ICollection<Friend> FriendContactid2Navigations { get; set; } = new List<Friend>();

    public virtual Gendertype Gendertype { get; set; } = null!;
    public virtual Location? Location { get; set; }

    public virtual Authorization? Authorization { get; set; }
    public virtual Userpicture? Userpicture { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public virtual ICollection<Humanquality> Humanqualities { get; set; } = new List<Humanquality>();
}
