using static FinanceConnect.Client.ViewModels.RoleChangeViewModel;

namespace FinanceConnect.Client.Services
{
    public class RoleChangingService
    {
        private int _nextId = 151;

        public static readonly Dictionary<RoleType, RoleInfo> Roles = new()
        {
            [RoleType.Intern] = new() { Label = "Intern", Color = "#64748b", Accent = "#3d3d3d", Level = 8, Icon = "○" },
            [RoleType.Junior] = new() { Label = "Junior Dev", Color = "#6b7280", Accent = "#3e3e3e", Level = 7, Icon = "◎" },
            [RoleType.Senior] = new() { Label = "Senior Dev", Color = "#ec4899", Accent = "#ff088e", Level = 6, Icon = "◈" },
            [RoleType.Lead] = new() { Label = "Tech Lead", Color = "#f97316", Accent = "#ff8200", Level = 5, Icon = "◆" },
            [RoleType.TeamLeader] = new() { Label = "Team Leader", Color = "#10b981", Accent = "#00a061", Level = 4, Icon = "❖" },
            [RoleType.Manager] = new() { Label = "Manager", Color = "#0ea5e9", Accent = "#00adff", Level = 3, Icon = "⬡" },
            [RoleType.ProjectManager] = new() { Label = "Project Manager", Color = "#8b5cf6", Accent = "#7812ff", Level = 2, Icon = "⬟" },
            [RoleType.Director] = new() { Label = "Director", Color = "#f59e0b", Accent = "#ffc300", Level = 1, Icon = "★" },
        };

