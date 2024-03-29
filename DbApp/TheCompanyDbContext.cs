﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
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
        private Guid Owner;

        public TheCompanyDbContext(DbContextOptions<TheCompanyDbContext> options) : base(options)
        {
        }

        public TheCompanyDbContext(Guid owner)
        {
            Owner = owner;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Individual> Individuals { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Quote> Quotes { get; set; }
        public virtual DbSet<AdditionalFieldDefinition> AdditionalFieldDefinitions { get; set; }
        public virtual DbSet<AdditionalField> AdditionalFields { get; set; }
        public virtual DbSet<ExtractionSettings> ExtractionSettings { get; set; }
        public virtual DbSet<LineItemDefinition> LineItemDefinitions { get; set; }
        public virtual DbSet<LineItem> LineItems { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FilePreview> FilePreviews { get; set; }
        public virtual DbSet<Company> Companies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost;Database=TheCompany;Trusted_Connection=True;"); // TODO: use configuration file
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
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

                AddGenericFields<User>(entity);
            });

            modelBuilder.Entity<User>().HasIndex(t => new { t.Login }).IsUnique(true);

            modelBuilder.Entity<Individual>(entity =>
            {
                entity.Property(e => e.CustomerNumber)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .IsRequired();

                entity.Property(e => e.FirstName);

                entity.Property(e => e.Email);

                entity.Property(e => e.PhoneNumber);

                entity.Property(e => e.MobilePhoneNumber);

                entity.Property(e => e.Address);

                AddGenericFields<Individual>(entity);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Number);

                entity.Property(e => e.RecipientNumber);

                entity.Property(e => e.RecipientId);

                entity.Property(e => e.RecipientFirstName);

                entity.Property(e => e.RecipientLastName);

                entity.Property(e => e.RecipientAddress);

                AddGenericFields<Invoice>(entity);
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(e => e.Number);

                entity.Property(e => e.RecipientNumber);

                entity.Property(e => e.RecipientId);

                entity.Property(e => e.RecipientFirstName);

                entity.Property(e => e.RecipientLastName);

                entity.Property(e => e.RecipientAddress);

                AddGenericFields<Quote>(entity);
            });

            modelBuilder.Entity<AdditionalFieldDefinition>(entity =>
            {
                entity.Property(e => e.DataSource)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                AddGenericFields<AdditionalField>(entity);
            });

            modelBuilder.Entity<AdditionalField>(entity =>
            {
                entity.Property(e => e.SourceId)
                    .IsRequired();

                entity.Property(e => e.FieldId)
                    .IsRequired();

                entity.Property(e => e.Value);

                AddGenericFields<AdditionalField>(entity);
            });

            modelBuilder.Entity<ExtractionSettings>(entity =>
            {
                entity.Property(e => e.DataSource)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.X);

                entity.Property(e => e.Y);

                entity.Property(e => e.Height);

                entity.Property(e => e.Width);

                AddGenericFields<ExtractionSettings>(entity);
            });

            modelBuilder.Entity<LineItem>(entity =>
            {
                entity.Property(e => e.InvoiceId)
                    .IsRequired();

                entity.Property(e => e.Reference);

                entity.Property(e => e.Description);

                entity.Property(e => e.Quantity);

                entity.Property(e => e.Unit);

                entity.Property(e => e.VAT);

                entity.Property(e => e.PriceVAT);

                entity.Property(e => e.PriceNoVAT);

                entity.Property(e => e.TotalPrice);

                AddGenericFields<LineItem>(entity);
            });

            modelBuilder.Entity<LineItemDefinition>(entity =>
            {
                entity.Property(e => e.Reference)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.Unit);

                entity.Property(e => e.VAT);

                entity.Property(e => e.PriceNoVAT);

                entity.Property(e => e.PriceVAT);

                AddGenericFields<LineItemDefinition>(entity);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.FilePath)
                    .IsRequired();

                AddGenericFields<File>(entity);
            });

            modelBuilder.Entity<FilePreview>(entity =>
            {
                entity.Property(e => e.FileId)
                    .IsRequired();

                entity.Property(e => e.Page)
                    .IsRequired();

                AddGenericFields<FilePreview>(entity);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Address);

                entity.Property(e => e.PhoneNumber);

                entity.Property(e => e.MobilePhoneNumber);

                entity.Property(e => e.Siret);

                entity.Property(e => e.Logo);

                AddGenericFields<Company>(entity);
            });

            Expression<Func<ISoftDeleteable, bool>> filterSoftDeleteable = bm => !bm.Deleted;
            Expression<Func<IOwnable, bool>> filterOwnable = bm => Owner == Guid.Empty || bm.Owner == Owner;
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                Expression filter = null;
                var param = Expression.Parameter(type.ClrType, "entity");
                if (typeof(ISoftDeleteable).IsAssignableFrom(type.ClrType))
                {
                    filter = AddFilter(filter, ReplacingExpressionVisitor.Replace(filterSoftDeleteable.Parameters.First(), param, filterSoftDeleteable.Body));
                }

                if (typeof(IOwnable).IsAssignableFrom(type.ClrType))
                {
                    filter = AddFilter(filter, ReplacingExpressionVisitor.Replace(filterOwnable.Parameters.First(), param, filterOwnable.Body));
                }

                if (filter != null)
                {
                    type.SetQueryFilter(Expression.Lambda(filter, param));
                }
            }

            OnModelCreatingPartial(modelBuilder);
        }

        private Expression AddFilter(Expression filter, Expression newFilter)
        {
            if (filter == null)
            {
                filter = newFilter;
            }
            else
            {
                filter = Expression.And(filter, newFilter);
            }
            return filter;
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void AddGenericFields<T>(EntityTypeBuilder entity)
        {
            entity.Property("Id")
                  .ValueGeneratedOnAdd();

            if (typeof(IAttachment).IsAssignableFrom(typeof(T)))
            {
                entity.Property("FileId");

                entity.Property("FileName");

                entity.Property("FileSize");
            }

            if (typeof(IDateTimeTrackable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("CreationDateTime")
                   .IsRequired();

                entity.Property("LastModificationDateTime");
            }

            if (typeof(IExtractable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("ShouldBeExtracted")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property("IsExtracted")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property("ExtractId");

                entity.Property("ExtractDateTime");
            }

            if (typeof(IGeneratable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("ShouldBeGenerated")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property("IsGenerated")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property("GenerationDateTime");
            }

            if (typeof(ILockable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("LockedBy");
            }

            if (typeof(IOwnable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("Owner")
                      .IsRequired();
            }

            if (typeof(ISoftDeleteable).IsAssignableFrom(typeof(T)))
            {
                entity.Property("Deleted")
                    .IsRequired()
                    .HasDefaultValue(false);
            }
        }

        public override int SaveChanges()
        {
            SoftDelete();
            TimeTrack();
            Ownable();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SoftDelete();
            TimeTrack();
            Ownable();
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
                    if (item.State == EntityState.Added && (entity.CreationDateTime==null || entity.CreationDateTime==DateTime.MinValue))
                    {
                        entity.CreationDateTime = now;
                    }
                }
            }
        }

        private void Ownable()
        {
            ChangeTracker.DetectChanges();
            var markedEntries = ChangeTracker.Entries();
            foreach (var item in markedEntries)
            {
                if (item.Entity is IOwnable entity)
                {
                    if (item.State == EntityState.Added)
                    {
                        if (Owner != Guid.Empty)
                        {
                            entity.Owner = Owner;
                        }
                        else
                        {
                            throw new Exception("Unauthorized database insert detected");
                        }
                    }
                    if (entity.Owner != Owner && Owner != Guid.Empty)
                    {
                        throw new Exception("Unauthorized database request detected");
                    }
                }
            }
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
                                if (x.FieldValue == null)
                                    throw new Exception(x.FieldName + " property value cannot be null in query filters");
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
