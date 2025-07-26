---
title: "Another SharePoint Security Flaw"
date: "2025-07-25T10:26:46Z"
slug: "another-sharepoint-security-flaw"
layout: "post.njk"
categories: []
---

[Ellen Jennings-Trace writing for Tech Radar](https://www.techradar.com/pro/security/microsoft-sharepoint-attack-now-sees-victim-count-rises-to-400-organizations-including-us-nuclear-agency):

> New estimates regarding the recently-exploited Microsoft SharePoint vulnerabilities now evaluate that as many as 400 organizations may have been targeted.
> 
> The figure is a sharp increase from the original count of around 100, with Microsoft pointing the finger at Chinese threat actors for the hacks, namely Linen Typhoon, Violet Typhoon, and Storm-2603.
> 
> The victims are primarily US based, and amongst these are some high value targets, including the National Nuclear Security Administration - the US agency responsible for maintaining and designing nuclear weapons, Bloomberg reports.

Microsoft [makes it clear](https://www.microsoft.com/en-us/security/blog/2025/07/22/disrupting-active-exploitation-of-on-premises-sharepoint-vulnerabilities/) this is an issue with on-prem instances of SharePoint, not the cloud based Office 365 solution.

One might question why an organisation would choose to run these services on premises in 2025. In my experience, banks and other security-focused institutions often believe their own teams can outperform Microsoft Azure, Google Cloud or AWS. Yet [time and time again](https://msrc.microsoft.com/blog/2021/03/guidance-for-responders-investigating-and-remediating-on-premises-exchange-server-vulnerabilities/?utm_source=chatgpt.com), we see on-prem is actually less secure than the cloud. Unless your service is complelty air-locked from the Internet, I see very few reasons to be relying on on-premisis software, especially Microsoft products, in 2025.
