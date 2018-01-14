using NUnit.Framework;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Data.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PostsCommentsSample.TestHarness.Data
{
    public class PostsRepositoryTests
    {
		public class MockPostsRepository : PostsRepository
		{
			public MockPostsRepository()
			{
				_storage.Clear();
			}

			public List<Post> Storage => _storage;
		}

		public class TestSetup
		{
			public MockPostsRepository SetupRepository(IEnumerable<Post> list = null)
			{
				var repository = new MockPostsRepository();
				if (list != null)
					repository.Storage.AddRange(list);

				return repository;
			}
		}

		[TestFixture]
		public class GetPostByIdTests
		{
			#region Test Cases
			public class GetPostByIdTestCases
			{
				private static List<Post> _data = new List<Post>
				{
					new Post
					{
						PostId = 2,
						Title = "test1",
						CreationDate = DateTime.Now,
						OwnerName = "user1",
						Content = "testContent1"
					},
					new Post
					{
						PostId = 4,
						Title = "test2",
						CreationDate = DateTime.Now,
						OwnerName = "user2",
						Content = "testContent2"
					},
					new Post
					{
						PostId = 7,
						Title = "test3",
						CreationDate = DateTime.Now,
						OwnerName = "user3",
						Content = "testContent3"
					},
					new Post
					{
						PostId = 11,
						Title = "test4",
						CreationDate = DateTime.Now,
						OwnerName = "user4",
						Content = "testContent4"
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(4, _data.Find(c => c.PostId == 4), _data);
						yield return new TestCaseData(8, (Post)null, (List<Post>)null);
						yield return new TestCaseData(10, (Post)null, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(GetPostByIdTestCases), nameof(GetPostByIdTestCases.TestCases))]
			public void GetPostById_ExistingItem_ReturnsPost(int postId, Post expected, IEnumerable<Post> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				var result = repository.GetPostById(postId).Result;

				// Assert
				Assert.AreEqual(expected, result);
				Assert.IsTrue(result == null || result.PostId == postId);
			}
		}

		[TestFixture]
		public class GetPostsTests
		{
			#region Test Cases
			public class GetPostsTestCases
			{
				private static List<Post> _data = new List<Post>
				{
					new Post
					{
						PostId = 1,
						Title = "test1",
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Content = "I'm first Poster!",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 2,
						Title = "test2",
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Content = "Post",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 3,
						Title = "test3",
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Content = "Post",
						OwnerName = "User1",
					},
					new Post
					{
						PostId = 4,
						Title = "test4",
						CreationDate = DateTime.UtcNow.AddMinutes(-765),
						Content = "Post",
						OwnerName = "John Doe",
					},
					new Post
					{
						PostId = 5,
						Title = "test5",
						CreationDate = DateTime.UtcNow.AddMinutes(-654),
						Content = "Post",
						OwnerName = "User1",
					},
					new Post
					{
						PostId = 7,
						Title = "test6",
						CreationDate = DateTime.UtcNow.AddMinutes(-543),
						Content = "Post",
						OwnerName = "NewUser",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new PostsFilter(), _data, _data);
						yield return new TestCaseData(new PostsFilter(), new List<Post>(), (List<Post>)null);
						yield return new TestCaseData(
							new PostsFilter { PageIndex = 1, PageSize = 2 },
							_data.OrderByDescending(i => i.CreationDate).Skip(1 * 2).Take(2), 
							_data);
						yield return new TestCaseData(
							new PostsFilter { StartDate = DateTime.UtcNow.AddMinutes(-654), EndDate = DateTime.UtcNow.AddMinutes(-1212) },
							_data
								.OrderByDescending(i => i.CreationDate)
								.Where(i => i.CreationDate >= DateTime.UtcNow.AddMinutes(-654) && i.CreationDate < DateTime.UtcNow.AddMinutes(-1212)),
							_data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(GetPostsTestCases), nameof(GetPostsTestCases.TestCases))]
			public void GetPosts_ExistingItems_ReturnsPosts(PostsFilter filter, IEnumerable<Post> expected, IEnumerable<Post> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				var result = repository.GetPosts(filter).Result;

				// Assert
				CollectionAssert.AreEquivalent(expected, result);
			}
		}

		[TestFixture]
		public class CreatePostTests
		{
			#region Test Cases
			public class CreatePostTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Post());
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(CreatePostTestCases), nameof(CreatePostTestCases.TestCases))]
			public void CreatePost_Success(Post post)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();

				// Act
				repository.CreatePost(post).Wait();

				// Assert
				CollectionAssert.IsNotEmpty(repository.Storage);
				Assert.AreNotEqual(default(int), post.PostId);
				Assert.AreNotEqual(default(DateTime), post.CreationDate);
			}
		}

		[TestFixture]
		public class UpdatePostTests
		{
			#region Test Cases
			public class UpdatePostTestCases
			{
				private static List<Post> _data = new List<Post>
				{
					new Post
					{
						PostId = 1,
						Title = "test1",
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Content = "I'm first Poster!",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 2,
						Title = "test2",
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Content = "Post",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 3,
						Title = "test3",
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Content = "Post",
						OwnerName = "User1",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Post { PostId = 3, Title = "test1" }, _data);
						yield return new TestCaseData(new Post { PostId = 3, Title = "test1" }, new List<Post>());
						yield return new TestCaseData(new Post { PostId = 66, Title = "test1" }, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(UpdatePostTestCases), nameof(UpdatePostTestCases.TestCases))]
			public void UpdatePost_Success(Post post, IEnumerable<Post> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);
				var initialState = data.Select(i => i.Title);

				// Act
				repository.UpdatePost(post).Wait();

				// Assert
				CollectionAssert.AreEqual(initialState, repository.Storage.Select(i => i.Title));
				Assert.AreNotEqual(default(DateTime), post.LastUpdateDate);
			}
		}

		[TestFixture]
		public class DeletePostTests
		{
			#region Test Cases
			public class DeletePostTestCases
			{
				private static List<Post> _data = new List<Post>
				{
					new Post
					{
						PostId = 1,
						Title = "test1",
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Content = "I'm first Poster!",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 2,
						Title = "test2",
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Content = "Post",
						OwnerName = "NewUser",
					},
					new Post
					{
						PostId = 3,
						Title = "test3",
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Content = "Post",
						OwnerName = "User1",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(3, _data);
						yield return new TestCaseData(3, new List<Post>());
						yield return new TestCaseData(77, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(DeletePostTestCases), nameof(DeletePostTestCases.TestCases))]
			public void DeletePost_Success(int postId, IEnumerable<Post> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				repository.DeletePost(postId).Wait();

				// Assert
				CollectionAssert.AreEquivalent(repository.Storage, repository.Storage.Where(c => c.PostId != postId));
			}
		}
	}
}
