using VexaDriveAPI.Models;

namespace VexaDriveAPI.Services.Lifecycle
{
    // Simple deterministic state machine for the service request lifecycle
        public class ServiceLifecycle : IServiceLifecycle
        {
            private static readonly Dictionary<ServiceStatus, ServiceStatus[]> _allowed = new()
            {
                { ServiceStatus.RequestCreated, new[] { ServiceStatus.ServiceInProgress } },
                { ServiceStatus.ServiceInProgress, new[] { ServiceStatus.ServiceCompleted } },
                { ServiceStatus.ServiceCompleted, Array.Empty<ServiceStatus>() }
            };        public bool CanTransition(ServiceStatus from, ServiceStatus to)
        {
            if (from == to) return true; // idempotent
            return _allowed.TryGetValue(from, out var next) && next.Contains(to);
        }

        public IEnumerable<ServiceStatus> GetAllowedNext(ServiceStatus from)
        {
            if (_allowed.TryGetValue(from, out var next)) return next;
            return Array.Empty<ServiceStatus>();
        }
    }
}
