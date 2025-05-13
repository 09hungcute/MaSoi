using Microsoft.AspNetCore.Mvc;
using WerewolfGame.Services;
using WerewolfGame.Models;

namespace WerewolfGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public IActionResult CreateGame()
        {
            var game = _gameService.CreateGame();
            return Ok(game);
        }

        [HttpPost("{gameId}/join")]
        public IActionResult JoinGame(string gameId, [FromBody] Player player)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
                return NotFound("Game not found.");

            if (game.IsStarted)
                return BadRequest("Game has already started.");

            _gameService.AddPlayer(gameId, player.Name);
            return Ok("Player added successfully.");
        }

        [HttpPost("{gameId}/start")]
        public IActionResult StartGame(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            if (game.Players.Count < 5)
            {
                return BadRequest("Not enough players to start the game.");
            }

            _gameService.StartGame(gameId);
            return Ok("Game started.");
        }

        [HttpPost("{gameId}/choose-role")]
        public IActionResult ChooseRole(string gameId, [FromBody] ChooseRoleRequest request)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
                return NotFound("Game not found.");

            var player = game.Players.FirstOrDefault(p => p.Id == request.PlayerId);
            if (player == null)
                return NotFound("Player not found.");

            _gameService.AssignRoleToPlayer(gameId, request.PlayerId, request.Role);
            return Ok($"Role {request.Role} assigned to {player.Name}.");
        }

        // CŨ - Bỏ phiếu trực tiếp loại ngay (không dùng nữa)
        // [HttpPost("{gameId}/vote")]
        // public IActionResult Vote(string gameId, [FromBody] string playerId)
        // {
        //     var game = _gameService.GetGame(gameId);
        //     if (game == null) return NotFound("Game not found.");

        //     var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        //     if (player == null) return NotFound("Player not found.");

        //     _gameService.VoteOutPlayer(gameId, playerId);
        //     return Ok($"Vote casted against {player.Name}.");
        // }

        // ✅ Thay thế bằng: Người chơi thực hiện vote
        [HttpPost("{gameId}/cast-vote")]
        public IActionResult CastVote(string gameId, [FromBody] VoteRequest request)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null) return NotFound("Game not found.");

            _gameService.CastVote(gameId, request.VoterId, request.TargetId);
            return Ok("Vote registered.");
        }

        // ✅ Giải quyết người bị vote nhiều nhất
        [HttpPost("{gameId}/resolve-vote")]
        public IActionResult ResolveVote(string gameId)
        {
            var result = _gameService.ResolveVoting(gameId);
            return Ok(result);
        }


        // ✅ Bắt đầu pha ban ngày (cho phép vote và khởi tạo 30s)
        [HttpPost("{gameId}/start-day")]
        public IActionResult StartDay(string gameId)
        {
            _gameService.StartDayPhase(gameId);
            return Ok("Day phase started.");
        }

        [HttpGet("{gameId}")]
        public IActionResult GetGameInfo(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            return game != null ? Ok(game) : NotFound("Game not found.");
        }

        [HttpPost("{gameId}/end")]
        public IActionResult EndGame(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            if (game != null && game.IsStarted)
            {
                game.IsStarted = false;
                return Ok("Game ended.");
            }
            return NotFound("Game not found or already ended.");
        }

        [HttpPut("{gameId}/update-player/{playerId}")]
        public IActionResult UpdatePlayer(string gameId, string playerId, [FromBody] Player updatedPlayer)
        {
            var game = _gameService.GetGame(gameId);
            var player = game?.Players.FirstOrDefault(p => p.Id == playerId);
            if (game != null && player != null)
            {
                player.Name = updatedPlayer.Name;
                player.Role = updatedPlayer.Role;
                return Ok("Player updated.");
            }
            return NotFound("Player not found.");
        }
    }

    public class ChooseRoleRequest
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class VoteRequest
    {
        public string VoterId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
    }

}
