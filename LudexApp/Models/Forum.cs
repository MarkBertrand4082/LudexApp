namespace LudexApp.Models
{
    public class Forum
    {
        public /*Insert game class*/void game { get; set; }

        public string name { get; set; }

        private List<Post> posts;

        public int id { get; set; }

        public Post getPost(int _id)
        {
            foreach(Post post in posts)
            {
                if (post.id == _id)
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
    }
}
