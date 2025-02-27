using EnvironmentVolunteer.DataAccess.Models;
using System.Security;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Permission> PermissionRepository { get; set; }
        IRepository<RolePermission> RolePermissionRepository { get; set; }

        Task SaveChangesAsync();
    }
}
