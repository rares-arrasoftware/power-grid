using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.PlayerManager;
using Serilog;

namespace PlayerInput.Model.Managers.RemoteManager
{
    public enum Button
    {
        BtnA = 1,
        BtnB = 2,
        BtnC = 4,
        BtnD = 8,
    }

    public class RemoteManager
    {
        private static readonly RemoteManager _instance = new();
        public static RemoteManager Instance => _instance;
        private RemoteManager() { }

        private readonly Dictionary<int, Player> _remotes = [];

        private readonly Dictionary<string, int> _playersRemote = [];

        public event Action<Player, Button>? ButtonPressed;

        public void OnButtonPressed(int remoteId, int btn)
        {
            Log.Information($"remoteId: {remoteId}, btn: {btn}");

            if (!_remotes.TryGetValue(remoteId, out var player) ||
                !Enum.IsDefined(typeof(Button), btn))
                return;

            App.LogPanelViewModel.Add($"Player: {_remotes[remoteId].Name} pressed {btn}");

            ButtonPressed?.Invoke(player, (Button)btn);
        }

        public void AssignRemote(int remoteId, Player player)
        {
            Log.Information($"{remoteId} was assigned to {player.Name}");
            _remotes[remoteId] = player;
            _playersRemote[player.Name] = remoteId;
        }


        public int GetPlayerRemote(string playerName)
        {
            return _playersRemote[playerName];
        }
    }
}
