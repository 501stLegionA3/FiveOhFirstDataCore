using FiveOhFirstDataCore.Data.Account;
using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages;
public partial class Index
{
    [CascadingParameter]
    public Trooper? CurrentUser { get; set; }
}
