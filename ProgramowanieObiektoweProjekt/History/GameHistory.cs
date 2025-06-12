using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ProgramowanieObiektoweProjekt
{
    public class GameHistory
    {
        private static readonly string HistoryFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "History");

        public GameHistory()
        {
            if (!Directory.Exists(HistoryFolder))
            {
                Directory.CreateDirectory(HistoryFolder);
            }
        }

        public void SaveGameResult(string gameResult)
        {
            string fileName = $"Game_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string fullPath = Path.Combine(HistoryFolder, fileName);
            File.WriteAllText(fullPath, gameResult, Encoding.UTF8);
        }

        public void AppendToLatestGame(string line)
        {
            var files = Directory.GetFiles(HistoryFolder, "Game_*.txt");
            if (files.Length == 0)
                throw new InvalidOperationException("No game history files found.");

            Array.Sort(files);
            string latestFile = files[files.Length - 1];

            File.AppendAllText(latestFile, line + Environment.NewLine, Encoding.UTF8);
        }

        public string[] ListHistoryFiles()
        {
            return Directory.GetFiles(HistoryFolder, "Game_*.txt");
        }

        public string ReadGameHistory(string fileName)
        {
            string fullPath = Path.Combine(HistoryFolder, fileName);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath, Encoding.UTF8) : null;
        }

        public string[] ReadLatestThreeGames()
        {
            var files = Directory.GetFiles(HistoryFolder, "Game_*.txt")
                .OrderByDescending(f => f)
                .Take(3)
                .ToArray();

            return files.Select(f => File.ReadAllText(f, Encoding.UTF8)).ToArray();
        }
    }
}