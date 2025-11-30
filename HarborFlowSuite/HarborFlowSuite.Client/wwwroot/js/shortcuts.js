window.registerCommandPaletteShortcut = (dotNetHelper) => {
    document.addEventListener('keydown', (e) => {
        if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('OpenCommandPalette');
        }
    });
};

window.registerServiceRequestShortcuts = (dotNetHelper) => {
    const handler = (e) => {
        // Ignore if typing in an input, textarea, or contenteditable
        if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.isContentEditable) {
            return;
        }

        if (e.key.toLowerCase() === 'a') {
            dotNetHelper.invokeMethodAsync('TriggerApprove');
        } else if (e.key.toLowerCase() === 'r') {
            dotNetHelper.invokeMethodAsync('TriggerReject');
        }
    };

    document.addEventListener('keydown', handler);

    // Return a cleanup function (or handle cleanup in DotNet via another method if needed)
    // For simplicity, we attach it to the window object to remove it later
    window.serviceRequestShortcutHandler = handler;
};

window.unregisterServiceRequestShortcuts = () => {
    if (window.serviceRequestShortcutHandler) {
        document.removeEventListener('keydown', window.serviceRequestShortcutHandler);
        window.serviceRequestShortcutHandler = null;
    }
};
