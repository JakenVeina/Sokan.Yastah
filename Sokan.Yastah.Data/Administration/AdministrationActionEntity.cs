using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActions", Schema = "Administration")]
    internal class AdministrationActionEntity
    {
        public AdministrationActionEntity(
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

        public AdministrationActionTypeEntity Type { get; internal set; }
            = null!;

        public DateTimeOffset Performed { get; }

        [ForeignKey(nameof(PerformedBy))]
        public ulong? PerformedById { get; }

        public UserEntity? PerformedBy { get; internal set; }
    }

    internal class AdministrationActionEntityTypeConfiguration
        : IEntityTypeConfiguration<AdministrationActionEntity>
    {
        public void Configure(
            EntityTypeBuilder<AdministrationActionEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Performed);

            entityBuilder
                .Property(x => x.PerformedById)
                .HasConversion<long?>();
        }
    }
}
