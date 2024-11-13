import { NextRequest, NextResponse } from 'next/server';
import { Storage } from '@google-cloud/storage';
import { randomUUID } from 'crypto';
import { getEnvVariable } from '@/util/getEnvironmentalVariable';
import { imgToPDF } from '@/util/imgToPDF';
import { pdfExtractor } from '@/util/pdfExtractor';


const bucketName = getEnvVariable("PROJECT_NAME");
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

        const filePath = await imgToPDF(result.split('/')[1]);
        const receiptData = await pdfExtractor(filePath);                        

        return NextResponse.json({ data: receiptData}, {status: 200});

    } catch (error) {
        return NextResponse.json({ error: error}, { status: 500})
    }        
}