namespace LudexApp.Models
{
    public class Forum
    {
        public /*Insert game class*/void game { get; set; }

        public string name { get; set; }

        public List<Post> posts;

        public int id { get; set; }

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
