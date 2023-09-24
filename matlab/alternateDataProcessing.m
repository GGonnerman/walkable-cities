%Open and read json file containing height information
fname = 'Detroit.json'; 
fid = fopen(fname); 
raw = fread(fid,inf); 
str = char(raw'); 
fclose(fid); 
val = jsondecode(str);

%Set value for how large the json file is
resolution = 250;

%Makes array to base 3d render on
square = zeros(resolution,resolution);
for i = 1:size(square, 1)
    for j = 1:size(square, 2)
        square(i,j)=j;
    end
end

%disp(square)

%Initalize the graph
graph = meshc(square, 1:250, val.data);

%Set Colors
colormap(GrassColorTwo)

%Turn axis' off
axis off











