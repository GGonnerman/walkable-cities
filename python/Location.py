class Location:
    def __init__(self, y, x):
        self.y = y
        self.x = x
    
    # Get the euclidean distance between two 2d vectors.  
    def distance_from(self, second_location):
        return ( (self.x - second_location.x)**2 + (self.y - second_location.y)**2 )**0.5