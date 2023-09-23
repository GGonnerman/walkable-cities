from osgeo import gdal
from osgeo import ogr
from osgeo import osr
from osgeo import gdal_array
from osgeo.gdalconst import *
import os
import json
import numpy as np

#os.chdir(r"../../Map Data")

gdal.UseExceptions()

#print("IGNORE THAT" + "\n"*5)

file_name = input("file name: ")
ds = gdal.Open(f"../../Map Data/{file_name}")

band = ds.GetRasterBand(1)

elevation = band.ReadAsArray()

elevation = (np.round(elevation, 0)).astype(int)

import matplotlib.pyplot as plt

#print(elevation)

elevation = elevation[4500:5500, 4500:5500]
#elevation = [row[4500:5500] for row in elevation]

#plt.imshow(elevation, cmap="gist_earth")
#plt.show()

elevation_list = elevation.tolist()

file_path = "elevationData.json"
with open(file_path, 'w') as f:
    json.dump({"data": elevation_list}, f)


#[{x} for i in elevation]

#print(elevation)

