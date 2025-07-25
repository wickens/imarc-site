// src/posts/posts.11tydata.js
const slugify = require("slugify");

module.exports = {
  eleventyComputed: {
    // no need to override `date` unless you want a fallback;
    // but if you do, coerce to Date here:
    date: data => {
      let d = data.page.date;
      return (d instanceof Date) ? d : new Date(d);
    },

    slug: data =>
      slugify(data.title || data.page.fileSlug, {
        lower: true,
        strict: true
      }),

    permalink: data => {
      // Make 100% sure `data.date` is a Date
      let d = (data.date instanceof Date) ? data.date : new Date(data.date);
      let year  = d.getFullYear();
      let month = String(d.getMonth() + 1).padStart(2, "0");
      let day   = String(d.getDate()).padStart(2, "0");

      return `/${year}/${month}/${day}/${data.slug}/`;
    }
  }
};