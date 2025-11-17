using System;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// Central game manager. Persistent across scenes.
    /// Controls game state (phases), holds GameStats, references to data like StormData. Will update more later
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Data & runtime refs")]
        public GameStats gameStats;
        public ScriptableStormData stormDataAsset; // assign via Inspector

        // Current state
        private GameStateBase _currentState;

        // Runtime storm controller created from ScriptableStormData
        public StormBase CurrentStorm { get; private set; }

        #region Events
        // Important events other systems can subscribe to
        public event Action<GameStateBase> OnStateChanged;
        public event Action OnGamePaused;
        public event Action OnGameResumed;
        public event Action OnGameEnded;
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (gameStats == null)
                gameStats = new GameStats(); // minimal fallback

            // instantiate runtime storm controller from ScriptableObject
            if (stormDataAsset != null)
                CurrentStorm = new StormBase(stormDataAsset);
        }

        private void Start()
        {
            // Start with main menu state by default
            ChangeState(new GameStartMenuState(this));
        }

        private void Update()
        {
            _currentState?.OnUpdate();
        }

        public void ChangeState(GameStateBase newState)
        {
            if (newState == null) return;

            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();

            OnStateChanged?.Invoke(_currentState);
        }

        public void PauseGame()
        {
            // you can also set timeScale if appropriate
            OnGamePaused?.Invoke();
            ChangeState(new GamePauseState(this, _currentState)); // pause wrapper
        }

        public void ResumeFromPause()
        {
            OnGameResumed?.Invoke();

            // If current state is a GamePauseState we can restore previous state
            if (_currentState is GamePauseState pauseState)
            {
                ChangeState(pauseState.RestoreState ?? new GameStartMenuState(this));
            }
        }

        public void EndGame()
        {
            OnGameEnded?.Invoke();
            ChangeState(new GameEndState(this));
        }

        #region Helpers
        // Convenience methods called by UI/buttons
        public void StartPreparePhase()
        {
            ChangeState(new PreparePhaseState(this));
        }

        public void StartStormPhase()
        {
            // ensure storm data present
            if (CurrentStorm == null && stormDataAsset != null)
                CurrentStorm = new StormBase(stormDataAsset);

            ChangeState(new StormPhaseState(this));
        }

        public void StartStormEndPhase()
        {
            ChangeState(new StormEndPhaseState(this));
        }
        #endregion
    }
}
