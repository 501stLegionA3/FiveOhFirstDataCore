namespace ProjectDataCore.Components.Roster;
public partial class RosterDisplay
{
	private class RosterDisplayOptions
	{
		public Guid SelectedRoster { get; set; }
		public List<RosterDisplaySettings> AvalibleRosters { get; set; } = new();
		/// <summary>
		/// IF true the roster will only show a list of members rather than
		/// an ordered collection of elements for this roster.
		/// </summary>
		/// <remarks>
		/// When this option is true further details can be displayed about a single
		/// user. Furthermore, this allows further sorting and search options to
		/// be conducted on the roster iteself.
		/// </remarks>
		public bool ShowUserLisiting { get; set; } = false;

		// TODO: Other display settings.
	}

	private RosterTree? Roster { get; set; } = null;
	private RosterDisplayOptions Options { get; set; } = new();

	[Inject]
	public IModularRosterService RosterService { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			// Load the roster with the defualt display.
			await ReloadRoster();
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		// TODO: Use cascading values to pull settings from
		// the database for this component. Different pages
		// will have a different subset of rosters that they
		// are allowed to display.
	}

	private async Task ReloadRoster()
	{
		// TODO: Read from selected roster options on this component
		// so we pull the correct Guid.

		// Reset the roster display.
		Roster = null;
		StateHasChanged();
		try
		{
			// Start getting roster data.
			var rosterParts = RosterService.GetRosterTreeForSettingsAsync(Options.SelectedRoster);

			Stack<RosterTree> parents = new();
			await foreach (var x in rosterParts)
			{
				if (Roster is null)
				{
					// The first value will always be the parent.
					Roster = x;
				}
				else
				{
					// Otherwise, check the top of the stack ...
					var last = parents.Peek();
					// ... while the current value does not match the parent
					// value ...
					while (!x.RosterParentLinks.Any(x
						=> x.ForRosterSettingsId == Options.SelectedRoster
						&& x.ParentRosterId == last.Key))
					{
						// ... pop the stack and peek the next value ...
						_ = parents.Pop();
						last = parents.Peek();
					}
					// ... until the value is found and the roster added ...
					last.ChildRosters.Add(x);
					// ... and refresh to include the latest roster part ...
					StateHasChanged();
				}
				// ... then push the curerent value to the stack.
				parents.Push(x);
			}
		}
		catch (ArgumentException)
		{
			// Roster GUID failed to be a settings value,
			// display an error message/defualt message.
		}
	}
}
