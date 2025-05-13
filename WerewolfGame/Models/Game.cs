using System.Collections.Generic;

namespace WerewolfGame.Models
{
    public class Game
    {
        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public string CurrentPhase { get; set; }
        public int Round { get; set; }
        public bool IsStarted { get; set; }

        // Constructor khởi tạo giá trị mặc định
        public Game()
        {
            Id = Guid.NewGuid().ToString();  // Khởi tạo ID bằng Guid mới
            Players = new List<Player>();  // Khởi tạo danh sách người chơi
            CurrentPhase = "Lobby";  // Mặc định ở giai đoạn Lobby
            Round = 0;  // Bắt đầu từ vòng 0
            IsStarted = false;  // Trò chơi chưa bắt đầu
        }
    }
}
