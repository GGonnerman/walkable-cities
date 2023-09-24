import uuid
from Edge import Edge

class Node:
    def __init__(self, location, name="Node"):
        self.location = location
        self.name = name
        self.connections = []
        self.destinations = []
        self.id = str(uuid.uuid4())
    
    def __str__(self):
        return f"{self.name}: [{self.location.y}][{self.location.x}]"
    
    def __repr__(self):
        return f"{self.name}: [{self.location.y}][{self.location.x}]"
    
    # Add a 1-way connection to another node and store that as an edge (and resort edges short-large)
    def add_connection(self, node):
        self.connections.append(Edge(self, node))
        self.connections.sort(key=lambda x: x.distance )