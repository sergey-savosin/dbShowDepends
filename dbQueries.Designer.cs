﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dbShowDepends {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class dbQueries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal dbQueries() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("dbShowDepends.dbQueries", typeof(dbQueries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на use [&lt;DB&gt;];.
        /// </summary>
        internal static string changeDB {
            get {
                return ResourceManager.GetString("changeDB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select d.name
        ///from sys.databases d
        ///order by d.name;.
        /// </summary>
        internal static string databaseList {
            get {
                return ResourceManager.GetString("databaseList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на --declare @ObjectName sysname = &apos;dbo.korm_Request_CheckBusinessLogic&apos;;
        ///
        ///if object_id(&apos;tempdb..#weakObj&apos;) is null
        ///	create table #weakObj (objName sysname not null);
        ///
        ///IF EXISTS (
        ///    select 1
        ///    from #weakObj w
        ///    where w.objName = @ObjectName
        ///)
        ///BEGIN
        ///  SELECT &apos;&lt;not avaible: weak object&gt;&apos; refName;
        ///END
        ///ELSE
        ///BEGIN
        ///  SELECT DISTINCT
        ///    ISNULL(re.referenced_schema_name, &apos;dbo&apos;)
        ///    + &apos;.&apos;
        ///    + re.referenced_entity_name
        ///    + ISNULL(&apos;.&apos; + re.referenced_minor_name, &apos;&apos;) refName
        ///  , re.reference [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string findReferencedObjs {
            get {
                return ResourceManager.GetString("findReferencedObjs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на --declare @TableName sysname = &apos;dbo.Table1&apos;
        ///
        ///declare @objid int;
        ///
        ///select @objid = object_id(@TableName);
        ///
        ///select
        /// refName = schema_name(schema_id) + &apos;.&apos; + o.name,
        /// refDbName = NULL, /* current db */
        /// [type] = o.type 
        /// from sys.objects o
        /// where parent_object_id = @objid and type IN (&apos;TR&apos;,&apos;TA&apos;).
        /// </summary>
        internal static string findReferencedTriggers {
            get {
                return ResourceManager.GetString("findReferencedTriggers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select db_name();.
        /// </summary>
        internal static string getDbName {
            get {
                return ResourceManager.GetString("getDbName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select ltrim(rtrim(o.type)) object_type
        ///from sys.objects o
        ///where o.object_id = object_id(@ObjectName)
        ///
        ///UNION ALL
        ///
        ///select ltrim(rtrim(obj.type)) object_type
        ///from sys.table_types tt
        ///INNER JOIN sys.objects AS obj ON obj.object_id = tt.type_table_object_id
        ///where schema_name(tt.schema_id) + &apos;.&apos; + tt.name = @ObjectName.
        /// </summary>
        internal static string getObjectType {
            get {
                return ResourceManager.GetString("getObjectType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на --declare @SearchText sysname;
        ///
        ///select SCHEMA_NAME(o.schema_id) + &apos;.&apos;+ OBJECT_NAME(o.object_id) FullName
        ///, DB_NAME() DatabaseName
        ///, o.type
        ///, o.type_desc
        ///, o.modify_date
        ///from sys.objects o
        ///where o.name like &apos;%&apos; + isnull(@SearchText,&apos;&apos;) + &apos;%&apos;
        ///  AND o.type not in (&apos;S&apos;, &apos;IT&apos;, &apos;SQ&apos;)
        ///  AND o.type IN (&lt;objTypes&gt;)
        ///  and o.is_ms_shipped = 0
        ///
        ///UNION ALL
        ///
        ///select schema_name(tt.schema_id) + &apos;.&apos; + tt.name FullName
        ///, DB_NAME() DatabaseName
        ///, obj.type --&apos;TT&apos; [type]
        ///, obj.type_desc --&apos;TableType&apos; [type_desc [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string objectList {
            get {
                return ResourceManager.GetString("objectList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на --declare @SearchText varchar(max) = &apos;data&apos;;
        ///
        ///with cte as
        ///(
        ///select
        ///	SCHEMA_NAME(o.schema_id) schema_name,
        ///	o.name object_name,
        ///	o.type object_type,
        ///	o.type_desc object_type_desc,
        ///	o.modify_date,
        ///	m.definition [definition],
        ///	left(m.definition, 100) start_definition,
        ///	PATINDEX(&apos;%&apos; + @SearchText + &apos;%&apos;, m.definition) found_definition_index
        ///from sys.sql_modules m
        ///	inner join sys.objects o on o.object_id = m.object_id
        ///WHERE
        ///	m.definition like &apos;%&apos; + @SearchText + &apos;%&apos;
        ///	--AND o.type = &apos;P&apos;
        ///)
        ///select [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string objectSearchBySource {
            get {
                return ResourceManager.GetString("objectSearchBySource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на -- поиск &quot;плохих&quot; объектов
        ///-- (на которые нельзя найти ссылки)
        ///
        ///set nocount on;
        ///
        ///if OBJECT_ID(&apos;tempdb..#weakObj&apos;) is not null
        ///  drop table #weakObj;
        ///create table #weakObj (objId int primary key, objName varchar(100) not null);
        ///
        ///declare @objId int, @objName sysname;
        ///declare @res table (r int not null);
        ///
        ///declare cr cursor local
        ///for
        ///  select o.object_id objId
        ///  , OBJECT_SCHEMA_NAME(o.object_id, DB_ID()) + &apos;.&apos; + o.name objName
        ///  from sys.all_objects o
        ///  where o.type in (&apos;u&apos;, &apos;f&apos;, &apos;fn&apos;, &apos;p&apos;);
        ///        /// [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string prepareWeakObjects {
            get {
                return ResourceManager.GetString("prepareWeakObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на --declare @ObjectName sysname;
        ///--set @ObjectName = &apos;[dbo].[CustOrdersOrders]&apos;
        ///--set @ObjectName = &apos;dbo.Orders&apos;;
        ///
        ///declare @otype char(1);
        ///select top(1) @otype = o.type
        ///from sys.objects o
        ///where o.object_id = object_id(@ObjectName);
        ///
        ///if @otype not in (&apos;U&apos;) /* table */
        ///begin
        ///	SELECT sm.object_id
        ///	, object_schema_name(sm.object_id) + &apos;.&apos; + OBJECT_NAME(sm.object_id) AS object_name
        ///	, o.type
        ///	, o.type_desc
        ///	, sm.definition
        ///	FROM sys.sql_modules AS sm
        ///	JOIN sys.objects AS o ON sm.object_id = o.obje [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string showSource {
            get {
                return ResourceManager.GetString("showSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на set nocount on
        ///--declare @ObjectName sysname = &apos;dbo.LicenseActivation20Event&apos;
        ///
        ////* входящие объекты */
        ///declare @objects table (
        ///	schemaName sysname not null,
        ///	tableName sysname not null
        ///
        ///	primary key (schemaName, tableName)
        ///)
        ///
        ////* исходящие строки */
        ///declare @output table (
        ///	schemaName sysname not null,
        ///	tableName sysname not null,
        ///	rownum bigint not null,
        ///	rowtype varchar(50) not null,
        ///	indent int default(0), --отступ при форматировании запроса
        ///	sqltext varchar(max) null,
        ///	endType tinyint  [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string showTableSource {
            get {
                return ResourceManager.GetString("showTableSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на set nocount on
        ///declare @ObjectFullName sysname;
        ///--set @ObjectFullName = &apos;dbo.UserTableTypeOne&apos;
        ///set @ObjectFullName = @ObjectName;
        ///
        ///declare @ObjectFullNameWithBrackets nvarchar(2000);
        ///
        ////* исходящие строки */
        ///declare @output table (
        ///	objectFullName sysname not null,
        ///	rownum bigint not null,
        ///	rowtype varchar(50) not null,
        ///	indent int default(0), --отступ при форматировании запроса
        ///	sqltext varchar(max) null,
        ///	endType tinyint default(0) --тип строки. 0-обычная, 1-после строки GO, 2-после строки за [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string showTableTypeSource {
            get {
                return ResourceManager.GetString("showTableTypeSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select OBJECT_ID(&apos;tempdb..#weakObj&apos;).
        /// </summary>
        internal static string tempTableId {
            get {
                return ResourceManager.GetString("tempTableId", resourceCulture);
            }
        }
    }
}
