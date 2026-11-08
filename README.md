# Asset Tracking System

A C# console application for tracking company assets (computers and mobile phones) in offices in different countries. You can add assets, see them in a table sorted by office or by type, search by brand or model, remove assets by id, and keep the assets between runs in a JSON file. Every price is shown in US dollars and in the office's local currency using live exchange rates, and assets that are close to the end of their life are highlighted in yellow or red.

## Description

The app is built for a company that needs to know what equipment each office has and when it has to be replaced. It was written as a course assignment (Mini Project: Asset Tracking) to practise object-oriented programming (abstract classes, inheritance, polymorphism), LINQ sorting, `DateTime` calculations, input validation, custom exceptions, working with files (JSON serialization), calling a web API and unit testing with xUnit.

The code is split into layers, so that each class has one job:

- **Models** – `Asset` is an abstract class with the data that every asset has (id, brand, model, purchase date, price in USD, office) and validates it in its constructor. `Computer` and `MobilePhone` inherit from it. `Office` holds an office name and its currency. `AssetData` is what is saved to the file: the assets and the next free id. `CurrencyRate` is one exchange rate from the bank.
- **Services** – `AssetService` stores the asset list and does the work on it: adding (with a new id), finding by id, removing, sorting and searching. `AssetFileStorage` saves the list to a JSON file and loads it back. `CurrencyConverter` downloads exchange rates and converts prices. None of them writes to the console.
- **UI** – `ConsoleUI` shows the menu and the tables, colors the rows and catches errors; `InputHelper` reads and validates user input and prints colored messages.
- **Exceptions** – custom exceptions (`ValidationException`, `AssetNotFoundException`, `UserCancelledException`, `InputClosedException`, `DataFileException`, `CurrencyRateNotFoundException`) carry error messages from the logic to the UI.

### Features

