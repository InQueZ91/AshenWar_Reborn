# AshenWar Codebase Notes

These docs are lightweight context references for developers and AI agents working in this unfinished codebase. They describe the current shape of the repository, the main architectural relationships, and areas that appear incomplete or in flux.

They are not intended to be exhaustive API documentation. Prefer updating these notes when a layer changes meaningfully, not when an individual method changes implementation detail.

## Documents

- [Codebase Overview](codebase-overview.md) - repo purpose, project layout, major runtime concepts, and where to look first.
- [Architecture Layers](architecture-layers.md) - responsibilities and relationships across Domain, Application, Infrastructure, API, and Tests.
- [Incomplete Areas](incomplete-areas.md) - known placeholders, unfinished integration points, and areas that need confirmation.

## Reading Order

Start with `codebase-overview.md`, then read `architecture-layers.md` for the layer boundaries. Use `incomplete-areas.md` before extending a feature so you do not assume every system is already wired end-to-end.

