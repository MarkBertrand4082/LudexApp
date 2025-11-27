namespace LudexApp.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PostViewModel> Posts { get; set; } = new();
        public List<ReviewItemViewModel> Reviews { get; set; } = new();
        public List<GameViewModel> Games { get; set; } = new();
        public List<FriendViewModel> Friends { get; set; } = new();
    }
