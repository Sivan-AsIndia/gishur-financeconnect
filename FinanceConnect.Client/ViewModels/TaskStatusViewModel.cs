using System.ComponentModel.DataAnnotations;

namespace FinanceConnect.Client.ViewModels
{
    public class TaskStatusViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Status name is required.")]
        [MaxLength(30)]
        public string? Name { get; set; }

        public bool IsDefault { get; set; }

        public int SortOrder { get; set; }

        public string? Color { get; set; }   // optional for UI badge

        public bool IsActive { get; set; } = true;
    }
}
