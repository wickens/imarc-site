---
title: "Using Dropbox to synchronise existing folders"
date: "2011-11-05T13:57:48Z"
slug: "using-dropbox-to-synchronise-existing-folders"
layout: "post.njk"
categories: ["Uncategorized"]
tags: ["dropbox","windows"]
excerpt: "Dropbox is a great service at a decent price, it’s limited however by the fact that it only synchronises files stored inside your ‘Dropbox’ folder. To get around this limitation on Windows 7 here’s what you need to do"
permalink: "/2011/11/05/using-dropbox-to-synchronise-existing-folders/"
original_url: "https://www.lostinlogic.com/2011/11/05/using-dropbox-to-synchronise-existing-folders/index.html"
---

Dropbox is a great service at a decent price, it’s limited however by the fact that it only synchronises files stored inside your ‘Dropbox’ folder. To get around this limitation on Windows 7 here’s what you need to do (as with anything like this, always backup everything to an external drive first, usual disclaimers apply, follow these instructions at your own risk).

1.  Inside your Dropbox folder, create folders with the same name as the ones you already have on your hard disk, in my case “Music”, “Documents” and “Pictures”.
2.  _Move_ all your data from the existing folders into your Dropbox folder (they should start uploading to Dropbox) – Yes I said move, so make sure you backed.
3.  Delete the original folders (which will now be empty as you moved everything into your Dropbox)
4.  For each folder, create a [symbolic link](http://www.windows7home.net/how-to-create-symbolic-link-in-windows-7/) from your original location to the new one inside your Dropbox by running **mklink** from an administrative command prompt.
    Example:
    **mklink /D “C:UsersMarcDocuments”  “C:UsersMarcDropboxDocuments”**
5.  That’s it! When you setup a new PC, you will have to start from step 4 and all your documents, pictures and music will just appear.

_It will also work the other way round, creating a link from inside the Dropbox folder to your existing documents folder, however Dropbox only recognises changes when it gets restarted which defeats the point if you’re using it as a backup service and want to make use of it’s ‘Previous Versions’ feature._

Hope this might be of use to someone, and I hope Dropbox make doing this easier like Windows Live Mesh. Happy syncing!
