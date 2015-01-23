BusyBeetle
==========
A simulation of Beetleworld.

How to play:  
1. Edit server config and choose "BusyBeetle" or "GameOfLife"
2. Start the server.  
3. Start one or more clients.   
4. (Choose a color and) click on the quadrangle.  
5. Wait, watch and win. 


##World
The World is rendered as a BitmapImage, while the program itself uses a normal bitmap to work with. The conversion from Bitmap to BitmapImage is done by the bitmap converter.

##Networking
The server uses a round-robin principle. It ensures that all clients are up to date on every turn. It also prevents beetles from interfering with eachother by the world not being up to date.








*PS. You cannot win*
