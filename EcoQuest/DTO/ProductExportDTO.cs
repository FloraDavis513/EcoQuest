namespace EcoQuest
{
    public class ProductExportDTO
    {
        public ProductExportDTO()
        {
            ProductIds = new List<long>();
        }

        public ICollection<long> ProductIds { get; set; }
        public string? FileName { get; set; }
    }
}