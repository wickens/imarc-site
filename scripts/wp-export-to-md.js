/* eslint-disable no-console */
const fs = require("fs");
const path = require("path");
const { XMLParser } = require("fast-xml-parser");
const TurndownService = require("turndown");
const slugify = require("slugify");
const fetch = require("node-fetch");
const { format } = require("date-fns");

// ---------------- CONFIG ----------------
const XML_FILE = path.join(__dirname, "wordpress-export.xml");
const OUTPUT_POSTS = path.join(__dirname, "..", "src", "posts");
const OUTPUT_PAGES = path.join(__dirname, "..", "src", "pages");
const DOWNLOAD_MEDIA = true;
const MEDIA_ROOT = path.join(__dirname, "..", "uploads"); // becomes wp-content/uploads via passthrough
const ACCEPT_TYPES = new Set(["post", "page"]); // include only these wp post_types
const OVERWRITE_EXISTING = true; // set false to skip already generated md files
// ----------------------------------------

ensureDir(OUTPUT_POSTS);
ensureDir(OUTPUT_PAGES);
ensureDir(MEDIA_ROOT);

// Turndown (HTML -> Markdown) configuration
const turndown = new TurndownService({
  headingStyle: "atx",
  codeBlockStyle: "fenced",
  emDelimiter: "_"
});

// Helper: ensure directory exists
function ensureDir(p) {
  if (!fs.existsSync(p)) fs.mkdirSync(p, { recursive: true });
}


function rewriteMediaUrls(html) {
  if (!html) return html;
  // Recognise multiple possible hostnames that served your media
  const hosts = [
    "https://imarc.co.uk",
    "http://imarc.co.uk",
    "https://www.imarc.co.uk",
    "http://www.imarc.co.uk",
    "https://imarcme.wordpress.com",
    "http://imarcme.wordpress.com"
    // add any CDN host e.g. "https://cdn.oldsite.com"
  ];

  for (const h of hosts) {
    // Replace src="h/wp-content/uploads/..." and src='...'
    const re = new RegExp(`(<img[^>]+src=["'])${h.replace(/[-/\\.^$*+?()[\]{}|]/g, "\\$&")}(\\/wp-content\\/uploads\\/[^"']+)`, "gi");
    html = html.replace(re, (_m, prefix, pathPart) => `${prefix}${pathPart}`);
    // Also handle direct links in <a href="...">
    const reA = new RegExp(`(<a[^>]+href=["'])${h.replace(/[-/\\.^$*+?()[\]{}|]/g, "\\$&")}(\\/wp-content\\/uploads\\/[^"']+)`, "gi");
    html = html.replace(reA, (_m, prefix, pathPart) => `${prefix}${pathPart}`);
  }
  return html;
}

// Normalise / generate slug
function normaliseSlug(raw) {
  if (!raw) return "";
  return slugify(raw, { lower: true, strict: true });
}

// Convert WP date strings to ISO with T
function toIso(dateStr) {
  if (!dateStr || dateStr.startsWith("0000-00-00")) return null;
  // WP format: "YYYY-MM-DD HH:MM:SS"
  if (dateStr.includes(" ")) {
    return dateStr.replace(" ", "T") + "Z";
  }
  // If it already has T or Z just return as-is
  if (dateStr.endsWith("Z")) return dateStr;
  if (/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) {
    // date only
    return dateStr + "T00:00:00Z";
  }
  return dateStr + "Z";
}

function permalinkForPost(dateIso, slug) {
  const d = new Date(dateIso);
  if (Number.isNaN(d.getTime())) throw new Error("Invalid date for permalink: " + dateIso);
  const year = format(d, "yyyy");
  const month = format(d, "MM");
  const day = format(d, "dd");
  return `/${year}/${month}/${day}/${slug}/`;
}

