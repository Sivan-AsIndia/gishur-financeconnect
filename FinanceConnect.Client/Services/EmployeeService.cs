using FinanceConnect.Client.ViewModels;
using static FinanceConnect.Client.Data.MasterDataIds;

namespace FinanceConnect.Client.Services
{
    public class EmployeeService
    {
        private readonly MasterDataService _masterDataService;

        private  List<EmployeeViewModel> _employees = new();
        private static List<EmployeeViewModel> _seedemployees = new();
        private readonly List<LookupItem> _departments = new();
        private readonly List<LookupItem> _designations = new();
        private readonly List<LookupItem> _managers = new();
        private readonly List<CompanyModel> _companies = new();

        public EmployeeService(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();
            SeedDepartments();
            SeedDesignations();
            SeedManagers();
            _seedemployees = SeedEmployees();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _employees = CloneList(_seedemployees);
        }

        #region GET METHODS

        public List<EmployeeViewModel> GetAll()
            => _employees
                .OrderByDescending(x => x.ModifiedTime ?? x.AddedTime)
                .ToList();

        public EmployeeViewModel? GetById(Guid id)
            => _employees.FirstOrDefault(x => x.Id == id);

        public List<EmployeeViewModel> GetActiveEmployees()
            => _employees
                .Where(x => x.Status == EmployeeStatus.Active)
                .OrderBy(x => x.FirstName)
                .ToList();

        #endregion

        #region CREATE / UPDATE / DELETE

        public void Create(EmployeeViewModel employee)
        {
            employee.Id = Guid.NewGuid();
            employee.AddedTime = DateTime.UtcNow;
            employee.Status = EmployeeStatus.Active;

            _employees.Add(employee);
        }

        public void Update(EmployeeViewModel employee)
        {
            var existing = GetById(employee.Id);
            if (existing == null)
                return;

            employee.ModifiedTime = DateTime.UtcNow;

            _employees.Remove(existing);
            _employees.Add(employee);
        }

        public void Delete(Guid id)
        {
            var emp = GetById(id);
            if (emp != null)
                _employees.Remove(emp);
        }

        public void Activate(Guid id)
        {
            var emp = GetById(id);
            if (emp != null)
            {
                emp.Status = EmployeeStatus.Active;
                emp.ModifiedTime = DateTime.UtcNow;
            }
        }

        public void Deactivate(Guid id)
        {
            var emp = GetById(id);
            if (emp != null)
            {
                emp.Status = EmployeeStatus.Inactive;
                emp.ModifiedTime = DateTime.UtcNow;
            }
        }


        #endregion

        #region LOOKUP METHODS

        public List<LookupItem> GetDepartments()
            => _departments.OrderBy(d => d.Name).ToList();

        public List<LookupItem> GetDesignations()
            => _designations.OrderBy(d => d.Name).ToList();
        public List<LookupItem> GetManagers()
            => _managers.OrderBy(d => d.Name).ToList();

        public string GetDepartmentName(Guid? id)
            => _departments.FirstOrDefault(d => d.Id == id)?.Name ?? "-";

        public string GetDesignationName(Guid? id)
            => _designations.FirstOrDefault(d => d.Id == id)?.Name ?? "-";

        #endregion

        #region SEED DATA

        private static readonly Guid DeptAccounts = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid DeptSales = Guid.Parse("10000000-0000-0000-0000-000000000002");
        private static readonly Guid DeptHR = Guid.Parse("10000000-0000-0000-0000-000000000003");

        private static readonly Guid DesgManager = Guid.Parse("20000000-0000-0000-0000-000000000001");
        private static readonly Guid DesgExecutive = Guid.Parse("20000000-0000-0000-0000-000000000002");
        private static readonly Guid DesgDeveloper = Guid.Parse("20000000-0000-0000-0000-000000000003");

        private void SeedDepartments()
        {
            if (_departments.Any()) return;

            _departments.AddRange(new List<LookupItem>
            {
                new(DeptAccounts, "Accounts"),
                new(DeptSales, "Sales"),
                new(DeptHR, "Human Resources")
            });
        }

