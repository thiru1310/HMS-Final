-- Database Migration Script for Mandatory Notes Feature
-- Execute this script to add the required fields to your database

-- Add Notes field to tblAppointment table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'tblAppointment' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE tblAppointment
    ADD Notes NVARCHAR(MAX) NULL
END

-- Add NotesRequired field to tblAppointment table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'tblAppointment' AND COLUMN_NAME = 'NotesRequired')
BEGIN
    ALTER TABLE tblAppointment
    ADD NotesRequired BIT DEFAULT 1
END

-- Create NotesConfiguration table for storing configuration settings
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
               WHERE TABLE_NAME = 'NotesConfiguration')
BEGIN
    CREATE TABLE NotesConfiguration (
        ConfigId INT IDENTITY(1,1) PRIMARY KEY,
        DoctorId INT NULL,
        Specialization NVARCHAR(100) NULL,
        GlobalRequired BIT DEFAULT 1,
        MinLength INT DEFAULT 10,
        Configuration NVARCHAR(MAX) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (DoctorId) REFERENCES tblDoctor(DoctorId)
    )
END

-- Insert default configuration
IF NOT EXISTS (SELECT * FROM NotesConfiguration WHERE DoctorId IS NULL)
BEGIN
    INSERT INTO NotesConfiguration (DoctorId, GlobalRequired, MinLength, Configuration)
    VALUES (NULL, 1, 10, '{"globalRequired":true,"minLength":10,"expertises":{}}')
END

-- Update existing appointments to have default values
UPDATE tblAppointment 
SET NotesRequired = 1 
WHERE NotesRequired IS NULL

PRINT 'Database migration completed successfully!'
PRINT 'Added Notes and NotesRequired fields to tblAppointment table'
PRINT 'Created NotesConfiguration table for managing settings'