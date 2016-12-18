/******************************************
 Описание
   процедура получения строки создания таблиц БД
 
 Аргументы
   @ObjectName
 
 Возвращаемое значение
   сейчас в таблице @output

 Примечание
rownum: 0:4 - параметры до запроса

		5 - выражение CREATE TYPE...

		11:1034 (1024) - описание столбцов (сюда можно добавить выражения default)
				Столбцов в таблице может быть <=1024.

		2001:6500 (18*250) - описание индексов-ограничений:
				2001: шапка
				2002-2017:столбцы
				2018:подвал

				Индексов может быть <= 250 штук, столбцов <= 16.

		7000 - закрытие выражения CREATE TABLE

		7001:267500 (1042*250) - описание индексов:
				7001 - шапка
				7002-7017 (16) - столбцы в ключе индекса
				7018 - шапка INCLUDE
				7019-8041 (1023) - столбцы в include
				8042 - подвал индекса

				Индексов может быть <= 250 штук, ключевых столбцов <= 16, include-столбцов <= 1023.

Что делать с атрибутом столбца таблицы - IsPersistent ?
 ******************************************/

set nocount on
declare @ObjectName sysname = 'UserTableTypeOne'
declare @ObjectSchemaName sysname = 'dbo'

/* исходящие строки */
declare @output table (
	schemaName sysname not null,
	objectName sysname not null,
	rownum bigint not null,
	rowtype varchar(50) not null,
	indent int default(0), --отступ при форматировании запроса
	sqltext varchar(max) null,
	endType tinyint default(0) --тип строки. 0-обычная, 1-после строки GO, 2-после строки запятая

	primary key (schemaName, objectName, rownum)
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


--1. определение параметров объекта
SELECT
	ISNULL(s1tt.name, N'') AS [Owner],
	CAST(case when tt.principal_id is null then 1 else 0 end AS bit) AS [IsSchemaOwned],
	tt.name AS [Name],
	tt.type_table_object_id AS [ID],
	SCHEMA_NAME(tt.schema_id) AS [Schema],
	obj.create_date AS [CreateDate],
	obj.modify_date AS [DateLastModified],
	tt.max_length AS [MaxLength],
	tt.is_nullable AS [Nullable],
	ISNULL(tt.collation_name, N'') AS [Collation],
	CAST(case when tt.is_user_defined = 1 then 1 else 0 end AS bit) AS [IsUserDefined],
	CAST(tt.is_memory_optimized AS bit) AS [IsMemoryOptimized]
into #ansi_params
FROM
	sys.table_types AS tt
	LEFT JOIN sys.database_principals AS s1tt ON s1tt.principal_id = ISNULL(tt.principal_id, (TYPEPROPERTY(QUOTENAME(SCHEMA_NAME(tt.schema_id)) + '.' + QUOTENAME(tt.name), 'OwnerId')))
	INNER JOIN sys.schemas AS stt ON stt.schema_id = tt.schema_id
	LEFT JOIN sys.objects AS obj ON obj.object_id = tt.type_table_object_id
WHERE
	tt.name=@ObjectName
	and SCHEMA_NAME(tt.schema_id)=@ObjectSchemaName
;

insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	[Schema],
	[Name],
	0,
	'ANSI-params',
	'SET ANSI_NULLS ON' sqltext,
	1
from
	#ansi_params

insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	[Schema],
	[Name],
	1,
	'ANSI-params',
	'SET QUOTED_IDENTIFIER ON' sqltext,
	1
from
	#ansi_params

--2. нужен ли SET ANSI_PADDING?

--3a. создание названия таблицы
insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	[Schema],
	[Name],
	5,
	'tableHeader',
	'CREATE TABLE [' +[Schema] +'].[' +[Name] +'](',
	0
from
	#ansi_params


insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	sqltext,
	endType
)
select
	[Schema],
	[Name],
	7000,
	'tableFooter',
	')',
	1
from
	#ansi_params


