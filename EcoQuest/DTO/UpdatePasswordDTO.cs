namespace EcoQuest
{
    public class UpdatePasswordDTO
    {
        public string Login { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}