using System.Reflection.Metadata;
using AssetTrackingSystem.Exceptions;
using AssetTrackingSystem.Models;
using AssetTrackingSystem.Services;

namespace AssetTrackingSystem.UI;

// Console UI: menu, input, tables, colors; the only class that prints and catches errors
public class ConsoleUI
{
    // Logic lives in AssetService; readonly, so it is never replaced
    private readonly AssetService _service = new();

    // Offices the user can choose from.
    // TODO: move to a separate Offices class in the step with the office list
    private static readonly List<Office> Offices = new()
    {
        new Office("Sweden", Currency.SEK),
        new Office("USA", Currency.USD),
        new Office("Turkey", Currency.TRY),
    };

    private const int ColumnPadding = 2; // extra spaces added to each column width

    public void Run()
    {
        ShowHeader("COMPANY ASSET TRACKING SYSTEM");
        try
        {
            // Menu-based system: repeat until the user enters 0
            bool run = true;
            while (run)
            {
                Console.WriteLine();
                Console.WriteLine("1-Add an Asset");
                Console.WriteLine("2-List Assets by Office");
                Console.WriteLine("3-List Assets by Type");
                Console.WriteLine("4-Search Assets");
                Console.WriteLine("5-Remove an Asset");
                Console.WriteLine("0-Quit");

                string menuOption = InputHelper.ReadText("Select Option 0-5: ", "Menu option can't be empty");

                switch (menuOption)
                {
                    case "1":
                        AddAssets();
                        break;
                    case "2":
                        ListAssets(_service.GetAssetsSortedByOfficeAndDate());
                        break;
                    case "3":
                        ListAssets(_service.GetAssetsSortedByTypeAndDate());
                        break;
                    case "4":
                        SearchAssets();
                        break;
                    case "5":
                        RemoveAssets();
                        break;
                    case "0":
                        InputHelper.ShowInfo("Thank you for using this application");
                        run = false;
                        break;
                    default:
                        InputHelper.ShowError("Invalid menu option. Select 0-5.");
                        break;
                }
            }
        }
        // Console.ReadLine() returns null only when the input stream is closed:
        // Ctrl+Z then Enter (Windows) / Ctrl+D (Linux, macOS), or redirected input (dotnet run < input.txt) has ended
        catch (InputClosedException e)
        {
            InputHelper.ShowWarning(e.Message);
        }
    }

    // 1: add assets until the user enters q
    private void AddAssets()
    {
        ShowHeader("To add a new asset - follow the steps | To quit - enter 'Q'");
        try
        {
            while (true)
            {
                try
                {
                    int type = InputHelper.ReadNumberInRange("Type (1-Computer, 2-Mobile phone): ", 1, 2, "Type can't be empty", "Please enter 1 or 2");
                    string brand = InputHelper.ReadTextOrCancel("Enter Brand: ", "Brand can't be empty");
                    string model = InputHelper.ReadTextOrCancel("Enter Model: ", "Model can't be empty");
                    DateTime purchaseDate = InputHelper.ReadDate("Enter Purchase Date (yyyy-MM-dd): ", "Purchase date can't be empty", "Date must be in the format yyyy-MM-dd and can't be in the future");
                    decimal priceUsd = InputHelper.ReadPrice("Enter Price in USD: ", "Price can't be empty", "Price must be greater than 0 with at most 2 decimals");
                    Office office = ReadOffice();

                    // Polymorphism: both are stored as Asset
                    Asset asset = type == 1
                        ? new Computer(brand, model, purchaseDate, priceUsd, office)
                        : new MobilePhone(brand, model, purchaseDate, priceUsd, office);

                    _service.AddAsset(asset);
                    InputHelper.ShowSuccess($"The asset {asset.Brand} {asset.Model} was successfully added to the {asset.Office.Name} office!\n ");
                }
                // Should not happen because input is checked above, but the Asset constructor has the final word
                catch (ValidationException e)
                {
                    InputHelper.ShowError(e.Message);
                }
            }
        }
        catch (UserCancelledException e)
        {
            InputHelper.ShowWarning(e.Message);
        }
    }

    // Show the office list and return the chosen office
    private Office ReadOffice()
    {
        for (int i = 0; i < Offices.Count; i++)
        {
            Console.WriteLine($"{i + 1}-{Offices[i].Name} ({Offices[i].Currency})");
        }
        int choice = InputHelper.ReadNumberInRange($"Select Office 1-{Offices.Count}: ", 1, Offices.Count, "Office can't be empty", $"Please enter a number 1-{Offices.Count}");
        return Offices[choice - 1];
    }

