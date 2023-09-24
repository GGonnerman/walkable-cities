from osgeo import gdal
from osgeo import ogr
from osgeo import osr
from osgeo import gdal_array
from osgeo.gdalconst import *
import os
import json
import numpy as np
#Imports

#Sets gdal maping software to use exceptions
gdal.UseExceptions()

#Set file location of maping data
file_name = input("file name: ")
ds = gdal.Open(f"../../Map Data/{file_name}")

#Properly orient data
band = ds.GetRasterBand(1)
elevation = band.ReadAsArray()

#Round data to lessen storage inpact
elevation = (np.round(elevation, 0)).astype(int)

#Zoom in photo
elevation = elevation[10250:10500, 5000:5250]

#Transforms from numpy to list
elevation_list = elevation.tolist()

#Save data as json file
file_path = "elevationData.json"
with open(file_path, 'w') as f:
    json.dump({"data": elevation_list}, f)
