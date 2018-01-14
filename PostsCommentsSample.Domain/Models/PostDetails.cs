using System;
using System.Collections.Generic;
using PostsCommentsSample.Data.Models;

namespace PostsCommentsSample.Domain.Models
{
    public class PostDetails
    {
	    public int PostId { get; set; }

	    public string Title { get; set; }

	    public string Content { get; set; }

	    public DateTime CreationDate { get; set; }

	    public DateTime? LastUpdateDate { get; set; }

	    public string OwnerName { get; set; }

		public List<Comment> Comments { get; set; }
	}
}
