Use [TeduIdentity]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON 
GO 

IF EXISTS (SELECT *
	FROM sysobjects
	WHERE Id = object_id(N'[dbo].[Delete_Permission]')
		AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[Delete_Permission]
END
GO

CREATE PROCEDURE Delete_Permission
	@roleId varchar(50),
	@function varchar(50),
	@command varchar(50)
AS 
BEGIN
	DELETE
	FROM [Identity].Permissions
	WHERE [RoleId] = @roleId AND
		[Function] = @function AND
		[Command] = @command
END

GO