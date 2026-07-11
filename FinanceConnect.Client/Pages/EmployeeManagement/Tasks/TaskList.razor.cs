using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.EmployeeManagement.Tasks
{
    public partial class TaskList
    {
        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] EmployeeService EmployeeService { get; set; } = default!;
        [Inject] TaskService TaskService { get; set; } = default!;
        [Inject] TaskStatusService TaskStatusService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        DotNetObjectReference<TaskList>? objRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);
            }
        }


        private bool isInitialized = false;
        private bool isLoading = false;
        string CurrentView = "list";

        private EditContext _editContext;
        RichTextEditor? _descriptionEditor;
        bool IsEdit = false;
        bool IsStatusEdit = false;
        bool IsInitializing = false;

        public static List<TaskViewModel> TaskLists = new();
        List<TaskStatusViewModel> TaskStatuses = new();
        TaskStatusViewModel EditingStatus = new();
        TaskViewModel Tasks = new();
        TaskViewModel? draggedTask;
        TaskViewModel? SelectedTask = new();
        private TaskViewModel? existing = new();
        List<TaskViewModel> FilteredTasks = new();

        List<EmployeeViewModel> Employees = new();
        TaskStatusViewModel NewStatus = new();
        string searchText = "";
        Guid? selectedStatus = null;
        Guid? selectedEmployee = null;

        int PageSize = 10;
        int CurrentPage = 1;
        int PageWindowSize = 2;
        private bool canDeactivate = true;
        private bool canDelete = true;
        private int VisibleColumnCount;
        int TotalPages =>
            FilteredTasks.Count == 0
                ? 1
                : (int)Math.Ceiling((double)FilteredTasks.Count / PageSize);

        Guid? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilters();
            }
        }

        Guid? SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                ApplyFilters();
            }
        }

        IEnumerable<int> VisiblePages
        {
            get
            {
                if (TotalPages <= PageWindowSize)
                    return Enumerable.Range(1, TotalPages);

                int start = Math.Max(1, CurrentPage - (PageWindowSize / 2));
                int end = start + PageWindowSize - 1;

                if (end > TotalPages)
                {
                    end = TotalPages;
                    start = end - PageWindowSize + 1;
                }

                return Enumerable.Range(start, end - start + 1);
            }
        }

        protected override void OnInitialized()
        {
            TaskLists = TaskService.GetAll();
            Employees = EmployeeService.GetAll();
            TaskStatuses = TaskStatusService.GetAll();
            ApplyFilters();
            _editContext = new EditContext(Tasks);
            isInitialized = true;

        }

        void LoadData()
        {
            TaskLists = TaskService.GetAll();
            Employees = EmployeeService.GetAll();
            TaskStatuses = TaskStatusService.GetAll();
            ApplyFilters();
        }

        void OnSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString() ?? "";
            ApplyFilters();
        }



        void ApplyFilters()
        {
            IEnumerable<TaskViewModel> query = TaskLists;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(t =>
                    (t.TaskName ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (t.Description ?? "").Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            if (selectedStatus.HasValue)
            {
                query = query.Where(t => t.StatusId == selectedStatus);
            }

            if (selectedEmployee.HasValue)
            {
                query = query.Where(t => t.AssignedToId == selectedEmployee.Value);
            }

            FilteredTasks = query
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        List<TaskViewModel> PagedTasks =>
            FilteredTasks
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        async Task ViewTask(TaskViewModel task)
        {
            SelectedTask = task;
            await JS.InvokeVoidAsync("blazorOffcanvas.show", "viewTaskOffcanvas");
        }

        async Task SetView(string view)
        {
            CurrentView = view;

            StateHasChanged();

            if (view == "kanban")
            {
                await Task.Delay(150);
                await JS.InvokeVoidAsync("initKanban", objRef);
            }
        }

        void OpenRowDetails(TaskViewModel task)
        {
            SelectedTask = task;
        }
        void closeOffCanvas()
        {
            IsEdit = false;
            SelectedTask = null;
        }


        private bool IsTaskOverdue(TaskViewModel task)
        {
            var status = TaskStatuses.FirstOrDefault(x => x.Id == task.StatusId);

            if (status?.Name == "Completed")
                return false;

            return task.DueDate.Date < DateTime.Today;
        }

        private async Task EditTask(TaskViewModel task)
        {
            isLoading = true;
            IsEdit = true;
            StateHasChanged();
            await Task.Delay(200);

            SelectedTask = task;
            if (IsEdit)
            {
                existing = TaskService.GetById(task.Id);

                if (existing != null)
                {
                    IsInitializing = true;

                    Tasks = new TaskViewModel
                    {
                        Id = existing.Id,
                        TaskCode = existing.TaskCode,

                        // Owner
                        TaskOwnerId = existing.TaskOwnerId,
                        TaskOwnerName = existing.TaskOwnerName,

                        // Task Info
                        TaskName = existing.TaskName,
                        Description = existing.Description,

                        // Dates
                        StartDate = existing.StartDate,
                        DueDate = existing.DueDate,
                        Reminder = existing.Reminder,

                        // Assignment
                        AssignedToId = existing.AssignedToId,

                        // Priority & Status
                        Priority = existing.Priority,
                        StatusId = existing.StatusId,
                        StatusName = existing.StatusName,

                        // System
                        CreatedAt = existing.CreatedAt,
                        UpdatedAt = existing.UpdatedAt,
                        CompletedAt = existing.CompletedAt,
                        CreatedBy = existing.CreatedBy,
                        UpdatedBy = existing.UpdatedBy,
                        CompletedBy = existing.CompletedBy
                    };

                    IsInitializing = false;
                }
            }
            else
            {
                Tasks = new TaskViewModel
                {
                    StartDate = DateTime.Today,
                    DueDate = DateTime.Today,
                    Priority = TaskPriority.Moderate,
                    TaskOwnerName = AuthService.CurrentUser?.UserName ?? "-"
                };
            }

            _editContext = new EditContext(Tasks);
            isLoading = false;
            StateHasChanged();

            await JS.InvokeVoidAsync("blazorOffcanvas.show", "taskFormOffcanvas");
        }
        private async Task CreateTask()
        {
            isLoading = true;
            StateHasChanged();
            await Task.Delay(200);

            Tasks = new TaskViewModel
            {
                TaskCode = TaskService.GenerateTaskCode(),
                StartDate = DateTime.Today,
                DueDate = DateTime.Today,
                Priority = TaskPriority.Moderate,
                TaskOwnerName = AuthService.CurrentUser?.UserName ?? "-"
            };

            _editContext = new EditContext(Tasks);

            isLoading = false;
            StateHasChanged();

            await JS.InvokeVoidAsync("blazorOffcanvas.show", "taskFormOffcanvas");
        }


        async Task SaveTask()
        {
            // Collect Quill editor values before validation
            if (_descriptionEditor != null)
                Tasks.Description = await _descriptionEditor.GetHtmlAsync();

            if (Tasks.StartDate > Tasks.DueDate)
            {
                ToastService.ShowError("Due Date cannot be earlier than Start Date");
                return;
            }
            if (IsEdit)
            {
                TaskService.Update(Tasks);
                ToastService.ShowSuccess("Task updated successfully");
            }
            else
            {
                TaskService.Create(Tasks);
                ToastService.ShowSuccess("Task created successfully"); 
            }
            LoadData();
            await JS.InvokeVoidAsync("blazorOffcanvas.hide", "taskFormOffcanvas");
        }

        void ConfirmDelete(TaskViewModel task)
        {
            SelectedTask = task;
        }

        async Task DeleteConfirmed()
        {
            if (SelectedTask == null)
                return;

            TaskService.Delete(SelectedTask.Id);
            ToastService.ShowError($"{SelectedTask.TaskCode} Deleted Successfully", "Deleted");
            TaskLists = TaskService.GetAll();
            // Sort by most recent first (UpdatedAt if exists, otherwise CreatedAt)
            FilteredTasks = TaskLists
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            SelectedTask = null;

            CurrentPage = 1;

            await JS.InvokeVoidAsync("closeDeleteModal");

        }

        string GetStatusColor(TaskViewModel task)
        {
            var status = TaskStatusService.GetAll().FirstOrDefault(x => x.Id == task.StatusId);

            if (status == null)
                return "bg-primary text-primary";

            if(string.IsNullOrWhiteSpace(status.Color))
                return "bg-primary text-primary";

            return status.Color.Replace("-transparent", "");
        }

        string GetStatusColorByStatus(TaskStatusViewModel status)
        {
            var statusData = TaskStatusService.GetAll().FirstOrDefault(x => x.Id == status.Id);
            return statusData?.Color ?? "bg-primary-transparent text-primary";
        }
        string GetStatusName(Guid? StatusId)
            {
                var status = TaskStatusService.GetAll().FirstOrDefault(x => x.Id == StatusId);
                return status?.Name ?? "-";
            }

            string GetStatusBadge(string? statusName)
            {
                if (string.IsNullOrWhiteSpace(statusName))
                    return "bg-secondary text-white";

                statusName = statusName.ToLower();

                return statusName switch
                {
                    "open" => "bg-primary text-white",
                    "in progress" => "bg-warning text-dark",
                    "completed" => "bg-success text-white",
                    "cancelled" => "bg-danger text-white",
                    _ => "bg-secondary text-white"
                };
            }

        string getAssigneeName(Guid? id)
        {
            var assignee = Employees.FirstOrDefault(e => e.Id == id);

            if (assignee == null)
                return "-";

            return $"{assignee.FirstName} {assignee.LastName}".Trim();
        }
        string GetPriorityBadge(TaskPriority priority)
            {
                return priority switch
                {
                    TaskPriority.Low => "bg-primary-transparent text-primary",
                    TaskPriority.Moderate => "bg-warning-transparent text-warning",
                    TaskPriority.High => "bg-danger-transparent text-danger",
                    _ => "bg-primary-transparent text-primary"
                };
            }


        void OnPageSizeChange(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);
            CurrentPage = 1;
        }

        void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
        }

        void StartDrag(DragEventArgs e, TaskViewModel task)
        {
            draggedTask = task;
        }

        void DropTask(DragEventArgs e, Guid statusId, string statusName)
        {
            if (draggedTask == null)
                return;

            draggedTask.StatusId = statusId;
            draggedTask.StatusName = statusName;

            draggedTask = null;

            StateHasChanged();
        }

        [JSInvokable]
        public async Task UpdateTaskStatus(string taskId, string statusId)
        {
            if (!Guid.TryParse(taskId, out var taskGuid))
                return;

            if (!Guid.TryParse(statusId, out var statusGuid))
                return;

            var status = TaskStatuses.FirstOrDefault(x => x.Id == statusGuid);

            var updated = TaskService.UpdateStatus(
                taskGuid,
                statusGuid,
                status?.Name
            );

            if (updated)
            {
                ApplyFilters();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task OnRefreshAsync()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(300);

            TaskService.ResetToSeed();
            LoadData();
            CurrentPage = 1;
            isLoading = false;
            await JS.InvokeVoidAsync("feather.replace");
            ToastService.ShowInfo("Task List refreshed", "Refresh");
        }



        async Task EditStatus(TaskStatusViewModel status)
        {
            if (status == null)
                return;

            IsStatusEdit = true;

            // clone to avoid editing list directly
            NewStatus = new TaskStatusViewModel
            {
                Id = status.Id,
                Name = status.Name,
                Color = status.Color,
                SortOrder = status.SortOrder,
                IsActive = status.IsActive,
            };

        }

        async Task CreateStatus()
        {
            if (IsStatusEdit)
            {
                var updated = TaskStatusService.Update(NewStatus);

                if (updated)
                {
                    TaskStatuses = TaskStatusService.GetAll();
                    ToastService.ShowSuccess("Status updated successfully");
                }

            }
            else
            {
                NewStatus.Id = Guid.NewGuid();

                TaskStatusService.Create(NewStatus);

                TaskStatuses = TaskStatusService.GetAll();

                ToastService.ShowSuccess("Status created successfully");
            }

            IsStatusEdit = false;
            await JS.InvokeVoidAsync("blazorModal.hide", "createStatusModal");
            NewStatus = new TaskStatusViewModel();
        }

        void CloseStatusModel()
        {
            IsStatusEdit = false;
            NewStatus = new TaskStatusViewModel();
        }

        void DeleteStatus(TaskStatusViewModel status)
        {
            // Check if any task is using this status
            var hasTasks = TaskLists.Any(t => t.StatusId == status.Id);

            if (hasTasks)
            {
                ToastService.ShowError(
                    $"Cannot delete '{status.Name}'. Tasks exist in this status.",
                    "Delete Blocked"
                );
                return;
            }

            var deleted = TaskStatusService.Delete(status.Id);

            if (deleted)
            {
                TaskStatuses = TaskStatusService.GetAll();
                StateHasChanged();

                ToastService.ShowSuccess(
                    $"Status '{status.Name}' deleted successfully",
                    "Deleted"
                );
            }
            else
            {
                ToastService.ShowError("Unable to delete status");
            }
        }



    }
    
}
