window.authInterop = {
    init: function (dotnetHelper) {
        window.authProviderRef = dotnetHelper;
        firebase.auth().onAuthStateChanged(user => {
            if (user) {
                user.getIdToken().then(token => {
                    console.log("auth-init.js: OnAuthStateChanged invoked with user:", user.email);
                    dotnetHelper.invokeMethodAsync('OnAuthStateChanged', {
                        email: user.email,
                        uid: user.uid,
                        token: token
                    });
                });
            } else {
                console.log("auth-init.js: OnAuthStateChanged invoked with null user");
                dotnetHelper.invokeMethodAsync('OnAuthStateChanged', null);
            }
        });
    }
};
