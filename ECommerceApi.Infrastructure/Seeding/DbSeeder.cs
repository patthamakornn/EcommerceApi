using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Seeding
{
	public static class DbSeeder
	{
		public static async Task SeedAsync(ApplicationDbContext context)
		{
			if (!await context.Products.AnyAsync())
			{
				var products = new List<Product>
				{
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
				};

				await context.Products.AddRangeAsync(products);
				await context.SaveChangesAsync();
			}
		}
	}
}
