using ServerManager.Controllers;

namespace ServerManager.Pages;

/// <summary>
/// Model for uploading .met files. Need to refactor this out and rethink how to
/// handle .met files.
/// </summary>
public class UploadMetModel : IndexModel
{
    /// <summary>
    /// Initialises a new <see cref="UploadMetModel"/> instance.
    /// </summary>
    /// <param name="logger">Deforestation service.</param>
    /// <param name="controller">Controller object.</param>
    public UploadMetModel(ILogger<IndexModel> logger, ServerController controller) : base(logger, controller)
    {
    }

    public override async Task SubmitAsync()
    {
        if (!ModelState.IsValid)
            throw new InvalidOperationException($"Model state is invalid");

        // UploadedFile cannot be null, as model state is valid.
        await controller.UploadMetFileAsync(UserInputs.UploadedFile!);
    }
}
