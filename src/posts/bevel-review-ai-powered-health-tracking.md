---
title: "Bevel Review: AI Powered Health Tracking"
date: "2026-01-31T18:51:53Z"
slug: "bevel-review---ai-powered-health-tracking"
layout: "post.njk"
categories: []
---




 ![My Recovery Score](/static/img/bevel-watch.png)

The Apple Watch has been out for over a decade now, and yet its health and fitness insights have only improved marginally. GPS brought maps of your running routes, while more accurate accelerometers and gyroscopes enabled more in-depth running analysis. Better heart rate sensors allow for ongoing heart monitoring, and the addition of a thermometer helps with sleep and menstrual cycle tracking. There have been other improvements, admittedly, but the Health app itself has mostly just accumulated more charts.

Bevel aims to augment Apple’s base offering with its own app that analyses and interprets data from an Apple Watch, or any fitness tracker that writes to Apple Health, and provides actual recommendations, commentary, and more actionable insights. At least, that is the pitch.

The key tenets of the app are the scores it provides for “Strain”, “Recovery”, “Sleep”, and “Stress”. The app is often compared to the Whoop fitness tracker, as both provide similar functionality[^1]. Strain is a measure of how much you have put your body through on any given day. Unlike Apple’s Activity metric, it does not just look at movement and heart rate. It also factors in the toll your overall stress level takes on your body, as well as your ability to cope with that stress based on how well you recovered from the previous day, hence the Recovery and Sleep scores. Sleep takes into account how much time you spend in each sleep stage, not just total time asleep, along with wake-ups and bedtime consistency. Recovery looks at how quickly your nervous system calms down after stress and exercise. Stress uses resting heart rate and heart rate variability to estimate how stressed you are.

Overall, I have found the scores to be pretty helpful. It is important to take them in context. Sleep tracking using a device on your wrist is far from perfect, so if I wake up feeling great but Bevel says I had a bad night, I am not going to lose any sleep over it. Where it does help is in showing the impact of lifestyle choices on things like sleep and stress. Having a single pint of 4% IPA at lunchtime led to my HRV spiking for the next 24 hours and reduced the amount of deep sleep I got that night. Training too hard by pushing myself on runs too close together also spikes my stress levels. I more or less knew this already, but seeing the data as evidence, and seeing when positive changes make a difference, is genuinely useful. The stress metric is also much simpler than trying to work what your HRV means. As someone who has a tendency to over train and over work while not paying attention to my what my body is telling me, having a large stress score a tap away is good way to know when to skip that run or log off early.


And of course, it would not be 2026 if the app did not include a conversational interface courtesy of an LLM. The chat feature is actually very good. As someone who has worked in AI for the past decade, I cannot imagine that even three years ago we would have had a chatbot capable of making such accurate and useful insights about my own health. I can ask whether I should attempt a 10-mile run tomorrow or hold off for another day, and it will look at my scores, workout history, and other data to make a data-driven recommendation. It is not perfect. Like many LLM-based systems, it keeps a memory of things you say that it considers significant. This data is then fed into subsequent prompts, allowing an otherwise stateless model to learn about you over time. This can lead to amusing results when it overplays the importance or timing of something it has stored, such as when it assumed a beer I had at lunchtime on a Saturday was consumed at 6am on Sunday morning before a 10-mile run. Still, while many tech companies and LinkedIn influencers talk up so-called “agentic” software, Bevel is actually doing it, and it is providing real value. The LLM is cloud-based, probably OpenAI, Google Gemini, or Claude Sonnet, and Bevel state that they do not bulk-share your health data. Instead, they appear to send limited snippets as part of the prompt.

Bevel also supports logging food and drink, allowing you to correlate this with your scores and see how they are affected. For example, log a cup of tea at 5pm and you will probably see your sleep score drop noticeably. I have not really used this feature properly. For me, life is too short to estimate food intake precisely enough to be genuinely useful, although I can see how this would be worthwhile for some people.

Finally, there is the cost. It is £50 a year for the features mentioned above. Compared to the competition, namely Whoop or possibly the Oura Ring, that is very good value, assuming you already own an Apple Watch. The fact that you also get a capable smartwatch that does many other things, rather than a commodity fitness tracker or a very limited ring, makes this feel like a much better deal. It is also widely expected that Apple will introduce its own AI-based offering at some point in the near future.

Overall, I am very pleased with it and will be subscribing for a year.

[^1]: As an aside, I have noticed Whoop becoming increasingly popular recently, which shows where Fitbit could have gone. It is essentially a Fitbit for the 2020s. I have even noticed people wearing both a Whoop and an Apple Watch. This really should not be necessary and is the primary reason I sought out a software-based solution.
