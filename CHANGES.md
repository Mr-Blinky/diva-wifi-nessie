# Changes: Diva WiFi Nessie v0.9.0 RC1

This is an **unofficial compatibility port**, not a rewrite of Diva WiFi. The original compatibility changes, packaging refinements and attribution improvements are documented below. No original upstream source files were intentionally removed. Generated `.csproj` files are excluded from the release source.

## Build-system compatibility

- `00Data/prebuild.xml`
- `00Data/prebuild.MySQL.xml`
- `00DivaInterfaces/prebuild.xml`
- `01DivaUtils/prebuild.xml`
- `1DivaOpenSimServices/prebuild.xml`
- `20WifiScriptEngine/prebuild.xml`
- `21Wifi/prebuild.xml`

The module Prebuild definitions were updated for `net8_0` and legacy framework references removed where necessary for the Nessie/.NET 8 build. `21Wifi/prebuild.xml` also embeds `Localization/*.resx` so satellite assemblies can be produced through the normal generated project build.

## Source compatibility and cleanup

- `01DivaUtils/Request.cs`: removed obsolete `System.Web` usage and the unused `HttpCookieCollection` field/assignment.
- `01DivaUtils/WebAppUtils.cs`: removed unused exception variables from two `catch` clauses (warning cleanup; behavior intended unchanged).
- `21Wifi/WebApp/WifiScriptFace.cs`: replaced the unavailable `OpenSim.Framework.Util.GetRuntimeInformation()` call with `System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription`. The original `Version` property (`OpenSim.VersionInfo.Version`) and the footer's opt-in display were preserved. Both version and runtime information were subsequently verified in the rendered Nessie WiFi footer; no further code change was needed.
- `21Wifi/WifiMain.cs`: removed an unused exception variable from a `catch (InvalidOperationException)` clause (warning cleanup; behavior intended unchanged).


## Pending validation: avatar-specific password recovery

This branch ports the avatar-name and UUID recovery flow contributed by Robyn Starbrook (original commits `a609512` and `dee0d8c`) without PostgreSQL, SQLite or SMTP transport refactoring. A reset request now requires first name, last name and registered email, and the recovery link identifies the account by UUID. Unknown account details receive the same public confirmation as a matching account. Reset failure no longer falsely reports success. The existing asynchronous SMTP sender remains unchanged; acceptance or delivery is not guaranteed by the confirmation.

**Test deployment:** stop ROBUST; back up any customised pages; delete only the two extracted pages `forgotpasswordform.html` and `recoveringpassword.html` from the active `WifiPages` directory (and any language-specific copies being tested). Restart ROBUST and confirm the new pages were regenerated before testing. If the pages do not regenerate, restore the backups and inspect the resource extraction path. This change has not yet been compiled or runtime-tested on the target deployment. Check shared-email avatars, mismatched email, invalid/expired tokens, successful reset and SMTP delivery. Rate limiting and token replay behaviour require separate verification.

## In-memory single-use recovery tokens (tested on Nessie/MariaDB)

- WiFi now retains only a SHA-256 hash of the newest recovery token per avatar plus a fixed UTC expiration time **in process memory**. No new database table, schema change, persistent token register or OpenSimulator core modification is required.
- Only the newest link for an avatar is accepted. Links expire **60 minutes after issuance**; opening a link does not extend WiFi's deadline.
- A valid reset consumes the WiFi token before attempting the password change. Reset attempts are serialized under a lock. On success WiFi also releases the underlying OpenSimulator authentication token. If the password update fails, users must request a new link.
- **All outstanding WiFi recovery links are invalidated when ROBUST restarts.** This includes links issued by older WiFi builds after deploying the in-memory implementation. Users must simply request a fresh email.
- The previous file-based test build created `WifiPasswordRecoveryTokens.json` in the test ROBUST executable directory. The new build **does not read or write this file**. Stop ROBUST and securely delete the leftover JSON file once the new DLL is deployed; do not delete unrelated files.
- This memory-only approach assumes one WiFi/ROBUST process handles recovery requests. Load-balanced or separate WiFi processes do not share tokens and require a coordinated implementation.
- **Verified in test ROBUST (22 November 2026):** clean .NET 8 build (0 warnings, 0 errors), recovery email delivery, successful password reset, rejection of a previously used link, invalidation of an unused link after ROBUST restart, and rejection of an older email A after a newer recovery email B was requested for the same avatar. Previously, the avatar-name/UUID flow and two separate avatars sharing one registered email were also tested successfully.\n- **Still pending verification:** using newer email B to complete a reset in that same two-request test, and live testing of the 60-minute expiry (the fixed expiry check is present in code). No HTML page replacement is required for this code-only change.

## Packaging, SMTP configuration and attribution

- `21Wifi/Wifi.ini`: documented generic outgoing SMTP settings and provided placeholder values instead of Gmail sample credentials.
- `21Wifi/WifiPages/links.html`: added a link to this unofficial Nessie port beneath the original Diva Distro link, retaining original credits and navigation.
- `INSTALL.md` and `README.md`: updated for the binary installation procedure, email settings and verified functional tests.
- `UserDocsPath = "."` documented and included in the sample `Wifi.ini` to keep editable WiFi pages within the ROBUST working directory; the original fallback `..` remains available.

## Build and runtime evidence

Seven required DLLs were built and deployed for the Windows/MariaDB test installation. Five localization satellite assemblies compiled. A clean install from the release-candidate ZIP on a copied ROBUST deployment succeeded.

The user and administrator interface, account creation and viewer login, inventory, groups, user management, password recovery via SMTP (including successful password change), and REST console commands were exercised successfully. Explicit Hypergrid links to two OSGrid regions were created, displayed in WiFi and in-world map search, then deleted successfully. A remote destination's access restrictions are independent of the linking mechanism.

The original footer's optional `Version` and `RuntimeInfo` display was enabled for a live-page test and correctly rendered `OpenSim 0.9.3.1 Nessie Dev .NET 8.0.32`. As in upstream Diva WiFi, these details remain hidden by default because advertising software versions can aid targeted attacks. The test required only enabling the existing template directives in `WifiPages/footer.html`, not recompiling the software.

The earlier DivaData invalid-cast warnings did not recur after all three provider settings were configured. One intermittent shutdown exception appears to originate in upstream OpenSimulator's HTTP timeout-manager teardown and is deferred.

## Explicit limitations

- **MySQL/MariaDB supported; SQLite unsupported and untested.** The generated SQLite project is not evidence of support.
- Linux and other OpenSimulator commits untested.
- Localized page behavior not separately verified.
- Final clean-copy ROBUST test passed with the updated binary package, including extracted WiFi web resources. The recommended `UserDocsPath = "."` layout was also verified.

## Future possibilities

SQLite compatibility work and a separate unofficial Diva D3 Nessie standalone distribution are under consideration, not part of v0.9.0 RC1.
