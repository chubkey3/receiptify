import axios from "axios";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import React from "react";

export default async function DashboardLayout({ children }: {children: React.ReactNode}) {
  const cookieStore = cookies();  
  
  let status = 0

    try {    
   const res = await axios.get(`${process.env.NEXT_PUBLIC_API_URL}user`, {
    headers: {
      Cookie: (await cookieStore).toString()
    },
    validateStatus: () => true
   })

   if (res.status === 200) {
    status = 200;
   }
    

  } catch (err) {
    console.error("Failed to check user auth:", err);
  }

  if (status !== 200) {      
      redirect("/");
  }

  return <>{children}</>;
}
