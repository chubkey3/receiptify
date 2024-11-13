import mysql from 'mysql2/promise'

const dbConfig = {
    host: '35.226.152.169',
    user: 'root'    
}

const db = await mysql.createConnection(dbConfig)

if (process.env.NODE_ENV === "development") {
    db.execute("CREATE DATABASE IF NOT EXISTS receiptify;");
    db.execute("USE receiptify;");
    db.execute(`CREATE TABLE IF NOT EXISTS users (
  uid VARCHAR(128) PRIMARY KEY,
  username VARCHAR(32) NOT NULL,
  email VARCHAR(128) UNIQUE NOT NULL
);`);
    

    // reset firebase database too
}

export { db };