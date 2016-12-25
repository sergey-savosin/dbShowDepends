/******************************************
 Описание
   процедура получения строки создания таблиц БД
 
 Аргументы
   @ObjectName
 
 Возвращаемое значение
   сейчас в таблице @output

 Примечание
rownum: 0:4 - параметры до запроса

		5 - выражение CREATE TABLE...

		11:1034 (1024) - описание столбцов (сюда можно добавить выражения default)
				Столбцов в таблице может быть <=1024.

		2001:6500 (18*250) - описание индексов-ограничений:
				2001: шапка
				2002-2017:столбцы
				2018:подвал

				Индексов может быть <= 250 штук, столбцов <= 16.

		7000 - закрытие выражения CREATE TABLE

		7001:268250 (1045*250) - описание индексов:
				7001 - шапка
				7002-7017 (16) - столбцы в ключе индекса
				7018 - подвал ключевых столбцов индекса
				7019 - шапка INCLUDE
				7020-8042 (1023) - столбцы в include
				8043 - подвал INCLUDE
				8044 - where-секция (выражение фильтра)
				8045 - подвал индекса

				Индексов может быть <= 250 штук, ключевых столбцов <= 16, include-столбцов <= 1023.

		269001:788156 (2052*253) - описание внешних ключей
				269001 - шапка
				269002-270025 (1024) - столбцы таблицы, содержащей внешний ключ
				270026 - слово REFERENCES
				270027-270050 (1024) - столбцы таблицы, на которую ссылается ключ
				271051 - подвал
				271052 - активация проверки внешнего ключа

				Внешних ключей рекомендуется 253 штуки, столбцов в ключе, как и в ссылаемой таблице, может быть сколько угодно (=1024)

		790001:792048 (2*1024) - проверки на таблицу. Например, 1024 штук
				790001 - создание ограничения
				790002 - включение ограничения

Что делать с атрибутом столбца таблицы - IsPersistent ?
 ******************************************/

set nocount on
--declare @ObjectName sysname = 'dbo.Products'

/* входящие объекты */
declare @objects table (
	schemaName sysname not null,
	tableName sysname not null

	primary key (schemaName, tableName)
)

/* исходящие строки */
declare @output table (
	schemaName sysname not null,
	tableName sysname not null,
	rownum bigint not null,
	rowtype varchar(50) not null,
	indent int default(0), --отступ при форматировании запроса
	sqltext varchar(max) null,
	endType tinyint default(0) --тип строки. 0-обычная, 1-после строки GO, 2-после строки запятая

	primary key (schemaName, tableName, rownum)
)

if object_id('tempdb..#ansi_params') is not null
  drop table #ansi_params;

if object_id('tempdb..#columns') is not null
  drop table #columns;

IF OBJECT_ID('tempdb..#colDefaults') IS NOT NULL
  DROP TABLE #colDefaults;

if object_id('tempdb..#index_name') is not null
  drop table #index_name;

if object_id('tempdb..#index_columns') is not null
  drop table #index_columns;

if object_id('tempdb..#fk_name') is not null
  drop table #fk_name;

if object_id('tempdb..#fk_cols') is not null
  drop table #fk_cols;

if object_id('tempdb..#table_chk') is not null
  drop table #table_chk;

--0. подготовка списка объектов
insert @objects (schemaName, tableName)
select
	schema_name(t.schema_id) sName,
	t.name tName
from
	sys.tables t
where t.object_id = object_id(@ObjectName)
order by
	schema_name(t.schema_id), t.name


--1. определение параметров ANSI_NULLS, QUOTED_IDENTIFIER
SELECT
	SCHEMA_NAME(tbl.schema_id) AS sName,
	tbl.name AS tName,
	tbl.uses_ansi_nulls AS AnsiNullsStatus,
	CASE WHEN 'FG'=dsidx.type THEN dsidx.name ELSE N'' END AS [FileGroup],
	CAST(
	 case 
		when tbl.is_ms_shipped = 1 then 1
		when (
			select 
				major_id 
			from 
				sys.extended_properties 
			where 
				major_id = tbl.object_id and 
				minor_id = 0 and 
				class = 1 and 
				name = N'microsoft_database_tools_support') 
			is not null then 1
		else 0
	 end
	AS bit) AS IsSystemObject,
	CAST(OBJECTPROPERTY(tbl.object_id,N'IsQuotedIdentOn') AS bit) AS QuotedIdentifierStatus
