﻿using gui.Model.Managers.CardManager;
using gui.Model.Managers.InfoManager;
using gui.Model.Managers.MarketManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Managers.ResupplyManager;
using Serilog;

namespace gui.Model.Phases.BureaucracyPhase
{
    public class BureaucracyPhase : Phase
    {
        public override Task Execute()
        {
            Log.Information("Bureaucracy Phase started.");

            Log.Information("Processing supply for Level {Level}.", ResupplyManager.Instance.Level);

            UpdateInfo();

            Enum.GetValues(typeof(ResourceType))
                .Cast<ResourceType>()
                .SelectMany(resource => Enumerable.Repeat(resource, ResupplyManager.Instance.GetSupply(resource)))
                .ToList()
                .ForEach(resource => _ = MarketManager.Instance.Sell(resource)); // Discard return value

            if (ResupplyManager.Instance.Level == 1 && PlayerManager.Instance.Players.Any(player => player.CitiesCount >= 7))
            {
                Log.Information("A player has 7+ cities. Advancing Resupply Level to 2.");
                App.LogPanelViewModel.Add($"7+ cities -> Level2");
                ResupplyManager.Instance.Level = 2;
            }

            if (CardManager.Instance.IsLevel3)
            {
                Log.Information("Level 3 activated by CardManager.");
                App.LogPanelViewModel.Add($"Level 3 activated by CardManager.");
                ResupplyManager.Instance.Level = 3;
            }

            PlayerManager.Instance.Reorder();

            return Task.CompletedTask;
        }

        public void UpdateInfo()
        {
            InfoManager.Instance.PhaseName = "Bureaucracy";
            InfoManager.Instance.PlayerName = "N/A";
            InfoManager.Instance.OptionA = "N/A";
            InfoManager.Instance.OptionB = "N/A";
            InfoManager.Instance.OptionC = "N/A";
            InfoManager.Instance.OptionD = "N/A";
        }
    }
}
