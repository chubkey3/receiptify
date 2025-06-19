// app/AuthListener.tsx
'use client';

import { useEffect } from 'react';
import { auth } from '@/init/firebaseAuthInit';
import axios from '@/util/axios'

export function AuthHelper() {
  useEffect(() => {    
    const unsubscribe = auth.onAuthStateChanged(async (user) => {
      if (user) {
        const idToken = await user.getIdToken();
        await refreshSessionCookie(idToken);
      }
    });

    return () => unsubscribe();
  }, []);

    // 🔁 Set interval to refresh cookie every 50 minutes
   useEffect(() => {
    const interval = setInterval(async () => {
      const user = auth.currentUser;
      if (user) {
        const idToken = await user.getIdToken(true); // force refresh
        await refreshSessionCookie(idToken);
      }
    }, 50 * 60 * 1000); // 50 mins

    return () => clearInterval(interval);
  }, []);


  return null;
}

export async function refreshSessionCookie(idToken: string) {
  try {
    await axios.post('/session/refresh', {IdToken: idToken}) 
  } catch (err) {
    console.error('Failed to refresh session cookie:', err);
  }
}