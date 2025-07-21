---
title: "Microsoft Is Replacing Outlook With a Web App"
date: "2021-01-05T20:51:00Z"
permalink: "/2021/01/05/microsoft-is-replacing-the-outlook-app-with-a-web-app/"
slug: "microsoft-is-replacing-the-outlook-app-with-a-web-app"
categories:
  - "uncategorized"
wordpress_id: "1573"
layout: "post.njk"
excerpt: ""
---

According to [Windows Central](https://www.windowscentral.com/project-monarch-outlook-web-universal-email-client-microsoft), Microsoft is planning to replace the aging native Outlook apps for both Windows and Mac with a [Chromium-wrapped HTML app](https://www.electronjs.org/). I came across this link thanks to John Gruber, whose [take on it](https://daringfireball.net/linked/2021/01/04/bowden-outlook) is unsurprisingly negative - within the Mac community there is a definite preference for native over web software over web apps.

While I am inclined to agree - native software can often be superior to web software, it really depends on the UI framework being used. Outlook for Windows has a codebase that dates back to at least 1997, and so is likely using the Microsoft Foundation Classes, which themselves date back to 1992. Age itself is not the issue - Apple's own Cocoa framework derives much from early 90s NeXTSTEP technologies. The difference is Apple have constantly updated Cocoa, whereas Microsoft have instead introduced a series of new frameworks that never took off ([here](https://en.wikipedia.org/wiki/Windows_Forms), [here](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation) and [here](https://en.wikipedia.org/wiki/Universal_Windows_Platform_apps)) and using them would have meant rewriting much of the Outlook codebase. This means the latest version of Outlook in 2021 still operates very much like a piece of software from the mid-2000s. Searching a few thousand emails causes Outlook to stop responding on a latest generate Intel i7.

So given where Microsoft are, with no decent, modern and mature UI framework for Windows, what are the Outlook team supposed to do? Their biggest competitors are Gmail, Slack and Microsoft's own Teams - all web apps. They could spend the next 3 years rewriting the native app in a new UI framework, or they could utilise the already feature rich web app and spend the next 3 years building competitive features. Chromium based apps are also slow, but they're faster than a poorly written native apps. It's a crappy situation, but I think they've made a pragmatic choice.

Of course if Outlook were Mac-only then it would be a no-brainier. The difficult choice would be between AppKit and UIKit.
