namespace FinanceConnect.Client.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; } = ToastType.Info;
        public int Duration { get; set; } = 3000; // milliseconds
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class ToastService
    {
        private readonly List<ToastMessage> _toasts = new();
        public IReadOnlyList<ToastMessage> Toasts => _toasts.AsReadOnly();

        public event Action? OnChange;

        public void ShowSuccess(string message, string title = "Success", int duration = 3000)
        {
            Show(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Success,
                Duration = duration
            });
        }

        public void ShowError(string message, string title = "Error", int duration = 5000)
        {
            Show(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Error,
                Duration = duration
            });
        }

        public void ShowWarning(string message, string title = "Warning", int duration = 4000)
        {
            Show(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Warning,
                Duration = duration
            });
        }

        public void ShowInfo(string message, string title = "Info", int duration = 3000)
        {
            Show(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Info,
                Duration = duration
            });
        }

        private void Show(ToastMessage toast)
        {
            _toasts.Add(toast);
            OnChange?.Invoke();

            // Auto-remove after duration
            _ = Task.Run(async () =>
            {
                await Task.Delay(toast.Duration);
                Remove(toast.Id);
            });
        }

        public void Remove(string id)
        {
            var toast = _toasts.FirstOrDefault(t => t.Id == id);
            if (toast != null)
            {
                _toasts.Remove(toast);
                OnChange?.Invoke();
            }
        }

        public void Clear()
        {
            _toasts.Clear();
            OnChange?.Invoke();
        }
    }
}
