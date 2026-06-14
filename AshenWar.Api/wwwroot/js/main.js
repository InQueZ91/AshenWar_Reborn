import { state, BASE_URL } from './state.js';
import { connect } from './hub.js';
import { HEX } from './hex.js';

// ── Init ───────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', async() => {
    log('Ashen War Demo', 'info');
    setDeployZoneActive(false)
    await fetchUnitDefinitions();

    HEX.onSpawnPicked = (rowId, q, r) => {
        const alreadyTaken = [...document.querySelectorAll('#deploy-list .deploy-row')]
            .some(row => {
                if (row.id === rowId) return false;
                return parseInt(row.dataset.q, 10) === q && parseInt(row.dataset.r, 10) === r;
            });
        if (alreadyTaken) {
            document.getElementById('mapTargetMode').textContent = 'Tile already occupied — pick another spawn';
            return;
        }
        const row = document.getElementById(rowId);
        if (!row) return;
        row.dataset.q = q;
        row.dataset.r = r;
        const badge = row.querySelector('.pos-badge');
        if (badge) {
            badge.textContent = `${q}, ${r}`;
            badge.classList.add('set');
        }
        HEX.exitPickMode();
        HEX.setChosenSpawns(collectChosenSpawns());
    };

    HEX.onPickModeChange = (active, rowId) => {
        const badge = rowId ? document.querySelector(`#${rowId} .pos-badge`) : null;
        if (active) {
            badge?.classList.add('picking');
            document.getElementById('mapTargetMode').classList.remove('hidden');
            document.getElementById('mapHint').classList.add('hidden');
        } else {
            badge?.classList.remove('picking');
            document.getElementById('mapTargetMode').classList.add('hidden');
            document.getElementById('mapHint').classList.remove('hidden');
        }
    };

    HEX.setupCanvas();
    resizeCanvas();
    HEX.render();
    window.addEventListener('resize', resizeCanvas);
    if (loadSession()) {
        connect();
    }
});


// ── Utilities ──────────────────────────────────────────────────

/** Unwrap a Guid value object or pass through a plain string. */
export function gid(v) {
    return typeof v === 'object' ? (v?.value ?? v?.Value ?? JSON.stringify(v)) : String(v ?? '');
}

/** Short display form of an id. */
export function shortId(v) {
    return gid(v).slice(0, 8) + '…';
}

/**
 * Normalise a camelCase/PascalCase field pair from a server DTO.
 * e.g. field(u, 'unitId') reads u.unitId ?? u.UnitId
 */
export function field(obj, key) {
    const pascal = key[0].toUpperCase() + key.slice(1);
    return obj?.[key] ?? obj?.[pascal];
}

/** HTML-escape a value for safe insertion. */
export function esc(v) {
    return String(v).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

export function normalizeId(v) {
    return gid(v).toLowerCase().replace(/-/g, '');
}


// ── Session ──────────────────────────────────────────────────

function parseUsername(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.unique_name
            ?? payload.name
            ?? payload.email
            ?? null;
    } catch {
        return null;
    }
}

function parseUserId(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.sub
            ?? payload.nameid
            ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
            ?? null;
    } catch {
        return null;
    }
}

function saveSession() {
    const rememberMe = document.getElementById('rememberMe').checked;
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem('accessToken', state.accessToken);
    storage.setItem('refreshToken', state.refreshToken);
}

function clearSession() {
    // Clear local storage
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');

    // Clear session storage
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');

    // Clear state
    state.accessToken = null;
    state.refreshToken = null;
    setConnected(false);
}

function loadSession() {
    state.accessToken = localStorage.getItem('accessToken') ?? sessionStorage.getItem('accessToken');
    state.refreshToken = localStorage.getItem('refreshToken') ?? sessionStorage.getItem('refreshToken');

    if (state.accessToken) {
        log('Session restored', 'info');
        state.myUserId = parseUserId(state.accessToken);
        state.myUsername = parseUsername(state.accessToken);
    }

    return state.accessToken != null; // true if we have a valid session
}


