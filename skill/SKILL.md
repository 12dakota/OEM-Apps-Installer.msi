---
name: oem-apps-installer
description: Build and deploy OEM Apps Installer.msi via GitHub Actions. Extracts apps.zip to C:\Windows\Apps only. Use when the user mentions OEM Apps Installer, apps.zip, C:\Windows\Apps, WiX MSI, or 12dakota/OEM-Apps-Installer.msi.
---

# OEM Apps Installer.msi

Account: `12dakota`  
Repository: https://github.com/12dakota/OEM-Apps-Installer.msi  
Artifact: `OemApps.msi`  
Locked install directory: `C:\Windows\Apps`

## What it does

WiX 4 MSI copies `apps.zip` into `C:\Windows\Apps` and extracts every entry there. No folder UI. Extract custom action ignores arguments.

## GitHub Actions (required deploy path)

Workflow file in the repo: `.github/workflows/build-msi.yml`

- Runner: `windows-latest`
- Triggers: `push` to `main`, `pull_request`, `workflow_dispatch`
- Steps: checkout → setup-dotnet 8 → placeholder zip if payload missing → `dotnet publish` extract CA → `wix build` → upload `OemApps-msi`

### Trigger a run

```
POST /repos/12dakota/OEM-Apps-Installer.msi/actions/workflows/build-msi.yml/dispatches
{"ref":"main"}
```

Or `gh workflow run build-msi.yml --repo 12dakota/OEM-Apps-Installer.msi`

### After apps.zip is ready

Commit the zip to `msi/payload/apps.zip` and push `main` (or dispatch the workflow). Do not commit GitHub PATs.

## Local build

```bat
copy your\apps.zip msi\payload\apps.zip
msi\build.cmd
```

## Agent protocol

1. Use GitHub tools against `12dakota/OEM-Apps-Installer.msi` only for this product.
2. Keep extract path `C:\Windows\Apps`.
3. Deploy by pushing to `main` or dispatching `build-msi.yml`.
4. Confirm with the Actions run URL and the `OemApps-msi` artifact.
