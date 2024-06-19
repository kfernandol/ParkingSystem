using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ParkingSystem.Entities;

public partial class ParkingContext : DbContext
{
    public ParkingContext()
    {
    }

    public ParkingContext(DbContextOptions<ParkingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ParkingHistory> ParkingHistories { get; set; }

    public virtual DbSet<ParkingStatus> ParkingStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParkingHistory>(entity =>
        {
            entity.ToTable("ParkingHistory");

            entity.Property(e => e.IsEnter).HasColumnType("BOOLEAN");
        });

        modelBuilder.Entity<ParkingStatus>(entity =>
        {
            entity.ToTable("ParkingStatus");

            entity.Property(e => e.EntryDoorStatus).HasColumnType("INT");
            entity.Property(e => e.ExitDoorStatus).HasColumnType("INT");
            entity.Property(e => e.SpacesAvailable).HasColumnType("INT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
