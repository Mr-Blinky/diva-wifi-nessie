# Original authorship, copyright and licensing notes

**Release preparation note:** This document records findings from an audit of the source distribution. It is **not** a substitute for the full applicable licence texts. Before redistributing a binary or publishing a release, obtain and include the complete licence terms required by each relevant copyright notice, including those accompanying the original OpenSimulator/Diva source. Do not describe this entire mixed-content distribution as governed by one blanket licence without further verification.

## Original project

Diva WiFi and associated modules originate from the Diva distribution project. Credit belongs to the original authors and contributors, including **Crista Lopes (Diva Canto)**, **Marck**, and **OpenSimulator contributors**, as indicated in the original files. This Nessie compatibility port is unofficial and is not endorsed by the original project.

## Source code notices

In the inspected upstream tree, **96 of 101 C# files** contained copyright notices with BSD 3-Clause-style redistribution/disclaimer provisions, including wording variations. All original file headers have been retained in the staged source. The complete original provisions, not a shortened paraphrase, must be retained or reproduced as required for source and binary redistribution.

Five C# files had no header in the inspected upstream source:

- `00DivaInterfaces/Info.cs`
- `00DivaInterfaces/ISceneActor.cs`
- `00DivaInterfaces/IWifiAddon.cs`
- `00DivaInterfaces/IWifiApp.cs`
- `01DivaUtils/CSVUtil.cs`

The available upstream Git history attributes the commits introducing these files to **Diva Canto** (2013–2015). Git authorship helps establish provenance but does **not** by itself establish a licence grant. Their headers have not been invented or altered.

## Web design

The original `21Wifi/WifiPages/footer.html` states:

> Web design © Crista Lopes and Marck. Distributed under Creative Commons.

It links to **Creative Commons Attribution 3.0**: https://creativecommons.org/licenses/by/3.0/

Equivalent attribution appears in localized footer templates. Preserve these footer notices and the original credits. The footer establishes a web-design attribution; it does **not** conclusively identify the licence of every image, script, translation, or other resource in the distribution.

## Resource review

An automated search of text-based web, localization, and configuration resources found six Creative Commons references, all in localized/default footer templates. No additional explicit third-party notices were found by that search. This is not proof that all non-text resources have been independently cleared for redistribution.

## Before public or binary release

- Include the **complete applicable original BSD-style licence text(s)** and any required copyright/redistribution notices with source and binaries. Compare wording variants rather than silently substituting a generic template.
- Preserve the original CC BY 3.0 web-design attribution and licence link; include licence text or a suitable copy where required.
- Check resource provenance and any upstream contributor/notice files before making a definitive blanket licence claim.
- Clearly mark compatibility changes and identify this port as unofficial.

**Current state:** attribution and licence audit documented; final distribution-licence assembly and review still pending.
