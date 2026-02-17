FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SchedulePlanner/SchedulePlanner.csproj", "SchedulePlanner/"]
RUN dotnet restore "SchedulePlanner/SchedulePlanner.csproj"
COPY . .
WORKDIR "/src/SchedulePlanner"
RUN dotnet build "./SchedulePlanner.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish

#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet tool install --global dotnet-ef --version 9.*
#RUN dotnet ef migrations add InitialCreate -Project "./SchedulePlanner.csproj"  
#RUN ls
ARG BUILD_CONFIGURATION=Release 
RUN dotnet publish "./SchedulePlanner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

#ENV CONNECTION_STRING="Server=env_net;Database=mydb;User Id=sa;Password=password123!;TrustServerCertificate=True;Encrypt=False;"
#RUN dotnet ef migrations add InitialCreate --project "./SchedulePlanner.csproj"
#RUN echo "$CONNECTION_STRING"
#RUN dotnet ef database update --project "./SchedulePlanner.csproj" --connection "${CONNECTION_STRING}"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchedulePlanner.dll"]
