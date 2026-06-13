export const state = {
    connection: null,
    myUserId: null,
    myUsername: null,
    mySide: null,

    currentPowerLimit: 0,
    deployRowCounter: 0,
    selectedUnit: null,
    activeUnitRow: null,
    stagingComplete: false,

    lobbyId: null,
    matchId: null,
    unitDefinitionCache: {},

    boardUnits: [],
    tiles: [],

    accessToken: null,
    refreshToken: null,

    unitOrders: [],
    unitOrderCounter: 0,
    abilityOrderCounter: 0,
    pendingAbilityOrder: null,
};

export const BASE_URL = 'http://localhost:5128';
