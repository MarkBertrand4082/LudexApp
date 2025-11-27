namespace LudexApp.Models.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public int ForumId { get; set; }

        public int UserId { get; set; }

        public bool CanDelete { get; set; }
    }
}
