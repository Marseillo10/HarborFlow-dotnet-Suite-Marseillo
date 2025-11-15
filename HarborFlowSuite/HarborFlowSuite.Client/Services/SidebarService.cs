namespace HarborFlowSuite.Client.Services
{
    public class SidebarService
    {
        public bool IsCollapsed { get; private set; }

        public event Action? OnChange;

        public void ToggleSidebar()
        {
            IsCollapsed = !IsCollapsed;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
