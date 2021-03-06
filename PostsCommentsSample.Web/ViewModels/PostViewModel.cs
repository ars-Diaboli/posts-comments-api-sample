﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PostsCommentsSample.Web.ViewModels
{
    public class PostViewModel
    {
	    public int PostId { get; set; }

	    [Required]
	    [StringLength(255)]
	    public string Title { get; set; }

	    [Required]
	    public string Content { get; set; }

	    public DateTime CreationDate { get; set; }

	    public DateTime? LastUpdateDate { get; set; }

	    [Required]
	    public string OwnerName { get; set; }
	}
}
