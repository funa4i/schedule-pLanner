IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'mydb'
)
BEGIN
    CREATE DATABASE [mydb];
END
GO
