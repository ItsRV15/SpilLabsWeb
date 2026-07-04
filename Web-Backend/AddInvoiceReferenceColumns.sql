-- SQL script to add InvoiceNo and ReferenceNo columns to salesorder table
-- Run this script if the columns don't exist in your database

-- Add InvoiceNo column (required field)
ALTER TABLE salesorder 
ADD COLUMN IF NOT EXISTS InvoiceNo VARCHAR(255) NOT NULL DEFAULT '';

-- Add ReferenceNo column (optional field)
ALTER TABLE salesorder 
ADD COLUMN IF NOT EXISTS ReferenceNo VARCHAR(255) NULL;

-- Note: If your database doesn't support IF NOT EXISTS, use these commands instead:
-- ALTER TABLE salesorder ADD COLUMN InvoiceNo VARCHAR(255) NOT NULL DEFAULT '';
-- ALTER TABLE salesorder ADD COLUMN ReferenceNo VARCHAR(255) NULL;