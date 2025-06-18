import mysql from 'mysql2/promise'

const dbConfig = {
    host: '35.226.152.169',
    user: 'root',
    database: 'receiptify'
}



const db = mysql.createPool(dbConfig)



export { db };

if (process.env.NODE_ENV === "development") {

}

