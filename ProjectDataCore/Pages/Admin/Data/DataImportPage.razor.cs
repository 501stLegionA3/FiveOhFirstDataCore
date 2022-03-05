using Microsoft.AspNetCore.Components.Forms;
using ProjectDataCore.Components.Parts.Edit;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Import;
using ProjectDataCore.Data.Services.Logging;
using ProjectDataCore.Data.Structures.Util;
using ProjectDataCore.Data.Structures.Util.Import;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Data;
public partial class DataImportPage : IDisposable
{
#pragma warning disable CS8618 // Imports are never null.
    [Inject]
    public IAlertService AlertService { get; set; }
    [Inject]
    public IImportService ImportService { get; set; }
    [Inject]
    public IWebHostEnvironment WebHostEnvironment { get; set; }
    [Inject]
    public IAssignableDataService AssignableDataService { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public IInstanceLogger InstanceLogger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected enum ImportStage
    {
        FileSelect,
        FileOptions,
        ColumnConfiguration,
        Errored,
        Import,
        Done
    }

    protected ImportStage Stage { get; set; } = ImportStage.FileSelect;

    protected IBrowserFile? ImportFile { get; set; }
    protected DataImportConfiguration? ImportConfiguration { get; set; } = null;

    protected bool AllowMultipleValuesPerCol { get; set; } = true;
    protected DataImportBinding? ToEdit { get; set; } = null;
    protected int ToEditCol { get; set; } = -1;
    protected string ColumnTitle { get; set; } = "";

    private FileStream? ImportStream { get; set; }
    private string? FilePath { get; set; }

    private DataCoreUser? MockTrooper { get; set; }
    private Type[] AllowedStaticTypes { get; init; } = new Type[]
    {
        typeof(string),
        typeof(int),
        typeof(double),
        typeof(ulong),
        typeof(DateTime)
    };

    private CancellationTokenSource? CancellationSource { get; set; }
    public Guid LogScope { get; set; }

    private void LoadFile(InputFileChangeEventArgs e)
    {
        foreach (IBrowserFile file in e.GetMultipleFiles(1))
        {
            ImportFile = file;
        }
    }

    private void GoBack()
    {
        Stage--;

        CloseValueBinding();
    }

    protected async Task StartImportAsync()
    {
        if (ImportFile is not null)
        {
            Stage = ImportStage.FileOptions;

            // Get a temp name for the file ...
            var storageName = Path.GetRandomFileName();
            FilePath = Path.Combine(WebHostEnvironment.ContentRootPath,
                WebHostEnvironment.EnvironmentName, "unsafe_uploads", storageName);

            // ... then create a new local object to store ...
            ImportStream = new(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            // ... then read the file to the local object ...
            await using (var read = ImportFile.OpenReadStream())
                await read.CopyToAsync(ImportStream);

            // ... save and rest the stream ...
            await ImportStream.FlushAsync();
            ImportStream.Seek(0, SeekOrigin.Begin);


            // ... and create our import config.
            ImportConfiguration = new();

            // ... and get rid of the ImportFile
            ImportFile = null;

            // ... then get a mock dataset for the trooper object
            var res = await AssignableDataService.GetMockUserWithAssignablesAsync();

            if (res.GetResult(out var t, out var err))
            {
                MockTrooper = t;
            }
            else
            {
                Stage = ImportStage.FileSelect;
                AlertService.CreateErrorAlert(err);
            }
        }
        else
        {
            AlertService.CreateWarnAlert("No file was selected to import.", true);
        }
    }

    protected async Task RegisterFileOptions()
    {
        if(ImportConfiguration is not null)
        {
            if(!string.IsNullOrWhiteSpace(ImportConfiguration.StandardDelimiter))
            {
                if(!string.IsNullOrWhiteSpace(ImportConfiguration.MultipleValueDelimiter))
                {
                    AllowMultipleValuesPerCol = true;
                }
                else
                {
                    AllowMultipleValuesPerCol = false;
                    AlertService.CreateWarnAlert("No delimiter for multiple values was selected. The delimiter can not be a space. Multiple" +
                        "values per column will not allowed to be imported. Press the back button to change this option.", true);
                }

                if(ImportStream is null && FilePath is not null)
                {
                    // Create a new local object to store ...
                    ImportStream = new(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                }

                if (ImportStream is not null)
                {
                    var res = await ImportService.GetCSVUniqueValuesAsync(ImportStream, ImportConfiguration);

                    if(!res.GetResult(out var err))
                    {
                        AlertService.CreateErrorAlert(err);
                        Stage = ImportStage.FileSelect;
                    }
                    else
                    {
                        if (ImportConfiguration.DataRows.Count > 0)
                        {
                            Stage = ImportStage.ColumnConfiguration;
                        }
                        else
                        {
                            AlertService.CreateWarnAlert("No data rows were read from the provided file.", true);
                            Stage = ImportStage.FileSelect;
                        }
                    }

                    // No matter the result, dispose of the stream.
                    await ImportStream.DisposeAsync();
                    ImportStream = null;
                }
                else
                {
                    AlertService.CreateErrorAlert("No Import File stream has been created.");
                }
            }
            else
            {
                AlertService.CreateWarnAlert("No delimiter was selected. The delimiter can not be a space.", true);
            }
        }
        else
        {
            AlertService.CreateErrorAlert("No Import Configuration has been created.");
        }
    }

    protected async Task ImportDataAsync()
    {
        if (ImportConfiguration is not null)
        {
            if (!string.IsNullOrWhiteSpace(ImportConfiguration.StandardDelimiter))
            {
                CancellationSource = new();
                var token = CancellationSource.Token;
                LogScope = Guid.NewGuid();

                // Change the stage so we get logging information
                Stage = ImportStage.Import;
                StateHasChanged();

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5));

                        InstanceLogger.Log("Import starting in 5 seconds.", LogLevel.Information, LogScope);

                        // Give time for the logger to load.
                        await Task.Delay(TimeSpan.FromSeconds(5), token);

                        var res = await ImportService.BulkUpdateUsersAsync(ImportConfiguration, token, LogScope);

                        if (res.GetResult(out var err))
                        {
                            Stage = ImportStage.Done;
                            AlertService.CreateSuccessAlert("Data Import Completed.", true);
                            await InvokeAsync(StateHasChanged);
                        }
                        else
                        {
                            Stage = ImportStage.Errored;
                            AlertService.CreateErrorAlert(err);
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                    catch
                    {
                        InstanceLogger.Log("Import canclled before it began.", LogLevel.Critical, LogScope);
                        Stage = ImportStage.Errored;
                        await InvokeAsync(StateHasChanged);
                    }
                });
            }
            else
            {
                AlertService.CreateWarnAlert("No delimiter was selected. The delimiter can not be a space.", true);
            }
        }
        else
        {
            AlertService.CreateErrorAlert("No Import Configuration has been created.");
        }
    }

