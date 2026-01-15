# Arquitetura Hexagonal + DDD - Template de Estrutura

## Estrutura Padrão para Cada Projeto

```
SystemDesign.{Pattern}/
├── Domain/
│   ├── Entities/              # Entidades do Domínio (Aggregates Root)
│   ├── ValueObjects/          # Objetos de Valor
│   ├── Interfaces/            # Interfaces de Repositório e Serviços de Domínio
│   ├── Events/                # Eventos de Domínio
│   └── Exceptions/            # Exceções de Domínio
├── Application/
│   ├── UseCases/              # Handlers de Casos de Uso
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Interfaces/            # Interfaces de Serviços da Aplicação
│   ├── Mappers/               # Mapeadores (Domain -> DTO)
│   └── Validators/            # Validadores de Entrada
├── Infrastructure/
│   ├── Persistence/           # Implementação de Repositórios
│   ├── ExternalServices/      # Integrações Externas (Redis, Kafka, S3, etc.)
│   ├── Configuration/         # Configurações e Injeção de Dependência
│   └── Adapters/              # Adaptadores para Tecnologias Externas
└── Presentation/
    ├── Controllers/           # Controladores (Adapters de Entrada)
    └── Middleware/            # Middleware de Aplicação
```

## Princípios Aplicados

1. **Separação de Responsabilidades:** Cada camada tem uma responsabilidade clara.
2. **Inversão de Dependências:** Camadas externas dependem de abstrações internas.
3. **Isolamento de Negócio:** A lógica de domínio é independente de frameworks.
4. **Testabilidade:** Cada componente pode ser testado isoladamente.
5. **Escalabilidade:** Fácil adicionar novos casos de uso sem afetar o existente.
