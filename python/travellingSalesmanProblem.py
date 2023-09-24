"""
Train
Houses
Hostpial
police station
fire station
grocery store
shop
Capital building (iowa city specific)
"""

from random import randrange
import json
import math
from WeightedGraph import WeightedGraph
from Node import Node

elevation_data = None

with open("elevationData2.json", "r") as f:
    elevation_data = json.load(f)['data']

width = len(elevation_data)
height = len(elevation_data[0])

building_size = 15
building_radius = math.ceil( ((building_size/2)**2 + (building_size/2)**2)**0.5 )

def get_random_location():
    # Figure out water and edges later
    return [randrange(building_radius, height - building_radius), randrange(building_radius, width - building_radius)]

locations = {
    "train": get_random_location(),
    "house": get_random_location(),
    "hospital": get_random_location(),
    "police_station": get_random_location(),
    "fire_station": get_random_location(),
    "shop": get_random_location(),
    "capital_building": get_random_location(),
}

for key in locations:
    print(f"{key}: {locations[key]}")

# Setup the weighted graph with correct elements
graph = WeightedGraph()
for key in locations:
    graph.add_node(Node(locations[key], key))

# Implementation of Prims Algorithm
chosen_path = []
# Create a list of possible vertexes
possible_vertexes = [x for x in graph.nodes]
# Choose a random vertex to be starting point
homeVertex = possible_vertexes.pop(0)
possible_edges = homeVertex.connections
possible_edges.sort(key=lambda x: x.distance)

while len(possible_vertexes) > 0:
    # While the next edge does not explore a new vertex, delete that edge
    while not (possible_edges[0].end in possible_vertexes):
        print(f"Skipping {possible_edges[0]}")
        # Skip each edge with an endpoint that is already travelled to
        possible_edges.pop(0)
    # Remove the vertex we are able to travel to
    new_vertex = possible_vertexes.pop( possible_vertexes.index(possible_edges[0].end) )
    # Add the just travelled edeg to the chosen path
    chosen_path.append( possible_edges.pop(0) )
    # Add the new possible edges from the new vertex and resort edges
    possible_edges += new_vertex.connections
    possible_edges.sort(key=lambda x: x.distance)

print("\n" * 1)
print(chosen_path)

# We now have a chosen path, which contains all of the travelled edges in the optimal solution
travel = {}
for edge in chosen_path:
    start = edge.start
    end = edge.end

    start.destinations.append(end)
    end.destinations.append(start)

current_node = homeVertex
final_path = [current_node]
while len(current_node.destinations) > 0:
    # Prioritize nodes that have not been travelled to
    current_node.destinations.sort(key=lambda x: x in final_path)
    # Get the next node in the graph
    next_node = current_node.destinations.pop(0)
    # Add the new node to the final path
    final_path.append(next_node)
    # Update current node to be the next node
    current_node = next_node

print("Okay. My Final path is")

for node in final_path:
    print(node)

final_path = {
    "location_data": locations,
    "path_data": [[node.x, node.y] for node in final_path]
}

with open("prims-algorithm.json", "w") as f:
    json.dump(final_path, f)