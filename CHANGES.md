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
- `21Wifi/WebApp/Services.cs`, `Services.PasswordRecovery.cs`: password recovery now sends synchronously and reports "Email could not be sent." when the SMTP server rejects the message, instead of always reporting success. Mail failures are logged at WARN instead of DEBUG. Each message uses its own `SmtpClient` (30 s timeout), so concurrent sends no longer collide on a shared client.
- Password recovery now asks for the avatar's first and last name as well as the email, and only sends a link when the email matches that avatar (several avatars may share one email; previously an arbitrary one was chosen). The recovery link and form identify the account by UUID instead of email. Setting the new password reports "Invalid token." instead of falsely confirming success. A name/email pair that matches no account now shows "No account matches that name and email address." (new string, translated in all five `Localization` languages) instead of returning silently to the home page. Changed: `21Wifi/WebApp/Services.PasswordRecovery.cs`, `21Wifi/Handlers/WifiForgotPasswordHandlers.cs`, and `forgotpasswordform.html` / `recoveringpassword.html` in all six page languages. Existing installations must copy these pages into their extracted `WifiPages` folder, because WiFi never overwrites pages that already exist there.

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
