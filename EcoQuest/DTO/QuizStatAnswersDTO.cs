namespace EcoQuest
{
    public class QuizStatAnswersDTO
    {
        public long QuestionId { get; set; }
        public long ProductId { get; set; }
        public long Duration { get; set; }
        public long UsedHelps { get; set; }
        public long IsCorrect { get; set; }
    }
}
