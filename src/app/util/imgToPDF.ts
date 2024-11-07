import axios from "axios";
import { getEnvVariable } from "./util"

const convertURL = getEnvVariable("IMG_TO_PDF_URL");

export async function imgToPDF(filename: string) {
    try {
        const response = await axios.post(convertURL, {filename: filename});
        
        return response.data;
        
    } catch (error) {
        console.log(error);

        return null;
    }
}


