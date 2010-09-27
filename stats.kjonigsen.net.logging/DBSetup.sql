USE [IISLogs]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 09/27/2010 20:04:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Method] [nvarchar](20) NOT NULL,
	[IPAddress] [nvarchar](30) NOT NULL,
	[Url] [nvarchar](1000) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[UserAgent] [nvarchar](500) NOT NULL,
	[ResponseCode] [nvarchar](10) NOT NULL,
	[SiteName] [nvarchar](100) NOT NULL,
	[ApplicationName] [nvarchar](500) NOT NULL,
	[Referer] [nvarchar](500) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[IsLocalSite]    Script Date: 09/27/2010 20:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[IsLocalSite] 
(
	@Url nvarchar(500)
)
RETURNS bit
AS
BEGIN
	DECLARE @IsLocalSite bit

	IF @Url LIKE 'http://%.kjonigsen.net%'
	OR @Url LIKE 'http://%.feedmebacon.com%'
	OR @Url LIKE 'http://%.sayumin.org%'
	OR @Url LIKE 'http://%.hazelnutsinlove.com%'
	OR @Url LIKE 'http://%.aslaugstrige.com%'
		SET @IsLocalSite = 1
	ELSE
		SET @IsLocalSite = 0
	
	RETURN @IsLocalSite
END
GO
/****** Object:  UserDefinedFunction [dbo].[IsLocalProcess]    Script Date: 09/27/2010 20:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[IsLocalProcess] 
(
	@IPAddress nvarchar(30)
)
RETURNS bit
AS
BEGIN
	DECLARE @IsLocalProcess bit

	IF @IPAddress = '2002:5bbd:b2ae::5bbd:b2ae'
	OR @IPAddress = '127.0.0.1'
	OR @IPAddress = '91.189.178.174'
		SET @IsLocalProcess = 1
	ELSE
		SET @IsLocalProcess = 0
	
	RETURN @IsLocalProcess
END
GO
/****** Object:  UserDefinedFunction [dbo].[IsContentPage]    Script Date: 09/27/2010 20:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[IsContentPage] 
(
	@Url nvarchar(500)
)
RETURNS bit
AS
BEGIN
	DECLARE @IsContentPage bit

	IF (@Url LIKE '%/'
	OR @Url LIKE '%.aspx'
	OR @Url LIKE '%.aspx?%'
	OR @Url LIKE '%.ashx'
	OR @Url LIKE '%.ashx?%'
	OR @Url LIKE '%.html'
	OR @Url LIKE '%.htm'
	OR @Url LIKE '%.txt')
	AND @Url NOT LIKE '%/Json%'
	AND @Url NOT LIKE '%/robots.txt'
	AND @Url NOT LIKE '%/Rss%'
		SET @IsContentPage = 1
	ELSE
		SET @IsContentPage = 0
	
	RETURN @IsContentPage
END
GO
/****** Object:  View [dbo].[vContentPages]    Script Date: 09/27/2010 20:04:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vContentPages]
AS
	SELECT *
	FROM Log
	WHERE dbo.IsContentPage(Url) = 1
	AND dbo.IsLocalProcess(IPAddress) = 0
	AND UserAgent NOT LIKE '%bot%'
	AND UserAgent NOT LIKE '%yahoo%'
	AND UserAgent NOT LIKE 'baiduspider%'
	AND ResponseCode LIKE '2%'
	AND Url NOT LIKE '%default.aspx%'
GO
/****** Object:  StoredProcedure [dbo].[usp_PurgeLogs]    Script Date: 09/27/2010 20:03:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_PurgeLogs]
AS
BEGIN
	DECLARE @LastDate datetime
	DECLARE @LastID int

	SET @LastDate = DATEADD(month, -1, GETDATE())
	SET @LastID = (
		SELECT TOP(1) ID
		FROM Log
		WHERE Date < @LastDate
		ORDER BY ID DESC
	)
	
	IF @LastID IS NOT NULL
	BEGIN
		DELETE FROM Log
		WHERE ID <= @LastID
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetLastReferers]    Script Date: 09/27/2010 20:03:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetLastReferers]
(
	@Limit int = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #Referer (
		[ID] [int] NOT NULL,
		[Date] [datetime] NOT NULL,
		[Method] [nvarchar](20) NOT NULL,
		[IPAddress] [nvarchar](30) NOT NULL,
		[Url] [nvarchar](1000) NOT NULL,
		[UserName] [nvarchar](100) NULL,
		[UserAgent] [nvarchar](500) NOT NULL,
		[ResponseCode] [nvarchar](10) NOT NULL,
		[SiteName] [nvarchar](100) NOT NULL,
		[ApplicationName] [nvarchar](500) NOT NULL,
		[Referer] [nvarchar](500) NULL
	)
	DECLARE @ID		 int
	DECLARE @Url	 nvarchar(500)
	DECLARE @Referer nvarchar(500)
	DECLARE @Count	 int
	SET @Count = 0

	DECLARE LogCursor CURSOR FORWARD_ONLY READ_ONLY FOR
	SELECT ID, Url, Referer FROM Log WHERE Referer <> '' ORDER BY ID DESC

	OPEN LogCursor

	FETCH NEXT FROM LogCursor INTO @ID, @Url, @Referer

	WHILE @@FETCH_STATUS = 0 AND (@Count < @Limit OR @Limit IS NULL)
	BEGIN
		DECLARE @Hits int
		SELECT @Hits = COUNT(*)
		FROM #Referer
		WHERE Referer = @Referer

		DECLARE @IsLocal bit
		SELECT @IsLocal = dbo.IsLocalSite(@Referer)
		
		IF (@Hits = 0 OR @Hits IS NULL) AND @IsLocal = 0
		BEGIN
			INSERT INTO #Referer
			SELECT ID, Date, Method, IPAddress, Url, UserName, UserAgent, ResponseCode, SiteName, ApplicationName, Referer FROM Log WHERE ID = @ID
			SET @Count = @Count + 1
		END
		
		FETCH NEXT FROM LogCursor INTO @ID, @Url, @Referer		
	END

	CLOSE LogCursor
	DEALLOCATE LogCursor

	SELECT *
	FROM #Referer

	DROP TABLE #Referer

	SET NOCOUNT OFF
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetLastLogEntries]    Script Date: 09/27/2010 20:03:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[usp_GetLastLogEntries]
(
	@NumberOfEntries int = NULL
)
AS
BEGIN
	IF @NumberOfEntries IS NULL
	BEGIN
		SELECT *
		FROM vContentPages
		ORDER BY ID DESC
	END
	ELSE
	BEGIN
		SELECT TOP(@NumberOfEntries) *
		FROM vContentPages
		ORDER BY ID DESC
	END
END
GO
/****** Object:  Default [DF_Log_Date]    Script Date: 09/27/2010 20:04:01 ******/
ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_Date]  DEFAULT (getdate()) FOR [Date]
GO
