cd PrepareInput/

dotnet publish -c release -r win-x64
dotnet publish -c release -r linux-x64

cd ../MosaicGenerator/ 

dotnet publish -c release -r win-x64
dotnet publish -c release -r linux-x64

cd ../