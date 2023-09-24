class Location:

    elevation_data = None
    elevation_delta = None
    lowest_elevation = None

    def __init__(self, y, x):
        self.y = y
        self.x = x
        self.elevation = Location.elevation_data[y][x]
    
    # Get the euclidean distance between two 2d vectors.  
    def distance_from(self, second_location):
        return ( (self.x - second_location.x)**2 + (self.y - second_location.y)**2 )**0.5 + (self.elevation - second_location.elevation)
    
    # Get the euclidean distance between two 2d vectors.  
    def augmented_distance_from(self, second_location):
        # Change in incline scale (-1.0 to 1.0)
        scaled_incline = ((second_location.elevation - self.elevation) / Location.elevation_delta)
        # Allow 50% modification in distance from change in incline
        return self.distance_from(second_location) + (0.5) * (scaled_incline * self.distance_from(second_location))