namespace EcoQuest
{
    public class CheckAnswerDTO
    {
        public long QuestionId { get; set; }
        public string Answer { get; set; } = null!;
        public long UserId { get; set; }
        public long Duration { get; set; }
    }
}
