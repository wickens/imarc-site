---
title: "Review: Outlook for iPad"
date: "2015-02-23T23:28:05Z"
slug: "outlook-for-ipad"
layout: "post.njk"
categories: []
---

Last month Microsoft released Outlook for iPad (based on Acompli, an app it previously purchased). Since the company I work for uses Exchange 2013, I was able to take advantage of this and try it out. The interface is a breath of fresh air for anyone like me who is stuck using Outlook 2013’s confusing and dated interface. My favourite feature is the ‘Focused’ inbox, which automatically shows you messages deemed important. Newsletters, alerts and other noise are quietly hidden away so you only see emails from real people. The ‘Other’ inbox is only a swipe away, and the focused view is just that—a view—so it won’t affect your desktop email layout. This is surprisingly accurate and didn’t require much training. Replying to and managing email is pleasant, with the ability to swipe to archive or flag messages quickly.

Outside the corporate world, the app supports Outlook.com, Gmail and other well-known providers. I like to keep work and personal email separate, so I haven’t tried these yet.

## Attachments

Another surprising feature of Outlook for iPad is the ability to connect to cloud services such as Dropbox and Google Drive[^1]. A lot of network administrators will lose sleep over this, but ultimately it’s a step forward—especially for Office 365 users, who can access all their OneDrive for Business files and attach them to emails wherever they happen to be.

A weak point, however, is the lack of a system extension, so you can’t share a link from Safari to Outlook or send a document directly from Word for iPad. I’m sure this is on the way, but I do think it should have been included in the initial release.

## Calendar

The calendar seems quite basic. It doesn’t do a great job of letting me see other invitees’ free/busy information (the main benefit of the desktop version of Outlook), but it’s serviceable for a version 1.0 release. It’s quite buggy; for example, I tried to update an appointment’s start and end times, but it just didn’t work. No crash, no error message—it simply did nothing. I’m sure Microsoft’s latest purchase, [Sunrise](http://blogs.microsoft.com/blog/2015/02/11/microsoft-acquires-sunrise-creator-innovative-calendar-app-mobile-devices/), indicates that the company is putting some thought into its calendaring strategy, so major improvements should follow. I’m not sure about the unified-app approach—I’ve always wished Outlook on the PC were separate applications instead of one big conglomerate (especially since it’s still full of modal dialog boxes!). Separate apps seem especially fitting for iOS, and I can only assume it’s a branding decision to have one big ‘Outlook’ app on iOS.

## Security concerns?

The first release had no security requirements at all, so if your system administrator mandated a passcode on your device, Outlook would ignore it. This has been resolved, though unfortunately it now requires you to set a system-wide PIN rather than just one for the app (as was the case with the previous OWA app). I liked that I could have laxer security on my personal device (for example, ‘ask me for a PIN after one hour’) while the app could be stricter (‘ask me for a PIN after five minutes’)—this worked in the old OWA app, but not anymore, which is a major disappointment. Some administrators might lament the fact that the app will store your emails on Amazon’s AWS servers (soon to be Azure, I would imagine), but this does allow the app to perform cloud processing that ultimately benefits users. The bigger concern, in my view, is that Microsoft released the app without warning or a way to block it, which surely upset organisations with established security practices (ISO et al).

## Conclusion

Overall, Outlook for iPad cements the iPad as a tool for business and makes me think that one day many users will be able to use an iPad (or similar device) exclusively at work. It’s missing some key features at the moment (you can’t set your Out of Office), but I’m certain they will arrive in time. The bigger question is whether tablet devices will ever replace traditional PCs in the workplace. That’s probably the subject of a future post, but with Outlook, Office and the cloud it’s becoming an increasing possibility. I personally use Outlook for iPad as more of a sidekick device than a laptop replacement—my job involves traditional desktop software such as Visual Studio and macro-enabled spreadsheets. That said, for many enterprise users an iPad with a decent hardware keyboard is now a viable alternative, if only the screen were larger.


[^1]: Great to see Microsoft embracing interoperability, in contrast to Google, which refuses to support Windows Phone.
