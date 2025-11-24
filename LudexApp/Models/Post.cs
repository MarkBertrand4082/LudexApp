namespace LudexApp.Models
{
    public class Post
    {
        public int id { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        public Forum forum { get; set; }

        public Post(string _title, string _content, Forum _forum)
        {
            this.title = _title;
            this.content = _content;
            this.forum = _forum;
        }
    }
}
