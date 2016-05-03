SELECT sm.object_id
, object_schema_name(sm.object_id) + '.' + OBJECT_NAME(sm.object_id) AS object_name
, o.type
, o.type_desc
, sm.definition
FROM sys.sql_modules AS sm
JOIN sys.objects AS o ON sm.object_id = o.object_id
where sm.object_id = object_id(@ObjectName)