---
title: "Iomega NAS Media Server Review"
date: "2010-03-11T17:39:00Z"
permalink: "/2010/03/11/from-the-archive-iomega-nas-media-server-review/"
slug: "from-the-archive-iomega-nas-media-server-review"
categories:
  - "uncategorized"
wordpress_id: "2873"
layout: "post.njk"
excerpt: ""
---

![](/wp-content/uploads/2024/04/iomega-nas.png?w=244)

I have a lot of media on my various computers: music, photos, videos, and the like, but I’ve never found a satisfactory way of storing them all. My solution was to put them on my main laptop, which has a large hard disk, and then share the folders to my other laptop and Xbox 360. This is fine, except I tend to reformat my laptop quite often, and I don’t keep it on all the time at a desk. Instead, it’s packed away in its bag when not in use, making playing some music from my Xbox a bit of a hassle. So I decided a NAS box was what I needed. My only consideration really was price; I really don’t care about speed since most of the time I’ll be using 802.11g to access the files, and so the Iomega 1TB drive is what I picked. It cost £120 from PC World, which I thought was good value. I’d read a few reviews, but since those reviews, Iomega has released a new firmware (which came preinstalled on mine), and so a lot of the drawbacks have been addressed.

I have to say, I am very impressed. The drive is fully accessible via SMB, meaning the software that comes with the drive is not essential (I only needed to install it once to find out what IP address my DHCP server had assigned it). The drive is actually a very small computer (probably running Linux, though I can’t be sure), and so it offers extra functionality. So-called “Live Folders” allow you to create a folder that’s contents get uploaded to Facebook, YouTube, or resized, for example. The NAS box can also act as a BitTorrent client, but unfortunately, it can’t be set to download a large file over FTP/HTTP. The device is also an iTunes server as well as a DLNA server (this is an open standard that the Xbox 360 uses to stream music and video over a LAN). If only TV shows purchased from iTunes didn’t have DRM, I’d buy a few, since being able to download a series of 24 straight to this device and watch it on my Xbox would be amazing. Still, having all your media always available via Media Centre/Xbox/iTunes is great.

The downsides to this device are that the hard disk appears to be FAT32 (according to another review I read) – this means it’s not journaled and in the event of a power cut, the drive/data could be corrupted. There is only one hard disk, and 1TB of data is a LOT of data to be held at the whim of someone spilling something on it or a cat knocking it over, so I will still be keeping the stuff I value most in two places. Anyone wanting a proper backup solution rather than a media server solution would be wise to get something that has RAID. It’s also quite noisy and doesn’t spin down the hard disk when it’s not in use.

Overall, I am pleased with this purchase and would recommend this media server to others looking for a good value solution.

## Update

I found the following information in the manual that states the drive actually uses the XFS file system, which was one of the first journaled file systems. Hooray!

> The Home Media Drive shares files using the SMB (Server Message Block) protocol and stores its files on XFS (built-in), FAT32 (external), and NTFS (external) hard drives.

I didn’t mention this before, but you can also plug in a USB drive and share it, or you can plug in a USB printer and then share that instead. Shame there’s no SSH access, however. :)
