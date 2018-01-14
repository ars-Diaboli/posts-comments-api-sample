using System;

namespace PostsCommentsSample.Data.Models
{
	public class Comment
	{
		public int CommentId { get; set; }

		public int PostId { get; set; }

		public string Text { get; set; }

		public DateTime CreationDate { get; set; }

		public string OwnerName { get; set; }
	}
}
