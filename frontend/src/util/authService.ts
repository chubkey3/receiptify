// src/authService.js
import { createUserWithEmailAndPassword, signInWithEmailAndPassword, signOut, updateProfile, sendEmailVerification, deleteUser, User } from 'firebase/auth';
import { auth } from '@/init/firebaseAuthInit';

export const registerWithEmailAndPassword = async (email: string, password: string, displayName: string) => {
  try {
    const userCredential = await createUserWithEmailAndPassword(auth, email, password);
    
    await updateProfile(userCredential.user, {displayName: displayName})
    await sendEmailVerification(userCredential.user);
    return userCredential.user;
  } catch (error) {
    console.error("Error registering:", error);
    throw error;
  }
};

export const deleteCurrentUser = async (user: User) => {
  try {
    
    await deleteUser(user);
            
  } catch (error) {
    console.log('Error deleting:', error)
    throw error;
  }
}

export const loginWithEmailAndPassword = async (email: string, password: string) => {
  try {
    const userCredential = await signInWithEmailAndPassword(auth, email, password);
    return userCredential.user;
  } catch (error) {
    console.error("Error logging in:", error);
    throw error;
  }
};

export const logout = async () => {
  try {
    await signOut(auth);
  } catch (error) {
    console.error("Error logging out:", error);
    throw error;
  }
};