into #ansi_params
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.indexes AS idx ON idx.object_id = tbl.object_id and idx.index_id < 2
	LEFT JOIN sys.data_spaces AS dsidx ON dsidx.data_space_id = idx.data_space_id

insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	sName,
	tName,
	0,
	'ANSI-params',
	case
		when AnsiNullsStatus=1
		then 'SET ANSI_NULLS ON'
		else 'SET ANSI_NULLS OFF'
	end sqltext,
	1
from
	#ansi_params

insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	sName,
	tName,
	1,
	'ANSI-params',
	case
		when QuotedIdentifierStatus=1
		then 'SET QUOTED_IDENTIFIER ON'
		else 'SET QUOTED_IDENTIFIER OFF'
	end sqltext,
	1
from
	#ansi_params

--2. нужен ли SET ANSI_PADDING?

--3a. создание названия таблицы
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	sName,
	tName,
	5,
	'tableHeader',
	'CREATE TABLE [' +sName +'].[' +tName +'](',
	0
from
	#ansi_params


insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	sName,
	tName,
	7000,
	'tableFooter',
	') ON [' +[FileGroup] +']',
	1
from
	#ansi_params


--3b. создание столбцов таблицы
SELECT
	SCHEMA_NAME(tbl.schema_id) AS [sName],
	tbl.name AS [tName],
	Row_Number() OVER (Partition by tbl.schema_id, tbl.name Order by clmns.column_id) AS [ID],
	clmns.name AS [colName],
	clmns.is_ansi_padded AS [AnsiPaddingStatus],
	clmns.is_computed AS [Computed],
	ISNULL(cc.definition,N'') AS [ComputedText],
	usrt.name AS [DataType],
	ISNULL(baset.name, N'') AS [SystemType],
	sclmns.name AS [DataTypeSchema],
	CAST(clmns.is_rowguidcol AS bit) AS [RowGuidCol],
	CAST(CASE WHEN baset.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1 THEN clmns.max_length/2 ELSE clmns.max_length END AS int) AS [Length],
	CAST(clmns.precision AS int) AS [NumericPrecision],
	clmns.is_identity AS [Identity],
	CAST(ISNULL(ic.seed_value,0) AS bigint) AS [IdentitySeed],
	CAST(ISNULL(ic.increment_value,0) AS bigint) AS [IdentityIncrement],
	ISNULL(clmns.collation_name, N'') AS [Collation],
	CAST(clmns.scale AS int) AS [NumericScale],
	clmns.is_nullable AS [Nullable],
	CAST(ISNULL(cc.is_persisted, 0) AS bit) AS [IsPersisted],
	tbl.object_id as objectid
INTO #columns
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id
	LEFT OUTER JOIN sys.computed_columns AS cc ON cc.object_id = clmns.object_id and cc.column_id = clmns.column_id
	LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = clmns.user_type_id
	LEFT OUTER JOIN sys.types AS baset ON baset.user_type_id = clmns.system_type_id and baset.user_type_id = baset.system_type_id
	LEFT OUTER JOIN sys.schemas AS sclmns ON sclmns.schema_id = usrt.schema_id
	LEFT OUTER JOIN sys.identity_columns AS ic ON ic.object_id = clmns.object_id and ic.column_id = clmns.column_id
ORDER by
	sName, tName, ID


-- 3c. Default значения
SELECT
	SCHEMA_NAME(tbl.schema_id) AS [sName],
	tbl.name AS [tName],
	clmns.name AS [colName],
	cstr.name AS [defaultName],
	CAST(cstr.is_system_named AS BIT) AS [IsSystemNamed],
	cstr.definition AS [Text]
INTO #colDefaults
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.all_columns AS clmns
		ON  clmns.object_id = tbl.object_id
	INNER JOIN sys.default_constraints AS cstr
		ON  cstr.object_id = clmns.default_object_id
