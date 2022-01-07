using ProjectDataCore.Data.Services.Roster;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Selectors;
public partial class UserPropertySelector
{
#pragma warning disable CS8618 // Injects can not be null.
    [Inject]
    public IAssignableDataService AssignableDataService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public Action<string> SetPropertyName { get; set; }

    [Parameter]
    public Action<bool> SetPropertyType { get; set; }

    [Parameter]
    public DataCoreUser User { get; set; } 

    [Parameter]
    public bool UseStaticProperties { get; set; }
    [Parameter]
    public string? PropertyName { get; set; }
    [Parameter]
    public Type[] AllowedStaticTypes { get; set; } = Array.Empty<Type>();


    public List<(string, string)> Properties { get; set; } = new();

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

        if(UseStaticProperties)
		{
            ReloadStaticProperties();
		}
        else
		{
            await ReloadAssignableProperties();
		}
	}

    protected void ReloadStaticProperties()
	{
        Properties.Clear();
        
        var typ = typeof(DataCoreUser);

        var propertyList = typ.GetProperties();

        foreach(var property in propertyList)
		{
            if (AllowedStaticTypes.Contains(property.PropertyType))
            {
                var disp = property.GetCustomAttributes(false)
                    .FirstOrDefault(x => x is DescriptionAttribute)
                    as DescriptionAttribute;

                Properties.Add((property.Name, disp?.Description ?? property.Name));
            }
		}
	}

    protected async Task ReloadAssignableProperties()
	{
        Properties.Clear();

        var propList = await AssignableDataService.GetAllAssignableConfigurationsAsync();

        if(propList.GetResult(out var data, out var err))
        {
            foreach (var prop in data)
                Properties.Add((prop.NormalizedPropertyName, prop.PropertyName));
        }
        else
        {
            // TODO handle errors
        }
	}

	protected void OnNameChanged(string e)
	{
        if(SetPropertyName is not null)
		{
            SetPropertyName.Invoke(e);
            PropertyName = e;
		}
	}

    protected async Task OnStaticSelectorChanged(bool e)
	{
        if (SetPropertyType is not null)
        {
            SetPropertyType.Invoke(e);
            UseStaticProperties = e;

            if(UseStaticProperties)
			{
                ReloadStaticProperties();
			}
            else
			{
                await ReloadAssignableProperties();
			}
        }
	}
}
