using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleSheetData", menuName = "Google Sheets/Data")]
public class GoogleSheetData : ScriptableObject
{
    [SerializeField] private string sheetId; // ID ????? ???????
    [SerializeField] private string sheetName; // ??? ????? (????????, "Sheet1")
    [SerializeField] private string credentialsPath = "Assets/Resources/credentials.json"; // ???? ? JSON
    [SerializeField] private LocalizationData localizationData;

    private SheetsService GetSheetsService()
    {
        try
        {
            var credential = GoogleCredential.FromFile(credentialsPath)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "UnityGoogleSheets",
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize Sheets Service: {ex.Message}");
            return null;
        }
    }

    public void DownloadAndParseSheet()
    {
        if (string.IsNullOrEmpty(sheetId) || string.IsNullOrEmpty(sheetName))
        {
            Debug.LogError("Sheet ID or Sheet Name is empty!");
            return;
        }

        var service = GetSheetsService();
        if (service == null) return;

        try
        {
            string range = $"{sheetName}!A1:Z"; // ???????? ??? ?????? (????? ?????????)
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(sheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                List<string[]> data = new List<string[]>();
                foreach (var row in values)
                {
                    List<string> rowData = new List<string>();
                    foreach (var cell in row)
                    {
                        rowData.Add(cell?.ToString() ?? "");
                    }
                    data.Add(rowData.ToArray());
                }

                if (localizationData != null)
                {
                    localizationData.SetData(data);
                    Debug.Log("Localization data updated via Google Sheets API!");
                }
                else
                {
                    Debug.LogWarning("LocalizationData is not assigned!");
                }
            }
            else
            {
                Debug.LogWarning("No data found in the specified range.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to fetch data: {ex.Message}");
        }
    }

    public void OpenSheetInBrowser()
    {
        if (string.IsNullOrEmpty(sheetId))
        {
            Debug.LogError("Sheet ID is empty!");
            return;
        }
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{sheetId}");
    }
}