USE [DCordBot]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.[spCheckUserMarriageStatus]
@UserID bigint,
@ServerID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RESULT int = 0;
	DECLARE @userID1 bigint;
	DECLARE @userID2 bigint;
    -- Insert statements for procedure here
	SELECT User1,User2 FROM dbo.Marriages WHERE (@UserID = User1 OR @UserID = User2) AND @ServerID = ServerID
	
	if @@ERROR<> 0 
	BEGIN
		RETURN @RESULT
	END
	
	SET @RESULT = @@ERROR
	return @RESULT
END
GO
