﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterGuilds", Schema = "Characters")]
    internal class CharacterGuildEntity
    {
        public CharacterGuildEntity(
            long id)
        {
            Id = id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterGuildEntity>();
    }
}