// ── Connection ──────────────────────────────────────────────────────
async function login(email, password) {
    try {
        const res = await fetch(`${BASE_URL}/api/auth/login`, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({email, password})
        });

        if (!res.ok) {
            const text = await res.text();
            setLoginStatus('Wrong email or password', true);
            log(`Login failed: HTTP ${res.status} ${text} ${text}`, 'err');
            return false;
        }

        const data = await res.json();
        state.accessToken = data.accessToken ?? data.AccessToken;
        state.refreshToken = data.refreshToken ?? data.RefreshToken;
        state.myUserId = parseUserId(state.accessToken);
        state.myUsername = parseUsername(state.accessToken);

        saveSession();
        return true;
    } catch (e) {
        setLoginStatus('Could not reach server' + e, true);
        return false;
    }
}

async function loginAndConnect() {
    const email = document.getElementById('authEmail').value.trim();
    const password = document.getElementById('authPassword').value.trim();

    if (!email || !password) {
        setLoginStatus('Email and password required', true);
        return;
    }

    setLoginStatus('Logging in...');
    const ok = await login(email, password);
    if (!ok) return;

    await connect();
}

async function logout() {
    clearSession();

    if (state.connection) {
        await state.connection.stop();
        state.connection = null;
    }

    resetPage();
}


// ── API fetches ──────────────────────────────────────────────────────
async function fetchUnitDefinitions() {
    try {
        const res = await fetch(`${BASE_URL}/api/definitions/units`);
        if (!res.ok) {
            log(`fetchUnitDefinitions HTTP ${res.status}`, 'err');
            return;
        }
        const units = await res.json();
        log(`Unit definition cached - ${units.length} units`, 'info');
        units.forEach(u => {
            state.unitDefinitionCache[u.id] = u;
            log(`  → ${u.name}`, 'info');
        });
    } catch (e) {
        log(`fetchUnitDefinitions error: ${e}`, 'err');
    }
}

export async function fetchMapDefinition(mapDefinitionId) {
    if (!mapDefinitionId) {
        log('fetchMapDefinition: no mapDefinitionId', 'err');
        return;
    }
    try {
        const res = await fetch(`${BASE_URL}/api/definitions/maps/${mapDefinitionId}`, {
            headers: {'Authorization': 'Bearer ' + state.accessToken}
        });
        if (!res.ok) {
            log(`fetchMapDefinition HTTP ${res.status}`, 'err');
            return;
        }
        const data = await res.json();
        // Response shape: { tiles: [position:{q,r}, tileDefinitionId}], deploymentPoints:{Blue:[],Red:[]} }
        const tiles = (field(data, 'tiles') ?? []).map(t => {
            const pos = field(t, 'position') ?? {};
            return {q: pos.q ?? pos.Q ?? 0, r: pos.r ?? pos.R ?? 0, id: gid(field(t, 'tileDefinitionId'))};
        });
        const points = field(data, 'deploymentPoints') ?? {};
        const myPoints = (points[state.mySide] ?? points[state.mySide.toLowerCase()] ?? []).map(p => ({q: p.q ?? p.Q, r: p.r ?? p.R}));
        const opSide = state.mySide === 'Blue' ? 'Red' : 'Blue';
        const opPoints = (points[opSide] ?? points[state.mySide.toLowerCase()] ?? []).map(p => ({q: p.q ?? p.Q, r: p.r ?? p.R}));

        log(`Map loaded - ${tiles.length} tiles`, 'info');
        HEX.setData(tiles, myPoints, opPoints);
        document.getElementById('mapHint').textContent = `${tiles.length} tiles · ▲ your spawns · ▼ opponent spawns`;
        document.getElementById('mapPhaseLabel').textContent = 'Deploy';
    } catch (e) {
        log(`fetchMapDefinition error: ${e}`, 'err');
    }
}

export async function fetchMatchDefinition(matchDefinitionId) {
    if (!matchDefinitionId) {
        log('fetchMatchDefinition: no matchDefinitionId', 'err');
        return;
    }
    try {
        const res = await fetch(`${BASE_URL}/api/definitions/matches/${matchDefinitionId}`);
        if (!res.ok) {
            log(`fetchMatchDefinition HTTP ${res.status}`, 'err');
            return;
        }
        const data = await res.json();
        state.currentPowerLimit = field(data, 'powerLimit') ?? 0;
        log(`Match definition loaded — power limit ${state.currentPowerLimit}`, 'info');
        updatePowerBar(0); // re-render bar now that we have the limit
    } catch (e) {
        log(`fetchMatchDefinition error: ${e}`, 'err');
    }
}


// ── Panel Helpers ──────────────────────────────────────────────────────
function showPanel(name) {
    document.getElementById('lobbyCard').classList.toggle('hidden', name !== 'lobby');
    document.getElementById('orderCard').classList.toggle('hidden', name !== 'order');
}

