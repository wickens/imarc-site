# Raspberry Pi setup

## Base OS

- Use a 64-bit Raspberry Pi OS / Debian 12+ image.
- Create a dedicated service account such as `imarc-admin`.
- Install:
  - `git`
  - `cloudflared`
  - `aspnetcore-runtime-10.0`

## Directories

Suggested layout:

- `/srv/imarc-admin/app`
- `/srv/imarc-admin/temp`
- `/srv/imarc-admin/.ssh`
- `/srv/imarc-site-clone`

## systemd

Example service:

```ini
[Unit]
Description=iMarc Admin
After=network.target

[Service]
WorkingDirectory=/srv/imarc-admin/app
ExecStart=/usr/bin/dotnet /srv/imarc-admin/app/ImarcAdmin.dll
Restart=always
User=imarc-admin
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Then:

```bash
sudo systemctl daemon-reload
sudo systemctl enable imarc-admin
sudo systemctl start imarc-admin
```

