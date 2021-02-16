Param([string] $publish)
echo "publish = $publish"

$targets = @("win-x64", "linux-x64", "osx-x64")

foreach ($target in $targets) {
    dotnet publish ./src/SnippetBuilder/ -c Release --self-contained false -p:PublishSingleFile=true -r $target -o $publish/$target
}