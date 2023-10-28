# MazeMaker

## Introduction

This is a good example of what happens when you are bored. Being inspired by **Deep Rock Galactic** I asked myself:

Wouldn't it be cool if we had procedurally generated maps in StarCraft Broodwar?

Yes, but at what cost?

## Technical dificulties

Maps are stored in a propietary format. I overcame this buy looking into a similar project.

This is based on the work of ShadowFlare who wrote this tool for editing MPQ. This DLL is needed, SFmpq.dll

[Shadowflare Github repo](https://github.com/ShadowFlare/WinMPQ/blob/master/SFmpqapi.bas)

[FaRTy1billion's MapSketch](http://www.staredit.net/topic/10328/)

[Wiki bout the chk file](http://www.staredit.net/wiki/index.php?title=Scenario.chk)

or here:
[another wiki](https://www.starcraftai.com/wiki/CHK_Format)

Now we are in the world of editing binaries. That is when you know you have gone too deep.

And digging into old VB6 souce code. Yay!


## The results

Some very crazy looking maps that are basically mazes.

![image](https://github.com/LorenzoAlfaro/MazeMaker/assets/58958983/5d5cf652-5ff7-4518-abe6-bd2908ddc54b)

![image](https://github.com/LorenzoAlfaro/MazeMaker/assets/58958983/f7b69a59-7111-415b-be65-524f1149dd1c)






## Installation

I'm hosting the installer for a ClickOnce application in my Azure Storage.
[Download installer here](https://deploymentapps.z22.web.core.windows.net/MazeMaker/publish.htm)


## Acknowledgements
FaRTy1billion from [startedit.net](http://www.staredit.net/)
and
Alberto Uriarte from https://www.starcraftai.com/

ShowdowFlare for her [library](https://sfsrealm.hopto.org/)
