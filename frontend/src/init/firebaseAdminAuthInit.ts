import { getApps, initializeApp } from "firebase-admin/app";
import admin from 'firebase-admin';
import { Auth, getAuth } from "firebase-admin/auth";

let auth: Auth;

import { getEnvVariable } from "@/util/getEnvironmentalVariable";

if (getApps().length > 0) { 
    const app = getApps()[0];   
    auth = getAuth(app);
} else {           
    const app = initializeApp({credential: admin.credential.cert({
        projectId: getEnvVariable('SERVICE_ACCOUNT_PROJECT_ID'),
        clientEmail: getEnvVariable('SERVICE_ACCOUNT_CLIENT_EMAIL'),
        privateKey: getEnvVariable('SERVICE_ACCOUNT_PRIVATE_KEY')
    })}, "admin");
    
    auth = getAuth(app);
}


export { auth };