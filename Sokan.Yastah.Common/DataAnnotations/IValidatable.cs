namespace System.ComponentModel.DataAnnotations
{
    public interface IValidatable
    {
        void Validate(IServiceProvider serviceProvider);
    }
}
