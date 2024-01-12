USE DbName

DECLARE @DropConstraintStatements NVARCHAR(MAX) = ''
DECLARE @DropTableStatements NVARCHAR(MAX) = ''
DECLARE @DropSequenceStatements NVARCHAR(MAX) = ''

-- Create drop constraint statements
SELECT @DropConstraintStatements += 'ALTER TABLE ' + QUOTENAME(sch.name) + '.' + QUOTENAME(tbl.name) + ' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';' + CHAR(13)
FROM sys.foreign_keys fk
INNER JOIN sys.tables tbl ON fk.parent_object_id = tbl.object_id
INNER JOIN sys.schemas sch ON tbl.schema_id = sch.schema_id
ORDER BY sch.name, tbl.name

-- Create drop table statements
SELECT @DropTableStatements += 'DROP TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + ';' + CHAR(13) 
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
ORDER BY s.name, t.name

-- Create drop sequence statements
SELECT @DropSequenceStatements += 'DROP SEQUENCE ' + QUOTENAME(sch.name) + '.' + QUOTENAME(seq.name) + ';' + CHAR(13)
FROM sys.sequences seq
INNER JOIN sys.schemas sch ON seq.schema_id = sch.schema_id
ORDER BY sch.name, seq.name

-- Execute the generated statements
EXEC sp_executesql @DropConstraintStatements
EXEC sp_executesql @DropTableStatements
EXEC sp_executesql @DropSequenceStatements