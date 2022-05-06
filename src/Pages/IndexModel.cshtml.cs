using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerManager.Controllers;
using ServerManager.Extensions;

namespace ServerManager.Pages;

public class IndexModel : PageModel
{
    /// <summary>
    /// Logging service.
    /// </summary>
    private readonly ILogger<IndexModel> logger;

    /// <summary>
    /// The controller.
    /// </summary>
    private readonly ServerController controller;

    /// <summary>
    /// The raw inputs provided by the user.
    /// </summary>
    [BindProperty]
    public Inputs UserInputs { get; set; } = new Inputs();

    /// <summary>
    /// Initialises a new <see cref="IndexModel"/> instance.
    /// </summary>
    /// <param name="logger">Logging service.</param>
    /// <param name="controller">The controller.</param>
    public IndexModel(ILogger<IndexModel> logger, ServerController controller)
    {
        this.logger = logger;
        this.controller = controller;
    }

    /// <summary>
    /// POST callback method.
    /// </summary>
    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        return await HandleRequestAsync(SubmitAsync);
    }

    /// <summary>
    /// Handler for submit button click.
    /// </summary>
    public async Task SubmitAsync()
    {
        if (!ModelState.IsValid)
            throw new InvalidOperationException($"Invalid model state: {ModelState}");

        // Cast to non-nullable should be safe because the property has a
        // [Required] attribute, which will cause invalid ModelState if null.
        await controller.UploadFile(UserInputs.UploadedFile!);
    }

    /// <summary>
    /// Handle a request to the page. Return a HTTP 200 result if the request is
    /// handled successfully, or return a HTTP 500 result upon error.
    /// </summary>
    /// <param name="callback">The request handler.</param>
    private async Task<ActionResult> HandleRequestAsync(Func<Task> callback)
    {
        try
        {
            await callback();
            return RedirectToPage("Success");
        }
        catch (Exception error)
        {
            return HandleError(error);
        }
    }

    /// <summary>
    /// Handle an error by logging the exception details and returning
    /// a HTTP 500 internal server error result.
    /// </summary>
    /// <param name="error">The caught exception.</param>
    private ActionResult HandleError(Exception error)
    {
        logger.LogError(error, error.ToString());
        return StatusCode(StatusCodes.Status500InternalServerError, error.GetAggregateMessage());
    }
}