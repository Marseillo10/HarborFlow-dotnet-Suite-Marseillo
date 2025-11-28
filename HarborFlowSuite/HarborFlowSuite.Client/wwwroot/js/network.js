window.networkStatus = {
    registerListener: function (dotNetHelper) {
        window.addEventListener('online', () => {
            dotNetHelper.invokeMethodAsync('OnNetworkStatusChanged', true);
        });
        window.addEventListener('offline', () => {
            dotNetHelper.invokeMethodAsync('OnNetworkStatusChanged', false);
        });
        // Return initial state
        return navigator.onLine;
    }
};
