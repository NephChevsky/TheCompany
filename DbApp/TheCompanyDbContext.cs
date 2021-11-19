using Microsoft.EntityFrameworkCore;
using ModelsApp;
using ModelsApp.DbInterfaces;
using ModelsApp.DbModels;
using ModelsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Models
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
        public virtual DbSet<Individual> Individuals { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<AdditionalField> AdditionalFields { get; set; }
        public virtual DbSet<ExtractionSettings> ExtractionSettings { get; set; }
        public virtual DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer("Server=localhost;Database=TheCompany;Trusted_Connection=True;"); // TODO: use configuration file
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

            modelBuilder.Entity<Individual>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CustomerNumber)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .IsRequired();

                entity.Property(e => e.FirstName);

                entity.Property(e => e.Email);

                entity.Property(e => e.PhoneNumber);

                entity.Property(e => e.MobilePhoneNumber);

                entity.Property(e => e.Address);

                entity.Property(e => e.Owner) // TODO: handle owner automatically
                    .IsRequired(); 

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.InvoiceNumber);

                entity.Property(e => e.CustomerNumber);

                entity.Property(e => e.CustomerId);

                entity.Property(e => e.CustomerFirstName);

                entity.Property(e => e.CustomerLastName);

                entity.Property(e => e.CustomerAddress);

                entity.Property(e => e.LockedBy);

                entity.Property(e => e.FileId)
                    .IsRequired();

                entity.Property(e => e.FileName)
                    .IsRequired();

                entity.Property(e => e.FileSize)
                    .IsRequired();

                entity.Property(e => e.ShouldBeExtracted)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsExtracted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ExtractId);

                entity.Property(e => e.ExtractDateTime);

                entity.Property(e => e.Owner) // TODO: handle owner automatically
                    .IsRequired(); 

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

            modelBuilder.Entity<AdditionalField>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DataSource)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Owner) // TODO: handle owner automatically
                    .IsRequired();

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

            modelBuilder.Entity<ExtractionSettings>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DataSource)
                    .IsRequired();

                entity.Property(e => e.IsLineItem)
                    .IsRequired();

                entity.Property(e => e.Field)
                    .IsRequired();

                entity.Property(e => e.X);

                entity.Property(e => e.Y);

                entity.Property(e => e.Height);

                entity.Property(e => e.Width);

                entity.Property(e => e.Owner) // TODO: handle owner automatically
                    .IsRequired(); 

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

            modelBuilder.Entity<InvoiceLineItem>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.InvoiceId)
                    .IsRequired();

                entity.Property(e => e.Reference);

                entity.Property(e => e.Description);

                entity.Property(e => e.Quantity);

                entity.Property(e => e.UnitaryPrice);

                entity.Property(e => e.Price);

                entity.Property(e => e.Owner) // TODO: handle owner automatically
                    .IsRequired();

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreationDateTime)
                   .IsRequired();

                entity.Property(e => e.LastModificationDateTime);
            });

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
                    if (item.State == EntityState.Added && entity.CreationDateTime==null)
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

    public static class QueryableExtensions
    {
        public static IQueryable<T> FilterDynamic<T>(this IQueryable<T> query, List<Filter> filters)
        {
            if (filters != null)
			{
                filters.ForEach(x =>
                {
                    var param = Expression.Parameter(typeof(T), "e");
                    var prop = Expression.PropertyOrField(param, x.FieldName);
                    Expression<Func<T, bool>> predicate;
                    switch (x.Operator)
                    {
                        case "=":
                            Expression left = prop;
                            Expression right;
                            if (prop.Type == typeof(Guid))
							{
                                right = Expression.Constant(Guid.Parse(x.FieldValue));
                            }
                            else
							{
                                right = Expression.Constant(x.FieldValue);
                            }
                            Expression exp = Expression.Equal(left, right);
                            predicate = Expression.Lambda<Func<T, bool>>(exp, param);
                            break;
                        default:
                            throw new Exception("InvalidOperator");
                    }
                    query = query.Where(predicate);
                });
            }
            
            return query;
        }
    }
}
