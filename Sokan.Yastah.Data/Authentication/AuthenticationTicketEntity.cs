using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Authentication
{
    [Table("AuthenticationTickets", Schema = "Authentication")]
    internal class AuthenticationTicketEntity
    {
        public AuthenticationTicketEntity(
            long id,
            ulong userId,
            long creationId,
            long? deletionId)
        {
            Id = id;
            UserId = userId;
            CreationId = creationId;
            DeletionId = deletionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(User))]
        public ulong UserId { get; }

        public UserEntity User { get; internal set; }
            = null!;

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AdministrationActionEntity? Deletion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AuthenticationTicketEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.UserId)
                    .HasConversion<long>();
            });
    }
}
