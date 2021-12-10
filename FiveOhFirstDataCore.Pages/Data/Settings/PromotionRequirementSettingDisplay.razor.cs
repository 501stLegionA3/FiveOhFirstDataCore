using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Promotions;

namespace FiveOhFirstDataCore.Components.Data.Settings
{
    public partial class PromotionRequirementSettingDisplay
    {
        public SortedDictionary<int, PromotionDetails> Bindings { get; set; } = new();
        public SortedList<int, Enum> Roles { get; set; } = new();
        public int Counter { get; set; } = 0;
        public bool Loaded { get; set; } = false;
        public int AddNewSegment { get; set; } = -1;
        public int ToDelete { get; set; } = -1;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Bindings = new(await _settings.GetSavedPromotionDetails());
                Loaded = true;
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Roles = new();
            List<Enum> roles = new();
            foreach (var i in Enum.GetValues<TrooperRank>())
                roles.Add(i);
            foreach (var i in Enum.GetValues<RTORank>())
                roles.Add(i);
            foreach (var i in Enum.GetValues<MedicRank>())
                roles.Add(i);
            foreach (var i in Enum.GetValues<PilotRank>())
                roles.Add(i);
            foreach (var i in Enum.GetValues<WarrantRank>())
                roles.Add(i);
            foreach (var i in Enum.GetValues<WardenRank>())
                roles.Add(i);
            try
            {
                foreach (var i in roles)
                    Roles.Add(Convert.ToInt32(i), i);
            }
            catch
            {
            // Whoops. Something shouldn't have broken.
            }
        }

        public async Task OnRevert()
        {
            Bindings = new(await _settings.GetSavedPromotionDetails());
            StateHasChanged();
        }

        public async Task OnSave()
        {
            await _settings.OverridePromotionRequirementsAsync(new(Bindings));
            await OnRevert();
        }

        public void AddSegement()
        {
            if (AddNewSegment > -1)
            {
                Bindings.Add(AddNewSegment, new()
                {RequirementsFor = AddNewSegment});
            }

            AddNewSegment = -1;
        }

        public void DeleteSegement()
        {
            if (ToDelete > -1)
            {
                Bindings.Remove(ToDelete);
            }

            ToDelete = -1;
        }
    }
}