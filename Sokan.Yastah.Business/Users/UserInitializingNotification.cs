﻿namespace Sokan.Yastah.Business.Users
{
    public class UserInitializingNotification
    {
        public UserInitializingNotification(
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
