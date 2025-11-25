using IGDB.Models;

namespace LudexApp.Models
{
    public class User
    {
        private List<User> friends;
        public User() { }

        private List<Game> gameLibrary;

        private List<Review> gameReviews;

        private List<Post> posts;

        private string password;

        public int id { get; set; }

        public List<Post> GetPosts()
        {
            return posts;
        }

        public List<Review> GetReviews()
        {
            return gameReviews;
        }

        public List<User> GetFriends()
        {
            return friends;
        }

        public List<Game> GetGames()
        {
            return gameLibrary;
        }


        public void AddPost(Post post)
        {
            posts.Add(post);
        }

        public void RemovePost(int _id)
        {
            foreach (Post post in posts)
            {
                if (post.id == _id)
                {
                    posts.Remove(post);
                    break;
                }
            }
        }


        public void AddGame(Game game)
        {
            gameLibrary.Add(game);
        }

        public void RemoveGame( int _id )
        {
            /* similar to get post but with the game class */
        }

        public /* review class */void GetReview(int _id)
        {
            /* similar to post */
        }

        public void AddReview(Review review)
        {
            gameReviews.Add(review);
        }

        public void RemoveReview(int _id)
        {
            /* Similar to post */
        }


        public void AddFriend(User user)
        {
            friends.Add(user);
        }
        public void RemoveFriend(int _id)
        {
            foreach (User user in friends)
            {
                if (user.id == _id)
                {
                    friends.Remove(user);
                    break;
                }
            }
        }

    }
}
