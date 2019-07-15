namespace System.ComponentModel.DataAnnotations
{
    public abstract class AutoValidatableObject
        : IValidatable
    {
        public void Validate(IServiceProvider serviceProvider)
            => Validator.ValidateObject(this, new ValidationContext(this, serviceProvider, null), validateAllProperties: true);
    }
}
