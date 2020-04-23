#!bin/sh

sh ./build-all.sh

svn_repo_url=https://svn.code.sf.net/p/genielamp/code
version=1.1
product_name=GenieLamp
release_dir=GenieLamp
#exclude_filter="--exclude=.svn --exclude=bin --exclude=*.pidb --exclude=*.userprefs --exclude=*.*~"

rm -f -r ./$release_dir
mkdir $release_dir
mkdir ./$release_dir/Bin
mkdir ./$release_dir/Docs
mkdir ./$release_dir/Examples
cp -r ../Bin/Release/* ./$release_dir/Bin/
#rsync -a $exclude_filter ../Examples/AllLayers/ ./$release_dir/Examples/AllLayers
#rsync -a $exclude_filter ../Examples/GLQuickStart/ ./$release_dir/Examples/GLQuickStart
svn checkout -q $svn_repo_url/Examples/AllLayers ./$release_dir/Examples/AllLayers
svn checkout -q $svn_repo_url/Examples/GLQuickStart ./$release_dir/Examples/GLQuickStart
svn checkout -q $svn_repo_url/Examples/Libs ./$release_dir/Examples/Libs
svn checkout -q $svn_repo_url/Docs ./$release_dir/Docs
cp ../COPYING.* ./$release_dir
rm ./$release_dir/Bin/GenieLamp.ModelEditor*

revision=$(svn info -rHEAD $svn_repo_url | grep Revision: | cut -c11-)
echo Last revision is: $revision

rm $product_name*.zip
zip -9 -r -q $product_name".v"$version"r"$revision.zip ./$release_dir
rm -f -r ./$release_dir
