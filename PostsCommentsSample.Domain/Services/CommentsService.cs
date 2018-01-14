using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Data.Repositories;

namespace PostsCommentsSample.Domain.Services
{
	public interface ICommentsService
	{
		Task<Comment> GetCommentById(int commentId);

		Task<List<Comment>> GetComments(CommentsFilter filter);

		Task CreateComment(Comment comment);

		Task UpdateComment(int commentId, Comment comment);

		Task DeleteComment(int commentId);
	}

	public class CommentsService : ICommentsService
	{
		private readonly ICommentsRepository _commentsRepository;

		public CommentsService(ICommentsRepository commentsRepository)
		{
			_commentsRepository = commentsRepository;
		}

		public Task<Comment> GetCommentById(int commentId)
		{
			return _commentsRepository.GetCommentById(commentId);
		}

		public Task<List<Comment>> GetComments(CommentsFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException(nameof(filter));

			return _commentsRepository.GetComments(filter);
		}

		public Task CreateComment(Comment comment)
		{
			return _commentsRepository.CreateComment(comment);
		}

		public Task UpdateComment(int commentId, Comment comment)
		{
			comment.CommentId = commentId;
			return _commentsRepository.UpdateComment(comment);
		}

		public Task DeleteComment(int commentId)
		{
			return _commentsRepository.DeleteComment(commentId);
		}
	}
}
