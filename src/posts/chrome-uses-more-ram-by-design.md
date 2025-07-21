---
title: "Chrome Uses More RAM by Design"
date: "2021-02-26T10:21:59Z"
permalink: "/2021/02/26/chrome-uses-more-ram-by-design/"
slug: "chrome-uses-more-ram-by-design"
categories:
  - "uncategorized"
wordpress_id: "1834"
layout: "post.njk"
excerpt: ""
---

Great [video by Matt](https://birchtree.me/blog/yes-chrome-uses-more-ram-than-safari-but-how-much-more/) that shows that Chrome isn't always _that_ different to Safari in its memory consumption as others [have found](https://www.flotato.com/post/memory-chrome-safari-flotato). Most of my daily collaboration is done using Microsoft Teams, which is an Electron app - [which is to say a web application bundled in its own Chromium wrapper so it can run like a desktop application](https://imarc.co.uk/2021/01/05/microsoft-is-replacing-the-outlook-app-with-a-web-app/). I've often noticed Microsoft Teams takes up _huge_ amounts of RAM, especially considering it's not really doing anything that complicated most of the time. A while back I researched phenomena and found this article about **[how Chromium manages memory](https://docs.microsoft.com/en-us/microsoftteams/teams-memory-usage-perf)** on Microsoft's site:

> Chromium detects how much system memory is available and utilizes enough of that memory to optimize the rendering experience. When other apps or services require system memory, Chromium gives up memory to those processes. Chromium tunes Teams memory usage on an ongoing basis in order to optimize Teams performance without impacting anything else currently running.  
> …  
> When computers have more memory, Teams will use that memory. In systems where memory is scarce, Teams will use less.

Put simply, Chrome is taking the idea that _'empty RAM is wasted RAM'_, and running with it. The article goes on to link to the specific documentation from the Chromium project.

This reminds me of when Windows Vista came out, complete with a [sidebar full of widgets](/wp-content/uploads/2021/02/windows-vista-sidebar.jpg). One of the default widgets showed CPU and RAM usage. At the time I was working in my local computer retailer, and remember customers being shocked that even with no applications running, Windows Vista was often using 40% of the available RAM, even on higher spec'd machines. Of course Windows was doing a smart thing by pre-loading applications likely to be used into memory so they could be launched faster, and so it was nothing to worry about.

Chrome's philosophy has long been that _it is an operating system for the web_ - it even has its own task manager. So it makes sense from that perspective to grab as much memory as possible, and use it to speed things up for users. For users who spend most of their day inside Chrome, it's probably doing them a favour. The problem is that these days we often end up running multiple instances of Chrome or Chromium derived apps like Slack or Teams competing alongside each other for system resources.

As Matt shows, the difference between Safari isn't always that much, but the important thing to remember is that Chrome's memory usage will depend on how much overall memory your system has _available_.
