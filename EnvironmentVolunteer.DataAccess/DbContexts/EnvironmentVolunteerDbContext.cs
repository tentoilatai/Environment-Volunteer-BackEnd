using EnvironmentVolunteer.Core.Enums;
using EnvironmentVolunteer.Core.Exceptions;
using EnvironmentVolunteer.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.DbContexts
{
    public class EnvironmentVolunteerDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly EnvironmentVolunteer.Core.ApiModels.UserContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public EnvironmentVolunteerDbContext(DbContextOptions<EnvironmentVolunteerDbContext> options, EnvironmentVolunteer.Core.ApiModels.UserContext userContext, IConfiguration configuration, IServiceProvider serviceProvider) : base(options)
        {
            _userContext = userContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public virtual DbSet<AuditChange> AuditChanges { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<UserToken>().ToTable("UserTokens");
            builder.Entity<AuditChange>();

            builder.Entity<User>()
               .HasIndex(u => u.UserName)
               .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int rowCount;
                var mappings = ChangeAudit();
                rowCount = await base.SaveChangesAsync(cancellationToken);

                mappings.ForEach(m =>
                {
                    m.AuditChange.RecordId = GetRecordId(m);
                });

                await AuditChanges.AddRangeAsync(mappings.Select(m => m.AuditChange));

                await base.SaveChangesAsync();
                return rowCount;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ErrorException(StatusCodeEnum.ConcurrencyConflict);
            }
        }

        private string GetRecordId(AuditChangeMapping m)
        {
            if (m.DbRecord != null)
            {
                return m.DbRecord.Id.ToString();
            }
            else if (m.User != null)
            {
                return m.User.Id.ToString();
            }
            else if (m.GuidTableRecord != null)
            {
                return m.GuidTableRecord.Id.ToString();
            }
            else if (m.IntIdTableRecord != null)
            {
                return m.IntIdTableRecord.Id.ToString();
            }
            else
            {
                return "";
            }
        }

        private List<AuditChangeMapping> ChangeAudit()
        {
            DateTime now = DateTime.Now;

            var entityEntries = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added ||
                            x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted).ToList();

            List<AuditChangeMapping> result = new List<AuditChangeMapping>();

            foreach (EntityEntry entityEntry in entityEntries)
            {
                var entities = new List<string> { "User", "Merchant", "MerchantUser", "UserRole", "Transaction", "PaymentCard", "BankAccount", "Subscription", "BillingInfo", "SupportRequest", "Invoice", "UploadFile", "InvoiceStatus" };
                var entityName = entityEntry.Entity.GetType().Name;

                if (entities.Contains(entityName))
                {
                    var auditChange = CreateAuditAsync(entityEntry, now);
                    if (auditChange != null)
                    {
                        result.AddRange(auditChange);
                    }
                }
            }

            return result;
        }

        private IEnumerable<AuditChangeMapping> CreateAuditAsync(EntityEntry entityEntry, DateTime timeStamp)
        {
            if (entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Deleted)
            {
                var changeAudit = GetChangeAuditAsync(entityEntry, timeStamp);
                yield return changeAudit;
            }
            else
            {
                var excludeColumns = new List<string> { "ConcurrencyStamp", "UpdatedAt", "CreatedAt", "CreatedBy", "UpdatedBy" };
                foreach (var prop in entityEntry.Properties)
                {

                    if (!excludeColumns.Contains(prop.Metadata.Name))
                    {
                        string oldValue = string.Empty;
                        string newValue = string.Empty;
                        if (prop.Metadata.ClrType == typeof(DateTime) || prop.Metadata.ClrType == typeof(DateTime?))
                        {
                            oldValue = prop.OriginalValue != null ? ((DateTime)prop.OriginalValue).ToString("o") : string.Empty;
                            newValue = prop.CurrentValue != null ? ((DateTime)prop.CurrentValue).ToString("o") : string.Empty;
                        }
                        else
                        {
                            oldValue = prop.OriginalValue?.ToString() ?? string.Empty;
                            newValue = prop.CurrentValue?.ToString() ?? string.Empty;
                        }


                        if (oldValue != newValue)
                        {
                            var changeAudit = GetChangeAuditAsync(entityEntry, timeStamp);
                            changeAudit.AuditChange.ColumnEffect = prop.Metadata.Name;
                            changeAudit.AuditChange.OldValue = oldValue;
                            changeAudit.AuditChange.NewValue = newValue;
                            yield return changeAudit;
                        }
                    }
                }
            }
        }

        private AuditChangeMapping GetChangeAuditAsync(EntityEntry entityEntry, DateTime timeStamp)
        {
            var properties = entityEntry.Properties;
            var primaryKey = properties.Where(p => p.Metadata.IsPrimaryKey()).FirstOrDefault();
            var auditChange = new AuditChange
            {
                EntityName = entityEntry.Entity.GetType().Name,
                Action = entityEntry.State.ToString(),
                RecordId = primaryKey != null ? primaryKey.CurrentValue.ToString() : "",
                UserId = _userContext.UserId
            };

            if (_userContext.ActionId != null)
            {
                auditChange.ActionId = _userContext.ActionId;
            }

            var mapping = new AuditChangeMapping
            {
                AuditChange = auditChange
            };

            if (entityEntry.Entity is User)
            {
                mapping.User = (User)entityEntry.Entity;
            }
            else if (entityEntry.Entity is UserRole)
            {
                mapping.UserRole = (UserRole)entityEntry.Entity;
            }
            else if (entityEntry.Entity is BaseTable<Guid>)
            {
                mapping.GuidTableRecord = (BaseTable<Guid>)entityEntry.Entity;
            }
            else if (entityEntry.Entity is BaseTable<int>)
            {
                mapping.IntIdTableRecord = (BaseTable<int>)entityEntry.Entity;
            }
            else
            {
                mapping.DbRecord = (BaseTable<Guid>)entityEntry.Entity;
            }

            return mapping;
        }

    }

    class AuditChangeMapping
    {
        public AuditChange AuditChange { get; set; }
        public BaseTable<Guid> DbRecord { get; set; }
        public User User { get; set; }
        public UserRole UserRole { get; set; }
        public BaseTable<Guid> GuidTableRecord { get; set; }
        public BaseTable<int> IntIdTableRecord { get; set; }
    }
}
