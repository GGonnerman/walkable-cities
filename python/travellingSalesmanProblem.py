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
from Location import Location

random.seed(1)

files = ["IowaCityHeight2.json", "Detroit.json", "Boston.json"]

for file_name in files:

    city = None
    if "Iowa" in file_name:
        city = "iowa"
    elif "Detroit" in file_name:
        city = "detroit"
    elif "Boston" in file_name:
        city = "boston"
    else:
        raise Exception("Invalid city with name " + file_name)

    elevation_data = None

    with open(file_name, "r") as f:
        elevation_data = json.load(f)['data']

    row_count = len(elevation_data)
    column_count = len(elevation_data[0])

    building_size = 15
    building_radius = math.ceil( ((building_size/2)**2 + (building_size/2)**2)**0.5 )

    flat_elevation = []
    for row in elevation_data: flat_elevation += row
    flat_elevation.sort()

    highest_point = flat_elevation[-1]
    lowest_point = flat_elevation[0]
    elevation_delta = (highest_point - lowest_point)

    Location.elevation_data = elevation_data
    Location.elevation_delta = elevation_delta
    Location.lowest_elevation = lowest_point

    from PIL import Image
    import numpy as np

    def normalize(v):
        return 255 * (v - lowest_point) / elevation_delta


    pixels = []
    for row in reversed(elevation_data):
        pixels.append([(normalize(v), normalize(v), normalize(v)) for v in row])

    #print(pixels)

    # Convert the pixels into an array using numpy
    array = np.array(pixels, dtype=np.uint8)

    # Use PIL to create an image from the new array of pixels
    new_image = Image.fromarray(array)
    new_image.save(f"out/elevation-generation-{city}.png")

    #for i in range(len(elevation_data)):
    #    for j in range(len(elevation_data[i])):
    #        if elevation_data[i][j] == lowest_point:
    #            print(f"[{i}, {j}]: {lowest_point}")

    # Check if this potential location is touching an existing building 
    def has_building_collisions(potential_location):
        for location in locations:
            if location.distance_from(potential_location) < (2.1*building_radius):
                return True
        return False

    # Check if this potential location overlaps water
    def has_water_collision(potential_location):
        covered_points = []
        for y in range(potential_location.y - building_radius, potential_location.y + building_radius):
            for x in range(potential_location.x - building_radius, potential_location.x + building_radius):
                if potential_location.distance_from(Location(y, x)) < ((2**0.5)*building_radius+2): covered_points.append(Location(y, x))
        
        print(f"Number of Covered Points {len(covered_points)}")
        for location in covered_points:
            covered_point_elevation = elevation_data[location.y][location.x]
            normalized_elevation = normalize(covered_point_elevation)
            if normalized_elevation < 50: return True
        
        return False

    def get_random_location():
        # Figure out water and edges later
        potential_location = Location(random.randrange(building_radius+2, row_count - building_radius - 2), random.randrange(building_radius+2, column_count - building_radius - 2))

        while has_building_collisions(potential_location) or has_water_collision(potential_location):
            potential_location = Location(random.randrange(building_radius+2, row_count - building_radius - 2), random.randrange(building_radius+2, column_count - building_radius - 2))
        
        return potential_location

    # TODO: Check whether to start at train station or at house
    names = [ "train", "house", "hospital", "police_station", "fire_station", "shop", "capital_building", ]
    #"01", "02", "03", "03", "04", "05", "06", "07", "04", "05", "06", "07", "024", "025"]
    locations = []

    # Make a corresponding location for each name
    for name in names: locations.append(get_random_location())

    # Pair the names and locations into a buildings dict
    buildings = {}
    for name, location in zip(names, locations):
        buildings[name] = location
        #print(f"{name}: {location}")

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

    serializable_buildings = {}
    for name, location in zip(names, locations):
        serializable_buildings[name] = [location.x, location.y]

    print("About to generate edges")
    list_of_edges = {}
    for i in range(len(final_path)-1):
        combined_id = "_".join(sorted([final_path[i].id, final_path[i+1].id]))
        list_of_edges[combined_id] = [[final_path[i].location.x, final_path[i].location.y], [final_path[i+1].location.x, final_path[i+1].location.y]]
    print("Generated edges")
    print(list_of_edges)
    print([list_of_edges[key] for key in list_of_edges])

    final_path = {
        "location_data": serializable_buildings,
        "path_data": [[node.location.x, node.location.y] for node in final_path],
        "list_of_edges": [list_of_edges[key] for key in list_of_edges]
    }

    with open(f"out/prims-algorithm-{city}.json", "w") as f:
        json.dump(final_path, f)