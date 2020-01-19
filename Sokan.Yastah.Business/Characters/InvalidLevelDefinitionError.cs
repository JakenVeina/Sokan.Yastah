using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Characters
{
    public class InvalidLevelDefinitionError
        : OperationError
    {
        public InvalidLevelDefinitionError(
                int level,
                decimal experienceThreshold,
                decimal previousExperienceThreshold)
            : base(BuildMessage(
                level,
                experienceThreshold,
                previousExperienceThreshold))
        {
            Level = level;
            ExperienceThreshold = experienceThreshold;
            PreviousExperienceThreshold = previousExperienceThreshold;
        }

        public int Level { get; }

        public decimal ExperienceThreshold { get; }

        public decimal PreviousExperienceThreshold { get; }

        private static string BuildMessage(
                int level,
                decimal experienceThreshold,
                decimal previousExperienceThreshold)
            => $"Invalid experience threshold value ({experienceThreshold}) for level {level}: Must be greater than previous level ({previousExperienceThreshold})";
    }
}
