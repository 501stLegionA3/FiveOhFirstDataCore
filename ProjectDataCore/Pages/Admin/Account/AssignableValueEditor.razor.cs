using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using ProjectDataCore;
using ProjectDataCore.Shared;
using ProjectDataCore.Components.Roster;
using ProjectDataCore.Components.Parts;

namespace ProjectDataCore.Pages.Admin.Account
{
    public partial class AssignableValueEditor
    {
#pragma warning disable CS8618 // Injections are not null.
        [Inject]
        public IAssignableDataService AssignableDataService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    }
}