using FinanceConnect.Client.Pages.EmployeeManagement.Tasks;
using FinanceConnect.Client.ViewModels;

namespace FinanceConnect.Client.Services
{
    public class TaskService
    {
        private readonly AuthService _authService;
        private readonly EmployeeService _employeeService;
        private readonly TaskStatusService _taskStatusService;
        private  List<TaskViewModel> _tasks = new();
        private readonly List<TaskViewModel> _SeedTasks = new();
        private readonly List<EmployeeViewModel> _employees = new();
        private readonly List<TaskStatusViewModel> _taskStatus = new();

        public TaskService(AuthService authService, EmployeeService employeeService, TaskStatusService taskStatusService)
        {
            _authService = authService;
            _employeeService = employeeService;
            _taskStatusService = taskStatusService;
            _employees = _employeeService.GetAll().Where(e => e.Status == EmployeeStatus.Active)
                .ToList();

            _taskStatus = _taskStatusService.GetAll();

            _SeedTasks = SeedTasks();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _tasks = CloneList(_SeedTasks);
        }

        // Get All
        public List<TaskViewModel> GetAll()
        {
            return _tasks
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        // Get By Id
        public TaskViewModel? GetById(Guid id)
        {
            return _tasks.FirstOrDefault(x => x.Id == id);
        }

        // Create
        public void Create(TaskViewModel task)
        {
            if (string.IsNullOrWhiteSpace(task.StatusName))
            {
                var StatusName = _taskStatus.FirstOrDefault(x => x.Id == task.StatusId);
                task.StatusName = StatusName?.Name;
            }
            task.Id = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;

            _tasks.Add(task);
        }

        public string GenerateTaskCode()
        {
            if (!_tasks.Any())
                return "TSK-0001";

            var lastNumber = _tasks
                .Where(x => !string.IsNullOrEmpty(x.TaskCode))
                .Select(x =>
                {
                    var parts = x.TaskCode.Split('-');
                    return parts.Length > 1 && int.TryParse(parts[1], out var num) ? num : 0;
                })
                .Max();

            return $"TSK-{(lastNumber + 1):D4}";
        }

        // Update
        public void Update(TaskViewModel task)
        {
            var existing = _tasks.FirstOrDefault(x => x.Id == task.Id);


            if (existing == null)
                return;

            if (string.IsNullOrWhiteSpace(task.StatusName))
            {
                var StatusName = _taskStatus.FirstOrDefault(x => x.Id == task.StatusId);
                task.StatusName = StatusName?.Name;
            }


            existing.TaskOwnerId = task.TaskOwnerId;
            existing.TaskOwnerName = task.TaskOwnerName;
            existing.TaskName = task.TaskName;
            existing.Description = task.Description;
            existing.StartDate = task.StartDate;
            existing.DueDate = task.DueDate;
            existing.Reminder = task.Reminder;
            existing.Priority = task.Priority;
            existing.StatusId = task.StatusId;
            existing.StatusName = task.StatusName;

            existing.UpdatedAt = DateTime.UtcNow;
        }

        // Delete
        public void Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == id);

            if (task != null)
                _tasks.Remove(task);
        }

        // Change Status


        public bool UpdateStatus(Guid taskId, Guid statusId, string? statusName = null)
        {
            var task = _tasks.FirstOrDefault(x => x.Id == taskId);

            if (task == null)
                return false;

            task.StatusId = statusId;
            task.StatusName = statusName;
            task.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        // Get Tasks by Employee
        public List<TaskViewModel> GetByEmployee(Guid employeeId)
        {
            return _tasks
                .Where(x => x.TaskOwnerId == employeeId)
                .ToList();
        }

        // Get Overdue Tasks
        public List<TaskViewModel> GetOverdueTasks()
        {
            return _tasks
                .Where(x =>
                    x.DueDate < DateTime.Today &&
                    x.StatusName != "Completed")
                .ToList();
        }

        // Seed Sample Tasks
        private List<TaskViewModel> SeedTasks()
        {

            //if (_tasks.Any())
            //    return;

            var userId = Guid.NewGuid();
            var userName = _authService.CurrentUser?.UserName ?? "System";

            var today = DateTime.Today;

            // Employees
            var employees = _employeeService.GetAll().ToList();

            // Status lookup
            var open = _taskStatus.FirstOrDefault(x => x.Name == "Open");
            var inProgress = _taskStatus.FirstOrDefault(x => x.Name == "In Progress");
            var completed = _taskStatus.FirstOrDefault(x => x.Name == "Completed");

            _tasks.AddRange(new List<TaskViewModel>
    {
        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0001",
            TaskName = "Prepare Financial Report",
            Description = "Generate monthly finance report",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[0].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(3),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0002",
            TaskName = "Vendor Ledger Reconciliation",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[1].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Moderate,
            StatusId = inProgress?.Id,
            StatusName = inProgress?.Name,
            StartDate = today,
            DueDate = today.AddDays(5),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0003",
            TaskName = "Review Purchase Orders",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[2].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(2),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0004",
            TaskName = "Update GST Settings",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[3].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Low,
            StatusId = completed?.Id,
            StatusName = completed?.Name,
            StartDate = today.AddDays(-4),
            DueDate = today.AddDays(-1),
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0005",
            TaskName = "Audit Trial Balance",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[4].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(7),
            CreatedAt = DateTime.UtcNow
        },

        // Overdue Task 1
        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0006",
            TaskName = "Verify Bank Reconciliation",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[5].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Moderate,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today.AddDays(-5),
            DueDate = today.AddDays(-2),
            CreatedAt = DateTime.UtcNow
        },

        // Overdue Task 2
        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0007",
            TaskName = "Customer Aging Review",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[6].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = inProgress?.Id,
            StatusName = inProgress?.Name,
            StartDate = today.AddDays(-4),
            DueDate = today.AddDays(-1),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0008",
            TaskName = "Prepare TDS Filing",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[7].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(8),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0009",
            TaskName = "Review Credit Notes",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[8].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Low,
            StatusId = inProgress?.Id,
            StatusName = inProgress?.Name,
            StartDate = today,
            DueDate = today.AddDays(6),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0010",
            TaskName = "Validate Ledger Opening Balances",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[9].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.High,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(12),
            CreatedAt = DateTime.UtcNow
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0011",
            TaskName = "Check Fiscal Year Settings",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[10].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Moderate,
            StatusId = completed?.Id,
            StatusName = completed?.Name,
            StartDate = today.AddDays(-3),
            DueDate = today.AddDays(-1),
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-4)
        },

        new TaskViewModel
        {
            Id = Guid.NewGuid(),
            TaskCode = "TSK-0012",
            TaskName = "Ledger Adjustment Verification",
            TaskOwnerId = userId,
            TaskOwnerName = userName,
            AssignedToId = employees[11].Id,
            AssignedAt = DateTime.UtcNow,
            Priority = TaskPriority.Moderate,
            StatusId = open?.Id,
            StatusName = open?.Name,
            StartDate = today,
            DueDate = today.AddDays(4),
            CreatedAt = DateTime.UtcNow
        }
    });
            return _tasks;
        }

    }
}