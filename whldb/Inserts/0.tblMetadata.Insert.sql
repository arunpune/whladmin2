SET NOCOUNT ON;
GO

DECLARE @Metadata TABLE (
    [MetadataID] [int] NOT NULL,
    [CodeID] [int] NOT NULL,
    [Code] [varchar](20) NOT NULL,
    [Description] [varchar](1000) NOT NULL,
    [Active] [bit] NOT NULL DEFAULT(1),
    [AssociatedCodeID] [int] NULL,
    [AssociatedCode] [varchar](20) NULL,
    [ChangeType] [char](1) NOT NULL DEFAULT ('-')
);

-- Entity Types (101)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (101001, 101, 'AMENITY', 'Amenity')
        , (101002, 101, 'FAQ', 'FAQ')
        , (101003, 101, 'HOUSEHOLD', 'Household')
        , (101004, 101, 'LISTING', 'Listing')
        , (101005, 101, 'NOTIFICATION', 'Notification')
        , (101006, 101, 'QUESTION', 'Question')
        , (101007, 101, 'QUOTE', 'Quote')
        , (101008, 101, 'RESOURCE', 'Resource / Link')
        , (101009, 101, 'UNIT', 'Unit')
        , (101010, 101, 'USER', 'User')
        , (101011, 101, 'VIDEO', 'Video')
        , (101012, 101, 'LISTINGIMAGE', 'Listing Image')
        , (101013, 101, 'LISTINGDOCUMENT', 'Listing Document')
        , (101014, 101, 'LISTINGDECL', 'Listing Declaration')
        , (101015, 101, 'LISTINGDSCL', 'Listing Disclosure')
        , (101016, 101, 'MKTAGENT', 'Marketing Agent')
        , (101017, 101, 'AMI', 'AMI')
        , (101018, 101, 'AMORTIZATION', 'Amortization')
        , (101019, 101, 'ADMINUSER', 'Admin User')
    ;

-- User Roles (102)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (102001, 102, 'SYSADMIN', 'System Administrator')
        , (102002, 102, 'OPSADMIN', 'Operations Administrator')
        , (102003, 102, 'OPSVIEWER', 'Operations Viewer')
        , (102004, 102, 'LOTADMIN', 'Lottery Administrator')
        , (102005, 102, 'LOTVIEWER', 'Lottery Viewer')
    ;

-- Notification Categories (103)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (103001, 103, 'APPLICANT', 'Applicant')
        , (103002, 103, 'INTERNAL', 'Internal')
    ;

-- Notification Frequencies (104)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (104001, 104, 'BEFORE', 'Days prior to')
        , (104002, 104, 'ON', 'On the day of')
    ;

-- Answer Types (105)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (105001, 105, 'STRING', 'String / Text')
        , (105002, 105, 'NUMBER', 'Number')
        , (105003, 105, 'SINGLESEL', 'Single Selection')
        , (105004, 105, 'MULTISEL', 'Multiple Selection')
    ;

-- Listing Statuses (106)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (106001, 106, 'DRAFT', 'Draft')
        , (106002, 106, 'REVIEW', 'In Review')
        , (106003, 106, 'REVISE', 'Needs Revision')
        , (106004, 106, 'PUBLISHED', 'Published')
        , (106005, 106, 'DELETED', 'Deleted')
    ;

-- Listing Types (107)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (107001, 107, 'SALE', 'Sale')
        , (107002, 107, 'RENTAL', 'Rental')
        , (107999, 107, 'BOTH', 'Sale and Rental')
    ;

-- Unit Types (108)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (108001, 108, 'STUDIO', 'Studio')
        , (108002, 108, '1BED', '1 Bedroom')
        , (108003, 108, '2BED', '2 Bedroom')
        , (108004, 108, '3BED', '3 Bedroom')
        , (108005, 108, '4BED', '4+ Bedrooms')
    ;

