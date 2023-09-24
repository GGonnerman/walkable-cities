from Node import Node

class WeightedGraph():
    def __init__(self):
        self.nodes = []

    def add_node(self, new_node):
        for node in self.nodes:
            node.add_connection(new_node)
            new_node.add_connection(node)
        self.nodes.append(new_node)