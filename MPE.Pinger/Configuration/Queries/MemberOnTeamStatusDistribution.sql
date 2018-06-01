SELECT
	tms.Alias AS Alias
	, COUNT(*) AS Value
FROM 
	TeamMember tm WITH(NOLOCK) 
	JOIN Member m WITH(NOLOCK) ON tm.MemberID = m.MemberID AND m.Deleted = 0 AND tm.Deleted = 0 AND tm.IsPrimary = 1
	JOIN TeamMemberStatus tms WITH(NOLOCK) ON tm.TeamMemberStatusID = tms.TeamMemberStatusID AND tms.Deleted = 0
	JOIN Team t WITH(NOLOCK) ON tm.TeamID = t.TeamID AND t.SystemTeam = 0 AND t.Deleted = 0
	JOIN TeamType tt WITH(NOLOCK) ON t.TeamTypeID = tt.TeamTypeID AND tt.Alias = 'Team'
	JOIN Season s WITH(NOLOCK) ON t.SeasonID = s.SeasonID AND s.StartDate <= GETDATE() AND s.EndDate >= GETDATE()
GROUP BY 
	tms.Alias