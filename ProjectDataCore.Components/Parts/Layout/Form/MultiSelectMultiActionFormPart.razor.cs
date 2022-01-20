using ProjectDataCore.Data.Structures.Model.User;

namespace ProjectDataCore.Components.Parts.Layout.Form;

[LayoutComponent(Name = "Multi-select Multi-action Form", Form = true)]
public partial class MultiSelectMultiActionFormPart : FormLayoutBase
{
    public override async Task OnSubmitAsync()
    {
        for(int i = 0; i < SelectedUsers.Count; i++)
        {
            if(OnSubmitListeners.TryGetValue(i, out var listener))
            {
                var model = new DataCoreUserEditModel();

                List<Task> runners = new();
                foreach (var l in listener)
                    runners.Add(l.Invoke(model));

                await Task.WhenAll(runners);

                var res = await UserService.UpdateUserAsync(SelectedUsers[i].Id, x =>
                {
                    x.StaticValues = model.StaticValues;
                    x.AssignableValues = model.AssignableValues;
                });

                if(!res.GetResult(out var err))
                {
                    // TODO display errors.
                    return;
                }
            }
        }
    }
}