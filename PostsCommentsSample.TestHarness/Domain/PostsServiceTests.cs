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
	public class PostsServiceTests
	{
		public class TestSetup
		{
			public Mock<IPostsRepository> SetupRepository(
				Post post = null,
				List<Post> list = null)
			{
				var mockRepository = new Mock<IPostsRepository>();
				mockRepository
					.Setup(i => i.GetPostById(It.IsAny<int>()))
					.ReturnsAsync(post);
				mockRepository
					.Setup(i => i.GetPosts(It.IsNotNull<PostsFilter>()))
					.ReturnsAsync(list);
				mockRepository
					.Setup(i => i.CreatePost(It.IsNotNull<Post>()))
					.Returns(Task.CompletedTask)
					.Verifiable();
				mockRepository
					.Setup(i => i.UpdatePost(It.IsNotNull<Post>()))
					.Returns(Task.CompletedTask)
					.Verifiable();
				mockRepository
					.Setup(i => i.DeletePost(It.IsAny<int>()))
					.Returns(Task.CompletedTask)
					.Verifiable();

				return mockRepository;
			}

			public IPostsService SetupService(
				Post post = null,
				List<Post> list = null,
				Mock<IPostsRepository> mockRepository = null)
			{
				var postsRepository = mockRepository ?? SetupRepository(post, list);
				var commentsRepository = new Mock<ICommentsRepository>();
				commentsRepository
					.Setup(i => i.DeletePostComments(It.IsAny<int>()))
					.Returns(Task.CompletedTask);

				var sevice = new PostsService(postsRepository.Object, commentsRepository.Object);

				return sevice;
			}
		}

		[TestFixture]
		public class GetPostByIdTests
		{
			public class GetPostByIdTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Post());
						yield return new TestCaseData((Post) null);
					}
				}
			}

			[Test]
			[TestCaseSource(typeof(GetPostByIdTestCases), nameof(GetPostByIdTestCases.TestCases))]
			public void GetPostById_ExistingItem_ReturnsPost(Post post)
			{
				// Arrange
				var sevice = new TestSetup().SetupService(post);

				// Act
				var result = sevice.GetPostById(1).Result;

				// Assert
				Assert.AreEqual(post, result);
			}
		}

		[TestFixture]
		public class GetPostsTests
		{
			public class GetPostsTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new List<Post>());
						yield return new TestCaseData(new List<Post>
						{
							new Post(),
							new Post { PostId = 7 }
						});
					}
				}
			}

			[Test]
			[TestCaseSource(typeof(GetPostsTestCases), nameof(GetPostsTestCases.TestCases))]
			public void GetPosts_ExistingItems_ReturnsPosts(List<Post> items)
			{
				// Arrange
				var service = new TestSetup().SetupService(list: items);

				// Act
				var result = service.GetPosts(new PostsFilter()).Result;

				// Assert
				CollectionAssert.AreEqual(items, result);
			}

			[Test]
			public void GetPosts_EmptyFilter_ThrowsException()
			{
				// Arrange
				var service = new TestSetup().SetupService();

				// Act
				TestDelegate testDelegate = () => { service.GetPosts(null).Wait(); };

				// Assert
				Assert.Throws<ArgumentNullException>(testDelegate);
			}
		}

		[TestFixture]
		public class CreatePostTests
		{
			[Test]
			public void CreatePost_CallRepository_Success()
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);

				// Act
				service.CreatePost(new Post()).Wait();

				// Assert
				repository.Verify(i => i.CreatePost(It.IsNotNull<Post>()));
			}
		}

		[TestFixture]
		public class UpdatePostTests
		{
			[Test]
			[TestCase(7)]
			public void UpdatePost_CallRepository_Success(int postId)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);
				var Post = new Post();

				// Act
				service.UpdatePost(postId, Post).Wait();

				// Assert
				Assert.AreEqual(postId, Post.PostId);
				repository.Verify(i => i.UpdatePost(Post));
			}
		}

		[TestFixture]
		public class DeletePostTests
		{
			[Test]
			[TestCase(7)]
			public void DeletePost_CallRepository_Success(int postId)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();
				var service = new TestSetup().SetupService(mockRepository: repository);

				// Act
				service.DeletePost(postId).Wait();

				// Assert
				repository.Verify(i => i.DeletePost(postId));
			}
		}
	}
}
