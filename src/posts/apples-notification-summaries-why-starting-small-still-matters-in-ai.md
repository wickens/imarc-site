---
title: "Apple’s Notification Summaries: Why Starting Small Still Matters in AI"
date: "2024-12-19T21:42:07Z"
permalink: "/2024/12/19/apples-notification-summaries-why-starting-small-still-matters-in-ai/"
slug: "apples-notification-summaries-why-starting-small-still-matters-in-ai"
categories:
  - "uncategorized"
tags:
  - "ai"
  - "apple"
  - "ios"
  - "iphone"
  - "technology"
wordpress_id: "2935"
layout: "post.njk"
excerpt: ""
---

![A iOS notification summary from BBC News that reads Luigi Mangione shoots himself; Syrian mother hopes Assad pays the price; South Korea police raid Yoon Suk Yeol's office."](/wp-content/uploads/2024/12/e453dc00-bdeb-11ef-a2ca-e99d0c9a24e3.jpg.png?w=1024)

_Image source: [BBC News](https://www.bbc.co.uk/news/articles/cx2v778x85yo)_

[Graham Fraser reporting for BBC News](https://www.bbc.co.uk/news/articles/cx2v778x85yo) about Apple's new notification summarisation feature:

> A major journalism body has urged Apple to scrap its new generative AI feature after it created a misleading headline about a high-profile killing in the United States.  
> The BBC made a complaint to the US tech giant after Apple Intelligence, which uses artificial intelligence (AI) to summarise and group together notifications, falsely created a headline about murder suspect Luigi Mangione.
> 
>   
> The AI-powered summary falsely made it appear that BBC News had published an article claiming Mangione, the man accused of the murder of healthcare insurance CEO Brian Thompson in New York, had shot himself. He has not.

I’ve worked on implementing AI into software products for the past seven years, and the first rule is always to start with a narrow domain, carefully assess how effective it is, and, if your approach is working, gradually expand your domain coverage. When it comes to notification summaries, I can’t help but wonder why Apple didn’t adopt this approach. Instead, they’ve delivered something that feels more like a hasty student project than the polished innovation we’ve come to expect.

Specifically, I would have started by limiting the product to summarising notifications where summaries are genuinely most useful. Of course, not being a product manager at Apple, I haven’t done the research, but let’s assume messaging apps would top the list. A "TL;DR" summary for lengthy WhatsApp group chats, for instance, could be genuinely valuable. On the other hand, attempting to summarise product promotions or delivery notifications from Amazon, breaking news, or even the alerts I get when my wife logs a feed on our baby-tracking app feels far less useful. Most of the complaints I’ve seen online seem to involve apps like BBC News, Amazon, or other non-communication apps. Apple would be better off avoiding attempts to summarise these types of notifications and instead focusing on apps where summarisation truly adds value: messaging apps such as WhatsApp or Slack.

That’s not to say the current implementation is flawless when it comes to messaging apps either. In an example shared by _WSJ_ technology journalist Joanna Stern, [the iPhone mistakenly assumed her wife was referring to a non-existent husband](https://www.threads.net/@joannastern/post/DCezF5pRbjM). The issue likely arose because Stern has her wife saved as “Wife” in her address book. The smaller language model onboard the iPhone, relying on statistical assumptions, concluded that someone called “Wife” must be referring to a husband when mentioning another unnamed person. It’s exactly the sort of edge case[1](#835b0802-a0c7-4b2f-8d5a-37d7a5a1be2d) that could have been caught more easily during testing if the first version had maintained a laser-like focus on summarising messaging app content.

By starting small and focusing on where summarisation adds the most value, Apple could have delivered a more refined and impactful feature, avoiding many of the issues we've seen reported .
