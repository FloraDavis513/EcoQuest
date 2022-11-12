namespace EcoQuest
{
    public class GameBoardDTO<T>
    {
        public GameBoardDTO()
        {
            Products = new HashSet<T>();
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<T> Products { get; set; }
        public int? NumFields { get; set; }
    }
}