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
    sendPasswordResetEmail: async function (email) {
        console.log("Attempting to send password reset email to:", email);
        try {
            await firebase.auth().sendPasswordResetEmail(email);
            console.log("Password reset email sent successfully.");
            return true;
        } catch (error) {
            console.error("Error sending password reset email:", error);
            throw error;
        }
    },
    changePassword: async function (newPassword) {
        try {
            const user = firebase.auth().currentUser;
            if (user) {
                await user.updatePassword(newPassword);
                return { success: true };
            } else {
                return { success: false, error: "No user logged in." };
            }
        } catch (error) {
            console.error("Error changing password:", error);
            return { success: false, error: error.message };
        }
    },
    reauthenticate: async function (password) {
        try {
            const user = firebase.auth().currentUser;
            if (user) {
                const credential = firebase.auth.EmailAuthProvider.credential(user.email, password);
                await user.reauthenticateWithCredential(credential);
                return { success: true };
            } else {
                return { success: false, error: "No user logged in." };
            }
        } catch (error) {
            console.error("Error re-authenticating:", error);
            return { success: false, error: error.message };
        }
    }
};
