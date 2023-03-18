namespace EcoQuest
{
    public class GameBoardDTO
    {
        public GameBoardDTO()
        {
            Products = new HashSet<ProductDTO>();
        }

        public long GameBoardId { get; set; }
        public string Name { get; set; } = null!;
        public int NumFields { get; set; }
        public long UserId { get; set; }

        public virtual ICollection<ProductDTO> Products { get; set; }

        public class ProductDTO
        {
            public ProductDTO()
            {
                AllQuestions = new HashSet<Question>();
                ActiveQuestions = new HashSet<long>();
            }

            public long GameBoardId { get; set; }
            public long ProductId { get; set; }
            public string Colour { get; set; } = null!;
            public string Name { get; set; } = null!;
            public int Round { get; set; }
            public string? Logo { get; set; }
            public int NumOfRepeating { get; set; }

            public virtual ICollection<Question> AllQuestions { get; set; }
            public virtual ICollection<long> ActiveQuestions { get; set; }
        }
    }
}