export function resetMap() {
    HEX.reset();
    document.getElementById('mapHint').textContent = 'Waiting for match...';
    document.getElementById('mapPhaseLabel').textContent = '—';
}

function collectChosenSpawns() {
    return [...document.querySelectorAll('#deploy-list .deploy-row')]
        .filter(row => !isNaN(parseInt(row.dataset.q, 10)))
        .map(row => ({q: parseInt(row.dataset.q, 10), r: parseInt(row.dataset.r, 10)}));
}

function resetPage() {
    state.mySide = null;

    // Connection panel
    setLoginStatus('—');
    clearConnectionState();
    showLoginPanel();

    // Lobby panel
    setQueueButtons(false);
    setQueueStatus('—');

    setDeployZoneActive(false);
    setDeployButtons(false);
    setDeployStatus('—');

    // Canvas
    resetMap();
}


// ── Connection Panel ──────────────────────────────────────────────────────────
export function setConnected(yes) {
    setConnectionStatus(yes);
    setUsername(state.myUsername ?? state.myUserId ?? '—');
    showConnectedPanel();
    setQueueButtons(yes);
}

function setConnectionStatus(bool) {
    document.getElementById('dot').className = 'dot' + (bool ? ' on' : '');
    document.getElementById('statusText').textContent = bool ? 'Connected' : 'Disconnected';
}

function showLoginPanel() {
    document.getElementById('loginSection').classList.remove('hidden');
    document.getElementById('connectedSection').classList.add('hidden');
}

function setLoginStatus(msg, isError = false) {
    const el = document.getElementById('loginStatus');
    el.textContent = msg;
    el.className = isError ? 'status-span-err' : 'status-span';
}

function showConnectedPanel() {
    document.getElementById('loginSection').classList.add('hidden');
    document.getElementById('connectedSection').classList.remove('hidden');
}

export function setPhase(p, style = '') {
    const el = document.getElementById('csPhase');
    el.textContent = p;
    el.className = 'phase-badge' + (style ? ' ' + style : '');
}

function setUsername(u) {
    document.getElementById('csUsername').textContent = u;
}

export function setLobbyId(id) {
    document.getElementById('csLobby').textContent = shortId(id);
    state.lobbyId = id;
}

export function setSide(v) {
    state.mySide = v;
    document.getElementById('csSide').textContent = v;
}

export function setMatchId(id) {
    document.getElementById('csMatch').textContent = shortId(id);
    state.matchId = id;
}

function clearConnectionState() {
    setUsername('—');
    setPhase('—');
    setSide('—');
    setLobbyId('—');
    setMatchId('—');
}


// ── Log Panel ──────────────────────────────────────────────────────
export function log(msg, type = '') {
    const t = new Date().toTimeString().slice(0, 8);
    const dir = {send: '→', recv: '←', err: '✕', info: '·'}[type] || '·';
    appendLog(`<div class="log-row ${type}"> <span class="t">${t}</span> <span class="d">${dir}</span> <span class="m">${esc(msg)}</span> </div>`);
}

function appendLog(html) {
    document.getElementById('log').insertAdjacentHTML('beforeend', html);
    bumpCount();
    maybeScroll();
}

function clearLog() {
    document.getElementById('log').innerHTML = '';
    const el = document.getElementById('logCount');
    if (el) el.textContent = '0';
}

function bumpCount() {
    const el = document.getElementById('logCount');
    if (el) el.textContent = parseInt(el.textContent || '0') + 1;
}

function maybeScroll() {
    if (document.getElementById('autoScroll')?.checked) {
        const el = document.getElementById('log');
        el.scrollTop = el.scrollHeight;
    }
}


// ── Lobby Panel ──────────────────────────────────────────────────────
export function setDeployZoneActive(active) {
    document.querySelector('.deploy-zone')
        .classList.toggle('inactive', !active);
    setDeployButtons(active);
}

function setQueueButtons(enable) {
    document.getElementById('btnJoinQueue').disabled = !enable;
    document.getElementById('btnLeaveQueue').disabled = !enable;
}

function setDeployButtons(enable) {
    document.getElementById('btnSubmitDeployment').disabled = !enable;
    document.getElementById('btnCancelDeployment').disabled = !enable;
}

