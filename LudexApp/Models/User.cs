using IGDB.Models;

namespace LudexApp.Models
{
    public class User
    {
        public User()
        {
            Friends = new List<User>();
            GameLibrary = new List<Game>();
            GameReviews = new List<Review>();
            Posts = new List<Post>();
        }

        public int Id { get; set; }

        // Must be properties so EF can map them
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Stored hashed password
        public string PasswordHash { get; set; } = string.Empty;

        // These should be public so EF can map them
        public List<User> Friends { get; set; }
        public List<Game> GameLibrary { get; set; }
        public List<Review> GameReviews { get; set; }
        public List<Post> Posts { get; set; }

        // ---- Helper Methods ----

        public void AddPost(Post post) => Posts.Add(post);

        public void RemovePost(int _id)
        {
            var post = Posts.FirstOrDefault(p => p.id == _id);
            if (post != null)
                Posts.Remove(post);
        }

        public void AddGame(Game game) => GameLibrary.Add(game);

        public void RemoveGame(int id)
        {
            var game = GameLibrary.FirstOrDefault(g => g.Id == id);
            if (game != null)
                GameLibrary.Remove(game);
        }

        public void AddReview(Review review) => GameReviews.Add(review);

        public void RemoveReview(int id)
        {
            //var review = GameReviews.FirstOrDefault(r => r.Id == id);
            //if (review != null)
            //    GameReviews.Remove(review);
        }

        public void AddFriend(User user) => Friends.Add(user);

        public void RemoveFriend(int id)
        {
            var friend = Friends.FirstOrDefault(f => f.Id == id);
            if (friend != null)
                Friends.Remove(friend);
        }
    }
}