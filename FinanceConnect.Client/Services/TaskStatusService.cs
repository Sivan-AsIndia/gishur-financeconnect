using FinanceConnect.Client.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace FinanceConnect.Client.Services
{
    public class TaskStatusService
    {
        private readonly List<TaskStatusViewModel> _statuses = new();

        public TaskStatusService()
        {
            SeedStatuses();
        }

        public List<TaskStatusViewModel> GetAll()
            => _statuses.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList();

        public void Create(TaskStatusViewModel status)
        {
            status.Id = Guid.NewGuid();
            _statuses.Add(status);
        }

        public bool Update(TaskStatusViewModel status)
        {
            var existing = _statuses.FirstOrDefault(x => x.Id == status.Id);

            if (existing == null)
                return false;

            existing.Name = status.Name;
            existing.Color = status.Color;
            existing.SortOrder = status.SortOrder;
            existing.IsActive = status.IsActive;

            return true;
        }


        public bool Delete(Guid id)
        {
            var status = _statuses.FirstOrDefault(x => x.Id == id);

            if (status == null)
                return false;

            _statuses.Remove(status);

            return true;
        }

        private void SeedStatuses()
        {
            if (_statuses.Any()) return;

            _statuses.Add(new TaskStatusViewModel { Id = Guid.NewGuid(), Name = "Open", IsDefault = true, SortOrder = 1, Color= "bg-danger-transparent text-danger" });
            _statuses.Add(new TaskStatusViewModel { Id = Guid.NewGuid(), Name = "In Progress", IsDefault = true, SortOrder = 2, Color = "bg-warning-transparent text-warning" });
            _statuses.Add(new TaskStatusViewModel { Id = Guid.NewGuid(), Name = "Completed", IsDefault = true, SortOrder = 3, Color = "bg-success-transparent text-success" });
        }
    }
}
