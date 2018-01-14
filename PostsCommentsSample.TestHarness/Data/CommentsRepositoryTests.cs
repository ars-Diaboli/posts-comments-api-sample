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
    public class CommentsRepositoryTests
    {
		public class MockCommentsRepository : CommentsRepository
		{
			public MockCommentsRepository()
			{
				_storage.Clear();
			}

			public List<Comment> Storage => _storage;
		}

		public class TestSetup
		{
			public MockCommentsRepository SetupRepository(IEnumerable<Comment> list = null)
			{
				var repository = new MockCommentsRepository();
				if (list != null)
					repository.Storage.AddRange(list);

				return repository;
			}
		}

		[TestFixture]
		public class GetCommentByIdTests
		{
			#region Test Cases
			public class GetCommentByIdTestCases
			{
				private static List<Comment> _data = new List<Comment>
				{
					new Comment
					{
						CommentId = 2,
						Text = "test1",
						CreationDate = DateTime.Now,
						OwnerName = "user1",
						PostId = 4
					},
					new Comment
					{
						CommentId = 4,
						Text = "test2",
						CreationDate = DateTime.Now,
						OwnerName = "user2",
						PostId = 4
					},
					new Comment
					{
						CommentId = 7,
						Text = "test3",
						CreationDate = DateTime.Now,
						OwnerName = "user3",
						PostId = 6
					},
					new Comment
					{
						CommentId = 11,
						Text = "test4",
						CreationDate = DateTime.Now,
						OwnerName = "user4",
						PostId = 8
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(4, _data.Find(c => c.CommentId == 4), _data);
						yield return new TestCaseData(8, (Comment)null, (List<Comment>)null);
						yield return new TestCaseData(10, (Comment)null, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(GetCommentByIdTestCases), nameof(GetCommentByIdTestCases.TestCases))]
			public void GetCommentById_ExistingItem_ReturnsComment(int commentId, Comment expected, IEnumerable<Comment> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				var result = repository.GetCommentById(commentId).Result;

				// Assert
				Assert.AreEqual(expected, result);
				Assert.IsTrue(result == null || result.CommentId == commentId);
			}
		}

		[TestFixture]
		public class GetCommentsTests
		{
			#region Test Cases
			public class GetCommentsTestCases
			{
				private static List<Comment> _data = new List<Comment>
				{
					new Comment
					{
						CommentId = 1,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Text = "I'm first commenter!",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 2,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Text = "comment",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 3,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Text = "comment",
						OwnerName = "User1",
					},
					new Comment
					{
						CommentId = 4,
						PostId = 3,
						CreationDate = DateTime.UtcNow.AddMinutes(-765),
						Text = "comment",
						OwnerName = "John Doe",
					},
					new Comment
					{
						CommentId = 5,
						PostId = 5,
						CreationDate = DateTime.UtcNow.AddMinutes(-654),
						Text = "comment",
						OwnerName = "User1",
					},
					new Comment
					{
						CommentId = 7,
						PostId = 6,
						CreationDate = DateTime.UtcNow.AddMinutes(-543),
						Text = "comment",
						OwnerName = "NewUser",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new CommentsFilter(), _data, _data);
						yield return new TestCaseData(new CommentsFilter(), new List<Comment>(), (List<Comment>)null);
						yield return new TestCaseData(
							new CommentsFilter { PageIndex = 1, PageSize = 2 },
							_data.OrderByDescending(i => i.CreationDate).Skip(1 * 2).Take(2), 
							_data);
						yield return new TestCaseData(
							new CommentsFilter { PostId = 3 },
							_data.OrderByDescending(i => i.CreationDate).Where(i => i.PostId == 3),
							_data);
						yield return new TestCaseData(
							new CommentsFilter { StartDate = DateTime.UtcNow.AddMinutes(-654), EndDate = DateTime.UtcNow.AddMinutes(-1212) },
							_data
								.OrderByDescending(i => i.CreationDate)
								.Where(i => i.CreationDate >= DateTime.UtcNow.AddMinutes(-654) && i.CreationDate < DateTime.UtcNow.AddMinutes(-1212)),
							_data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(GetCommentsTestCases), nameof(GetCommentsTestCases.TestCases))]
			public void GetComments_ExistingItems_ReturnsComments(CommentsFilter filter, IEnumerable<Comment> expected, IEnumerable<Comment> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				var result = repository.GetComments(filter).Result;

				// Assert
				CollectionAssert.AreEquivalent(expected, result);
			}
		}

		[TestFixture]
		public class CreateCommentTests
		{
			#region Test Cases
			public class CreateCommentTestCases
			{
				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Comment());
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(CreateCommentTestCases), nameof(CreateCommentTestCases.TestCases))]
			public void CreateComment_Success(Comment comment)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository();

				// Act
				repository.CreateComment(comment).Wait();

				// Assert
				CollectionAssert.IsNotEmpty(repository.Storage);
				Assert.AreNotEqual(default(int), comment.CommentId);
				Assert.AreNotEqual(default(DateTime), comment.CreationDate);
			}
		}

		[TestFixture]
		public class UpdateCommentTests
		{
			#region Test Cases
			public class UpdateCommentTestCases
			{
				private static List<Comment> _data = new List<Comment>
				{
					new Comment
					{
						CommentId = 1,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Text = "I'm first commenter!",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 2,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Text = "comment",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 3,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Text = "comment",
						OwnerName = "User1",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(new Comment { CommentId = 3, Text = "test1" }, _data);
						yield return new TestCaseData(new Comment { CommentId = 3, Text = "test1" }, new List<Comment>());
						yield return new TestCaseData(new Comment { CommentId = 66, Text = "test1" }, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(UpdateCommentTestCases), nameof(UpdateCommentTestCases.TestCases))]
			public void UpdateComment_Success(Comment comment, IEnumerable<Comment> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);
				var initialState = data.Select(i => i.Text);

				// Act
				repository.UpdateComment(comment).Wait();

				// Assert
				CollectionAssert.AreEqual(initialState, repository.Storage.Select(i => i.Text));
			}
		}

		[TestFixture]
		public class DeleteCommentTests
		{
			#region Test Cases
			public class DeleteCommentTestCases
			{
				private static List<Comment> _data = new List<Comment>
				{
					new Comment
					{
						CommentId = 1,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Text = "I'm first commenter!",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 2,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Text = "comment",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 3,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Text = "comment",
						OwnerName = "User1",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(3, _data);
						yield return new TestCaseData(3, new List<Comment>());
						yield return new TestCaseData(77, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(DeleteCommentTestCases), nameof(DeleteCommentTestCases.TestCases))]
			public void DeleteComment_Success(int commentId, IEnumerable<Comment> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				repository.DeleteComment(commentId).Wait();

				// Assert
				CollectionAssert.AreEquivalent(repository.Storage, repository.Storage.Where(c => c.CommentId != commentId));
			}
		}

		[TestFixture]
		public class DeletePostCommentsTests
		{
			#region Test Cases
			public class DeletePostCommentsTestCases
			{
				private static List<Comment> _data = new List<Comment>
				{
					new Comment
					{
						CommentId = 1,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1345),
						Text = "I'm first commenter!",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 2,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1111),
						Text = "comment",
						OwnerName = "NewUser",
					},
					new Comment
					{
						CommentId = 3,
						PostId = 1,
						CreationDate = DateTime.UtcNow.AddMinutes(-1212),
						Text = "comment",
						OwnerName = "User1",
					},
				};

				public static IEnumerable TestCases
				{
					get
					{
						yield return new TestCaseData(1, _data);
						yield return new TestCaseData(1, new List<Comment>());
						yield return new TestCaseData(77, _data);
					}
				}
			}
			#endregion Test Cases

			[Test]
			[TestCaseSource(typeof(DeletePostCommentsTestCases), nameof(DeletePostCommentsTestCases.TestCases))]
			public void DeletePostComments_Success(int postId, IEnumerable<Comment> data)
			{
				// Arrange
				var repository = new TestSetup().SetupRepository(list: data);

				// Act
				repository.DeletePostComments(postId).Wait();

				// Assert
				CollectionAssert.AreEquivalent(repository.Storage, repository.Storage.Where(c => c.PostId != postId));
			}
		}
	}
}
