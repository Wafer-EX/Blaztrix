using Blaztrix.Core;

namespace Blaztrix.Services
{
    public enum CurrentState : byte
    {
        NotInitialized, InGame, Paused, Lost
    }

    public class BlaztrixStateContainer
    {
        private Session? _session;

        public event Action? StateChanged;

        public Session? Session
        {
            get => _session;
            set
            {
                _session = value;
                if (_session != null)
                    _session.FieldState.Blocked += () => StateChanged?.Invoke();

                StateChanged?.Invoke();
            }
        }

        public CurrentState CurrentState
        {
            get
            {
                if (_session == null)
                {
                    return CurrentState.NotInitialized;
                }
                else
                {
                    if (!_session.IsLost)
                    {
                        if (_session.FieldState.IsBlocked)
                        {
                            return CurrentState.Paused;
                        }
                        else
                        {
                            return CurrentState.InGame;
                        }
                    }
                    else
                    {
                        return CurrentState.Lost;
                    }
                }
            }
        }
    }
}