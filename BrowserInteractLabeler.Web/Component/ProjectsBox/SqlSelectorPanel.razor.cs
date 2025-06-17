using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.ProjectsBox;

public class SqlSelectorPanelModel : ComponentBase
{
    [Parameter] public string[] SqlDbNames { get; set; }
    [Parameter] public string CurrentSqlDbNames { get; set; }
    [Parameter] public string CurrentInformationSqlDb { get; set; }

    [Parameter] public SizeF RootWindowsSize { get; set; }
    [Parameter] public bool IsShowSpinnerLpadDB { get; set; }
    [Parameter] public EventCallback<string> ChoseActiveDataBaseAsync { get; set; }
    [Parameter] public EventCallback<string> ChoseExportDataBaseAsync { get; set; }

    internal string GetHeightPanel()
    {
        const double coef = 0.93d;
        return $"{RootWindowsSize.Height * coef}px";
    }

    internal (string index, string name)[] GetSqlOnlyName()
    {
        if (SqlDbNames is null || !SqlDbNames.Any())
            return new[] { (" ", "Not load database") };

        var arrNames = SqlDbNames.Select((path, index) =>
        {
            var name = Path.GetFileName(path);
            return (index.ToString(), name);
        }).ToArray();

        return arrNames;
    }

    private string FindFullPath(string nameFile)
    {
        if (SqlDbNames is null || !SqlDbNames.Any() || nameFile is null || !nameFile.Any())
            return String.Empty;

        var retPath = SqlDbNames.FirstOrDefault(path => Path.GetFileName(path) == nameFile);
        return retPath ?? String.Empty;
    }

    internal async Task ButtonClickLoadTaskAsync(string nameFile)
    {
        var path = FindFullPath(nameFile);
        await ChoseActiveDataBaseAsync.InvokeAsync(path);
    }

    internal async Task ButtonClickSaveTaskAsync(string nameFile, bool save)
    {
        if (save)
        {
            var path = FindFullPath(nameFile);
            await ChoseExportDataBaseAsync.InvokeAsync(path);
        }
    }

    internal bool CheckActivePanel(string nameFile)
    {
        if (CurrentSqlDbNames is null || !CurrentSqlDbNames.Any() || nameFile is null || !nameFile.Any())
            return false;

        var name = Path.GetFileName(CurrentSqlDbNames);

        return name == nameFile;
    }
    
    internal bool InvertValueToBool(bool value)
    {
        return !value;
    }
}