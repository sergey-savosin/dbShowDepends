declare @ObjectName sysname;
set @ObjectName = '[dbo].[CustOrdersOrders]'
set @ObjectName = 'dbo.Orders';

SELECT sm.object_id
, object_schema_name(sm.object_id) + '.' + OBJECT_NAME(sm.object_id) AS object_name
, o.type
, o.type_desc
, sm.definition
FROM sys.sql_modules AS sm
JOIN sys.objects AS o ON sm.object_id = o.object_id
where sm.object_id = object_id(@ObjectName)
and o.type not in ('U') /* table */

select object_schema_name(o.object_id) + '.' + OBJECT_NAME(o.object_id) AS object_name
, c.name col_name
, t.system_type_id
, t.name system_type_name
, t.is_table_type
, t.is_user_defined
, c.system_type_id
, c.user_type_id
, c.is_identity
, c.is_nullable
, c.is_computed
, c.is_xml_document
, c.xml_collection_id
, c.max_length
, c.precision
, c.scale
, c.collation_name
from sys.objects o
inner join sys.columns c on c.object_id = o.object_id
left join sys.types t on t.user_type_id = c.user_type_id
--left join sys.types ut on ut.user_type_id = c.user_type_id
where o.object_id = object_id(@ObjectName)
and o.type = 'U' /* table */
order by c.column_id
;