const { DateTime } = require("luxon");
const pluginRss = require("@11ty/eleventy-plugin-rss");

module.exports = function (eleventyConfig) {

    /* ---------------- Passthrough assets ---------------- */
    eleventyConfig.addPassthroughCopy({ "uploads": "wp-content/uploads" });
    eleventyConfig.addPassthroughCopy("static");                 // copies everything under static/*
    eleventyConfig.addPassthroughCopy({ "static/imarc-icon.png": "imarc-icon.png" });


    // eleventyConfig.addPassthroughCopy({ "uploads": "wp-content/uploads" });
    // eleventyConfig.addPassthroughCopy({ "static": "static" });
    /* favicon */
    // eleventyConfig.addPassthroughCopy({ "static/imarc-icon.png": "imarc-icon.png" });
    // eleventyConfig.addPassthroughCopy({ "static/js": "static/js" }); // or map to "js"

    /* Optional: watch so JS/CSS changes trigger reloads */
    eleventyConfig.addWatchTarget("static");

    /* ---------------- Plugins ---------------- */
    eleventyConfig.addPlugin(pluginRss);

  // Add a JSON stringify filter for templates
        eleventyConfig.addFilter("jsonify", function (value) {
            return JSON.stringify(value);
        });
    

    /* ---------------- Date filters ---------------- */
    function toDate(d) {
        return (d instanceof Date) ? d : new Date(d);
    }
    eleventyConfig.addFilter("isoDate", d => {
        if (!d) return "";
        return DateTime.fromJSDate(toDate(d)).toISO();
    });
    eleventyConfig.addFilter("year", d => DateTime.fromJSDate(toDate(d)).toFormat("yyyy"));
    eleventyConfig.addFilter("month", d => DateTime.fromJSDate(toDate(d)).toFormat("MM"));
    eleventyConfig.addFilter("day", d => DateTime.fromJSDate(toDate(d)).toFormat("dd"));
    eleventyConfig.addFilter("dateReadable", d =>
        DateTime.fromJSDate(toDate(d)).toFormat("d LLL yyyy")
    );
    // Alias so existing templates using | date keep working
    eleventyConfig.addFilter("date", d =>
        DateTime.fromJSDate(toDate(d)).toFormat("d LLL yyyy")
    );


    eleventyConfig.addFilter("truncate", function (str, n = 160, useWordBoundary = true, ellipsis = "…") {
        if (!str) return "";
        if (str.length <= n) return str;
        let sub = str.slice(0, n - ellipsis.length);
        if (useWordBoundary) sub = sub.replace(/\s+\S*$/, "");
        return sub + ellipsis;
    });


    // Image slider
    // before: "Before" label, after: "After" label
    eleventyConfig.addPairedShortcode("compare", function (content, before = "Before", after = "After") {
        // Grab the first two markdown image URLs inside the block
        const imgs = [...content.matchAll(/!\[[^\]]*\]\(([^)]+)\)/g)].map(m => m[1]);
        if (imgs.length < 2) return content; // fallback if not two images

        return `
<div class="compare" data-before-label="${before}" data-after-label="${after}">
  <img src="${imgs[0]}" alt="${before}" data-compare="before">
  <img src="${imgs[1]}" alt="${after}" data-compare="after">
</div>
`;
    });

    /* ---------------- Collections ---------------- */
    eleventyConfig.addCollection("posts", collection => {
        return collection.getFilteredByGlob("src/posts/**/*.md")
            .sort((a, b) => b.date - a.date);
    });

    eleventyConfig.addCollection("pages", collection => {
        return collection.getFilteredByGlob("src/pages/**/*.md");
    });

    eleventyConfig.addCollection("tagList", collection => {
        const tagSet = new Set();
        collection.getFilteredByGlob("src/posts/**/*.md").forEach(item => {
            (item.data.tags || []).forEach(t => tagSet.add(t));
        });
        return [...tagSet].sort();
    });

    eleventyConfig.addCollection("categoryList", collection => {
        const catSet = new Set();
        collection.getFilteredByGlob("src/posts/**/*.md").forEach(item => {
            (item.data.categories || []).forEach(c => catSet.add(c));
        });
        return [...catSet].sort();
    });

    /* ---------------- Helper filters for taxonomy pages ---------------- */
    eleventyConfig.addFilter("withTag", function (posts, tag) {
        return (posts || [])
            .filter(p => (p.data.tags || []).includes(tag))
            .sort((a, b) => b.date - a.date);
    });

    eleventyConfig.addFilter("withCategory", function (posts, category) {
        return (posts || [])
            .filter(p => (p.data.categories || []).includes(category))
            .sort((a, b) => b.date - a.date);
    });

    /* ---------------- Utility filters ---------------- */
    eleventyConfig.addFilter("limit", function (arr, n) {
        return (arr || []).slice(0, n);
    });

    eleventyConfig.addFilter("dump", obj => JSON.stringify(obj, null, 2));

    /* ---------------- Return base config ---------------- */
    return {
        dir: {
            input: "src",
            includes: "_includes",
            data: "_data",
            output: "_site"
        },
        markdownTemplateEngine: "njk",
        htmlTemplateEngine: "njk",
        dataTemplateEngine: "njk",
        templateFormats: ["md", "njk", "html"]
    };
};