ORDER BY
	sName, tName, colName

insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	c.sName,
	c.tName,
	10 +c.ID,--rowid,
	'columns',
	1,
	'[' +c.colName +'] [' +c.DataType +'] ' +
	case
		when lower(c.DataType)='char' OR lower(c.DataType)='nchar' OR lower(c.DataType)='binary'
		then '(' +cast(c.[Length] as varchar) +') '

		when lower(c.DataType)='varchar' OR lower(c.DataType)='nvarchar' OR lower(c.DataType)='varbinary'
		then '('+
			case c.[Length]
				when -1 then 'max'
				else cast(c.[Length] as varchar)
			end +
			') '

		when lower(c.DataType)='numeric' OR lower(c.DataType)='decimal'
		then '(' +cast(c.NumericPrecision as varchar) +',' +cast(c.NumericScale as varchar) +') '

		when lower(c.DataType)='float'
		then '(' +cast(c.NumericPrecision as varchar) +') '		

		else ''
	end +
	case
		when isnull(c.Collation, '')=''
		then ''
		else 'COLLATE ' +c.Collation +' '
	end +

--	[DescriptTableId] [tinyint] IDENTITY(1,1) NOT NULL,
	CASE
		WHEN c.[Identity] = 1
		THEN 'IDENTITY(' + LTRIM(c.IdentitySeed) + ',' + LTRIM(c.IdentityIncrement) + ') '
		ELSE ''
	END +
	case
		when c.Nullable=0
		then 'NOT NULL'
		else 'NULL'
	end +

-- [ModifiedDate] [datetime] NULL CONSTRAINT [DF_DescriptTable_ModifiedDate] DEFAULT (getdate()),
-- [SupportedDirections] [varchar] (10) COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ((1))
	case
		when cd.colName is not null AND cd.[IsSystemNamed] = 1
		then ' DEFAULT ' + cd.[Text]
		when cd.colName is not null AND cd.[IsSystemNamed] = 0
		then ' CONSTRAINT ['+ cd.[defaultName] + '] DEFAULT ' + cd.[Text]
		else ''
	END +
	''
	 as sqltext,
	case
		when c.ID < cols.cnt
		then 2
		else 0
	end as rowEnd
from
	#columns c
	outer apply (
		select count(1) cnt
		from sys.all_columns ac (nolock)
		where ac.object_id = c.objectid
	) cols
	LEFT JOIN #colDefaults cd
		ON cd.sName = c.sName
		AND cd.tName = c.tName
		AND cd.colName = c.colName

--4a. список индексов-ограничений (PK, UI)
SELECT
	Row_Number() Over (Partition by tbl.schema_id, tbl.name order by i.index_id) rn,
	Row_Number() Over (Partition by tbl.schema_id, tbl.name, case when is_primary_key=0 and is_unique_constraint=0 then 0 else 1 end order by i.index_id) rn_for_comma,
	i.index_id,
	SCHEMA_NAME(tbl.schema_id) AS sName,
	tbl.name AS tName,
	i.name AS iName,
	CAST(ISNULL(k.is_system_named, 0) AS bit) AS [IsSystemNamed],
	i.is_primary_key + 2*i.is_unique_constraint AS [IndexKeyType],
	i.is_unique AS [IsUnique],
	CAST(CASE i.index_id WHEN 1 THEN 1 ELSE 0 END AS bit) AS [IsClustered],
	CASE WHEN 'PS'=dsi.type THEN dsi.name ELSE N'' END AS [PartitionScheme],
	--CAST(case when i.type=3 then 1 else 0 end AS bit) AS [IsXmlIndex],
	--case UPPER(ISNULL(xi.secondary_type,'')) when 'P' then 1 when 'V' then 2 when 'R' then 3 else 0 end AS [SecondaryXmlIndexType],
	CASE WHEN 'FG'=dsi.type THEN dsi.name ELSE N'' END AS [FileGroup],
	i.ignore_dup_key AS [IgnoreDuplicateKeys],
	i.fill_factor AS [FillFactor],
	CAST(INDEXPROPERTY(i.object_id, i.name, N'IsPadIndex') AS bit) AS [PadIndex],
	~i.allow_row_locks AS [DisallowRowLocks],
	~i.allow_page_locks AS [DisallowPageLocks],
	s.no_recompute AS [NoAutomaticRecomputation],
	ISNULL(i.filter_definition, N'') AS [FilterDefinition]
