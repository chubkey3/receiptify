import { db } from '@/init/databaseInit';
import { auth } from '@/init/firebaseAdminAuthInit';
import { User } from 'firebase/auth';
import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';


export async function POST(request: NextRequest) {
  let user;
  try {
    const token = ((await cookies()).get('token'))?.value
  
    if (token) {
      const decodedToken = await auth.verifyIdToken(token);
      const uid = decodedToken.uid;
      user = await auth.getUser(uid);
      const email = user.email;
      const username = user.displayName;
            
      const result = await db.execute(`INSERT INTO user(uid, username, email, budgetAmount, budgetRange) VALUES('${uid}', '${username}', '${email}', '${0}', '${'month'}');`);

      console.log(result);

      return NextResponse.json({}, {status: 200});
    } else {
      return NextResponse.json({}, {status: 400});
    }
  } catch (error) {
    console.log(error);
    
    return NextResponse.json({}, {status: 500});
  }  
}
