SELECT 
	CONCAT(tms.Alias, '.', CASE WHEN TermsNodeID IS NOT NULL THEN 'Accepted' ELSE 'NotAccepted' END) AS Alias
	, COUNT(*) AS Value
FROM MemberAcceptedToS ac WITH (NOLOCK)
	FULL OUTER JOIN Member m WITH (NOLOCK) ON ac.MemberID = m.MemberID
	JOIN TeamMember tm WITH(NOLOCK) ON m.MemberID = tm.MemberID AND tm.Deleted = 0 AND tm.IsPrimary = 1
	JOIN TeamMemberStatus tms WITH(NOLOCK) ON tm.TeamMemberStatusID = tms.TeamMemberStatusID AND tms.Deleted = 0
	JOIN Team t WITH(NOLOCK) ON tm.TeamID = t.TeamID AND t.SystemTeam = 0 AND t.Deleted = 0
	JOIN TeamType tt WITH(NOLOCK) ON t.TeamTypeID = tt.TeamTypeID AND tt.Alias = 'Team'
	JOIN Season s WITH(NOLOCK) ON t.SeasonID = s.SeasonID AND s.StartDate <= GETDATE() AND s.EndDate >= GETDATE()
WHERE
	m.Deleted = 0 
	AND (ac.Deleted = 0 OR ac.Deleted IS NULL)
	AND (ac.TermsNodeID = (select distinct top 1  termsnodeid from TermsOfService order by 1 desc)
		OR ac.TermsNodeID IS NULL)
GROUP BY
	TermsNodeID
	, tms.Alias