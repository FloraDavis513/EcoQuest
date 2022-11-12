namespace EcoQuest
{
    public class ProductFoBoardRequestDTO
    {
        public ProductFoBoardRequestDTO()
        {
            QuestionIds = new HashSet<long>();
        }

        public long ProductId { get; set; }
        public int? NumberOfRepeating { get; set; }
        public virtual ICollection<long> QuestionIds { get; set; }
    }
}