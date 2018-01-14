using System;
using System.Collections.Generic;

namespace PostsCommentsSample.Web.ViewModels
{
    public class CommentsFilterModel
    {
	    public int PageSize { get; set; } = 25;

	    public int PageNumber { get; set; } = 1;

	    public int? PostId { get; set; }

	    public DateTime? StartDate { get; set; }

	    public DateTime? EndDate { get; set; }
    }
}
