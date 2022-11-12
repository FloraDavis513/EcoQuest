namespace EcoQuest
{
    public partial class Session
    {
        public Guid Uuid { get; set; }
        public string? Username { get; set; }
        public string? State { get; set; }
        public long? IdCurrentQuestion { get; set; }
    }
}