namespace WerewolfGame.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        // Constructor khởi tạo giá trị mặc định
        public Player()
        {
            Id = Guid.NewGuid().ToString();  // Khởi tạo Id bằng Guid mới
            Name = string.Empty;  // Khởi tạo Name mặc định là chuỗi rỗng
            Role = "Villager";  // Vai trò mặc định là Villager
        }
    }
}
