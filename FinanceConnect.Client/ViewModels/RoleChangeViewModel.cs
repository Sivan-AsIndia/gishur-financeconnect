namespace FinanceConnect.Client.ViewModels
{
    public class RoleChangeViewModel
    {
        public enum RoleType
        {
            Intern, Junior, Senior, Lead, TeamLeader, Manager, ProjectManager, Director
        }

        public enum StatusType
        {
            Online, Away, Offline
        }

        public class RoleInfo
        {
            public string Label { get; set; } = "";
            public string Color { get; set; } = "";
            public string Accent { get; set; } = "";
            public int Level { get; set; }
            public string Icon { get; set; } = "";
        }

        public class RoleChange
        {
            public RoleType From { get; set; }
            public RoleType To { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.Now;
        }

        public class Person
        {
            public int Id { get; set; }
            public string First { get; set; } = "";
            public string Last { get; set; } = "";
            public string Title { get; set; } = "";
            public string Dept { get; set; } = "";
            public RoleType Role { get; set; }
            public StatusType Status { get; set; }
            public int? ManagerId { get; set; }
            public List<RoleChange> History { get; set; } = new();

            public string FullName => $"{First} {Last}";
            public string Initials => $"{First[0]}{Last[0]}";

            // DiceBear illustrated avatar — unique per person based on name seed
            public string AvatarUrl =>
                $"https://api.dicebear.com/7.x/avataaars/svg?seed={Uri.EscapeDataString(First + Last)}" +
                $"&backgroundColor=b6e3f4,c0aede,d1d4f9,ffd5dc,ffdfbf&radius=50";
        }
    }
}
