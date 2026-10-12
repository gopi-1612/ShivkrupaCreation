using System.Text;
using System.Text.RegularExpressions;

namespace ShivkrupaCreation.API.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            var str = phrase.ToLowerInvariant().Trim();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty); // remove invalid chars
            str = Regex.Replace(str, @"\s+", "-").Trim('-');          // spaces -> hyphens
            str = Regex.Replace(str, @"-+", "-");                     // collapse multiple hyphens

            return string.IsNullOrWhiteSpace(str) ? Guid.NewGuid().ToString("N")[..8] : str;
        }
    }
}