export function setQueueStatus(msg, isError = false) {
    const el = document.getElementById('queueStatus');
    el.textContent = msg;
    el.className = isError ? 'status-span-err' : 'status-span';
}

export function updatePowerBar(totalPower) {
    const fill = document.getElementById('powerBarFill');
    const text = document.getElementById('powerBarText');
    if (!fill || !text) return;

    const limit = state.currentPowerLimit;
    const ratio = Math.min((totalPower / limit) * 100, 100);
    const percent = limit > 0 ? ratio : 0;

    fill.style.width = percent + '%';
    text.textContent = limit > 0 ? `${totalPower} / ${limit}` : `${totalPower} / —`;

    const over = limit > 0 && totalPower > limit;
    fill.classList.toggle('over', over);
    setDeployStatus(over ? 'Power limit exceeded!' : '–', over);
    document.getElementById('btnSubmitDeployment').disabled = over;
    document.getElementById('btnCancelDeployment').disabled = over;
}

function onDeploymentUnitChanged() {
    const rows = document.querySelectorAll('#deploy-list .deploy-row');
    let totalPower = 0;
    rows.forEach(row => {
        const select = row.querySelector('.unit-select');
        if (!select || !select.value) return;
        const unit = state.unitDefinitionCache[select.value];
        if (unit) totalPower += unit.baseStats.power;
    });
    updatePowerBar(totalPower);
    HEX.render();
}

function addDeploymentPlan(coord = null) {
    const rowId = state.deployRowCounter++;
    const q = coord ? coord.q : '';
    const r = coord ? coord.r : '';

    // Build dropdown options from cache
    const options = Object.values(state.unitDefinitionCache)
        .map(u => `<option value="${u.id}">${esc(u.name)} (${u.baseStats.power} pw)</option>`)
        .join('');
    const placeholder = options.length ? '' : '<option value="">No units loaded</option>';

    const row = document.createElement('div');
    row.className = 'deploy-row';
    row.dataset.rid = rowId;
    row.id = `deploy-row-${rowId}`;
    row.innerHTML =
        `<select class="unit-select" onchange="onDeploymentUnitChanged()">${placeholder}${options}</select>` +
        `<span class="pos-badge" id="pos-${rowId}" title="Click to pick from map" onclick="HEX.enterPickMode('deploy-row-${rowId}')">—</span>` +
        `<button class="del-btn" onclick="removeDeploymentPlan(${rowId})">×</button>`;
    document.getElementById('deploy-list').appendChild(row);
    onDeploymentUnitChanged();
}

function removeDeploymentPlan(rid) {
    document.querySelector(`.deploy-row[data-rid="${rid}"]`)?.remove();
    onDeploymentUnitChanged();
    HEX.setChosenSpawns(collectChosenSpawns());
}

export function buildDeploymentPlan() {
    const out = [];
    for (const row of document.querySelectorAll('#deploy-list .deploy-row')) {
        const uid = row.querySelector('.unit-select')?.value?.trim();
        const q = parseInt(row.dataset.q, 10);
        const r = parseInt(row.dataset.r, 10);
        if (!uid) {
            log('Deployment plan missing unit', 'err');
            return null;
        }
        if (isNaN(q) || isNaN(r)) {
            log('Deployment plan missing position — pick a spawn tile', 'err');
            return null;
        }
        out.push({unitDefinitionId: uid, q, r});
    }
    return out;
}

export function setDeployStatus(msg, isError = false) {
    const el = document.getElementById('deployStatus');
    el.textContent = msg;
    el.className = isError ? 'status-span-err' : 'status-span';
}


// ── Unit List Panel ──────────────────────────────────────────────────────

export function createUnitOrder(unitId = '') {
    return {
        _id:           state.unitOrderCounter++,
        unitId:        unitId,
        abilityOrders: [],
    };
}

function createAbilityOrder(abilityId = '', abilityName = '') {
    return {
        _id:         state.abilityOrderCounter++,
        abilityId:   abilityId,
        abilityName: abilityName,
        selections:  [],
    };
}

function createSelection(abilityStepId = '', targetType = 2, targetId = null) {
    return {
        abilityStepId: abilityStepId,  // Guid string
        targetType:    targetType,     // 0=Unit, 1=Tile, 2=Self
        targetId:      targetId,       // Guid string | null
    };
}

