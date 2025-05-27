SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Amortizations TABLE (
        [Rate] [decimal](8,5) NOT NULL,
        [RateInterestOnly] [decimal](8,5) NOT NULL,
        [Rate10Year] [decimal](8,5) NOT NULL,
        [Rate15Year] [decimal](8,5) NOT NULL,
        [Rate20Year] [decimal](8,5) NOT NULL,
        [Rate25Year] [decimal](8,5) NOT NULL,
        [Rate30Year] [decimal](8,5) NOT NULL,
        [Rate40Year] [decimal](8,5) NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @Amortizations (Rate, RateInterestOnly, Rate10Year, Rate15Year, Rate20Year, Rate25Year, Rate30Year, Rate40Year)
        VALUES (2, 0.16667, 9.20135, 6.43509, 5.05883, 4.23854, 3.69619, 3.02826)
            , (2.125, 0.17708, 9.25743, 6.49281, 5.11825, 4.29966, 3.75902, 3.09444)
            , (2.25, 0.1875, 9.31374, 6.55085, 5.17808, 4.36131, 3.82246, 3.16142)
            , (2.375, 0.19792, 9.37026, 6.60921, 5.23834, 4.42348, 3.88653, 3.22921)
            , (2.5, 0.20833, 9.42699, 6.66789, 5.29903, 4.48617, 3.95121, 3.29778)
            , (2.625, 0.21875, 9.48394, 6.72689, 5.36014, 4.54938, 4.01651, 3.36714)
            , (2.75, 0.22917, 9.5411, 6.78622, 5.42166, 4.61311, 4.08241, 3.43728)
            , (2.875, 0.23958, 9.59848, 6.84586, 5.48361, 4.67735, 4.14892, 3.50818)
            , (3, 0.25, 9.65607, 6.90582, 5.54598, 4.74211, 4.21604, 3.57984)
            , (3.125, 0.26042, 9.71388, 6.96609, 5.60876, 4.80738, 4.28375, 3.65226)
            , (3.25, 0.27083, 9.7719, 7.02669, 5.67196, 4.87316, 4.35206, 3.72541)
            , (3.375, 0.28125, 9.83014, 7.0876, 5.73557, 4.93945, 4.42096, 3.7993)
            , (3.5, 0.29167, 9.88859, 7.14883, 5.7996, 5.00624, 4.49045, 3.87391)
            , (3.625, 0.30208, 9.94725, 7.21037, 5.86404, 5.07352, 4.56051, 3.94923)
            , (3.75, 0.3125, 10.00612, 7.27222, 5.92888, 5.14131, 4.63116, 4.02526)
            , (3.875, 0.32292, 10.06521, 7.3344, 5.99414, 5.20959, 4.70237, 4.10198)
            , (4, 0.33333, 10.12451, 7.39688, 6.0598, 5.27837, 4.77415, 4.17938)
            , (4.125, 0.34375, 10.18403, 7.45968, 6.12587, 5.34763, 4.8465, 4.25746)
            , (4.25, 0.35417, 10.24375, 7.52278, 6.19234, 5.41738, 4.9194, 4.3362)
            , (4.375, 0.36458, 10.30369, 7.5862, 6.25922, 5.48761, 4.99285, 4.41559)
            , (4.5, 0.375, 10.36384, 7.64993, 6.32649, 5.55832, 5.06685, 4.49563)
            , (4.625, 0.38542, 10.4242, 7.71397, 6.39417, 5.62951, 5.1414, 4.57629)
            , (4.75, 0.39583, 10.48477, 7.77832, 6.46224, 5.70117, 5.21647, 4.65758)
            , (4.875, 0.40625, 10.54556, 7.84297, 6.5307, 5.7733, 5.29208, 4.73947)
            , (5, 0.41667, 10.60655, 7.90794, 6.59956, 5.8459, 5.36822, 4.82197)
            , (5.125, 0.42708, 10.66776, 7.9732, 6.66881, 5.91896, 5.44487, 4.90505)
            , (5.25, 0.4375, 10.72917, 8.03878, 6.73844, 5.99248, 5.52204, 4.9887)
            , (5.375, 0.44792, 10.79079, 8.10465, 6.80847, 6.06645, 5.59971, 5.07293)
            , (5.5, 0.45833, 10.85263, 8.17083, 6.87887, 6.14087, 5.67789, 5.1577)
            , (5.625, 0.46875, 10.91467, 8.23732, 6.94966, 6.21575, 5.75656, 5.24302)
            , (5.75, 0.47917, 10.97692, 8.3041, 7.02084, 6.29106, 5.83573, 5.32888)
            , (5.875, 0.48958, 11.03938, 8.37118, 7.09238, 6.36682, 5.91538, 5.41525)
            , (6, 0.5, 11.10205, 8.43857, 7.16431, 6.44301, 5.99551, 5.50214)
            , (6.125, 0.51042, 11.16493, 8.50625, 7.23661, 6.51964, 6.07611, 5.58952)
            , (6.25, 0.52083, 11.22801, 8.57423, 7.30928, 6.59669, 6.15717, 5.6774)
            , (6.375, 0.53125, 11.2913, 8.6425, 7.38232, 6.67417, 6.2387, 5.76575)
            , (6.5, 0.54167, 11.3548, 8.71107, 7.45573, 6.75207, 6.32068, 5.85457)
            , (6.625, 0.55208, 11.4185, 8.77994, 7.5295, 6.83039, 6.40311, 5.94385)
            , (6.75, 0.5625, 11.48241, 8.84909, 7.60364, 6.90912, 6.48598, 6.03357)
            , (6.875, 0.57292, 11.54653, 8.91854, 7.67814, 6.98825, 6.56929, 6.12373)
            , (7, 0.58333, 11.61085, 8.98828, 7.75299, 7.06779, 6.65302, 6.21431)
            , (7.125, 0.59375, 11.67537, 9.05831, 7.8282, 7.14773, 6.73719, 6.30531)
            , (7.25, 0.60417, 11.7401, 9.12863, 7.90376, 7.22807, 6.82176, 6.39672)
            , (7.375, 0.61458, 11.80504, 9.19923, 7.97967, 7.3088, 6.90675, 6.48852)
            , (7.5, 0.0625, 11.87017, 9.27101, 8.05593, 7.38991, 6.99215, 6.58071)
            , (7.625, 0.63542, 11.93552, 9.3413, 8.13254, 7.47141, 7.07794, 6.67327)
            , (7.75, 0.64583, 12.00106, 9.41276, 8.20949, 7.55329, 7.16412, 6.7662)
            , (7.875, 0.65625, 12.06681, 9.4845, 8.28677, 7.63554, 7.25069, 6.85948)
            , (8, 0.66667, 12.13276, 9.55652, 8.3644, 7.71816, 7.33765, 6.95312)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Amortizations M
    WHERE M.Rate NOT IN (SELECT Rate FROM dbo.tblMasterAmortizations);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Amortizations M
    JOIN dbo.tblMasterAmortizations T ON T.Rate = M.Rate;

    -- Insert new entries
    INSERT INTO dbo.tblMasterAmortizations (Rate, RateInterestOnly, Rate10Year, Rate15Year
                                            , Rate20Year, Rate25Year, Rate30Year, Rate40Year
                                            , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.Rate, 'ADD', 'Added Amortizations'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT Rate, RateInterestOnly, Rate10Year, Rate15Year
            , Rate20Year, Rate25Year, Rate30Year, Rate40Year
            , N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Amortizations M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.RateInterestOnly  = M.RateInterestOnly
        , T.Rate10Year      = M.Rate10Year
        , T.Rate15Year      = M.Rate15Year
        , T.Rate20Year      = M.Rate20Year
        , T.Rate25Year      = M.Rate25Year
        , T.Rate30Year      = M.Rate30Year
        , T.Rate40Year      = M.Rate40Year
        , T.ModifiedBy      = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.Rate, 'UPDATE', 'Updated Amortizations'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterAmortizations T
    JOIN @Amortizations M ON M.Rate = T.Rate
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'AMORTIZATION', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO