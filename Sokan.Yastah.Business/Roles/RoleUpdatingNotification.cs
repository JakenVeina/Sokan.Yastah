namespace Sokan.Yastah.Business.Roles
{
    public class RoleUpdatingNotification
    {
        public RoleUpdatingNotification(
            long userId,
            long actionId)
        {
            RoleId = userId;
            ActionId = actionId;
        }

        public long RoleId { get; }

        public long ActionId { get; }
    }
}
