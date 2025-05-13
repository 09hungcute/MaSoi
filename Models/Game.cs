using System;
using System.Collections.Generic;

namespace WerewolfGame.Models
{
    public class Game
    {
        public string Id { get; set; }
        public int Round { get; set; }
        public bool IsStarted { get; set; }
        public string CurrentPhase { get; set; } = "Lobby";

        public List<Player> Players { get; set; } = new List<Player>();

        // Dữ liệu phiếu bầu
        public Dictionary<string, int> Votes { get; set; } = new Dictionary<string, int>();
        public HashSet<string> VotedPlayers { get; set; } = new HashSet<string>();  // Những người đã vote
        public DateTime? VotingStartTime { get; set; } = null;

        // Constructor khởi tạo giá trị mặc định
        public Game()
        {
            Id = Guid.NewGuid().ToString();
            Players = new List<Player>();
            CurrentPhase = "Lobby";
            Round = 0;
            IsStarted = false;
        }
    }
}
