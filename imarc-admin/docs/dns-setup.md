# DNS setup

`admin.imarc.co.uk` should be exposed through Cloudflare Tunnel, not by pointing a public A or AAAA record directly at the Pi.

## Recommended path

1. Keep the zone managed in Cloudflare DNS.
2. In the Tunnel configuration, add the public hostname `admin.imarc.co.uk`.
3. Let Cloudflare create and proxy the tunnel-backed DNS entry.

This keeps the Raspberry Pi private while still making the admin UI reachable on the public hostname.