        public List<Person> People { get; private set; } = new()
        {
            // ── L1: Director (1) ──────────────────────────────────────────
            new() { Id=1,   First="Vikram",    Last="Nair",      Title="Engineering Director",          Dept="Engineering",  Role=RoleType.Director,       Status=StatusType.Online,  ManagerId=null },

            // ── L2: Project Managers (4) → report to Director ─────────────
            new() { Id=2,   First="Sunita",    Last="Reddy",     Title="PM — Platform",                 Dept="Product",      Role=RoleType.ProjectManager, Status=StatusType.Online,  ManagerId=1 },
            new() { Id=3,   First="Deepak",    Last="Pillai",    Title="PM — Mobile",                   Dept="Product",      Role=RoleType.ProjectManager, Status=StatusType.Away,    ManagerId=1 },
            new() { Id=4,   First="Priya",     Last="Sharma",    Title="PM — Infrastructure",           Dept="Engineering",  Role=RoleType.ProjectManager, Status=StatusType.Online,  ManagerId=1 },
            new() { Id=5,   First="Rahul",     Last="Menon",     Title="PM — Data",                     Dept="Analytics",    Role=RoleType.ProjectManager, Status=StatusType.Online,  ManagerId=1 },

            // ── L3: Managers (8) ──────────────────────────────────────────
            new() { Id=6,   First="Aarav",     Last="Joshi",     Title="Engineering Manager",           Dept="Engineering",  Role=RoleType.Manager,        Status=StatusType.Online,  ManagerId=2 },
            new() { Id=7,   First="Kavitha",   Last="Krishnan",  Title="QA Manager",                    Dept="QA",           Role=RoleType.Manager,        Status=StatusType.Online,  ManagerId=2 },
            new() { Id=8,   First="Sanjay",    Last="Bhat",      Title="Mobile Engineering Manager",    Dept="Engineering",  Role=RoleType.Manager,        Status=StatusType.Away,    ManagerId=3 },
            new() { Id=9,   First="Divya",     Last="Patel",     Title="Product Manager",               Dept="Product",      Role=RoleType.Manager,        Status=StatusType.Online,  ManagerId=3 },
            new() { Id=10,  First="Karthik",   Last="Iyer",      Title="DevOps Manager",                Dept="DevOps",       Role=RoleType.Manager,        Status=StatusType.Online,  ManagerId=4 },
            new() { Id=11,  First="Meenakshi", Last="Rao",       Title="Security Manager",              Dept="Security",     Role=RoleType.Manager,        Status=StatusType.Offline, ManagerId=4 },
            new() { Id=12,  First="Vijay",     Last="Kumar",     Title="Data Engineering Manager",      Dept="Analytics",    Role=RoleType.Manager,        Status=StatusType.Online,  ManagerId=5 },
            new() { Id=13,  First="Lakshmi",   Last="Suresh",    Title="BI Manager",                    Dept="Analytics",    Role=RoleType.Manager,        Status=StatusType.Away,    ManagerId=5 },

            // ── L4: Team Leaders (16) ─────────────────────────────────────
            new() { Id=14,  First="Arjun",     Last="Dev",       Title="TL — Frontend",                 Dept="Engineering",  Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=6 },
            new() { Id=15,  First="Rani",      Last="Pillai",    Title="TL — Backend",                  Dept="Engineering",  Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=6 },
            new() { Id=16,  First="Nikhil",    Last="Desai",     Title="TL — Fullstack",                Dept="Engineering",  Role=RoleType.TeamLeader,     Status=StatusType.Away,    ManagerId=6 },
            new() { Id=17,  First="Pooja",     Last="Nair",      Title="TL — QA Automation",            Dept="QA",           Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=7 },
            new() { Id=18,  First="Ravi",      Last="Shankar",   Title="TL — Manual QA",                Dept="QA",           Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=7 },
            new() { Id=19,  First="Meera",     Last="Raj",       Title="TL — iOS",                      Dept="Mobile",       Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=8 },
            new() { Id=20,  First="Suresh",    Last="Anand",     Title="TL — Android",                  Dept="Mobile",       Role=RoleType.TeamLeader,     Status=StatusType.Away,    ManagerId=8 },
            new() { Id=21,  First="Gayathri",  Last="Menon",     Title="TL — UX",                       Dept="Design",       Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=9 },
            new() { Id=22,  First="Pranav",    Last="Gupta",     Title="TL — CI/CD",                    Dept="DevOps",       Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=10 },
            new() { Id=23,  First="Ananya",    Last="Bose",      Title="TL — Cloud",                    Dept="DevOps",       Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=10 },
            new() { Id=24,  First="Rohan",     Last="Verma",     Title="TL — AppSec",                   Dept="Security",     Role=RoleType.TeamLeader,     Status=StatusType.Offline, ManagerId=11 },
            new() { Id=25,  First="Tara",      Last="Singh",     Title="TL — Pipelines",                Dept="Analytics",    Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=12 },
            new() { Id=26,  First="Kiran",     Last="Das",       Title="TL — ML",                       Dept="Analytics",    Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=12 },
            new() { Id=27,  First="Sneha",     Last="Reddy",     Title="TL — Dashboards",               Dept="Analytics",    Role=RoleType.TeamLeader,     Status=StatusType.Away,    ManagerId=13 },
            new() { Id=28,  First="Aditya",    Last="Kapoor",    Title="TL — Reports",                  Dept="Analytics",    Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=13 },
            new() { Id=29,  First="Ishaan",    Last="Trivedi",   Title="TL — Platform API",             Dept="Engineering",  Role=RoleType.TeamLeader,     Status=StatusType.Online,  ManagerId=15 },

            // ── L5: Tech Leads (20) ───────────────────────────────────────
            new() { Id=30,  First="Nandini",   Last="Joshi",     Title="Tech Lead — React",             Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=14 },
            new() { Id=31,  First="Harish",    Last="Varma",     Title="Tech Lead — Vue",               Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Away,    ManagerId=14 },
            new() { Id=32,  First="Aditi",     Last="Ghosh",     Title="Tech Lead — Node",              Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=15 },
            new() { Id=33,  First="Manish",    Last="Choudhary", Title="Tech Lead — Go",                Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=15 },
            new() { Id=34,  First="Preethi",   Last="Rajan",     Title="Tech Lead — Java",              Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Offline, ManagerId=16 },
            new() { Id=35,  First="Siddharth", Last="Mishra",    Title="Tech Lead — Python",            Dept="Engineering",  Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=16 },
            new() { Id=36,  First="Anitha",    Last="Nathan",    Title="Tech Lead — Selenium",          Dept="QA",           Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=17 },
            new() { Id=37,  First="Manoj",     Last="Pillai",    Title="Tech Lead — Cypress",           Dept="QA",           Role=RoleType.Lead,           Status=StatusType.Away,    ManagerId=17 },
            new() { Id=38,  First="Chitra",    Last="Krishnan",  Title="Tech Lead — Performance",       Dept="QA",           Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=18 },
            new() { Id=39,  First="Dev",       Last="Sharma",    Title="Tech Lead — Swift",             Dept="Mobile",       Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=19 },
            new() { Id=40,  First="Rekha",     Last="Iyer",      Title="Tech Lead — Flutter",           Dept="Mobile",       Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=20 },
            new() { Id=41,  First="Arvind",    Last="Kumar",     Title="Tech Lead — Kotlin",            Dept="Mobile",       Role=RoleType.Lead,           Status=StatusType.Away,    ManagerId=20 },
            new() { Id=42,  First="Shruti",    Last="Bansal",    Title="Tech Lead — Figma",             Dept="Design",       Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=21 },
            new() { Id=43,  First="Nitin",     Last="Aggarwal",  Title="Tech Lead — Jenkins",           Dept="DevOps",       Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=22 },
            new() { Id=44,  First="Pallavi",   Last="Desai",     Title="Tech Lead — AWS",               Dept="DevOps",       Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=23 },
            new() { Id=45,  First="Ramesh",    Last="Babu",      Title="Tech Lead — Pentesting",        Dept="Security",     Role=RoleType.Lead,           Status=StatusType.Away,    ManagerId=24 },
            new() { Id=46,  First="Geeta",     Last="Nair",      Title="Tech Lead — Spark",             Dept="Analytics",    Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=25 },
            new() { Id=47,  First="Vijayalakshmi", Last="Rao",   Title="Tech Lead — Kafka",             Dept="Analytics",    Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=25 },
            new() { Id=48,  First="Chiranjeevi",Last="Reddy",    Title="Tech Lead — TensorFlow",        Dept="Analytics",    Role=RoleType.Lead,           Status=StatusType.Online,  ManagerId=26 },
            new() { Id=49,  First="Usha",      Last="Menon",     Title="Tech Lead — Tableau",           Dept="Analytics",    Role=RoleType.Lead,           Status=StatusType.Away,    ManagerId=27 },

            // ── L6: Senior Devs (30) ──────────────────────────────────────
            new() { Id=50,  First="Amit",      Last="Verma",     Title="Senior Frontend Dev",           Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=30 },
            new() { Id=51,  First="Bhavana",   Last="Rao",       Title="Senior React Dev",              Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=30 },
            new() { Id=52,  First="Chandan",   Last="Kaur",      Title="Senior Vue Dev",                Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=31 },
            new() { Id=53,  First="Deepika",   Last="Pillai",    Title="Senior Node Dev",               Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=32 },
            new() { Id=54,  First="Elan",      Last="Murugan",   Title="Senior Go Dev",                 Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=33 },
            new() { Id=55,  First="Fathima",   Last="Begum",     Title="Senior Java Dev",               Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=34 },
            new() { Id=56,  First="Ganesh",    Last="Subramanian",Title="Senior Python Dev",            Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=35 },
            new() { Id=57,  First="Haritha",   Last="Nambiar",   Title="Senior QA Engineer",            Dept="QA",           Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=36 },
            new() { Id=58,  First="Indu",      Last="Prakash",   Title="Senior Automation QA",          Dept="QA",           Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=37 },
            new() { Id=59,  First="Jayanthi",  Last="Krishnan",  Title="Senior Perf Engineer",          Dept="QA",           Role=RoleType.Senior,         Status=StatusType.Offline, ManagerId=38 },
            new() { Id=60,  First="Kalpana",   Last="Das",       Title="Senior iOS Dev",                Dept="Mobile",       Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=39 },
            new() { Id=61,  First="Lalith",    Last="Mohan",     Title="Senior Flutter Dev",            Dept="Mobile",       Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=40 },
            new() { Id=62,  First="Mala",      Last="Sundaram",  Title="Senior Kotlin Dev",             Dept="Mobile",       Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=41 },
            new() { Id=63,  First="Naveen",    Last="Raj",       Title="Senior UX Designer",            Dept="Design",       Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=42 },
            new() { Id=64,  First="Oviya",     Last="Selvam",    Title="Senior DevOps Engineer",        Dept="DevOps",       Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=43 },
            new() { Id=65,  First="Pradeep",   Last="Anand",     Title="Senior Cloud Engineer",         Dept="DevOps",       Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=44 },
            new() { Id=66,  First="Queenie",   Last="D'Souza",   Title="Senior Security Engineer",      Dept="Security",     Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=45 },
            new() { Id=67,  First="Raghu",     Last="Venkat",    Title="Senior Data Engineer",          Dept="Analytics",    Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=46 },
            new() { Id=68,  First="Saranya",   Last="Pandian",   Title="Senior Kafka Engineer",         Dept="Analytics",    Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=47 },
            new() { Id=69,  First="Thirumal",  Last="Rajan",     Title="Senior ML Engineer",            Dept="Analytics",    Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=48 },
            new() { Id=70,  First="Uma",       Last="Balaji",    Title="Senior BI Developer",           Dept="Analytics",    Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=49 },
            new() { Id=71,  First="Vasanth",   Last="Krishnamurthy",Title="Senior API Dev",             Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=29 },
            new() { Id=72,  First="Waqar",     Last="Ali",       Title="Senior Integration Dev",        Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=29 },
            new() { Id=73,  First="Yamini",    Last="Balasubramanian",Title="Senior Platform Dev",      Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Offline, ManagerId=29 },
            new() { Id=74,  First="Zanele",    Last="Iyer",      Title="Senior UI Dev",                 Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=30 },
            new() { Id=75,  First="Abhilash",  Last="Menon",     Title="Senior TypeScript Dev",         Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=31 },
            new() { Id=76,  First="Bindhu",    Last="Nair",      Title="Senior GraphQL Dev",            Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Away,    ManagerId=32 },
            new() { Id=77,  First="Chandrasekhar",Last="Pillai", Title="Senior Microservices Dev",      Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=33 },
            new() { Id=78,  First="Dharini",   Last="Suresh",    Title="Senior Spring Dev",             Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=34 },
            new() { Id=79,  First="Elakiya",   Last="Murugesan", Title="Senior Django Dev",             Dept="Engineering",  Role=RoleType.Senior,         Status=StatusType.Online,  ManagerId=35 },

            // ── L7: Junior Devs (35) ──────────────────────────────────────
            new() { Id=80,  First="Faisal",    Last="Khan",      Title="Junior Frontend Dev",           Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=50 },
            new() { Id=81,  First="Geetha",    Last="Ramaswamy", Title="Junior React Dev",              Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=51 },
            new() { Id=82,  First="Hari",      Last="Prasad",    Title="Junior Vue Dev",                Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=52 },
            new() { Id=83,  First="Indrani",   Last="Bose",      Title="Junior Node Dev",               Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=53 },
            new() { Id=84,  First="Jagan",     Last="Mohan",     Title="Junior Go Dev",                 Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=54 },
            new() { Id=85,  First="Kamala",    Last="Devi",      Title="Junior Java Dev",               Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Offline, ManagerId=55 },
            new() { Id=86,  First="Lokesh",    Last="Sharma",    Title="Junior Python Dev",             Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=56 },
            new() { Id=87,  First="Mythili",   Last="Krishnan",  Title="Junior QA",                    Dept="QA",           Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=57 },
            new() { Id=88,  First="Naresh",    Last="Babu",      Title="Junior Automation QA",          Dept="QA",           Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=58 },
            new() { Id=89,  First="Ojas",      Last="Trivedi",   Title="Junior Perf Tester",            Dept="QA",           Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=59 },
            new() { Id=90,  First="Padmavathi",Last="Rao",       Title="Junior iOS Dev",                Dept="Mobile",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=60 },
            new() { Id=91,  First="Qasim",     Last="Siddiqui",  Title="Junior Flutter Dev",            Dept="Mobile",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=61 },
            new() { Id=92,  First="Revathi",   Last="Subramaniam",Title="Junior Kotlin Dev",            Dept="Mobile",       Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=62 },
            new() { Id=93,  First="Sivakumar", Last="Rajan",     Title="Junior UX Designer",            Dept="Design",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=63 },
            new() { Id=94,  First="Thenmozhi", Last="Raj",       Title="Junior DevOps",                 Dept="DevOps",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=64 },
            new() { Id=95,  First="Udaya",     Last="Kumar",     Title="Junior Cloud Engineer",         Dept="DevOps",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=65 },
            new() { Id=96,  First="Vaishnavi",  Last="Pillai",   Title="Junior Security Analyst",       Dept="Security",     Role=RoleType.Junior,         Status=StatusType.Offline, ManagerId=66 },
            new() { Id=97,  First="Wasim",     Last="Ansari",    Title="Junior Data Engineer",          Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=67 },
            new() { Id=98,  First="Xavier",    Last="Fernandez", Title="Junior Kafka Dev",              Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=68 },
            new() { Id=99,  First="Yashoda",   Last="Krishnamurthy",Title="Junior ML Engineer",         Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=69 },
            new() { Id=100, First="Zeenat",    Last="Begum",     Title="Junior BI Developer",           Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=70 },
            new() { Id=101, First="Arun",      Last="Selvam",    Title="Junior API Dev",                Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=71 },
            new() { Id=102, First="Bharathi",  Last="Natarajan", Title="Junior Integration Dev",        Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=72 },
            new() { Id=103, First="Chidambaram",Last="Rajan",    Title="Junior Platform Dev",           Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=73 },
            new() { Id=104, First="Durgadevi",  Last="Shankar",  Title="Junior UI Dev",                 Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=74 },
            new() { Id=105, First="Ezhilarasan",Last="Murugan",  Title="Junior TypeScript Dev",         Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=75 },
            new() { Id=106, First="Femila",    Last="Joseph",    Title="Junior GraphQL Dev",            Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Offline, ManagerId=76 },
            new() { Id=107, First="Gopalakrishnan",Last="Nair",  Title="Junior Microservices Dev",      Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=77 },
            new() { Id=108, First="Hemamalini", Last="Balaji",   Title="Junior Spring Dev",             Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=78 },
            new() { Id=109, First="Ilancheliyan",Last="Raj",     Title="Junior Django Dev",             Dept="Engineering",  Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=79 },
            new() { Id=110, First="Janani",    Last="Suresh",    Title="Junior React Native Dev",       Dept="Mobile",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=40 },
            new() { Id=111, First="Kaviraj",   Last="Pillai",    Title="Junior Terraform Dev",          Dept="DevOps",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=43 },
            new() { Id=112, First="Logeshwari",Last="Devi",      Title="Junior Ansible Dev",            Dept="DevOps",       Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=44 },
            new() { Id=113, First="Muthukumaran",Last="Rajan",   Title="Junior Spark Dev",              Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Away,    ManagerId=46 },
            new() { Id=114, First="Nandhini",  Last="Krishnan",  Title="Junior PyTorch Dev",            Dept="Analytics",    Role=RoleType.Junior,         Status=StatusType.Online,  ManagerId=48 },

            // ── L8: Interns (36) ──────────────────────────────────────────
            new() { Id=115, First="Om",        Last="Prakash",   Title="Frontend Intern",               Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=80 },
            new() { Id=116, First="Pavithra",  Last="Sundaram",  Title="React Intern",                  Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=81 },
            new() { Id=117, First="Rajakumar", Last="Pal",       Title="Node Intern",                   Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=83 },
            new() { Id=118, First="Saraswathi",Last="Venkat",    Title="Java Intern",                   Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=85 },
            new() { Id=119, First="Tamizharasan",Last="Raj",     Title="Python Intern",                 Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=86 },
            new() { Id=120, First="Umarani",   Last="Pillai",    Title="QA Intern",                    Dept="QA",           Role=RoleType.Intern,         Status=StatusType.Offline, ManagerId=87 },
            new() { Id=121, First="Vasuki",    Last="Nair",      Title="Automation Intern",             Dept="QA",           Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=88 },
            new() { Id=122, First="Wisam",     Last="Patel",     Title="iOS Intern",                    Dept="Mobile",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=90 },
            new() { Id=123, First="Xena",      Last="D'Souza",   Title="Flutter Intern",                Dept="Mobile",       Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=91 },
            new() { Id=124, First="Yazhini",   Last="Rajan",     Title="Design Intern",                 Dept="Design",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=93 },
            new() { Id=125, First="Zubair",    Last="Khan",      Title="DevOps Intern",                 Dept="DevOps",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=94 },
            new() { Id=126, First="Abirami",   Last="Selvam",    Title="Data Intern",                   Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=97 },
            new() { Id=127, First="Balachandran",Last="Iyer",    Title="ML Intern",                     Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=99 },
            new() { Id=128, First="Chelladurai",Last="Raj",      Title="BI Intern",                     Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=100 },
            new() { Id=129, First="Dhanabalan",Last="Pillai",    Title="API Intern",                    Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=101 },
            new() { Id=130, First="Eswari",    Last="Murugan",   Title="Integration Intern",            Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=102 },
            new() { Id=131, First="Felicita",  Last="Rodrigues", Title="Platform Intern",               Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Offline, ManagerId=103 },
            new() { Id=132, First="Gokulakannan",Last="Nair",    Title="UI Intern",                     Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=104 },
            new() { Id=133, First="Hemalatha", Last="Suresh",    Title="TypeScript Intern",             Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=105 },
            new() { Id=134, First="Indumathi", Last="Balaji",    Title="GraphQL Intern",                Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=106 },
            new() { Id=135, First="Jothilakshmi",Last="Krishnan",Title="Microservices Intern",          Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=107 },
            new() { Id=136, First="Kalaivani", Last="Raj",       Title="Spring Intern",                 Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=108 },
            new() { Id=137, First="Loganathan",Last="Pillai",    Title="Django Intern",                 Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=109 },
            new() { Id=138, First="Manimegalai",Last="Rao",      Title="React Native Intern",           Dept="Mobile",       Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=110 },
            new() { Id=139, First="Nithyashree",Last="Devi",     Title="Cloud Intern",                  Dept="DevOps",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=112 },
            new() { Id=140, First="Palaniappan",Last="Murugan",  Title="Spark Intern",                  Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=113 },
            new() { Id=141, First="Rajalakshmi",Last="Nair",     Title="PyTorch Intern",                Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Offline, ManagerId=114 },
            new() { Id=142, First="Santhosh",  Last="Kumar",     Title="Go Intern",                     Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=84 },
            new() { Id=143, First="Tamilarasi",Last="Selvam",    Title="Kotlin Intern",                 Dept="Mobile",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=92 },
            new() { Id=144, First="Udhayakumar",Last="Rajan",    Title="Vue Intern",                    Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=82 },
            new() { Id=145, First="Vijayakumar",Last="Subramaniam",Title="Security Intern",             Dept="Security",     Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=96 },
            new() { Id=146, First="Soundarya", Last="Rajan",     Title="UX Intern",                     Dept="Design",       Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=63 },
            new() { Id=147, First="Murugesan", Last="Pillai",    Title="Perf Test Intern",              Dept="QA",           Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=89 },
            new() { Id=148, First="Krishnaveni",Last="Iyer",     Title="Tableau Intern",                Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Away,    ManagerId=70 },
            new() { Id=149, First="Periyasamy",Last="Raj",       Title="Kafka Intern",                  Dept="Analytics",    Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=68 },
            new() { Id=150, First="Selvarani", Last="Devi",      Title="Fullstack Intern",              Dept="Engineering",  Role=RoleType.Intern,         Status=StatusType.Online,  ManagerId=35 },
        };

