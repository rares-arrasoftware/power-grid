using System.IO;
using gui.Model.Managers.MarketManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.RemoteManager;
using gui.Model.Phases;
using gui.Model.Phases.ResourceBuyingPhase;
using Serilog;


namespace gui.Model
{
    public class GameManager
    {
        public static GameManager Instance { get; } = new(); // Singleton instance

        private readonly List<Round> _rounds = [];

        public event Action<Phase>? PhaseChanged;

        public event Action<PurchaseData>? PurchaseRecordUpdated;

        public event Action<int, Player>? BuildUpdated;


        private GameManager() 
        { 
            LoadPlayersFromCsv();
        }

        public async Task StartRound()
        {
            Round r = new();
            _rounds.Add(r);

            Log.Information("Round number {RoundCount} started!", _rounds.Count);
            App.LogPanelViewModel.Add($"Round {_rounds.Count} started");

            r.PhaseChanged += OnPhaseChanged;
            r.PurchasedUpdated += OnPurchaseRecordUpdated;
            r.BuildUpdated += OnBuildUpdated;
            await r.Start();
            r.PhaseChanged -= OnPhaseChanged;
            r.PurchasedUpdated -= OnPurchaseRecordUpdated;
            r.BuildUpdated -= OnBuildUpdated;
        }

        public void AddPlayer(string name, int remoteId)
        {
            Player player = PlayerManager.Instance.AddPlayer(name);
            RemoteManager.Instance.AssignRemote(remoteId, player);
        }

        // 4. Private Methods
        private void LoadPlayersFromCsv()
        {
            // Path to the CSV file in the Assets folder
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Players.csv");

            Log.Information("csvPath: {csvPath}", csvPath);

            // Read all lines from the CSV file
            var lines = File.ReadAllLines(csvPath);

            // Process each line (excluding the header row)
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');

                if (parts.Length < 3)
                {
                    Log.Warning("Skipping malformed line: {line}", line);
                    continue;
                }

                string playerName = parts[0].Trim('"').Trim();
                bool isActive = parts[1].Trim('"').Trim() == "1";
                bool parsedRemoteId = int.TryParse(parts[2].Trim('"').Trim(), out int remoteId);

                if (isActive && parsedRemoteId)
                {
                    AddPlayer(playerName, remoteId);
                }
                else
                {
                    Log.Warning("Skipping inactive player or invalid remote ID: {line}", line);
                }
            }
        }

        public void OnPhaseChanged(Phase phase)
        {
            PhaseChanged?.Invoke(phase);
        }

        public void OnPurchaseRecordUpdated(PurchaseData purchaseRecord)
        {
            Log.Information("OnPurchaseRecordUpdated");
            PurchaseRecordUpdated?.Invoke(purchaseRecord);
        }

        public void OnBuildUpdated(int amount, Player player)
        {
            Log.Information("OnBuildUpdated");
            BuildUpdated?.Invoke(amount, player);
        }

        public void UpdatePurchaseRecord(ResourceType type, int amount)
        {
            _rounds.Last().UpdatePurchaseRecord(type, amount);
        }

        public void UpdateBuild(int amount)
        {
            _rounds.Last().UpdateBuild(amount);
        }

        public void Done()
        {
            _rounds.Last().Done();
        }

        public void Ready()
        {
            _rounds.Last().Ready();
        }

        public void ReloadPlayers()
        {
            PlayerManager.Instance.Players.Clear();
            LoadPlayersFromCsv();
        }

        public bool IsRound(int round)
        {
            return _rounds.Count == round;
        }
    }
}
