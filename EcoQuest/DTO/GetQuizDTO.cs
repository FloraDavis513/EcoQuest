namespace EcoQuest
{
    public class GetQuizDTO
    {
        public GetQuizDTO()
        {
            SelectedProduct = new List<long>();
        }
        public long UserId { get; set; }
        public ICollection<long> SelectedProduct { get; set; }
    }
}
