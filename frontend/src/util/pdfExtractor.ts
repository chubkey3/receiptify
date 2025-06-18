import axios from "axios";
import { getEnvVariable } from "./getEnvironmentalVariable"

const extractURL = getEnvVariable("PDF_EXTRACTOR_URL");

export async function pdfExtractor(filePath: string) {
    try {
        const response = await axios.post(extractURL, {filename: filePath});                

        return response.data;
        
    } catch (error) {
        console.log(error);
        
        return null;
    }
}

