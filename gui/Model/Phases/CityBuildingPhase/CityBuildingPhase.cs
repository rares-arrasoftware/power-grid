using gui.Model.Managers.InfoManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using gui.Model.Phases.ResourceBuyingPhase;
using Serilog;
using System;
using System.IO;
using System.Media;
using System.Numerics;
using static gui.Model.Managers.PlayerManager.Status;

namespace gui.Model.Phases.CityBuildingPhase
{

    public class CityBuildingPhase : Phase
    {
        private Stack<Player> _players = [];

        private TaskCompletionSource<bool>? _nextTcs = null;

        private Action<int, Player>? _buildUpdated;

        private Player? _active;

        private readonly Dictionary<Button, Action> _buttonActions;

        private int _currentCities = 0;

        public CityBuildingPhase(Action<int, Player>? buildUpdated)
        {
            _buildUpdated = buildUpdated;

            _buttonActions = new()
            {
                { Button.BtnA, IncreaseCities },
                { Button.BtnB, DecreaseCities },
                { Button.BtnC, ReadyBuild },
                { Button.BtnD, DoneBuild }
            };
        }

        public override Task Execute()
        {
            var tcs = new TaskCompletionSource<bool>();

            Log.Information("Running: CityBuildingPhase");
            RemoteManager.Instance.ButtonPressed += OnButtonPressed;

            // Run the task asynchronously on a background thread
            Task.Run(async () =>
            {
                await RunPlayerTasks();
                RemoteManager.Instance.ButtonPressed -= OnButtonPressed;
                tcs.SetResult(true); // Signal that the phase is done
            });

            return tcs.Task;
        }

        private async Task RunPlayerTasks()
        {
            _players = new Stack<Player>(PlayerManager.Instance.GetPlayers());


            while (_players.Count > 0)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", "attention.wav");
                using (var media = new SoundPlayer(path))
                {
                    media.Play(); // or PlaySync() if you want to block
                }

                _nextTcs = new TaskCompletionSource<bool>();
                _active = _players.Pop();
                PlayerManager.Instance.SetPlayerState(_active, PlayerState.Active);
                Log.Information("{PlayersName} to act.", _active.Name);
                UpdateInfo(_active.Name);
                _buildUpdated?.Invoke(_currentCities, _active);
                await _nextTcs.Task; // Await here while .Wait() blocks in Execute
            }
        }

        public void OnButtonPressed(Player player, Button btn)
        {
            if (player is not { Status.State: PlayerState.Active })
                return;

            if (_buttonActions.TryGetValue(btn, out var action))
            {
                action.Invoke();
            }
        }                

        public void UpdateInfo(string playerName)
        {
            InfoManager.Instance.PhaseName = "Build Cities";
            InfoManager.Instance.PlayerName = playerName;
            InfoManager.Instance.OptionA = "+1";
            InfoManager.Instance.OptionB = "-1";
            InfoManager.Instance.OptionC = "Ready";
            InfoManager.Instance.OptionD = "Done";
        }

        private void IncreaseCities()
        {
            Log.Information("IncreasCities");
            _currentCities++;
            if(_active != null)
                _buildUpdated?.Invoke(_currentCities, _active);
        }

        public void DecreaseCities()
        {
            Log.Information("DecreaseCities");
            if (_currentCities > 0) 
            { 
                _currentCities--;
                if (_active != null)
                    _buildUpdated?.Invoke(_currentCities, _active);
            }       
        }

        public void ReadyBuild()
        {
            _active?.BuildCities(_currentCities);
            _currentCities = 0;
            if (_active != null)
                _buildUpdated?.Invoke(_currentCities, _active);
        }

        public void DoneBuild()
        {
            ReadyBuild();
            if (_active != null)
                PlayerManager.Instance.SetPlayerState(_active, PlayerState.Done);
            _nextTcs?.TrySetResult(true);
        }

        public void UpdateBuild(int amount)
        {
            Log.Information("UpdateBuild {Amount}", amount);
            (amount < 0 ? (Action)DecreaseCities : IncreaseCities).Invoke();
        }
    }
}

    
    