INTO
	#index_name
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
	LEFT OUTER JOIN sys.key_constraints AS k ON k.parent_object_id = i.object_id AND k.unique_index_id = i.index_id
	LEFT OUTER JOIN sys.data_spaces AS dsi ON dsi.data_space_id = i.data_space_id
	--LEFT OUTER JOIN sys.xml_indexes AS xi ON xi.object_id = i.object_id AND xi.index_id = i.index_id
	LEFT OUTER JOIN sys.stats AS s ON s.stats_id = i.index_id AND s.object_id = i.object_id
ORDER BY
	sName ASC, tName ASC, i.index_id ASC

/* шапка индекса */
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	sName,
	tName,
	2001 +20*(rn-1) as rownum,
	'constr-index head' as rowtype,
	1,
	'CONSTRAINT [' +iName.iName +'] ' +
		case IndexKeyType
			when 1 then 'PRIMARY KEY '
			when 2 then 'UNIQUE '
			else '<error>'
		end +
		case IsClustered
			when 1 then 'CLUSTERED'
			else 'NONCLUSTERED'
		end +
		'(',
	0
from
	#index_name iName
where
	iName.IndexKeyType > 0

/* подвал индекса-ограничения: в последней строке запятую можно оставить. Выполняется на mssql2005 */
union all
select
	sName,
	tName,
	2018 +20*(rn-1) as rownum,
	'constr-index footer' as rowtype,
	1,
	') '+
	case [FillFactor]
		when 0 then ''
		else 'WITH (FILLFACTOR=' +cast([FillFactor] as varchar) +') '
	end +
	'ON [' +[FileGroup] +']',
	case rn_for_comma
		when indexes.cnt then 0
		else 2
	end
from
	#index_name iName
	outer apply (
		select count(1) cnt
		from #index_name i
		where i.sName = iName.sName
			AND i.tName = iName.tName
			AND IndexKeyType >0
	) indexes
where
	iName.IndexKeyType > 0

--4b. столбцы индексов-ограничений
SELECT
	SCHEMA_NAME(tbl.schema_id) AS sName,
	tbl.name AS tName,
	i.name AS iName,
	--(case ic.key_ordinal when 0 then cast(1 as tinyint) else ic.key_ordinal end) AS [ID],
	ic.key_ordinal ID,
	clmns.name AS colName,
	ic.is_included_column AS [IsIncluded],
	Row_Number() OVER (
		Partition by tbl.[schema_id], tbl.name, i.name, ic.key_ordinal, ic.is_included_column
		Order by ic.index_column_id
		) Included_ID,
	ic.is_descending_key AS [Descending],
	CAST(COLUMNPROPERTY(ic.object_id, clmns.name, N'IsComputed') AS bit) AS [IsComputed]
INTO #index_columns
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
	INNER JOIN sys.index_columns AS ic ON (ic.column_id > 0 and (ic.key_ordinal > 0 or ic.partition_ordinal = 0 or ic.is_included_column != 0)) AND (ic.index_id=CAST(i.index_id AS int) AND ic.object_id=i.object_id)
	INNER JOIN sys.columns AS clmns ON clmns.object_id = ic.object_id and clmns.column_id = ic.column_id
ORDER BY
	sName ASC, tName ASC, iName ASC,[ID] ASC


insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	iCols.sName,
	iCols.tName,
	2001 +20 *(iName.rn -1) +iCols.ID, --iCols.ID начинается с 1
	'constr-index column',
	2,
	'[' +iCols.colName +'] ' +
		case iCols.Descending
			when 0 then 'ASC'
			else 'DESC'
		end,
	case
		when iCols.ID = colsCount.cnt
		then 0
		else 2
	end
