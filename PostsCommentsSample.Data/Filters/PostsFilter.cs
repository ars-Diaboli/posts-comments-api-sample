using System;
using System.Collections.Generic;

namespace PostsCommentsSample.Data.Filters
{
	public class PostsFilter
	{
		public int PageSize { get; set; } = 25;

		public int PageIndex { get; set; } = 0;

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }
	}
}
