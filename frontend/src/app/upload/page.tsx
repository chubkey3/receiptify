"use client"

import axios from 'axios';
import { useState } from 'react';

export default function FileUpload() {
  const [file, setFile] = useState<File>();
  const [statusMessage, setStatusMessage] = useState('');

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
        // test if image is valid
        const file = event.target.files[0];
        const img = new Image();
        const imgURL = URL.createObjectURL(file);
        img.src = imgURL;

        img.onload = () => {
          setFile(file);          
          setStatusMessage("");
        }
        img.onerror = () => {
          setFile(undefined);          
          setStatusMessage('Invalid Image File!')        ;
        }
    }    
  };

  const handleUpload = async () => {
    if (!file) {
      alert('Please select a valid file to upload.');
      return;
    }

    let response;
    const formData = new FormData();
    
    formData.append('file', file);

    setStatusMessage("Loading...");

    try {            
      response = await axios.post('http://localhost:5134/upload', formData, {withCredentials: true});      
            
    } catch (error) {      
      setStatusMessage(`File Upload Failed!\nStatus Code: ${response?.status}\n Error: ${error}`);
      
    }

    if (response && response.status === 200) {      
      setStatusMessage(`File uploaded successfully with data "${JSON.stringify(response.data)}"!`);
    } else {
      setStatusMessage(`File Upload Failed!\nStatus Code: ${response?.status}`);
    }    
  };

  return (
    <div>
      <input type="file" onChange={handleFileChange} accept="image/*"/>
      <button onClick={handleUpload}>Upload File</button>
      <p>{statusMessage}</p>
    </div>
  );
}
