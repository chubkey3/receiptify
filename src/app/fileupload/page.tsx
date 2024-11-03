"use client"

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
      alert('Please select a file to upload.');
      return;
    }

    let response;
    const formData = new FormData();
    
    formData.append('file', file);

    setStatusMessage("Loading...");

    try {      
      
      response = await fetch('/api/fileupload', {
        method: 'POST',
        body: formData,
      });
    } catch (error) {      
      setStatusMessage(`File Upload Failed!\nStatus Code: ${response?.status}\n Error: ${error}`);
      
    }

    if (response && response.status === 201) {
      const result = await response.json();
      setStatusMessage(`File uploaded successfully with path "${result.path}"!`);
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
