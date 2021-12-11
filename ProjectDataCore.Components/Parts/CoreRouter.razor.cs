using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using ProjectDataCore.Data.Services.Roster;
using ProjectDataCore.Data.Structures;
using ProjectDataCore.Data.Structures.Assignable;
using ProjectDataCore.Data.Structures.Model;
using ProjectDataCore.Data.Structures.Result;
using ProjectDataCore.Data.Structures.Util;
using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Page;
using System.Reflection;
using ProjectDataCore.Data.Structures.Page.Attributes;

namespace ProjectDataCore.Components.Parts
{
    public partial class CoreRouter
    {
#pragma warning disable CS8618 // Injections are non-nullable.
        [Inject]
        public IRoutingService RoutingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Determines if this instance of the router is for editing
        /// pages or is a live page.
        /// </summary>
        [CascadingParameter(Name = "PageEdit")]
        public bool Editing { get; set; } = false;

        /// <summary>
        /// The route to display.
        /// </summary>
        [Parameter]
        public string? Route { get; set; }

        private CustomPageSettings? PageSettings { get; set; }
        private Type? ComponentType { get; set; }
        private Dictionary<string, object> ComponentParams { get; } = new()
        {
            { "ComponentData", null }
        };
        private Type[] AttributeTypes { get; } = new Type[] { typeof(LayoutComponentAttribute) };

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (Route is not null 
                && (PageSettings?.Route ?? null) != Route)
            {
                PageSettings = await RoutingService.GetPageSettingsFromRouteAsync(Route);
                if (PageSettings is null)
                {
                    // TODO: Redirect to 404 not found page.
                }
                else
                {
                    _ = Task.Run(async () => await ReloadPage());
                }
            }
        }

        private async Task ReloadPage()
        {
            if (PageSettings is null)
                return;

            bool first = true;
            await foreach(bool _ in RoutingService.LoadPageSettingsAsync(PageSettings))
            {
                if (first && PageSettings.Layout is not null)
                {
                    ComponentParams["ComponentData"] = PageSettings.Layout;
                    ComponentType = RoutingService.GetComponentType(PageSettings.Layout.QualifiedTypeName);
                    first = false;
                }

                await InvokeAsync(StateHasChanged);
            }
        }
    }
}