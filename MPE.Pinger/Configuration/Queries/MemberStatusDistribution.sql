SELECT
	ms.Alias AS Alias
	, COUNT(*) AS Value
FROM	
	Member m WITH(NOLOCK)
	JOIN MemberStatus ms WITH(NOLOCK) ON m.MemberStatusID = ms.MemberStatusID AND m.Deleted = 0 AND ms.Deleted = 0
GROUP BY
	ms.Alias