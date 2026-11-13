# Diva WiFi for OpenSimulator Nessie
## Installation Guide | v0.1.0

Diva WiFi provides a web-based administration and user interface for OpenSimulator grids.

This release is a compatibility port of the original Diva WiFi modules for OpenSimulator 0.9.3.1 Nessie Dev, built for .NET 8.

### 1. Requirements

The release has been built and tested with:

- OpenSimulator 0.9.3.1 Nessie Dev, commit `58c211829b`
- .NET 8
- Windows
- MariaDB/MySQL
- ROBUST running in grid or Hypergrid mode

Other OpenSimulator revisions and configurations may work but have not been verified with this release.

### 2. Back up your installation

Before installing:

1. Stop ROBUST.
2. Back up your ROBUST configuration files.
3. Back up your OpenSimulator database.
4. Make a backup copy of your existing `bin` directory, or perform the installation on a separate test copy.

The installation uses the existing OpenSimulator database and services.

### 3. Install the files

1. Download the **Diva WiFi Nessie v0.1.0 binary ZIP** from the project's GitHub release page.
2. Extract the ZIP using your preferred archive extractor or Windows Explorer.
3. Open the extracted `bin` folder.
4. Copy its contents into your OpenSimulator `bin` directory, preserving the language subdirectories.

The package includes seven Diva DLLs, five language resource assemblies, `Wifi.ini` and `Diva.Wifi.pot`.

The packaged `Wifi.ini` is provided as a reference and for alternative configuration arrangements. The recommended installation method below configures WiFi directly through ROBUST.

### 4. Configure the database providers

Open the ROBUST configuration file used to start your grid, typically `Robust.HG.ini` or `Robust.ini`.

Locate the following existing sections and set their `StorageProvider` values:

```ini
[GridService]
    StorageProvider = "Diva.Data.MySQL.dll"

[UserAccountService]
    StorageProvider = "Diva.Data.MySQL.dll"

[GridUserService]
    StorageProvider = "Diva.Data.MySQL.dll"
```

**Edit the existing sections rather than creating duplicate sections.**

Retain all other settings, including existing database connection strings and service configuration.

These settings match the configuration used during development testing.

### 5. Configure WiFi

Add a `[WifiService]` section to the ROBUST configuration, or edit the existing section if one is already present:

```ini
[WifiService]
    Enabled = true

    GridName = "My Grid"

    LoginURL = "http://YOUR-GRID-HOST:8002"
    WebAddress = "http://YOUR-GRID-HOST:8002"

    AdminFirst = "Wifi"
    AdminLast = "Admin"

    AccountConfirmationRequired = false

    ServePath_BinData = "data, /bin/data"
```

Change the following values to match your grid:

| Setting | Description |
|---|---|
| `GridName` | The name displayed by WiFi |
| `LoginURL` | Your grid's login service URL |
| `WebAddress` | Public address used for WiFi links |
| `AdminFirst` | First name of the WiFi administrator account |
| `AdminLast` | Last name of the WiFi administrator account |

The example uses port `8002`, matching the tested configuration. Use your own public ROBUST port if different.

#### 5.1 Choosing the WiFi administrator account

WiFi uses the `AdminFirst` and `AdminLast` settings to identify its administrative account.

**For an existing grid**

Choose the account that will administer WiFi **before starting ROBUST with WiFi enabled**.

Set `AdminFirst` and `AdminLast` to match that account's existing avatar name exactly.

For example, if your chosen administrator is `Grid Manager`:

```ini
AdminFirst = "Grid"
AdminLast = "Manager"
```

This is particularly important on an established grid where administrator accounts and permissions may already have been configured.

WiFi's initialization code looks up the configured administrator account and includes logic to create or update an account when necessary. Consequently, leaving the example administrator name in place could result in unintended account changes.

Review your administrator selection and keep a database backup before enabling WiFi for the first time.

**For a new grid**

When creating a new grid, the administrator account can be planned as part of the initial WiFi configuration.

Set `AdminFirst` and `AdminLast` to the intended administrator identity before the first startup.

The original Diva WiFi implementation includes administrator account initialization functionality. Initial account creation on a completely new Nessie grid has not yet been separately tested in this compatibility release.

**For both installations**

The administrator account should be a deliberately selected, secured account. Avoid retaining the example `Wifi Admin` name unless that is genuinely the account you intend to use.

#### 5.2 Other WiFi settings

`AccountConfirmationRequired = false` disables the account-confirmation requirement. Review this setting before enabling public user registration.

`ServePath_BinData` is included in the tested configuration and should be retained.

WiFi normally uses ROBUST's configured public HTTP port. A separate `ServerPort` setting is available, but is not required for the tested configuration.

### 6. Start ROBUST

Start ROBUST normally, using the configuration file you edited.

For example:

```powershell
.\Robust.exe -inifile=Robust.HG.ini
```

Check the console output for WiFi initialization messages.

WiFi is loaded through OpenSimulator's add-in system. No additional `[ServiceList]` entry is required for the tested installation.

On first startup, WiFi may extract its web resources into a `WifiPages` directory.

### 7. Open the WiFi interface

Open a browser and navigate to:

`http://YOUR-GRID-HOST:8002/wifi`

Replace the hostname and port with your grid's actual address.

The tested installation is accessible at the `/wifi` path on ROBUST's public HTTP service.

Sign in using the configured WiFi administrator account.

### 8. Testing status

The following functionality has been exercised successfully during development:

- ROBUST startup and WiFi initialization
- Initial web-resource extraction
- Administrator login
- User listing and search
- Inventory browsing
- Groups display
- Region and Hypergrid hyperlink display
- Console page display

The source has also been rebuilt successfully from a clean Nessie checkout.

Additional functions, including new account registration, password recovery, remote console command execution and the language interfaces, have not yet been individually verified.

SQLite and standalone Diva D3 configurations are outside the scope of this release.

### 9. Troubleshooting

**WiFi page does not load**

Confirm that ROBUST has started, `[WifiService]` is enabled, and you are using the correct hostname, port and `/wifi` path.

**Database or service initialization errors**

Check the three Diva storage-provider settings and confirm that your existing database connection strings remain unchanged.

**WiFi configuration is not detected**

Check that `[WifiService]` is present in the ROBUST configuration actually being loaded. Avoid duplicate sections or conflicting values in included configuration files.

**Web pages or resources are missing**

Check the ROBUST startup log for errors and confirm that the package files were copied into the correct `bin` directory. WiFi's web resources are extracted during initialization.

### 10. Removing the installation

1. Stop ROBUST.
2. Restore the original storage-provider settings.
3. Remove or disable the `[WifiService]` configuration.
4. Restore your original `bin` files from backup, or remove the Diva files installed by this package.
5. Restart ROBUST and verify normal operation.

Database backups should be retained independently of the file installation.

### Project information

This release preserves the original Diva WiFi source and attribution while providing compatibility changes for the Nessie/.NET 8 environment.

See `README.md`, `CHANGES.md` and `THIRD-PARTY-NOTICES.md` in the source repository for additional information.
