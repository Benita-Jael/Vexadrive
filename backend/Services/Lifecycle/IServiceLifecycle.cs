using VexaDriveAPI.Models;

namespace VexaDriveAPI.Services.Lifecycle
{
    public interface IServiceLifecycle
    {
        bool CanTransition(ServiceStatus from, ServiceStatus to);
        IEnumerable<ServiceStatus> GetAllowedNext(ServiceStatus from);
    }
}
