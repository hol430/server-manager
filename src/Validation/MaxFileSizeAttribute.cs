using System.ComponentModel.DataAnnotations;

namespace ServerManager.Validation;

/// <summary>
/// A custom validation attribute which may be attached to an
/// <see cref="IFormFile"/> property. Ensures that the file specified
/// by the property doesn't exceed a maximum file size.
/// </summary>
public class MaxFileSizeAttribute : ValidationAttribute
{
    /// <summary>
    /// Maximum allowed size of the file.
    /// </summary>
    private uint maxFileSize;

    /// <summary>
    /// Create a new <see cref="MaxFileSizeAttribute"/> instance.
    /// </summary>
    /// <param name="maxSize">Maximum allowed size of the file.</param>
    public MaxFileSizeAttribute(uint maxSize)
    {
        maxFileSize = maxSize;
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file && file.Length > maxFileSize)
            return new ValidationResult($"{file.FileName} exceeds maximum allowed file size of {maxFileSize}.");
        return ValidationResult.Success;
    }
}
