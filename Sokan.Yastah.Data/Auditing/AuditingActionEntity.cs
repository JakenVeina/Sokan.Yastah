using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Auditing
{
    [Table("AuditableActions", Schema = "Auditing")]
    internal class AuditableActionEntity
    {
        public AuditableActionEntity(
            long id,
            int typeId,
            DateTimeOffset performed,
            ulong? performedById)
        {
            Id = id;
            TypeId = typeId;
            Performed = performed;
            PerformedById = performedById;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Type))]
        public int TypeId { get; }

        public AuditableActionTypeEntity Type { get; internal set; }
            = null!;

        public DateTimeOffset Performed { get; }

        [ForeignKey(nameof(PerformedBy))]
        public ulong? PerformedById { get; }

        public UserEntity? PerformedBy { get; internal set; }
    }

    internal class AdministrationActionEntityTypeConfiguration
        : IEntityTypeConfiguration<AuditableActionEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Performed);

            entityBuilder
                .Property(x => x.PerformedById)
                .HasConversion<long?>();
        }
    }
}