-- Household Member Types (109)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (109001, 109, 'SPOUSE', 'Spouse/Husband/Wife')
        , (109002, 109, 'CHILD', 'Child/Legal Dependent')
        , (109003, 109, 'PARENT', 'Parent/Mother/Father/Legal Guardian')
        , (109004, 109, 'GRANDPARENT', 'Grandparent')
        , (109005, 109, 'GRANDCHILD', 'Grandchild')
        , (109006, 109, 'AUNTUNCLE', 'Aunt/Uncle')
        , (109007, 109, 'COUSIN', 'Cousin')
        , (109008, 109, 'NIECENEPHEW', 'Niece/Nephew')
        , (109009, 109, 'PARTNER', 'Partner/Domestic Partner')
        , (109010, 109, 'SIBLING', 'Sibling/Brother/Sister')
        , (109999, 109, 'OTHER', 'Other')
    ;

-- Gender Types (110)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (110001, 110, 'MALE', 'Male')
        , (110002, 110, 'FEMALE', 'Female')
        , (110003, 110, 'NONBINARY', 'Non-Binary')
        , (110999, 110, 'NOANS', 'Choose not to respond')
    ;

-- Race Types (111)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (111001, 111, 'NATIVE', 'American Indian or Alaska Native')
        , (111002, 111, 'ASIAN', 'Asian')
        , (111003, 111, 'BLACK', 'Black or African American')
        , (111004, 111, 'PACIFIC', 'Native Hawaiian or Other Pacific Islander')
        , (111005, 111, 'WHITE', 'White')
        , (111011, 111, 'NATIVEBLACK', 'American Indian or Alaska Native & Black or African American')
        , (111012, 111, 'NATIVEWHITE', 'American Indian or Alaska Native & White')
        , (111013, 111, 'ASIANWHITE', 'Asian & White')
        , (111014, 111, 'BLACKWHITE', 'Black or African American & White')
        , (111099, 111, 'OTHER', 'Other Multi Racial')
        , (111999, 111, 'NOANS', 'Choose not to respond')
    ;

-- Ethnicity Types (112)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (112001, 112, 'HISPANIC', 'Hispanic or Latino')
        , (112002, 112, 'NOTHISPANIC', 'Not Hispanic or Latino')
        , (112999, 112, 'NOANS', 'Choose not to respond')
    ;

-- Language Types (113)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (113001, 113, 'EN', 'English')
        , (113003, 113, 'CH', 'Mandarin')
        , (113002, 113, 'ES', 'Spanish')
        , (113999, 113, 'OTHER', 'Other')
    ;

-- Phone Number Types (114)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (114001, 114, 'CELL', 'Cell Phone')
        , (114002, 114, 'LAND', 'Landline')
        , (114999, 114, 'OTHER', 'Other')
    ;

-- Asset Types (115)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (115001, 115, 'REALESTATE', 'Real Estate')
        , (115999, 115, 'OTHER', 'Other')
    ;

-- Income Types (116)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (116001, 116, 'PAYCHECK', 'Paycheck')
        , (116999, 116, 'OTHER', 'Other')
    ;

-- Question Categories (117)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (117001, 117, 'PROFILE', 'Profile Questions')
        , (117002, 117, 'PREFERENCE', 'Preferences Questions')
        , (117003, 117, 'HHSUMMARY', 'Household Summary Questions')
        , (117004, 117, 'HHMEMBER', 'Household Member Questions')
        , (117005, 117, 'APPLICATION', 'Application Questions')
        , (117006, 117, 'STATISTICAL', 'Statistical Information Questions')
        , (117007, 117, 'ACCESSIBILITY', 'Accessibility Questions')
    ;

-- Organization Types (118)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (118001, 118, 'DOP', 'Department of Planning')
        , (118002, 118, 'DOIT', 'Department of Information Technology')
        , (118003, 118, 'WRO', 'Westchester Residential Opportunities')
    ;

