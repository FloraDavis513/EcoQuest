﻿namespace EcoQuest
{
    public class GetQuizDTO
    {
        public GetQuizDTO()
        {
            SelectedProduct = new List<long>();
        }
        public long UserId { get; set; }
        public string Mode { get; set; } = null!;
        public string Password { get; set; }
        public ICollection<long> SelectedProduct { get; set; }
    }
}
