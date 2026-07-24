using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class CoreDbContext : IdentityDbContext<ProfileModel, IdentityRole<Guid>, Guid>
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProfileModel>(e =>
            {
                e.ToTable("Profile");
                e.HasKey(x => x.Id);

                e.Property(i => i.UserName).HasMaxLength(200).IsRequired();
                e.HasIndex(i => i.UserName).IsUnique();
                e.Property(i => i.FirstName).HasMaxLength(200).IsRequired();
                e.Property(i => i.LastName).HasMaxLength(200).IsRequired();
                e.Property(i => i.ProfilePictureUrl).IsRequired(false);
            });

            builder.Entity<CarBodyModel>(e =>
            {
                e.ToTable("CarBody");
                e.HasKey(x => x.Id);
                e.Property(i => i.BodyType).HasMaxLength(200).IsRequired();
            });

            builder.Entity<CarBrandModel>(e =>
            {
                e.ToTable("CarBrand");
                e.HasKey(x => x.Id);
                e.Property(i => i.Brand).HasMaxLength(200).IsRequired();
            });

            builder.Entity<CarEngineModel>(e =>
            {
                e.ToTable("CarEngine");
                e.HasKey(x => x.Id);
                e.Property(i => i.EngineType).HasMaxLength(200).IsRequired();
            });

            builder.Entity<CarModel>(e =>
            {
                e.ToTable("Car");
                e.HasKey(x => x.Id);

                e.Property(i => i.Model).HasMaxLength(200).IsRequired();
                e.Property(i => i.ManufactureDate).IsRequired();
                e.Property(i => i.CarPictureUrl).IsRequired(false);

                e.HasOne(i => i.BodyType)
                    .WithMany()
                    .HasForeignKey(i => i.BodyTypeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(i => i.Brand)
                    .WithMany()
                    .HasForeignKey(i => i.BrandId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(i => i.EngineType)
                    .WithMany()
                    .HasForeignKey(i => i.EngineTypeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserCarsModel>(e =>
            {
                e.ToTable("UserCars");

                e.HasIndex(x => x.UserId);
                e.HasIndex(x => x.CarId).IsUnique();

                e.HasKey(x => new { x.UserId, x.CarId });

                e.HasOne<ProfileModel>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<CarModel>()
                    .WithMany()
                    .HasForeignKey(x => x.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<HandlersUsersModel>(e =>
            {
                e.ToTable("HandlersUsers");

                e.HasIndex(x => x.HandlerId);
                e.HasIndex(x => x.UserId);

                e.HasKey(x => new { x.UserId, x.HandlerId });

                e.HasOne<ProfileModel>()
                    .WithMany()
                    .HasForeignKey(x => x.HandlerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<ProfileModel>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<HandlersCarsModel>(e =>
            {
                e.ToTable("HandlersCars");

                e.HasIndex(x => x.HandlerId);
                e.HasIndex(x => x.CarId);

                e.HasKey(x => new { x.CarId, x.HandlerId });

                e.HasOne<ProfileModel>()
                    .WithMany()
                    .HasForeignKey(x => x.HandlerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<CarModel>()
                    .WithMany()
                    .HasForeignKey(x => x.CarId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
