# =============================================
# Stage 1: Build
# =============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY Social_Media_Chatting_APP.slnx ./

# Copy all .csproj files first — layer caching means restore only re-runs when a .csproj changes
COPY Social-Media-Chatting-APP-Domain/Social-Media-Chatting-APP-Domain.csproj                                 Social-Media-Chatting-APP-Domain/
COPY Social-Media-Chatting-APP-SharedLibrary/Social-Media-Chatting-APP-SharedLibrary.csproj                   Social-Media-Chatting-APP-SharedLibrary/
COPY Social-Media-Chatting-APP-ServiceAbstraction/Social-Media-Chatting-APP-ServiceAbstraction.csproj         Social-Media-Chatting-APP-ServiceAbstraction/
COPY Social-Media-Chatting-APP-Persistence/Social-Media-Chatting-APP-Persistence.csproj                       Social-Media-Chatting-APP-Persistence/
COPY Social-Media-Chatting-APP-Service/Social-Media-Chatting-APP-Service.csproj                               Social-Media-Chatting-APP-Service/
COPY Social-Media-Chatting-APP-Service.Tests/Social-Media-Chatting-APP-Service.Tests.csproj                   Social-Media-Chatting-APP-Service.Tests/
COPY Social-Media-Chatting-APP-Presentation/Social-Media-Chatting-APP-Presentation.csproj                     Social-Media-Chatting-APP-Presentation/
COPY Social-Media-Chatting-APP-Web/Social-Media-Chatting-APP-Web.csproj                                       Social-Media-Chatting-APP-Web/

# Restore NuGet packages for the full solution
RUN dotnet restore Social_Media_Chatting_APP.slnx

# Copy remaining source
COPY . .

# Publish the Web host project in Release mode
RUN dotnet publish Social-Media-Chatting-APP-Web/Social-Media-Chatting-APP-Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =============================================
# Stage 2: Runtime  (~220 MB — no SDK, no source)
# =============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Copy published artefacts from build stage
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app
USER appuser

# ASP.NET Core listens on 8080 (non-privileged port)
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Social-Media-Chatting-APP-Web.dll"]
