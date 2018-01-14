using System;
using System.ComponentModel.DataAnnotations;

namespace PostsCommentsSample.Web.ViewModels
{
    public class CommentViewModel
    {
	    public int CommentId { get; set; }

	    [Required]
		public int PostId { get; set; }

	    [Required]
	    [StringLength(1000)]
		public string Text { get; set; }

	    public DateTime CreationDate { get; set; }

	    [Required]
		public string OwnerName { get; set; }
    }
}
