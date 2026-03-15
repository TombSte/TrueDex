# TrueDex API

## Prerequisites

- .NET 10 SDK
- Access to external services.

The API calls external services:

- `PokeApiNet` to fetch Pokemon species data
- the Fun Translations API client to translate descriptions

## Run Locally

From the `src` folder:

```bash
dotnet restore TrueDex.slnx
dotnet run --project TrueDex.Api
```

The http launch profile expose these local URLs: `http://localhost:5000`

ScalarUI is available navigate to:
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

- Swagger UI is exposed through Scalar at `/docs`
- translation behavior depends on the Pokemon habitat
- if translation fails, the API falls back to the original description
