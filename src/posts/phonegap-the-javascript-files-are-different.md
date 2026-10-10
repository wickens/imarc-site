---
title: "PhoneGap: The JavaScript files are different!"
date: "2011-03-26T17:01:01Z"
slug: "phonegap-the-javascript-files-are-different"
layout: "post.njk"
categories: ["Code","Tech"]
tags: ["android","blackberry","iphone","mobile"]
excerpt: "I have recently been developing with PhoneGap, the cross platform development kit for mobile devices (essentially a browser window that lets you run HTML 5 web sites as an App)"
permalink: "/2011/03/26/phonegap-the-javascript-files-are-different/"
original_url: "https://www.lostinlogic.com/2011/03/26/phonegap-the-javascript-files-are-different/index.html"
---

I have recently been developing with **[PhoneGap](http://www.phonegap.com/)**, the cross platform development kit for mobile devices (essentially a browser window that lets you run HTML 5 web sites as an App)

I started off developing for iPhone and Android, using the packages provided by their download to get up and running. Great stuff. You write an app using HTML 5, JavaScript and CSS3 and then deploy to each device. The PhoneGap package contains a directory for each platform, with a number of files – the important one being **phonegap.0.9.4.js**.

So I developed the app on my Windows PC, copying the files across to my trusty Mac to deploy to iPhone. There are some subtle differences in the way Safari and Chrome render, so it was important to test across both devices. I kept coming across intermittent problems on the iPhone however, with the _deviceready_ event not firing. What was going on?

### phonegap.0.9.4.js

Due to the negligible amount of documentation available on PhoneGap’s site (Hey – I am used to having MSDN at my disposal!) I found myself digging through the sourcecode to work out how on earth I could do things. I noticed a lot of Android specific code, but no iPhone code. How odd. I then checked in my BlackBerry project folder and noticed the file was much bigger.

Yes, folks – the various _phonegap.0.9.4.js_ files – despite having the same filename are totally different for each OS implementation. Not once in the many hours spent reading PhoneGap’s web site did I see anything that told me that. Turned out my iPhone problems were because I was deploying an Android version of PhoneGap to the iPhone. Having the same filename is confusing and they seem destined to get used in place of one and other. I suspect many others will have tried to deploy the wrong file to the wrong device.

I would really like to see the folks at PhoneGap have a more sensible naming convention. e.g.  phonegap\_android.0.9.4.js? It would be apparent straight away the it was specific to that OS, and avoid what amounts to [DLL hell](http://en.wikipedia.org/wiki/DLL_hell) issues for mobile apps.

Of course a file is a file, and I can rename it to whatever I like (and probably will) – but it’s nice not to have to ([who renames jQuery?](https://github.com/jquery/jquery-mobile/issues/593)) and would certainly make learning the platform easier.
