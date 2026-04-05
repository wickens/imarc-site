# iMarc Admin

`iMarc Admin` is a Blazor Server admin UI for editing `wickens/imarc-site`.

It assumes access is controlled outside the app, using Apache Basic Auth on the Pi plus WireGuard access to your home LAN.

It is designed to:

- list existing blog posts
- add new posts with generated front matter
- edit both publish date and publish time
- upload images and generate markdown snippets
- preview posts using the live site CSS
- publish directly to `main`, letting the existing GitHub Pages workflow deploy the site

## Local development

1. Create or use a separate clone of the blog repo, for example `/tmp/imarc-site-admin-dev-clone`.
2. Update `src/ImarcAdmin/appsettings.Development.json`.
3. Run:

```bash
dotnet run --project src/ImarcAdmin/ImarcAdmin.csproj
```

4. Open `http://127.0.0.1:5005`.

`PushChangesToRemote` is disabled in development, so saving a post creates a local git commit in the managed clone without pushing to GitHub.
The posts list is cached briefly in memory to keep the UI snappy; use the refresh button in the app if you want an immediate rescan.
The managed clone is refreshed from `origin/main` before the posts list loads, before a post is opened, and again before publish or delete. If `main` changes after you already opened an editor, the app will block the stale save and ask you to reload instead of silently overwriting newer work.

## Verification

Run the lightweight offline spec runner:

```bash
dotnet run --project tests/ImarcAdmin.Specs/ImarcAdmin.Specs.csproj
```

Then work through the manual deployment checks in `docs/testing.md`.

## Deployment

Use a machine with the .NET 10 SDK installed and publish the self-contained single-file Linux ARM64 build with:

```bash
dotnet publish src/ImarcAdmin/ImarcAdmin.csproj -p:PublishProfile=LinuxArm64SelfContained
```

Copy the entire publish output folder to the Pi, not just the `ImarcAdmin` executable, because the publish output also contains config files and static web assets.

Then follow:

- `docs/pi-setup.md`
- `docs/apache-setup.md`
- `docs/dns-setup.md`
- `docs/github-credentials.md`
- `docs/testing.md`

## Notes

- The checked-in project now targets `net10.0`.
- The app does not include built-in authentication; keep it behind Apache Basic Auth and your LAN/VPN boundary.
- The managed blog clone must stay clean between publishes; the app will refuse to operate against a dirty clone.
- Direct git changes pushed from another machine are picked up the next time the app lists posts, opens a post, publishes, or deletes. The app does not auto-refresh an editor that is already open.
- Existing posts can be deleted from the editor; in development that delete is committed locally without pushing.
- The editor shows live body and excerpt word counts while you write.
