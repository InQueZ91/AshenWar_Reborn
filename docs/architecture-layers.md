# Architecture Layers

This document describes the major code layers as they currently appear. It focuses on responsibilities and relationships rather than method-level detail.

## Domain Layer

Location: `AshenWar.Domain`

The Domain layer contains the game model and rules that should not depend on frameworks, databases, transport, or hosting concerns.

Primary responsibilities:

- model match lifecycle, turns, players, boards, units, tiles, and conditions
- define ability, action, effect, validator, target shape, and target filter concepts
- expose typed identifiers instead of raw `Guid` values
- raise domain events from state changes
- enforce local invariants around placement, resources, conditions, and lifecycle transitions

Important areas:

- `Entities/Match` - `Match`, `Board`, `Turn`, `PlayerOrders`, map and match definitions.
- `Entities/Units` and `Entities/Tiles` - runtime units/tiles and their definitions.
- `Entities/Abilities` - active/passive ability definitions and runtime abilities.
- `Entities/Actions` - action definitions, operations, and value sources.
- `Entities/Conditions` - unit/tile/global condition definitions, instances, and stacking behavior.
- `Entities/Modifiers` and `Entities/Stats` - stat representation and modifier calculation.
- `Events` - domain events emitted by match, unit, tile, and condition changes.
- `Interfaces` - narrow contracts used by domain and application execution code.
- `ValueObjects/Identifiers` - strongly typed ids.

Relationship to other layers:

- Application calls Domain methods to perform use cases.
- Infrastructure creates Domain objects from persisted data and JSON definitions.
- Domain should not call Application, Infrastructure, Api, EF Core, MongoDB, SignalR, MediatR, or logging.

## Application Layer

Location: `AshenWar.Application`

The Application layer coordinates use cases and game execution. It owns process-level flow: matchmaking, match creation, deployment, planning, resolution, auth, validation, event collection, and broadcasting through abstractions/framework integrations.

Primary responsibilities:

- orchestrate match and lobby workflows
- validate user commands before mutating match state
- execute active abilities, effects, actions, conditions, and passive triggers
- collect and translate domain events into resolution batches and hub messages
- define repository/service contracts that Infrastructure implements
- coordinate timers and planning expiration behavior

Important areas:

- `Commands/Matches` - MediatR request/handler flows for match creation and deployment confirmation.
- `Planning` - order submission, planning timer messages, and planning timer service.
- `ResolutionService.cs` - main turn resolution pipeline.
- `Execution` - executors and typed handlers for actions, effects, conditions, and passive triggers.
- `Validators` - affordability and roster checks.
- `Contracts` - application-facing persistence and token abstractions.
- `Auth` - auth service, JWT settings, and login/register/refresh/logout handlers.
- `Hubs/GameHub.cs` - SignalR hub used by Application services for match group communication.
- `Events` and `ValueObjects` - DTO-like records used to report application/runtime outcomes.

Relationship to other layers:

- Depends on Domain for game objects and rules.
- Defines contracts that Infrastructure fulfills.
- Currently references SignalR types directly for real-time messaging.
- Should not know specific database implementation details.

## Infrastructure Layer

Location: `AshenWar.Infrastructure`

Infrastructure adapts outside systems and data formats into Application/Domain concepts.

Primary responsibilities:

- configure MongoDB and Postgres dependencies
- persist matches through MongoDB
- persist users and refresh tokens through EF Core/Postgres
- issue JWT tokens
- load JSON game definitions from disk into an in-memory cache
- expose in-memory repositories over loaded definitions
- register infrastructure services with dependency injection

Important areas:

- `Persistence/Mongo` - match repository and Mongo serializer configuration.
- `Persistence/Postgres` - EF Core `AppDbContext`, user repository, and refresh token repository.
- `JWT/TokenService.cs` - implementation of the application token contract.
- `Definitions/DefinitionLoader.cs` - JSON-to-domain definition loading.
- `Definitions/DefinitionStore.cs` - singleton cache for loaded content.
- `Definitions/Dtos` and `Definitions/Converters` - JSON DTO shapes and polymorphic/id conversion.
- `Definitions/Repositories` - read repositories backed by `DefinitionStore`.
- `InfrastructureServiceRegistration.cs` - DI extension for Mongo, Postgres, and definitions.

Relationship to other layers:

- Depends on Application contracts and Domain model types.
- Should be called by the API host at startup.
- Should hide storage details from Application services.

## API Layer

Location: `AshenWar.Api`

The API layer is the intended ASP.NET Core host. Currently, it appears to still be in starter form.

Current responsibilities:

- creates a `WebApplication`
- registers OpenAPI
- maps a sample `/weatherforecast` endpoint
- references `AshenWar.Infrastructure`

Expected future responsibilities:

- call Application and Infrastructure service registration
- load definitions at startup
- configure auth/JWT middleware
- map application endpoints for auth, lobby, match, deployment, and planning
- map `GameHub` for SignalR clients
- expose only transport-level request/response concerns, not game rules

Relationship to other layers:

- Should be the composition root.
- Should depend on Infrastructure to pull in Application/Domain dependencies.
- Should not contain domain rules or resolution logic.

## Test Layer

Location: `AshenWar.Tests`

The test project is configured for xUnit and references Domain. It appears sparse or unfinished.

Expected responsibilities:

- protect domain invariants around match lifecycle, board placement, targeting, conditions, and stat/modifier behavior
- test action handlers and resolution flows where orchestration risk is high
- add integration-style tests around definition loading and repositories once those shapes stabilize

Current caveat:

- Build output exists under `bin` and `obj`, but source test files are not currently visible from the repo file list.

## Cross-Layer Flow

At a high level, the intended control flow is:

`Client/API/Hub -> Application use case -> Domain mutation -> Domain events -> Application execution/trigger handling -> Infrastructure persistence -> Hub/API response`

The current source has much of the Domain and Application middle in place. The outer transport/composition surface still needs significant wiring.

