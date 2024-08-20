using UnityEngine;

namespace Signals
{
    // UI Signals
    public struct SignalUiMainMenuStartGame
    {
        public string levelToLoad;
    }

    public struct SignalUiMainMenuTooltipChange
    {
        public bool Showing;
        public string LevelName, ScoreType1, ScoreType2;
    }



    // Game End Signals
    public enum GameEndCondition
    {
        Loss, WinType1, WinType2
    }
    public struct SignalGameEnded
    {
        public GameEndCondition result;
        public int score;
    }
    // Game Signals
    public struct SignalToggleEffect
    {
        public bool Enabled;
    }

    public struct SignalSwitchCameraBoundary
    {
        public Collider2D ColliderToSwitch;
    }

    public struct SignalBoxesSwitchToContinuousRbDetection
    {
        // Boxes can clip through terrain if Puffy inflates while very close to them.
        // To prevent expensive operations every frame, call this Signal to rapidly turn on/off the continous detection
        public bool ContinuousMode;
    }
}

