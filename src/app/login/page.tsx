"use client"
import { LoginForm } from "@/components/login-page";
import { loginWithEmailAndPassword } from '@/util/authService';
import { useRouter } from "next/navigation";
import React, { useState } from 'react';
import { toast } from "sonner";
import { refreshSessionCookie } from "../auth/AuthHelper";

export default function Login() {    
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');  

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      loginWithEmailAndPassword(email, password).then((user) => {
        user.getIdToken().then((token) => {            
            refreshSessionCookie(token).then(() => router.push('/'));
        })
      }).catch(() => toast("Invalid Login.", {position: "top-center"}))
    } catch (error) {      
      
    }
    
  };  
  

  return (
      <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-sm">
        <LoginForm email={email} password={password} setEmail={setEmail} setPassword={setPassword} submit={handleLogin} />
      </div>
    </div>
  );
}
