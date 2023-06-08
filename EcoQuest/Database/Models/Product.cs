namespace EcoQuest
{
    public partial class Product
    {
        public Product()
        {
            GameBoardsProducts = new HashSet<GameBoardsProduct>();
            Questions = new HashSet<Question>();
            QuestionWeights = new HashSet<QuestionWeight>();
        }

        public long ProductId { get; set; }
        public string Colour { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Round { get; set; }
        public string? Logo { get; set; }

        public virtual ICollection<GameBoardsProduct> GameBoardsProducts { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuestionWeight> QuestionWeights { get; set; }
        public virtual RelationProduct RelationsProduct { get; set; }
    }
}