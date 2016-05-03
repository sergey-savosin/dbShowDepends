-- поиск "плохих" объектов
-- (на которые нельзя найти ссылки)

set nocount on;

if OBJECT_ID('tempdb..#weakObj') is not null
  drop table #weakObj;
create table #weakObj (objId int primary key, objName varchar(100) not null);

declare @objId int, @objName sysname;
declare @res table (r int not null);

declare cr cursor local
for
  select o.object_id objId
  , OBJECT_SCHEMA_NAME(o.object_id, DB_ID()) + '.' + o.name objName
  from sys.all_objects o
  where o.type in ('u', 'f', 'fn', 'p');

open cr;

while 1=1
begin
  fetch cr into @objId, @objName;
  if @@FETCH_STATUS<>0
    break;

  begin try
  
  insert @res(r)
  select 1
  from sys.dm_sql_referenced_entities(@objName, 'object') re;
  
  end try
  begin catch
    print @objName + ' (' + cast(@objId as varchar(50)) + ')';
    print error_message();
    

    insert #weakObj (objId, objName)
    values (@objId, @objName);

  end catch
end;

close cr;
deallocate cr;

select
  wo.objId
, wo.objName
, o.type
from #weakObj wo
inner join sys.all_objects o on wo.objId = o.object_id ;

-- поиск входящих объектов
declare @ObjectName sysname = 'dbo.FeedBackCategoryLstForMailer';

IF EXISTS (
    select 1
    from #weakObj w
    where w.objName = @ObjectName
)
BEGIN
  select '<not avaible>' refName;
END
ELSE
BEGIN

	select isnull(re.referenced_schema_name, 'dbo') + '.'
	  + re.referenced_entity_name
	  + ISNULL('.' + re.referenced_minor_name, '') refName
	, re.*
	from sys.dm_sql_referenced_entities(@ObjectName, 'object') re;

END;