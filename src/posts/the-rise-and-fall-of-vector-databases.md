---
title: "The Rise and Fall of Vector Databases"
date: "2025-05-05T15:59:45Z"
permalink: "/2025/05/05/the-rise-and-fall-of-vector-databases/"
slug: "the-rise-and-fall-of-vector-databases"
categories:
  - "uncategorized"
wordpress_id: "2947"
layout: "post.njk"
excerpt: ""
---

[Jo Kristian Bergum on Twitter](https://twitter.com/jobergum/status/1872923872007217309):

> This surge was partly driven by a widespread misconception that embedding-based similarity search was the only viable method for retrieving context for LLMs. The resulting "vector database gold rush" saw massive investment and attention directed toward vector search infrastructure, even though traditional information retrieval techniques remained equally valuable for many RAG applications. … However, the landscape has evolved rapidly. What started as pure vector search engines now expand their capabilities to match traditional search functionality. Vector database providers have recognized that real-world applications often require more than just similarity search.

I’ve never quite understood the hype around using similarity search to retrieve content for a RAG system. Yes, cosine similarity can be useful in certain situations, but in many cases, there are better ways to find the right content. This is especially true when the user’s question has little semantic resemblance to the answer. Not to mention when you have potentially multiple versions of the same documents that may or may not be relevant depending on the context of the question. Now every major database supports vector search, is there even a need for a dedicated product? I’d recommend giving the latest episode of the [Latent Space](https://podcasts.apple.com/gb/podcast/latent-space-the-ai-engineer-podcast/id1674008350?i=1000705713887) podcast a listen where they explore these issues, and alternatives, in more depth.
