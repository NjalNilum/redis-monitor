window.redisMonitor = {
    copyToClipboard: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.error('Failed to copy text: ', err);
            return false;
        }
    },
    
    scrollToBottom: function (element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },

    initializeHorizontalSplitter: function (container, primaryPanel, handle, storageKey, defaultWidth, minWidth, maxPercent) {
        if (!container || !primaryPanel || !handle) {
            return;
        }

        if (handle._splitterCleanup) {
            handle._splitterCleanup();
        }

        const mobileQuery = window.matchMedia('(max-width: 768px)');
        const minimumWidth = Number(minWidth) || 240;
        const maximumPanelRatio = Number(maxPercent) || 0.7;
        const minimumSecondaryWidth = 320;

        const getSavedWidth = function () {
            if (!storageKey) {
                return null;
            }

            try {
                const rawValue = window.localStorage.getItem(storageKey);
                const parsed = Number.parseFloat(rawValue);
                return Number.isFinite(parsed) ? parsed : null;
            } catch {
                return null;
            }
        };

        const persistWidth = function (width) {
            if (!storageKey) {
                return;
            }

            try {
                window.localStorage.setItem(storageKey, String(width));
            } catch {
                // Ignore storage failures
            }
        };

        const getBounds = function () {
            const containerRect = container.getBoundingClientRect();
            const handleWidth = handle.offsetWidth || 12;
            const totalWidth = Math.max(containerRect.width - handleWidth, minimumWidth);
            const widthByRatio = totalWidth * maximumPanelRatio;
            const widthBySecondaryPanel = totalWidth - minimumSecondaryWidth;
            const maxWidth = Math.max(minimumWidth, Math.min(widthByRatio, widthBySecondaryPanel > minimumWidth ? widthBySecondaryPanel : widthByRatio));

            return {
                left: containerRect.left,
                maxWidth,
                minWidth: minimumWidth
            };
        };

        const applyWidth = function (requestedWidth) {
            if (mobileQuery.matches) {
                container.style.removeProperty('--primary-panel-width');
                return;
            }

            const bounds = getBounds();
            const clampedWidth = Math.min(Math.max(requestedWidth, bounds.minWidth), bounds.maxWidth);
            container.style.setProperty('--primary-panel-width', `${Math.round(clampedWidth)}px`);
            primaryPanel.style.width = `${Math.round(clampedWidth)}px`;
            primaryPanel.style.flexBasis = `${Math.round(clampedWidth)}px`;
            persistWidth(Math.round(clampedWidth));
        };

        const resetWidth = function () {
            applyWidth(defaultWidth);
        };

        const syncInitialWidth = function () {
            const savedWidth = getSavedWidth();
            applyWidth(savedWidth ?? defaultWidth);
        };

        const handlePointerDown = function (event) {
            if (mobileQuery.matches) {
                return;
            }

            event.preventDefault();

            const updatePointerPosition = function (moveEvent) {
                const bounds = getBounds();
                applyWidth(moveEvent.clientX - bounds.left);
            };

            const stopDragging = function () {
                document.body.classList.remove('redis-monitor-resizing');
                handle.classList.remove('dragging');
                window.removeEventListener('pointermove', updatePointerPosition);
                window.removeEventListener('pointerup', stopDragging);
            };

            document.body.classList.add('redis-monitor-resizing');
            handle.classList.add('dragging');
            window.addEventListener('pointermove', updatePointerPosition);
            window.addEventListener('pointerup', stopDragging, { once: true });
        };

        const handleViewportChange = function () {
            syncInitialWidth();
        };

        handle.addEventListener('pointerdown', handlePointerDown);
        handle.addEventListener('dblclick', resetWidth);

        if (typeof mobileQuery.addEventListener === 'function') {
            mobileQuery.addEventListener('change', handleViewportChange);
        } else {
            mobileQuery.addListener(handleViewportChange);
        }

        syncInitialWidth();

        handle._splitterCleanup = function () {
            handle.removeEventListener('pointerdown', handlePointerDown);
            handle.removeEventListener('dblclick', resetWidth);

            if (typeof mobileQuery.removeEventListener === 'function') {
                mobileQuery.removeEventListener('change', handleViewportChange);
            } else {
                mobileQuery.removeListener(handleViewportChange);
            }
        };
    }
};
