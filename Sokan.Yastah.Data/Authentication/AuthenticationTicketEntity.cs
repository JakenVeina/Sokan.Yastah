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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public ulong UserId { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; set; }

        public AdministrationActionEntity Creation { get; set; }

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AdministrationActionEntity Deletion { get; set; }

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