function collectUnitOrders() {
    return state.unitOrders.map(o => ({
        unitId: o.unitId,
        abilityOrders: o.abilityOrders.map(a => ({
            abilityId:  a.abilityId,
            selections: a.selections.map(s => ({
                abilityStepId: s.abilityStepId,
                targetType:    s.targetType,
                targetId:      s.targetId,
            })),
        })),
    }));
}

function addUnitOrder(unitId = '') {
    state.unitOrders.push(createUnitOrder(unitId));
}

function removeUnitOrder(unitId = '') {
    const idx = state.unitOrders.findIndex(o => o.unitId === unitId);
    if (idx !== -1) state.unitOrders.splice(idx, 1);
}

function addAbilityOrder(unitId, abilityId, abilityName = '') {
    const unit = state.unitOrders.find(o => o.unitId === unitId);
    if (!unit) {
        log(`addAbilityOrder: no order for unit ${unitId}`, 'err');
        return;
    }
    unit.abilityOrders.push(createAbilityOrder(abilityId, abilityName));
    updateOrderList(unitId);
}

function removeAbilityOrder(unitId, fromIndex) {
    const unit = state.unitOrders.find(o => o.unitId === unitId);
    if (!unit) {
        log(`removeAbilityOrder: no order for unit ${unitId}`, 'err');
        return;
    }

    const target = unit.abilityOrders[fromIndex];
    if (!target) {
        log(`removeAbilityOrder: no ability order at index ${fromIndex}`, 'err');
        return;
    }

    unit.abilityOrders.splice(fromIndex);
    updateOrderList(unitId);

    // re-enter add flow with that ability pre-selected
    selectAbility(target.abilityId, target.abilityName);
}

function addSelection(unitId, abilityOrderId, abilityStepId, targetType, targetId) {
    const unit = state.unitOrders.find(o => o.unitId === unitId);
    if (!unit) {
        log(`addSelection: no order for unit ${unitId}`, 'err');
        return;
    }
    const abilityOrder = unit.abilityOrders.find(a => a._id === abilityOrderId);
    if (!abilityOrder) {
        log(`addSelection: no ability order ${abilityOrderId}`, 'err');
        return;
    }
    abilityOrder.selections.push(createSelection(abilityStepId, targetType, targetId));
}

function removeSelection(unitId, abilityOrderId, selIdx) {
    const unit = state.unitOrders.find(o => o.unitId === unitId);
    if (!unit) {
        log(`removeSelection: no order for unit ${unitId}`, 'err');
        return;
    }
    const abilityOrder = unit.abilityOrders.find(a => a._id === abilityOrderId);
    if (!abilityOrder) {
        log(`removeSelection: no ability order ${abilityOrderId}`, 'err');
        return;
    }
    abilityOrder.selections.splice(selIdx, 1);
}

// UI
export function addUnitRow(u) {
    const defId = field(u, 'definitionId');
    if (!defId) {
        log('Order plan missing unit definition ID', 'err');
        return;
    }

    const def      = state.unitDefinitionCache[defId];
    const name     = field(def, 'name') ?? 'Unknown Unit';
    const unitId   = gid(field(u, 'unitId'));
    const finalStats = field(u, 'finalStats');
    const speed    = field(finalStats, 'speed');

    const row = document.createElement('div');
    row.className      = 'unit-row';
    row.dataset.unitId = unitId;
    row.innerHTML = `
        <div class="unit-header">
            <label>
                <input type="checkbox">
                <span class="unit-speed">${speed}</span>
                <span class="unit-name">${esc(name)}</span>
            </label>
        </div>
        <div class="order-list"></div>
    `;

    row.addEventListener('click', () => selectUnit(row, u));
    document.getElementById('unitList').appendChild(row);
}

export function clearUnitList() {
    document.getElementById('unitList').innerHTML = '';
    state.selectedUnit  = null;
    state.activeUnitRow = null;
}

function selectUnit(row, u) {
    if (state.activeUnitRow) {
        state.activeUnitRow.classList.remove('selected');
        state.activeUnitRow.querySelector('.unit-header input').checked = false;
    }
    state.activeUnitRow = row;
    state.selectedUnit  = u;
    row.classList.add('selected');
    row.querySelector('.unit-header input').checked = true;
    showUnitDetail(u);
}

