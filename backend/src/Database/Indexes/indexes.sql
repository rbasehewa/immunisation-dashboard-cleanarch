@"
-- =============================================
-- Author:      Ryan Maddumahewa
-- Create date: 2026-02-03
-- Description: Performance indexes for the
--              ImmunisationDashboard database.
-- =============================================

-- INDEX on Status
-- Why: API filters users by status frequently
CREATE INDEX IX_Users_Status 
ON [dbo].[Users] ([Status]);
GO

-- INDEX on LastImmunisationDate
-- Why: Finding overdue users needs date filtering
CREATE INDEX IX_Users_LastImmunisationDate 
ON [dbo].[Users] ([LastImmunisationDate]);
GO

-- COVERING INDEX
-- Why: Has all columns the query needs in the index
-- SQL Server never needs to touch the main table
CREATE INDEX IX_Users_Status_Covering 
ON [dbo].[Users] ([Status]) 
INCLUDE ([LastName], [FirstName], [Email], [LastImmunisationDate]);
GO
"@ | Set-Content -Path "Database\Indexes\indexes.sql"