-- Organization Types (119)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (119001, 119, 'WHEELCHAIR', 'Adapted for Mobility Impairments')
        , (119002, 119, 'HEARING', 'Adapted for Hearing Impairments')
        , (119003, 119, 'VISION', 'Adapted for Vision Impairments')
    ;

-- Lead Types (120)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (120001, 120, 'FRIEND', 'Friend/Word of Mouth')
        , (120002, 120, 'EMPLOYER', 'Employer')
        , (120003, 120, 'SITE', 'Sign Posted at Site')
        , (120004, 120, 'WEBSITE', 'Website (please specify)')
        , (120005, 120, 'NEWSPAPERAD', 'Newspaper Advertisement')
        , (120006, 120, 'NEWSPAPERART', 'Newspaper Article (please specify)')
        , (120007, 120, 'CHURCH', 'Church/Synagogue')
        , (120008, 120, 'COMMUNITYORG', 'Community Organization')
        , (120009, 120, 'EXPO', 'Affordable Housing Expo')
        , (120999, 120, 'OTHER', 'Other (please specify)')
    ;

-- Application Declarations (121)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (121001, 121, 'DECL0001', 'I declare that the statements contained in this application are true and complete to the best of my knowledge.  I understand that this is an initial application, and that I will be required to provide additional information and documentation on my income and assets if/when the property manager considers my application.  WARNING:  Willful false statements or misrepresentations are a criminal offense.')
        , (121002, 121, 'DECL0002', 'I (We) authorize my (our) consent to have management verify the information in this application for the purpose of providing my (our) eligibility for occupancy. I will provide all necessary information including source names, addresses, phone numbers, and account numbers where applicable and any other information required for expediting this process. I (We) understand that my (our) occupancy is contingent on meeting management''s resident selection criteria and requirements.')
    ;

-- Application Disclosures (122)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (122001, 122, 'DISC0001', 'Only one (1) application per household.  If your name appears on more than one application, you may be disqualified, and the application may not be considered.')
        , (122002, 122, 'DISC0002', 'Applications must be signed in all requested places.')
        , (122003, 122, 'DISC0003', 'Applications must be returned either electronically, by mail or hand delivered.')
        , (122004, 122, 'DISC0004', 'No payment should be given to anyone in connection with the preparation or filing of this application. If/when your application is considered by the property manager, you may be required to reimburse the cost of the credit and/or background checks for your households'' members, to the extent permitted by New York law.')
        , (122005, 122, 'DISC0005', 'This is an initial application. It does not include all the information and documentation that will be required to qualify if/when the property manager considers your application. To income qualify, you will need to complete additional information requests regarding your household''s income and assets and provide documentation to support that income and assets.')
        , (122006, 122, 'DISCPETYES', 'This building allows pets, subject to the restrictions in its pets policy. Service and assistance animals (like emotional support animals) are allowed and are not considered pets.')
        , (122007, 122, 'DISCPETNO', 'This building does not allow pets. Service and assistance animals (like emotional support animals) are allowed and are not considered pets.')
    ;

-- Vouchers / Rental Assistance (123)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (123001, 123, 'SECTION8', 'Section 8 (Housing Choice Voucher)')
        , (123002, 123, 'HOPWA', 'HOPWA')
        , (123003, 123, 'VASH', 'VASH')
        , (123004, 123, 'CHI', 'CHI')
        , (123005, 123, 'CITYFHEPS', 'CityFHEPs')
        , (123999, 123, 'OTHER', 'Other')
    ;

-- Bank Account Types (124)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (124001, 124, 'CHECKING', 'Checking')
        , (124002, 124, 'SAVINGS', 'Savings/Money Market Account')
        , (124003, 124, 'BROKERAGE', 'Brokerage/Investment Account')
        , (124004, 124, 'CERTIFICATE', 'Certificate of Deposit')
        , (124005, 124, 'MUTUALFUND', 'Mutual Fund Account')
        , (124006, 124, 'RETIREMENT', 'Pension/Retirement Account')
        , (124007, 124, 'CASHAPP', 'Cash App (Venmo, PayPal, ApplePay, etc.)')
        , (124008, 124, 'CRYPTO', 'Cryptocurrency (Bitcoin, Ethereum, etc.)')
        , (124999, 124, 'OTHER', 'Other')
    ;

