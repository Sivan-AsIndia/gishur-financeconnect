using FinanceConnect.Client.Services;
using FinanceConnect.Client.Shared;
using FinanceConnect.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Pages.EmployeeManagement.Employee
{
    public partial class CreateEmployee
    {

        [Inject] NavigationManager Nav { get; set; } = default!;
        [Inject] MasterDataService MasterDataService { get; set; } = default!;
        [Inject] EmployeeService EmployeeService { get; set; } = default!;
        [Inject] ToastService ToastService { get; set; } = default!;

        [Parameter] public Guid? Id { get; set; }

        private EmployeeViewModel Employee = new();
        private EditContext editContext;
        RichTextEditor? _notesEditor;
        RichTextEditor? _expertiseEditor;
        private bool isInitialized = false;
        private bool IsInitializing = false;
        private bool IsEdit => Id.HasValue;
        private string PageTitle => IsEdit ? "Edit Employee" : "Create Employee";
        private string PageSubTitle => IsEdit ? "Update employee details" : "Add new employee";

        private List<LookupItem> Departments = new();
        private List<LookupItem> Designations = new();
        private List<LookupItem> Managers = new();
        private List<CountryModel> Countries = new();
        private List<StateProvinceModel> States = new();
        private List<StateProvinceModel> AddressStates = new();
        private HashSet<int> validationAttemptedSteps = new();
        private string PANError = "PAN Number is required";
        private int CurrentStep = 1;

        List<WizardStep> Steps = new()
    {
        new("Basic Details","Employee basic info","ti ti-user"),
        new("Work Details","Department & designation","ti ti-briefcase"),
        new("Personal Info","DOB & gender","ti ti-id"),
        new("Contact Details","Phone & address","ti ti-phone"),
        new("Identity","Government IDs","ti ti-id"),
        new("Academic & Experience","Previous work","ti ti-history")
    };
        string IndicatorToppx => $"{40 + ((CurrentStep - 1) * 76)}px";

        protected override void OnInitialized()
        {
            editContext = new EditContext(Employee);

            Departments = EmployeeService.GetDepartments();
            Designations = EmployeeService.GetDesignations();
            Managers = EmployeeService.GetManagers();
            Countries = MasterDataService.GetAllCountries().Where(c => c.IsActive).ToList();
            States = MasterDataService.GetAllStateProvinces().Where(s => s.IsActive).ToList();


            if (IsEdit)
            {
                var existing = EmployeeService.GetById(Id!.Value);

                if (existing != null)
                {
                    IsInitializing = true;

                    Employee = new EmployeeViewModel
                    {
                        Id = existing.Id,
                        EmployeeId = existing.EmployeeId,
                        OfficialEmail = existing.OfficialEmail,
                        FirstName = existing.FirstName,
                        LastName = existing.LastName,

                        DepartmentId = existing.DepartmentId,
                        DesignationId = existing.DesignationId,
                        EmploymentType = existing.EmploymentType,
                        ReportingManagerId = existing.ReportingManagerId,

                        DateOfJoining = existing.DateOfJoining,
                        CurrentExperience = existing.CurrentExperience,
                        SourceOfHire = existing.SourceOfHire,
                        Expertise = existing.Expertise,

                        DateOfBirth = existing.DateOfBirth,
                        Gender = existing.Gender,
                        MaritalStatus = existing.MaritalStatus,
                        AboutMe = existing.AboutMe,

                        PersonalMobileNumber = existing.PersonalMobileNumber,
                        PersonalEmail = existing.PersonalEmail,

                        PAN = existing.PAN,
                        Aadhaar = existing.Aadhaar,
                        UAN = existing.UAN,

                        PresentAddress = new AddressModel
                        {
                            AddressLine1 = existing.PresentAddress?.AddressLine1,
                            AddressLine2 = existing.PresentAddress?.AddressLine2,
                            CountryId = existing.PresentAddress?.CountryId ?? Guid.Empty,
                            StateId = existing.PresentAddress?.StateId,
                            City = existing.PresentAddress?.City,
                            PostalCode = existing.PresentAddress?.PostalCode
                        },

                        Educations = existing.Educations?
                            .Select(e => new EducationModel
                            {
                                Degree = e.Degree,
                                Specialization = e.Specialization,
                                InstituteName = e.InstituteName,
                                DateOfCompletion = e.DateOfCompletion
                            }).ToList() ?? new(),

                        WorkExperiences = existing.WorkExperiences?
                            .Select(w => new WorkExperienceModel
                            {
                                CompanyName = w.CompanyName,
                                JobTitle = w.JobTitle,
                                FromDate = w.FromDate,
                                ToDate = w.ToDate
                            }).ToList() ?? new()
                    };

                    editContext = new EditContext(Employee);

                    // Populate cascading dropdowns
                    if (Employee.PresentAddress.CountryId != Guid.Empty)
                    {
                        AddressStates = States
                            .Where(s => s.CountryId == Employee.PresentAddress.CountryId)
                            .ToList();
                    }

                    IsInitializing = false;
                }
                else
                {
                    Nav.NavigateTo("/employees");
                    return;
                }
            }
            isInitialized = true;
        }

        async Task Next()
        {
            //validationAttemptedSteps.Add(CurrentStep);

            //if (!IsCurrentStepValid())
            //{
            //    StateHasChanged();
            //    return;
            //}
            // Save editor values before step change
            // Save editor values ONLY when leaving those steps
            if (!IsFormValid())
            {
                ToastService.ShowError("Enter the valid data", "Validation Error");
                StateHasChanged();
                return;
            }
            if (CurrentStep == 2 && _expertiseEditor != null)
            {
                Employee.Expertise = await _expertiseEditor.GetHtmlAsync();
            }

            if (CurrentStep == 3 && _notesEditor != null)
            {
                Employee.AboutMe = await _notesEditor.GetHtmlAsync();
            }

            if (CurrentStep < Steps.Count)
                CurrentStep++;
        }

        async Task Back()
        {
            if (CurrentStep == 2 && _expertiseEditor != null)
            {
                Employee.Expertise = await _expertiseEditor.GetHtmlAsync();
            }

            if (CurrentStep == 3 && _notesEditor != null)
            {
                Employee.AboutMe = await _notesEditor.GetHtmlAsync();
            }
            if (CurrentStep > 1)
                CurrentStep--;
        }

        async Task Save()
        {
            RemoveEmptyRows();
            //if (_notesEditor != null)
            //{
            //    Employee.AboutMe = await _notesEditor.GetHtmlAsync();
            //}
            //if (_expertiseEditor != null)
            //{
            //    Employee.Expertise = await _expertiseEditor.GetHtmlAsync();
            //}
            if (HasPANError())
            {
                ToastService.ShowError(PANError, "Validation Error");
                CurrentStep = 5;
                validationAttemptedSteps.Add(5);
                return;
            }

            if (!IsCurrentStepValid())
            {
                validationAttemptedSteps.Add(CurrentStep);
                return;
            }

            if (string.IsNullOrWhiteSpace(Employee.EmployeeId))
            {
                ToastService.ShowError("Employee Id is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }

            if (string.IsNullOrWhiteSpace(Employee.FirstName))
            {
                ToastService.ShowError("First Name is required", "Validation Error");
                CurrentStep = 1;
                validationAttemptedSteps.Add(1);
                return;
            }
            if (!IsFormValid())
            {
                ToastService.ShowError("Enter the valid data", "Validation Error");
                return;
            }
            if (IsEdit)
            {
                EmployeeService.Update(Employee);
                ToastService.ShowSuccess("Employee updated successfully", "Updated");
            }
            else
            {
                Employee.Id = Guid.NewGuid();
                EmployeeService.Create(Employee);
                ToastService.ShowSuccess("Employee created successfully", "Created");
            }

            Nav.NavigateTo("/employees");
        }

        void RemoveEmptyRows()
        {
            Employee.WorkExperiences = Employee.WorkExperiences
                .Where(x => !string.IsNullOrWhiteSpace(x.CompanyName)
                         || !string.IsNullOrWhiteSpace(x.JobTitle)
                         || x.FromDate.HasValue
                         || x.ToDate.HasValue)
                .ToList();

            Employee.Educations = Employee.Educations
                .Where(x => !string.IsNullOrWhiteSpace(x.Degree)
                         || !string.IsNullOrWhiteSpace(x.Specialization)
                         || !string.IsNullOrWhiteSpace(x.InstituteName)
                         || x.DateOfCompletion.HasValue)
                .ToList();
        }

        string StepClass(int i)
        {
            if (i < CurrentStep) return "done";
            if (i == CurrentStep) return "active";
            return "";
        }

        private void OnAddressCountryChange(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var countryId) && countryId != Guid.Empty)
            {
                Employee.PresentAddress.CountryId = countryId;
                AddressStates = States.Where(s => s.CountryId == countryId).ToList();

                if (!IsInitializing)
                {
                    Employee.PresentAddress.StateId = null;
                }
            }
            else
            {
                Employee.PresentAddress.CountryId = Guid.Empty;
                AddressStates = new();

                if (!IsInitializing)
                {
                    Employee.PresentAddress.StateId = null;
                }
            }
        }


        bool ShowFieldError(int step, string field)
        {
            if (!validationAttemptedSteps.Contains(step))
                return false;

            return field switch
            {
                //step 1
                "EmployeeId" => string.IsNullOrWhiteSpace(Employee.EmployeeId),
                "EmployeeIdFormat" => !string.IsNullOrWhiteSpace(Employee.EmployeeId) && !System.Text.RegularExpressions.Regex.IsMatch(Employee.EmployeeId.Trim(), @"^[A-Za-z0-9_\-]+$"),
                "OfficialEmail" => string.IsNullOrWhiteSpace(Employee.OfficialEmail),
                "EmailFormat" => !string.IsNullOrWhiteSpace(Employee.OfficialEmail) && !IsValidEmail(Employee.OfficialEmail),
                "FirstName" => string.IsNullOrWhiteSpace(Employee.FirstName),
                "LastName" => string.IsNullOrWhiteSpace(Employee.LastName),

                // step 2 
                "DepartmentId" => !Employee.DepartmentId.HasValue || Employee.DepartmentId == Guid.Empty,
                "DesignationId" => !Employee.DesignationId.HasValue || Employee.DesignationId == Guid.Empty,
                "EmploymentType" => !Employee.EmploymentType.HasValue,
                "DateOfJoining" => !Employee.DateOfJoining.HasValue,

                //step 3
                "MaritalStatus" => !Employee.MaritalStatus.HasValue,
                "Gender" => !Employee.Gender.HasValue,
                "DateOfBirth" => !Employee.DateOfBirth.HasValue,


                // Step 4
                "PersonalEmailFormat" => !string.IsNullOrWhiteSpace(Employee.PersonalEmail) && !IsValidEmail(Employee.PersonalEmail),
                "PersonalEmail" => string.IsNullOrWhiteSpace(Employee.PersonalEmail),
                "PersonalMobileNumber" => string.IsNullOrWhiteSpace(Employee.PersonalMobileNumber),
                "MobileNumberFormat" => !string.IsNullOrWhiteSpace(Employee.PersonalMobileNumber) && !IsValidPhone(Employee.PersonalMobileNumber),
                "AddressLine1" => string.IsNullOrWhiteSpace(Employee.PresentAddress.AddressLine1),
                "CountryId" => !Employee.PresentAddress.CountryId.HasValue || Employee.PresentAddress.CountryId == Guid.Empty,
                "StateProvinceId" => !Employee.PresentAddress.StateId.HasValue || Employee.PresentAddress.StateId == Guid.Empty,
                "City" => string.IsNullOrWhiteSpace(Employee.PresentAddress.City),
                "PostalCode" => string.IsNullOrWhiteSpace(Employee.PresentAddress.PostalCode),

                "PAN" => HasPANError(),

                _ => false
            };
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(email.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(phone.Trim(),
                @"^[\d\+\-\s\(\)]+$");
        }

        private bool HasPANError()
        {
            if (!string.IsNullOrWhiteSpace(Employee.PAN))
            {

                if (!System.Text.RegularExpressions.Regex.IsMatch(Employee.PAN.Trim(), @"^[A-Za-z]{5}[0-9]{4}[A-Za-z]$"))
                {
                    PANError = "Invalid PAN format (must be AAAAA9999A)";
                    return true;
                }
            }
            return false;
        }
        bool IsCurrentStepValid()
        {
            return CurrentStep switch
            {
                1 => !string.IsNullOrWhiteSpace(Employee.EmployeeId)
                     && !string.IsNullOrWhiteSpace(Employee.OfficialEmail)
                      && IsValidEmail(Employee.OfficialEmail)
                     && !string.IsNullOrWhiteSpace(Employee.FirstName)
                     && !string.IsNullOrWhiteSpace(Employee.LastName),

                2 => Employee.DepartmentId.HasValue && Employee.DepartmentId != Guid.Empty
                     && Employee.DesignationId.HasValue && Employee.DesignationId != Guid.Empty
                     && Employee.EmploymentType.HasValue
                     && Employee.DateOfJoining.HasValue,

                3 => Employee.Gender.HasValue
                     && Employee.MaritalStatus.HasValue
                     && Employee.DateOfBirth.HasValue,

                4 => !string.IsNullOrWhiteSpace(Employee.PersonalMobileNumber)
                     && IsValidPhone(Employee.PersonalMobileNumber)
                     && !string.IsNullOrWhiteSpace(Employee.PersonalEmail)
                     && IsValidEmail(Employee.PersonalEmail)
                     && !string.IsNullOrWhiteSpace(Employee.PresentAddress.AddressLine1)
                     && Employee.PresentAddress.CountryId != Guid.Empty
                     && Employee.PresentAddress.StateId.HasValue && Employee.PresentAddress.StateId != Guid.Empty
                     && !string.IsNullOrWhiteSpace(Employee.PresentAddress.City)
                     && !string.IsNullOrWhiteSpace(Employee.PresentAddress.PostalCode),

                5 => true,

                6 => true,

                _ => true
            };
        }

        bool IsFormValid()
        {
            return
                // Official Email (validate only if entered)
                (string.IsNullOrWhiteSpace(Employee.OfficialEmail)
                    || IsValidEmail(Employee.OfficialEmail))

                // Personal Mobile (validate only if entered)
                && (string.IsNullOrWhiteSpace(Employee.PersonalMobileNumber)
                    || IsValidPhone(Employee.PersonalMobileNumber))

                // Personal Email (validate only if entered)
                && (string.IsNullOrWhiteSpace(Employee.PersonalEmail)
                    || IsValidEmail(Employee.PersonalEmail));
        }

        void AddEducation()
        {
            Employee.Educations.Add(new EducationModel());
        }

        void RemoveEducation(EducationModel edu)
        {
            Employee.Educations.Remove(edu);
        }

        void AddWorkExperience()
        {
            Employee.WorkExperiences.Add(new WorkExperienceModel());
        }

        void RemoveWorkExperience(WorkExperienceModel exp)
        {
            Employee.WorkExperiences.Remove(exp);
        }

        public record WizardStep(string Title, string Description, string Icon);


    }
}
