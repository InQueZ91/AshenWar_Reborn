# Codebase Overview

AshenWar is a .NET backend/game-domain codebase for a turn-based tactical game. The repository is organized around a clean-ish layered architecture: Domain holds the game model, Application orchestrates use cases and resolution flows, Infrastructure provides persistence and definition loading, and Api is intended to host the runtime surface.

The codebase is still under active construction. Some core domain and application systems are present, while the public API host and some dependency registration paths still look like placeholders.

## Solution Layout

- `AshenWar.Domain` - pure game domain model. This includes matches, boards, units, tiles, abilities, actions, effects, conditions, stats, events, identifiers, and domain interfaces.
- `AshenWar.Application` - orchestration layer. This includes lobby flow, match creation/deployment commands, planning and resolution services, action/effect/condition execution, auth use cases, SignalR hub contracts, validators, and repository/service interfaces.
- `AshenWar.Infrastructure` - adapter layer. This includes Mongo match persistence, Postgres user/token persistence, JWT token service, JSON definition loading, definition DTOs/converters, and in-memory definition repositories.
- `AshenWar.Api` - ASP.NET Core host. Currently still resembles a starter API with OpenAPI and weather forecast endpoint; it references Infrastructure but does not yet appear to wire the real AshenWar services into the HTTP/SignalR surface.
- `AshenWar.Tests` - xUnit test project referencing Domain. At the time of writing, no source test files are visible in the tracked source tree, only project/build output.
- `CLAUDE.md` - existing AI/developer guidance. It contains useful architecture notes, though some names appear stale relative to the current source.

## Core Concepts

### Definition vs Runtime Instance

Most game objects follow a definition/instance split.

- `XDefinition` classes are content blueprints, usually loaded from JSON definitions or repositories.
- Runtime `X` classes are match-state instances created from definitions and mutated during play.

Examples include units, tiles, active/passive abilities, effects, conditions, maps, and match definitions.

### Match as Aggregate Root

`Domain.Entities.Match.Match` is the main aggregate root. It owns or coordinates:

- match phase and turn history
- players and deployment state
- board state
- units and tiles
- global conditions
- trigger registry
- match lifecycle transitions

Units and tiles do not hold references back to the match. Cross-entity coordination is routed through `Match` and `Board`.

### Board and Hex Positioning

`Board` stores tile and unit placement by `HexCoord`. It provides query helpers for tiles, units, targetables, neighbors, range lookup, and cone lookup. Board mutation is intentionally guarded: placing a unit requires an existing tile, placing a tile fails on occupied positions, and removing a tile can also remove the unit on it.

### Actions, Effects, Abilities, and Conditions

The combat model is built from composable behavior:

- action definitions describe atomic behavior such as movement, resource changes, condition application, tile spawning, and cooldown changes.
- effects group actions by effect definition id.
- active ability phases resolve target shape/filter/validator, then execute effects.
- passive triggers listen to domain events and can execute effects through the trigger chain.
- conditions can affect units, tiles, or the global match and are ticked during resolution.

`Application.Execution.ActionExecutor` dispatches action definitions to typed handlers through `ActionHandlerRegistry`, then collects domain events from touched entities.

## Main Runtime Flow

The intended multiplayer flow appears to be:

1. Users authenticate through application auth handlers and infrastructure token/user repositories.
2. Users enter matchmaking through `LobbyService.JoinQueueAsync`.
3. A lobby is created when two users are matched.
4. Players select rosters and mark ready.
5. `CreateMatchHandler` creates the match from selected match/map/unit definitions.
6. Players deploy units through confirm deployment commands.
7. The match enters planning.
8. `PlanningService.SubmitOrders` accepts per-player unit orders.
9. Once both players submit, or when a planning timer expires, `ResolutionService.Resolve` runs the turn.
10. Resolution calculates initiative, executes active abilities, ticks conditions, applies global events, runs triggers, persists match state, and broadcasts results over SignalR.

Some of this path is implemented in Application/Domain, but not all of it is currently wired through the API host.

## Where to Look First

- Match lifecycle: `AshenWar.Domain/Entities/Match/Match.cs`
- Board placement and targeting queries: `AshenWar.Domain/Entities/Match/Board.cs`
- Turn planning: `AshenWar.Application/Planning/PlanningService.cs`
- Turn resolution: `AshenWar.Application/ResolutionService.cs`
- Ability execution: `AshenWar.Application/Execution/ActiveAbilityExecutor.cs`
- Generic action dispatch: `AshenWar.Application/Execution/ActionExecutor.cs`
- Matchmaking/lobby flow: `AshenWar.Application/LobbyService.cs`
- Definition loading and cache: `AshenWar.Infrastructure/Definitions/DefinitionLoader.cs` and `DefinitionStore.cs`
- Persistence adapters: `AshenWar.Infrastructure/Persistence`
- API composition root: `AshenWar.Api/Program.cs`

## Naming and Dependency Notes

Project namespaces currently use short roots such as `Domain`, `Application`, and `AshenWar.Infrastructure`. The project folders are prefixed with `AshenWar.*`, but not every namespace is.

The intended dependency direction is:

`Api -> Infrastructure -> Application -> Domain`

Infrastructure also references Domain directly because it maps loaded/persisted data into domain objects. Domain should remain dependency-free.

