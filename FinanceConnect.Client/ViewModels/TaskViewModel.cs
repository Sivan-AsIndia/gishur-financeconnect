using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{

    public enum TaskPriority
    {
        Low = 1,
        Moderate = 2,
        High = 3
    }

    public class TaskViewModel
    {

        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }
        //[Required(ErrorMessage = "Task owner is required.")]
        public Guid? TaskOwnerId { get; set; }
        public string? TaskOwnerName { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        [MaxLength(200)]
        public string? TaskName { get; set; }

        [Required(ErrorMessage = "Task Code is required")]
        [StringLength(15)]
        [RegularExpression("^[A-Z0-9_-]+$", ErrorMessage = "Only letters, numbers, - and _ allowed")]
        public string TaskCode { get; set; } = "";

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Today;

        public DateTime? Reminder { get; set; }

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Moderate;

        [Required(ErrorMessage = "Status is required.")]
        public Guid? StatusId { get; set; }

        public string? StatusName { get; set; }

        //[Required(ErrorMessage = "Assigned To is required")]
        public Guid? AssignedToId { get; set; }
        public DateTime? AssignedBy { get; set; }
        public DateTime? AssignedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? CompletedBy { get; set; }
    }

}
