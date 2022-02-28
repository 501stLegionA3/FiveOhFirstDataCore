using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using ProjectDataCore.Data.Structures.Util;

using System.Net;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ProjectDataCore.Api.Image;

[Route("api/image")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public UploadController(IConfiguration configuration, IWebHostEnvironment env)
        => (_configuration, _env) = (configuration, env);

    [Route("upload")]
    [HttpPost]
    public async Task<JsonResult> OnImageUploadPostAsync()
    {
        var imageFolder = _configuration["Startup:ImageFolder"];
        if(string.IsNullOrWhiteSpace(imageFolder))
        {
            return Error("No image folder configured for this application. Contact your site administartor to setup an image folder.");
        }
        else
        {
            // Ensure directories
            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, imageFolder));
        }

        try
        {
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), Request.ContentLength ?? 10000);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section is not null)
            {
                var hasContentDispositionHeader =
                ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        var untrustedFile = contentDisposition.FileName.Value;

                        var name = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
                        name += Path.GetExtension(untrustedFile);
                        name = WebUtility.HtmlEncode(name);

                        var trusted = $"{Request.Scheme}://{Request.Host}/{imageFolder}/{name}";

                        await SaveFileAsync(imageFolder, name, section.Body);

                        return Success(trusted);
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return Error("No file was uploaded");
        }
        catch (Exception ex)
        {
            return Error($"The image upload failed: {ex.Message}");
        }
    }

    private static JsonResult Success(string path)
    {
        Dictionary<string, string> data = new() { { "url", path } };
        return new JsonResult(data);
    }

    private static JsonResult Error(string errorMessage)
    {
        Dictionary<string, object> test = new()
        {
            {
                "error",
                new Dictionary<string, string>() {
                    { "message", errorMessage }
                }
            }
        };

        var res = new JsonResult(test);
        return res;
    }

    private async Task SaveFileAsync(string path, string name, Stream file)
    {
        var fullPath = Path.Combine(_env.WebRootPath, path, name);
        await using FileStream fs = new(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        await file.CopyToAsync(fs);
    }
}
