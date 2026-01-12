using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;

namespace Modique.Infrastructure.Data
{
    public class ModiqueDbContext : DbContext
    {
        public ModiqueDbContext(DbContextOptions<ModiqueDbContext> options) : base(options) { }

      
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Color> Colors => Set<Color>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<ProductOption> ProductOptions => Set<ProductOption>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        // Address & Location
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

        // Interactions & Content
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewImage> ReviewImages => Set<ReviewImage>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();
        public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();
        public DbSet<ProductView> ProductViews => Set<ProductView>();
        public DbSet<SearchFilter> SearchFilters => Set<SearchFilter>();
        public DbSet<BannerAd> BannerAds => Set<BannerAd>();
        public DbSet<SeoMeta> SeoMetas => Set<SeoMeta>();
        public DbSet<RelatedProduct> RelatedProducts => Set<RelatedProduct>();
        public DbSet<BestsellerRanking> BestsellerRankings => Set<BestsellerRanking>();
        public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
        public DbSet<SizeGuide> SizeGuides => Set<SizeGuide>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<ProductQuestion> ProductQuestions => Set<ProductQuestion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.NoAction; 
            }

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Administrator" },
                new Role { RoleId = 2, Name = "Customer" }
            );

           
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);


            modelBuilder.Entity<RelatedProduct>(entity =>
            {
                entity.HasKey(rp => rp.RelatedProductId);

                entity.HasOne(rp => rp.Product)
                    .WithMany(p => p.RelatedProducts)
                    .HasForeignKey(rp => rp.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rp => rp.RelatedToProduct)
                    .WithMany()
                    .HasForeignKey(rp => rp.RelatedToProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });



        }
    }
}
