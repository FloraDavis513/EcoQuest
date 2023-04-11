namespace EcoQuest
{
    public partial class Quiz
    {
        public long QuizId { get; set; }
        public long UserId { get; set; }
        public long Duration { get; set; }
        public string Helps { get; set; } = null!;
        public string Questions { get; set; } = null!;
        public long CurrentQuestion { get; set; }
        public long CorrectAnswers { get; set; }

        public virtual User User { get; set; } = null!;
    }
}