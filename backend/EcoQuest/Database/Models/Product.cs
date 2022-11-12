namespace EcoQuest
{
    public partial class Product
    {
        public Product()
        {
            Questions = new HashSet<Question>();
        }

        public long Id { get; set; }
        public string? Colour { get; set; }
        public string? Name { get; set; }
        public int? Round { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}