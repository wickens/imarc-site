---
title: "A Podcast Archive Utility"
date: "2022-06-04T15:54:00Z"
permalink: "/2022/06/04/a-podcast-archive-utility/"
slug: "a-podcast-archive-utility"
categories:
  - "uncategorized"
wordpress_id: "2374"
layout: "post.njk"
excerpt: ""
---

I have been an avid podcast listener since it first emerged in the mid-2000s, in the past few years everyone who is anyone it seems now has a podcast. What was once the domain of nerdy introverts is now decidedly mainstream.

The beauty of podcasts is that like the web, they use (mostly) open standards. HTTP, RSS, XML and MP3. Like the web, they also disappear sometimes. In the case of the web, clicking links on pages written 10 years ago will inevitability result in frequent dead links, where a site’s owner has decided to move on. Go back another 10 years the the ratio of working links to 404 errors increases even more. Thankfully Archive.org can be used to recover many dead links on the web.

Like blogging before it, which was similarity open in spirit, economic realities will one day set in for podcasts too. Already there are shows I remember whose feeds no longer exists. The episodes are gone forever.

That's why I've written [Podcast Archive.](https://github.com/wickens/PodcastArchive) A simple Command-line utility written in .NET that downloads an entire podcast feed in a tidy Year > Month > Date folder structure. It's smart enough not to download files that have already been downloaded, so it's safe to run over and over again without wasting bandwidth. Files downloaded will have their creation date set to the original publish date, so once downloaded you can easily use tools like Spotlight to search by date.

![](/wp-content/uploads/2022/06/screenshot3.png?w=1024)

At the moment you'll need to compile the app using .NET 6, but I plan to add pre-compiled versions for Windows, Linux and Mac very soon.
