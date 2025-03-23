using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text.Json;
using gui.Model.Managers.MarketManager;
using gui.Model.Managers.PlayerManager;
using gui.Model.Phases;
using gui.View;
using gui.View.CardEditor;
using Serilog;

namespace gui.Model.Managers.CardManager
{
    public class CardManager
    {
        private static readonly CardManager _instance = new();
        public static CardManager Instance => _instance;

        private readonly Dictionary<int, Card> _cards = [];
        private readonly Dictionary<Card, Player> _assignedCards = [];
        private const string CardsFilePath = "cards.json";

        public event Action<Card>? NewCardScanned;
        public event Action<int>? UnknownCardScanned;

        public bool Level3 = false;

        private CardManager()
        {
            LoadCards();
        }

        /// <summary>
        /// Loads all cards from `cards.json` into memory.
        /// </summary>
        private void LoadCards()
        {
            if (!File.Exists(CardsFilePath))
            {
                File.WriteAllText(CardsFilePath, "[]"); // Create an empty JSON array if the file doesn't exist
            }

            try
            {
                string json = File.ReadAllText(CardsFilePath);
                var loadedCards = JsonSerializer.Deserialize<List<Card>>(json) ?? [];

                foreach (var card in loadedCards)
                {
                    _cards[card.Id] = card;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cards.json: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles scanning a card, adding it to memory if unknown.
        /// </summary>
        public void OnCardScanned(int cardId)
        {
            Log.Information($"OnCardScanned, id: {cardId}");

            if (!_cards.ContainsKey(cardId))
            {
                Log.Information($"We don't know this");
                UnknownCardScanned?.Invoke(cardId);
                return;
            }

            Card card = _cards[cardId];

            if (card.Type == CardType.Event)
            {
                for (int i = 0; i < card.MarketEffect.Count; i++)
                {
                    MarketManager.MarketManager.Instance.Update((ResourceType)i, card.MarketEffect[i]);
                }
                MarketManager.MarketManager.Instance.UpdateMostLimitedResource(card.MarketEffectLowest);
                
                Level3 = card.Level3;
            }
            App.LogPanelViewModel.Add($"Card scanned: {cardId}, {card.Type}, {card.Rank}");

            BurnCard(card);

            NewCardScanned?.Invoke(card);
        }


        /// <summary>
        /// Saves all cards back to `cards.json` for redundancy.
        /// </summary>
        private void SaveCards()
        {
            Log.Information($"Save Cards: {CardsFilePath}");
            try
            {
                var json = JsonSerializer.Serialize(_cards.Values, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(CardsFilePath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Error saving cards.json: {ex.Message}");
            }
        }

        public void AddCard(Card card)
        {
            _cards[card.Id] = card;

            SaveCards();
        }

        public Card GetCard(int cardId)
        {
            return _cards[cardId];
        }

        public void AssignCard(Player player, Card? card)
        {
            if (card == null) return;
            _assignedCards[card] = player;
            player.AddCard(card);
        }

        public void BurnCard(Card card)
        {
            if (_assignedCards.Remove(card, out var assignedPlayer))
            {
                App.LogPanelViewModel.Add($"Card Burned: {card.Rank}, player: {assignedPlayer.Name}");
                assignedPlayer.BurnCard(card);
            }
        }
    }
}
