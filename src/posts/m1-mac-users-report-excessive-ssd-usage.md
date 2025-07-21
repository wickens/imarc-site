---
title: "‘M1 Mac Users Report Excessive SSD Usage’"
date: "2021-02-24T07:52:56Z"
permalink: "/2021/02/24/m1-mac-users-report-excessive-ssd-usage/"
slug: "m1-mac-users-report-excessive-ssd-usage"
categories:
  - "uncategorized"
wordpress_id: "1812"
layout: "post.njk"
excerpt: ""
---

From [9to5 Mac](https://9to5mac.com/2021/02/23/m1-mac-users-report-excessive-ssd-usage-potentially-affecting-the-components-lifespan/):

> Some advanced users have been reporting an overuse of the SSD for writing and reading data on the newly-released Macs with M1, Apple’s first computer chip based on ARM architecture. The issue could eventually affect the lifespan of the internal SSD used in M1 Macs — not to mention the machine itself.

My betting is on this being a bug in the software being used to measure SSD usage (a utility called **[smartmontools](https://formulae.brew.sh/formula/smartmontools)** that can be installed via [**Homebrew**](http://brew.sh)). SSD usage seems like something that should be pretty easy to automate testing for, and so for this to be a bug would be a major omission from Apple's test suite. [Not impossible of course](https://www.theguardian.com/technology/2017/nov/29/macos-high-sierra-bug-apple-mac-unlock-blank-password-security-flaw).
