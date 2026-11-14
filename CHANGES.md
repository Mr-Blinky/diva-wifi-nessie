# Changes: Diva WiFi Nessie v0.1.0 candidate

This is an **unofficial compatibility port**, not a rewrite of Diva WiFi. The clean source staging tree retains the original 398 source/resource files; The original compatibility changes are documented below; subsequent packaging and UI attribution improvements are noted separately. No original files were removed. Generated `.csproj` files are excluded from the release source.

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
- `21Wifi/WebApp/WifiScriptFace.cs`: replaced the unavailable `OpenSim.Framework.Util.GetRuntimeInformation()` call with `System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription`.
- `21Wifi/WifiMain.cs`: removed an unused exception variable from a `catch (InvalidOperationException)` clause (warning cleanup; behavior intended unchanged).

## Packaging, SMTP configuration and attribution

- `21Wifi/Wifi.ini`: documented generic outgoing SMTP settings and provided placeholder values instead of Gmail sample credentials.
- `21Wifi/WifiPages/links.html`: added a link to this unofficial Nessie port beneath the original Diva Distro link, retaining original credits and navigation.
- `INSTALL.md` and `README.md`: updated for the binary installation procedure, email settings and verified functional tests.

## Build and runtime evidence

Seven required DLLs were built and deployed for the Windows/MariaDB test installation. Five localization satellite assemblies compiled. A clean install from the release-candidate ZIP on a copied ROBUST deployment succeeded.

The user and administrator interface, account creation and viewer login, inventory, groups, user management, password recovery via SMTP (including successful password change), and REST console commands were exercised successfully. Explicit Hypergrid links to two OSGrid regions were created, displayed in WiFi and in-world map search, then deleted successfully. A remote destination's access restrictions are independent of the linking mechanism.

The earlier DivaData invalid-cast warnings did not recur after all three provider settings were configured. One intermittent shutdown exception appears to originate in upstream OpenSimulator's HTTP timeout-manager teardown and is deferred.

## Explicit limitations

- **MySQL/MariaDB supported; SQLite unsupported and untested.** The generated SQLite project is not evidence of support.
- Linux and other OpenSimulator commits untested.
- Localized page behavior not separately verified.
- The updated binaries containing the new sidebar attribution link still require rebuilding and a final installation check.

## Future possibilities

SQLite compatibility work and a separate unofficial Diva D3 Nessie standalone distribution are under consideration, not part of v0.1.0.
