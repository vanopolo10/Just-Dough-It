using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class LocalizationSettingsExtension
{
    public static async Task<string> FindStringInAllTablesAsync(string key)
    {
        var task = LocalizationSettings.StringDatabase.GetAllTables();
        await task.Task;
        List<StringTable> tables = (List<StringTable>)task.Result;
        foreach (var table in tables)
        {
            StringTableEntry entry = table.GetEntry(key);
            if (entry != null)
                return entry.GetLocalizedString();
        }
        return key;
    }
}