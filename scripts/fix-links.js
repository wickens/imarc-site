const fs = require("fs");
const path = require("path");
const ROOT = path.join(__dirname, "..", "src");
const OLD_DOMAIN = "https://imarc.co.uk";

function walk(dir) {
  for (const f of fs.readdirSync(dir)) {
    const p = path.join(dir, f);
    const s = fs.statSync(p);
    if (s.isDirectory()) walk(p);
    else if (p.endsWith(".md")) {
      let txt = fs.readFileSync(p, "utf8");
      const replaced = txt.replace(new RegExp(OLD_DOMAIN, "g"), "");
      if (replaced !== txt) {
        fs.writeFileSync(p, replaced);
        console.log("Fixed links in", p);
      }
    }
  }
}
walk(ROOT);