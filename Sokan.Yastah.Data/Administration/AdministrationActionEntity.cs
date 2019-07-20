using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActions", Schema = "Administration")]
    internal class AdministrationActionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(Type))]
        public int TypeId { get; set; }

        public AdministrationActionTypeEntity Type { get; set; }

        public DateTimeOffset Performed { get; set; }

        [ForeignKey(nameof(PerformedBy))]
        public ulong PerformedById { get; set; }

        public UserEntity PerformedBy { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.PerformedById)
                    .HasConversion<long>();
            });
    }
}