        public void ReassignManager(int personId, int newManagerId)
        {
            var person = People.FirstOrDefault(p => p.Id == personId);
            if (person == null) return;
            person.ManagerId = newManagerId;
            OnChange?.Invoke();
        }

        public event Action? OnChange;

        public List<Person> GetTeam(Person person, HashSet<int>? visited = null)
        {
            visited ??= new HashSet<int>();
            if (visited.Contains(person.Id)) return new();
            visited.Add(person.Id);
            var direct = People.Where(p => p.ManagerId == person.Id).ToList();
            var all = new List<Person>(direct);
            foreach (var d in direct)
                all.AddRange(GetTeam(d, visited));
            return all;
        }

        public void ChangeRole(int personId, RoleType newRole)
        {
            var person = People.FirstOrDefault(p => p.Id == personId);
            if (person == null || person.Role == newRole) return;
            person.History.Insert(0, new RoleChange { From = person.Role, To = newRole });
            person.Role = newRole;
            OnChange?.Invoke();
        }

        public void PromoteTeam(int personId)
        {
            var person = People.FirstOrDefault(p => p.Id == personId);
            if (person == null) return;
            var team = GetTeam(person);
            foreach (var m in team.Where(m => m.Role != person.Role))
            {
                m.History.Insert(0, new RoleChange { From = m.Role, To = person.Role });
                m.Role = person.Role;
            }
            OnChange?.Invoke();
        }

