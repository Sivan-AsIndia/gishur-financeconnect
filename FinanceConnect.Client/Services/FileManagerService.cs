namespace FinanceConnect.Client.Services
{
    public class FileManagerService
    {
        public static string FmtMB(double mb) =>
            mb >= 1024 ? $"{mb / 1024:F1} GB" :
            mb >= 1 ? $"{mb:F0} MB" :
            mb > 0 ? $"{mb * 1024:F0} KB" : "0 KB";

        public FileMeta GetMeta(string ext) =>
            MetaMap.TryGetValue(ext, out var m) ? m : new("#f3f4f6", "#6b7280", "FILE");

        public int NextFolderId() =>
            Folders.Count > 0 ? Folders.Max(f => f.Id) + 1 : 100;

        public int NextFileId() =>
            Files.Count > 0 ? Files.Max(f => f.Id) + 1 : 1000;

        public FolderItem? GetFolderById(int id) =>
            Folders.FirstOrDefault(f => f.Id == id);

        // ══ MODELS ══════════════════════════════════════════════════════
        public class FileItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Ext { get; set; } = "";
            public double Size { get; set; }
            public int Items { get; set; }
            public string Date { get; set; } = "";
            public bool Starred { get; set; }
            public bool Deleted { get; set; }
            public int? FolderId { get; set; }
            public byte[]? FileBytes { get; set; }
            public string? DataUrl { get; set; }
            public string? TextContent { get; set; }
            public string? MimeType { get; set; }
        }

        public class FolderItem
        {
            public int Id { get; set; }
            /// <summary>0 = root folder, >0 = sub-folder of that parent ID</summary>
            public int ParentId { get; set; } = 0;
            public string Name { get; set; } = "";
            public double Size { get; set; }
            public double Total { get; set; }
            public string Color { get; set; } = "#4f46e5";
            public bool Starred { get; set; }
        }

        public class DriveItem
        {
            public string Name { get; set; } = "";
            public string Total { get; set; } = "";
            public string Color { get; set; } = "";
            public string IconBg { get; set; } = "";
            public string Icon { get; set; } = "";
            public int Files { get; set; }
            public double UsedMB { get; set; }
            public string Used => FmtMB(UsedMB);
            public double TotalMB { get; set; }
            public int Pct => TotalMB > 0 ? (int)Math.Min(100, UsedMB / TotalMB * 100) : 0;
        }

        public class NavItem
        {
            public string Key { get; set; } = "";
            public string Label { get; set; } = "";
            public string Icon { get; set; } = "";
            public int Badge { get; set; }
        }

        public class StorageCatItem
        {
            public string Name { get; set; } = "";
            public string[] Exts { get; set; } = Array.Empty<string>();
            public string Size => FmtMB(SizeMB);
            public double SizeMB { get; set; }
            public string Color { get; set; } = "";
            public string Bg { get; set; } = "";
            public string Icon { get; set; } = "";
            public int Count { get; set; }
        }

        public record FileMeta(string Bg, string Color, string Label);

        public void RecalcStorage(double addedMB) => RecalcStorage();
        public void RecalcStorage()
        {
            var allFiles = Files.Where(f => !f.Deleted).ToList();

            var localMB = allFiles.Where(f => f.FolderId == null).Sum(f => f.Size);
            var localDrive = Drives.FirstOrDefault(d => d.Name == "Local Drive");
            if (localDrive != null)
            {
                localDrive.UsedMB = localMB + 10_000;
                localDrive.Files = allFiles.Count(f => f.FolderId == null);
            }

            foreach (var cat in StorageCats)
            {
                var catFiles = allFiles.Where(f => cat.Exts.Contains(f.Ext.ToLower())).ToList();
                cat.SizeMB = catFiles.Sum(f => f.Size);
                cat.Count = catFiles.Count;
            }

            var starred = NavItems.FirstOrDefault(n => n.Key == "starred");
            if (starred != null) starred.Badge = allFiles.Count(f => f.Starred);
            var recent = NavItems.FirstOrDefault(n => n.Key == "recent");
            if (recent != null) recent.Badge = allFiles.Count;
            var trashNav = NavItems.FirstOrDefault(n => n.Key == "trash");
            if (trashNav != null) trashNav.Badge = Files.Count(f => f.Deleted);
        }

        // ══ DATA ════════════════════════════════════════════════════════
        public List<DriveItem> Drives { get; set; } = new()
        {
            new() { Name="Local Drive",   Icon="ti ti-device-desktop",    IconBg="#eef2ff", Color="#4f46e5", Total="100 GB", TotalMB=102400, UsedMB=46080,  Files=12 },
            new() { Name="Google Drive",  Icon="ti ti-brand-google-drive", IconBg="#fef2f2", Color="#ef4444", Total="15 GB",  TotalMB=15360,  UsedMB=12288,  Files=45 },
            new() { Name="OneDrive",      Icon="ti ti-cloud",              IconBg="#f0fdf4", Color="#10b981", Total="50 GB",  TotalMB=51200,  UsedMB=8192,   Files=23 },
            new() { Name="External HDD",  Icon="ti ti-device-floppy",      IconBg="#fffbeb", Color="#f59e0b", Total="500 GB", TotalMB=512000, UsedMB=204800, Files=88 },
        };

        public List<NavItem> NavItems { get; } = new()
        {
            new() { Key="all",     Label="All Folders / Files", Icon="ti ti-folder-up",    Badge=0 },
            new() { Key="drive",   Label="Drive",               Icon="ti ti-star",          Badge=0 },
            new() { Key="dropbox", Label="Dropbox",             Icon="ti ti-octahedron",    Badge=0 },
            new() { Key="shared",  Label="Shared with Me",      Icon="ti ti-share-2",       Badge=3 },
            new() { Key="docs",    Label="Documents",           Icon="ti ti-file",          Badge=0 },
            new() { Key="recent",  Label="Recent Files",        Icon="ti ti-clock-hour-11", Badge=0 },
            new() { Key="starred", Label="Starred",             Icon="ti ti-star-filled",   Badge=4 },
            new() { Key="media",   Label="Media",               Icon="ti ti-music",         Badge=0 },
        };

        public List<StorageCatItem> StorageCats { get; set; } = new()
        {
            new() { Icon="ti ti-photo",     Name="Images",    Color="#8b5cf6", Bg="#ede9fe", Exts=new[]{"jpg","jpeg","png","gif","webp","svg","bmp","ico"}, SizeMB=1228, Count=5 },
            new() { Icon="ti ti-file-text", Name="Documents", Color="#4f8ef7", Bg="#e8f4fd", Exts=new[]{"pdf","doc","docx","txt","md","csv"},              SizeMB=819,  Count=6 },
            new() { Icon="ti ti-music",     Name="Audio",     Color="#10b981", Bg="#d1fae5", Exts=new[]{"mp3","wav","ogg","flac","aac"},                   SizeMB=512,  Count=2 },
            new() { Icon="ti ti-video",     Name="Video",     Color="#f59e0b", Bg="#fef3c7", Exts=new[]{"mp4","mov","avi","webm","mkv"},                   SizeMB=2048, Count=3 },
            new() { Icon="ti ti-package",   Name="Archives",  Color="#ef4444", Bg="#fee2e2", Exts=new[]{"zip","rar","7z","tar","gz"},                      SizeMB=307,  Count=2 },
        };

        public List<FolderItem> Folders { get; set; } = new()
        {
            // Root folders — ParentId = 0
            new() { Id=1, Name="3D Objects",      Size=25.67, Total=50, Color="#ef4444", Starred=true,  ParentId=0 },
            new() { Id=2, Name="Document",        Size=45,    Total=52, Color="#f59e0b", Starred=false, ParentId=0 },
            new() { Id=3, Name="Scores",          Size=25.67, Total=50, Color="#ef4444", Starred=true,  ParentId=0 },
            new() { Id=4, Name="Picture",         Size=25.50, Total=50, Color="#10b981", Starred=false, ParentId=0 },
            new() { Id=5, Name="Tex",             Size=25.67, Total=50, Color="#6b7280", Starred=true,  ParentId=0 },
            new() { Id=6, Name="Cubase",          Size=25.67, Total=50, Color="#8b5cf6", Starred=false, ParentId=0 },
            new() { Id=7, Name="Projects",        Size=25.67, Total=50, Color="#4f46e5", Starred=true,  ParentId=0 },
            new() { Id=8, Name="Personal Assets", Size=24,    Total=50, Color="#f59e0b", Starred=false, ParentId=0 },
            new() { Id=9, Name="Handyimages",     Size=1.4,   Total=10, Color="#06b6d4", Starred=false, ParentId=0 },
        };

        public List<FileItem> Files { get; set; } = new()
        {
            // Root files (FolderId=null)
            new() { Id=1,  Name="Annual Report",      Ext="pdf",  Size=12, Date="12 Jan 2024" },
            new() { Id=2,  Name="Budget Sheet",       Ext="xlsx", Size=4,  Date="15 Jan 2024" },
            new() { Id=3,  Name="Team Photo",         Ext="jpg",  Size=8,  Date="20 Jan 2024" },
            new() { Id=4,  Name="Project Readme",     Ext="txt",  Size=1,  Date="22 Jan 2024" },
            new() { Id=5,  Name="Company Logo",       Ext="svg",  Size=2,  Date="25 Jan 2024" },
            new() { Id=6,  Name="Meeting Notes",      Ext="doc",  Size=3,  Date="28 Jan 2024" },
            new() { Id=7,  Name="Data Export",        Ext="csv",  Size=5,  Date="2 Feb 2024"  },
            new() { Id=8,  Name="App Config",         Ext="json", Size=1,  Date="5 Feb 2024"  },
            new() { Id=9,  Name="Stylesheet",         Ext="css",  Size=1,  Date="8 Feb 2024"  },
            new() { Id=10, Name="Presentation Draft", Ext="pptx", Size=18, Date="10 Feb 2024" },

            // Folder 1
            new() { Id=101, Name="Meeting Notes March",  Ext="txt", Size=4,  Date="15 Mar 2024", FolderId=1 },
            new() { Id=102, Name="Project Proposal v2",  Ext="doc", Size=32, Date="22 Feb 2024", FolderId=1, Starred=true },
            new() { Id=103, Name="Q1 Financial Summary", Ext="xls", Size=45, Date="10 Jan 2024", FolderId=1, Starred=true },

            // Folder 2
            new() { Id=201, Name="Banner",   Ext="png", Size=6, Date="1 Mar 2024", FolderId=2 },
            new() { Id=202, Name="Profile",  Ext="jpg", Size=3, Date="5 Mar 2024", FolderId=2, Starred=true },
            new() { Id=203, Name="Icon Set", Ext="svg", Size=1, Date="8 Mar 2024", FolderId=2 },

            // Folder 3
            new() { Id=301, Name="App Design", Ext="fig",  Size=18, Date="2 Feb 2024",  FolderId=3 },
            new() { Id=302, Name="API Docs",   Ext="md",   Size=2,  Date="10 Feb 2024", FolderId=3 },
            new() { Id=303, Name="Config",     Ext="json", Size=1,  Date="12 Feb 2024", FolderId=3 },

            // Folder 7
            new() { Id=701, Name="Brand Guidelines", Ext="pdf", Size=22, Date="3 Mar 2024", FolderId=7 },
            new() { Id=702, Name="Color Palette",    Ext="fig", Size=5,  Date="4 Mar 2024", FolderId=7 },
        };

        // ══ HELPERS ═════════════════════════════════════════════════════
        public static readonly Dictionary<string, FileMeta> MetaMap = new()
        {
            ["pdf"] = new("#FDEAEA", "#ef4444", "PDF"),
            ["doc"] = new("#E8F4FD", "#4f8ef7", "DOC"),
            ["docx"] = new("#E8F4FD", "#4f8ef7", "DOC"),
            ["fig"] = new("#F0EBFF", "#8b5cf6", "FIG"),
            ["img"] = new("#E8FDF2", "#10b981", "IMG"),
            ["jpg"] = new("#E8FDF2", "#10b981", "IMG"),
            ["jpeg"] = new("#E8FDF2", "#10b981", "IMG"),
            ["png"] = new("#E8FDF2", "#10b981", "PNG"),
            ["svg"] = new("#FFF4E6", "#f59e0b", "SVG"),
            ["txt"] = new("#FFFDE8", "#ca8a04", "TXT"),
            ["csv"] = new("#E8FDF2", "#16a34a", "CSV"),
            ["json"] = new("#FEF3C7", "#d97706", "JSON"),
            ["xml"] = new("#FEF3C7", "#d97706", "XML"),
            ["md"] = new("#F0EBFF", "#8b5cf6", "MD"),
            ["audio"] = new("#FDE8F5", "#db2777", "MP3"),
            ["mp3"] = new("#FDE8F5", "#db2777", "MP3"),
            ["mp4"] = new("#FEF3C7", "#f59e0b", "MP4"),
            ["xlsx"] = new("#E8FDF2", "#16a34a", "XLS"),
            ["xls"] = new("#E8FDF2", "#16a34a", "XLS"),
            ["pptx"] = new("#FDEAEA", "#ef4444", "PPT"),
            ["zip"] = new("#F1F5F9", "#64748b", "ZIP"),
            ["css"] = new("#E8F4FD", "#4f8ef7", "CSS"),
        };
    }
}