from
	#index_name iName
	INNER JOIN #index_columns iCols ON iCols.sName = iName.sName
									AND iCols.tName = iName.tName
									AND iCols.iName = iName.iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.sName = iName.sName
			AND ic.tName = iName.tName
			AND ic.iName = iName.iName
	) colsCount
where
	iName.IndexKeyType > 0


--5a. создание индексов

/* шапка индекса */
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	sName,
	tName,
	7001 +1045*(rn-1) as rownum,
	'index head' as rowtype,
	0,
	'CREATE ' +
		case IsUnique
			when 1 then 'UNIQUE '
			else ''
		end +
		case IsClustered
			when 1 then 'CLUSTERED '
			else 'NONCLUSTERED '
		end +
		'INDEX [' +iName.iName +'] ON [' +iName.sName +'].[' +iName.tName +'] (',
	0
from
	#index_name iName
where
	iName.IndexKeyType = 0

/* подвал списка ключевых столбцов индекса */
union all
select
	sName,
	tName,
	7018 +1045*(rn-1) as rownum,
	'index key columns footer section' as rowtype,
	0,
	')',
	0
from
	#index_name iName
where
	iName.IndexKeyType = 0

/* шапка include-секции */
union all
select
	sName,
	tName,
	7019 +1045*(rn-1) as rownum,
	'index include-section' as rowtype,
	0,
	'INCLUDE (',
	0
from
	#index_name iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.sName = iName.sName
			AND ic.tName = iName.tName
			AND ic.iName = iName.iName
			AND ic.isIncluded = 1
	) inclColumns
where
	iName.IndexKeyType = 0
	and inclColumns.cnt > 0

/* ToDo: подвал include-секции */
union all
select
	sName,
	tName,
	8043 +1045*(rn-1) as rownum,
	'index include-footer section' as rowtype,
	0,
	')',
	0
from
	#index_name iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.sName = iName.sName
			AND ic.tName = iName.tName
			AND ic.iName = iName.iName
			AND ic.isIncluded = 1
	) inclColumns
where
	iName.IndexKeyType = 0
	and inclColumns.cnt > 0

/* where-секция (фильтр индекса) */
union all
select
	sName,
	tName,
	8044 +1045*(rn-1) as rownum,
	'index where-section (index filter)' as rowtype,
	0,
	'WHERE ' + iName.FilterDefinition,
	0
from
	#index_name iName
where
	iName.FilterDefinition <> ''

/* подвал индекса */
union all
select
	sName,
	tName,
	8045 +1045*(rn-1) as rownum,
	'index footer' as rowtype,
	0,
	case [FillFactor]
		when 0 then ''
		else 'WITH (FILLFACTOR=' +cast([FillFactor] as varchar) +') '
	end +
	'ON [' +[FileGroup] +']',
	1
from
	#index_name iName
where
	iName.IndexKeyType = 0


-- 5b. столбцы индекса
/* ключевые поля */
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	iCols.sName,
	iCols.tName,
	7001 +1045 *(iName.rn -1) +iCols.ID, --iCols.ID начинается с 1
	'index key-column',
	1,
	'[' +iCols.colName +'] ' +
		case iCols.Descending
			when 0 then 'ASC'
			else 'DESC'
		end,
	case
		when iCols.ID = colsCount.cnt
		then 0
		else 2
	end
from
	#index_name iName
	INNER JOIN #index_columns iCols ON iCols.sName = iName.sName
									AND iCols.tName = iName.tName
									AND iCols.iName = iName.iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.sName = iName.sName
			AND ic.tName = iName.tName
			AND ic.iName = iName.iName
			AND ic.IsIncluded = 0
	) colsCount
where
	iName.IndexKeyType = 0
	and iCols.IsIncluded = 0

union all
/* include-поля индекса */
select
	iCols.sName,
	iCols.tName,
	7020 +1045 *(iName.rn -1) +iCols.Included_ID, --iCols.IncludedID начинается с 1
	'index key-column',
	1,
	'[' +iCols.colName +']',
	case
		when iCols.Included_ID = colsCount.cnt
		then 0
		else 2
	end
