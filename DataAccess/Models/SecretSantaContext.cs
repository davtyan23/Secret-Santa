using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class SecretSantaContext : DbContext
{
    public DbSet<PassResetConfiramtionCode> PasswordResetConfirmationCodes { get; set; }
    public SecretSantaContext()
    {
    }

    public SecretSantaContext(DbContextOptions<SecretSantaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssignedRole> AssignedRoles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPass> UserPasses { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<UserGroup> UserGroups { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<GroupInfo> GroupsInfo { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source = MARIALAPTOP\\SQLEXPRESS; Initial Catalog = SecretSanta.bak; Integrated Security = True; Trust Server Certificate = True").LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AssignedRole>(entity =>
        {
            entity.HasKey(e => e.UserRolesId).HasName("PK__Assigned__43D8C0CD0CFF31EB");
            entity.Property(e => e.UserRolesId).HasColumnName("UserRolesID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A89E5068C");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("USER");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id)
                  .HasName("PK__Users__3214EC27A8DFC82C"); // Matches your database's PK name

            entity.Property(e => e.Id)
                  .HasColumnName("ID") 
                  .ValueGeneratedOnAdd(); 

            entity.Property(e => e.FirstName)
                  .HasColumnName("Firstname")
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(e => e.LastName)
                  .HasColumnName("Lastname")  
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(e => e.PhoneNumber)
                  .HasColumnName("PhoneNumber") 
                  .HasMaxLength(15)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasColumnName("IsActive") 
                  .HasDefaultValue(true) 
                  .IsRequired();

            entity.Property(e => e.RegisterTime)
                  .HasColumnName("RegisterTime") 
                  .HasColumnType("datetime");  

            // Map to the Users table
            entity.ToTable("Users");
        });

        modelBuilder.Entity<UserPass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPass__3214EC27F9C34B81");

            entity.ToTable("UserPass");

            entity.HasIndex(e => e.Email, "UQ__UserPass__A9D10534ED3EF5B4").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PassHash).HasMaxLength(60);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupID);
            entity.Property(e => e.GroupID).HasColumnName("GroupID");
            entity.ToTable("Groups");

        //entity.Property(e => e.GroupName).HasMaxLength(55).IsRequired();
        //entity.Property(e => e.GroupLocation).IsRequired();
        //entity.Property(e => e.MaxBudget).IsRequired();
        //entity.Property(e => e.MinBudget).IsRequired();
    });


        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.UserGroupID);
            entity.ToTable("UserGroups");
        
            //entity.HasKey(e => e.UserGroupID);
            //entity.HasOne(e => e.User)
            //      .WithMany()
            //      .HasForeignKey(e => e.UserID)
            //      .OnDelete(DeleteBehavior.Restrict)
            //      .HasConstraintName("FK_UserGroups_User");
        
            //entity.HasOne(e => e.Groups)
            //      .WithMany()
            //      .HasForeignKey(e => e.GroupID)
            //      .OnDelete(DeleteBehavior.Restrict)
            //      .HasConstraintName("FK_UserGroups_Group");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRolesID); 

            entity.HasOne(e => e.Role)
                  .WithMany() 
                  .HasForeignKey(e => e.RoleID)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_UserRoles_Role");

            entity.HasOne(e => e.User)
                  .WithMany()  
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_UserRoles_User");
        });

        modelBuilder.Entity<GroupInfo>(entity =>
        {
            entity.HasKey(e => e.GroupInfoID); 

            entity.HasOne(e => e.UserGroups)
                  .WithMany() 
                  .HasForeignKey(e => e.UserGroupID)
                  .OnDelete(DeleteBehavior.Restrict
                  ) 
                  .HasConstraintName("FK_GroupInfo_UserGroup");

            entity.HasOne(e => e.Receiver)
                  .WithMany() 
                  .HasForeignKey(e => e.ReceiverID)
                  .OnDelete(DeleteBehavior.Restrict)                  
                  .HasConstraintName("FK_GroupInfo_Receiver");
        });

        //OnModelCreating(modelBuilder);
    }
}