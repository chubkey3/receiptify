import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";

const firebaseConfig = {
	apiKey: process.env.NEXT_PUBLIC_IDENTITY_PLATFORM_API_KEY,
	authDomain: process.env.NEXT_PUBLIC_IDENTITY_PLATFORM_AUTH_DOMAIN,
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

export { app, auth };
