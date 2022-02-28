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
    [Route("upload")]
    [HttpPost]
    public async Task<JsonResult> OnImageUploadPostAsync()
    {
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

                        var trusted = WebUtility.HtmlEncode(name);


                    }
                }
            }


        }
        catch (Exception ex)
        {
            Dictionary<string, object> test = new()
            {
                {
                    "error",
                    new Dictionary<string, string>() {
                    { "message", $"The image upload failed: {ex.Message}" }
                }
                }
            };

            var res = new JsonResult(test);
            return res;
        }
    }
}
