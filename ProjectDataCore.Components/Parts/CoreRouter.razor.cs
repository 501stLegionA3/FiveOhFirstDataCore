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

        /// <summary>
        /// The settings for the page to display
        /// </summary>
        private CustomPageSettings? PageSettings { get; set; }
        /// <summary>
        /// The type of base layout component to display.
        /// </summary>
        private Type? ComponentType { get; set; }
        /// <summary>
        /// The parameters for the component.
        /// </summary>
        private Dictionary<string, object> ComponentParams { get; } = new()
        {
            { "ComponentData", null }
        };
        /// <summary>
        /// The types of attributes that can be added to a mian page.
        /// </summary>
        private Type[] AttributeTypes { get; } = new Type[] { typeof(LayoutComponentAttribute) };

        private Func<Task> RefreshRequest { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            RefreshRequest = ReloadPage;

            // If the route is null or has been changed ...
            if (Route is not null 
                && (PageSettings?.Route ?? null) != Route)
            {
                // ... Get the new route settings ...
                PageSettings = await RoutingService.GetPageSettingsFromRouteAsync(Route);
                if (PageSettings is null)
                {
                    // TODO: Redirect to 404 not found page.
                }
                else
                {
                    // ... and start the page reloader.
                    _ = Task.Run(async () => await ReloadPage());
                }
            }
        }

        private async Task ReloadPage()
        {
            // ... if there are no setting, return.
            if (PageSettings is null)
                return;

            bool first = true;
            // ... otherwise, for each setp in the load method ...
            await foreach(bool _ in RoutingService.LoadPageSettingsAsync(PageSettings))
            {
                if (first && PageSettings.Layout is not null)
                {
                    // ... if its the first load, set the layout parameter ...
                    ComponentParams["ComponentData"] = PageSettings.Layout;
                    // ... and the component type ...
                    ComponentType = RoutingService.GetComponentType(PageSettings.Layout.QualifiedTypeName);
                    first = false;
                }
                // ... refresh the page ...
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}