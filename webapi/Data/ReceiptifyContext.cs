using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using webapi.Models;

namespace webapi.Data;



public partial class ReceiptifyContext : DbContext
{
    public ReceiptifyContext()
    {
    }

    public ReceiptifyContext(DbContextOptions<ReceiptifyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql("server=35.226.152.169;database=receiptify;user=testing;password=123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));      
        }
       
    }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PRIMARY");

            entity.ToTable("expense");

            entity.HasIndex(e => e.ReceiptId, "idx_expense_receipt_id");

            entity.HasIndex(e => e.SupplierId, "idx_expense_supplier_id");

            entity.HasIndex(e => e.Uid, "idx_expense_uid");

            entity.Property(e => e.ExpenseId).HasColumnName("expense_id");
            entity.Property(e => e.ExpenseDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("expense_date");
            entity.Property(e => e.ReceiptId).HasColumnName("receipt_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.Uid).HasColumnName("uid");

            entity.HasOne(d => d.Receipt).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ReceiptId)
                .HasConstraintName("expense_ibfk_2");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_ibfk_3");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_ibfk_1");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("PRIMARY");

            entity.ToTable("receipt");

            entity.HasIndex(e => e.UploadedBy, "uploaded_by");

            entity.Property(e => e.ReceiptId).HasColumnName("receipt_id");
            entity.Property(e => e.ReceiptUrl)
                .HasMaxLength(255)
                .HasColumnName("receipt_url");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("upload_date");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("receipt_ibfk_1");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.SupplierName, "supplier_name").IsUnique();

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName).HasColumnName("supplier_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
