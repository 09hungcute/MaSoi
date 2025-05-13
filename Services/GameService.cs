using WerewolfGame.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace WerewolfGame.Services
{
    public class GameService
    {
        private readonly List<Game> _games = new();

        public Game CreateGame()
        {
            var game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Players = new List<Player>(),
                CurrentPhase = "Lobby",
                Round = 0,
                IsStarted = false,
                Votes = new Dictionary<string, int>(),
                VotedPlayers = new HashSet<string>(),
                VotingStartTime = null
            };
            _games.Add(game);
            return game;
        }

        public void AddPlayer(string gameId, string playerName)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && !game.IsStarted && game.Players.Count < 10)
            {
                game.Players.Add(new Player
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = playerName
                });
            }
        }

        public void StartGame(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null && game.Players.Count >= 5)
            {
                AssignRoles(game);
                game.IsStarted = true;
                game.CurrentPhase = "Night";
                game.Round = 1;
            }
        }

        private void AssignRoles(Game game)
        {
            var roles = new List<string> { "Werewolf", "Seer", "Villager", "Villager", "Villager" };
            var random = new Random();

            foreach (var player in game.Players)
            {
                var roleIndex = random.Next(roles.Count);
                player.Role = roles[roleIndex];
                roles.RemoveAt(roleIndex);
            }
        }

        public void StartVoting(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                game.Votes = new Dictionary<string, int>();
                game.VotedPlayers = new HashSet<string>(game.VotedPlayers.ToList());
                game.VotingStartTime = DateTime.UtcNow;
            }
        }

        public void Vote(string gameId, string voterId, string targetId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);

            if (game == null || game.VotedPlayers.Contains(voterId))
                throw new InvalidOperationException("You have already voted or game not found.");

            if (!game.Votes.ContainsKey(targetId))
                game.Votes[targetId] = 0;

            game.Votes[targetId]++;
            game.VotedPlayers.Add(voterId);
        }

        // public string EndVoting(string gameId)
        // {
        //     var game = _games.FirstOrDefault(g => g.Id == gameId);
        //     if (game == null || game.Votes.Count == 0)
        //         return null;

        //     var maxVotes = game.Votes.Max(v => v.Value);
        //     var votedOut = game.Votes.FirstOrDefault(v => v.Value == maxVotes).Key;

        //     var player = game.Players.FirstOrDefault(p => p.Id == votedOut);
        //     if (player != null)
        //     {
        //         game.Players.Remove(player);
        //     }

        //     return player?.Name;
        // }

        public void StartDayPhase(string gameId)
        {
            var game = GetGame(gameId);
            if (game != null)
            {
                game.CurrentPhase = "Day";
                game.Votes = new Dictionary<string, int>();
                game.VotedPlayers = new HashSet<string>(game.VotedPlayers.ToList());
                game.VotingStartTime = DateTime.UtcNow;
            }
        }

        public void CastVote(string gameId, string voterId, string targetId)
        {
            var game = GetGame(gameId);
            if (game == null || game.CurrentPhase != "Day") return;

            if (game.VotingStartTime.HasValue && (DateTime.UtcNow - game.VotingStartTime.Value).TotalSeconds > 30)
            {
                return;
            }

            if (!game.Votes.ContainsKey(targetId))
                game.Votes[targetId] = 0;

            game.Votes[targetId]++;
        }

        public string ResolveVoting(string gameId)
        {
            var game = GetGame(gameId);
            if (game == null || game.Votes.Count == 0)
                return "Không có phiếu bầu.";

            // Kiểm tra nếu chưa đến 30 giây thì không xử lý
            if (!game.VotingStartTime.HasValue || (DateTime.UtcNow - game.VotingStartTime.Value).TotalSeconds < 30)
                return "Chưa đến thời điểm kết thúc vote.";

            var voteResult = game.Votes
                .GroupBy(v => v.Key)
                .Select(g => new { PlayerId = g.Key, Count = g.Sum(v => v.Value) })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            if (voteResult != null)
            {
                var eliminatedPlayer = game.Players.FirstOrDefault(p => p.Id == voteResult.PlayerId);
                if (eliminatedPlayer != null)
                {
                    game.Players.Remove(eliminatedPlayer);
                    game.Votes.Clear();
                    game.VotedPlayers.Clear();
                    game.CurrentPhase = "Night";
                    return $"{eliminatedPlayer.Name} đã bị loại.";
                }
            }

            game.Votes.Clear();
            game.VotedPlayers.Clear();
            game.CurrentPhase = "Night";
            return "Không ai bị loại.";
        }


        public void AssignRoleToPlayer(string gameId, string playerId, string role)
        {
            var game = GetGame(gameId);
            var player = game?.Players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                player.Role = role;
            }
        }

        public Game? GetGame(string gameId)
        {
            return _games.FirstOrDefault(g => g.Id == gameId);
        }

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
