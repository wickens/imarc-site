# DNS setup

`admin.imarc.co.uk` should resolve to the Raspberry Pi's stable LAN IP so it works for WireGuard clients that can already route your home LAN.

## Recommended path

1. Keep the zone managed in public DNS, for example Cloudflare DNS.
2. Create an `A` record for `admin.imarc.co.uk` that points to the Pi's LAN IP, such as `192.168.1.20`.
3. If you use Cloudflare DNS, keep the record `DNS only`, not proxied.
4. Confirm your WireGuard clients can already reach that LAN subnet through the tunnel.

Expected behavior:

- off VPN, the name may still resolve but the private IP will not be reachable
- on VPN, the name resolves to the same private IP and should work through Apache on the Pi
