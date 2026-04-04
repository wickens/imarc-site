# iMarc Site Agent Notes

This repository is Marc Wickens' personal blog at [www.imarc.co.uk](https://www.imarc.co.uk). It is a small Eleventy site with hand-authored Markdown content, a lightweight custom theme, a few utility templates, and a GitHub Actions deploy pipeline.

## What this site is

- Primary purpose: a personal technology blog with long-form posts, a simple chronological archive, a gear page, a projects page, and a few bespoke/static pages.
- Stack: Eleventy 3, Nunjucks templates, Markdown content, RSS plugin, Luxon date formatting, a little vanilla JS.
- Tone/design: deliberately simple, content-first, mostly single-column, minimal chrome, magenta accent, system-style typography with Fira Sans/Fira Mono loaded from Google Fonts.

## Source of truth

When changing the live site, the main source files are:

- `src/posts/*.md`: blog posts. These are the main content body of the site.
- `src/pages/*.md`: standalone pages such as `/gear/`, `/projects/`, `/riseley/`, and redirects/holding pages like `/about/`.
- `src/_includes/base.njk`: global page shell, header/footer, nav, search box, GoatCounter analytics, compare slider JS include.
- `src/_includes/post.njk`: post wrapper layout.
- `.eleventy.js`: collections, filters, passthrough copies, Markdown config, and the custom image-compare shortcode.
- `static/`: CSS, JS, favicon, and a standalone `aboutmarc.html` page that is copied through directly.
- `uploads/`: local copy of WordPress-era media, published as `/wp-content/uploads/...`.

Generated output should not be treated as source:

- `_site/` is the default local Eleventy output directory.
- GitHub Actions deploys from `public/`, not `_site/`, by invoking Eleventy with `--output=public`.

## Build and run

- `npm run dev`: runs Eleventy in serve mode.
- `npm run build`: builds to `_site/`.
- `npm run migrate`: converts the legacy WordPress XML export to Markdown.
- `npm run rebuild`: reruns migration, then rebuilds.

Dependencies show this is a mostly static build:

- `@11ty/eleventy`
- `@11ty/eleventy-plugin-rss`
- `date-fns`, `luxon`
- `fast-xml-parser`, `turndown`, `slugify`, `node-fetch`
- `markdown-it` with `markdown-it-footnote`

## Content model

Posts generally look like:

- `title`
- `date`
- `permalink`
- `slug`
- `categories`
- optional `tags`
- `layout: post.njk`
- `excerpt`

Pages generally use:

- `layout: base.njk`
- an explicit `permalink`
- long-form Markdown or inline HTML

The homepage (`src/index.njk`) paginates the `posts` collection 10 at a time and renders full post content, not just excerpts. The archive page (`src/archive.njk`) lists every post in reverse chronological order.

Current content footprint when this file was written:

- about 143 posts in `src/posts`
- 8 Markdown pages in `src/pages`

## Eleventy behavior

Important bits from `.eleventy.js`:

- Passthrough copy:
  - `uploads -> wp-content/uploads`
  - `static/ -> static/`
  - `static/imarc-icon.png -> /imarc-icon.png`
- Custom Markdown:
  - raw HTML enabled
  - linkify enabled
  - typographer enabled
  - footnotes restyled to avoid the default separator rule
- Collections:
  - `posts`
  - `pages`
  - `tagList`
  - `categoryList`
- Filters:
  - date helpers
  - `truncate`
  - `limit`
  - taxonomy helpers
  - JSON dump/stringify helpers
- Shortcode:
  - paired `{% compare %}` shortcode turns two Markdown images into a draggable before/after component

## Special pages and features

- `src/feed.njk`: RSS XML feed.
- `src/feed-redirect.njk`: HTML redirect from `/feed/` to `/feed/index.xml`.
- `src/sitemap.njk`: sitemap based on `collections.all`.
- `src/search/index.njk`: client-side search UI that fetches `/search/index.json`.
- `src/search-index.json.njk`: JSON search index.
- `src/404.njk`: custom 404 page.
- `src/pages/riseley.md`: a substantial image-heavy history page that exercises the compare shortcode heavily.
- `static/aboutmarc.html`: standalone static page outside the Eleventy template flow.

## Publishing flow

Deployment is handled by `.github/workflows/deploy.yml`.

- Trigger: push to `main` or manual dispatch.
- CI steps:
  - checkout
  - `npm ci`
  - `npx @11ty/eleventy --input=src --output=public`
  - deploy `public/` to GitHub Pages via `peaceiris/actions-gh-pages`
- Custom domain: `www.imarc.co.uk`

This means the content and templates in this repo are the publishing source; the GitHub Pages branch is a deploy artifact produced by automation.

## Legacy / migration context

This repo still carries clear WordPress migration history.

- `scripts/wordpress-export.xml` is the source export.
- `scripts/wp-export-to-md.js` converts published WordPress posts/pages into Markdown under `src/posts` and `src/pages`.
- The migration script also downloads referenced media into `uploads/`.
- The generated front matter structure in the current content largely comes from that script.

Because of that history, some pages are clearly transitional:

- `/about/` is now a move notice pointing to `marc.wickens.org.uk`.
- `/archives/` points readers to an older archived blog.
- some pages are sparse or clearly placeholders, such as `/test/`.

## Quirks worth knowing before editing

- Post pages link tags to `/tag/<tag>/`, but active tag archive generation does not currently appear to be enabled. There is an old `src/tags.njk.old`, suggesting taxonomy pages were started and then disabled.
- The search page expects an `excerpt` field in the fetched JSON, but `src/search-index.json.njk` currently only emits `title`, `url`, and `tags`. Search still works on titles, but excerpt scoring/rendering looks incomplete.
- Local builds default to `_site/`, while CI deploys `public/`. If a change depends on generated output, keep that split in mind.
- `_site/` is present in the repo, so it is easy to confuse generated output with maintainable source. Prefer editing `src/`, `static/`, `uploads/`, scripts, and config.
- Some migrated content has minor inconsistencies typical of automated import: empty excerpts, occasional odd typography/quotes, inconsistent filenames, and a few pages with legacy links.
- The WordPress export may contain more than just clean public post content; treat it as legacy data, not a file to casually reshape.

## Safe working assumptions for future agents

- Preserve the simple single-column presentation unless the task explicitly calls for redesign.
- Keep content edits in Markdown where possible rather than baking content into templates.
- Do not manually edit `_site/` or `public/` as source.
- If touching deployment, verify whether the intended output directory is `_site` or `public`.
- If improving search or tags, check both the data emitted by Eleventy and the template/runtime expectations.
- If re-running migration, expect it to overwrite generated Markdown because `OVERWRITE_EXISTING` is currently `true`.

## Practical mental model

Think of this repo as:

1. A Markdown-first personal blog in `src/posts`.
2. A thin Eleventy layer that adds layout, pagination, feed/search/sitemap output, and a custom compare component.
3. A repository with visible WordPress migration residue, so some rough edges are historical rather than architectural.
4. A GitHub Actions driven publishing setup where `main` is the authoring branch and GitHub Pages receives built output automatically.
