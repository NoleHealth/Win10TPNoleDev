--C:\CodeSoa\trunk\SQL\Rollout Scripts\PW 153795 - SOA RankTypes.sql
IF NOT EXISTS (
		SELECT 'x'
		FROM sys.tables st
		INNER JOIN sys.columns sc ON st.object_id = sc.object_id
		INNER JOIN sys.default_constraints dc ON dc.parent_object_id = st.object_id
			AND dc.parent_column_id = sc.column_id
		WHERE st.NAME = 'RankTypes'
			AND sc.NAME = 'CreatedBy'
			AND dc.[DEFINITION] = '(suser_name())'
	)
BEGIN
	ALTER TABLE dbo.RankTypes ADD DEFAULT(SUSER_NAME())
	FOR CreatedBy
END




--*******************************
-- CREATE A NEW LEAD STATUS MENU
--*******************************
DECLARE @NextMenuItemID INT

SELECT @NextMenuItemID = MAX(MenuItemID) + 1
  FROM MilonicMenuItem

IF NOT EXISTS (SELECT 'X' 
                 FROM MilonicMenuItem WITH (NOLOCK) 
                WHERE MenuItemName LIKE 'Admin_SOARepClassificationTypes')
BEGIN                	
	INSERT INTO MilonicMenuItem (MenuItemID,ParentMenuID,MenuItemName,MenuItemText,ShowMenuID,StatusText,Url,SeperatorSize,Position,IsAdmin,IsActive,IsCustom,Setting,SettingValue,Setting_ShowAdminOnlyIfFalse,IsRelatedToCompEngine,ApplicationID)
	VALUES(@NextMenuItemID,26,'Admin_SOARepClassificationTypes','Rep Classification Types',NULL,NULL,'~/Admin/RepClassificationType',0,300,0,1,0,NULL,NULL,0,0,300)	
	
	UPDATE MilonicMenuItem SET
	       IsActive = 0
	 WHERE MenuItemName LIKE 'Admin_RepClassificationTypes'
	 
	PRINT 'SOA Admin_SOARepClassificationTypes MENU WAS CREATED AND OLD MENU WAS DISABLED'
END
ELSE BEGIN
	PRINT 'SOA Admin_SOARepClassificationTypes MENU EXISTS AND OLD MENU WAS DISABLED'
END