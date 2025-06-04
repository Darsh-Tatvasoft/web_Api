using System;
using System.Collections.Generic;
using Api.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository.Data;

public partial class LibraryDBContext : DbContext
{
    public LibraryDBContext()
    {
    }

    public LibraryDBContext(DbContextOptions<LibraryDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=LibraryDB;Username=postgres;Password=Tatva@123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("books_pkey");

            entity.ToTable("books");

            entity.HasIndex(e => e.Isbn, "books_isbn_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .HasColumnName("genre");
            entity.Property(e => e.Isavailable)
                .HasDefaultValueSql("true")
                .HasColumnName("isavailable");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .HasColumnName("language");
            entity.Property(e => e.Pagecount).HasColumnName("pagecount");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Publisheddate).HasColumnName("publisheddate");
            entity.Property(e => e.Publisher)
                .HasMaxLength(100)
                .HasColumnName("publisher");
            entity.Property(e => e.Stockquantity)
                .HasDefaultValueSql("0")
                .HasColumnName("stockquantity");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Updatedat).HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.HasIndex(e => e.Mobilenumber, "user_mobilenumber_key").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Mobilenumber).HasMaxLength(15);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Role).HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
