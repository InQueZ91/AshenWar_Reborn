# Incomplete Areas

This codebase is still evolving. This document captures areas that appear incomplete, placeholder-like, or in need of confirmation. Treat this as a working map, not a final defect list.

## API Host

Status: placeholder

`AshenWar.Api/Program.cs` still contains the default weather forecast endpoint and only registers OpenAPI. It does not currently appear to:

- call `AddApplicationServices`
- call `AddInfrastructure`
- map `GameHub`
- configure authentication/authorization
- load JSON definitions through `DefinitionLoader`
- expose endpoints for auth, lobby, roster selection, deployment, planning, or match state

This is likely the largest missing end-to-end integration point.

## Dependency Injection Registration

Status: partially implemented

`ApplicationServiceRegistration` registers MediatR but leaves action handler/executor registration commented or unfinished. Many Application services require concrete dependencies such as handlers, registries, executors, channels, timers, repositories, logging, and SignalR contexts.

`InfrastructureServiceRegistration` contains Mongo, Postgres, and definition registrations, but should be checked against the language/features supported by the configured compiler because it uses extension block syntax.

Before relying on runtime composition, confirm all required services can be resolved from the final API host.

## Definition Loading

Status: implementation present, startup wiring unclear

`DefinitionLoader` can load effects, abilities, conditions, tiles, units, maps, and match definitions from JSON folders. The expected root layout is a `definitions` directory with subfolders such as:

- `effects`
- `abilities/passive`
- `abilities/active`
- `conditions`
- `tiles`
- `units`
- `maps`
- `matches`

The loader comments say to call `LoadAll()` once before `app.Run()`, but the API host does not currently do that. Confirm where definition files should live in deployed output and whether they are copied by project files.

## Persistence

Status: adapters present, integration needs confirmation

Infrastructure includes:

- Mongo match repository
- Mongo serializer configuration
- Postgres EF Core context
- user repository
- refresh token repository
- JWT token service

Still confirm:

- connection strings in each environment
- migration strategy for Postgres
- serialization coverage for all Domain types stored in Mongo
- whether match persistence handles newly added/renamed domain types
- whether repositories are registered with the intended lifetimes

## SignalR and Realtime Events

Status: application usage present, host wiring unclear

Application services broadcast events such as lobby found, roster selected, match created, initiative rolled, ability failed, resolution completed, planning started, and match ended.

`GameHub` exists, but the API host does not currently map the hub. Also verify group/user id formatting; some code uses `matchId.Value.ToString()` while `GameHub.JoinMatch` uses `matchId.ToString()`.

## Planning Timer Flow

Status: present but needs integration review

Planning and resolution services use `ChannelWriter<TimerMessage>` with `StartTimer` and `CancelTimer` messages. `PlanningTimerService` should be reviewed with the final DI setup to confirm it is registered as a hosted service and receives the same channel used by planning/resolution.

Placeholder questions:

- What happens when the server restarts mid-planning?
- Should timers be reconstructed from persisted match state?
- Are expired turns resolved with empty orders, default orders, or last known submitted orders?

## Resolution Pipeline

Status: substantial implementation, still evolving

`ResolutionService` currently coordinates:

- begin resolution
- match triggers
- initiative
- active ability execution
- unit/tile/global condition ticks
- global event application
- end resolution
- outcome evaluation
- next-turn planning
- persistence and SignalR broadcast

Areas to confirm:

- failure handling when one unit/order/ability is invalid during resolution
- whether resolution batches preserve enough data for replay/client animation
- whether trigger execution should run after every major action, batch, or lifecycle transition
- whether random seeding is stable enough for deterministic replay

## Action and Handler Coverage

Status: broad surface, likely incomplete registration/tests

There are many action definition types and matching handler folders for ability, condition, cost, movement, resource, tile, and unit behavior. Because the worktree contains many active renames/additions around actions and cooldowns, verify:

- every `IActionDefinition` has exactly one registered handler
- polymorphic JSON registration includes all definition types
- old action names are not still referenced by definitions or DTOs
- handler behavior emits expected domain events through touched entities

## Tests

Status: configured but sparse

`AshenWar.Tests` is configured with xUnit and references Domain. Source tests are not currently visible in the file list.

Useful first test targets:

- `Board` placement/removal invariants
- `Match` phase transitions
- unit/tile/global condition stacking
- modifier calculation ordering
- active ability affordability
- action handler dispatch coverage
- definition loader smoke tests
- resolution service behavior around invalid/partial orders

## Existing AI Guidance Drift

Status: useful but partly stale

`CLAUDE.md` contains helpful architecture notes, but some project paths and type names appear older than the current repository. Use it as intent/context, then verify against current source before making changes.

