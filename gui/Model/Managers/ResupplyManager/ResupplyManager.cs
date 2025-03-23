using gui.Model.Managers.MarketManager;
using gui.Model.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gui.Model.Managers.ResupplyManager
{
    public class ResupplyManager
    {
        private static readonly ResupplyManager _instance = new();
        public static ResupplyManager Instance => _instance;

        public event EventHandler? LevelChanged; // New event for updates

        private ResupplyManager()
        {
            LoadSuppliesFromCsv();
        }

        private int _level = 1;
        public int Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    LevelChanged?.Invoke(this, EventArgs.Empty); // Notify listeners
                }
            }
        }

        private readonly List<List<int>> _levels = [];

        public int GetSupply(ResourceType resource)
        {
            return _levels[(int)resource][Level - 1];
        }

        public List<List<int>> GetLevels()
        {
            return _levels;
        }

        public void Reload()
        {
            _levels.Clear();
            LoadSuppliesFromCsv();
        }

        private void LoadSuppliesFromCsv()
        {
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Supplies.csv");

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException($"CSV file not found: {csvPath}");
            }

            var lines = File.ReadAllLines(csvPath);

            foreach (var line in lines.Skip(1))
            {
                var columns = line.Split(',');

                if (Enum.TryParse<MarketManager.ResourceType>(columns[0].Trim(), true, out var resource))
                {
                    var levels = columns.Skip(1)
                                        .Select(col => int.TryParse(col, out var value) ? value : -1)
                                        .ToList();
                    _levels.Add(levels);
                }
            }
        }
    }
}
