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

        public Post GetPost(int _id)
        {
            foreach (Post post in posts)
            {
                if (post.id  == _id)
                {
                    return post;
                }
            }
            return null;
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

        public Game GetGame(int _id)
        {
            return 
        }

        public void AddGame(/* insert game class */)
        {
            gameLibrary.Add(/* insert item */);
        }

        public void RemoveGame( int _id )
        {
            /* similar to get post but with the game class */
        }

        public /* review class */void GetReview(int _id)
        {
            /* similar to post */
        }

        public void AddReview(/* Insert Review class */)
        {
            gameReviews.Add(/* Insert Item */);
        }

        public void RemoveReview(int _id)
        {
            /* Similar to post */
        }

        public User GetFriend(int _id)
        {
            foreach (User user in friends)
            {
                if (user.id == _id)
                {
                    return user;
                }
            }
            return null;
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
