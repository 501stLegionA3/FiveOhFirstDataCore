using Microsoft.AspNetCore.Mvc;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ProjectDataCore.Api.Image;

[Route("api/image")]
[ApiController]
public class UploadController : ControllerBase
{
    [Route("upload")]
    public async Task<JsonResult> OnImageUploadPostAsync()
    {
        Dictionary<string, object> test = new() {
            {
                "error",
                new Dictionary<string, string>() {
                    { "message", "The image upload failed because this is not implemented." }
                }
            }
        };

        var res = new JsonResult(test);
        return res;
    }
}
