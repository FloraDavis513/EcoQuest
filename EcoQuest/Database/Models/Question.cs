﻿namespace EcoQuest
{
    public partial class Question
    {
        public Question()
        {
            GameBoards = new HashSet<GameBoard>();
            QuestionWeights = new HashSet<QuestionWeight>();
        }

        public long QuestionId { get; set; }
        public string? Answers { get; set; }
        public string? Type { get; set; }
        public string? ShortText { get; set; }
        public string? Text { get; set; }
        public long ProductId { get; set; }
        public string? Media { get; set; }
        public string LastEditDate { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
        public virtual RelationQuestion RelationQuestion { get; set; } = null!;

        public virtual ICollection<GameBoard> GameBoards { get; set; }
        public virtual ICollection<QuestionWeight> QuestionWeights { get; set; }
    }
}