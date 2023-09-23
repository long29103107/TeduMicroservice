Use [TeduIdentity]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON 
GO 

IF EXISTS (SELECT *
	FROM sysobjects
	WHERE Id = object_id(N'[Update_Permissions_ByRole]')
		AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE Update_Permissions_ByRole
END
GO

DROP TYPE IF EXISTS [dbo].[Permission]
CREATE TYPE [dbo].[Permission] AS TABLE(
	[RoleId] varchar(50) not null,
	[Function] varchar(50) not null,
	[Command] varchar(50) not null
)
GO

CREATE PROCEDURE Update_Permissions_ByRole
	@roleId varchar(50) null,
	@permissions Permission readonly
AS 
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN
	BEGIN TRY
			
	DELETE FROM [Identity].Permissions WHERE RoleId = @roleId;
	INSERT INTO [Identity].Permissions([RoleId], [Function], [Command])
	SELECT [RoleId], [Function], [Command]
	FROM @permissions

	COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		DECLARE @ErrorMessage varchar(2000)
		SELECT @ErrorMessage = 'ERROR: ' + ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16,1)
	END CATCH
END

GO