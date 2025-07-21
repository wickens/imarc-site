---
title: "Making Siri Timers More Timely"
date: "2021-02-01T21:06:05Z"
permalink: "/2021/02/01/making-siri-timers-more-timely/"
slug: "making-siri-timers-more-timely"
categories:
  - "uncategorized"
wordpress_id: "1682"
layout: "post.njk"
excerpt: ""
---

![](/wp-content/uploads/2021/02/siri-icon.jpg?w=1024)

I can be a perfectionist when it comes to software, and one thing that bugs me when using Siri timers is the fact that the time taken for Siri to understand me is added on to the timer duration.

Frequently I will start a timer using the "Hey Siri" wake word, either using my Apple Watch or iPhone. Depending on where I am in the house and general WiFi signal, the time it takes for Siri to respond can range from a few seconds to 10 or more.

From the time between saying "Hey Siri" and completing the sentence "Set a timer for 5 minutes." the device will upload the audio to a server, which will use a set of speech-to-text machine learning models to predict the words said. It will then use a natural language model to classify this text into an intent, deriving meaning from the sentence.

In many cases, this results in my 5 minute timer not going off 5 minutes after I decided I wanted to start a timer, but 5 minutes and 7 seconds after.

It doesn't have to be this way however. The device presumably knows the exact time it successfully detected a "Hey Siri" command (a [complex set of systems in their own right](https://machinelearning.apple.com/research/hey-siri)), and so it could simply deduct this from the requested timer duration. If Siri takes 7 seconds to figure out what I meant, then my 5 minute timer becomes a 4 minutes and 53 seconds timer.

A small request, but if any company could pay attention to small features like this, it's Apple.
