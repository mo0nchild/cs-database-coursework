using System;
using System.Collections.Generic;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DatabaseAccess;

using LoggerConfiguration = IOptionsMonitor<DatabaseLoggerConfiguration>;
public partial class DatabaseContext : Microsoft.EntityFrameworkCore.DbContext
{
    public static ILoggerFactory DatabaseLoggerFactory = default!;
    public DatabaseContext(LoggerConfiguration configuration) 
        : base() { base.Database.EnsureCreated(); this.LoggerInitial(configuration); }

    public DatabaseContext(DbContextOptions<DatabaseContext> options, LoggerConfiguration configuration) 
        : base(options) { base.Database.EnsureCreated(); this.LoggerInitial(configuration); }

    public virtual DbSet<Models.Authorization> Authorizations { get; set; } = null!;
    public virtual DbSet<Models.Contact> Contacts { get; set; } = null!;
    public virtual DbSet<Models.Gendertype> Gendertypes { get; set; } = null!;

    public virtual DbSet<Models.City> Cities { get; set; } = null!;
    public virtual DbSet<Models.Location> Locations { get; set; } = null!;

    public virtual DbSet<Models.Post> Posts { get; set; } = null!;
    public virtual DbSet<Models.Employee> Employees { get; set; } = null!;

    public virtual DbSet<Models.Datingtype> Datingtypes { get; set; } = null!;
    public virtual DbSet<Models.Friend> Friends { get; set; } = null!;
    public virtual DbSet<Models.Message> Messages { get; set; } = null!;

    public virtual DbSet<Models.Userpicture> Userpictures { get; set; } = null!;
    public virtual DbSet<Models.Hobby> Hobbies { get; set; } = null!;
    public virtual DbSet<Models.Humanquality> Humanqualities { get; set; } = null!;

    protected void LoggerInitial(IOptionsMonitor<DatabaseLoggerConfiguration> configuration)
    {
        DatabaseContext.DatabaseLoggerFactory = LoggerFactory.Create((ILoggingBuilder builder) =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name)
                .AddProvider(new DatabaseLoggerProvider(configuration));
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseNpgsql("Server=localhost;Database=db_course_work;Username=postgres;Password=prolodgy778;");
        optionsBuilder.UseLoggerFactory(DatabaseContext.DatabaseLoggerFactory);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) => this.OnModelCreatingPartial(modelBuilder);
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
