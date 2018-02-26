CREATE TABLE [dbo].[Api_Key](
	[KeyID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Key] [nvarchar](64) NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[Admin] [bit] NOT NULL,
 CONSTRAINT [PK_Api_Key] PRIMARY KEY CLUSTERED 
(
	[KeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Api_Key] ADD  CONSTRAINT [DF_Api_Key_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[Api_Key] ADD  CONSTRAINT [DF_Api_Key_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Api_Key] ADD  CONSTRAINT [DF_Api_Key_Key]  DEFAULT (newid()) FOR [Key]
GO

ALTER TABLE [dbo].[Api_Key] ADD  CONSTRAINT [DF_Api_Key_Admin]  DEFAULT ((0)) FOR [Admin]
GO