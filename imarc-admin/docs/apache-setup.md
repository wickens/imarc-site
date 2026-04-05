# Apache reverse proxy and Basic Auth

Use Apache on the Raspberry Pi as the only exposed web server for `imarc-admin`.
The app itself should stay bound to `127.0.0.1:5005`.

## Required modules

Enable:

- `proxy`
- `proxy_http`
- `proxy_wstunnel`
- `headers`
- `auth_basic`
- `authn_file`

On Debian or Raspberry Pi OS:

```bash
sudo a2enmod proxy proxy_http proxy_wstunnel headers auth_basic authn_file
sudo systemctl restart apache2
```

## Basic Auth credentials

Create a local password file with a single admin user:

```bash
sudo htpasswd -c /etc/apache2/.htpasswd-imarc-admin marc
sudo chown root:www-data /etc/apache2/.htpasswd-imarc-admin
sudo chmod 640 /etc/apache2/.htpasswd-imarc-admin
```

Use a different username if you prefer; Apache only needs a valid user in the file.

## Virtual host

Example vhost for `admin.imarc.co.uk` over plain HTTP:

```apache
<VirtualHost *:80>
    ServerName admin.imarc.co.uk

    <Location />
        AuthType Basic
        AuthName "iMarc Admin"
        AuthUserFile /etc/apache2/.htpasswd-imarc-admin
        Require valid-user
    </Location>

    ProxyPreserveHost On
    RequestHeader set X-Forwarded-Proto "http"

    ProxyPass "/_blazor"  "ws://127.0.0.1:5005/_blazor"
    ProxyPassReverse "/_blazor"  "ws://127.0.0.1:5005/_blazor"

    ProxyPass "/" "http://127.0.0.1:5005/"
    ProxyPassReverse "/" "http://127.0.0.1:5005/"
</VirtualHost>
```

Save it as `/etc/apache2/sites-available/imarc-admin.conf`, then enable it:

```bash
sudo a2ensite imarc-admin
sudo systemctl reload apache2
```

Keep the Basic Auth scope stable for the whole host. The single `AuthName`, `AuthUserFile`, and `Require valid-user` block above should apply to both `/` and the Blazor WebSocket endpoint under `/_blazor`; avoid mixing multiple realms or per-path auth rules for this host.

If the default Apache site is still serving the Pi on port 80, disable it:

```bash
sudo a2dissite 000-default
sudo systemctl reload apache2
```

## Cutover

After the new `imarc-admin` build is deployed and healthy on `127.0.0.1:5005`:

1. Create the `htpasswd` file.
2. Enable the Apache modules.
3. Add the vhost.
4. Reload Apache.
5. Visit `http://admin.imarc.co.uk` while on WireGuard.
6. Confirm the browser shows a Basic Auth prompt before loading the app.

## Network model

- `imarc-admin` listens only on `127.0.0.1:5005`
- Apache listens on the Pi's LAN IP
- WireGuard clients reach Apache using `http://admin.imarc.co.uk`
- Apache Basic Auth is the only application-facing auth layer
- no direct access to port `5005` should be allowed from the LAN or internet

## iPhone and iPad note

Safari on iOS and iPadOS can still re-prompt for HTTP Basic Auth more often than desktop browsers, especially after reconnects, tab restores, or Blazor WebSocket renegotiation. The vhost above keeps the auth realm stable to avoid unnecessary extra challenges, but some repeated prompts are still a browser limitation rather than an app bug.

If the iOS experience becomes too annoying, the next step should be changing the front-door auth approach, not further patching the app. In practice that means either app-level auth or a different reverse-proxy auth mechanism.
