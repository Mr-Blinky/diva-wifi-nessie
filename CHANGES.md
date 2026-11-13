# Changes: Diva WiFi Nessie v0.1.0 candidate

This is an **unofficial compatibility port**, not a rewrite of Diva WiFi. The clean source staging tree retains the original 398 source/resource files; **11 existing files differ** from the inspected upstream Diva distribution. No original files were removed. Generated `.csproj` files are excluded from the release source.

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

## Build and runtime evidence

Seven required DLLs were built and deployed for the Windows/MariaDB test installation. Five localization satellite assemblies compiled. ROBUST startup, addin discovery, resource extraction, admin login, inventory, user listing/search, groups, region links, and console page loading were exercised successfully. This does not constitute comprehensive regression coverage.

## Explicit limitations

- **MySQL/MariaDB supported; SQLite unsupported and untested.** The generated SQLite project is not evidence of support.
- Linux and other OpenSimulator commits untested.
- Registration, password recovery, remote console command execution, and localized page behavior untested.
- A clean build from the eventual published repository has not yet been completed.

## Future possibilities

SQLite compatibility work and a separate unofficial Diva D3 Nessie standalone distribution are under consideration, not part of v0.1.0.