--3b. создание столбцов таблицы
SELECT
	@ObjectName [objectName],
	@ObjectSchemaName [schemaName],
	clmns.name AS [colName],
	clmns.column_id AS [ID],
	clmns.object_id as objectId,
	clmns.is_nullable AS [Nullable],
	clmns.is_computed AS [Computed],
	CAST(ISNULL(cik.index_column_id, 0) AS bit) AS [InPrimaryKey],
	clmns.is_ansi_padded AS [AnsiPaddingStatus],
	CAST(clmns.is_rowguidcol AS bit) AS [RowGuidCol],
	CAST(ISNULL(cc.is_persisted, 0) AS bit) AS [IsPersisted],
	ISNULL(clmns.collation_name, N'') AS [Collation],
	CAST(ISNULL((select TOP 1 1 from sys.foreign_key_columns AS colfk where colfk.parent_column_id = clmns.column_id and colfk.parent_object_id = clmns.object_id), 0) AS bit) AS [IsForeignKey],
	clmns.is_identity AS [Identity],
	CAST(ISNULL(ic.seed_value,0) AS bigint) AS [IdentitySeed],
	CAST(ISNULL(ic.increment_value,0) AS bigint) AS [IdentityIncrement],
	(case when clmns.default_object_id = 0 then N'' when d.parent_object_id > 0 then N'' else d.name end) AS [Default],
	(case when clmns.default_object_id = 0 then N'' when d.parent_object_id > 0 then N'' else schema_name(d.schema_id) end) AS [DefaultSchema],
	(case when clmns.rule_object_id = 0 then N'' else r.name end) AS [Rule],
	(case when clmns.rule_object_id = 0 then N'' else schema_name(r.schema_id) end) AS [RuleSchema],
	CAST(ISNULL(COLUMNPROPERTY(clmns.object_id, clmns.name, N'IsDeterministic'),0) AS bit) AS [IsDeterministic],
	CAST(ISNULL(COLUMNPROPERTY(clmns.object_id, clmns.name, N'IsPrecise'),0) AS bit) AS [IsPrecise],
	ISNULL(ic.is_not_for_replication, 0) AS [NotForReplication],
	CAST(COLUMNPROPERTY(clmns.object_id, clmns.name, N'IsFulltextIndexed') AS bit) AS [IsFullTextIndexed],
	CAST(COLUMNPROPERTY(clmns.object_id, clmns.name, N'StatisticalSemantics') AS int) AS [StatisticalSemantics],
	CAST(clmns.is_filestream AS bit) AS [IsFileStream],
	CAST(clmns.is_sparse AS bit) AS [IsSparse],
	CAST(clmns.is_column_set AS bit) AS [IsColumnSet],
	usrt.name AS [DataType],
	s1clmns.name AS [DataTypeSchema],
	ISNULL(baset.name, N'') AS [SystemType],
	CAST(CASE WHEN baset.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1 THEN clmns.max_length/2 ELSE clmns.max_length END AS int) AS [Length],
	CAST(clmns.precision AS int) AS [NumericPrecision],
	CAST(clmns.scale AS int) AS [NumericScale],
	ISNULL(xscclmns.name, N'') AS [XmlSchemaNamespace],
	ISNULL(s2clmns.name, N'') AS [XmlSchemaNamespaceSchema],
	ISNULL( (case clmns.is_xml_document when 1 then 2 else 1 end), 0) AS [XmlDocumentConstraint],
	CASE WHEN usrt.is_table_type = 1 THEN N'structured' ELSE N'' END AS [UserType]
