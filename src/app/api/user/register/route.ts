import { db } from '@/init/databaseInit';
import { auth } from '@/init/firebaseAdminAuthInit';
import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';


export async function POST(request: NextRequest) {
  const token = ((await cookies()).get('token'))?.value
  
  if (token) {
    const decodedToken = await auth.verifyIdToken(token);
    const uid = decodedToken.uid;
    const user = await auth.getUser(uid);
    const email = user.email;
    const username = user.displayName;
        
    const result = await db.execute(`INSERT INTO users(uid, username, email) VALUES('${uid}', '${username}', '${email}');`)

    console.log(result);

    return NextResponse.json({}, {status: 200});
  } else {
    return NextResponse.json({}, {status: 401});
  }

}
