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

Locate the existing `[GridService]`, `[UserAccountService]` and `[GridUserService]` sections. In each section, **add** this setting:

```ini
    StorageProvider = "Diva.Data.MySQL.dll"
```

If a section already contains an active `StorageProvider` setting, change its value instead of adding a duplicate.

For example, a standard Nessie configuration may contain:

```ini
[GridService]
    LocalServiceModule = "OpenSim.Services.GridService.dll:GridService"
```

After the addition, that part of the section should read:

```ini
[GridService]
    StorageProvider = "Diva.Data.MySQL.dll"
    LocalServiceModule = "OpenSim.Services.GridService.dll:GridService"
```

Make the same `StorageProvider` addition under `[UserAccountService]` and `[GridUserService]`.

**Do not replace entire configuration sections or create duplicate section headings.** Keep all existing entries, especially `LocalServiceModule`, database connection strings, region settings and Hypergrid settings. `LocalServiceModule` selects the service implementation; `StorageProvider` selects its database storage provider.

These three additions match the configuration used during development testing.

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

    SmtpHost = "YOUR_EMAIL_SERVER"
    SmtpPort = 587
    SmtpUsername = "YOUR_MAILBOX"
    SmtpPassword = "YOUR_MAILBOX_PASSWORD"

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

#### 5.3 Configure outgoing SMTP email

WiFi uses SMTP for password recovery and account notifications. Add the following settings to the same `[WifiService]` section (they are included in the example above):

```ini
    SmtpHost = "YOUR_EMAIL_SERVER"
    SmtpPort = 587
    SmtpUsername = "YOUR_MAILBOX"
    SmtpPassword = "YOUR_MAILBOX_PASSWORD"
```

Replace all placeholders with your SMTP server, sending mailbox and password. `SmtpUsername` also serves as the **From** email address, so it should contain a valid mailbox address.

**SMTP encryption in this release:** port **25** disables TLS; all other ports enable STARTTLS using .NET `SmtpClient`. Port **587** is suitable for SMTP servers configured for authenticated STARTTLS submission. Port **25** was verified with a local mail server without TLS; only use unencrypted SMTP on a controlled, trusted network. This implementation does not provide implicit TLS (SMTPS) on port **465**.

Keep the real SMTP password in your private ROBUST configuration, not in source control or the published ZIP. Restrict access to that configuration.

To test email, restart ROBUST, request a password reset for a test account with an email address you control, check that the message arrives, follow its recovery link and set a new password. The reset link uses the configured `WebAddress`, so ensure that address is reachable by the recipient. If WiFi is served publicly, use HTTPS before exposing password-reset links to general users.

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

The following functionality has been exercised successfully on the tested Windows/Nessie/MariaDB configuration:

- Clean binary ZIP installation on a copy of an existing ROBUST grid
- ROBUST startup, WiFi add-in discovery and initial web-resource extraction
- Administrator and ordinary-user access and navigation
- New account creation (user level 0, "Local User"), initial appearance and successful viewer login
- User listing, search, account management and account-detail changes
- Inventory browsing and group management
- Password-recovery email delivery, recovery-link use and successful password change
- Web/REST console commands (tested, but disabled by default in the test deployment)
- Other available user and administrator forms and controls exercised in the test deployment
- Explicit linked Hypergrid region creation, listing, in-world map discovery and deletion

The source has also been rebuilt successfully from a clean Nessie checkout.

Linked Hypergrid region management was verified against OSGrid: WiFi created two explicit region links, ROBUST retrieved the remote region map image, the destinations appeared in the linked-region list and in-world map search, and WiFi successfully deleted both links. Hypergrid destination access policies still apply independently of successful linking.

**Not yet separately verified:** language/localization behavior. An intermittent `System.ObjectDisposedException` was observed once during ROBUST HTTP-listener shutdown; source inspection points to an upstream timeout-manager race, so it is deferred unless reproduced. No DivaData invalid-cast messages have been observed since all three storage-provider settings were configured.

SQLite and standalone Diva D3 configurations are outside the scope of this release.

### 9. Troubleshooting

**WiFi page does not load**

Confirm that ROBUST has started, `[WifiService]` is enabled, and you are using the correct hostname, port and `/wifi` path.

**Database or service initialization errors**

Check the three Diva storage-provider settings and confirm that your existing database connection strings remain unchanged.

**WiFi configuration is not detected**

Check that `[WifiService]` is present in the ROBUST configuration actually being loaded. Avoid duplicate sections or conflicting values in included configuration files.

**Password-recovery email fails**

Check `SmtpHost`, `SmtpPort`, `SmtpUsername` and `SmtpPassword` in `[WifiService]`. Confirm your SMTP server accepts the connection and the selected port matches its TLS mode. Inspect the ROBUST log for SMTP errors. Also verify `WebAddress` produces a reachable recovery link.

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