export function updateOrderList(uid) {
    const unitRow = document.querySelector(`.unit-row[data-unit-id="${uid}"]`);
    if (!unitRow) {
        log(`updateOrderList: no unit row for unit ${uid}`, 'err');
        return;
    }

    const orderList = unitRow.querySelector('.order-list');
    if (!orderList) {
        log(`updateOrderList: no order list for unit ${uid}`, 'err');
        return;
    }

    orderList.innerHTML = '';

    const unitOrder = state.unitOrders.find(o => o.unitId === uid);
    if (!unitOrder) {
        log(`updateOrderList: no order for unit ${uid}`, 'err');
        return;
    }

    for (const [index, abilityOrder] of unitOrder.abilityOrders.entries()) {
        const row = document.createElement('div');
        row.className = 'order-row';
        row.innerHTML = `
            <span class="order-seq">${index + 1}</span>
            <div class="order-content">
                <span class="order-name">${esc(abilityOrder.abilityName ?? shortId(abilityOrder.abilityId))}</span>
                <div class="order-selections"></div>
            </div>
            <button class="order-btn remove" onclick="removeAbilityOrder('${uid}', ${index})">×</button>
        `;

        const selectionsContainer = row.querySelector('.order-selections');
        for (const selection of abilityOrder.selections) {
            const targetType = selection.targetType === 0 ? 'Unit'
                : selection.targetType === 1 ? 'Tile'
                    : 'Self';
            const selectionElement = document.createElement('span');
            selectionElement.className   = 'order-selection';
            selectionElement.textContent = `${targetType} ${selection.targetId ? shortId(selection.targetId) : ''}`;
            selectionsContainer.appendChild(selectionElement);
        }

        orderList.appendChild(row);
    }
}


// ── Unit Detail Panel ──────────────────────────────────────────────────────
function showUnitDetail(u) {
    state.selectedUnit = u;

    // Show panel
    document.getElementById('unitDetailCard').classList.remove('hidden');

    // Always default to ability tab
    switchTab('abilities');

    // Populate both tabs
    buildAbilityTab(u);
    buildInfoTab(u);
}

function switchTab(tab) {
    document.getElementById('tabAbilities').classList.toggle('active', tab === 'abilities');
    document.getElementById('tabContentAbilities').classList.toggle('hidden', tab !== 'abilities');
    document.getElementById('tabInfo').classList.toggle('active', tab === 'info');
    document.getElementById('tabContentInfo').classList.toggle('hidden', tab !== 'info');
}

function buildAbilityTab(u) {
    if (!u) {
        log('No unit selected', 'err');
        return;
    }

    const grid = document.getElementById('abilityGrid');
    grid.innerHTML = '';

    const abilities = field(u, 'abilities') ?? [];
    if (!abilities.length) {
        grid.innerHTML = '<span style="font-family:var(--mono);font-size:11px;color:var(--text-3);">No abilities</span>';
        return;
    }

    const resources  = field(u, 'resources');
    const finalStats = field(u, 'finalStats');

    // Build ability grid
    for (const a of abilities) {
        const abilityId = gid(field(a, 'abilityId'));
        const name = field(a, 'name') ?? 'Ability';
        const cooldown = field(a, 'remainingCooldown') ?? 0;
        const isReady = cooldown <= 0;

        const btn = document.createElement('button');
        btn.className = 'ability-btn ' + (isReady ? 'available' : 'unavailable');
        btn.dataset.abilityId = abilityId;
        btn.innerHTML = `<span class="name">${name}</span>`;

        if (isReady) {
            btn.addEventListener('click', () => selectAbility(btn));
        }

        grid.appendChild(btn);
    }

    // Add dummy rows to fill out the grid
    for (let i = grid.childElementCount; i < 4; i++) {
        const btn = document.createElement('button');
        btn.className = 'ability-btn unavailable';
        btn.innerHTML = `<span class="icon">⚔️</span>`;
        grid.appendChild(btn);
    }

    const curHp = field(resources, 'health');
    const finalHp = field(finalStats, 'health');
    const curStamina = field(resources, 'stamina');
    const finalStamina = field(finalStats, 'stamina');
    const curSteps = field(resources, 'steps');
    const finalSteps = field(finalStats, 'steps');
    const hpPct = curHp / finalHp;

    // Health
    document.getElementById('health-seg-label').textContent = `Health (${curHp}/${finalHp})`;
    const healthBar = document.getElementById('health-fill');
    healthBar.classList.remove('danger');
    if (hpPct <= 0.25) {
        healthBar.classList.add('danger');
    }
    setSegBar('health-track', 'health-fill', curHp, finalHp);

    // Stamina and Steps
    document.getElementById('stamina-seg-label').textContent = `Stamina (${curStamina}/${finalStamina})`;
    document.getElementById('steps-seg-label').textContent = `Steps (${curSteps}/${finalSteps})`;

    setSegBar('stamina-track', 'stamina-fill', curStamina, finalStamina);
    setSegBar('steps-track', 'steps-fill', curSteps, finalSteps);
}

