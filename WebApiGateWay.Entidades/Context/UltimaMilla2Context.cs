﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.Extensions.Configuration;

namespace WebApiGateWay.Entidades.Context;

public partial class UltimaMilla2Context : DbContext
{
   

    public UltimaMilla2Context(DbContextOptions<UltimaMilla2Context> options)
        : base(options)
    {
      
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //para utilizar settingjson
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        var connectionString = _configuration.GetConnectionString("DefaultConnection");
    //        optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));
    //    }
    //}
    ///// 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf32_general_ci")
            .HasCharSet("utf32");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("customers")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Tag, "customers_UN").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Tag)
                .HasMaxLength(10)
                .HasColumnName("tag");
            entity.Property(e => e.Url)
                .HasMaxLength(100)
                .HasColumnName("url");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => new { e.CustomerId, e.Device }, "customer_id").IsUnique();

            entity.HasIndex(e => e.CustomerId, "idx_customer_id");

            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CustomerId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("customer_id");
            entity.Property(e => e.Device)
                .HasMaxLength(40)
                .HasColumnName("device");
            entity.Property(e => e.LastConnection)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("last_connection");
            entity.Property(e => e.TokensValidSince)
                .HasColumnType("timestamp")
                .HasColumnName("tokens_valid_since");

            entity.HasOne(d => d.Customer).WithMany(p => p.Users)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("fk_users_customer_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
