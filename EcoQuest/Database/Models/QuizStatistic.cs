namespace EcoQuest
{
    public partial class QuizStatistic
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Mode { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string? UserAnswers { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}