using System;

namespace PostsCommentsSample.Data.Models
{
	public class Post
	{
		public int PostId { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? LastUpdateDate { get; set; }

		public string OwnerName { get; set; }
	}
}
