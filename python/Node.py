from Edge import Edge

class Node:
    def __init__(self, location, name="Node"):
        self.x = location[0]
        self.y = location[1]
        self.name = name
        self.connections = []
        self.destinations = []
    
    def __str__(self):
        return f"{self.name}: [{self.x}, {self.y}]"
    
    def __repr__(self):
        return f"{self.name}: [{self.x}, {self.y}]"
    
    def add_connection(self, node):
        self.connections.append(Edge(self, node))
        self.connections.sort(key=lambda x: x.distance )