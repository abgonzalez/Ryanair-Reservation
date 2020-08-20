using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using Ryanair.Reservation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Ryanair.Reservation.Data
{
    public class ReservationApiDbContext : DbContext
    {
        public ReservationApiDbContext(DbContextOptions options)
            : base(options)
        {
        }
        #region Tables
        public DbSet<FlightEntity> Flights { get; set; }

        public DbSet<ReservationEntity> Reservations { get; set; }

        public DbSet<ReservationDetailsEntity> ReservationDetails { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Database Model
            modelBuilder.Entity<FlightEntity>(entity =>
            {
                entity.HasKey("FlightKey");

                entity.HasIndex(e => e.FlightKey)
                    .HasName("IDX_Flights")
                    .IsUnique();

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FlightKey)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Origin)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Time).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReservationEntity>(entity =>
            {
                entity.HasKey("ReservationNumber");

                entity.HasIndex(e => e.ReservationNumber)
                    .HasName("IDX_Reservation")
                    .IsUnique();

                entity.Property(e => e.CreditCard)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ReservationNumber)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReservationDetailsEntity>(entity =>
            {
                entity.HasKey("Id");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_ReservationDetails")
                    .IsUnique();

                entity.Property(e => e.FlightKey)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ReservationNumber)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.HasOne(d => d.FlightKeyNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.FlightKey)
                    .HasForeignKey(d => d.FlightKey)
                       .HasConstraintName("FK_ReservationDetails_Flights");

                entity.HasOne(d => d.ReservationNoNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.ReservationNumber)
                    .HasForeignKey(d => d.ReservationNumber)
                       .HasConstraintName("FK_ReservationDetails_Reservation");
            });
            #endregion

        }

   
    }
}
