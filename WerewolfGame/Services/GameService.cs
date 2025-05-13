using WerewolfGame.Models;
using System.Linq;

namespace WerewolfGame.Services
{
    public class GameService
    {
        private readonly List<Game> _games = new();

        // Tạo trò chơi mới


        public Game CreateGame()
        {
            var game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Players = new List<Player>(),  // Khởi tạo danh sách người chơi
                CurrentPhase = "Lobby",       // Giai đoạn ban đầu là "Lobby"
                Round = 0,                    // Bắt đầu với vòng 0
                IsStarted = false             // Trò chơi chưa bắt đầu
            };

            _games.Add(game);  // Thêm game vào danh sách
            return game;
        }

        // Thêm người chơi vào trò chơi
        public void AddPlayer(string gameId, string playerName)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && !game.IsStarted && game.Players.Count < 10) // Giới hạn số lượng người chơi tối đa là 10
            {
                game.Players.Add(new Player
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = playerName
                });
            }
        }

        // Bắt đầu trò chơi
        public void StartGame(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && game.Players.Count >= 5) // Đảm bảo có đủ người chơi
            {
                AssignRoles(game);
                game.IsStarted = true;
                game.CurrentPhase = "Night";
                game.Round = 1;
            }
        }

        // Gán vai trò ngẫu nhiên cho người chơi
        private void AssignRoles(Game game)
        {
            // Giả sử: 1 Sói, 1 Tiên Tri, còn lại là Dân
            var roles = new List<string> { "Werewolf", "Seer", "Villager", "Villager", "Villager" };
            var random = new Random();

            foreach (var player in game.Players)
            {
                var roleIndex = random.Next(roles.Count);
                player.Role = roles[roleIndex];
                roles.RemoveAt(roleIndex); // Đảm bảo mỗi vai trò chỉ được gán 1 lần
            }
        }

        // Gán vai trò cho người chơi
        public void AssignRoleToPlayer(string gameId, string playerId, string role)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                var player = game.Players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                {
                    player.Role = role;
                }
            }
        }

        // Bỏ phiếu và loại bỏ người chơi khỏi trò chơi
        public void VoteOutPlayer(string gameId, string playerId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                var player = game.Players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                {
                    game.Players.Remove(player);

                    // Nếu hết người chơi, kết thúc trò chơi
                    if (game.Players.Count == 0)
                    {
                        EndGame(gameId);
                    }
                }
            }
        }

        // Lấy thông tin trò chơi theo gameId
        public Game GetGame(string gameId)
        {
            return _games.FirstOrDefault(g => g.Id == gameId);
        }

        // Kết thúc trò chơi
        public void EndGame(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                game.IsStarted = false;
                game.CurrentPhase = "End";
            }
        }
    }
}
