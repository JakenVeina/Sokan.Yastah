﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public ICollection<CharacterGuildDivisionEntity> Divisions { get; internal set; }
            = null!;

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterGuildEntity>();
    }
}