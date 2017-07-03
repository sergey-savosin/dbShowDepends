#Set-ExecutionPolicy Unrestricted
#[System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.SMO')

$serverName   = "(localdb)\ProjectsV12"
$databaseName = "Northwind"
$filePath = "result.sql"

$objectArray = `
,"proc dbo.CustOrderHist"`
,"view dbo.Invoices"

$scriptingOptions = New-SqlScriptingOptions
$scriptingOptions.ScriptBatchTerminator = $true
$scriptingOptions.IncludeDatabaseContext = $true
$scriptingOptions.IncludeHeaders = $true
$scriptingOptions.EnforceScriptingOptions = $true
$scriptingOptions.FileName = "d:\work\sql\result.sql"
$scriptingOptions.ToFileOnly = $true
$scriptingOptions.AppendToFile = $true

# Header: Contents of the file
$strHeader = ("USE " + $databaseName)
$strHeader = $strHeader + ("`r`n`r`n/* This file contains:`r`n")

foreach ($object in $objectArray)
{
	$objectType = $object.Split(" ")[0]
	$schemaName = $object.Split(" ")[1].Split(".")[0].Replace("[", "").Replace("]","")
	$objectName = $object.Split(" ")[1].Split(".")[1].Replace("[", "").Replace("]","")

 	$strHeader = $strHeader +  ($objectType + " " + $schemaName + "." + $objectName + "`r`n")
}

$strHeader = $strHeader + $("*/`r`n`r`n")

Out-File -FilePath $filePath -InputObject $strHeader



# Source code of DB objects
foreach ($object in $objectArray)
{
#Get-SqlData "(localdb)\ProjectsV12" "Northwind" "select schema_name(schema_id) sch, name, type from sys.objects where name = 'Orders'"
	$objectType = $object.Split(" ")[0]
	$schemaName = $object.Split(" ")[1].Split(".")[0].Replace("[", "").Replace("]","")
	$objectName = $object.Split(" ")[1].Split(".")[1].Replace("[", "").Replace("]","")

	# Header
 	Write-Output ("")
 	Write-Output ("----------------------------------------------")
 	Write-Output ("-- " + $objectType + ": " + $schemaName + "." + $objectName)
 	Write-Output ("----------------------------------------------")
 	Write-Output ("")

	# Object text
	if ($objectType = "view")
	{
		Get-SqlDatabase -dbname $databaseName -sqlserver $serverName | Get-SqlView -schema $schemaName -name $objectName | Get-SqlScripter -scriptingOptions $scriptingOptions
	}
	if ($objectType = "proc")
	{
		Get-SqlDatabase -dbname $databaseName -sqlserver $serverName | Get-SqlStoredProcedure -schema $schemaName -name $objectName | Get-SqlScripter -scriptingOptions $scriptingOptions
	}
	if ($objectType = "table")
	{
		Get-SqlDatabase -dbname $databaseName -sqlserver $serverName | Get-SqlTable -schema $schemaName -name $objectName | Get-SqlScripter -scriptingOptions $scriptingOptions
	}
##index
##Get-SqlForeignKey
##Get-SqlTrigger
##Get-SqlUserDefinedDataType
##Get-SqlUserDefinedFunction
##Get-SqlForeignKey
##Get-SqlForeignKey

	# Footer
}