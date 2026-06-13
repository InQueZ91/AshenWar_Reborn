// ── Demo Map ────────────────────────────────────────────────────────
let demoMapActive = false;
let demoPlanActive = false;
let demoOrderActive = false;

const DEMO_MAP = (() => {
    // Corridor map matching your test JSON — 13 tiles from (-3,0) to (3,0) with neighbors
    const tiles = [
        {q:-3,r:0},{q:-2,r:0},{q:-1,r:0},{q:0,r:0},{q:1,r:0},{q:2,r:0},{q:3,r:0},
        {q:-2,r:-1},{q:-1,r:-1},{q:0,r:-1},{q:1,r:-1},
        {q:-1,r:1},{q:0,r:1},{q:1,r:1},
    ];
    const mySpawns  = [{q:-2,r:0},{q:-3,r:0}];
    const opSpawns  = [{q:2,r:0},{q:3,r:0}];
    return {tiles, mySpawns, opSpawns};
})();

const DEMO_UNITS = [
    {
        unitId:       '00000000-0000-0000-0000-000000000001',
        definitionId: '00000000-0000-0000-0001-000000000001',
        resources:    { health: 15, stamina: 8, steps: 2 },
        finalStats:   { health: 30, stamina: 10, steps: 10, speed: 20, vision: 2, power: 12 },
        abilities: [
            { id: '00000000-0000-0000-0002-000000000001', remainingCooldown: 0 },
            { id: '00000000-0000-0000-0002-000000000002', remainingCooldown: 0 },
            { id: '00000000-0000-0000-0002-000000000003', remainingCooldown: 0 },
        ],
    },
    {
        unitId:       '00000000-0000-0000-0000-000000000002',
        definitionId: '00000000-0000-0000-0001-000000000002',
        resources:    { health: 28, stamina: 5, steps: 0 },
        finalStats:   { health: 40, stamina: 6, steps: 6, speed: 8, vision: 2, power: 20 },
        abilities: [
            { id: '00000000-0000-0000-0002-000000000004', remainingCooldown: 0 },
            { id: '00000000-0000-0000-0002-000000000005', remainingCooldown: 0 },
        ],
    },
    {
        unitId:       '00000000-0000-0000-0000-000000000003',
        definitionId: '00000000-0000-0000-0001-000000000003',
        resources:    { health: 10, stamina: 3, steps: 4 },
        finalStats:   { health: 20, stamina: 4, steps: 8, speed: 14, vision: 3, power: 8 },
        abilities: [
            { id: '00000000-0000-0000-0002-000000000006', remainingCooldown: 0 },
        ],
    },
];

function toggleDebugDeploy() {
    demoMapActive = !demoMapActive;
    setDeployZoneActive(demoMapActive);
    const btn = document.getElementById('btnDemoMap');

    if (demoMapActive) {
        // Populate unit dropdown with demo units if cache is empty
        if (!Object.keys(unitDefinitionCache).length) {
            const demoUnits = [
                {id:'00000000-0000-0000-0001-000000000001', name:'Knight',  baseStats:{power:12}},
                {id:'00000000-0000-0000-0001-000000000002', name:'Archer',  baseStats:{power:8}},
                {id:'00000000-0000-0000-0001-000000000003', name:'Cleric',  baseStats:{power:10}},
            ];
            demoUnits.forEach(u => unitDefinitionCache[u.id] = u);
            log('Demo units loaded', 'info');
        }

        // Simulate LobbyFounded state
        mySide = 'Blue';
        currentPowerLimit = 100;
        setSide('Blue');
        setPhase('Lobby', 'active');
        updatePowerBar(0);
        document.getElementById('btnSubmitDeployment').disabled = false;
        document.getElementById('btnCancelDeployment').disabled = false;

        HEX.setData(DEMO_MAP.tiles, DEMO_MAP.mySpawns, DEMO_MAP.opSpawns);

        btn.textContent = 'ON';
        btn.style.borderColor = 'var(--green-b)';
        btn.style.color = 'var(--green)';
        log('Demo map loaded', 'info');
    }
    else {
        resetMap();
        currentPowerLimit = 0;
        updatePowerBar(0);
        setPhase('—');
        setSide('—');
        document.getElementById('btnSubmitDeployment').disabled = true;
        document.getElementById('btnCancelDeployment').disabled = true;

        btn.textContent = 'DeployDemo';
        btn.style.borderColor = '';
        btn.style.color = '';
        log('Demo map cleared', 'info');
    }
}

