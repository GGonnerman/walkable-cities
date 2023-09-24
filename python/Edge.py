class Edge:
    def __init__(self, start, end):
        self.start = start
        self.end = end
        self.distance = Edge.get_distance(start, end)
    
    def __repr__(self):
        return f"{self.start}-{self.distance}-{self.end}"

    def get_distance(start, end):
        return ((start.x - end.x)**2 + (start.y - end.y)**2)**0.5