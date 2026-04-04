# iMarc Admin

`iMarc Admin` is a Blazor Server admin UI for editing `wickens/imarc-site`.

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

`AllowLocalBypass` is enabled in development so Cloudflare headers are not required on localhost.
`PushChangesToRemote` is disabled in development, so saving a post creates a local git commit in the managed clone without pushing to GitHub.
The posts list is cached briefly in memory to keep the UI snappy; use the refresh button in the app if you want an immediate rescan.

## Verification

Run the lightweight offline spec runner:

```bash
dotnet run --project tests/ImarcAdmin.Specs/ImarcAdmin.Specs.csproj
```

Then work through the manual deployment checks in `docs/testing.md`.

## Deployment

Use a machine with the .NET 10 SDK installed and publish with one of the provided profiles:

```bash
dotnet publish src/ImarcAdmin/ImarcAdmin.csproj -p:PublishProfile=LinuxArm64FrameworkDependent
```

Then follow:

- `docs/pi-setup.md`
- `docs/dns-setup.md`
- `docs/cloudflare-setup.md`
- `docs/github-credentials.md`
- `docs/testing.md`

## Notes

- The checked-in project targets `net7.0` in this workspace because that is the installed SDK available for local validation here.
- The publish profiles target `net10.0` for the Raspberry Pi deployment described in the plan.
- The managed blog clone must stay clean between publishes; the app will refuse to operate against a dirty clone.
- Existing posts can be deleted from the editor; in development that delete is committed locally without pushing.
- The editor shows live body and excerpt word counts while you write.