    #region Value Bindings
    protected void EditValueBinding(int col)
    {
        if(ImportConfiguration?.ValueBindings.TryGetValue(col, out var binding)
            ?? false)
        {
            ToEdit = binding;
            ColumnTitle = ImportConfiguration.HeaderValues[col];
            ToEditCol = col;

            StateHasChanged();
        }
        else
        {
            AddValueBinding(col);
        }
    }

    protected void AddValueBinding(int col)
    {
        var binding = new DataImportBinding();
        if(ImportConfiguration?.ValueBindings.TryAdd(col, binding) ?? false)
        {
            ToEdit = binding;
            ColumnTitle = ImportConfiguration.HeaderValues[col];
            ToEditCol = col;

            StateHasChanged();
        }
    }

    protected void CloseValueBinding()
    {
        ToEdit = null;
        ToEditCol = -1;

        StateHasChanged();
    }

    protected void DeleteValueBinding()
    {
        if(ToEdit is not null && ToEditCol >= 0)
        {
            _ = ImportConfiguration?.ValueBindings.TryRemove(ToEditCol, out _);

            ClearIdentifierTags();
            RemoveOldIdentifierColumn();
            CloseValueBinding();
        }
    }

    private void ClearIdentifierTags(DataImportBinding? edit = null)
    {
        edit ??= ToEdit;

        if (edit is not null)
        {
            edit.EmailIdentifier = false;
            edit.IsUserIdIdentifier = false;
            edit.IsUsernameIdentifier = false;
            edit.PasswordIdentifier = false;
        }
    }

    private void RemoveOldIdentifierColumn()
    {
        if (ImportConfiguration is not null)
        {
            if (ImportConfiguration.IdentifierColumn == ToEditCol)
                ImportConfiguration.IdentifierColumn = -1;

            if (ImportConfiguration.PasswordColumn == ToEditCol)
                ImportConfiguration.PasswordColumn = -1;

            if (ImportConfiguration.EmailColumn == ToEditCol)
                ImportConfiguration.EmailColumn = -1;
        }
    }

    protected void ToggleUserIdIdentifier()
    {
        if (ImportConfiguration is not null
            && ToEdit is not null)
        {
            if (!ToEdit.IsUserIdIdentifier)
            {
                RemoveOldIdentifierColumn();

                if(ImportConfiguration.IdentifierColumn != ToEditCol
                    && ImportConfiguration.IdentifierColumn >= 0)
                {
                    if(ImportConfiguration.ValueBindings
                        .TryGetValue(ImportConfiguration.IdentifierColumn, out var binding))
                        ClearIdentifierTags(binding);
                }

                ImportConfiguration.IdentifierColumn = ToEditCol;

                ClearIdentifierTags();
                ToEdit.IsUserIdIdentifier = true;
            }
            else
            {
                ToEdit.IsUserIdIdentifier = false;
                ImportConfiguration.IdentifierColumn = -1;
            }
        }
    }

