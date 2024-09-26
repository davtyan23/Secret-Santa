﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class SecretSantaContext : DbContext
{
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source = INTERN-IS\\MSSQLSERVER01; Initial Catalog = SecretSanta; Integrated Security = True; Trust Server Certificate = True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignedRole>(entity =>
        {
            entity.HasKey(e => e.UserRolesId).HasName("PK__Assigned__43D8C0CD0CFF31EB");

            entity.Property(e => e.UserRolesId).HasColumnName("UserRolesID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.AssignedRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__AssignedR__RoleI__4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.AssignedRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AssignedR__UserI__412EB0B6");
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
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27A8DFC82C");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Firstname).HasMaxLength(30);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Lastname).HasMaxLength(30);
            entity.Property(e => e.Number).HasMaxLength(15);
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

            entity.HasOne(d => d.User).WithMany(p => p.UserPasses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserPass__UserID__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}