- **Add assets** – choose the type (Computer or Mobile phone), enter brand, model, purchase date and price in USD, and choose the office (Sweden, USA or Turkey). Keep adding until you type `Q` (works at any step).
- **Asset IDs** – every new asset automatically gets the next number: 1, 2, 3… An id is never given out again, even after the asset is removed or the app is restarted.
- **Input validation** – empty brand or model is rejected; the purchase date must be in the format `yyyy-MM-dd` and can't be in the future; the price must be a number greater than 0 with at most 2 decimals, and both a dot and a comma work as the decimal separator (`12.50` or `12,50`). On invalid input the app asks again instead of crashing.
- **List assets by office** – sorted by office, then by purchase date.
- **List assets by type** – sorted by type, then by purchase date.
- **Local price** – the USD price converted to the currency of the asset's office (SEK, USD or TRY) with today's exchange rates from the European Central Bank. If there is no internet connection, the app shows a warning on start, the Local Price column shows `N/A`, and everything else still works.
- **End-of-life colors** – rows in every table are red or yellow when the asset has to be replaced soon (see [End-of-life colors](#end-of-life-colors) below).
- **Search** – by part of the brand or model, case-insensitive (`oo` finds `Google Pixel` and `Lenovo Yoga`). Search again or type `Q` to go back to the menu.
- **Remove assets** – enter an asset id, check the asset shown in a table and confirm with `y` or cancel with `n`. Type `Q` to go back to the menu.
- **Save & load** – the list is saved to `assets.json` after every change (add or remove), and it is loaded automatically when the app starts. If the file is missing (first run), the app starts with an empty list; if the file is corrupted or contains invalid data, the app shows a message and starts with an empty list instead of crashing.
- **Colored messages** – green for success, red for errors, yellow for warnings, magenta for information.

## End-of-life colors

Every asset has a lifespan of **3 years** from its purchase date:

```
End of life = Purchase date + 3 years
```

Each time a table is printed, the app compares today's date with the end of life of every asset and colors the row:

| Color | Rule | Meaning |
|---|---|---|
| **Red** | 3 months or less left until the end of life, or the end of life has already passed | Replace now |
| **Yellow** | 6 months or less left (but more than 3 months) | Plan the replacement |
| Default | More than 6 months left | Nothing to do |

The checks are done from the most urgent to the least urgent: first "is it red?", then "is it yellow?", otherwise the default color.

Example for an asset bought on **2024-04-20** (end of life **2027-04-20**):

| Today is… | Time left | Row color |
|---|---|---|
| before 2026-10-20 | more than 6 months | default |
| 2026-10-20 – 2027-01-19 | 6 months or less | yellow |
| from 2027-01-20 | 3 months or less, or already expired | red |

The status is always calculated from today's date and is never stored, so the same asset becomes yellow and then red by itself as time goes by.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An internet connection for the exchange rates (optional: without it, local prices show `N/A`)

### Installation

```bash
git clone https://github.com/alakhmakova/AssetTrackingSystem.git
cd AssetTrackingSystem/AssetTrackingSystem
dotnet run
```

### Where the data file is

The assets are saved in `assets.json` in the **folder the app is started from** (the current working directory). Which folder that is depends on how you start the app:

| How you start the app | Where `assets.json` is |
|---|---|
| `dotnet run` inside `AssetTrackingSystem/AssetTrackingSystem` (as above) | `AssetTrackingSystem/AssetTrackingSystem/assets.json`, the file with test assets that comes with the repository |
| `dotnet run --project AssetTrackingSystem` from the repository root | `AssetTrackingSystem/assets.json` in the repository root (a new, empty file) |
| Visual Studio (F5) | `AssetTrackingSystem/AssetTrackingSystem/bin/Debug/net10.0/assets.json`, a copy of the test file that is placed there on build |

If the app says *"No saved assets found. Starting with an empty list."*, it was started from a folder without `assets.json`. The file is created there after the first asset is added. To see the test assets, start the app from the project folder as shown above or from Visual Studio.

### Running the tests

From the repository root:

```bash
dotnet test
```

## How to use

After start, the app downloads the exchange rates, loads the saved assets and shows the main menu. Type the number of an option and press Enter.

```
-----------------------------
COMPANY ASSET TRACKING SYSTEM
-----------------------------
3 assets loaded from file

1-Add an Asset
2-List Assets by Office
3-List Assets by Type
4-Search Assets
5-Remove an Asset
0-Quit
Select Option 0-5:
```

### 1 – Add an asset

```
Select Option 0-5: 1
-----------------------------------------------------------
To add a new asset - follow the steps | To quit - enter 'Q'
-----------------------------------------------------------
Type (1-Computer, 2-Mobile phone): 1
Enter Brand: Lenovo
Enter Model: ThinkPad X1
Enter Purchase Date (yyyy-MM-dd): 2030-01-01
Date must be in the format yyyy-MM-dd and can't be in the future
Enter Purchase Date (yyyy-MM-dd): 2024-04-20
Enter Price in USD: -5
Price must be greater than 0 with at most 2 decimals
Enter Price in USD: 1300
1-Sweden (SEK)
2-USA (USD)
3-Turkey (TRY)
Select Office 1-3: 1
The asset "Lenovo ThinkPad X1" was successfully added to the Sweden office!

Type (1-Computer, 2-Mobile phone): q
User cancelled the operation.
```

### 2 – List assets by office

Sorted by office, then by purchase date. Local prices depend on the exchange rates of the day.

```
Select Option 0-5: 2
------------------------------------------------------------------------------------------------
Id   Type           Brand    Model         Office   Purchase Date     Price (USD)    Local Price
------------------------------------------------------------------------------------------------
1    Computer       Apple    MacBook Pro   Sweden   2023-03-15            1500.00   14607.00 SEK
3    Computer       Lenovo   ThinkPad X1   Sweden   2024-04-20            1300.00   12659.40 SEK
2    Mobile phone   Apple    iPhone 14     USA      2025-09-10             999.00     999.00 USD
------------------------------------------------------------------------------------------------
```

In the console, row 1 is red (its end of life, 2026-03-15, has passed) and row 3 is yellow (less than 6 months left). Colors are not visible in this README.

### 3 – List assets by type

The same table, sorted by type (`Computer`, then `Mobile phone`), then by purchase date.

### 4 – Search assets

Type part of a brand or a model. The matching assets are shown in a table. Type `Q` to go back to the menu.

```
Select Option 0-5: 4
-----------------------------------------------
Search by brand or model | To quit - enter 'Q'
-----------------------------------------------
Search: think
--------------------------------------------------------------------------------------------
Id   Type       Brand    Model         Office   Purchase Date     Price (USD)    Local Price
--------------------------------------------------------------------------------------------
3    Computer   Lenovo   ThinkPad X1   Sweden   2024-04-20            1300.00   12659.40 SEK
--------------------------------------------------------------------------------------------
Search: xyz
No assets found for "xyz".
Search: q
User cancelled the operation.
```

### 5 – Remove an asset

Type the id of the asset. The asset is shown in a table, and you confirm with `y` or keep it with `n`. Type `Q` to go back to the menu.

```
Select Option 0-5: 5
----------------------------------------------------------
Enter the id of the asset to remove | To quit - enter 'Q'
----------------------------------------------------------
Enter asset id to remove: 7
Asset with id 7 not found.
Enter asset id to remove: 2
--------------------------------------------------------------------------------------------
Id   Type           Brand   Model       Office   Purchase Date     Price (USD)   Local Price
--------------------------------------------------------------------------------------------
2    Mobile phone   Apple   iPhone 14   USA      2025-09-10             999.00    999.00 USD
--------------------------------------------------------------------------------------------
Remove this asset? (y/n): y
The asset "Apple iPhone 14" was successfully removed!

Enter asset id to remove: q
User cancelled the operation.
```

### 0 – Quit

Closes the application.

## Why Local Price is not stored

`Asset` has no `LocalPrice` property, and the local price is not saved in `assets.json`. It is calculated only when a table is printed.

- `PriceUsd` and `Office` (with its currency) are facts about the asset. They never change, so they are stored.
- The exchange rate changes every day. A local price saved yesterday would already be wrong today.

So the app stores the facts and calculates the local price from them when it is needed: `CurrencyConverter.Convert(asset.PriceUsd, Currency.USD, asset.Office.Currency)`. This also keeps `Asset` independent from the internet: if the rates can't be downloaded, assets still load, save and display, and only the Local Price column shows `N/A`.

The same idea is used for the end of life: `EndOfLife` and the row color are calculated from the purchase date and today's date, and are not stored (`[JsonIgnore]`).

The exchange rates come from the [European Central Bank](https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml). The bank gives every rate against the euro, so a price is converted through EUR: `amount / rate(from) * rate(to)`.

## Project structure

```
AssetTrackingSystem/                       ← repository root, AssetTrackingSystem.slnx
├── AssetTrackingSystem/                   ← console application
│   ├── Program.cs                         ← entry point: starts ConsoleUI
│   ├── assets.json                        ← saved assets
│   ├── Models/
│   │   ├── Asset.cs                       ← abstract base class: common data, validation, end-of-life status
│   │   ├── Computer.cs                    ← Type = "Computer"
│   │   ├── MobilePhone.cs                 ← Type = "Mobile phone"
│   │   ├── Office.cs                      ← office name + currency
│   │   ├── AssetData.cs                   ← what is saved: assets + next id
│   │   ├── CurrencyRate.cs                ← one exchange rate (currency code + rate from EUR)
│   │   ├── Currency.cs                    ← enum: SEK, USD, TRY, EUR
│   │   └── LifeStatus.cs                  ← enum: Normal, Yellow, Red
│   ├── Exceptions/
│   │   ├── ValidationException.cs         ← invalid asset data
│   │   ├── AssetNotFoundException.cs      ← no asset found (search or id)
│   │   ├── UserCancelledException.cs      ← user typed Q
│   │   ├── InputClosedException.cs        ← input stream was closed (Ctrl+Z)
│   │   ├── DataFileException.cs           ← data file can't be read or written
│   │   └── CurrencyRateNotFoundException.cs ← no exchange rate for a currency
│   ├── Services/
│   │   ├── AssetService.cs                ← holds the asset list and works with it
│   │   ├── AssetFileStorage.cs            ← saves and loads assets (JSON)
│   │   └── CurrencyConverter.cs           ← downloads ECB rates and converts prices
│   └── UI/
│       ├── ConsoleUI.cs                   ← menu, tables, row colors, error handling
│       └── InputHelper.cs                 ← reads and validates input, colored messages
└── AssetTrackingSystem.Tests/             ← xUnit tests
    ├── AssetTests.cs
    ├── OfficeTests.cs
    ├── CurrencyRateTests.cs
    ├── InputHelperTests.cs
    ├── AssetServiceTests.cs
    └── CurrencyConverterTests.cs
```

## Tests

71 xUnit tests, all passing.

| Test class | What is tested |
|---|---|
| `AssetTests` | Constructor sets all properties; empty brand/model, a future date, an invalid price (0, negative, more than 2 decimals) and a missing office throw `ValidationException`; spaces around brand and model are trimmed; `IsValidPrice` and `IsValidDate` |
| `OfficeTests` | Constructor sets name and currency; empty name throws `ValidationException`; spaces around the name are trimmed |
| `InputHelperTests` | `TryParsePrice` reads a price with a dot or a comma (`12.50` and `12,50`) and rejects text, thousands separators, two separators and a minus sign |
| `CurrencyRateTests` | Constructor sets code and rate; a rate of 0 or below and an empty currency code throw `ValidationException` |
| `AssetServiceTests` | Ids grow and are never reused; both sorts (including an empty list); `HasAssets`; `GetData` / `LoadData` (including a hand-edited `NextId` that is too small); get and remove by id (found / not found); search by brand, by model, case-insensitive, with spaces around the text, empty text throws `ValidationException`, nothing found; `null` passed to `AddAsset` or `LoadData` throws `ArgumentNullException` |
| `CurrencyConverterTests` | USD→SEK, USD→EUR, EUR→SEK, same currency, missing rate throws `CurrencyRateNotFoundException`. Uses fixed fake rates, so no internet is needed |

### Out of test scope (on purpose)

- **`ConsoleUI` and the console part of `InputHelper`** – they only read from and write to the console. They were tested by hand. The price parsing (`TryParsePrice`) is a separate method and is tested.
- **`CurrencyConverter.FetchRates`** – it downloads live data from the bank, so the result depends on the internet and changes every day. The conversion logic is tested with fake rates instead.

### Not written yet

- Tests for `AssetFileStorage` (save and load round trip, missing file, corrupted file → `DataFileException`).
- Tests for `Asset.GetStatus` (Normal / Yellow / Red at the boundary dates).

## Levels completed

- [x] Level 1 – Basic classes & objects (`Computer`, `MobilePhone`, `List<Asset>`)
- [x] Level 2 – Sorting by type and purchase date, end-of-life warning colors
- [x] Level 3 – Offices in Sweden, USA and Turkey, currency conversion with a live API, sorting by office and purchase date
- [x] Level 4 – Abstract base class, inheritance, polymorphism, unique asset id, menu system
- [x] Level 5 (partly) – save & load assets from a JSON file, auto-save on add and remove, ids are never reused
- Bonus: search by brand/model, remove asset, colorful console UI, exception handling, API-based currency conversion, unit tests

## Credits

Created by Anastasiya Lakhmakova – GitHub: [@alakhmakova](https://github.com/alakhmakova)

The exchange-rate download is based on the course example `LiveCurrency.cs`.
