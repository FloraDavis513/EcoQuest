namespace EcoQuest
{
    public partial class User
    {
        public long UserId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}