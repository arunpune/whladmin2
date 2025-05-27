SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @FAQs TABLE (
        [CategoryName] [varchar](100) NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](4000) NOT NULL,
        [Url] [varchar](500) NULL,
        [Url1] [varchar](500) NULL,
        [Url2] [varchar](500) NULL,
        [Url3] [varchar](500) NULL,
        [Url4] [varchar](500) NULL,
        [Url5] [varchar](500) NULL,
        [Url6] [varchar](500) NULL,
        [Url7] [varchar](500) NULL,
        [Url8] [varchar](500) NULL,
        [Url9] [varchar](500) NULL,
        [DisplayOrder] [int] NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @FAQs (CategoryName, Title, [Text], [Url], Url1, Url2, Url3, Url4, Url5, Url6, Url7, Url8, Url9, DisplayOrder)
        VALUES ('General', 'What is HomeSeeker?', 'HomeSeeker is Westchester County''s website for marketing Fair and Affordable Housing.  Westchester County provides assistance to for profit and not for profit developers to create affordable housing opportunities for various eligible household income levels and sizes.  These developments are privately owned and operated.  HomeSeeker is also your portal to find and apply for affordable housing opportunities in Westchester after you set up a Household Profile.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
            , ('General', 'What is Affordable Housing?', 'Affordable housing is generally defined as housing for which the occupants are paying no more than 30 percent of their gross income for all housing-related costs, including utilities. The rents or sales prices for affordable housing are determined so as to be affordable to households at a certain specified area median income levels established by the federal government, generally between 30% and 80% of the area median income. See - Westchester County 2024 Income & Rent Limits Program Guidelines (westchestergov.com)', 'https://homes.westchestergov.com/images/stories/pdfs/24inclimguide53124.pdf', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 2)
            , ('General', 'What kind of affordable housing is marketed by Westchester County on HomeSeeker?', 'Affordable housing opportunities can be either apartments for rent or homes for sale.  Rentals are regulated so the rent increases are limited over time. Condos, coops, and 1-4 family homes have limited re-sale prices to ensure they stay affordable.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3)
            , ('General', 'How does HomeSeeker work?', 'HomeSeeker allows you to create a Household Profile, search housing opportunities, download brochures, review income and occupancy requirements, check eligibility and electronically submit applications for affordable housing. ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 4)
            , ('General', 'What is the benefit of creating a Household Profile?', 'Completing a Household Profile is the first step in applying for housing through HomeSeeker.  It allows you to receive consistent updates on affordable housing opportunities in Westchester County. It also allows you to learn about upcoming housing lotteries and gives you a head start on applying for specific housing opportunities without having to create a separate profile or application for each opportunity.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 5)
            , ('General', 'Is there a fee for creating Household Profile or for submitting an application?', 'There is no fee for using HomeSeeker.  With respect to any particular affordable housing opportunity, you may be charged a non-refundable credit and/or background check fee by the managing agent for that property.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 6)
            , ('General', 'What information is included in a Household Profile?', 'The Household Profile captures general information about the number of people you expect to live with you in any prospective affordable housing, total annual gross income for the entire household and the value of certain household assets or accounts.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7)
            , ('General', 'How to start your search?', 'Go to the HomeSeeker Active properties pages and review the housing opportunities that are currently accepting applications.  Are you interested in any of the locations?  Does your household size and household income match with what has been designated by the building representative?', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 8)
            , ('General', 'What is the application process?', 'Before applying for a specific opportunity, go to your Profile page and confirm your information in your Household Profile is up-to-date and accurate.  The make-up of your household, or your household income, may change over time, please make sure that information is updated. Are you interested in any of the buildings? Check the details to see if your household size and income fit the limits for the buildings. Click "Apply".', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9)
            , ('General', 'How do you determine which applications are approved to rent or purchase a property?', 'Most properties select applicants based on a lottery. However some opportunities will instead be available on a first-come, first-served basis.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 10)
            , ('General', 'What kind of affordable housing applications exist?', 'Some applications are for rental housing and some are for home purchase.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 11)
            , ('General', 'How does a housing lottery work?', 'Check HomeSeeker for "Open Lotteries" to see which property or properties are accepting applications.  If you are interested in living in any of those buildings, check the details to see if your household size and income fit the limits for the buildings. Click "Apply" before the application deadline date and be sure to complete the application. Need help? Not sure if you qualify? Visit Westchester Residential Opportunities, Inc or the specific Building Representative listed as the contact.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12)
            , ('General', 'What happens after the lottery deadline?', 'Each complete application gets a random number - a lottery number.  That lottery number determines the order in which applicants will be considered for that housing opportunity. No one controls who has a better or worse number. It does not matter if you applied first or last, online or on paper - your lottery number is chosen at random. Just be sure to apply before the deadline. You will be notified of your lottery number within approximately 10 days after the lottery. Please make sure that your email address in your profile is up-to-date and working, since that''s the primary way we will correspond with you.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13)
            , ('General', 'What happens after you apply?', 'After you apply, you may not hear anything until the lottery drawing occurs. The listing for each building should tell you the lottery date. Once the lottery is complete, you will be notified of your lottery number. The property manager will then start reviewing applications in their lottery order (starting at #1, #2, #3, etc.). It can take several months or longer to hear about your application. Sometimes you might not hear back for a while even if you qualify. While you''re waiting, carefully check what your current lease says about moving out before the lease ends. If you''re offered an affordable housing opportunity, you might need to move quickly. If you aren''t selected or don''t hear back, keep searching for other apartments and apply when you are ready. NOTE: If your Household Summary is flagged as incomplete, your application may NOT be selected for processing.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14)
            , ('General', 'What happens if your household is selected in a lottery or by a property manager for a housing opportunity?', 'If the application for your household is selected, the property manager or their representative will be in contact with you.  You''ll be invited to respond to a document request to confirm you qualify for the opportunity.  Get ready ahead of time and make sure you''re prepared.  Keep your contact information and profile up to date, so that you don''t lose out on any opportunities.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 15)
            , ('General', 'What documents and information will have to be shared to ensure you qualify for the specific housing opportunity?', 'This document exchange or interview could be in person, by phone or by email.  It is very important. At the interview, you''ll need to show documents that prove the information you put in your application. Start collecting copies today! You''ll have to show: 1) Who will live with you. Examples: birth certificates, government IDs, and 2) The incomes and assets of everyone who will live with you. Examples: pay stubs; 1099s federal or state tax returns; proof of Social Security, veteran, or public assistance benefits income, bank statements, investment and pension statements, alimony and child support orders, retirement statements', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 16)
            , ('General', 'What happens if you are asked to respond to a request for documents to confirm your Household qualifies?', 'If a marketing agent or building representative determines that your household either qualifies or was rejected you will be notified on your Household Summary page and you will be contacted by email.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 17)
            , ('General', 'Why would your application for a specific housing opportunity be rejected?', 'The most common reasons for rejection are not meeting income (minimum or maximum) or household eligibility requirements.  In some cases one member of the Household may have recently turned 18 and has no income or assets associated with this account. You may also be denied based on the results of credit or background checks conducted by the managing agent, or based on your previous rental history.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 18)
            , ('General', 'What if I don''t want to apply online?', 'Applying online is easy and secure, so we recommend it. But if you don''t want to apply online, you have the option of submitting a paper application. You may have an application mailed to you, or emailed to you so you can print it out yourself and complete it. Refer to the details of any open lottery to learn more about who to contact to request a paper application and where completed applications must be sent. Submit ONLY one application per household. You may be disqualified if more than one application is received per lottery for your household. If you submit an application online, your household should NOT also submit an application via mail for that development. If you submit an application via mail, your household should NOT also submit an application online for that development.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 19)
            , ('General', 'How do I appeal a denial?', 'If at any point in the course of applying for a particular development, your application is denied or rejected by the managing agent for that development, you will receive a written denial or rejection notice from the managing agent. This notice should explain why you were denied, what the process is for appealing the decision, and how long you have to appeal the decision before it will become final and non-appealable. If you believe that your application was denied in error, follow the instructions in the denial notice. You may be entitled to submit additional documentation to support your application, or to request a meeting to argue why the denial was inappropriate. You may be entitled to bring a representative with you to that meeting. Act quickly. The notice will tell you how many days you have to submit an appeal.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 20)
            , ('People with Disabilities', 'What qualifies as a disability in housing?', 'The federal Fair Housing Act defines disability as a physical or mental impairment that substantially limits one or more major life activities.  To meet this definition a person must have an impairment that prevents or severely restricts the person from doing activities that are of central importance in most people''s daily lives.  The New York State Human Rights Law defines disability as (a) a physical, mental or medical impairment resulting from anatomical, physiological, genetic or neurological conditions which prevents the exercise of a normal bodily function or is demonstrable by medically accepted clinical or laboratory diagnostic techniques or (b) a record of such an impairment or (c) a condition regarded by others as such an impairment.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101)
            , ('People with Disabilities', 'What is a reasonable accommodation or modification?', 'An accommodation is a change or exception to a rule, policy, practice or procedures of the housing provider. For example, allowing a service animal in a no-pets building. A modification is a structural change to the premises, like installing a ramp or adding grab bars in the bathroom. You are entitled to equal housing opportunity. Part of what that means is that your housing provider is required to make reasonable accommodations and modifications to the building or living space that are necessary to alleviate symptoms or effects of your disability. If you request a reasonable accommodation or modification, the housing provider must grant your request if (A) the accommodation requested is reasonable and necessary to allow the individual to fully use and enjoy residing in our community or (B) the modification of our physical premises is reasonable and necessary to afford a disabled resident full enjoyment of the premises.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 102)
            , ('People with Disabilities', 'How do I request a reasonable accommodation?', 'Many housing providers have written forms available for making a request. However, a request does not need to be made in writing, and does not need to follow the housing provider''s form to be valid. If you need help making your request, you can ask your housing provider for assistance. If your disability is obvious (visible) or already known to the property manager, the property manager should not ask you for verification of your disability. If your disability or disability-related need is not obvious, the property manager may request that you provide verification from a qualified third-party provider that you have a disability-related need for the requested accommodation or modification. The qualified provider does not need to be a medical provider to provide the verification.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 103)
            , ('Fair Housing Rights', 'What are my fair housing rights?', 'Federal, New York State and Westchester County laws all protect you against housing discrimination in any housing-related transaction, including applying for a rental apartment or to purchase a home. In New York State, you are protected against housing discrimination based on race, color, religion, national origin, sex or gender, sexual orientation, gender identity or expression, age, disability, marital status, familial status (having children under 18), lawful source of income, status as a victim of domestic abuse, citizen or immigration status or military status. These categories are known as "protected classes." In Westchester County, it is also unlawful to discriminate based on alienage or citizenship status; and status as a victim of domestic violence, sexual abuse, or stalking.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 201)
            , ('Fair Housing Rights', 'How do I file a claim of housing discrimination?', 'Before filing a complaint, please consider discussing your options with an attorney, or with a fair housing advocate, like the fair housing department at Westchester Residential Opportunities (by email at info@wroinc.org or by phone at 914-428-4507), or the fair housing investigators at Westchester County Human Rights Commission (by email at humanrights@westchestercountyny.gov or by phone at 914-995-9500). Federal and State Courts:  You can file a lawsuit in federal or state court. We strongly recommend that you consult with an attorney about that first. Administrative Agencies: You can also file a complaint with administrative agencies of the federal, state and in Westchester County, the county governments. Federal:  At the federal level, the United States Department of Housing and Urban Development (HUD) accepts and investigates fair housing complaints. You can file a complaint online at https://www.hud.gov/program_offices/fair_housing_equal_opp/online-complaint or call the Housing Discrimination Hotline at (800) 669-9777. HUD has a specific complaint form, which you can fine online by following the link above. New York State:  In New York, the New York State Division of Human Rights accepts and investigates fair housing complaints:  You can file online (or just learn more about the process) at https://dhr.ny.gov/complaint or by phone at 1-888-392-3644. In addition to the NYS Division of Human Rights, if your complaint regards lawful source of income discrimination, you can also file a complaint with the New York State Attorney General''s office, which can investigate source of income discrimination. The Attorney General has a specific complaint form for source of income discrimination, which you can complete online at https://ag.ny.gov/source-income-discrimination-form. Westchester County:  If your complaint involves housing discrimination in Westchester County, the Westchester County Human Rights Commission accepts and investigates fair housing complaints.  You can learn about how to file a complaint at https://humanrights.westchestergov.com/file-a-complaint, or by contacting the Human Rights Commission by email at humanrights@westchestercountyny.gov or by phone at 914-995-9500.', 'https://www.hud.gov/program_offices/fair_housing_equal_opp/online-complaint', 'https://dhr.ny.gov/complaint', 'https://ag.ny.gov/source-income-discrimination-form', 'https://humanrights.westchestergov.com/file-a-complaint', NULL, NULL, NULL, NULL, NULL, NULL, 202)
            , ('Fair Housing Rights', 'Can my housing provider retaliate against me for filing a bona fide claim of housing discrimination?', 'RETALIATION IS ILLEGAL. Remember, regardless of where you file a fair housing complaint.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 203)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @FAQs M
    WHERE M.Title NOT IN (SELECT Title FROM dbo.tblMasterFAQs);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @FAQs M
    JOIN dbo.tblMasterFAQs T ON T.Title = M.Title
        AND (ISNULL(RTRIM(T.Text), '') <> ISNULL(RTRIM(M.Text), '')
            OR ISNULL(RTRIM(T.[Url]), '') <> ISNULL(RTRIM(M.[Url]), '')
            OR ISNULL(RTRIM(T.[Url1]), '') <> ISNULL(RTRIM(M.[Url1]), '')
            OR ISNULL(RTRIM(T.[Url2]), '') <> ISNULL(RTRIM(M.[Url2]), '')
            OR ISNULL(RTRIM(T.[Url3]), '') <> ISNULL(RTRIM(M.[Url3]), '')
            OR ISNULL(RTRIM(T.[Url4]), '') <> ISNULL(RTRIM(M.[Url4]), '')
            OR ISNULL(RTRIM(T.[Url5]), '') <> ISNULL(RTRIM(M.[Url5]), '')
            OR ISNULL(RTRIM(T.[Url6]), '') <> ISNULL(RTRIM(M.[Url6]), '')
            OR ISNULL(RTRIM(T.[Url7]), '') <> ISNULL(RTRIM(M.[Url7]), '')
            OR ISNULL(RTRIM(T.[Url8]), '') <> ISNULL(RTRIM(M.[Url8]), '')
            OR ISNULL(RTRIM(T.[Url9]), '') <> ISNULL(RTRIM(M.[Url9]), '')
            OR ISNULL(RTRIM(T.[CategoryName]), '') <> ISNULL(RTRIM(M.[CategoryName]), '')
            OR T.[DisplayOrder] <> M.[DisplayOrder]
        );

    -- Insert new entries
    INSERT INTO dbo.tblMasterFAQs (CategoryName, Title, [Text]
                                    , [Url], Url1, Url2, Url3, Url4, Url5, Url6, Url7, Url8, Url9
                                    , DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.FAQID, 'ADD', 'Added FAQ'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.CategoryName, M.Title, M.[Text]
            , M.[Url], M.Url1, M.Url2, M.Url3, M.Url4, M.Url5, M.Url6, M.Url7, M.Url8, M.Url9
            , M.DisplayOrder, N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @FAQs M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.CategoryName  = M.CategoryName
        , T.[Text]      = M.Text
        , T.[Url]       = M.[Url]
        , T.Url1		= M.Url1
        , T.Url2		= M.Url2
        , T.Url3		= M.Url3
        , T.Url4		= M.Url4
        , T.Url5		= M.Url5
        , T.Url6		= M.Url6
        , T.Url7		= M.Url7
        , T.Url8		= M.Url8
        , T.Url9		= M.Url9
        , T.DisplayOrder = M.DisplayOrder
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.FAQID, 'UPDATE', 'Updated FAQ'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterFAQs T
    JOIN @FAQs M ON M.Title = T.Title
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'FAQ', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO