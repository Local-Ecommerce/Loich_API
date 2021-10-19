﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DAL.Models
{
    public partial class LoichDBContext : DbContext
    {
        public LoichDBContext()
        {
        }

        public LoichDBContext(DbContextOptions<LoichDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<CollectionMapping> CollectionMappings { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual DbSet<LocalZone> LocalZones { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<MerchantLevel> MerchantLevels { get; set; }
        public virtual DbSet<MerchantStore> MerchantStores { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCombination> ProductCombinations { get; set; }
        public virtual DbSet<ProductInMenu> ProductInMenus { get; set; }
        public virtual DbSet<StoreMenuDetail> StoreMenuDetails { get; set; }
        public virtual DbSet<SystemCategory> SystemCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=LoichDB;User ID=sa;pwd=Hanquang@123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.AvatarImage).IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProfileImage).IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.ToTable("Collection");

                entity.Property(e => e.CollectionId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CollectionID");

                entity.Property(e => e.MerchantId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantID");

                entity.HasOne(d => d.Merchant)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.MerchantId)
                    .HasConstraintName("FK_tblCollection_tblMerchant");
            });

            modelBuilder.Entity<CollectionMapping>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.ProductId })
                    .HasName("PK_tblCollectionMapping");

                entity.ToTable("CollectionMapping");

                entity.Property(e => e.CollectionId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CollectionID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.CollectionMappings)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblCollectionMapping_tblCollection");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CollectionMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblCollectionMapping_tblProduct");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Gender).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_tblCustomer_tblAccount");
            });

            modelBuilder.Entity<DeliveryAddress>(entity =>
            {
                entity.ToTable("DeliveryAddress");

                entity.Property(e => e.DeliveryAddressId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DeliveryAddressID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.DeliveryAddress1).HasColumnName("DeliveryAddress");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_tblDeliveryAddress_tblCustomer");
            });

            modelBuilder.Entity<LocalZone>(entity =>
            {
                entity.ToTable("LocalZone");

                entity.Property(e => e.LocalZoneId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LocalZoneID");

                entity.Property(e => e.Address).HasMaxLength(250);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MenuID");

                entity.Property(e => e.MerchantId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantID");

                entity.HasOne(d => d.Merchant)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.MerchantId)
                    .HasConstraintName("FK_tblMenu_tblMerchant");
            });

            modelBuilder.Entity<Merchant>(entity =>
            {
                entity.ToTable("Merchant");

                entity.Property(e => e.MerchantId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.LevelId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LevelID");

                entity.Property(e => e.MerchantName).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Merchants)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_tblMerchant_tblAccount");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Merchants)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_tblMerchant_tblMerchantLevel");
            });

            modelBuilder.Entity<MerchantLevel>(entity =>
            {
                entity.HasKey(e => e.LevelId)
                    .HasName("PK_tblMerchantLevel");

                entity.ToTable("MerchantLevel");

                entity.Property(e => e.LevelId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LevelID");
            });

            modelBuilder.Entity<MerchantStore>(entity =>
            {
                entity.ToTable("MerchantStore");

                entity.Property(e => e.MerchantStoreId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantStoreID");

                entity.Property(e => e.LocalZoneId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LocalZoneID");

                entity.Property(e => e.MerchantId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantID");

                entity.Property(e => e.StoreName).HasMaxLength(250);

                entity.HasOne(d => d.LocalZone)
                    .WithMany(p => p.MerchantStores)
                    .HasForeignKey(d => d.LocalZoneId)
                    .HasConstraintName("FK_tblMerchantStore_tblLocalZone");

                entity.HasOne(d => d.Merchant)
                    .WithMany(p => p.MerchantStores)
                    .HasForeignKey(d => d.MerchantId)
                    .HasConstraintName("FK_tblMerchantStore_tblMerchant");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OrderID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.MerchantStoreId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantStoreID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_tblOrder_tblCustomer");

                entity.HasOne(d => d.MerchantStore)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.MerchantStoreId)
                    .HasConstraintName("FK_tblOrder_tblMerchantStore");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderDetailId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OrderDetailID");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OrderID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_tblOrderDetail_tblOrder");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_tblOrderDetail_tblProduct");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PaymentID");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OrderID");

                entity.Property(e => e.PaymentMethodId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PaymentMethodID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_tblPayment_tblOrder");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK_tblPayment_tblPaymentMethod");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod");

                entity.Property(e => e.PaymentMethodId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PaymentMethodID");

                entity.Property(e => e.PaymentName).HasMaxLength(250);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Color).HasMaxLength(250);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.Property(e => e.ProductType).HasMaxLength(250);

                entity.Property(e => e.Size).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasMaxLength(250);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory");

                entity.Property(e => e.ProductCategoryId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductCategoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MerchantId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.Property(e => e.SystemCategoryId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SystemCategoryID");

                entity.HasOne(d => d.Merchant)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.MerchantId)
                    .HasConstraintName("FK_tblProductCategory_tblMerchant");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_tblProductCategory_tblProduct");

                entity.HasOne(d => d.SystemCategory)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.SystemCategoryId)
                    .HasConstraintName("FK_tblProductCategory_tblSystemCategory");
            });

            modelBuilder.Entity<ProductCombination>(entity =>
            {
                entity.HasKey(e => new { e.BaseProductId, e.ProductId })
                    .HasName("PK_tblProductCombination");

                entity.ToTable("ProductCombination");

                entity.Property(e => e.BaseProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BaseProductID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.BaseProduct)
                    .WithMany(p => p.ProductCombinationBaseProducts)
                    .HasForeignKey(d => d.BaseProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblProductCombination_tblProduct1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductCombinationProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblProductCombination_tblProduct");
            });

            modelBuilder.Entity<ProductInMenu>(entity =>
            {
                entity.ToTable("ProductInMenu");

                entity.Property(e => e.ProductInMenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductInMenuID");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MenuID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.ProductInMenus)
                    .HasForeignKey(d => d.MenuId)
                    .HasConstraintName("FK_tblProductInMenu_tblMenu");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductInMenus)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_tblProductInMenu_tblProduct");
            });

            modelBuilder.Entity<StoreMenuDetail>(entity =>
            {
                entity.HasKey(e => e.PriceMenuDetailId)
                    .HasName("PK_tblStoreMenuDetail");

                entity.ToTable("StoreMenuDetail");

                entity.Property(e => e.PriceMenuDetailId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PriceMenuDetailID");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MenuID");

                entity.Property(e => e.MerchantStoreId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MerchantStoreID");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.StoreMenuDetails)
                    .HasForeignKey(d => d.MenuId)
                    .HasConstraintName("FK_tblStoreMenuDetail_tblMenu");

                entity.HasOne(d => d.MerchantStore)
                    .WithMany(p => p.StoreMenuDetails)
                    .HasForeignKey(d => d.MerchantStoreId)
                    .HasConstraintName("FK_tblStoreMenuDetail_tblMerchantStore");
            });

            modelBuilder.Entity<SystemCategory>(entity =>
            {
                entity.ToTable("SystemCategory");

                entity.Property(e => e.SystemCategoryId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SystemCategoryID");

                entity.Property(e => e.SysCategoryName).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasMaxLength(250);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}