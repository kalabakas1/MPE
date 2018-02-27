CREATE TABLE [dbo].[Api_Log](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Ip] [nvarchar](64) NOT NULL,
	[KeyID] [int] NULL,
	[Key] [nvarchar](64) NULL,
	[RequestTimestamp] [datetime] NOT NULL,
	[RequestMethod] [nvarchar](32) NOT NULL,
	[RequestUrl] [nvarchar](2048) NOT NULL,
	[RequestContent] [nvarchar](max) NULL,
	[ResponseTimestamp] [datetime] NOT NULL,
	[ResponseStatusCode] [int] NOT NULL,
	[ResponseContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_Api_Log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Api_Log] ADD  CONSTRAINT [DF_Api_Log_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[Api_Log] ADD  CONSTRAINT [DF_Api_Log_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Api_Log]  WITH CHECK ADD  CONSTRAINT [FK_Api_Log_Api_Key] FOREIGN KEY([KeyID])
REFERENCES [dbo].[Api_Key] ([KeyID])
GO

ALTER TABLE [dbo].[Api_Log] CHECK CONSTRAINT [FK_Api_Log_Api_Key]
GO