function toggleDebugPlan() {
    demoPlanActive = !demoPlanActive;
    const btn = document.getElementById('btnDemoPlan');

    if (demoPlanActive) {
        document.getElementById('lobbyCard').classList.toggle('hidden');
        document.getElementById('orderCard').classList.toggle('hidden');

        btn.textContent = 'ON';
        btn.style.borderColor = 'var(--green-b)';
        btn.style.color = 'var(--green)';
        log('Demo plan loaded', 'info');

        DEMO_UNITS.forEach(u => {addUnitRow(u)});
    }
    else {
        document.getElementById('lobbyCard').classList.toggle('hidden');
        document.getElementById('orderCard').classList.toggle('hidden');

        btn.textContent = 'PlanDemo';
        btn.style.borderColor = '';
        btn.style.color = '';
        log('Demo plan cleared', 'info');

        clearUnitList();
    }
}

function toggleDebugOrder() {
    demoOrderActive = !demoOrderActive;
    const btn = document.getElementById('btnDemoOrder');

    if (demoOrderActive) {
        loadDemoOrders();
        updateOrderList('00000000-0000-0000-0000-000000000001')
        updateOrderList('00000000-0000-0000-0000-000000000002')
        updateOrderList('00000000-0000-0000-0000-000000000003')

        btn.textContent = 'ON';
        btn.style.borderColor = 'var(--green-b)';
        btn.style.color = 'var(--green)';
        log('Demo order loaded', 'info');
    }
    else {
        clearDemoOrders();
        updateOrderList('00000000-0000-0000-0000-000000000001')
        updateOrderList('00000000-0000-0000-0000-000000000002')
        updateOrderList('00000000-0000-0000-0000-000000000003')

        btn.textContent = 'DemoOrder';
        btn.style.borderColor = '';
        btn.style.color = '';
        log('Demo order cleared', 'info');
    }
}

function loadDemoOrders() {
    unitOrders.length   = 0;
    unitOrderCounter    = 0;
    abilityOrderCounter = 0;

    const u1 = createUnitOrder('00000000-0000-0000-0000-000000000001');
    u1.abilityOrders.push(
        { _id: abilityOrderCounter++, abilityId: '00000000-0000-0000-0002-000000000001', abilityName: 'Move',         selections: [{ abilityStepId: '00000000-0000-0000-0003-000000000001', targetType: 1, targetId: 'tile-a8b2c3d4' }] },
        { _id: abilityOrderCounter++, abilityId: '00000000-0000-0000-0002-000000000002', abilityName: 'Flame Strike',  selections: [{ abilityStepId: '00000000-0000-0000-0003-000000000002', targetType: 0, targetId: '00000000-0000-0000-0000-000000000002' }] },
        { _id: abilityOrderCounter++, abilityId: '00000000-0000-0000-0002-000000000003', abilityName: 'Ember Aura',    selections: [] }
    );

    const u2 = createUnitOrder('00000000-0000-0000-0000-000000000002');
    u2.abilityOrders.push(
        { _id: abilityOrderCounter++, abilityId: '00000000-0000-0000-0002-000000000004', abilityName: 'Shield Bash',  selections: [{ abilityStepId: '00000000-0000-0000-0003-000000000003', targetType: 0, targetId: '00000000-0000-0000-0000-000000000001' }] },
        { _id: abilityOrderCounter++, abilityId: '00000000-0000-0000-0002-000000000005', abilityName: 'Charge',        selections: [{ abilityStepId: '00000000-0000-0000-0003-000000000004', targetType: 0, targetId: '00000000-0000-0000-0000-000000000001' }, { abilityStepId: '00000000-0000-0000-0003-000000000005', targetType: 1, targetId: 'tile-d4e5f6a7' }] }
    );

    const u3 = createUnitOrder('00000000-0000-0000-0000-000000000003');

    unitOrders.push(u1, u2, u3);
}

function clearDemoOrders() {
    unitOrders.length    = 0;
    unitOrderCounter     = 0;
    abilityOrderCounter  = 0;
    pendingAbilityOrder  = null;
}
