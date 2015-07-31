EXEC util_CreateEmptyProc 'sp[MODEL]_IsDeletable'
GO

ALTER PROCEDURE [dbo].[sp[MODEL]_IsDeletable] @[MODEL]Id INT,
	@UserName NVARCHAR(100),
	@IsDeletable BIT = 0 OUTPUT
	/* WITH ENCRYPTION */
AS
BEGIN
	/**************************************************************************************
	Purpose: Verify if LeadType record can be deletable

	Created By:	Rene Menjivar (ByDesign)
	Date created: 03/06/2014		
	PW Ticket #: 176858
	**************************************************************************************/
	
	SET @IsDeletable = 1
	
	IF EXISTS (SELECT 'X'
	             FROM Reps WITH (NOLOCK) 
	            WHERE [MODEL]ID = @[MODEL]Id)
	BEGIN	            
		SET @IsDeletable = 0
	END

END
GO

PRINT '<<< ALTERED PROC sp[MODEL]_IsDeletable >>>'
