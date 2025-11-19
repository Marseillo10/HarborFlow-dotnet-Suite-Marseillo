console.log("auth.js loaded");
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
                        console.error("getCurrentUserToken: Error getting token:", error);
                        reject(error);
                    });
                } else {
                    resolve(null);
                }
            });
        });
    },
};
