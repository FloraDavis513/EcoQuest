namespace EcoQuest
{
    public partial class QuestionWeight
    {
        public long QuestionWeightId { get; set; }
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public long QuestionId { get; set; }
        public long Weight { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}