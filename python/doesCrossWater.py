import os
import json

with open('IowaCityHeight2.json', 'r') as f:
  data = json.load(f)['data']

min = 99999

for i in data:
    #print(i)
    for j in i:
        #print(j)
        if j < min:
            min = j

print(min)
print(checkWater(min, 150))


def checkWater(minimum, depth):
    allowedDif = 50

    if DepthCheck > minimum + depth:
        return True
    else:
        return False    

