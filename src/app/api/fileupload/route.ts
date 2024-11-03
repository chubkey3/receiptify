import { NextRequest, NextResponse } from 'next/server';
import { Storage } from '@google-cloud/storage';
import { randomUUID } from 'crypto';
import { getEnvVariable } from '@/app/util/getEnvironmentalVariable';


// The ID of your GCS bucket
const bucketName = getEnvVariable("PROJECT_NAME");

// Creates a client
const storage = new Storage();


async function uploadFile(file: File) {
    
    // filename with random UUID name
    const destFileName = `images/${randomUUID()}.${file.name.split('.').pop()}`;
    const contents = await file.arrayBuffer();
    
    await storage.bucket(bucketName).file(destFileName).save(Buffer.from(contents));

    return destFileName;    
}


export async function POST(request: NextRequest) {  
    let formData: FormData;
    let file: File;

    try {        
        formData = await request.formData();
        file = formData.get('file') as File;

    }  catch (error) {
        return NextResponse.json({ error: error}, { status: 400})
    }
    
    try {
        const result = await uploadFile(file);

        return NextResponse.json({ path: result}, {status: 201});

    } catch (error) {
        return NextResponse.json({ error: error}, { status: 500})
    }        
}