using Microsoft.EntityFrameworkCore;

namespace VetLineApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CompletedService> CompletedServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Animal entity configuration
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.AnimalId);
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Animals)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.BirthDate)
                      .HasColumnType("date"); // PostgreSQL date tipini kullan
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.Name).IsRequired();
            });

            // Appointment entity configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Appointments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Animal)
                      .WithMany(e => e.Appointments)
                      .HasForeignKey(e => e.AnimalId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Service)
                      .WithMany()
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.AppointmentDateTime)
                      .HasColumnType("timestamp"); // PostgreSQL timestamp tipini kullan
            });

            // UserReview entity configuration
            modelBuilder.Entity<UserReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId);
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserReviews)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.ReviewDate)
                      .HasColumnType("timestamp with time zone");
            });

            // Service entity configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId);
                entity.Property(e => e.Title).IsRequired();
            });

            // CompletedService entity configuration
            modelBuilder.Entity<CompletedService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Animal)
                      .WithMany()
                      .HasForeignKey(e => e.AnimalId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Service)
                      .WithMany()
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Appointment)
                      .WithMany()
                      .HasForeignKey(e => e.AppointmentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.CompletedDate)
                      .HasColumnType("timestamp with time zone");
            });
        }
    }
}
