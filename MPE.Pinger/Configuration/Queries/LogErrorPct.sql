WITH result AS (
	SELECT 
		ds.Alias
		, COUNT(*) AS Value
	FROM 
		(SELECT
			CASE WHEN Message = 'Error' OR Message = 'Error Inner' THEN 'Error' ELSE 'Success' END AS 'Alias'
		FROM 
			(SELECT TOP 5000 * FROM Log WITH (NOLOCK) ORDER BY 1 DESC) ds) AS ds
	GROUP BY
		ds.Alias
)
SELECT 
	Alias
	, CAST(Value AS decimal) / 5000 * 100 AS Value
FROM 
	result