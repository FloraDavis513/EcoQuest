namespace EcoQuest
{
    public class QuestionQuizDTO
    {
        public QuestionQuizDTO()
        {
            Answers = new List<QuestionAnswersDTO>();
        }

        public long QuestionId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string Type { get; set; } = null!;
        public long Round { get; set; }
        public ICollection<QuestionAnswersDTO> Answers { get; set; }
    }
}
