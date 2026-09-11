# Migrating a Connector/Enricher to Multi-Version Targeting

This document tracks the migration of `CluedIn.Enricher.DnB` from a single-version build to the
multi-version targeting pattern. Modeled on the docs of the other repos migrated in the same
effort (GoogleMaps, Gleif, OpenCorporates, Permid, Brreg, KnowledgeGraph, ClearBit, CompanyHouse,
CVR, DuckDuckGo, RestApi, BvD, GoogleImages, libpostal, AzureOpenAI).

---

## Overview

The goal is to produce separate NuGet packages per CluedIn version from a single branch, using the
shared `crawler.build.jobs.yml` pipeline template.

| CluedIn version | .NET TFM | Package suffix |
|---|---|---|
| 4.7.0 | net6.0 | `.470` |
| 4.8.0 | net6.0 | `.480` |
| 5.0.0-beta.* | net10.0 | `.500` |

Verified `5.0.0-*` resolves to `5.0.0-beta.576` for this repo's own feeds. 4.6.0 excluded — no
evidence this repo's ExternalSearchProvider API surface needs it.

Branch: `feature/multi-version-targeting` (off `develop`).

---

## Step 1 — Pipeline template (`azure-pipelines.yml`)

Status: **Done**

Replaced the single-version `crawler.build.yml` steps-template (plus its explicit `UseDotNet@2`
installing 8.0 SDK) with `crawler.build.jobs.yml`, `multiVersionCluedInTargets` set to
4.7.0/4.8.0/5.0.0-beta.*, `useGitVersionDotNetTool: true` included from the start, switched pool
from `windows-latest` to `ubuntu-22.04`.

---

## Step 2 — `Directory.Build.props`

Status: **Done**

Honours `CluedInMultiVersionTargetFramework` (net10.0 local fallback), derives
`CLUEDIN_V47`/`V48`/`V50` `DefineConstants`, `LangVersion` pinned to 13.0. Repo's existing
`WarningsAsErrors`/`NoWarn` settings left untouched.

---

## Step 3 — `Packages.props` / `NuGet.config`

Status: **Done**

Both already correctly cased — no rename needed. `_CluedIn` guarded so the pipeline's per-leg
override wins.

**Local verification blocker (not a repo bug, a local-machine credential-provider issue):** this
repo's checked-in `NuGet.config` has hardcoded `packageSourceCredentials` (`ClearTextPassword`) for
the `develop`/`release`/`AzurePipelines` feeds. Restoring with them locally returned `401
Unauthorized` for `CluedIn.Processing`/`CluedIn.Processing.Web`/`CluedIn.Processing.Web.TechnologyDetection`
specifically (not `CluedIn.Core`, which restored fine). Confirmed via the Azure DevOps Packaging
REST API (`az account get-access-token` + `/_apis/packaging/Feeds/.../Packages`) that these
packages exist and are visible to the same identity — so this isn't a real permissions restriction,
it's the local Azure Artifacts Credential Provider plugin not presenting valid credentials for
these specific package IDs in a non-interactive shell. Worked around **for local verification
only** by using a scratch `NuGet.config` (never committed, not part of this repo) with an AAD
bearer token (`az account get-access-token --resource 499b84ac-1321-427f-aa17-267ca6975798`) as the
`ClearTextPassword`, passed via `dotnet restore --configfile <scratch>`. The repo's own
`NuGet.config` was not modified. **Flagging for a human:** the hardcoded PATs committed in this
repo's `NuGet.config` are worth rotating/removing in favor of `NuGetAuthenticate@0` (which is what
CI already uses) — no other migrated repo in this effort has static credentials committed like
this, and having them in git history is a minor but real exposure regardless of whether they still
work.

---

## Step 4 — Source — RestSharp 106-vs-114 API break

Status: **Done**

`DnBExternalSearchProvider.cs` was written against the newer RestSharp API (`Method.Get`/`Post`
PascalCase, `RestClientOptions`, `RestResponse` return/parameter types) — same family every prior
enricher in this effort hit, but here the code already assumed the *new* API as its unguarded
default, unlike some other repos. Fixed with `#if CLUEDIN_V50`/`#else` guards:

- Added `HttpGetMethod`/`HttpPostMethod` consts (`Method.Get`/`Post` under `CLUEDIN_V50`,
  `Method.GET`/`POST` otherwise), used at 5 call sites.
- `ExecuteWithRateLimitHandling` return type: `RestResponse` (5.0) vs `IRestResponse` (4.7/4.8).
- `ConstructFailedConnectionResponse` parameter type: same split.
- `RestClient` construction in `GetAuthToken`: `RestClientOptions` (107+-only) vs the legacy
  `RestClient(string baseUrl) { Timeout = ... }` pattern — note `RestClientOptions.Timeout` is a
  `TimeSpan?` (`Timeout.InfiniteTimeSpan`) on the new API but `RestClient.Timeout` is a millisecond
  `int` (`Timeout.Infinite`) on the old one, not just a type-name swap.

Verified 0 errors on all three legs (`dotnet build -p:_CluedIn=<v>
-p:CluedInMultiVersionTargetFramework=<tfm>`), plus the local-default net10.0 build.

**Authoring note:** an initial pass with `sed` for the `Method.Get`→`HttpGetMethod` replacements
stripped the file's CRLF line endings, which would have produced a ~2500-line diff on a ~30-line
change. Caught before committing by checking `git diff --stat` against expectations; restored CRLF
with a second `sed` pass, verified the diff was back to the expected minimal shape before
committing.

---

## Step 5 — `GitVersion.yml`

Status: **Done**

```yaml
next-version: 1.0
ignore:
  commits-before: 2026-09-07T00:00:00
```

No pre-existing `ignore:` key in this repo's `GitVersion.yml` (unlike CompanyHouse/CVR/several
others), so no merge-vs-clobber risk here. Highest pre-existing tag by real commit date (not
tag-name sort order) is `4.6.3` at `2026-09-04T21:17:27+08:00` — very recent, only ~6 days before
this migration. Padded to `2026-09-07T00:00:00` (just over 2 days past it). Verified with the
pipeline's actual pinned `GitVersion.Tool 5.9.0` (installed to a scratch tool-path):
`MajorMinorPatch: "1.0.0"`, confirmed correct.

---

## Step 6 — Push and confirm CI

Status: **Done**

Fully green on the **first push** — PR #52, build 151998: all three `Multi-version build+test` legs
(4.7.0, 4.8.0, 5.0.0-beta.*) and `Multi-version: publish` passed.

---

## Checklist

- [x] `azure-pipelines.yml` — switched to `crawler.build.jobs.yml`; pool switched to `ubuntu-22.04`
- [x] `Directory.Build.props` — `CluedInMultiVersionTargetFramework` handling, `DefineConstants`, `LangVersion` pinned
- [x] `Packages.props` / `NuGet.config` — already correctly cased; `_CluedIn` guarded
- [x] Source — `#if CLUEDIN_V50` guards for the RestSharp 106↔114 break (5 call sites + 3 signature/construction guards)
- [x] `GitVersion.yml` — `next-version: 1.0`; `commits-before: 2026-09-07T00:00:00`; verified `1.0.0` with pinned GitVersion.Tool 5.9.0
- [x] All three legs build clean locally (0 errors), verified via real `dotnet restore`/`build`
- [x] Pushed branch and confirmed the Azure DevOps pipeline is green end-to-end — PR #52, build 151998: all three legs + `Multi-version: publish` passed on the first run
- [ ] **Follow-up for a human:** consider rotating/removing the hardcoded PATs in this repo's `NuGet.config` in favor of `NuGetAuthenticate@0`
