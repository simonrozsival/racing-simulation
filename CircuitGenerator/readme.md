# Race Circuit Generator

I wrote this circuit generator to create random tracks for my master's thesis. I will use the data for testing models of autonomous vehicles.

![An example of a generated track.](./track-example.png)

## How To Build It And Use It

You can build and run the project using `dotnet`:

```sh
# install NuGet packages
dotnet restore

# build
dotnet build

# run with arguments
dotnet run -- --output some-directory

```

I use [svgexport](https://www.npmjs.com/package/svgexport) commandline tool to convert SVGs to PNGs. This helps me easily create occupancy grids from the bitmaps. You have to have this tool installed on your computer and it has to be in your `$PATH`. If you don't want to use it, you have to edit `Program.cs` and skip the PNG and occupancy grid generation. If you have an idea how to easily get rid of this library and how to calculate the occupancy grid without the PNG even with all the overlapping regions (when there is a really sharp angle), I will welcome any pull request or an issue with description of the solution!

Available commandline parameters:

```
                                                                                               
  --output                           Required. Output directory.                               
                                                                                               
  -w, --width                        (Default: 1000) Width of the track in meters.             
                                                                                               
  -h, --height                       (Default: 1000) Height of the track in meters.            
                                                                                               
  -m, --min-waypoints                (Default: 10) Minimum waypoints count.                    
                                                                                               
  -M, --max-waypoints                (Default: 20) Maximum waypoints count.                    
                                                                                               
  -t, --track-width                  (Default: 80) Track width.                                
                                                                                               
  -r, --random                       (Default: true) Ignore the seed parameter.                
                                                                                               
  -s, --seed                         (Default: 0) Seed for the random number generator.        
                                                                                               
  -o, --occupancy-grid-resolution    (Default: 5) Occupancy grid sampling resolution.          
                                                                                               
  -n, --number-of-tracks             (Default: 1) Number of generated tracks.                  
                                                                                               
  --help                             Display this help screen.                                 
                                                                                               
  --version                          Display version information.                              
                                                                                               
```

The program creates a new directory in the given output directory with a SVG, PNG and JSON files. The SVG and PNG are the visual representation of the trac, the JSON contains positions of the waypoints and an occupancy grid (`#` represents an obstacle region, ` ` represents a free region).

## Sources And Inspiration

The idea of the algorith is taken from here: http://www.gamasutra.com/blogs/GustavoMaciel/20131229/207833/Generating_Procedural_Racetracks.php

I copied convex hull algorithm from http://loyc-etc.blogspot.com/2014/05/2d-convex-hull-in-c-45-lines-of-code.html

I was inspired by https://metacar-project.com/ and I borrowed some colors from there.

## So you want to use it?

I build this tool for my personal use and I publish it only for anybody on the Internet to learn from and to use it for whatever they want. If you found this code interesting or useful, please send me an email or leave an issue here on GH, I will be more than happy to see what you did with this code.
