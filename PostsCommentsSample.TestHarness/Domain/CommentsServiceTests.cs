using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
					.Returns(Task.CompletedTask)
					.Verifiable();
				mockRepository
					.Setup(i => i.UpdateComment(It.IsNotNull<Comment>()))
					.Returns(Task.CompletedTask)
					.Verifiable();
				mockRepository
					.Setup(i => i.DeleteComment(It.IsAny<int>()))
					.Returns(Task.CompletedTask)
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

		[TestFixture]
		public class GetCommentsTests
		{
			public class GetCommentsTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new List<Comment>());
						yield return new TestCaseData(new List<Comment>
						{
							new Comment(),
							new Comment { CommentId = 7 }
						});
					}
				}
			}

			[Test]
			[TestCaseSource(typeof(GetCommentsTestCases), nameof(GetCommentsTestCases.TestCases))]
			public void GetComments_ExistingItems_ReturnsComments(List<Comment> items)
			{
				// Arrange
				var service = new TestSetup().SetupService(list: items);

				// Act
				var result = service.GetComments(new CommentsFilter()).Result;

				// Assert
				CollectionAssert.AreEqual(items, result);
			}

			[Test]
			public void GetComments_EmptyFilter_ThrowsException()
			{
				// Arrange
				var service = new TestSetup().SetupService();

				// Act
				TestDelegate testDelegate = () => { service.GetComments(null).Wait(); };

				// Assert
				Assert.Throws<ArgumentNullException>(testDelegate);
			}
		}

		[TestFixture]
		public class CreateCommentTests
		{
			[Test]
			public void CreateComment_CallRepository_Success()
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);

				// Act
				service.CreateComment(new Comment()).Wait();

				// Assert
				repository.Verify(i => i.CreateComment(It.IsNotNull<Comment>()));
			}
		}

		[TestFixture]
		public class UpdateCommentTests
		{
			[Test]
			[TestCase(7)]
			public void UpdateComment_CallRepository_Success(int commentId)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);
				var comment = new Comment();

				// Act
				service.UpdateComment(commentId, comment).Wait();

				// Assert
				Assert.AreEqual(commentId, comment.CommentId);
				repository.Verify(i => i.UpdateComment(comment));
			}
		}

		[TestFixture]
		public class DeleteCommentTests
		{
			[Test]
			[TestCase(7)]
			public void DeleteComment_CallRepository_Success(int commentId)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);

				// Act
				service.DeleteComment(commentId).Wait();

				// Assert
				repository.Verify(i => i.DeleteComment(commentId));
			}
		}
	}
}
