using LisoTetris.Components.Tetris.Engine;

namespace LisoTetris.Services
{
    public class TetrisStateContainer
    {
        public Session Session { get; set; }

        public bool SettingsAccepted { get; set; }

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