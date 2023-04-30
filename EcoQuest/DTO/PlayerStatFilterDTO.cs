namespace EcoQuest
{
    public class PlayerStatFilterDTO
    {
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public string Interval { get; set; } = null!;
        public string Mode { get; set; } = null!;
    }
}