from
	#index_name iName
	INNER JOIN #index_columns iCols ON iCols.sName = iName.sName
									AND iCols.tName = iName.tName
									AND iCols.iName = iName.iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.sName = iName.sName
			AND ic.tName = iName.tName
			AND ic.iName = iName.iName
			AND ic.IsIncluded = 1
	) colsCount
where
	iName.IndexKeyType = 0
	and iCols.IsIncluded = 1



--6a. Внешние ключи
/*внешние ключи: название*/
SELECT
	Row_Number() Over (Partition by tbl.schema_id, tbl.name order by cstr.object_id) rn,
	SCHEMA_NAME(tbl.schema_id) AS sName,
	tbl.name AS tName,
	cstr.name AS fkName,
	CAST(cstr.is_system_named AS bit) AS [IsSystemNamed],
	cstr.delete_referential_action AS [DeleteAction],
	cstr.update_referential_action AS [UpdateAction],
	rtbl.name AS [ReferencedTable],
	schema_name(rtbl.schema_id) AS [ReferencedTableSchema],
	~cstr.is_not_trusted AS [IsChecked],
	~cstr.is_disabled AS [IsEnabled],
	cstr.is_not_for_replication AS [NotForReplication]
INTO #fk_name
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.foreign_keys AS cstr ON cstr.parent_object_id=tbl.object_id
	INNER JOIN sys.tables rtbl ON rtbl.object_id = cstr.referenced_object_id
ORDER BY
	sName ASC, tName ASC, fkName ASC

/* шапка внешнего ключа */
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	sName,
	tName,
	269001 +2052 *(fkName.rn -1) as rownum,
	'fkey header',
	0,
	'ALTER TABLE [' +sName +'].[' +tName +'] WITH ' +
		case IsChecked
			when 1 then 'CHECK '
			else 'NOCHECK '
		end +
		char(13) + char(10) +
		'ADD CONSTRAINT [' +fkName +'] FOREIGN KEY ('
	as sqltext,
	0
from
	#fk_name fkName

union all
/* шапка перечисления столбцов таблицы, на которую ссылается внешний ключ */
select
	sName,
	tName,
	270026 +2052 *(fkName.rn -1),
	'fkey middle',
	0,
	') REFERENCES [' +ReferencedTableSchema +'].[' +ReferencedTable +'] (',
	0
from
	#fk_name fkName

union all
/* подвал внешнего ключа */
select
	sName,
	tName,
	271051 +2052 *(fkName.rn -1),
	'fkey footer',
	0,
	')',
	1
from
	#fk_name fkName


--6b. внешний ключ: столбцы
/* внешние ключи: определение */
SELECT
	SCHEMA_NAME(tbl.schema_id) AS sName,
	tbl.name AS tName,
	cstr.name AS fkName,
	fk.constraint_column_id AS ID,
	cfk.name AS colName,
	crk.name AS refColName
INTO #fk_cols
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.foreign_keys AS cstr ON cstr.parent_object_id=tbl.object_id
	INNER JOIN sys.foreign_key_columns AS fk ON fk.constraint_object_id=cstr.object_id
	INNER JOIN sys.columns AS cfk ON fk.parent_column_id = cfk.column_id and fk.parent_object_id = cfk.object_id
	INNER JOIN sys.columns AS crk ON fk.referenced_column_id = crk.column_id and fk.referenced_object_id = crk.object_id
ORDER BY
	sName ASC, tName ASC,fkName ASC, ID ASC

insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
/* столбцы таблицы, содержащей внешний ключ */
select
	fkName.sName,
	fkName.tName,
	269001 +2052 *(fkName.rn -1) +fkCols.ID, --ID начинается с 1
	'fkey columns',
	1 as indent,
	'[' +fkCols.colName +']',
	case fkCols.ID
		when fkColCount.cnt then 0
		else 2
	end
