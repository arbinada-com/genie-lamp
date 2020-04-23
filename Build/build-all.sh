#!bin/sh
common_build_options=/target:Build /verbosity:quiet
xbuild "../Sources/GenieLamp.Solution/GenieLamp.Solution.sln" $common_build_options /property:configuration=Release 
xbuild "../Sources/GenieLamp.Solution/GenieLamp.Solution.sln" $common_build_options /property:configuration=Debug
