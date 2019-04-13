FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build

WORKDIR /src
COPY [".","."]

RUN dotnet restore "Hangman.WebUI/Hangman.WebUI.csproj"
RUN dotnet build "Hangman.WebUI/Hangman.WebUI.csproj" -c Release

FROM build AS publish

# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_6.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

RUN dotnet publish "Hangman.WebUI/Hangman.WebUI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hangman.WebUI.dll"]