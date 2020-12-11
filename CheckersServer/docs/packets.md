# Packets documentation
## Server-client packets
Packets that the server sends to the client.
### PLAYER
This packet tells the client which player the client is. Player 1 is brown, Player 2 is white.

Info:

	-[playernum]: The player number of the client.
### MOVE
This packet tells the client to move a piece to another space on the board.

Info:

	-[start]: The place the piece is currently in.
	-[finish]: The place the piece will move to.
### ERROR
This packet tells the client that the move they just performed was illegal. For example, a player may have tried to move when it's not their turn.

Info:

	-[code]: The error code. See errors.md.
### END
This packet tells the client that the game has ended. 

Info:

	-[playernum]: The winner. 0 if the players tied.
### MOVELIST
The packet is sent after a request for possible moves is received.

Info:

	-[possible spaces seperated by commas]: The list of places this piece can move to.
### REMOVE
Remove a piece from play.

Info:

	-[piece]: The piece number to remove.

## Client-server packets
Packets that the client sends to the server.
### CLICK
Tells the server that the client has clicked a space.

Info:

	-[space]: The space number that the client clicked.
	-[color]: The color of the space the client clicked.
### SKIP
Tells the server to skip their turn.
### CONCEDE
Tells the server that the client concedes the match.