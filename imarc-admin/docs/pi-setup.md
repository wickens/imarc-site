# Raspberry Pi setup

## Base OS

- Use a 64-bit Raspberry Pi OS / Debian 12+ image.
- Create a dedicated service account such as `imarc-admin`.
- Install:
  - `git`
  - `apache2`
  - `apache2-utils`
  - only if you are not using the self-contained single-file publish

## Directories

Suggested layout:

- `/srv/imarc-admin/app`
- `/srv/imarc-admin/temp`
- `/srv/imarc-admin/.net`
- `/srv/imarc-admin/.ssh`
- `/srv/imarc-site-clone`

## Publish shape

Preferred deployment for this app:

- `linux-arm64`
- self-contained
- single-file

That means the Pi does not need a system-wide .NET runtime for the app itself.
Deploy the whole publish folder, not just the executable, because the output also includes app settings and static web assets.

## systemd

Example service:

```ini
[Unit]
Description=iMarc Admin
After=network.target

[Service]
WorkingDirectory=/srv/imarc-admin/app
ExecStart=/srv/imarc-admin/app/ImarcAdmin
Restart=always
RestartSec=5
User=imarc-admin
Group=imarc-admin
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_BUNDLE_EXTRACT_BASE_DIR=/srv/imarc-admin/.net

[Install]
WantedBy=multi-user.target
```

Then:

```bash
sudo mkdir -p /srv/imarc-admin/.net
sudo chown -R imarc-admin:imarc-admin /srv/imarc-admin/.net
sudo systemctl daemon-reload
sudo systemctl enable imarc-admin
sudo systemctl start imarc-admin
```

This service should continue listening on `127.0.0.1:5005` only.
Expose it on your LAN through Apache instead of binding the app directly to a public interface.

## Updating an existing deployment

If the Pi is still running an older build that expects the Cloudflare header:

1. Publish the current repo state on your Mac.
2. Copy the publish output to a staging folder under your Pi user account, for example `/home/marc/imarc-admin-publish`.
3. Stop the service:

```bash
sudo systemctl stop imarc-admin
```

4. Replace the app files:

```bash
sudo rsync -av /home/marc/imarc-admin-publish/ /srv/imarc-admin/app/
sudo chown -R imarc-admin:imarc-admin /srv/imarc-admin/app
```

5. Start the service again:

```bash
sudo systemctl start imarc-admin
sudo systemctl status imarc-admin --no-pager
curl http://127.0.0.1:5005/healthz
```

Once that passes, continue with Apache Basic Auth setup.
