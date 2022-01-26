FROM mcr.microsoft.com/dotnet/sdk:5.0
COPY bin/Release/net5.0/linux-x64/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "VenkaLite.dll"]
