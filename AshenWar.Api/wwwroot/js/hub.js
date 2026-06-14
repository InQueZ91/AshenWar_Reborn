import { state, BASE_URL } from './state.js';
import {
    log, field, gid, normalizeId,
    setConnected, setPhase, setSide, setLobbyId, setMatchId,
    setQueueStatus, setDeployZoneActive, setDeployStatus,
    resetMap, addUnitRow,
    fetchMapDefinition, fetchMatchDefinition,
    buildDeploymentPlan,
} from './main.js';

// ── Connection ──────────────────────────────────────────────────────
export async function connect() {
    if (!state.accessToken) {
        log('No access token — please log in first', 'err');
        return;
    }

    state.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${BASE_URL}/hubs/game`, {accessTokenFactory: () => state.accessToken})
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    await registerHandlers(state.connection);

    state.connection.onreconnecting(() => {
        log('Reconnecting…', 'info')
    });
    state.connection.onreconnected(() => {
        log('Reconnected', 'info');
        setConnected(true);
    });
    state.connection.onclose(e => {
        log('Closed' + (e ? ': ' + e : ''), 'err');
        setConnected(false);
    });

    try {
        log('Connecting to ' + BASE_URL, 'info');
        await state.connection.start();
        log('Connected', 'info');
        setConnected(true);
    } catch (e) {
        log('Failed: ' + e, 'err');
        document.getElementById('dot').className = 'dot err';
    }
}

async function registerHandlers(conn) {
    conn.on('QueueResult', d => {
        const waiting = field(d, 'isWaiting');
        const alreadyInLobby = field(d, 'isAlreadyInLobby');
        const found = field(d, 'IsFound');

        if (found){
            log('QueueResult: Lobby found !!!', 'recv');
        }
        else if (alreadyInLobby) {
            log('QueueResult: Already in lobby', 'recv');
            setQueueStatus('Already in lobby');
        }
        else if (waiting) {
            setPhase('Queued', 'active');
            setQueueStatus('Waiting for opponent');
            log('QueueResult: Waiting for opponent', 'recv');
        }
        else {
            log('QueueResult: No result', 'err');
        }
    });

    conn.on('LobbyFounded', d => {
        const lobbyId = gid(field(d, 'lobbyId'));
        const sideNum = field(d, 'yourSide');
        const sideStr = sideNum === 0 ? 'Blue' : sideNum === 1 ? 'Red' : '?';
        const matchDefId = gid(field(d, 'matchDefinitionId'));
        const mapDefId = gid(field(d, 'mapDefinitionId'));

        if (!lobbyId) {
            log('LobbyFounded: No lobby ID', 'err');
            return;
        }

        setLobbyId(lobbyId);
        setSide(sideStr);
        setPhase('Lobby', 'active');

        fetchMapDefinition(mapDefId);
        fetchMatchDefinition(matchDefId);

        setQueueStatus('Lobby founded');
        setDeployZoneActive(true);
        log(`LobbyFounded — side: ${sideStr}  lobby: ${lobbyId ? s(lobbyId) : '?'}`, 'recv');
    });

    conn.on('LobbyDeleted', () => {
        log('LobbyDeleted', 'err');
        setPhase('—');
        setSide('—');
        setLobbyId('—');
        setQueueStatus('Lobby was deleted', 'err');
        document.getElementById('btnJoinQueue').disabled = false;
        document.getElementById('btnLeaveQueue').disabled = true;
        setDeployZoneActive(false);
        resetMap();
    });

    conn.on('LobbyUpdated', d => {
        const blue = field(d, 'blueStatus');
        const red = field(d, 'redStatus');
        log(`LobbyUpdated - blue: ${blue} red: ${red}`, 'recv');

        const opponentReady = (state.mySide === 'Blue' && red) || (state.mySide === 'Red' && blue);
        if (opponentReady) {
            setDeployStatus('Opponent ready — waiting to start...');
        }
    });

    conn.on('MatchStarted', async d => {
        const matchId = gid(field(d, 'matchId'));
        if (!matchId) {
            log('MatchStarted: No match ID', 'err');
            return;
        }

        state.boardUnits = field(d, 'units') ?? [];
        state.tiles = field(d, 'tiles') ?? [];
        let myUnits = state.boardUnits.filter(u => normalizeId(field(u, 'ownerId')) === normalizeId(state.myUserId));

        log(`MatchStarted — match: ${matchId}  units: ${state.boardUnits.length}  tiles: ${state.tiles.length}`, 'recv');
        log(`My units: ${myUnits.length}`, 'info');

        // Join Match
        await conn.invoke('JoinMatch', matchId);

        setMatchId(matchId);
        setPhase('Match', 'active');
        resetMap();

        document.getElementById('lobbyCard').classList.toggle('hidden');
        document.getElementById('orderCard').classList.toggle('hidden');

        myUnits.forEach(u => addUnitRow(u));
    });
}


// ── Hub invocations ──────────────────────────────────────────────────────
async function joinQueue() {
    try {
        await state.connection.invoke('JoinQueue');
        document.getElementById('btnJoinQueue').disabled = true;
    } catch (e) {
        log(`JoinQueue: ` + e, 'err');
    }
}

async function leaveQueue() {
    try {
        await state.connection.invoke('LeaveQueue');
    } catch (e) {
        log(`LeaveQueue: ` + e, 'err');
    }
}

async function submitDeployment() {
    const plans = buildDeploymentPlan();
    if (!state.lobbyId) {
        log('Missing lobby ID', 'err');
        return;
    }
    if (!plans || !plans.length) {
        log('Missing deployment plans', 'err');
        setDeployStatus('Missing deployment plans…');
        return;
    }

    log('SubmitDeployment', 'send');
    try {
        await state.connection.invoke('SubmitDeployment', state.lobbyId, plans);
        document.getElementById('btnSubmitDeployment').disabled = true;
    } catch (e) {
        log(`SubmitDeployment: ` + e, 'err');
    }
}

async function cancelDeployment() {
    if (!state.lobbyId) {
        log('Missing lobby ID', 'err');
        return;
    }
    log('CancelDeployment', 'send');
    try {
        await state.connection.invoke('CancelDeployment', state.lobbyId);
        setDeployStatus('—');
        document.getElementById('btnSubmitDeployment').disabled = false;
    } catch (e) {
        log(`CancelDeployment: ` + e, 'err');
    }
}

// ── Window (onclick hooks for non-module HTML) ──────────────────────────────
Object.assign(window, { joinQueue, leaveQueue, submitDeployment, cancelDeployment });
