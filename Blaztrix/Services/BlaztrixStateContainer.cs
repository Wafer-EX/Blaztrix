using Blaztrix.Core;

namespace Blaztrix.Services
{
    public enum CurrentState : byte { NotInitialized, InGame, Paused, Lost }

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
                    return CurrentState.NotInitialized;

                return _session.IsLost ? CurrentState.Lost : (_session.FieldState.IsBlocked ? CurrentState.Paused : CurrentState.InGame);
            }
        }
    }
}