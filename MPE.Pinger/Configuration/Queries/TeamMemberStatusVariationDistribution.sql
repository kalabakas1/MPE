SELECT 
	CONCAT(v.Alias,'.',tms.Alias) AS 'Alias'
	, COUNT(*) AS Value
FROM
	TeamMember tm WITH (NOLOCK)
	JOIN Team t WITH(NOLOCK) ON t.TeamID = tm.TeamID AND tm.Deleted = 0 AND t.Deleted = 0
	JOIN TeamMemberStatus tms WITH(NOLOCK) ON tms.TeamMemberStatusID = tm.TeamMemberStatusID AND tms.Deleted = 0
	JOIN Variety v WITH(NOLOCK) ON t.TeamVarietyID = v.VarietyID AND v.Deleted = 0
	JOIN Season s WITH(NOLOCK) ON t.SeasonID = s.SeasonID AND s.[From] <= GETDATE() AND s.[To] >= GETDATE() AND s.Deleted = 0 AND s.IsActive = 1
GROUP BY
	v.Alias
	, tms.Alias