# 1. Define o nome da Solution e dos projetos
$SOLUTION_NAME = "SampleSystemDesign"
$PROJECTS = @(
    "ScalingReads",
    "ScalingWrites",
    "LongRunning",
    "RealTime",
    "LargeFiles",
    "Contention",
    "MultiStep"
)

# 2. Cria a estrutura de pastas principal
Write-Host "Criando estrutura de pastas: src, test, Docs/Prompts" -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path "src", "test", "Docs/Prompts"

# 3. Cria o arquivo de Solution
Write-Host "Criando arquivo de Solution: $SOLUTION_NAME.sln" -ForegroundColor Cyan
dotnet new sln -n $SOLUTION_NAME

# 4. Cria os projetos de implementação (src) e de teste (test)
foreach ($PROJECT in $PROJECTS) {
    $SRC_NAME = "$SOLUTION_NAME.$PROJECT"
    $TEST_NAME = "$SRC_NAME.Tests"

    Write-Host "Criando projeto de implementação: $SRC_NAME" -ForegroundColor Green
    dotnet new classlib -n $SRC_NAME -o "src/$SRC_NAME"
    dotnet sln add "src/$SRC_NAME/$SRC_NAME.csproj"

    Write-Host "Criando projeto de teste: $TEST_NAME" -ForegroundColor Green
    dotnet new xunit -n $TEST_NAME -o "test/$TEST_NAME"
    dotnet sln add "test/$TEST_NAME/$TEST_NAME.csproj"

    # Adiciona a referência do projeto de implementação ao projeto de teste
    dotnet add "test/$TEST_NAME/$TEST_NAME.csproj" reference "src/$SRC_NAME/$SRC_NAME.csproj"

    # 5. Cria a estrutura de pastas Hexagonal/DDD
    Write-Host "Criando estrutura Hexagonal/DDD para $SRC_NAME" -ForegroundColor Yellow
    $basePath = "src/$SRC_NAME"
    $folders = @(
        "Domain/Entities", "Domain/ValueObjects", "Domain/Interfaces", "Domain/Events", "Domain/Exceptions",
        "Application/UseCases", "Application/DTOs", "Application/Interfaces", "Application/Mappers", "Application/Validators",
        "Infrastructure/Persistence", "Infrastructure/ExternalServices", "Infrastructure/Configuration", "Infrastructure/Adapters",
        "Presentation/Controllers", "Presentation/Middleware"
    )

    foreach ($folder in $folders) {
        New-Item -ItemType Directory -Force -Path "$basePath/$folder" | Out-Null
    }
}

Write-Host "`nEstrutura de projeto criada com sucesso!" -ForegroundColor Green
Write-Host "Próximo passo: Copiar o conteúdo dos arquivos de diretrizes e prompts para a pasta Docs." -ForegroundColor White
