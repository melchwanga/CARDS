DECLARE @ParentId INT;

insert into SystemCodes(Code,Description,IsUserMaintained)
	select T1.Code, T1.[Description], T1.IsUserMaintained
	from
		(
			SELECT 'CardStatus' Code, 'Options for various card statuses' [Description], 0 IsUserMaintained
			UNION
			SELECT 'CardSortableFields' Code, 'Options for various fields for which a list of cards can be sorted' [Description], 1 IsUserMaintained
			
		) T1 LEFT JOIN SystemCodes T2 ON T1.CODE = T2.Code 
	WHERE T2.Code IS NULL

		select @ParentId = Id
	from SystemCodes
	where Code = 'CardSortableFields'
    insert into SystemCodeDetails(SystemCodeId,Code,Description,CreatedOn)
	select @ParentId, T1.Code, T1.[Description], getdate() CreatedOn
	from
		(
			SELECT 'Name' Code, 'Name' [Description]
			  UNION
			SELECT 'Color' Code, 'Color' [Description]
			  UNION
			SELECT 'Status' Code, 'Status' [Description]
			  UNION
			SELECT 'DateCreated' Code, 'Date Created' [Description]
	) T1 LEFT JOIN SystemCodeDetails T2 ON T1.CODE = T2.Code  AND T2.SystemCodeId = @ParentId
	WHERE T2.Code IS NULL

	select @ParentId = Id
	from SystemCodes
	where Code = 'CardStatus'
    insert into SystemCodeDetails(SystemCodeId,Code,Description,CreatedOn)
	select @ParentId, T1.Code, T1.[Description], getdate() CreatedOn
	from
		(
			SELECT 'ToDo' Code, 'To Do' [Description]
			  UNION
			SELECT 'InProgress' Code, 'In Progress' [Description]
			  UNION
			SELECT 'Done' Code, 'Done' [Description]
	) T1 LEFT JOIN SystemCodeDetails T2 ON T1.CODE = T2.Code  AND T2.SystemCodeId = @ParentId
	WHERE T2.Code IS NULL


