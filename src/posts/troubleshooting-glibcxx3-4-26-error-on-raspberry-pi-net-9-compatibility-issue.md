---
title: "Troubleshooting GLIBCXX_3.4.26 Error on Raspberry Pi: .NET 9 Compatibility Issue"
date: "2024-08-15T19:37:54Z"
permalink: "/2024/08/15/troubleshooting-glibcxx3426-error-on-raspberry-pi-net-9-compatibility-issue/"
slug: "troubleshooting-glibcxx3426-error-on-raspberry-pi-net-9-compatibility-issue"
categories:
  - "uncategorized"
wordpress_id: "2893"
layout: "post.njk"
excerpt: ""
---

![](/wp-content/uploads/2024/08/error_glibcxx-.png?w=1024)

I was recently trying to compile a .NET project as a self-contained binary for `linux-arm` to run on my Raspberry Pi. When trying to run the binary, I kept getting the following error:

```
/usr/lib/arm-linux-gnueabihf/libstdc++.so.6: version GLIBCXX_3.4.26' not found (required by ./[executable])
```

It turned out the issue was caused by using .NET 9. **Downgrading the project to .NET 8 resolved it**. I'm unsure whether Microsoft or Raspbian needs to address compatibility with .NET 9, but as it didn't impact my code, it wasn't a problem for me.
