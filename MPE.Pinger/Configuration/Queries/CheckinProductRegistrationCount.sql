SELECT
	CONCAT(pc.Name, '.', pls.Alias) AS Alias
	, COUNT(*) AS Value
FROM 
	PluginCheckin_Registration r WITH(NOLOCK)
	JOIN ProductRelation pr WITH(NOLOCK) ON pr.ProductRelationID = r.ProductRelationID
	JOIN Product p WITH(NOLOCK) ON pr.ProductID = p.ProductID
	JOIN ProductCategory pc WITH(NOLOCK) ON pc.ProductCategoryID = p.ProductCategoryID
	JOIN ProductLogicStatus pls WITH(NOLOCK) ON pls.ProductLogicStatusID = pr.ProductLogicStatusID
	JOIN Season s WITH(NOLOCK) ON p.SeasonID = s.SeasonID AND s.StartDate <= GETDATE() AND s.EndDate >= GETDATE()
GROUP BY
	pc.Name
	, pls.Alias