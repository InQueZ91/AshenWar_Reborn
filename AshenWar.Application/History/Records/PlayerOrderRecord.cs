using System;
using System.Collections.Generic;

namespace AshenWar.Application.History.Records;

public sealed record PlayerOrderRecord(Guid OwnerId, IReadOnlyList<UnitOrderRecord> Orders);