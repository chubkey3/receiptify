"use client"
import styles from "../page.module.css";
import { registerWithEmailAndPassword } from '@/util/authService';
import axios from "axios";
import { useRouter } from "next/navigation";
import React, { useState } from 'react';

export default function Register() {    
  const router = useRouter();
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');  

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      registerWithEmailAndPassword(email, password, userName).then((user) => {
        user.getIdToken().then((token) => {
          document.cookie = `token=${token}; path=/; max-age=604800`;
          axios.post('/api/user/register').then(() => {            
            router.push('/');
          })  
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
    <form onSubmit={handleRegister}>
      <input
          type="text"
          placeholder="Username"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
        />
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
      <button type="submit">Register</button>
    </form>        
    </div>
      </main>
    </div>
  );
}
