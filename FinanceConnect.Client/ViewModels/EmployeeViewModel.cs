using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FinanceConnect.Client.ViewModels
{

    public enum EmploymentType
    {
        Permanent = 1,
        Contract = 2,
        Intern = 3,
        Consultant = 4
    }

    public enum EmployeeStatus
    {
        Active = 1,
        Inactive = 2,
        Resigned = 3,
        Terminated = 4
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum MaritalStatus
    {
        Single = 1,
        Married = 2
    }
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid CompanyId { get; set; }

        public Guid? BranchId { get; set; }

        #region Basic Information

        [Required]
        [MaxLength(20)]
        public string? EmployeeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }


        [MaxLength(100)]
        public string? NickName { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string? OfficialEmail { get; set; }

        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }

        #endregion

        #region Work Information

        public Guid? DepartmentId { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? DesignationId { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; }

        public EmploymentType? EmploymentType { get; set; }

        public EmployeeStatus? Status { get; set; } = EmployeeStatus.Active;

        public string? SourceOfHire { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfJoining { get; set; }

        public decimal? CurrentExperience { get; set; }
        public decimal? TotalExperience { get; set; }

        #endregion

        #region Hierarchy

        public Guid? ReportingManagerId { get; set; }

        #endregion

        #region Personal Details

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public MaritalStatus? MaritalStatus { get; set; }

        [MaxLength(500)]
        public string? AboutMe { get; set; }

        [MaxLength(250)]
        public string? Expertise { get; set; }

        #endregion

        #region Identity Information

        [MaxLength(12)]
        public string? UAN { get; set; }

        [MaxLength(10)]
        public string? PAN { get; set; }

        [MaxLength(12)]
        public string? Aadhaar { get; set; }

        #endregion

        #region Contact Details

        [MaxLength(20)]
        public string? WorkPhoneNumber { get; set; }

        [MaxLength(10)]
        public string? Extension { get; set; }

        [MaxLength(15)]
        public string? PersonalMobileNumber { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? PersonalEmail { get; set; }

        public AddressModel PresentAddress { get; set; } = new();
        public AddressModel PermanentAddress { get; set; } = new();

        public bool SameAsPresentAddress { get; set; }

        #endregion

        #region Separation

        [DataType(DataType.Date)]
        public DateTime? DateOfExit { get; set; }

        #endregion

        #region Child Collections

        public List<WorkExperienceModel> WorkExperiences { get; set; } = new();
        public List<EducationModel> Educations { get; set; } = new();
        public List<DependentModel> Dependents { get; set; } = new();

        #endregion

        #region System Fields

        public string? AddedBy { get; set; }
        public DateTime? AddedTime { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool OnboardingTriggered { get; set; }

        #endregion

    }

    public class AddressModel
    {
        [MaxLength(200)]
        public string? AddressLine1 { get; set; }

        [MaxLength(200)]
        public string? AddressLine2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }
    }

    public class WorkExperienceModel
    {
        public Guid Id { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        public string? JobTitle { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public string? JobDescription { get; set; }

        public bool IsRelevant { get; set; }
    }

    public class EducationModel
    {
        public Guid Id { get; set; }

        public string? InstituteName { get; set; }
        public string? Degree { get; set; }
        public string? Specialization { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfCompletion { get; set; }
    }

    public class DependentModel
    {
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Relationship { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
