using Microsoft.Extensions.Options;

namespace BuildingBlocks.API.Configuration.Extensions;

public static class OptionsExtensions
{
    public static T GetValidatedOptions<T>(this IOptionsSnapshot<T> options) where T : class
    {
        return options.Value;
    }

    public static T GetValidatedOptions<T>(this IOptions<T> options) where T : class
    {
        return options.Value;
    }

    public static bool IsConfigured<T>(this IOptions<T> options) where T : class
    {
        try
        {
            var value = options.Value;
            return value != null;
        }
        catch (OptionsValidationException)
        {
            return false;
        }
    }
}