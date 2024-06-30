using Blaztrix.Components.Tetris.Engine;
using System;

namespace Blaztrix.Services
{
    public class TetrisStateContainer
    {
        private bool settingsAccepted;

        private Session session;

        public event Action StateChanged;

        public Session Session
        {
            get => session;
            set
            {
                session = value;
                session.FieldState.Blocked += () => StateChanged?.Invoke();
                StateChanged?.Invoke();
            }
        }

        public bool SettingsAccepted
        {
            get => settingsAccepted;
            set
            {
                settingsAccepted = value;
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
                            return CurrentState.Paused;
                        else return CurrentState.InGame;
                    }
                    else return CurrentState.Lost;
                }
                else return CurrentState.Settings;
            }
        }
    }
}