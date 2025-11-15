console.log("auth.js loaded");
window.firebaseAuth = {
    signIn: async function (email, password) {
        try {
            const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
            const token = await userCredential.user.getIdToken();
            return token;
        } catch (error) {
            console.error("Firebase sign-in error:", error);
            return null;
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
    onTokenChanged: function (dotNetObjectReference) {
        firebase.auth().onIdTokenChanged(async (user) => {
            let token = null;
            if (user) {
                token = await user.getIdToken();
            }
            dotNetObjectReference.invokeMethodAsync('OnTokenChanged', token);
        });
    }
};
