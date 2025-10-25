using HarborFlow.Core.Models;
using System;

namespace HarborFlow.Wpf.Services
{
    public class SessionContext
    {
        private User? _currentUser;
        public User? CurrentUser 
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnUserChanged();
            }
        }

        public event Action? UserChanged;

        protected virtual void OnUserChanged()
        {
            UserChanged?.Invoke();
        }
    }
}