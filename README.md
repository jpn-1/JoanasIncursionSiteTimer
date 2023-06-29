# JoanasIncursionSiteTimer
Simple Windows Forms App to track respawning timers of Incursion HQ Sites

Using [tesseract](https://github.com/tesseract-ocr/tesseracta) to identify text from an image, which is taken every 0.5s in a setup area on the screen. 
Will pause and hint if the mouse pointer is inside the capture area.
If there are changes to the the amount of sites bigger than 1 (e.g. area is covered by other window), no action is taken. 
