namespace LudexApp.Models.ViewModels
{
    public class ForumViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string GameTitle { get; set; } = string.Empty;

        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
