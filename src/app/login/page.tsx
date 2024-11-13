"use client"
import styles from "../page.module.css";
import { loginWithEmailAndPassword } from '@/util/authService';
import { useRouter } from "next/navigation";
import React, { useState } from 'react';

export default function Login() {    
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');  

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      loginWithEmailAndPassword(email, password).then((user) => {
        user.getIdToken().then((token) => {
            document.cookie = `token=${token}; path=/; max-age=604800`;            
            router.push('/');
        })
      })
    } catch (error) {
      alert(error);
    }
    
  };  
  

  return (
    <div className={styles.page}>
      <main className={styles.main}>

      <div className={styles.page}>
    <form onSubmit={handleLogin}>
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />
      <button type="submit">Login</button>
    </form>        
    </div>
      </main>
    </div>
  );
}
