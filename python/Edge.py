class Edge:
    def __init__(self, start, end):
        self.start = start
        self.end = end
        self.distance = self.start.location.distance_from(self.end.location)
    
    def __repr__(self):
        return f"{self.start}-{self.distance}-{self.end}"