Use [TeduIdentity]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON 
GO 

IF EXISTS (SELECT *
	FROM sysobjects
	WHERE Id = object_id(N'[Create_Permission]')
		AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE Create_Permission
END
GO

CREATE PROCEDURE [Create_Permission]
	@roleId varchar(50) null,
	@function varchar(50) null,
	@command varchar(50) null,
	@newId bigint output
AS 
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN
	BEGIN TRY
			
	IF NOT EXISTS (SELECT * 
		FROM [Identity].Permissions
		WHERE [RoleId] = @roleId AND
			[Function] = @function AND
			[Command] = @command)
	BEGIN
		INSERT INTO [Identity].Permissions([RoleId],[Function],[Command])
		VALUES(@roleId, @function, @command)
		SET @newId = SCOPE_IDENTITY();
	END
	COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		DECLARE @ErrorMessage varchar(2000)
		SELECT @ErrorMessage = 'ERROR: ' + ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16,1)
	END CATCH
END