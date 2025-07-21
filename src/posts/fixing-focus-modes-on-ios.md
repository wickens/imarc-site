---
title: "Fixing Focus Modes on iOS"
date: "2023-12-03T11:50:19Z"
permalink: "/2023/12/03/fixing-focus-modes-on-ios/"
slug: "fixing-focus-modes-on-ios"
categories:
  - "uncategorized"
wordpress_id: "2841"
layout: "post.njk"
excerpt: ""
---

When Apple introduced Focus _Modes_ and Focus _Filters_, I hoped it might finally mean the ability to cleanly separate work and personal life within iOS. Unfortunately, the system is flawed.

Let’s briefly recap on how it works. Focus Modes are an extension of “Do Not Disturb” which has been with iOS since version 2011 (if my memory is correct) and allow precise control of notifications for different scenarios. Apple allows users to create various focus modes for different activities. For each focus mode, you can control who and which apps are allowed to send notifications. Apple ships iOS with several default modes such as “Sleep”, “Work”, and “Reading”. For example, I might turn off Slack notifications in the “Personal” Focus Mode, and then schedule that to activate in the evenings. 

Focus filters on the other hand control which data is available inside apps and are linked to a particular Focus Mode. For example, for to-do list app that allow you to create different lists, you could use Focus Filters to hide your shopping list when the “Work” Focus Mode is activated, and then hide your work tasks while your “Personal” Focus Mode is activated. 

The problem with Apple’s implementation is that it always requires a particular Focus Mode to be activated. There is no way to apply Focus Filters or control notifications when no Focus Mode is activated. Additionally, Focus Modes and Focus Filters are literally _filters_. This means they can only reduce content that is already enabled. In my use case, I want to turn off my work email and calendars by default but have then enabled during work hours. I want to disable Slack Notifications all the time, except during work hours. This is simply not possible with iOS and represents a fundamental flaw in the design. I can of course disable Slack Notifications fully, but then I can’t enable them for a particular Focus Mode. I could also painstakingly configure focus modes to consume contiguous time blocks throughout the day, but this is error prone and there are times when a focus mode gets switched off for some reason and I’m inundated with notifications I don’t want. The Apple Watch is also unaware of Focus Filters and will happily show calendar appointments despite the same appointments being filtered out on my Phone.

I’m not quite sure how Apple missed the obvious on this one. Perhaps they are stuck dealing with technical debt and the current system is a compromise based on what is technically achievable. Perhaps they genuinely think the current system is adequate. Perhaps I’m the outlier. Either way, I hope they address this issue. Currently, iOS still makes it far too difficult to separate work and personal content.
