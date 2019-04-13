FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY [".","."]

RUN dotnet restore "Hangman.Dictionary/Hangman.Dictionary.csproj"
RUN dotnet build "Hangman.Dictionary/Hangman.Dictionary.csproj" -c Release

FROM build AS publish
RUN dotnet publish "Hangman.Dictionary/Hangman.Dictionary.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hangman.Dictionary.dll"]