window.firebaseAuth = {
    signIn: async function (email, password) {
        try {
            await firebase.auth().signInWithEmailAndPassword(email, password);
            return true;
        } catch (error) {
            console.error("Firebase sign-in error:", error);
            return false;
        }
    },
    signOut: function () {
        return firebase.auth().signOut();
    },
    getCurrentUserToken: function () {
        return new Promise((resolve, reject) => {
            const unsubscribe = firebase.auth().onAuthStateChanged(user => {
                unsubscribe();
                if (user) {
                    user.getIdToken().then(token => {
                        resolve(token);
                    }, error => {
                        reject(error);
                    });
                } else {
                    resolve(null);
                }
            });
        });
    },
    onAuthStateChanged: function (dotnetHelper) {
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