-- Identification Types (125)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (125001, 125, 'STATEDL', 'Driver''s License')
        , (125002, 125, 'STATEID', 'State ID')
        , (125003, 125, 'PASSPORT', 'Passport')
        , (125999, 125, 'OTHER', 'Other')
    ;

-- View Types (126)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (126001, 126, 'ALL', 'Current Opportunities')
        , (126002, 126, 'CMNGOPPR', 'Coming Soon')
        --, (126003, 126, 'ACPTAPPL', 'Accepting Applications')
        , (126004, 126, 'WAITLIST', 'Waitlist Opportunities')
        , (126005, 126, 'PASTOPPR', 'Past Listings')
    ;

-- Lottery Application Statuses Types (127)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description], Active)
    VALUES (127001, 127, 'All', 'All Statuses', 1)
        , (127002, 127, 'DRAFT', 'Draft', 0)
        , (127003, 127, 'SUBMITTED', 'Submitted', 1)
        , (127004, 127, 'WAITLISTED', 'Waitlist', 1)
        , (127005, 127, 'ASSIGNED', 'Lottery # Assigned', 1)
        , (127006, 127, 'QUALIFIED', 'Qualified', 0)
        , (127007, 127, 'DISQUALIFIED', 'Disqualified', 0)
        , (127008, 127, 'CANCELLED', 'Cancelled', 0)
        , (127009, 127, 'EXPIRED', 'Deadline Expired', 1)
        , (127010, 127, 'WITHDRAWN', 'Withdrawn', 1)
        , (127011, 127, 'DUPLICATE', 'Duplicate - Eliminated', 1)
        , (127999, 127, 'OTHER', 'Other', 0)
    ;

-- Lottery Application Submission Types (128)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (128001, 128, 'ALL', 'All Submissions')
        , (128002, 128, 'ONLINE', 'Online Submissions')
        , (128003, 128, 'PAPER', 'Paper Submissions')
    ;

-- Sales/Rentals Age Preference Types (129)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (129001, 129, 'ALL', 'Not limited to Seniors')
        , (129002, 129, '55+', 'Units for 55+')
        , (129003, 129, '62+', 'Units for 62+')
    ;

-- Bathroom Part Options (130)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (130001, 130, '0', '0')
        , (130002, 130, '25', '25')
        , (130003, 130, '50', '50')
        , (130004, 130, '75', '75')
    ;

-- Allowed File Types (131)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (131001, 131, 'PDF', '*.PDF')
        , (131002, 131, 'PNG', '*.PNG')
        , (131003, 131, 'JPEG', '*.JPEG')
        , (131004, 131, 'JPG', '*.JPG')
    ;

-- Counties (132)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (132001, 132, 'MANHATTAN', 'Manhattan or New York County, NY')
        , (132002, 132, 'QUEENS', 'Queens County, NY')
        , (132003, 132, 'RICHMOND', 'Staten Island or Richmond County, NY')
        , (132004, 132, 'BRONX', 'Bronx County, NY')
        , (132005, 132, 'BROOKLYN', 'Brooklyn or Kings County, NY')
        , (132006, 132, 'WESTCHESTER', 'Westchester County, NY')
        , (132007, 132, 'PUTNAM', 'Putnam County, NY')
        , (132008, 132, 'ROCKLAND', 'Rockland County, NY')
        , (132009, 132, 'FAIRFIELD', 'Fairfield County, CT')
        , (132999, 132, 'OTHER', 'Other')
    ;

-- Adapted for Disability Search Options (133)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (133001, 133, 'ALL', 'All')
        , (133002, 133, 'YES', 'Yes')
        , (133003, 133, 'No', 'No')
    ;

