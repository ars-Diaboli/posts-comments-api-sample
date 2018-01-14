using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Data.Repositories;
using PostsCommentsSample.Domain.Services;

namespace PostsCommentsSample.TestHarness.Domain
{
	public class CommentsServiceTests
	{
		public class TestSetup
		{
			public Mock<ICommentsRepository> SetupRepository(
				Comment comment = null,
				List<Comment> list = null)
			{
				var mockRepository = new Mock<ICommentsRepository>();
				mockRepository
					.Setup(i => i.GetCommentById(It.IsAny<int>()))
					.ReturnsAsync(comment);
				mockRepository
					.Setup(i => i.GetComments(It.IsNotNull<CommentsFilter>()))
					.ReturnsAsync(list);
				mockRepository
					.Setup(i => i.CreateComment(It.IsNotNull<Comment>()))
					.Verifiable();
				mockRepository
					.Setup(i => i.UpdateComment(It.IsNotNull<Comment>()))
					.Verifiable();
				mockRepository
					.Setup(i => i.DeleteComment(It.IsAny<int>()))
					.Verifiable();

				return mockRepository;
			}

			public ICommentsService SetupService(
				Comment comment = null,
				List<Comment> list = null,
				Mock<ICommentsRepository> mockRepository = null)
			{
				var repository = mockRepository ?? SetupRepository(comment, list);
				var sevice = new CommentsService(repository.Object);

				return sevice;
			}
		}

		[TestFixture]
		public class GetCommentByIdTests
		{
			public class GetCommentByIdTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Comment());
						yield return new TestCaseData((Comment) null);
					}
				}
			}

			[Test]
			[TestCaseSource(typeof(GetCommentByIdTestCases), nameof(GetCommentByIdTestCases.TestCases))]
			public void GetCommentById_ExistingItem_ReturnsComment(Comment comment)
			{
				// Arrange
				var sevice = new TestSetup().SetupService(comment);

				// Act
				var result = sevice.GetCommentById(1).Result;

				// Assert
				Assert.AreEqual(comment, result);
			}
		}
	}
}