// Front matter builder (always quoted scalars)
function frontMatter(obj) {
  function quote(v) {
    if (v === null || v === undefined) return '""';
    if (typeof v !== "string") return JSON.stringify(v);
    const escaped = v.replace(/"/g, '\\"');
    return `"${escaped}"`;
  }

  const lines = [];
  for (const [k, v] of Object.entries(obj)) {
    if (v === undefined || v === null) continue;
    if (Array.isArray(v)) {
      if (v.length === 0) continue;
      lines.push(`${k}:`);
      v.forEach(item => lines.push(`  - ${quote(String(item))}`));
    } else if (typeof v === "object") {
      lines.push(`${k}:`);
      for (const [sk, sv] of Object.entries(v)) {
        lines.push(`  ${sk}: ${quote(String(sv))}`);
      }
    } else {
      lines.push(`${k}: ${quote(String(v))}`);
    }
  }
  return `---\n${lines.join("\n")}\n---\n\n`;
}

// Preprocess raw HTML to replace common WP shortcodes / patterns
function preprocessHtml(html) {
  if (!html) return "";
  let out = html;

  // [caption ...]...[/caption] -> figure
  out = out.replace(/\[caption.*?\](<img [^>]+>)(.*?)\[\/caption\]/gis, (_m, img, cap) => {
    return `<figure>${img}<figcaption>${cap.trim()}</figcaption></figure>`;
  });

  // [gallery ids="1,2,3"]
  out = out.replace(/\[gallery.*?ids="([^"]+)"[^\]]*\]/gi, (_m, ids) => {
    const list = ids.split(",").map(id => `<img data-gallery-id="${id.trim()}" alt="">`).join("\n");
    return `<div class="gallery">\n${list}\n</div>`;
  });

  // Basic YouTube [embed]https://www.youtube.com/watch?v=XXXX[/embed]
  out = out.replace(/\[embed\]\s*(https?:\/\/www\.youtube\.com\/watch\?v=[^[]+?)\s*\[\/embed\]/gi,
    (_m, url) => {
      try {
        const u = new URL(url.trim());
        const vid = u.searchParams.get("v");
        if (!vid) return url;
        return `<iframe src="https://www.youtube.com/embed/${vid}" loading="lazy" allowfullscreen title="YouTube video"></iframe>`;
      } catch {
        return url;
      }
    });

  // Remove leftover empty shortcodes like [embed]...[/embed] that weren't YouTube
  out = out.replace(/\[embed\][\s\S]*?\[\/embed\]/gi, "");

  return out;
}

