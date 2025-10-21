using ECommerceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext
{
	public ApplicationDbContext()
	{
	}

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Cart> Carts { get; set; }

	public virtual DbSet<CartItem> CartItems { get; set; }

	public virtual DbSet<Order> Orders { get; set; }

	public virtual DbSet<OrderItem> OrderItems { get; set; }

	public virtual DbSet<Product> Products { get; set; }

	public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

	public virtual DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasPostgresExtension("uuid-ossp");

		modelBuilder.Entity<Cart>(entity =>
		{
			entity.ToTable("carts");

			entity.HasIndex(e => e.UserId, "carts_user_id_key").IsUnique();

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("created_date");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("updated_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithOne(p => p.Cart).HasForeignKey<Cart>(d => d.UserId);
		});

		modelBuilder.Entity<CartItem>(entity =>
		{
			entity.ToTable("cart_items");

			entity.HasIndex(e => e.CartId, "IX_cart_items_cart_id");

			entity.HasIndex(e => e.ProductId, "IX_cart_items_product_id");

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.CartId).HasColumnName("cart_id");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("created_date");
			entity.Property(e => e.Price)
				.HasPrecision(18, 2)
				.HasColumnName("price");
			entity.Property(e => e.ProductId).HasColumnName("product_id");
			entity.Property(e => e.Quantity).HasColumnName("quantity");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("updated_date");

			entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasForeignKey(d => d.CartId);

			entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasForeignKey(d => d.ProductId);
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.ToTable("orders");

			entity.HasIndex(e => e.UserId, "IX_orders_user_id");

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.OrderDate).HasColumnName("order_date");
			entity.Property(e => e.TotalAmount)
				.HasPrecision(18, 2)
				.HasColumnName("total_amount");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.ToTable("order_items");

			entity.HasIndex(e => e.OrderId, "IX_order_items_order_id");

			entity.HasIndex(e => e.ProductId, "IX_order_items_product_id");

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.OrderId).HasColumnName("order_id");
			entity.Property(e => e.Price)
				.HasPrecision(18, 2)
				.HasColumnName("price");
			entity.Property(e => e.ProductId).HasColumnName("product_id");
			entity.Property(e => e.Quantity).HasColumnName("quantity");

			entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasForeignKey(d => d.OrderId);

			entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasForeignKey(d => d.ProductId);
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.ToTable("products");

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("created_date");
			entity.Property(e => e.Description)
				.HasMaxLength(500)
				.HasColumnName("description");
			entity.Property(e => e.Price)
				.HasPrecision(18, 2)
				.HasColumnName("price");
			entity.Property(e => e.ProductName)
				.HasMaxLength(100)
				.HasColumnName("product_name");
			entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("updated_date");
		});

		modelBuilder.Entity<RefreshToken>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("refresh_token_pkey");

			entity.ToTable("refresh_token");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AccessToken).HasColumnName("access_token");
			entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
			entity.Property(e => e.Token).HasColumnName("token");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("fk_refreshtoken_user");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("users_pkey");

			entity.ToTable("users");

			entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

			entity.Property(e => e.Id)
				.HasDefaultValueSql("uuid_generate_v4()")
				.HasColumnName("id");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("created_date");
			entity.Property(e => e.Email)
				.HasMaxLength(100)
				.HasColumnName("email");
			entity.Property(e => e.FirstName)
				.HasMaxLength(100)
				.HasColumnName("first_name");
			entity.Property(e => e.LastName)
				.HasMaxLength(100)
				.HasColumnName("last_name");
			entity.Property(e => e.PasswordHash)
				.HasMaxLength(255)
				.HasColumnName("password_hash");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("updated_date");
		});

		modelBuilder.Entity<Product>().HasData(
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Apple iPhone 14 Pro",
				Description = "Latest iPhone with A16 Bionic chip, ProMotion display, and triple-camera system.",
				Price = 999.99m,
				StockQuantity = 80,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Samsung Galaxy S23 Ultra",
				Description = "Flagship smartphone with Snapdragon 8 Gen 2, 12GB RAM, and 200MP camera.",
				Price = 1199.99m,
				StockQuantity = 50,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Xiaomi Redmi Note 12 Pro",
				Description = "Mid-range smartphone with AMOLED display, MediaTek Dimensity 1080, and 5000mAh battery.",
				Price = 299.99m,
				StockQuantity = 150,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Apple MacBook Air M2",
				Description = "Thin and light laptop with M2 chip and 13.6-inch Retina display.",
				Price = 1199.99m,
				StockQuantity = 40,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Samsung Galaxy Tab S8",
				Description = "High-end Android tablet with Snapdragon 8 Gen 1 and 11-inch display.",
				Price = 699.99m,
				StockQuantity = 70,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Xiaomi Mi Band 7",
				Description = "Smart fitness tracker with AMOLED display and heart rate monitor.",
				Price = 49.99m,
				StockQuantity = 300,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Apple AirPods Pro (2nd Gen)",
				Description = "Wireless earbuds with active noise cancellation and improved sound quality.",
				Price = 249.99m,
				StockQuantity = 120,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Samsung Galaxy Buds 2 Pro",
				Description = "Premium wireless earbuds with 24-bit Hi-Fi audio and active noise cancellation.",
				Price = 229.99m,
				StockQuantity = 100,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			},
			new Product
			{
				Id = Guid.NewGuid(),
				ProductName = "Xiaomi Mi 11 Lite",
				Description = "Slim and lightweight smartphone with AMOLED display and Snapdragon 732G.",
				Price = 349.99m,
				StockQuantity = 130,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			}
		);


		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
