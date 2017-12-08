namespace CoreUserService.Services.Interfaces
{
    /// <summary>
    /// IBase36GeneratorService interface
    /// Contract for random base 36 string generation functionalities
    /// </summary>
    public interface IBase36GeneratorService
    {
        /// <summary>
        /// Method implementation will generate a base 36 string
        /// </summary>
        string Generate();
    }
}
