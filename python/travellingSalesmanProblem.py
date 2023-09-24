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

import random
import json
import math
from WeightedGraph import WeightedGraph
from Node import Node

random.seed(1)

elevation_data = None

with open("IowaCityHeight2.json", "r") as f:
    elevation_data = json.load(f)['data']

width = len(elevation_data)
height = len(elevation_data[0])

building_size = 15
building_radius = math.ceil( ((building_size/2)**2 + (building_size/2)**2)**0.5 )

# Get the euclidean distance between two 2d vectors.  
def distance_between(first_location, second_location):
    return ( (first_location[0] - second_location[0])**2 + (first_location[1] - second_location[1])**2 )**0.5

# Check if this potential location is touching an existing building 
def has_building_collisions(potential_location):
    for location in locations:
        if distance_between(location, potential_location) < (2.1*building_radius):
            return True
    return False

def get_random_location():
    # Figure out water and edges later
    potential_location = [random.randrange(building_radius, height - building_radius), random.randrange(building_radius, width - building_radius)]

    while has_building_collisions(potential_location):
        potential_location = [random.randrange(building_radius, height - building_radius), random.randrange(building_radius, width - building_radius)]
    
    return potential_location

# TODO: Check whether to start at train station or at house
names = [ "train", "house", "hospital", "police_station", "fire_station", "shop", "capital_building", 
"01",
"02",
"03",
"04",
"05",
"06",
"07",
"024",
"025"]
locations = []

# Make a corresponding location for each name
for name in names: locations.append(get_random_location())

# Pair the names and locations into a buildings dict
buildings = {}
for name, location in zip(names, locations):
    buildings[name] = location
    print(f"{name}: {location}")

# Setup the weighted graph with correct elements
graph = WeightedGraph()
for key in buildings:
    graph.add_node(Node(buildings[key], key))

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
    "location_data": buildings,
    "path_data": [[node.x, node.y] for node in final_path]
}

with open("prims-algorithm.json", "w") as f:
    json.dump(final_path, f)