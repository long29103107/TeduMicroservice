Use [TeduIdentity]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON 
GO 

IF EXISTS (SELECT *
	FROM sysobjects
	WHERE Id = object_id(N'[Get_Permission_ByRoleId]')
		AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [Get_Permission_ByRoleId]
END
GO

CREATE PROCEDURE [Get_Permission_ByRoleId]
	@roleId varchar(50) null

AS 
BEGIN
	-- Set NOCOUNT ON added to prevent extra result sets from
	-- intefering  with SELECT statements
	SET NOCOUNT ON;
	SELECT *
	FROM [Identity].Permissions
	WHERE roleId = @roleId
END