-- Pets Allowed Search Options (134)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (134001, 134, 'ALL', 'All')
        , (134002, 134, 'YES', 'Yes, Pets Permitted')
        , (134003, 134, 'NO', 'No, Pets Not Permitted')
    ;

-- Senior Living Search Options (135)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (135001, 135, 'ALL', 'No')
        , (135002, 135, '55+', '55+')
        , (135003, 135, '62+', '62+')
    ;

-- Listing Type Search Options (135)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (136001, 136, 'BOTH', 'Rentals & Sales')
        , (136002, 136, 'RENTAL', 'Rentals')
        , (136003, 136, 'SALE', 'Sales')
    ;

-- State Codes/Names (137)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description])
    VALUES (137001, 137, N'AL', N'Alabama')
        , (137002, 137, N'AK', N'Alaska')
        , (137003, 137, N'AZ', N'Arizona')
        , (137004, 137, N'AR', N'Arkansas')
        , (137005, 137, N'CA', N'California')
        , (137006, 137, N'CO', N'Colorado')
        , (137007, 137, N'CT', N'Connecticut')
        , (137008, 137, N'DE', N'Delaware')
        , (137009, 137, N'DC', N'District of Columbia')
        , (137010, 137, N'FL', N'Florida')
        , (137011, 137, N'GA', N'Georgia')
        , (137012, 137, N'HI', N'Hawaii')
        , (137013, 137, N'ID', N'Idaho')
        , (137014, 137, N'IL', N'Illinois')
        , (137015, 137, N'IN', N'Indiana')
        , (137016, 137, N'IA', N'Iowa')
        , (137017, 137, N'KS', N'Kansas')
        , (137018, 137, N'KY', N'Kentucky')
        , (137019, 137, N'LA', N'Louisiana')
        , (137020, 137, N'ME', N'Maine')
        , (137021, 137, N'MD', N'Maryland')
        , (137022, 137, N'MA', N'Massachusetts')
        , (137023, 137, N'MI', N'Michigan')
        , (137024, 137, N'MN', N'Minnesota')
        , (137025, 137, N'MS', N'Mississippi')
        , (137026, 137, N'MO', N'Missouri')
        , (137027, 137, N'MT', N'Montana')
        , (137028, 137, N'NE', N'Nebraska')
        , (137029, 137, N'NV', N'Nevada')
        , (137030, 137, N'NH', N'New Hampshire')
        , (137031, 137, N'NJ', N'New Jersey')
        , (137032, 137, N'NM', N'New Mexico')
        , (137033, 137, N'NY', N'New York')
        , (137034, 137, N'NC', N'North Carolina')
        , (137035, 137, N'ND', N'North Dakota')
        , (137036, 137, N'OH', N'Ohio')
        , (137037, 137, N'OK', N'Oklahoma')
        , (137038, 137, N'OR', N'Oregon')
        , (137039, 137, N'PA', N'Pennsylvania')
        , (137040, 137, N'RI', N'Rhode Island')
        , (137041, 137, N'SC', N'South Carolina')
        , (137042, 137, N'SD', N'South Dakota')
        , (137043, 137, N'TN', N'Tennessee')
        , (137044, 137, N'TX', N'Texas')
        , (137045, 137, N'UT', N'Utah')
        , (137046, 137, N'VT', N'Vermont')
        , (137047, 137, N'VA', N'Virginia')
        , (137048, 137, N'WA', N'Washington')
        , (137049, 137, N'WV', N'West Virginia')
        , (137050, 137, N'WI', N'Wisconsin')
        , (137051, 137, N'WY', N'Wyoming')
        , (137052, 137, N'AS', N'American Samoa')
        , (137053, 137, N'GU', N'Guam')
        , (137054, 137, N'MP', N'Northern Mariana Islands')
        , (137055, 137, N'PR', N'Puerto Rico')
        , (137056, 137, N'VI', N'U.S. Virgin Islands')
        , (137057, 137, N'UM', N'U.S. Minor Outlying Islands')
    ;

