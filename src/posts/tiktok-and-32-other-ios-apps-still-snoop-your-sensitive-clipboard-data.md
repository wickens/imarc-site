---
title: "TikTok and 32 other iOS apps still snoop your sensitive clipboard data"
date: "2020-07-21T07:20:37Z"
permalink: "/2020/07/21/tiktok-and-32-other-ios-apps-still-snoop-your-sensitive-clipboard-data/"
slug: "tiktok-and-32-other-ios-apps-still-snoop-your-sensitive-clipboard-data"
categories:
  - "uncategorized"
wordpress_id: "253"
layout: "post.njk"
excerpt: ""
---

[Dan Goodin for Ars Technica](https://arstechnica.com/gadgets/2020/06/tiktok-and-53-other-ios-apps-still-snoop-your-sensitive-clipboard-data/):

> The privacy invasion is the result of the apps repeatedly reading any text that happens to reside in clipboards, which computers and other devices use to store data that has been cut or copied from things like password managers and email programs

I love that Apple have started to notify users when an app reads the clipboard without the user explicitly pressing 'Paste', however I worry that adding yet more warnings and notifications will result in users becoming fatigued and eventually ignoring them.

Instead, why not allow apps that need to store sensitive data on the clipboard (LastPass, for example) to set a specific data type indicating that the information should be classed as sensitive data - much like they can set the data type to text or image at the moment. In order to paste this "secure object", users would have to press the system 'Paste' button - any app just reading the clipboard by itself would just see it as empty.

This of course wouldn't solve for cases where data is not known to be sensitive - how many people store their passwords as a simple note instead of in a password manager, for example? Still, fewer notifications and stopping any app from reading passwords from the clipboard might be the right balance.