function selectAbility(btn) {
    if (btn.classList.contains('unavailable')) return;

    const isAlreadyActive = btn.classList.contains('active');

    // Toggle active state
    document.querySelectorAll('.ability-btn').forEach(b => {
        b.classList.remove('active');
        b.setAttribute('aria-pressed', 'false');
    });

    if (!isAlreadyActive) {
        btn.classList.add('active');
        btn.setAttribute('aria-pressed', 'true');
    }

    updateConfirm();
}

function updateConfirm() {
    const hasActive = !!document.querySelector('.ability-btn.active');
    const confirmBtn = document.getElementById('confirm-btn');
    confirmBtn.classList.toggle('ready', hasActive && state.stagingComplete);
}

function confirmAction() {
    const activeBtn = document.querySelector('.ability-btn.active');
    if (!activeBtn || !state.stagingComplete){
        log('No ability selected or ability requirements not satisfy', 'err');
        return;
    }

    // 1. Read what needs to be dispatched
    const abilityName = activeBtn.dataset.abilityId;
    // steps

    // 2. Send this to queue
    // adds a row to the order panel
    // addOrder();

    // 3. Update UI
    activeBtn.classList.remove('active');
    activeBtn.setAttribute('aria-pressed', 'false');
    state.stagingComplete = false;
    updateConfirm();
}

function setSegBar(trackId, fillId, current, max) {
    const track = document.getElementById(trackId);
    const fill = document.getElementById(fillId);

    track.style.setProperty('--seg-w', (100 / max).toFixed(4) + '%');
    fill.style.width = (current / max * 100).toFixed(4) + '%';
}

function buildInfoTab(u) {
    if (!u) {
        log('No unit selected', 'err');
        return;
    }

    // Build final stats
    const finalStats = field(u, 'finalStats');
    document.getElementById('info-pow').textContent = finalStats.power;
    document.getElementById('info-hp').textContent = finalStats.health;
    document.getElementById('info-sta').textContent = finalStats.stamina;
    document.getElementById('info-ste').textContent = finalStats.steps;
    document.getElementById('info-spd').textContent = finalStats.speed;
    document.getElementById('info-vis').textContent = finalStats.vision;

    // Build passives
    const passiveList = document.getElementById('passiveList');
    passiveList.innerHTML = '';
    for (const p of field(u, 'passives') ?? []) {
        const row = document.createElement('div');
        row.className = 'passive-row';
        row.innerHTML = `
            <div class="passive-name">${p.name ?? '—'}</div>
            <div class="passive-desc">${p.description ?? '—'}</div>
        `;

        passiveList.appendChild(row);
    }

    // Build conditions
    const conditionList = document.getElementById('conditionList');
    conditionList.innerHTML = '';
    for (const c of field(u, 'conditions') ?? []) {
        const row = document.createElement('div');
        row.className = 'condition-row';
        row.innerHTML = `
            <div class="condition-left">
                <span class="condition-name">${c.name ?? '—'} x ${c.currentStacks}</span>
                <span class="condition-effect">${c.effect ?? '—'}</span>
            </div>
            <span class="condition-turns">${c.remainingDuration} turns</span>
        `;

        conditionList.appendChild(row);
    }
}


function resizeCanvas() {
    const wrap = document.querySelector('.map-canvas-wrap');
    const canvas = document.getElementById('hexCanvas');
    if (!wrap || !canvas) return;
    canvas.width = wrap.clientWidth;
    canvas.height = Math.max(280, Math.round(wrap.clientWidth * 0.65));
    HEX.render();
}

// ── Window (onclick hooks for non-module HTML) ──────────────────────────────
window.HEX = HEX;
Object.assign(window, {
    loginAndConnect, logout,
    addDeploymentPlan, removeDeploymentPlan, onDeploymentUnitChanged,
    switchTab, confirmAction, clearLog,
    removeAbilityOrder,
});
