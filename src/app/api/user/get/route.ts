import { db } from '@/init/databaseInit';
import { auth } from '@/init/firebaseAdminAuthInit';
import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';


export async function GET(request: NextRequest) {
  const token = ((await cookies()).get('token'))?.value
  
  if (token) {
    const decodedToken = await auth.verifyIdToken(token);
    const uid = decodedToken.uid;
        
    const result = await db.execute(`SELECT * FROM users WHERE uid='${uid}';`)

    console.log(result);

    return NextResponse.json({result}, {status: 200});
  } else {
    return NextResponse.json({}, {status: 401});
  }
  
}
