window.registerCommandPaletteShortcut = (dotNetHelper) => {
    document.addEventListener('keydown', (e) => {
        if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('OpenCommandPalette');
        }
    });
};
