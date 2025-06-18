import { db } from '@/init/databaseInit';
import { auth } from '@/init/firebaseAdminAuthInit';
import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';


export async function POST() {
  let user;
  try {
    const token = ((await cookies()).get('token'))?.value
  
    if (token) {
      const decodedToken = await auth.verifyIdToken(token);
      const uid = decodedToken.uid;
      user = await auth.getUser(uid);
      const email = user.email;
      const username = user.displayName;
            
      const result = await db.execute(`INSERT INTO user(uid, username, email) VALUES('${uid}', '${username}', '${email}');`);      

      return NextResponse.json(result, {status: 200});
    } else {
      return NextResponse.json({}, {status: 400});
    }
  } catch (error) {
    console.log(error);
    
    return NextResponse.json({}, {status: 500});
  }  
}
