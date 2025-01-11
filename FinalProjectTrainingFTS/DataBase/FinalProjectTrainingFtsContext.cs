using System;
using System.Collections.Generic;
using FinalProjectTrainingFTS.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectTrainingFTS.DataBase;

public partial class FinalProjectTrainingFtsContext : DbContext
{
    public FinalProjectTrainingFtsContext()
    {
    }

    public FinalProjectTrainingFtsContext(DbContextOptions<FinalProjectTrainingFtsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<BookRoom> BookRooms { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WAJEED\\MSSQLSERVER01;Database=FinalProjectTrainingFTS;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true", x => x.UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.HasIndex(e => e.UserName, "IX_Admin").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<BookRoom>(entity =>
        {
            entity.ToTable("Book Room");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BookFrom)
                .HasColumnType("date")
                .HasColumnName("Book from");
            entity.Property(e => e.BookTo)
                .HasColumnType("date")
                .HasColumnName("Book to");
            entity.Property(e => e.RoomId).HasColumnName("Room ID");
            entity.Property(e => e.UserId).HasColumnName("User ID");

            entity.HasOne(d => d.Room).WithMany(p => p.BookRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Book Room_Room");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.HasIndex(e => e.Name, "IX_City").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PostOffice).HasColumnName("Post Office");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.ToTable("Hotel");

            entity.HasIndex(e => e.Name, "IX_Hotel").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Amenities)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CityId).HasColumnName("City ID");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.Latitude)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Owner)
                .HasMaxLength(50)
                .HasColumnName("owner");
            entity.Property(e => e.StarRate).HasColumnName("star rate");

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Hotel_City");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("Room ID");
            entity.Property(e => e.Descriptions).HasMaxLength(200);
            entity.Property(e => e.HotelId).HasColumnName("Hotel ID");
            entity.Property(e => e.Image).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "IX_User").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.VisitedHotels)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