INTO #columns
FROM
	sys.table_types AS tt
	INNER JOIN sys.schemas AS stt ON stt.schema_id = tt.schema_id
	INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tt.type_table_object_id
	LEFT JOIN sys.indexes AS ik ON ik.object_id = clmns.object_id and 1=ik.is_primary_key
	LEFT JOIN sys.index_columns AS cik ON cik.index_id = ik.index_id and cik.column_id = clmns.column_id and cik.object_id = clmns.object_id and 0 = cik.is_included_column
	LEFT JOIN sys.computed_columns AS cc ON cc.object_id = clmns.object_id and cc.column_id = clmns.column_id
	LEFT JOIN sys.identity_columns AS ic ON ic.object_id = clmns.object_id and ic.column_id = clmns.column_id
	LEFT JOIN sys.objects AS d ON d.object_id = clmns.default_object_id
	LEFT JOIN sys.objects AS r ON r.object_id = clmns.rule_object_id
	LEFT JOIN sys.types AS usrt ON usrt.user_type_id = clmns.user_type_id
	LEFT JOIN sys.schemas AS s1clmns ON s1clmns.schema_id = usrt.schema_id
	LEFT JOIN sys.types AS baset ON (baset.user_type_id = clmns.system_type_id and baset.user_type_id = baset.system_type_id) or ((baset.system_type_id = clmns.system_type_id) and (baset.user_type_id = clmns.user_type_id) and (baset.is_user_defined = 0) and (baset.is_assembly_type = 1)) 
	LEFT JOIN sys.xml_schema_collections AS xscclmns ON xscclmns.xml_collection_id = clmns.xml_collection_id
	LEFT JOIN sys.schemas AS s2clmns ON s2clmns.schema_id = xscclmns.schema_id
WHERE
	tt.name=@ObjectName
and SCHEMA_NAME(tt.schema_id)=@ObjectSchemaName



-- 3c. Default значения
SELECT
	@ObjectSchemaName as [schemaName],
	@ObjectName as [objectName],
	clmns.name as [colName],
	cstr.name AS [defaultName],
	CAST(cstr.is_system_named AS BIT) AS [IsSystemNamed],
	cstr.definition AS [Text]
INTO #colDefaults
FROM
	sys.table_types AS tt
	INNER JOIN sys.schemas AS stt ON stt.schema_id = tt.schema_id
	INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tt.type_table_object_id
	INNER JOIN sys.default_constraints AS cstr ON cstr.object_id=clmns.default_object_id
WHERE
	tt.name=@ObjectName
	and SCHEMA_NAME(tt.schema_id)=@ObjectSchemaName
ORDER BY
	[defaultName] ASC

insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	c.schemaName,
	c.objectName,
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
		where ac.object_id = c.objectId
	) cols
	LEFT JOIN #colDefaults cd
		ON cd.schemaName = c.schemaName
		AND cd.objectName = c.objectName
		AND cd.colName = c.colName

--4a. список индексов-ограничений (PK, UI)
SELECT
	@ObjectName objectName,
	@ObjectSchemaName schemaName,
	i.name AS [indexName],
	Row_Number() Over (Partition by tt.schema_id, tt.name order by i.index_id) rn,
	Row_Number() Over (Partition by tt.schema_id, tt.name, case when i.is_primary_key=0 and i.is_unique_constraint=0 then 0 else 1 end order by i.index_id) rn_for_comma,
	CAST(i.index_id AS int) AS [ID],
	CAST(OBJECTPROPERTY(i.object_id,N'IsMSShipped') AS bit) AS [IsSystemObject],
	ISNULL(s.no_recompute,0) AS [NoAutomaticRecomputation],
	i.fill_factor AS [FillFactor],
	CAST(CASE i.index_id WHEN 1 THEN 1 ELSE 0 END AS bit) AS [IsClustered],
	i.is_primary_key + 2*i.is_unique_constraint AS [IndexKeyType],
	i.is_unique AS [IsUnique],
	i.ignore_dup_key AS [IgnoreDuplicateKeys],
	~i.allow_row_locks AS [DisallowRowLocks],
	~i.allow_page_locks AS [DisallowPageLocks],
	CAST(ISNULL(INDEXPROPERTY(i.object_id, i.name, N'IsPadIndex'), 0) AS bit) AS [PadIndex],
	i.is_disabled AS [IsDisabled],
	CAST(ISNULL(k.is_system_named, 0) AS bit) AS [IsSystemNamed],
	CAST(INDEXPROPERTY(i.object_id,i.name,N'IsFulltextKey') AS bit) AS [IsFullTextKey],
	CAST(case when i.type=3 then 1 else 0 end AS bit) AS [IsXmlIndex],
	CAST(case when i.type=4 then 1 else 0 end AS bit) AS [IsSpatialIndex],
	i.has_filter AS [HasFilter],
	ISNULL(i.filter_definition, N'') AS [FilterDefinition],
	CASE WHEN 'PS'=dsi.type THEN dsi.name ELSE N'' END AS [PartitionScheme],
	CAST(CASE WHEN 'PS'=dsi.type THEN 1 ELSE 0 END AS bit) AS [IsPartitioned],
	CASE WHEN 'FD'=dstbl.type THEN dstbl.name ELSE N'' END AS [FileStreamFileGroup],
	CASE WHEN 'PS'=dstbl.type THEN dstbl.name ELSE N'' END AS [FileStreamPartitionScheme],
	CAST(CASE WHEN filetableobj.object_id IS NULL THEN 0 ELSE 1 END AS bit) AS [IsFileTableDefined],
	CAST(case when (i.type=7) then hi.bucket_count else 0 end AS int) AS [BucketCount]
