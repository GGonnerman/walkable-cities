class Edge:
    def __init__(self, start, end):
        self.start = start
        self.end = end
        # Distance accounting for change in elevation
        self.distance = self.start.location.augmented_distance_from(self.end.location)
    
    def __repr__(self):
        return f"{self.start}-{self.distance}-{self.end}"