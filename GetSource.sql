--declare @ObjectName sysname;
--set @ObjectName = '[dbo].[CustOrdersOrders]'
--set @ObjectName = 'dbo.Orders';

declare @otype char(1);
select top(1) @otype = o.type
from sys.objects o
where o.object_id = object_id(@ObjectName);

if @otype not in ('U') /* table */
begin
	SELECT sm.object_id
	, object_schema_name(sm.object_id) + '.' + OBJECT_NAME(sm.object_id) AS object_name
	, o.type
	, o.type_desc
	, sm.definition
	FROM sys.sql_modules AS sm
	JOIN sys.objects AS o ON sm.object_id = o.object_id
	where sm.object_id = object_id(@ObjectName)
	and o.type not in ('U') /* table */
end

if @otype = 'U' /* table */
begin
	with obj as
	(
		select
		object_schema_name(o.object_id) + '.' + OBJECT_NAME(o.object_id) AS object_name
		, o.object_id
		, o.type
		, o.type_desc
		from sys.objects o
		where o.object_id = object_id(@ObjectName)
	)
	, cols as
	(
		select
		c.name col_name
		, t.system_type_id
		, t.name system_type_name
		, t.is_table_type
		, t.is_user_defined
		, c.is_identity
		, c.is_nullable
		, c.is_computed
		, c.is_xml_document
		, c.xml_collection_id
		, c.max_length
		, c.precision
		, c.scale
		, c.collation_name
		, c.column_id
		from obj o
		inner join sys.columns c on c.object_id = o.object_id
		left join sys.types t on t.user_type_id = c.user_type_id
	)
	, sql_str as
	(
		select [definition] = 
		(
			select
				c.col_name + ' '
				+ c.system_type_name + ' '
				+ case c.is_nullable
					when 1 then 'NOT NULL'
					else 'NULL'
				end
				+ case c.is_identity
					when 1 then ' IDENTITY'
					else ''
				end
				+ char(10)
			from cols c
			order by c.column_id
			for xml path('')
		)
	)
	select 
	o.object_id
	, o.object_name
	, o.type
	, o.type_desc
	, definition =
	'Table: ' + o.object_name
	+ char(10) + 'Columns: '
	+ char(10) + s.definition
	from obj o
	cross join sql_str s;

end;
