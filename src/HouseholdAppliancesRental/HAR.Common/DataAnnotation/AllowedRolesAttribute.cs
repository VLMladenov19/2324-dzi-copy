using HAR.Common.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace HAR.Common.DataAnnotation
{
    /// <summary>
    /// Custom made data annotation to ensure roles are from the UserRole class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedRolesAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Allow null or empty values, as it's handled by [Required] attribute
                return true;
            }

            return typeof(UserRole).GetFields()
                .Select(f => f.GetValue(null)?.ToString()?.ToLower())
                .Contains(value.ToString()?.ToLower());
        }
    }
}
