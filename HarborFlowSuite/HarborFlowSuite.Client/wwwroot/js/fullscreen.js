window.fullscreenHelper = {
    toggleFullScreen: function (elementId) {
        var element = document.getElementById(elementId);
        if (!element) return;

        if (!document.fullscreenElement) {
            element.requestFullscreen().catch(err => {
                console.error(`Error attempting to enable full-screen mode: ${err.message} (${err.name})`);
            });
        } else {
            document.exitFullscreen();
        }
    },

    registerFullScreenChangeHandler: function (dotNetHelper) {
        document.addEventListener('fullscreenchange', () => {
            var isFullScreen = !!document.fullscreenElement;
            dotNetHelper.invokeMethodAsync('SetFullScreenState', isFullScreen);
        });
    }
};