-- NY/CT County Codes/Names (137)
INSERT INTO @Metadata (MetadataID, CodeID, Code, [Description], AssociatedCodeID, AssociatedCode)
    VALUES (138001, 138, N'ALBANY', N'Albany County, NY', 137, N'NY')
        , (138002, 138, N'ALLEGANY', N'Allegany County, NY', 137, N'NY')
        , (138003, 138, N'BRONX', N'Bronx County, NY', 137, N'NY')
        , (138004, 138, N'BROOME', N'Broome County, NY', 137, N'NY')
        , (138005, 138, N'CATTARAUGUS', N'Cattaraugus County, NY', 137, N'NY')
        , (138006, 138, N'CAYUGA', N'Cayuga County, NY', 137, N'NY')
        , (138007, 138, N'CHAUTAUQUA', N'Chautauqua County, NY', 137, N'NY')
        , (138008, 138, N'CHEMUNG', N'Chemung County, NY', 137, N'NY')
        , (138009, 138, N'CHENANGO', N'Chenango County, NY', 137, N'NY')
        , (138010, 138, N'CLINTON', N'Clinton County, NY', 137, N'NY')
        , (138011, 138, N'COLUMBIA', N'Columbia County, NY', 137, N'NY')
        , (138012, 138, N'CORTLAND', N'Cortland County, NY', 137, N'NY')
        , (138013, 138, N'DELAWARE', N'Delaware County, NY', 137, N'NY')
        , (138014, 138, N'DUTCHESS', N'Dutchess County, NY', 137, N'NY')
        , (138015, 138, N'ERIE', N'Erie County, NY', 137, N'NY')
        , (138016, 138, N'ESSEX', N'Essex County, NY', 137, N'NY')
        , (138017, 138, N'FRANKLIN', N'Franklin County, NY', 137, N'NY')
        , (138018, 138, N'FULTON', N'Fulton County, NY', 137, N'NY')
        , (138019, 138, N'GENESEE', N'Genesee County, NY', 137, N'NY')
        , (138020, 138, N'GREENE', N'Greene County, NY', 137, N'NY')
        , (138021, 138, N'HAMILTON', N'Hamilton County, NY', 137, N'NY')
        , (138022, 138, N'HERKIMER', N'Herkimer County, NY', 137, N'NY')
        , (138023, 138, N'JEFFERSON', N'Jefferson County, NY', 137, N'NY')
        , (138024, 138, N'KINGS', N'Kings County, NY', 137, N'NY')
        , (138025, 138, N'LEWIS', N'Lewis County, NY', 137, N'NY')
        , (138026, 138, N'LIVINGSTON', N'Livingston County, NY', 137, N'NY')
        , (138027, 138, N'MADISON', N'Madison County, NY', 137, N'NY')
        , (138028, 138, N'MONROE', N'Monroe County, NY', 137, N'NY')
        , (138029, 138, N'MONTGOMERY', N'Montgomery County, NY', 137, N'NY')
        , (138030, 138, N'NASSAU', N'Nassau County, NY', 137, N'NY')
        , (138031, 138, N'NEW YORK', N'New York County, NY', 137, N'NY')
        , (138032, 138, N'NIAGARA', N'Niagara County, NY', 137, N'NY')
        , (138033, 138, N'ONEIDA', N'Oneida County, NY', 137, N'NY')
        , (138034, 138, N'ONONDAGA', N'Onondaga County, NY', 137, N'NY')
        , (138035, 138, N'ONTARIO', N'Ontario County, NY', 137, N'NY')
        , (138036, 138, N'ORANGE', N'Orange County, NY', 137, N'NY')
        , (138037, 138, N'ORLEANS', N'Orleans County, NY', 137, N'NY')
        , (138038, 138, N'OSWEGO', N'Oswego County, NY', 137, N'NY')
        , (138039, 138, N'OTSEGO', N'Otsego County, NY', 137, N'NY')
        , (138040, 138, N'PUTNAM', N'Putnam County, NY', 137, N'NY')
        , (138041, 138, N'QUEENS', N'Queens County, NY', 137, N'NY')
        , (138042, 138, N'RENSSELAER', N'Rensselaer County, NY', 137, N'NY')
        , (138043, 138, N'RICHMOND', N'Richmond County, NY', 137, N'NY')
        , (138044, 138, N'ROCKLAND', N'Rockland County, NY', 137, N'NY')
        , (138045, 138, N'SARATOGA', N'Saratoga County, NY', 137, N'NY')
        , (138046, 138, N'SCHENECTADY', N'Schenectady County, NY', 137, N'NY')
        , (138047, 138, N'SCHOHARIE', N'Schoharie County, NY', 137, N'NY')
        , (138048, 138, N'SCHUYLER', N'Schuyler County, NY', 137, N'NY')
        , (138049, 138, N'SENECA', N'Seneca County, NY', 137, N'NY')
        , (138050, 138, N'ST. LAWRENCE', N'St. Lawrence County, NY', 137, N'NY')
        , (138051, 138, N'STEUBEN', N'Steuben County, NY', 137, N'NY')
        , (138052, 138, N'SUFFOLK', N'Suffolk County, NY', 137, N'NY')
        , (138053, 138, N'SULLIVAN', N'Sullivan County, NY', 137, N'NY')
        , (138054, 138, N'TIOGA', N'Tioga County, NY', 137, N'NY')
        , (138055, 138, N'TOMPKINS', N'Tompkins County, NY', 137, N'NY')
        , (138056, 138, N'ULSTER', N'Ulster County, NY', 137, N'NY')
        , (138057, 138, N'WARREN', N'Warren County, NY', 137, N'NY')
        , (138058, 138, N'WASHINGTON', N'Washington County, NY', 137, N'NY')
        , (138059, 138, N'WAYNE', N'Wayne County, NY', 137, N'NY')
        , (138060, 138, N'WESTCHESTER', N'Westchester County, NY', 137, N'NY')
        , (138061, 138, N'WYOMING', N'Wyoming County, NY', 137, N'NY')
        , (138062, 138, N'YATES', N'Yates County, NY', 137, N'NY')
        , (138063, 138, N'FAIRFIELD', N'Fairfield County, CT', 137, N'CT')
        , (138999, 138, N'OTHER', N'Other', NULL, NULL)
    ;

