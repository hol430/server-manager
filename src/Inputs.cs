using System.ComponentModel.DataAnnotations;
using ServerManager.Utility;
using ServerManager.Validation;

namespace ServerManager;

/// <summary>
/// User inputs from main page.
/// </summary>
public class Inputs
{
    /// <summary>
    /// The uploaded .apsimx file.
    /// </summary>
    [Required]
    [Display(Name = ".apsimx file")]
    [MaxFileSize(30 * Units.Megabyte)]
    public IFormFile? UploadedFile { get; set; }
}
