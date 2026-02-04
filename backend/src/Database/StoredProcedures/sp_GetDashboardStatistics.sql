/*
CREATE PROCEDURE [dbo].[sp_GetDashboardStatistics]
AS
BEGIN
    -- SET NOCOUNT ON prevents extra result sets from
    -- interfering with SELECT statements.
    -- This also slightly improves performance by not
    -- sending row count info back to the client.
    SET NOCOUNT ON;
     -- Single query to get all statistics at once
    -- instead of running 5 separate COUNT queries.
    --
    -- Status values map to our C# enum:
    --   0 = NonImmunised
    --   1 = PartiallyImmunised
    --   2 = FullyImmunised
    --   3 = Overdue
    --
    -- CASE WHEN works like C# ternary operator:
    --   Status = 2 ? 1 : 0
    -- SUM then adds up all the 1s to get the count
    -- for each status type.
    SELECT
        COUNT(*) AS TotalUsers,
        SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS FullyImmunised,
        SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS PartiallyImmunised,
        SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS NonImmunised,
        SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS Overdue
    FROM [dbo].[Users];
END;
GO
*/
EXEC [dbo].[sp_GetDashboardStatistics];