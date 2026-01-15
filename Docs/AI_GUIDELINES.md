\# AI Development Guidelines - SampleSystemDesign



\## 1. Architecture (Hexagonal + DDD)

\- \*\*Domain:\*\* Pure C#, no dependencies. Entities, Value Objects, Interfaces.

\- \*\*Application:\*\* Use Cases, DTOs, Orchestration.

\- \*\*Infrastructure:\*\* Implementations (Redis, EF Core, Kafka).

\- \*\*Presentation:\*\* Controllers/Minimal APIs.



\## 2. Rules

\- \*\*Language:\*\* Code (classes, methods, etc.) MUST be in \*\*English\*\*.

\- \*\*Namespace:\*\* `SampleSystemDesign.{Pattern}.{Layer}`.

\- \*\*Clean Code:\*\* Use PascalCase for members, camelCase for locals.

\- \*\*Framework:\*\* .NET 8.0+.



