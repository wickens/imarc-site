const fs = require("fs");
const path = require("path");
const { XMLParser } = require("fast-xml-parser");
const fetch = require("node-fetch");

(async () => {
  const xml = fs.readFileSync(path.join(__dirname, "wordpress-export.xml"), "utf8");
  const parser = new XMLParser({ ignoreAttributes: false });
  const data = parser.parse(xml);
  const items = data.rss.channel.item;
  const expected = [];
  items.forEach(it => {
    if (it["wp:status"] !== "publish") return;
    const type = it["wp:post_type"];
    if (!["post", "page"].includes(type)) return;
    const slug = it["wp:post_name"] || it.title;
    const date = new Date(it["wp:post_date"]);
    if (type === "post") {
      const y = date.getFullYear();
      const m = String(date.getMonth() + 1).padStart(2, "0");
      const d = String(date.getDate()).padStart(2, "0");
      expected.push(`/${y}/${m}/${d}/${slug}/`);
    } else {
      // For pages you might replicate logic used earlier
      // Simplified here
      expected.push(`/${slug}/`);
    }
  });

  let failures = 0;
  for (const url of expected) {
    const res = await fetch("http://localhost:8080" + url);
    if (res.status !== 200) {
      console.log("Missing:", url, res.status);
      failures++;
    }
  }
  console.log("Checked", expected.length, "URLs. Failures:", failures);
})();