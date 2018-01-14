using System;
using System.Collections.Generic;

namespace PostsCommentsSample.Data.Filters
{
	public class CommentsFilter
	{
		public int PageSize { get; set; } = 25;

		public int PageIndex { get; set; } = 0;

		public int? PostId { get; set; }

		public List<int> CommentIds { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string OwnerName { get; set; }
	}
}