INTO
	#index_name
FROM
	sys.table_types AS tt
INNER JOIN sys.schemas AS stt ON stt.schema_id = tt.schema_id
INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tt.type_table_object_id)
LEFT JOIN sys.stats AS s ON s.stats_id = i.index_id AND s.object_id = i.object_id
LEFT JOIN sys.key_constraints AS k ON k.parent_object_id = i.object_id AND k.unique_index_id = i.index_id
LEFT JOIN sys.data_spaces AS dsi ON dsi.data_space_id = i.data_space_id
LEFT JOIN sys.tables AS t ON t.object_id = i.object_id
LEFT JOIN sys.data_spaces AS dstbl ON dstbl.data_space_id = t.Filestream_data_space_id and (i.index_id < 2 or (i.type = 7 and i.index_id < 3))
LEFT JOIN sys.filetable_system_defined_objects AS filetableobj ON i.object_id = filetableobj.object_id
LEFT JOIN sys.hash_indexes AS hi ON i.object_id = hi.object_id AND i.index_id = hi.index_id
WHERE
	tt.name = @ObjectName
	and SCHEMA_NAME(tt.schema_id) = @ObjectSchemaName


/* шапка индекса */
insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	schemaName,
	objectName,
	2001 +20*(rn-1) as rownum,
	'constr-index head' as rowtype,
	1,
	'CONSTRAINT [' +iName.indexName +'] ' +
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
	IndexKeyType >0

/* подвал индекса-ограничения: в последней строке запятую можно оставить. Выполняется на mssql2005 */
union all
select
	schemaName,
	objectName,
	2018 +20*(rn-1) as rownum,
	'constr-index footer' as rowtype,
	1,
	') '+
	case [FillFactor]
		when 0 then ''
		else 'WITH (FILLFACTOR=' +cast([FillFactor] as varchar) +') '
	end,
	case rn_for_comma
		when indexes.cnt then 0
		else 2
	end
from
	#index_name iName
	outer apply (
		select count(1) cnt
		from #index_name i
		where i.schemaName = iName.schemaName
			AND i.objectName = iName.objectName
			AND IndexKeyType >0
	) indexes
where
	IndexKeyType>0




--4b. столбцы индексов-ограничений
SELECT
	@ObjectName objectName,
	@ObjectSchemaName schemaName,
	i.name indexName,
	clmns.name AS [colName],
	ic.key_ordinal ID,
	--(case ic.key_ordinal when 0 then ic.index_column_id else ic.key_ordinal end) AS [ID],
	ic.is_descending_key AS [Descending],
	ic.is_included_column AS [IsIncluded],
	Row_Number() OVER (
		Partition by tt.[schema_id], tt.name, i.name, ic.key_ordinal, ic.is_included_column
		Order by ic.index_column_id
		) Included_ID,
	CAST(COLUMNPROPERTY(ic.object_id, clmns.name, N'IsComputed') AS bit) AS [IsComputed]
INTO #index_columns
FROM
	sys.table_types AS tt
	INNER JOIN sys.schemas AS stt ON stt.schema_id = tt.schema_id
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tt.type_table_object_id)
	INNER JOIN sys.index_columns AS ic ON (ic.column_id > 0 and (ic.key_ordinal > 0 or ic.partition_ordinal = 0 or ic.is_included_column != 0)) AND (ic.index_id=CAST(i.index_id AS int) AND ic.object_id=i.object_id)
	INNER JOIN sys.columns AS clmns ON clmns.object_id = ic.object_id and clmns.column_id = ic.column_id
