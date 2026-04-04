# Cloudflare Tunnel and Access

## Tunnel

1. Install and authenticate `cloudflared`.
2. Create or select a tunnel.
3. Add a public hostname:
   - hostname: `admin.imarc.co.uk`
   - service: `http://127.0.0.1:5005`

## Access

1. In Cloudflare Zero Trust, create a self-hosted application for `admin.imarc.co.uk`.
2. Enable One-Time PIN as an identity provider.
3. Add an allow policy for your exact email address only.
4. Set the session duration you want, for example 24 hours.

The app verifies `Cf-Access-Authenticated-User-Email` against the configured admin email.

