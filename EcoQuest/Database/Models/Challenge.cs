namespace EcoQuest
{
    public partial class Challenge
    {
        public long ChallengeId { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Questions { get; set; } = null!;
    }
}