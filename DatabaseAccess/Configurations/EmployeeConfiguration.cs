using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Models.Employee>
    {
        public EmployeeConfiguration() : base() { }
        public virtual void Configure(EntityTypeBuilder<Models.Employee> entity_builder)
        {
            entity_builder.HasKey(e => e.Employeeid).HasName("employee_pkey");

            entity_builder.ToTable("employee");

            entity_builder.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity_builder.Property(e => e.Companyname)
                .HasMaxLength(50)
                .HasColumnName("companyname");
            entity_builder.Property(e => e.Contactid).HasColumnName("contactid");
            entity_builder.Property(e => e.Postid).HasColumnName("postid");
            entity_builder.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");

            entity_builder.HasOne(d => d.Contact).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Contactid)
                .HasConstraintName("employee_contactid_fkey");

            entity_builder.HasOne(d => d.Post).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Postid)
                .HasConstraintName("employee_postid_fkey");
        }
    }
}