-- Mark new entries
UPDATE M
SET M.ChangeType = 'A'
FROM @Metadata M
WHERE M.MetadataID NOT IN (SELECT MetadataID FROM dbo.tblMetadata);

-- Mark updates to existing entries
UPDATE M
SET M.ChangeType = 'U'
FROM @Metadata M
JOIN dbo.tblMetadata T ON T.MetadataID = M.MetadataID;

-- Insert new entries
INSERT INTO dbo.tblMetadata (MetadataID, CodeID, Code, [Description], AssociatedCodeID, AssociatedCode, Active)
    SELECT M.MetadataID, M.CodeID, M.Code, M.[Description], M.AssociatedCodeID, M.AssociatedCode, M.Active
    FROM @Metadata M
    WHERE M.ChangeType = 'A';

-- Update existing entries
UPDATE T
SET T.CodeID = M.CodeID, T.Code = M.Code, T.[Description] = M.[Description]
    , T.AssociatedCodeID = M.AssociatedCodeID, T.AssociatedCode = M.AssociatedCode
    , T.Active = M.Active
FROM dbo.tblMetadata T
JOIN @Metadata M ON M.MetadataID = T.MetadataID
WHERE M.ChangeType = 'U';

-- Delete old entries
DELETE M
FROM dbo.tblMetadata M
WHERE M.MetadataID NOT IN (SELECT MetadataID FROM @Metadata);

GO