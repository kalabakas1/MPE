SELECT
	CONCAT(pc.Name,'.', pls.Alias) AS Alias
	, p.Name AS Message
	, SUM(pr.Quantity) AS Value
FROM 
	ProductRelation pr WITH(NOLOCK)
	JOIN Product p WITH(NOLOCK) ON pr.ProductID = p.ProductID AND pr.Deleted = 0 AND p.Deleted = 0 AND pr.MemberID IS NOT NULL
	JOIN Season s WITH(NOLOCK) ON p.SeasonID = s.SeasonID AND s.StartDate <= GETDATE() AND s.EndDate >= GETDATE()
	JOIN ProductCategory pc WITH(NOLOCK) ON p.ProductCategoryID = pc.ProductCategoryID AND pc.Deleted = 0
	JOIN ProductType pt WITH(NOLOCK) ON p.ProductTypeID = pt.ProductTypeID AND pt.Deleted = 0
	JOIN ProductLogic pl WITH(NOLOCK) ON pl.ProductLogicID = pt.ProductLogicID AND pl.Deleted = 0
	JOIN ProductLogicStatus pls WITH(NOLOCK) ON pr.ProductLogicStatusID = pls.ProductLogicStatusID
GROUP BY
	pc.Name
	, p.Name
	, pls.Alias