SELECT 
	'Logs' AS Alias
	, COUNT(*) AS Value 
FROM 
	Log WITH (NOLOCK)