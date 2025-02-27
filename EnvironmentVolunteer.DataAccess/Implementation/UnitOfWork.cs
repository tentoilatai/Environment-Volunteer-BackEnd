using EnvironmentVolunteer.DataAccess.DbContexts;
using EnvironmentVolunteer.DataAccess.Interfaces;
using EnvironmentVolunteer.DataAccess.Models;
using System.Security;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            EnvironmentVolunteerDbContext dbContext,
            IRepository<Permission> permissionRepository,
            IRepository<RolePermission> rolePermissionRepository

        )
        {
            _dbContext = dbContext;
            PermissionRepository = permissionRepository;
            RolePermissionRepository = rolePermissionRepository;
        }

        private readonly EnvironmentVolunteerDbContext _dbContext;
        public IRepository<Permission> PermissionRepository { get; set; }
        public IRepository<RolePermission> RolePermissionRepository { get; set; }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
