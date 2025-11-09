window.authInterop = {
    init: function (dotnetHelper) {
        window.authProviderRef = dotnetHelper;
        firebase.auth().onAuthStateChanged(user => {
            if (user) {
                user.getIdToken().then(token => {
                    dotnetHelper.invokeMethodAsync('OnAuthStateChanged', {
                        email: user.email,
                        uid: user.uid,
                        token: token
                    });
                });
            } else {
                dotnetHelper.invokeMethodAsync('OnAuthStateChanged', null);
            }
        });
    }
};
