using System.Collections.Generic;
using System.Linq;
using Platform.Client.Common.Context;

namespace SywApplicationWireframe.Domain.Users
{
	public interface IUsersApi
	{
		UserDto Current();
		IList<UserDto> Get(IList<long> userIds);
		IList<UserDto> GetFollowing(long userId);
	}

	public class UsersApi : ApiBase, IUsersApi
	{
		protected override string BasePath { get { return "users"; } }

		public UsersApi(IContextProvider contextProvider) : base(contextProvider)
		{
		}

		public UserDto Current()
		{
			return Call<UserDto>("current");
		}

		public IList<UserDto> Get(IList<long> userIds)
		{
			return Call<IList<UserDto>>("get", new {Ids = userIds});
		}

		public IList<UserDto> GetFollowing(long userId)
		{
			var ids = Call<IList<long>>("followed-by", new {UserId = userId});
			return !ids.Any() ? 
				new UserDto[0] : 
				Get(ids);
		}
	}
}
