using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BackEndApp.Models
{
	public partial class TheCompanyDbContext : DbContext
	{
		public TheCompanyDbContext(DbContextOptions<TheCompanyDbContext> options) : base(options)
		{
		}

		public TheCompanyDbContext()
		{
		}

		public virtual DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfigurationRoot config = builder.Build();
			optionsBuilder.UseSqlServer(config.GetConnectionString("Database"));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.LastLoginDateTime);
                
                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

            modelBuilder.Entity<User>().HasIndex(t => new { t.Login }).IsUnique(true);

            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeleteable).IsAssignableFrom(type.ClrType))
                    modelBuilder.SetSoftDeleteFilter(type.ClrType);
            }

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override int SaveChanges()
        {
            SoftDelete();
            TimeTrack();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SoftDelete();
            TimeTrack();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SoftDelete()
        {
            ChangeTracker.DetectChanges();
            var markedAsDeleted = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
            foreach (var item in markedAsDeleted)
            {
                if (item.Entity is ISoftDeleteable entity)
                {
                    item.State = EntityState.Unchanged;
                    entity.Deleted = true;
                }
            }
        }

        private void TimeTrack()
        {
            ChangeTracker.DetectChanges();
            var markedEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in markedEntries)
            {
                if (item.Entity is IDateTimeTrackable entity)
                {
                    DateTime now = DateTime.Now;
                    entity.LastModificationDateTime = now;
                    if (item.State == EntityState.Added)
                    {
                        entity.CreationDateTime = now;
                    }
                }
            }
        }
    }

    public static class EFFilterExtensions
    {
        public static void SetSoftDeleteFilter(this ModelBuilder modelBuilder, Type entityType)
        {
            SetSoftDeleteFilterMethod.MakeGenericMethod(entityType)
                .Invoke(null, new object[] { modelBuilder });
        }

        static readonly MethodInfo SetSoftDeleteFilterMethod = typeof(EFFilterExtensions)
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteFilter");

        public static void SetSoftDeleteFilter<TEntity>(this ModelBuilder modelBuilder)
            where TEntity : class, ISoftDeleteable
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.Deleted);
        }
    }
}
