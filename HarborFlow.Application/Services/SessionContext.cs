using HarborFlow.Core.Models;
using System;

namespace HarborFlow.Application.Services
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

        public void Logout()
        {
            CurrentUser = null;
        }

        protected virtual void OnUserChanged()
        {
            UserChanged?.Invoke();
        }
    }
}
