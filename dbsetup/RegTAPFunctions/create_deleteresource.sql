USE [VORegTAP]
GO
/****** Object:  StoredProcedure [rr].[deletedeprecatedresourcefromtapcache]    Script Date: 3/21/2016 12:02:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--TODO: geenrate CREATE TABLE statements  from tables after key and index management.


--Deprecated resources are not tracked in the regtap cache tables.
--Delete all information relating to prior versions from regtap cache before ingesting new record.
CREATE PROCEDURE [rr].[deletedeprecatedresourcefromtapcache] 
   @ivorn varchar(512),  
   @existingCopies int OUTPUT
AS
  IF @ivorn IS NOT NULL 
  BEGIN
	BEGIN TRANSACTION
		SET @existingCopies = (SELECT COUNT(*) FROM rr.resource WHERE ivoid='@ivorn');
		DELETE FROM rr.capability WHERE ivoid=@ivorn;
		DELETE FROM rr.interface WHERE ivoid = @ivorn;
		DELETE FROM rr.intf_param WHERE ivoid = @ivorn;
		DELETE FROM rr.relationship WHERE ivoid = @ivorn;
		DELETE FROM rr.res_date WHERE ivoid = @ivorn;
		DELETE FROM rr.res_detail WHERE ivoid = @ivorn;
		DELETE FROM rr.res_role WHERE ivoid = @ivorn;
		DELETE FROM rr.res_schema WHERE ivoid = @ivorn;
		DELETE FROM rr.res_subject WHERE ivoid = @ivorn;
		DELETE FROM rr.res_table WHERE ivoid = @ivorn;
		DELETE FROM rr.table_column WHERE ivoid = @ivorn;
		DELETE FROM rr.validation WHERE ivoid = @ivorn;

		DELETE FROM rr.resource WHERE ivoid=@ivorn;
	COMMIT TRANSACTION
  END

GO