        public void RemovePerson(int personId)
        {
            var person = People.FirstOrDefault(p => p.Id == personId);
            if (person == null) return;
            foreach (var p in People.Where(p => p.ManagerId == personId))
                p.ManagerId = person.ManagerId;
            People.Remove(person);
            OnChange?.Invoke();
        }

        public Person AddPerson(string first, string last, string title, string dept,
                                RoleType role, StatusType status, int? managerId)
        {
            var p = new Person
            {
                Id = ++_nextId,
                First = first,
                Last = last,
                Title = title,
                Dept = dept,
                Role = role,
                Status = status,
                ManagerId = managerId
            };
            People.Add(p);
            OnChange?.Invoke();
            return p;
        }

        public static string TimeAgo(DateTime dt)
        {
            var d = DateTime.Now - dt;
            if (d.TotalMinutes < 1) return "just now";
            if (d.TotalHours < 1) return $"{(int)d.TotalMinutes}m ago";
            if (d.TotalDays < 1) return $"{(int)d.TotalHours}h ago";
            return $"{(int)d.TotalDays}d ago";
        }

        public static string StatusColor(StatusType s) => s switch
        {
            StatusType.Online => "#22c55e",
            StatusType.Away => "#f59e0b",
            StatusType.Offline => "#475569",
            _ => "#475569"
        };
    }
}