from
	#fk_name fkName
	INNER JOIN #fk_cols fkCols ON fkCols.sName = fkName.sName
								AND fkCols.tName = fkName.tName
								AND fkCols.fkName = fkName.fkName
	outer apply (
		select count(1) cnt
		from #fk_cols fc
		where fc.sName = fkName.sName
			AND fc.tName = fkName.tName
			AND fc.fkName = fkName.fkName
	) fkColCount

union all
/* столбцы таблицы, на которую ссылается внешний ключ */
select
	fkName.sName,
	fkName.tName,
	270026 +2052 *(fkName.rn -1) +fkCols.ID, --ID начинается с 1
	'fkey ref-columns',
	1 as indent,
	'[' +fkCols.refColName +']',
	case fkCols.ID
		when fkColCount.cnt then 0
		else 2
	end
from
	#fk_name fkName
	INNER JOIN #fk_cols fkCols ON fkCols.sName = fkName.sName
								AND fkCols.tName = fkName.tName
								AND fkCols.fkName = fkName.fkName
	outer apply (
		select count(1) cnt
		from #fk_cols fc
		where fc.sName = fkName.sName
			AND fc.tName = fkName.tName
			AND fc.fkName = fkName.fkName
	) fkColCount


--6c. выполнение проверки внешнего ключа
insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	sName,
	tName,
	271052 +2052 *(fkName.rn -1),
	'fkey check',
	0,
	'ALTER TABLE [' +sName +'].[' +tName +'] CHECK CONSTRAINT [' +fkName +']',
	1
from
	#fk_name fkName

--9. создание ограничений (проверок)
SELECT
	Row_Number() OVER (Partition by tbl.schema_id, tbl.name Order by cstr.name) rn,
	SCHEMA_NAME(tbl.schema_id) AS [sName],
	tbl.name AS [tName],
	cstr.name AS [constrName],
	cstr.is_not_for_replication AS [NotForReplication],
	~cstr.is_not_trusted AS [IsChecked],
	~cstr.is_disabled AS [IsEnabled],
	CAST(cstr.is_system_named AS bit) AS [IsSystemNamed],
	cstr.definition AS [Text]
INTO #table_chk
FROM
	sys.tables AS tbl
	INNER JOIN @objects ob ON ob.schemaName = SCHEMA_NAME(tbl.schema_id) AND ob.tableName = tbl.name
	INNER JOIN sys.check_constraints AS cstr ON cstr.parent_object_id=tbl.object_id
ORDER BY
	[sName] ASC,[tName] ASC,[constrName] ASC

insert @output (
	schemaName,
	tableName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	sName,
	tName,
	790001 +2*(rn-1),
	'check constraint',
	0,
	'ALTER TABLE [' +sName +'].[' +tName +'] WITH '+
		case IsChecked
			when 0 then 'NOCHECK '
			else 'CHECK '
		end +
		char(13) + char(10) +
		'ADD CONSTRAINT [' +constrName +'] CHECK ' +
		case NotForReplication
			when 1 then 'NOT FOR REPLICATION '
			else ''
		end +
		[Text],
	1
from
	#table_chk

union all
select
	sName,
	tName,
	790002 +2*(rn-1),
	'enable check constraint',
	0,
	'ALTER TABLE [' +sName +'].[' +tName +'] '+
		case IsEnabled
			when 1 then 'CHECK '
			else 'NOCHECK '
		end +
		'CONSTRAINT [' +constrName +']',
	1
from
	#table_chk
--select * from #ansi_params
--select * from #columns
--SELECT * FROM #colDefaults
--select * from #index_name
--select * from #index_columns
--select * from #fk_name
--select * from #fk_cols
--select * from #table_chk

declare @v varchar(max), @v_fk varchar(max)
select @v = '', @v_fk = ''
select
	@v = @v +
	space(o.indent) +
	o.sqltext +
	case endtype
		when 0 then ''
		when 1 then char(13) +char(10) +'GO' + char(13) +char(10) + '---------------------------------------------------'
		when 2 then ','
	end +
	char(13) +char(10)
from @output o
where o.rowType <> 'ANSI-params' -- исключаем из вывода на экран
order by schemaName, tableName, o.rownum

select [definition] = @v

--select *
--from @output
--order by schemaName, tableName, rownum