WHERE
	tt.name=@ObjectName
	and SCHEMA_NAME(tt.schema_id)=@ObjectSchemaName
ORDER BY
	schemaName ASC, objectName ASC, indexName ASC, ic.key_ordinal ASC



insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	iCols.schemaName,
	iCols.objectName,
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
	INNER JOIN #index_columns iCols ON iCols.schemaName = iName.schemaName
									AND iCols.objectName = iName.objectName
									AND iCols.indexName = iName.indexName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.schemaName = iName.schemaName
			AND ic.objectName = iName.objectName
			AND ic.indexName = iName.indexName
	) colsCount
where
	iName.IndexKeyType>0


--5a. создание индексов

/* шапка индекса */
insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	schemaName,
	objectName,
	7001 +1042*(rn-1) as rownum,
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
		'INDEX [' +iName.indexName +'] ON [' +iName.schemaName +'].[' +iName.objectName +'] (',
	0
from
	#index_name iName
where
	IndexKeyType =0

/* шапка include-секции */
union all
select
	schemaName,
	objectName,
	7018 +1042*(rn-1) as rownum,
	'index include-section' as rowtype,
	0,
	') INCLUDE (',
	0
from
	#index_name iName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.schemaName = iName.schemaName
			AND ic.objectName = iName.objectName
			AND ic.indexName = iName.indexName
			AND ic.isIncluded = 1
	) inclColumns
where
	IndexKeyType =0
	and inclColumns.cnt>0

/* подвал индекса */
union all
select
	schemaName,
	objectName,
	8042 +1042*(rn-1) as rownum,
	'index footer' as rowtype,
	0,
	') ' +
	case [FillFactor]
		when 0 then ''
		else 'WITH (FILLFACTOR=' +cast([FillFactor] as varchar) +') '
	end,
	1
from
	#index_name
where
	IndexKeyType=0


-- 5b. столбцы индекса
/* ключевые поля */
insert @output (
	schemaName,
	objectName,
	rownum,
	rowtype,
	indent,
	sqltext,
	endType
)
select
	iCols.schemaName,
	iCols.objectName,
	7001 +1042 *(iName.rn -1) +iCols.ID, --iCols.ID начинается с 1
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
	INNER JOIN #index_columns iCols ON iCols.schemaName = iName.schemaName
									AND iCols.objectName = iName.objectName
									AND iCols.indexName = iName.indexName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.schemaName = iName.schemaName
			AND ic.objectName = iName.objectName
			AND ic.indexName = iName.indexName
			AND ic.IsIncluded = 0
	) colsCount
where
	iName.IndexKeyType=0
	and iCols.IsIncluded = 0

union all
/* include-поля индекса */
select
	iCols.schemaName,
	iCols.objectName,
	7019 +1042 *(iName.rn -1) +iCols.Included_ID, --iCols.IncludedID начинается с 1
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
	INNER JOIN #index_columns iCols ON iCols.schemaName = iName.schemaName
									AND iCols.objectName = iName.objectName
									AND iCols.indexName = iName.indexName
	outer apply (
		select count(1) cnt
		from #index_columns ic
		where ic.schemaName = iName.schemaName
			AND ic.objectName = iName.objectName
			AND ic.indexName = iName.indexName
			AND ic.IsIncluded = 1
	) colsCount
where
	iName.IndexKeyType=0
	and iCols.IsIncluded = 1


--select * from #ansi_params
--select * from #columns
--SELECT * FROM #colDefaults
--select * from #index_name
--select * from #index_columns

declare @v varchar(max)
select @v = ''
select
	@v = @v +
	space(o.indent) +
	o.sqltext +
	case endtype
		when 0 then ''
		when 1 then char(13) +char(10) +'GO'
		when 2 then ','
	end +
	char(13) +char(10)
from @output o
order by schemaName, objectName, o.rownum



select [definition] =
	@v +char(13) +char(10);

--select *
--from @output
--order by schemaName, objectName, rownum