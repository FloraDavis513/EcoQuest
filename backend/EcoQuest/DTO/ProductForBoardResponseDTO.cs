namespace EcoQuest
{
    public class ProductForBoardResponseDTO
    {
        public ProductForBoardResponseDTO()
        {
            Questions = new HashSet<Question>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Colour { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public int? NumOfRepeating { get; set; }
    }
}