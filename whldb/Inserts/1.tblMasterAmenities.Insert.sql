SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Amenities TABLE (
        [Name] [varchar](100) NOT NULL,
        [Description] [varchar](1000) NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @Amenities ([Name], [Description])
        VALUES (N'Accessible entrance', NULL)
            , (N'Air-conditioning', NULL)
            , (N'Assigned parking spaces', NULL)
            , (N'Bike storage lockers', NULL)
            , (N'Bus stop/Public transportation access', NULL)
            , (N'Business Center', NULL)
            , (N'Cable or satellite TV', NULL)
            , (N'Close to schools', NULL)
            , (N'Common area Wifi', NULL)
            , (N'Covered parking', NULL)
            , (N'Dishwasher in unit', NULL)
            , (N'Dog washing station', NULL)
            , (N'Elevator', NULL)
            , (N'Energy-efficient appliances', NULL)
            , (N'Garages', NULL)
            , (N'Gated access', NULL)
            , (N'Gymnasium', NULL)
            , (N'Hardwood floors', NULL)
            , (N'High-end countertops and finishes', NULL)
            , (N'High-end kitchen appliances', NULL)
            , (N'High-speed internet', NULL)
            , (N'Intercommunication device', NULL)
            , (N'Jogging/walking/bike path or access to one nearby', NULL)
            , (N'Online options for leasing, paying rent, and making maintenance requests', NULL)
            , (N'On-site resident manager', NULL)
            , (N'Outdoor areas', NULL)
            , (N'Outdoor terrace', NULL)
            , (N'Package lockers', NULL)
            , (N'Pedestrian-friendly - Walk Score', NULL)
            , (N'Pet-friendly', NULL)
            , (N'Rooftop terrace', NULL)
            , (N'Security cameras', NULL)
            , (N'Shared laundry room', NULL)
            , (N'Smart controls for heating/cooling', NULL)
            , (N'Smoke-free', NULL)
            , (N'Valet trash', NULL)
            , (N'Virtual doorman', NULL)
            , (N'Washers and dryers in units', NULL)
            , (N'Yoga/dance studio', NULL)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Amenities M
    WHERE M.[Name] NOT IN (SELECT [Name] FROM dbo.tblMasterAmenities);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Amenities M
    JOIN dbo.tblMasterAmenities T ON T.[Name] = M.[Name] AND ISNULL(RTRIM(T.[Description]), '') <> ISNULL(RTRIM(M.[Description]), '');

    -- Insert new entries
    INSERT INTO dbo.tblMasterAmenities ([Name], [Description], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.AmenityID, 'ADD', 'Added Amenity'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.[Name], M.[Description], N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Amenities M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Description] = M.[Description]
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.AmenityID, 'UPDATE', 'Updated Amenity'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterAmenities T
    JOIN @Amenities M ON M.[Name] = T.[Name]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'AMENITY', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO