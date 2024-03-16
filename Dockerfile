## Define build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/nightly/sdk:8.0-jammy-aot AS build
ARG TARGETARCH
WORKDIR /source
EXPOSE 80
EXPOSE 443

## Copy stage
COPY src/MyFirstAotWebApi/*.csproj .
COPY src/MyFirstAotWebApi/*.config .
#RUN ls -alrth
RUN dotnet restore -r linux-${TARGETARCH}

## Copy and publish app and libs
COPY . .
RUN dotnet publish -r linux-${TARGETARCH} -o /app src/MyFirstAotWebApi/MyFirstAotWebApi.csproj
RUN rm /app/*.dbg /app/*.Development.json

## Final stage
FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:8.0-jammy-chiseled-aot
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT [ "./MyFirstAotWebApi" ]