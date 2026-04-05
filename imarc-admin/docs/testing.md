# Testing

## Offline specs

Run the built-in spec runner:

```bash
dotnet run --project tests/ImarcAdmin.Specs/ImarcAdmin.Specs.csproj
```

It covers:

- slug generation
- permalink generation
- editor slug auto-generation rules for new versus existing posts
- front matter round-tripping with unknown key preservation
- front matter date/time round-tripping
- preview rewriting for staged uploads
- upload markdown snippet generation and file name collision handling
- post list caching and manual refresh behavior
- managed-clone refresh on load and stale-publish conflict blocking
- local-save behavior when remote push is disabled
- deleting an existing post

## Manual integration checks

Before these checks, make sure:

- `Admin:BlogRepoPath` points at a clean service-owned clone
- the Pi can push to GitHub with the configured deploy key
- Apache is reverse-proxying to `http://127.0.0.1:5005`
- Apache Basic Auth is enabled for `admin.imarc.co.uk`
- your WireGuard clients can reach the Pi's LAN IP

### Create a post

1. Connect to WireGuard.
2. Open `http://admin.imarc.co.uk`.
3. Confirm the browser prompts for Basic Auth credentials.
4. Enter the `htpasswd` username and password.
5. Create a new post.
6. Confirm the slug auto-fills from the title.
7. Confirm the permalink updates from the selected date.
8. Confirm the time field can be edited independently.
9. Confirm the editor shows a live word count while writing.
10. Publish.
11. Verify a new file appears under `src/posts/<slug>.md` in the managed clone.
12. In local development, verify the app creates a local git commit and does not push to GitHub.
13. In production, verify the app pushes to `main`.

### Edit a post

1. Open an existing post from `/posts`.
2. Change title, excerpt, and body.
3. Publish.
4. Verify the existing relative file path did not change.
5. Verify the slug and permalink did not change just because the title changed.
6. Verify any existing `wordpress_id` is still present in front matter.
7. Verify the chosen time is serialized into the front matter `date` value.

### Slug behavior

1. Create a new post.
2. Confirm the slug auto-fills from the title.
3. Manually edit the slug.
4. Continue editing the title.
5. Confirm the slug stays on your manual value.
6. Clear the slug field completely.
7. Change the title again.
8. Confirm the slug is regenerated from the new title.

### Delete a post

1. Open an existing post from `/posts`.
2. Tap `Delete post`.
3. Confirm the browser prompt.
4. Verify the app returns to the posts list.
5. Verify the file is removed from `src/posts`.
6. In local development, verify the delete is committed locally without a push.

### Upload images

1. Open a post editor on iPhone, iPad, or desktop.
2. Upload one or more images.
3. Confirm each upload appears in the uploaded images panel.
4. Tap `Copy Markdown` and paste into the editor.
5. Tap `Insert at Cursor` and confirm the snippet is inserted inline.
6. Confirm the generated markdown uses `/wp-content/uploads/YYYY/MM/file.ext`.
7. Publish and verify the files land in `uploads/YYYY/MM` inside the managed clone.

### Responsive checks

1. iPhone Safari portrait:
   ensure the metadata form stacks vertically, the preview toggle works, and the sticky action bar stays usable above the keyboard.
2. iPad Safari portrait and landscape:
   ensure editing, upload, and preview remain comfortable and touch friendly.
3. Desktop:
   ensure the wider editor layout, preview pane, and uploaded images panel remain usable together.

### Posts list cache

1. Open `/posts` and confirm the list appears quickly after the first load.
2. Make a repo change outside the app.
3. Confirm the posts list does not change until the cache expires or `Refresh list` is tapped.
4. Tap `Refresh list` and confirm the updated post set appears immediately.

### Security checks

1. Confirm `http://127.0.0.1:5005` is reachable locally on the Pi.
2. Confirm `/healthz` remains reachable for liveness checks.
3. Confirm `http://admin.imarc.co.uk` prompts for Basic Auth credentials.
4. On iPhone or iPad, browse around after logging in and note whether Safari still re-prompts after reconnects or tab restores; occasional repeats are expected with HTTP Basic Auth.
5. Confirm the app is reachable over WireGuard only after successful Basic Auth.
6. Confirm port `5005` is not exposed directly on the LAN or internet.

### Conflict handling

1. Open the same post in the editor.
2. Change the underlying repo `main` branch outside the app by pushing from another machine.
3. Open `/posts` or reload a different editor and confirm the newer remote content is picked up automatically.
4. Return to the stale editor and publish.
5. Confirm the app blocks publishing and asks for a reload.
6. Avoid making direct edits inside the Pi's managed clone; keep that clone clean and service-owned.
