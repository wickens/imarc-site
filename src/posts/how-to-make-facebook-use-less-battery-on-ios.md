---
title: "How to make Facebook use less battery on iOS"
date: "2016-03-26T20:21:04Z"
slug: "how-to-make-facebook-use-less-battery-on-ios"
layout: "post.njk"
categories: ["Uncategorized"]
excerpt: "I’m not a heavy Facebook user, I’ll check it a couple of times a day, perhaps a bit more if I’m on holiday and using it to check into places. For this reason I have the ‘Background App Refresh’ option turned off in iOS"
permalink: "/2016/03/26/how-to-make-facebook-use-less-battery-on-ios/"
original_url: "https://www.lostinlogic.com/2016/03/26/how-to-make-facebook-use-less-battery-on-ios/index.html"
---

I’m not a heavy Facebook user, I’ll check it a couple of times a day, perhaps a bit more if I’m on holiday and using it to check into places. For this reason I have the ‘Background App Refresh’ option turned off in iOS settings (under Settings > General). Despite this, when I looked at what was using up all of my battery, I was surprised to find that the biggest offender was not only Facebook, but it was background activity as well!

I suspected that the app was receiving lots of ‘silent notifications’ which can cause the app to wake up and start fetching data in the background, regardless of the ‘Background App Refresh’ setting. To counter this, I decided to uninstall Facebook, wait 24 hours and then reinstall it. Upon launching it for the first time after reinstalling, it asked if I wanted to receive notifications. This time I chose “Don’t Allow”. This means the Facebook app won’t get access to a unique token enabling it to send those silent notifications which I guessed were causing all of this background activity.

Sure enough Facebook is now reportedly using less battery, and none of it is background usage. Waiting 24 hours between deleting the app and reinsalling it is important because otherwise iOS will simply remember your previous notification settings, and not ask you if you’d like to allow them again.

This is based purely on my anecdotal usage, I’d love to know the actual machinism that causes this improvement
