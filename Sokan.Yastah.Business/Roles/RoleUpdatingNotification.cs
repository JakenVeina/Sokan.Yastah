namespace Sokan.Yastah.Business.Roles
{
    public class RoleUpdatingNotification
    {
        public RoleUpdatingNotification(
            long roleId,
            long actionId)
        {
            RoleId = roleId;
            ActionId = actionId;
        }

        public long RoleId { get; }

        public long ActionId { get; }
    }
}
