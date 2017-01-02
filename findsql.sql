declare @SearchText varchar(max) = 'data';

with cte as
(
select
	SCHEMA_NAME(o.schema_id) schema_name,
	o.name object_name,
	o.type object_type,
	o.type_desc object_type_desc,
	o.modify_date,
	m.definition [definition],
	left(m.definition, 100) start_definition,
	PATINDEX('%' + @SearchText + '%', m.definition) found_definition_index
from sys.sql_modules m
	inner join sys.objects o on o.object_id = m.object_id
WHERE
	m.definition like '%' + @SearchText + '%'
	--AND o.type = 'P'
)
select cte.schema_name + '.' + cte.object_name [FullName],
	DB_NAME() DatabaseName,
	cte.object_type [type],
	cte.object_type_desc [type_desc],
	cte.modify_date
	--cte.start_definition,
	--SUBSTRING(cte.definition, cte.found_definition_index - 20, cte.found_definition_index + 80) found_substring
from cte
order by 1, 2

RETURN
-- поиск среди CheckConstraint
select
  OBJECT_SCHEMA_NAME(o.object_id) + '.' + o.name object_name,
  c.name column_name,
  cc.name constraint_name,
  cc.definition
from sys.objects o
INNER JOIN sys.check_constraints cc on cc.parent_object_id = o.object_id
LEFT JOIN sys.columns c on c.object_id = o.object_id and cc.parent_column_id = c.column_id
where 1=1
  and cc.definition like @search;

