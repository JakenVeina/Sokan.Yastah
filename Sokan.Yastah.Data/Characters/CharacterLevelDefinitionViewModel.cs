using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Characters
{
    public class CharacterLevelDefinitionViewModel
    {
        public CharacterLevelDefinitionViewModel(
            int level,
            int experienceThreshold)
        {
            Level = level;
            ExperienceThreshold = experienceThreshold;
        }

        public int Level { get; }

        public int ExperienceThreshold { get; }

        internal static readonly Expression<Func<CharacterLevelDefinitionVersionEntity, CharacterLevelDefinitionViewModel>> FromVersionEntityProjection
            = ve => new CharacterLevelDefinitionViewModel(
                ve.Level,
                ve.ExperienceThreshold);
    }
}
