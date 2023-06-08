namespace EcoQuest
{
    public partial class RelationQuestion
    {
        public long RelationQuestionId { get; set; }
        public long FirstQuestion { get; set; }
        public long SecondQuestion { get; set; }

        public virtual Question Questions { get; set; }
    }
}