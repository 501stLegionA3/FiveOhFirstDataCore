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

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();


        }
    }
}