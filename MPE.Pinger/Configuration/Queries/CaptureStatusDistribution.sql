SELECT 
	t.CaptureStatusName AS Alias
	, COUNT(*) AS Value
FROM 
	[Transaction] t WITH (NOLOCK) 
	JOIN OrderLine ol WITH(NOLOCK) ON t.OrderLineID = ol.OrderLineID AND t.Deleted = 0 AND ol.Deleted = 0 AND t.CaptureStatusName IS NOT NULL
	JOIN [Order] o WITH (NOLOCK) ON o.OrderID = ol.OrderID AND o.Deleted = 0 
	JOIN Season s WITH (NOLOCK) ON o.SeasonID = s.SeasonID AND s.[From] <= GETDATE() AND s.[To] >= GETDATE() AND s.Deleted = 0 AND s.IsActive = 1
GROUP BY
	t.CaptureStatusName