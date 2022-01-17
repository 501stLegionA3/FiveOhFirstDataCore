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
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Result;
using ProjectDataCore.Data.Structures.Assignable;
using ProjectDataCore.Data.Structures.Model.Assignable;

namespace ProjectDataCore.Pages.Admin.Account
{
    public partial class AssignableValueEditor
    {
#pragma warning disable CS8618 // Injections are not null.
        [Inject]
        public IAssignableDataService AssignableDataService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public List<BaseAssignableConfiguration> CurrentAssignables { get; set; } = new();

        public BaseAssignableConfiguration? ToEdit { get; set; } = null;
        public AssignableConfigurationValueEditModel ValueEditModel { get; set; } = new();

        public List<string> ItemList { get; set; } = new();
        public int MoveIndex { get; set; } = -1;

        public string NewAssignable { get; set; } = "";
        public int NewConfigurationType { get; set; } = -1;

        List<(Type, string)> ConfigurationTypes { get; set; } = new();

        public bool ConfirmDelete = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
                await RefreshAssignables();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var asbly = Assembly.GetAssembly(typeof(BaseAssignableConfiguration));

            if (asbly is not null)
            {
                var types = asbly.GetTypes()
                    .Where(x => x.GetCustomAttribute<AssignableConfigurationAttribute>() is not null);

                foreach(var t in types)
                {
                    var dets = t.GetCustomAttribute<AssignableConfigurationAttribute>();

                    ConfigurationTypes.Add((t, dets!.Name));
                }
            }
        }

        private async Task RefreshAssignables()
        {
            var res = await AssignableDataService.GetAllAssignableConfigurationsAsync();

            if(res.GetResult(out var data, out _))
            {
                CurrentAssignables = data;
            }

            ConfirmDelete = true;

            StateHasChanged();
        }

        private async Task OnCreateAssignableAsync()
		{
            if (NewConfigurationType > -1)
            {
                var config = Activator.CreateInstance(ConfigurationTypes[NewConfigurationType].Item1) as BaseAssignableConfiguration;

                if (config is not null)
                {
                    config.PropertyName = NewAssignable;

                    var res = await AssignableDataService.AddNewAssignableConfiguration(config);

                    if(!res.GetResult(out var err))
                    {
                        // TODO: handle errors.
                    }

                    await RefreshAssignables();
                }
            }
		}

        private void StartEdit(BaseAssignableConfiguration config)
        {
            ToEdit = config;
            ItemList.Clear();
            ValueEditModel = new();
            switch (ToEdit)
            {
                case IAssignableConfiguration<DateTime> c:
                    foreach(var item in c.AllowedValues)
                        ItemList.Add(item.ToShortDateString() + " " + item.ToShortTimeString());
                    break;
                case IAssignableConfiguration<DateOnly> c:
                    foreach (var item in c.AllowedValues)
                        ItemList.Add(item.ToShortDateString());
                    break;
                case IAssignableConfiguration<TimeOnly> c:
                    foreach (var item in c.AllowedValues)
                        ItemList.Add(item.ToString("hh:mm:ss"));
                    break;

                case IAssignableConfiguration<int> c:
                    foreach (var item in c.AllowedValues)
                        ItemList.Add(item.ToString());
                    break;
                case IAssignableConfiguration<double> c:
                    foreach (var item in c.AllowedValues)
                        ItemList.Add(item.ToString());
                    break;

                case IAssignableConfiguration<string> c:
                    foreach (var item in c.AllowedValues)
                        ItemList.Add(item);
                    break;
            }

            StateHasChanged();
        }

        private async Task SaveEditAsync()
		{
            ActionResult? res = null;
            switch (ToEdit)
            {
                case ValueBaseAssignableConfiguration<DateTime> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<DateTime>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;
                case ValueBaseAssignableConfiguration<DateOnly> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<DateOnly>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;
                case ValueBaseAssignableConfiguration<TimeOnly> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<TimeOnly>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;

                case ValueBaseAssignableConfiguration<int> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<int>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;
                case ValueBaseAssignableConfiguration<double> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<double>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;

                case ValueBaseAssignableConfiguration<string> c:
                    res = await AssignableDataService.UpdateAssignableConfiguration<string>(ToEdit.Key, x =>
                    {
                        x.AllowedValues = c.AllowedValues;
                        RunUpdate(x, ToEdit);
                    });
                    break;
            }

            if(res is not null)
            {
                if(!res.GetResult(out var err))
                {
                    // TODO handle errors
                }

                await DiscardEditAsync();
            }
		}

        private static void RunUpdate<T>(AssignableConfigurationEditModel<T> cfg, BaseAssignableConfiguration update)
        {
            cfg.AllowedInput = update.AllowedInput;
            cfg.AllowMultiple = update.AllowMultiple;
            cfg.PropertyName = update.PropertyName;
        }

        private async Task DiscardEditAsync()
		{
            ToEdit = null;
            await RefreshAssignables();
		}

        private void AddNewOption()
        {
            switch(ToEdit)
            {
                case IAssignableConfiguration<DateTime> c:
                    var dateTime = ValueEditModel.DateValue;
                    dateTime += ValueEditModel.TimeValue;
                    c.AddElement(dateTime);
                    ItemList.Add(dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString());
                    break;
                case IAssignableConfiguration<DateOnly> c:
                    c.AddElement(DateOnly.FromDateTime(ValueEditModel.DateValue));
                    ItemList.Add(ValueEditModel.DateValue.ToShortDateString());
                    break;
                case IAssignableConfiguration<TimeOnly> c:
                    c.AddElement(TimeOnly.FromTimeSpan(ValueEditModel.TimeValue));
                    ItemList.Add(ValueEditModel.TimeValue.ToString("hh:mm:ss"));
                    break;

                case IAssignableConfiguration<int> c:
                    c.AddElement(ValueEditModel.IntValue);
                    ItemList.Add(ValueEditModel.IntValue.ToString());
                    break;
                case IAssignableConfiguration<double> c:
                    c.AddElement(ValueEditModel.DoubleValue);
                    ItemList.Add(ValueEditModel.DoubleValue.ToString());
                    break;

                case IAssignableConfiguration<string> c:
                    c.AddElement(ValueEditModel.StringValue);
                    ItemList.Add(ValueEditModel.StringValue);
                    break;
            }

            StateHasChanged();
        }

        private void OnMoveItem(int newPos)
        {
            if (ToEdit is IAssignableConfiguration config)
            {
                config.MoveElement(MoveIndex, newPos);

                var item = ItemList[MoveIndex];
                ItemList.RemoveAt(MoveIndex);
                ItemList.Insert(newPos, item);

                MoveIndex = -1;
                StateHasChanged();
            }
        }

        private void OnDeleteItem(int index)
        {
            if(ToEdit is IAssignableConfiguration config)
            {
                config.RemoveElement(index);
                ItemList.RemoveAt(index);

                StateHasChanged();
            }
        }

        private async Task DeleteAsync()
        {
            if (ConfirmDelete)
                ConfirmDelete = false;
            else if (ToEdit is not null)
            {
                var res = await AssignableDataService.DeleteAssignableConfiguration(ToEdit.Key);

                if(!res.GetResult(out var err))
                {
                    // TOOD handle errors
                }
                else
                {
                    await DiscardEditAsync();
                }
            }
        }
    }
}