async function downloadFile(url) {
  try {
    const u = new URL(url);
    // Only handle uploads path
    if (!u.pathname.includes("/wp-content/uploads/")) return;
    const rel = u.pathname.replace(/^\/wp-content\/uploads\//, "");
    const destPath = path.join(MEDIA_ROOT, rel);
    ensureDir(path.dirname(destPath));
    if (fs.existsSync(destPath)) return; // already downloaded
    const res = await fetch(url);
    if (!res.ok) throw new Error("HTTP " + res.status);
    const buf = await res.buffer();
    fs.writeFileSync(destPath, buf);
    console.log("Downloaded media:", rel);
  } catch (e) {
    console.warn("Media download failed:", url, e.message);
  }
}

(async function main() {
  if (!fs.existsSync(XML_FILE)) {
    console.error("XML export not found at", XML_FILE);
    process.exit(1);
  }

  console.log("Reading XML:", XML_FILE);
  const xmlRaw = fs.readFileSync(XML_FILE, "utf8");
  const parser = new XMLParser({ ignoreAttributes: false, attributeNamePrefix: "" });
  const data = parser.parse(xmlRaw);

  if (!data?.rss?.channel?.item) {
    console.error("No <item> elements found in export.");
    process.exit(1);
  }

  const items = Array.isArray(data.rss.channel.item)
    ? data.rss.channel.item
    : [data.rss.channel.item];

  // Map id -> item for parent resolution
  const byId = new Map();
  items.forEach(it => {
    const id = it["wp:post_id"];
    if (id) byId.set(id, it);
  });

  // Pre-compute hierarchical page paths
  const pagePaths = {};
  function buildPagePath(item) {
    const id = item["wp:post_id"];
    if (pagePaths[id]) return pagePaths[id];
    const rawSlug = item["wp:post_name"] || item["wp:post_title"] || ("page-" + id);
    const slug = normaliseSlug(rawSlug);
    const parentId = item["wp:post_parent"];
    if (parentId && parentId !== "0" && byId.has(parentId)) {
      pagePaths[id] = buildPagePath(byId.get(parentId)) + slug + "/";
    } else {
      pagePaths[id] = `/${slug}/`;
    }
    return pagePaths[id];
  }

  let written = 0;

  for (const item of items) {
    const status = item["wp:status"];
    const type = item["wp:post_type"];
    if (status !== "publish") continue;
    if (!ACCEPT_TYPES.has(type)) continue;

    // Choose date: prefer GMT version
    const dateIso =
      toIso(item["wp:post_date_gmt"]) ||
      toIso(item["wp:post_date"]) ||
      new Date().toISOString();

    let slug = normaliseSlug(item["wp:post_name"] || item["wp:post_title"]);
    if (!slug) slug = "untitled-" + item["wp:post_id"];

    // Build taxonomies
    let categories = [];
    let tags = [];
    if (item.category) {
      const catsArr = Array.isArray(item.category) ? item.category : [item.category];
      catsArr.forEach(c => {
        if (!c) return;
        const domain = c.domain;
        const text = (c["#text"] || c).toString();
        if (domain === "category") categories.push(normaliseSlug(text));
        else if (domain === "post_tag") tags.push(normaliseSlug(text));
      });
    }
    categories = [...new Set(categories.filter(Boolean))];
    tags = [...new Set(tags.filter(Boolean))];

    // Raw HTML content
    const rawHtml = item["content:encoded"] || "";
    const processedHtml = rewriteMediaUrls(preprocessHtml(rawHtml));
    const markdownBody = turndown.turndown(processedHtml);

    // Permalink + output file
    let permalink;
    let outFile;

    if (type === "post") {
      try {
        permalink = permalinkForPost(dateIso, slug);
      } catch (e) {
        console.warn("Bad date for post, using today():", slug, dateIso);
        const nowIso = new Date().toISOString();
        permalink = permalinkForPost(nowIso, slug);
      }
      outFile = path.join(OUTPUT_POSTS, `${slug}.md`);
    } else {
      // page
      permalink = buildPagePath(item); // ends with /
      // Build nested directory structure
      const parts = permalink.split("/").filter(Boolean); // ['parent', 'child']
      const fileName = parts.pop();
      const dirPath = path.join(OUTPUT_PAGES, parts.join("/"));
      ensureDir(dirPath);
      outFile = path.join(dirPath, `${fileName}.md`);
    }

    if (!OVERWRITE_EXISTING && fs.existsSync(outFile)) {
      console.log("Skip existing", outFile);
      continue;
    }

    const excerptRaw = (item.description || "").replace(/\s+/g, " ").trim();
    const excerpt = excerptRaw.slice(0, 240);

    const fm = {
      title: item.title || slug,
      date: dateIso,
      permalink: permalink,
      slug: slug,
      categories,
      tags,
      wordpress_id: item["wp:post_id"],
      layout: (type === "post") ? "post.njk" : "base.njk",
      excerpt: excerpt
    };

    // Validate date parse
    if (Number.isNaN(Date.parse(fm.date))) {
      console.warn("Invalid ISO date for", slug, "->", fm.date, "using now()");
      fm.date = new Date().toISOString();
    }

    const content = frontMatter(fm) + markdownBody + "\n";
    ensureDir(path.dirname(outFile));
    fs.writeFileSync(outFile, content, "utf8");
    written++;

    // Download media referenced in processed HTML
    if (DOWNLOAD_MEDIA) {
      const imgUrls = Array.from(processedHtml.matchAll(/<img[^>]+src=["']([^"']+)["']/gi)).map(m => m[1]);
      for (const u of imgUrls) { // sequential to be gentle
        // eslint-disable-next-line no-await-in-loop
        await downloadFile(u);
      }
    }
  }

  console.log(`Done. Wrote ${written} markdown file(s).`);
})();