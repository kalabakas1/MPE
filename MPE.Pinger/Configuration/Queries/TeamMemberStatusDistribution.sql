SELECT
	tms.Alias AS Alias
	, COUNT(*) AS Value
FROM
	TeamMember tm WITH(NOLOCK) 
	JOIN Member m WITH(NOLOCK) ON tm.MemberID = m.MemberID AND tm.Deleted = 0 AND m.Deleted = 0
	JOIN Team t WITH(NOLOCK) ON tm.TeamID = t.TeamID AND tm.IsPrimary = 1 AND t.Deleted = 0
	JOIN Season s WITH(NOLOCK) ON t.SeasonID = s.SeasonID AND s.StartDate <= GETDATE() AND s.EndDate <= GETDATE()
	JOIN TeamMemberStatus tms WITH(NOLOCK) ON tm.TeamMemberStatusID = tms.TeamMemberStatusID
GROUP BY
	tms.Alias