namespace EcoQuest
{
    public partial class RelationProduct
    {
        public long RelationProductId { get; set; }
        public long FirstProduct { get; set; }
        public long SecondProduct { get; set; }

        public virtual Product Products { get; set; }
    }
}