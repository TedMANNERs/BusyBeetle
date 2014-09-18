BusyBeetle
==========
A simulation of beetle world.

Choose a color and click on the quadrangle.

##World
The World is rendered as a BitmapImage, while the program itself uses a normal bitmap to work with. The conversion from Bitmap to BitmapImage is done by the bitmap converter.

##Networking
The server uses a round-robin principle. It ensures that all clients are up to date on every turn. It also prevents beetles from interfering with eachother by the world not being up to date.
