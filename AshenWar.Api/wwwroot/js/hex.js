export const HEX = (() => {
    const SIZE = 26;
    const SQRT3 = Math.sqrt(3);

    // ── Math ──────────────────────────────────────────────────────────────
    function hexToPixel(q, r, ox, oy) {
        return {
            x: ox + SIZE * 1.5 * q,
            y: oy + SIZE * (SQRT3 / 2 * q + SQRT3 * r)
        };
    }

    function hexCorners(cx, cy) {
        return Array.from({length: 6}, (_, i) => {
            const angle = Math.PI / 180 * (60 * i);
            return {x: cx + SIZE * Math.cos(angle), y: cy + SIZE * Math.sin(angle)};
        });
    }

    function pixelToHex(px, py, ox, oy) {
        const x = px - ox, y = py - oy;
        const q = (2 / 3 * x) / SIZE;
        const r = (-1 / 3 * x + SQRT3 / 3 * y) / SIZE;
        return hexRound(q, r);
    }

    function hexRound(q, r) {
        const s = -q - r;
        let rq = Math.round(q), rr = Math.round(r), rs = Math.round(s);
        const dq = Math.abs(rq - q), dr = Math.abs(rr - r), ds = Math.abs(rs - s);
        if (dq > dr && dq > ds) rq = -rr - rs;
        else if (dr > ds) rr = -rq - rs;
        return {q: rq, r: rr};
    }

    // ── State ──────────────────────────────────────────────────────────────
    let tiles        = [];
    let mySpawns     = [];
    let opSpawns     = [];
    let chosenSpawns = [];   // [{q, r}] — deploy overlay, managed by caller
    let activePlanRowId = null;

    const COLORS = {
        tile:            '#2a2a38',
        tileBorder:      '#3e3e52',
        spawnMine:       '#0f2d1a',
        spawnOp:         '#2d0f0f',
        spawnMineBorder: '#4ade80',
        spawnOpBorder:   '#f87171',
        spawnMineArrow:  '#4ade80',
        spawnOpArrow:    '#f87171',
        coordText:       '#ffffff28',
    };

    // ── Callbacks (set by caller) ──────────────────────────────────────────
    //   onSpawnPicked(rowId, q, r)      — spawn tile clicked while in pick mode
    //   onPickModeChange(active, rowId) — pick mode entered or exited
    let _onSpawnPicked    = null;
    let _onPickModeChange = null;

    // ── Render ────────────────────────────────────────────────────────────
    function render() {
        const canvas = document.getElementById('hexCanvas');
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        const W = canvas.width, H = canvas.height;
        ctx.clearRect(0, 0, W, H);

        if (!tiles.length) {
            ctx.fillStyle = '#ffffff18';
            ctx.font = '12px IBM Plex Mono, monospace';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('Waiting for match...', W / 2, H / 2);
            return;
        }

        const ox = W / 2, oy = H / 2;

        for (const t of tiles) {
            const {x, y} = hexToPixel(t.q, t.r, ox, oy);
            const isMine = mySpawns.some(s => s.q === t.q && s.r === t.r);
            const isOp   = opSpawns.some(s => s.q === t.q && s.r === t.r);
            const corners = hexCorners(x, y);

            ctx.beginPath();
            ctx.moveTo(corners[0].x, corners[0].y);
            for (let i = 1; i < 6; i++) ctx.lineTo(corners[i].x, corners[i].y);
            ctx.closePath();
            ctx.fillStyle = isMine ? COLORS.spawnMine : isOp ? COLORS.spawnOp : COLORS.tile;
            ctx.fill();

            ctx.strokeStyle = isMine ? COLORS.spawnMineBorder
                            : isOp   ? COLORS.spawnOpBorder
                            :           COLORS.tileBorder;
            ctx.lineWidth = isMine || isOp ? 1.5 : 1;
            ctx.stroke();

            if (isMine) {
                ctx.fillStyle = COLORS.spawnMineArrow;
                ctx.font = 'bold 11px sans-serif';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('▲', x, y);
            } else if (isOp) {
                ctx.fillStyle = COLORS.spawnOpArrow;
                ctx.font = 'bold 11px sans-serif';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('▼', x, y);
            }

            ctx.fillStyle = COLORS.coordText;
            ctx.font = '8px IBM Plex Mono, monospace';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText(`${t.q},${t.r}`, x, isMine || isOp ? y + 12 : y);
        }

        _drawChosenSpawns(ctx, ox, oy);
    }

    function _drawChosenSpawns(ctx, ox, oy) {
        for (const s of chosenSpawns) {
            const {x, y} = hexToPixel(s.q, s.r, ox, oy);
            const radius = SIZE * 0.45;

            ctx.beginPath();
            ctx.arc(x, y, radius, 0, Math.PI * 2);
            ctx.fillStyle = '#1a3d1a';
            ctx.fill();
            ctx.strokeStyle = COLORS.spawnMineBorder;
            ctx.lineWidth = 1.5;
            ctx.stroke();

            ctx.fillStyle = '#4ade80';
            ctx.font = `bold ${Math.floor(SIZE * 0.45)}px IBM Plex Mono, monospace`;
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('U', x, y);
        }
    }

    // ── Canvas interaction ─────────────────────────────────────────────────
    function setupCanvas() {
        const canvas = document.getElementById('hexCanvas');

        canvas.addEventListener('mousemove', e => {
            const {px, py} = getCanvasPos(canvas, e);
            const h = pixelToHex(px, py, canvas.width / 2, canvas.height / 2);
            const onTile  = tiles.some(t => t.q === h.q && t.r === h.r);
            const onSpawn = mySpawns.some(s => s.q === h.q && s.r === h.r);
            canvas.style.cursor = (activePlanRowId && onSpawn) ? 'crosshair'
                                : onTile                       ? 'pointer'
                                :                                'default';
        });

        canvas.addEventListener('click', e => {
            if (!activePlanRowId) return;
            const {px, py} = getCanvasPos(canvas, e);
            const h = pixelToHex(px, py, canvas.width / 2, canvas.height / 2);
            if (!mySpawns.some(s => s.q === h.q && s.r === h.r)) return;
            _onSpawnPicked?.(activePlanRowId, h.q, h.r);
        });
    }

    function getCanvasPos(canvas, e) {
        const rect = canvas.getBoundingClientRect();
        return {
            px: (e.clientX - rect.left) * (canvas.width / rect.width),
            py: (e.clientY - rect.top)  * (canvas.height / rect.height)
        };
    }

    // ── Pick mode ──────────────────────────────────────────────────────────
    function enterPickMode(rowId) {
        if (activePlanRowId && activePlanRowId !== rowId) {
            _onPickModeChange?.(false, activePlanRowId);
        }
        activePlanRowId = rowId;
        render();
        _onPickModeChange?.(true, rowId);
    }

    function exitPickMode() {
        const prev = activePlanRowId;
        activePlanRowId = null;
        render();
        if (prev !== null) _onPickModeChange?.(false, prev);
    }

    // ── Public API ─────────────────────────────────────────────────────────

    /** Replace tile + spawn data and re-render. Does not touch any DOM outside the canvas. */
    function setData(tileList, mySpawnList, opSpawnList) {
        tiles    = tileList    ?? [];
        mySpawns = mySpawnList ?? [];
        opSpawns = opSpawnList ?? [];
        render();
    }

    /** Update the chosen-spawn overlay (deploy phase circles). Re-renders. */
    function setChosenSpawns(list) {
        chosenSpawns = list ?? [];
        render();
    }

    /** Clear all state and re-render. Fires onPickModeChange(false) if pick was active. */
    function reset() {
        tiles        = [];
        mySpawns     = [];
        opSpawns     = [];
        chosenSpawns = [];
        exitPickMode();
        render();
    }

    return {
        setupCanvas,
        setData,
        setChosenSpawns,
        enterPickMode,
        exitPickMode,
        reset,
        render,
        set onSpawnPicked(cb)    { _onSpawnPicked    = cb; },
        set onPickModeChange(cb) { _onPickModeChange = cb; },
    };
})();
