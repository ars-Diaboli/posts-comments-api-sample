using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Domain.Services;
using PostsCommentsSample.Web.Framework;
using PostsCommentsSample.Web.ViewModels;

namespace PostsCommentsSample.Web.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
	    private readonly ICommentsService _commentsService;

	    public CommentsController(ICommentsService commentsService)
	    {
		    _commentsService = commentsService;
	    }

		// GET api/comments/1
		[HttpGet("{id}")]
        public async Task<CommentViewModel> Get(int id)
		{
			var comment = await _commentsService.GetCommentById(id);
			return comment != null ? map(comment) : null;
		}

		// GET api/comments
		[HttpGet]
        public async Task<IEnumerable<CommentViewModel>> Get(CommentsFilterModel filter)
        {
			var comments = await _commentsService.GetComments(map(filter));
			return comments.Select(map);
        }

		// POST api/comments
		[HttpPost]
		[ValidateModel]
        public Task Post([FromBody]CommentViewModel comment)
        {
	        return _commentsService.CreateComment(map(comment));
        }

		// PUT api/comments/5
		[HttpPut("{id}")]
		[ValidateModel]
		public Task Put(int id, [FromBody]CommentViewModel comment)
		{
			return _commentsService.UpdateComment(id, map(comment));
        }

		// DELETE api/comments/5
		[HttpDelete("{id}")]
        public Task Delete(int id)
        {
	        return _commentsService.DeleteComment(id);
        }

	    private static CommentsFilter map(CommentsFilterModel source)
	    {
		    var destination = new CommentsFilter();

		    destination.PageSize = source.PageSize;
		    destination.PageIndex = source.PageNumber - 1;
		    destination.PostId = source.PostId;
		    destination.CommentIds = source.CommentIds;
		    destination.StartDate = source.StartDate;
		    destination.EndDate = source.EndDate;
		    destination.OwnerName = source.OwnerName;

			return destination;
	    }

	    private static Comment map(CommentViewModel source)
	    {
		    var destination = new Comment();

		    destination.CommentId = source.CommentId;
		    destination.PostId = source.PostId;
		    destination.Text = source.Text;
		    destination.CreationDate = source.CreationDate;
		    destination.OwnerName = source.OwnerName;

			return destination;
	    }

	    private static CommentViewModel map(Comment source)
	    {
		    var destination = new CommentViewModel();

		    destination.CommentId = source.CommentId;
		    destination.PostId = source.PostId;
		    destination.Text = source.Text;
		    destination.CreationDate = source.CreationDate;
		    destination.OwnerName = source.OwnerName;

			return destination;
	    }
	}
}