    // 2 and 3: print the list, or a warning if it is empty
    private void ListAssets(List<Asset> assets)
    {
        if (assets.Count == 0)
        {
            InputHelper.ShowWarning("Your asset list is empty.");
            return;
        }
        DisplayAssetsTable(assets);
    }

    // 4: search by brand or model until the user enters q
    private void SearchAssets()
    {
        if (!_service.HasAssets)
        {
            InputHelper.ShowWarning("No assets to search yet. Add an asset first.");
            return;
        }

        ShowHeader("Search by brand or model | To quit - enter 'Q'");
        try
        {
            while (true)
            {
                try
                {
                    string search = InputHelper.ReadTextOrCancel("Search: ", "Search string can't be empty");
                    DisplayAssetsTable(_service.SearchAssets(search));
                }
                catch (AssetNotFoundException e)
                {
                    InputHelper.ShowWarning(e.Message);
                }
            }
        }
        catch (UserCancelledException e)
        {
            InputHelper.ShowWarning(e.Message);
        }
    }

    // 5: remove by id: show the asset, ask to confirm; repeat until the user enters q
    private void RemoveAssets()
    {
        if (!_service.HasAssets)
        {
            InputHelper.ShowWarning("No assets to remove yet. Add an asset first.");
            return;
        }

        ShowHeader("Enter the id of the asset to remove | To quit - enter 'Q'");
        try
        {
            // Stop when there is nothing left to remove
            while (_service.HasAssets)
            {
                try
                {
                    int id = InputHelper.ReadId("Enter asset id to remove: ", "Asset id can't be empty", "Asset id must be a whole number greater than 0");
                    Asset asset = _service.GetAssetById(id);
                    DisplayAssetsTable(new List<Asset> { asset });
                    if (InputHelper.ReadYesNo("Remove this asset? (y/n): ", "Answer can't be empty", "Please enter y or n"))
                    {
                        _service.RemoveAssetById(id);
                        InputHelper.ShowSuccess("The asset was successfully removed!\n ");
                    }
                    else
                    {
                        InputHelper.ShowWarning("The asset was not removed.\n ");
                    }
                }
                catch (AssetNotFoundException e)
                {
                    InputHelper.ShowWarning(e.Message);
                }
            }
            InputHelper.ShowWarning("Your asset list is empty.");
        }
        catch (UserCancelledException e)
        {
            InputHelper.ShowWarning(e.Message);
        }
    }

    // Text between two lines of the same length
    private void ShowHeader(string text)
    {
        InputHelper.ShowInfo(new string('-', text.Length));
        InputHelper.ShowInfo(text);
        InputHelper.ShowInfo(new string('-', text.Length));
    }

    // Print assets as a table; column width = longest value or header + padding
    // TODO: later add a "Local Price" column (currency step) and color rows by end-of-life status (lifecycle step)
    private void DisplayAssetsTable(List<Asset> assets)
    {
        string[] headers = { "Id", "Type", "Brand", "Model", "Office", "Purchase Date", "Price (USD)" };
        List<string[]> rows = assets.Select(ToRow).ToList();
        int[] widths = GetColumnWidths(headers, rows);
        int tableWidth = widths.Sum() + widths.Length - 1; // + one space between columns

        InputHelper.ShowInfo(new string('-', tableWidth));
        InputHelper.ShowInfo(FormatRow(headers, widths));
        InputHelper.ShowInfo(new string('-', tableWidth));
        foreach (string[] row in rows)
        {
            Console.WriteLine(FormatRow(row, widths));
        }
        InputHelper.ShowInfo(new string('-', tableWidth));
    }

    // One asset -> table cells as text
    private static string[] ToRow(Asset asset)
    {
        return new[]
        {
            asset.Id.ToString(),
            asset.Type,
            asset.Brand,
            asset.Model,
            asset.Office.Name,
            asset.PurchaseDate.ToString("yyyy-MM-dd"),
            asset.PriceUsd.ToString("0.00"),
        };
    }

    // Widest value in each column (header included) + padding
    private static int[] GetColumnWidths(string[] headers, List<string[]> rows)
    {
        int[] widths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++)
        {
            int longest = headers[i].Length;
            foreach (string[] row in rows)
            {
                longest = Math.Max(longest, row[i].Length);
            }
            widths[i] = longest + ColumnPadding;
        }
        return widths;
    }

    // Pad each cell to its column width; the last column (price) is aligned to the right
    private static string FormatRow(string[] cells, int[] widths)
    {
        string[] padded = new string[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            bool isLastColumn = i == cells.Length - 1;
            padded[i] = isLastColumn ? cells[i].PadLeft(widths[i]) : cells[i].PadRight(widths[i]);
        }
        return string.Join(" ", padded);
    }
}
