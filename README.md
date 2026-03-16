# TrueDex API

## Prerequisites

- .NET 10 SDK
- Access to external services.

## Run Locally

From the `src` folder:

```bash
dotnet restore TrueDex.slnx
dotnet run --project TrueDex.Api
```

The application will be available at:

`http://localhost:5000`

To invoke the endpoints, ScalarUI is available at:

`http://localhost:5000/docs`

## FunTranslation Client Update

To update or create the client of FunTranslation run from the `src` folder:

```bash
dotnet build FunTranslationsClient/FunTranslationsClient.csproj /p:GenerateApiClientOnBuild=true
```

## Run With Docker

From the `src` folder:

```bash
docker build -f TrueDex.Api/Dockerfile -t truedex-api .
docker run --rm -p 8080:8080 truedex-api
```

Then call:

```bash
curl http://localhost:8080/pokemon/lugia
```

## Notes

The application uses the PokeApi and FunTranslation web services.

To invoke PokeApi ws I used an existing and documented .NET client (https://gitlab.com/PoroCYon/PokeApi.NET).

To invoke FunTranslation api I created a new auto-generated client using nswag for the following reasons:

- Faster implementation
- Very consistent client with open api definition

## Improvements for production

- Improve error handling with more detailed errors. Now all errors produce 500 InternalErrorServer or 404 Not Found.
- Introduce a library to have structure logs (NLog, Serilog, ... ) and define a log strategy to write just necessary logs.
- Introduce Open Telemetry to enhance logs, traces and monitoring.
- Authentication/Authorization
- Introduce a cache to reduce the number of invocation of the external service (if possibile).
- Introduce circuit breaker if external services are not availables.
- Libraries like FunTranslationClient referenced as Package and not as Project to be better reused and versioned. I would also consider to set the base url of the FunTranslation service into appsettings.json/environment variables. 
- For more structured projects, I would consider the adoption of Clean Architecture, I used this simple project structure because the requirement was simple.
