using ProjectDataCore.Data.Structures.Model.User;

namespace ProjectDataCore.Data.Structures.Page;

public interface ISubmittable : IDisposable
{
    public Task OnSubmitAsync(DataCoreUserEditModel model);
}
