using Blaztrix.Core;

namespace Blaztrix.Services
{
    public class BlaztrixStateContainer
    {
        private bool _settingsAccepted;
        private Session _session;

        public event Action StateChanged;

        public Session Session
        {
            get => _session;
            set
            {
                _session = value;
                _session.FieldState.Blocked += () => StateChanged?.Invoke();
                StateChanged?.Invoke();
            }
        }

        public bool SettingsAccepted
        {
            get => _settingsAccepted;
            set
            {
                _settingsAccepted = value;
                StateChanged?.Invoke();
            }
        }

        public CurrentState CurrentState
        {
            get
            {
                if (SettingsAccepted)
                {
                    if (!Session.IsLost)
                    {
                        if (Session.FieldState.IsBlocked)
                        {
                            return CurrentState.Paused;
                        }
                        else return CurrentState.InGame;
                    }
                    else return CurrentState.Lost;
                }
                else return CurrentState.Settings;
            }
        }
    }
}