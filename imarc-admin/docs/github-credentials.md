# GitHub credentials

The app uses git-over-SSH with a local clone. A GitHub PAT is not required for v1.

## Deploy key

1. Generate a dedicated key pair on the Pi:

```bash
ssh-keygen -t ed25519 -f /srv/imarc-admin/.ssh/imarc-site
```

2. Add the public key to `wickens/imarc-site` as a deploy key with write access.
3. Configure:

- `Admin:GitSshKeyPath=/srv/imarc-admin/.ssh/imarc-site`
- `Admin:BlogRepoRemote=git@github.com:wickens/imarc-site.git`

