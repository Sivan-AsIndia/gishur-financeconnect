window.initCompanyTour = function () {

    var menu = document.querySelector("#shepherd-example1");
    if (!menu) return;
    if (menu.dataset.tourAttached === "true") return;
    menu.dataset.tourAttached = "true";

    // ── Inject styles ──────────────────────────────────────────────────────────
    var style = document.createElement("style");
    style.textContent = `
        @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap');

        /* ── Overlay fix ──
           Shepherd uses an SVG with a <rect> fill + a cutout mask.
           The OUTER rect must be dark (the dim background).
           The INNER cutout must be transparent so the real page shows through.
           Setting fill-opacity on the rect and keeping mix-blend-mode normal
           is the reliable cross-browser fix.                                   */
        .shepherd-modal-overlay-container {
            backdrop-filter: none !important;
            -webkit-backdrop-filter: none !important;
        }
        /* The big dark rectangle that covers the page */
        .shepherd-modal-overlay-container > svg {
            position: fixed !important;
        }
        .shepherd-modal-overlay-container > svg rect:first-child,
        .shepherd-modal-overlay-container svg > rect {
            fill: rgba(15, 15, 30, 0.55) !important;
            fill-opacity: 1 !important;
        }
        /* Shepherd cuts a hole using a <mask> — the hole rect must be white
           inside the mask (that's SVG spec) but we must NOT override it.
           What we DO override is the main overlay rect fill.
           The shepherd-modal-overlay-container element itself must NOT have
           a background-color that would show through the hole.               */
        .shepherd-modal-overlay-container {
            background: transparent !important;
        }
        /* Highlighted target — keep it fully visible, no tinting */
        .shepherd-modal-overlay-container ~ * .shepherd-target-highlighted,
        .shepherd-target-highlighted {
            position: relative;
            z-index: 9999 !important;
        }

        /* ── Bubble ── */
        .shepherd-element {
            font-family: 'Plus Jakarta Sans', sans-serif !important;
            background: #FFFFFF !important;
            border: 1.5px solid rgba(254,159,67,0.22) !important;
            border-radius: 16px !important;
            box-shadow:
                0 0 0 4px rgba(254,159,67,0.10),
                0 16px 48px rgba(0,0,0,0.13),
                0 4px 18px rgba(254,159,67,0.18) !important;
            max-width: 330px !important;
            overflow: visible !important;
            animation: tourBounceIn 0.42s cubic-bezier(0.34,1.56,0.64,1) both !important;
            z-index: 10000 !important;
        }
        @keyframes tourBounceIn {
            0%   { opacity:0; transform:scale(0.80) translateY(14px); }
            60%  { opacity:1; transform:scale(1.03) translateY(-2px); }
            100% { opacity:1; transform:scale(1)    translateY(0); }
        }

        /* ── Arrow ── */
        .shepherd-arrow::before {
            background: #FFFFFF !important;
            border-color: rgba(254,159,67,0.22) !important;
        }

        /* ── Top accent bar ── */
        .shepherd-content {
            position: relative !important;
            overflow: hidden !important;
        }
      
        /* ── Header ── */
        .shepherd-header {
            background: #F9F9FB !important;
            padding: 18px 18px 10px !important;
            border-radius: 16px 16px 0 0 !important;
            border-bottom: 1px solid rgba(0,0,0,0.055) !important;
        }
        .shepherd-title {
            font-size: 12px !important;
            font-weight: 700 !important;
            letter-spacing: 0.06em !important;
            text-transform: uppercase !important;
            color: var(--primary) !important;
            display: flex !important;
            align-items: center !important;
            gap: 9px !important;
            flex: 1 !important;
        }
        .tour-badge {
            display: inline-flex !important;
            align-items: center !important;
            justify-content: center !important;
            width: 22px; height: 22px;
            border-radius: 50%;
            background: var(--primary) !important;
            color: #fff;
            font-size: 11px; font-weight: 700;
            letter-spacing: 0; text-transform: none;
            flex-shrink: 0;
            box-shadow: 0 2px 8px rgba(254,159,67,0.38);
        }

        /* ── Cancel icon ── */
        .shepherd-cancel-icon {
            color: #6B7280 !important;
            opacity: 0.55 !important;
    transform: none;
    top: 7px;
    position: absolute;
            //transition: color 0.2s, transform 0.25s, opacity 0.2s !important;
        }
        .shepherd-cancel-icon:hover {
            color: var(--primary) !important;
            opacity: 1 !important;
        }

        /* ── Body ── */
        .shepherd-text {
            color: #1C1C2E !important;
            font-size: 13.5px !important;
            line-height: 1.7 !important;
            padding: 14px 18px 4px !important;
        }
        .shepherd-text p { margin: 0 !important; }
        .shepherd-text strong { color:var(--primary) !important; font-weight: 600 !important; }

        /* ── Progress bar block ── */
        .tour-progress-wrap { padding: 10px 18px 6px; }
        .tour-progress-row {
            display: flex; align-items: center;
            justify-content: space-between; margin-bottom: 6px;
        }
        .tour-step-label {
            font-size: 11px; color: #6B7280;
            font-weight: 500;
            font-family: 'Plus Jakarta Sans', sans-serif;
        }
        .tour-step-count {
            font-size: 11px; font-weight: 700;
            color: var(--primary);
            font-family: 'Plus Jakarta Sans', sans-serif;
        }
        .tour-progress-track {
            height: 5px;
            background: color-mix(in srgb, var(--primary) 16%, transparent);
            border-radius: 99px; overflow: hidden;
        }
        .tour-progress-fill {
       height: 100%;
background: linear-gradient(90deg, var(--primary) 0%, color-mix(in srgb, var(--primary) 60%, white) 100%);
border-radius: 99px;
transition: width 0.45s cubic-bezier(0.34, 1.2, 0.64, 1);
box-shadow: 0 0 8px color-mix(in srgb, var(--primary) 35%, transparent);
        }

        /* ── Footer ── */
        .shepherd-footer {
            padding: 12px 18px 16px !important;
            gap: 8px !important;
            border-top: 1px solid rgba(0,0,0,0.055) !important;
            background: #F9F9FB !important;
            border-radius: 0 0 16px 16px !important;
        }
        .shepherd-footer .shepherd-button {
            font-family: 'Plus Jakarta Sans', sans-serif !important;
            font-size: 12.5px !important; font-weight: 600 !important;
            letter-spacing: 0.02em !important;
            border-radius: 9px !important; padding: 7px 16px !important;
            cursor: pointer !important;
            transition: all 0.2s ease !important;
            margin: 0 !important;
        }
        .shepherd-enabled{
            border-radius: 0 !important;
        }
        .shepherd-footer .btn-primary {
            background-color: var(--primary) !important;
            border-color: var(--primary) !important;
            color: #fff !important;
            position: relative !important; overflow: hidden !important;
        }
        .shepherd-footer .btn-primary::before {
            content: ''; position: absolute; inset: 0;
            background: linear-gradient(160deg,rgba(255,255,255,0.20) 0%,transparent 55%);
            pointer-events: none; border-radius: 9px;
        }
    
        .shepherd-footer .btn-primary:active {
            filter: brightness(0.96) !important;
            transform: translateY(0) scale(0.97) !important;
        }
        .shepherd-footer .btn-outline-secondary {
            background: transparent !important; color: #6B7280 !important;
            border: 1.5px solid rgba(0,0,0,0.14) !important; box-shadow: none !important;
        }
        .shepherd-footer .btn-outline-secondary:hover {
            background: rgba(0,0,0,0.04) !important; color: #374151 !important;
            border-color: rgba(0,0,0,0.22) !important; transform: translateY(-1px) !important;
        }
        .shepherd-footer .btn-outline-secondary:active { transform: translateY(0) scale(0.97) !important; }

        /* ── Target highlight ring ── */
        .shepherd-target-highlighted {
            outline: 2px solid #FE9F43 !important;
            outline-offset: 5px !important;
            border-radius: 8px !important;
            animation: ringPulse 2.2s ease-in-out infinite !important;
        }
        @keyframes ringPulse {
            0%,100% { box-shadow: 0 0 0 5px rgba(254,159,67,0.10), 0 0 22px rgba(254,159,67,0.24); }
            50%     { box-shadow: 0 0 0 9px rgba(254,159,67,0.07), 0 0 42px rgba(254,159,67,0.32); }
        }
    `;
    document.head.appendChild(style);

    /* ─── Overlay cutout fix (JS patch) ────────────────────────────────────────
       Shepherd renders: <svg><rect fill="…"/> <rect mask="url(#…)"/></svg>
       The mask cuts a transparent hole over the target.
       If the page has a white/light body background, the hole looks white.
       We fix by observing the SVG and ensuring the overlay rect is always dark. */
    function fixOverlay() {
        var container = document.querySelector('.shepherd-modal-overlay-container');
        if (!container) return;
        var svg = container.querySelector('svg');
        if (!svg) return;
        /* Find the main fill rect (first rect child of svg, not inside defs/mask) */
        var rects = svg.querySelectorAll(':scope > rect');
        rects.forEach(function (rect) {
            /* Shepherd sets inline fill on the overlay rect — override it */
            rect.style.setProperty('fill', 'rgba(15,15,30,0.55)', 'important');
            rect.style.setProperty('fill-opacity', '1', 'important');
        });
    }

    /* Watch for Shepherd adding the overlay */
    var overlayObserver = new MutationObserver(function () {
        fixOverlay();
    });
    overlayObserver.observe(document.body, { childList: true, subtree: true });

    // ── Helpers ────────────────────────────────────────────────────────────────
    var btnOutline = "btn btn-sm btn-outline-secondary";
    var btnPrim = "btn btn-sm btn-primary";
    var TOTAL = 3;
    var STEP_LABELS = ["Page Header", "Create Company", "Tree View"];

    function waitForElement(selector, callback) {
        var el = document.querySelector(selector);
        if (el) callback();
        else setTimeout(function () { waitForElement(selector, callback); }, 100);
    }

    function progressBlock(current) {
        var pct = Math.round((current / TOTAL) * 100);
        return [
            '<div class="tour-progress-wrap">',
            '<div class="tour-progress-row">',
            '<span class="tour-step-label">' + STEP_LABELS[current - 1] + '</span>',
            '<span class="tour-step-count">' + current + ' / ' + TOTAL + '</span>',
            '</div>',
            '<div class="tour-progress-track">',
            '<div class="tour-progress-fill" style="width:' + pct + '%"></div>',
            '</div>',
            '</div>'
        ].join('');
    }

    function stepTitle(n, label) {
        return '<span class="tour-badge">' + n + '</span>' + label;
    }

    function markDone() {
        try { localStorage.setItem("companyTourShown", "true"); } catch (e) { }
    }

    // ── Main click handler ─────────────────────────────────────────────────────
    menu.addEventListener("click", function () {
        try { if (localStorage.getItem("companyTourShown") === "true") return; } catch (e) { }

        waitForElement("#CompanyPageTitle", function () {
            window.scrollTo({ top: 0, behavior: "instant" });

            var tour = new Shepherd.Tour({
                defaultStepOptions: {
                    scrollTo: { behavior: "smooth", block: "center" },
                    cancelIcon: { enabled: true },
                    popperOptions: {
                        modifiers: [
                            { name: "preventOverflow", options: { boundary: "viewport" } },
                            { name: "offset", options: { offset: [0, 14] } }
                        ]
                    }
                },
                useModalOverlay: true
            });

            tour.on("cancel", function () { markDone(); });

            /* Re-apply overlay fix on every step show (Shepherd re-renders the SVG) */
            tour.on("show", function () {
                setTimeout(fixOverlay, 50);
            });

            tour.on("complete", function () {
                overlayObserver.disconnect();
            });
            tour.on("cancel", function () {
                overlayObserver.disconnect();
            });

            // ── STEP 1 ────────────────────────────────────────────────────────
            tour.addStep({
                title: stepTitle(1, "Page Header"),
                text: progressBlock(1) +
                    "<p>This is your <strong>Page Title</strong> — it always shows the current section you are working in.</p>",
                attachTo: { element: "#CompanyPageTitle", on: "bottom" },
                buttons: [
                    { text: "Skip", classes: btnOutline, action: function () { markDone(); tour.cancel(); } },
                    { text: "Next →", classes: btnPrim, action: tour.next }
                ]
            });

            // ── STEP 2 ────────────────────────────────────────────────────────
            tour.addStep({
                title: stepTitle(2, "Create Company"),
                text: progressBlock(2) +
                    "<p>Click <strong>Create</strong> to add a new company record to your workspace instantly.</p>",
                attachTo: { element: "#CreateButton", on: "top" },
                buttons: [
                    { text: "Skip", classes: btnOutline, action: function () { markDone(); tour.cancel(); } },
                    { text: "← Back", classes: btnOutline, action: tour.back },
                    { text: "Next →", classes: btnPrim, action: tour.next }
                ]
            });

            // ── STEP 3 ────────────────────────────────────────────────────────
            tour.addStep({
                title: stepTitle(3, "Expand Tree"),
                text: progressBlock(3) +
                    "<p>Use the <strong>± icon</strong> to expand or collapse the company tree for a cleaner view.</p>",
                attachTo: { element: "#MinusPlus", on: "top" },
                buttons: [
                    { text: "← Back", classes: btnOutline, action: tour.back },
                    { text: "Finish", classes: btnPrim, action: function () { markDone(); tour.complete(); } }
                ]
            });

            tour.start();
        });
    });
};
