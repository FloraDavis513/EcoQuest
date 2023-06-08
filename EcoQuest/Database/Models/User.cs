namespace EcoQuest
{
    public partial class User
    {
        public User()
        {
            GameBoards = new HashSet<GameBoard>();
            Games = new HashSet<Game>();
            QuizStatistics = new HashSet<QuizStatistic>();
            QuestionWeights = new HashSet<QuestionWeight>();
        }

        public long UserId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;

        public virtual ICollection<GameBoard> GameBoards { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<QuizStatistic> QuizStatistics { get; set; }
        public virtual ICollection<QuestionWeight> QuestionWeights { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
    }
}