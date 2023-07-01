# JoanasIncursionSiteTimer
Simple Windows Forms App to track respawning timers of Incursion HQ Sites  
  
Using [tesseract](https://github.com/tesseract-ocr) to identify text from an image, which is taken every 0.5s in a setup area on the screen.   
To setup, hit the setup button. Then click twice on points on your screen outside the form. 
A rectangle is "drawn" from those two points (think top left & bottom right corner) forming the capture area.  
Will pause and hint if the mouse pointer is inside the capture area.  
If there are changes to the the amount of sites bigger than 1 (e.g. area is covered by other window), no action is taken.  
If there is a change of site amounts equal -1 (site despawns), a 7:15min timer is started and displayed in the listview, aswell as a ding sound will be played.
Timer is removed after expiry. 

How it looks like: 

![Example](https://i.imgur.com/CEje0E1.png)


Things to improve: 
* There is way too much code inside the form.
* Choosing the capture area is clunky, maybe try an approach like [sharex](https://getsharex.com/) uses
* Expand error handling
* Make playing the sound optional
* Let user pick their own .wav file
* Slider to adjust volume of the sound
* Optional sound on timer expiry
