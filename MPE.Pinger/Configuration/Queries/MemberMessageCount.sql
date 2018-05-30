SELECT 
	'Messages' AS Alias
	, COUNT(*) AS Value
FROM 
	MemberMessage mm WITH (NOLOCK)
