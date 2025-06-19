"use client"
import styles from "../page.module.css";
import { deleteCurrentUser, registerWithEmailAndPassword } from '@/util/authService';
import axios from "axios";
import { useRouter } from "next/navigation";
import React, { useState } from 'react';
import { refreshSessionCookie } from "../auth/AuthHelper";

export default function Register() {    
  const router = useRouter();
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');  

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();    
    registerWithEmailAndPassword(email, password, userName)
    .then((user) => {
      user.getIdToken().then((token) => {
        
        refreshSessionCookie(token).then(() => {
          axios.post('/user', {Email: email, Username: userName})
        .then(() => {            
          router.push('/');            
        })
        }).catch(() => {            
          document.cookie = `token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/`;
          deleteCurrentUser(user);          
        })
      })        
    })
    .catch((error) => {
      alert(error);
    })    
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
