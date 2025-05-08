using Microsoft.AspNetCore.Mvc;
using WerewolfGame.Services;  // Dịch vụ xử lý logic
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

        // Tạo phòng game mới
        [HttpPost("create")]
        public IActionResult CreateGame()
        {
            var game = _gameService.CreateGame();
            return Ok(game);  // Trả về game vừa tạo
        }

        // Tham gia phòng game
        [HttpPost("{gameId}/join")]
        public IActionResult JoinGame(string gameId, [FromBody] Player player)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            if (game.IsStarted)
            {
                return BadRequest("Game has already started.");
            }

            _gameService.AddPlayer(gameId, player.Name);
            return Ok("Player added successfully.");
        }

        // Bắt đầu trò chơi
        [HttpPost("{gameId}/start")]
        public IActionResult StartGame(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            // if (game.Players.Count < 5)
            // {
            //     return BadRequest("Not enough players to start the game.");
            // }

            _gameService.StartGame(gameId);
            return Ok("Game started.");
        }

        // Chọn vai trò cho player
        [HttpPost("{gameId}/choose-role")]
        public IActionResult ChooseRole(string gameId, [FromBody] ChooseRoleRequest request)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            var player = game.Players.FirstOrDefault(p => p.Id == request.PlayerId);
            if (player == null)
            {
                return NotFound("Player not found.");
            }

            _gameService.AssignRoleToPlayer(gameId, request.PlayerId, request.Role);
            return Ok($"Role {request.Role} assigned to {player.Name}.");
        }

        // Bỏ phiếu
        [HttpPost("{gameId}/vote")]
        public IActionResult Vote(string gameId, [FromBody] string playerId)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            var player = game.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                return NotFound("Player not found.");
            }

            _gameService.VoteOutPlayer(gameId, playerId);
            return Ok($"Vote casted against {player.Name}.");
        }

        // Lấy thông tin trò chơi
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

        // GameController.cs

        [HttpPut("{gameId}/update-player/{playerId}")]
        public IActionResult UpdatePlayer(string gameId, string playerId, [FromBody] Player updatedPlayer)
        {
            var game = _gameService.GetGame(gameId);
            var player = game?.Players.FirstOrDefault(p => p.Id == playerId);
            if (game != null && player != null)
            {
                player.Name = updatedPlayer.Name;  // Cập nhật tên
                player.Role = updatedPlayer.Role;  // Cập nhật vai trò nếu cần
                return Ok("Player updated.");
            }
            return NotFound("Player not found.");
        }

    }

    public class ChooseRoleRequest
    {
        public string PlayerId { get; set; }
        public string Role { get; set; }
    }
}
