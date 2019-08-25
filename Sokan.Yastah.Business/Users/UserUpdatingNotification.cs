namespace Sokan.Yastah.Business.Users
{
    public class UserUpdatingNotification
    {
        public UserUpdatingNotification(
            ulong userId,
            long actionId)
        {
            UserId = userId;
            ActionId = actionId;
        }

        public ulong UserId { get; }

        public long ActionId { get; }
    }
}
