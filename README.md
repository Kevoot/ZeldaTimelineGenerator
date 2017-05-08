# ZeldaTimelineGenerator
Application that uses a tree graph generating utility (Portions Copyright © 2007 Rotem Sapir)
and user specified attributes to create the closest representation of the data in a graph.

# Usage
Select the games you want to include in the initial menu from the drop downs. 
For each game, you can define both Direct Connections and Exclusions.

# Direct Connections
There are (at the moment) used to define the connection between games. Intended use:
Creating a connection for a game implies that game is the parents, and the specified game
is the target. The Direct Connection attribute is meant as a "strong", direct connection
between two games

# Exclusions
These are functional in the UI, but as of yet have no bearing on the graph generated.
Intended use: These will be used as a modifier to the Tree building algorithm, by calling
into question the linear direction of the games, and allow multiple graphs to be generated 
by each data set.

# Plans
For now, it only uses direct connections specified by the user, which require no background
logic beyond building the tree data structure. The next step is to use the exclusion
attributes and the weight of that value to generate separate graphs.

Also I should probably make the output prettier, and may end up replacing the graph lib.