    protected void ToggleUsernameIdentifier()
    {
        if (ImportConfiguration is not null
            && ToEdit is not null)
        {
            if (!ToEdit.IsUsernameIdentifier)
            {
                RemoveOldIdentifierColumn();

                if (ImportConfiguration.IdentifierColumn != ToEditCol
                    && ImportConfiguration.IdentifierColumn >= 0)
                {
                    if (ImportConfiguration.ValueBindings
                        .TryGetValue(ImportConfiguration.IdentifierColumn, out var binding))
                        ClearIdentifierTags(binding);
                }

                ImportConfiguration.IdentifierColumn = ToEditCol;

                ClearIdentifierTags();
                ToEdit.IsUsernameIdentifier = true;
            }
            else
            {
                ToEdit.IsUsernameIdentifier = false;
                ImportConfiguration.IdentifierColumn = -1;
            }
        }
    }

    protected void ToggleEmailIdentifier()
    {
        if (ImportConfiguration is not null
            && ToEdit is not null)
        {
            if (!ToEdit.EmailIdentifier)
            {
                RemoveOldIdentifierColumn();

                if (ImportConfiguration.EmailColumn != ToEditCol
                    && ImportConfiguration.EmailColumn >= 0)
                {
                    if (ImportConfiguration.ValueBindings
                        .TryGetValue(ImportConfiguration.EmailColumn, out var binding))
                        ClearIdentifierTags(binding);
                }

                ImportConfiguration.EmailColumn = ToEditCol;

                ClearIdentifierTags();
                ToEdit.EmailIdentifier = true;
            }
            else
            {
                ToEdit.EmailIdentifier = false;
                ImportConfiguration.EmailColumn = -1;
            }
        }
    }

    protected void TogglePasswordIdentifier()
    {
        if (ImportConfiguration is not null
            && ToEdit is not null)
        {
            if (!ToEdit.PasswordIdentifier)
            {
                RemoveOldIdentifierColumn();

                if (ImportConfiguration.PasswordColumn != ToEditCol
                    && ImportConfiguration.PasswordColumn >= 0)
                {
                    if (ImportConfiguration.ValueBindings
                        .TryGetValue(ImportConfiguration.PasswordColumn, out var binding))
                        ClearIdentifierTags(binding);
                }

                ImportConfiguration.PasswordColumn = ToEditCol;

                ClearIdentifierTags();
                ToEdit.PasswordIdentifier = true;
            }
            else
            {
                ToEdit.PasswordIdentifier = false;
                ImportConfiguration.PasswordColumn = -1;
            }
        }
    }
    #endregion

    #region Assignables
    protected void CreateSingleValueAssignable(string key)
    {
        if(ToEdit is not null && MockTrooper is not null)
        {
            var container = MockTrooper.GetAssignablePropertyContainer(ToEdit.PropertyName);

            if (container is not null)
            {
                // We never want to allow this for assignable setup.
                container.AssignableConfiguration.AllowMultiple = false;
                container = container.Clone();
                var editContainer = new EditBase()
                {
                    AssignableConfiguration = container.AssignableConfiguration,
                    AssignableValue = container,
                    
                };

                ToEdit.DataValueModels[key] = (editContainer, new());
                ToEdit.DataValues[key] = "";
            }
            else
            {
                AlertService.CreateErrorAlert("The container for the provided property name when creating an assignable config was not found.");
            }
        }
    }

    protected void DeleteSingleValueAssignable(string key)
    {
        if (ToEdit is not null)
        {
            _ = ToEdit.DataValues.TryRemove(key, out _);
            _ = ToEdit.DataValueModels.TryRemove(key, out _);
        }
    }

    protected void OnSingleValueAssignableUpdated(string key)
    {
        if (ToEdit is not null)
        {
            if (ToEdit.DataValueModels.TryGetValue(key, out var valuePair))
            {
                // property.ReplaceValue(item.Value);
                var edit = valuePair.Item1 as EditBase;
                var value = valuePair.Item2.ToList(x => x.Item2);

                if (value is not null && edit is not null)
                {
                    edit.AssignableValue!.ReplaceValue(value);
                    ToEdit.DataValues[key] = value.FirstOrDefault()?.ToString() ?? "";
                }
            }
        }
    }
    #endregion

    public void Dispose()
    {
        if (ImportStream is not null)
            ImportStream.Dispose();

        ImportStream = null;
        ImportFile = null;

        if(FilePath is not null)
        {
            try
            {
                File.Delete(FilePath);
            }
            catch { /* ignore */ }
        }

        MockTrooper = null;
        ImportConfiguration = null;
    }
}
