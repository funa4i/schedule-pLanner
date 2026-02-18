IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'mydb'
)
BEGIN
    CREATE DATABASE [mydb];
END

IF NOT EXISTS (
   SELECT name
   FROM sys.databases
   WHERE name = N'testdb'
)
BEGIN
    CREATE DATABASE [testdb]
END           
GO
