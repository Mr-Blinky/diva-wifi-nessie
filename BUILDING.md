# Building and deploying Diva WiFi Nessie

## Known-good development environment

- OpenSimulator **0.9.3.1 Nessie Dev**, commit **`58c211829b`**
- **.NET SDK 8.0.426**, targeting **`net8.0`**
- **Windows**, with OpenSimulator's `runprebuild.bat`
- **MariaDB 11.8** through `Diva.Data.MySQL.dll`

Other revisions, operating systems, and SQLite have not been verified. **Only MySQL/MariaDB is supported for the current release candidate.**

## Prepare the source tree

1. Obtain the matching OpenSimulator source checkout and its normal build prerequisites.
2. Copy the six directories under this repository's `addon-modules/` into the checkout's `addon-modules/` directory, preserving their names and contents.
3. From the **OpenSimulator source root**, run `runprebuild.bat`. This regenerates the `.csproj` files, including the Diva projects. The source package deliberately does not ship generated `.csproj` files.
4. Confirm the generated Diva project files target `net8.0`.

## Build the projects

From the OpenSimulator source root, after the OpenSimulator dependencies have been built, the following PowerShell example builds the seven runtime projects in a dependency-friendly order:

```powershell
$projects = @(
    'addon-modules/00DivaInterfaces/Diva.Interfaces.csproj',
    'addon-modules/00Data/Diva.Data.csproj',
    'addon-modules/01DivaUtils/Diva.Utils.csproj',
    'addon-modules/00Data/MySQL/Diva.Data.MySQL.csproj',
    'addon-modules/20WifiScriptEngine/Diva.Wifi.ScriptEngine.csproj',
    'addon-modules/1DivaOpenSimServices/Diva.OpenSimServices.csproj',
    'addon-modules/21Wifi/Diva.Wifi.csproj'
)
foreach ($project in $projects) {
    dotnet build $project -c Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $project" }
}
```

The command sequence above is a **documented build procedure**, not a claim that this exact script has been independently executed from a fresh checkout. OpenSimulator project references may trigger dependent builds. Review actual build output and verify that the expected DLLs are present in the OpenSimulator `bin` directory.

The `Diva.Data.SQLite.csproj` definition is generated, but **do not build or deploy it as a supported provider for this release**.

The updated `21Wifi/prebuild.xml` embeds `Localization/*.resx`, allowing the normal .NET build to generate language satellite assemblies. Satellite compilation has been verified; localized page behavior has not.

## Runtime package contents

The tested runtime staging set contained these **14 files**, preserving relative paths:

```text
Diva.Data.dll
Diva.Data.MySQL.dll
Diva.Interfaces.dll
Diva.OpenSimServices.dll
Diva.Utils.dll
Diva.Wifi.dll
Diva.Wifi.ScriptEngine.dll
Diva.Wifi.pot
Wifi.ini
de/Diva.Wifi.resources.dll
en/Diva.Wifi.resources.dll
es/Diva.Wifi.resources.dll
fr/Diva.Wifi.resources.dll
pt/Diva.Wifi.resources.dll
```

Copy these files to the target ROBUST `bin` directory with the language subdirectories intact. `Wifi.ini` is retained as an addin resource; the **active** configuration in the tested deployment was in `Robust.HG.ini`. Runtime packages must also include applicable original licence and attribution notices; these are **additional documentation files**, not part of the 14-file runtime payload.

## Tested ROBUST configuration

Back up the configuration first. In the active `Robust.HG.ini`, use the following as an **example**, replacing URLs, grid name, and administrator name with your own values:

```ini
[GridService]
    StorageProvider = "Diva.Data.MySQL.dll"

[UserAccountService]
    StorageProvider = "Diva.Data.MySQL.dll"

[GridUserService]
    StorageProvider = "Diva.Data.MySQL.dll"

[WifiService]
    Enabled = true
    GridName = "Example Grid"
    LoginURL = "https://grid.example.org:8002"
    WebAddress = "https://grid.example.org:8002"
    AdminFirst = "Admin"
    AdminLast = "Admin"
    AccountConfirmationRequired = false
    UserDocsPath = "."
    ServePath_BinData = "data, /bin/data"
```

The URLs are **illustrative**, not tested endpoints; use the scheme, hostname, and ports that match your actual ROBUST configuration. Do not duplicate INI sections if they already exist; update the existing sections. The example's account-confirmation setting mirrors the test setup, not a security recommendation. Review registration and administrative settings before exposing the service publicly.

`UserDocsPath = "."` is recommended when launching ROBUST from its installation directory: editable `WifiPages` resources remain inside that working directory, rather than being written to its parent (the original fallback `..`). The location is relative to the process working directory; users may choose `..` or an explicit path instead. The final clean-copy installation succeeded with this setting.

Do **not** add `WifiServiceConnector` under `[ServiceList]`. The tested setup uses Mono.Addins discovery of the enabled WiFi service; an explicit native service-list entry failed in testing.

## Deployment verification

1. Back up the database, configuration, and existing deployment. Prefer a disposable test ROBUST installation first.
2. Ensure the active ROBUST process can bind its configured ports; don't run test and production ROBUST simultaneously on the same listeners.
3. Start ROBUST with the intended configuration and inspect the log for WiFi addin discovery, initialization, and errors.
4. Test admin login and relevant pages. In development testing, new avatar registration, SMTP password recovery, account/group management, REST console commands, and explicit Hypergrid link creation/deletion worked. Review administrative controls before making the service available publicly.
5. On a test installation only, a fresh Mono.Addins cache and `WifiPages` extraction can be used to check resource completeness; don't delete production caches or content without backups.

## Clean-build release gate

The matching Nessie source worktree was rebuilt successfully (seven Diva projects, zero errors; three existing warnings in upstream OpenSimulator code). A 17-file binary ZIP was assembled and installed on a fresh ROBUST copy, using documented database-provider and `[WifiService]` settings. Startup, page extraction and UI availability passed. `UserDocsPath = "."` was independently tested and kept editable pages inside the ROBUST working directory. Prior comprehensive feature testing covered SMTP recovery, avatar creation, account/group management, REST console commands and Hypergrid link creation/deletion. Before publication, ensure the final ZIP includes the current documentation and sample `Wifi.ini`.
