using Microsoft.EntityFrameworkCore;
using ConnectDbAvalonia.Models; // Подключаем твои модели (House, Shift и т.д.)

namespace ConnectDbAvalonia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблицы
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingItem> BookingItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- НАСТРОЙКА HOUSE (Домики) ---
            modelBuilder.Entity<House>(entity =>
            {
                entity.HasKey(e => e.HouseId).HasName("pk_houses");
                entity.ToTable("houses");
                entity.Property(e => e.HouseId).HasColumnName("house_id");
                entity.Property(e => e.HouseName).HasMaxLength(150).HasColumnName("house_name");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.ImagePath).HasMaxLength(150).HasColumnName("image_path");
                entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)").HasColumnName("base_price");

                entity.HasOne(d => d.Category).WithMany(p => p.Houses)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("fk_houses_categories");
            });

            // --- НАСТРОЙКА USERS (Пользователи) ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("pk_users");
                entity.ToTable("users");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Login).HasMaxLength(50).HasColumnName("login");
                entity.Property(e => e.LastName).HasMaxLength(60).HasColumnName("last_name");
                entity.Property(e => e.FirstName).HasMaxLength(60).HasColumnName("first_name");
                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("fk_users_roles");
            });

            // --- НАСТРОЙКА ROLES (Роли) ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("pk_roles");
                entity.ToTable("roles");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("role_name");
            });

            // --- НАСТРОЙКА SHIFTS (Заезды) ---
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(e => e.ShiftId).HasName("pk_shifts");
                entity.ToTable("shifts");
                entity.Property(e => e.ShiftId).HasColumnName("shift_id");
                entity.Property(e => e.HouseId).HasColumnName("house_id");
                entity.Property(e => e.ShiftName).HasMaxLength(50).HasColumnName("shift_name");
                entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");

                entity.HasOne(d => d.House).WithMany(p => p.Shifts)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("fk_shifts_houses");
            });

            // --- НАСТРОЙКА BOOKINGS (Брони) ---
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId).HasName("pk_bookings");
                entity.ToTable("bookings");
                entity.Property(e => e.BookingId).HasColumnName("booking_id");
                entity.Property(e => e.BookingDate).HasColumnName("booking_date");
                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("fk_bookings_users");
            });

            // --- НАСТРОЙКА BOOKING ITEMS (Позиции брони) ---
            modelBuilder.Entity<BookingItem>(entity =>
            {
                entity.HasKey(e => e.BookingItemId).HasName("pk_booking_items");
                entity.ToTable("booking_items");
                entity.Property(e => e.BookingItemId).HasColumnName("booking_item_id");
                entity.Property(e => e.BookingId).HasColumnName("booking_id");
                entity.Property(e => e.ShiftId).HasColumnName("shift_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)").HasColumnName("unit_price");

                entity.HasOne(d => d.Booking).WithMany(p => p.BookingItems)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("fk_items_bookings");

                entity.HasOne(d => d.Shift).WithMany(p => p.BookingItems)
                    .HasForeignKey(d => d.ShiftId)
                    .HasConstraintName("fk_items_shifts");
            });
        }
    }
}