---
title: "Get Android photos into iCloud Photostream via Dropbox"
date: "2012-11-05T22:23:23Z"
slug: "get-android-photos-into-icloud-photostream"
layout: "post.njk"
categories: ["apple","mobile","Software","Tech"]
tags: ["cloud computing","dropbox"]
excerpt: "If like me you have an Android phone but also have an iPad, then you might find that getting photos you take on the phone onto the iPad a chore. Having photos on the iPad means you can take advantage of the beautiful"
permalink: "/2012/11/05/get-android-photos-into-icloud-photostream/"
original_url: "https://www.lostinlogic.com/2012/11/05/get-android-photos-into-icloud-photostream/index.html"
---

If like me you have an Android phone but also have an iPad, then you might find that getting photos you take on the phone onto the iPad a chore. Having photos on the iPad means you can take advantage of the beautiful screen and powerful software like iPhoto for which there simply is no alternative for on Android.

Yes you can use the built in [Dropbox](https://www.dropbox.com/) app, but it’s very clunky and doesn’t let you sort by date. With this solution, photos you take on your Android phone will appear in your iCloud photo stream, which is a far more elegant solution.

**What we will do here is essentially make your Dropbox Camera Uploads folder the same folder as your iCloud PhotoStream uploads folder.**

_Note: I got this working by using my Windows 8 PC with Dropbox installed – it should work fine for Windows 7, and instructions will be different for Mac OS X but it should still be possible. Follow this guide at your own risk!_

#### Step 1: Install Dropbox on your phone

If you haven’t done so already, [install Dropbox](https://www.dropbox.com/) on your Android device and make sure automatic camera uploads is switched on. This will create a new folder in your Dropbox called ‘Camera Uploads’

#### Step 2: Install iCloud Control panel on your PC

I already had iTunes installed, so this was a case of running Apple software update and selecting ‘iCloud Control Panel’

#### Step 3: Remove the iCloud Uploads folder

Open the iCloud control panel and find the location of your photo stream on disk:

Kill the any iCloud.exe and ApplePhotoStream.exe processes from the task manager, and then browse to your photostream folder and delete the ‘Uploads’ folder.

#### Step 4: Recreate uploads folder

Now the magic happens, open an [administrative command prompt](https://www.google.co.uk/search?q=administrative+command+prompt&rlz=1C1CHFX_enGB507GB507&aq=f&oq=administrative+command+prompt&sugexp=chrome,mod=0&sourceid=chrome&ie=UTF-8) and create a _[junction](http://en.wikipedia.org/wiki/NTFS_junction_point)_ that will recreate the Uploads folder, only showing the contents of your Dropbox’s ‘Camera Uploads’ folder.

Example:

**mklink /J “C:UsersMarcDesktopPhotoStreamUploads”  “C:UsersMarcDropboxCamera Uploads”**

The first path is where I want the uploads folder to go, and second is where my camera uploads folder in Drobox is. Remember: you haven’t made to copies, if you delete from one, it gets deleted from the other.

##### Step 5: That’s it…!

Now any photo you take will be uploaded to your PC, from your PC it will be sent to iCloud. and from iCloud it will find its way onto your iOS devices.