        private void SeedDesignations()
        {
            if (_designations.Any()) return;

            _designations.AddRange(new List<LookupItem>
            {
                new(DesgManager, "Manager"),
                new(DesgExecutive, "Executive"),
                new(DesgDeveloper, "Software Developer")
            });
        }
        private void SeedManagers()
        {
            if (_managers.Any()) return;

            _managers.AddRange(new List<LookupItem>
            {
                new(DesgManager, "Manager1"),
                new(DesgExecutive, "Manager2"),
                new(DesgDeveloper, "Manager3")
            });
        }

        private List<EmployeeViewModel> SeedEmployees()
        {
            //if (_employees.Any()) return;

            var companies = _masterDataService.GetAllCompanies()
                                .Where(c => c.Status == "Active")
                                .ToList();

            // Define lists of real names to pick from
            var firstNames = new[] { "Arjun", "Priya", "Vikram", "Sneha", "Rahul", "Anjali" ,"John" ,"Fabby" };
            int globalCounter = 1;

            foreach (var company in companies)
            {
                // Reduced to 2 employees per company
                for (int i = 1; i <= 2; i++)
                {
                    var employeeNumber = globalCounter.ToString("D3");

                    _employees.Add(new EmployeeViewModel
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = company.Id,

                        // Using index to pick a name from the list
                        EmployeeId = $"{company.LegalName.Substring(0, 2).ToUpper()}{employeeNumber}",
                        FirstName = firstNames[(globalCounter - 1) % firstNames.Length],
                        LastName = company.LegalName.Split(' ')[0],

                        OfficialEmail = $"staff{globalCounter}@company.com",
                        PersonalMobileNumber = $"+91-90000000{globalCounter}",
                        PersonalEmail = $"person{globalCounter}@gmail.com",

                        DepartmentId = i == 1 ? DeptAccounts : DeptSales,
                        DesignationId = i == 1 ? DesgManager : DesgExecutive,

                        EmploymentType = i == 1 ? EmploymentType.Permanent : EmploymentType.Contract,
                        Status = EmployeeStatus.Active,

                        Gender = i == 1 ? Gender.Male : Gender.Female,
                        MaritalStatus = i == 1 ? MaritalStatus.Married : MaritalStatus.Single,

                        DateOfBirth = DateTime.UtcNow.AddYears(-25 - i),
                        DateOfJoining = DateTime.UtcNow.AddMonths(-i * 6),

                        SourceOfHire = i == 1 ? "LinkedIn" : "Employee Referral",

                        Expertise = i == 1
                            ? "Finance Management, Accounting"
                            : "Sales Strategy, Negotiation",

                        TotalExperience = 2 + i,
                        AddedTime = DateTime.UtcNow.AddMonths(-i * 2),

                        // ---------------- EDUCATION ----------------

                        Educations = new List<EducationModel>

                            {

                            new EducationModel

                            {

                            Id = Guid.NewGuid(),

                            Degree = "Bachelor of Commerce",

                            Specialization = "Accounting",

                            InstituteName = "University of Madras",

                            DateOfCompletion = new DateTime(2018,5,1)

                            },

                            new EducationModel

                            {

                            Id = Guid.NewGuid(),

                            Degree = "Master Degree",

                            Specialization = i == 1 ? "Finance" : "Business Administration",

                            InstituteName = "Anna University",

                            DateOfCompletion = new DateTime(2020,5,1)

                            }

                            },



                        // ---------------- WORK EXPERIENCE ----------------

                        WorkExperiences = new List<WorkExperienceModel>

                        {

                        new WorkExperienceModel

                        {

                        Id = Guid.NewGuid(),

                        CompanyName = "ABC Solutions Pvt Ltd",

                        JobTitle = "Junior Executive",

                        FromDate = new DateTime(2020,6,1),

                        ToDate = new DateTime(2022,5,1)

                        },

                        new WorkExperienceModel

                        {

                        Id = Guid.NewGuid(),

                        CompanyName = "Global Tech Systems",

                        JobTitle = "Senior Executive",

                        FromDate = new DateTime(2022,6,1),

                        ToDate = null

                        }

                        }


                    });

                    globalCounter++;
                }
            }
            return _employees;
        }

        #endregion
    }

    #region HELPER CLASS

    public record LookupItem(Guid Id, string Name);

    #endregion
}