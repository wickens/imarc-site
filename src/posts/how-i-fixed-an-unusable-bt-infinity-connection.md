---
title: "How I fixed an unusable BT Infinity Connection"
date: "2016-02-28T19:02:50Z"
slug: "how-i-fixed-an-unusable-bt-infinity-connection"
layout: "post.njk"
categories: ["Tech"]
tags: ["tips","troubleshooting"]
excerpt: "I’d been having problems with BT Infinity for a few months. Every so often web pages would just hang, or streaming video would freeze. Oddly it would often coincide with a new certain devices connecting to the network."
permalink: "/2016/02/28/how-i-fixed-an-unusable-bt-infinity-connection/"
original_url: "https://www.lostinlogic.com/2016/02/28/how-i-fixed-an-unusable-bt-infinity-connection/index.html"
---

I’d been having problems with BT Infinity for a few months. Every so often web pages would just hang, or streaming video would freeze. Oddly it would often coincide with a new certain devices connecting to the network. Frustratingly, my connecting to Wi-Fi with my work laptop (Dell Latitude, Windows 10) would cause the Internet to stop working for a good half an hour.

I phoned BT and found their helpline very unhelpful. When asked to unplug the router overnight, I asked why this was needed and was told ‘it’s technical’. Throughout the process I felt as though I was just being read a script and not being listened to.

Anyway, I had managed to get my work laptop connected and had hooked into my company’s VPN when all the devices in the house stopped working again. My phone, the Apple TV, Kindles – all except for my work laptop. How odd. When I disconnected from the VPN, it too started to not work.

This made me think to try using [Google’ DNS](https://developers.google.com/speed/public-dns/) instead of BT’s. To my surprise, the Internet started behaving like a 70Mb/sec Internet connection should for the first time in months. My next step was to log into the painfully slow BT HomeHub router to try and change its DNS settings at the network level, rather than for each and every device. It turns out BT have restricted that, because normal people can’t be trusted to change their DNS servers, it seems.

After doing some digging, I discovered that the Apple Airport Express base station I’d been using to extend the range of the wireless network could be used as a NAT bridge and in place of the BT Homehub. I found this helpful post on the [BT Forums](https://community.bt.com/t5/BT-Infinity-Speed-Connection/Airport-Extreme/m-p/504189/highlight/true#M43691) which I will recap here incase BT decide to shut down or move their forums.

In short, you plug in the Ethernet cable from the white BT Openreach box into the WAN port on the Airport. Your username can be anything @btbroadband.com (I’m sure BT don’t rely on this for actual authentication, that’s tied to your line) and your password is simply a space.

You can then use Google’s DNS servers in place of BTs. One extra thing I had to do was set IPv6 to be ‘local link only’ under ‘Internet Options’.

![ss1](/wp-content/uploads/lostinlogic/2016/02/how-i-fixed-an-unusable-bt-infinity-connection-1.png)

![ss2](/wp-content/uploads/lostinlogic/2016/02/how-i-fixed-an-unusable-bt-infinity-connection-2.png)

This setup allowed me to retire my BT HomeHub for good, and the Internet connection has been flawless ever since. My Kindle can even connect, which is saying something.

Oddly, the default IP range for the Airport’s DHCP server is Class A, which means your devices won’t have the usual ‘192.168.1.X’ scheme, but this can be changed if needed. You can change this under the Network > Network options tab, though there really is no need other than it being a more common practice.
