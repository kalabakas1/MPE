CREATE TABLE [dbo].[Api_KeyField](
	[KeyFieldID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[KeyID] [int] NOT NULL,
	[TypeAlias] [nvarchar](1024) NOT NULL,
	[FieldName] [nvarchar](128) NULL,
 CONSTRAINT [PK_Api_KeyField] PRIMARY KEY CLUSTERED 
(
	[KeyFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Api_KeyField] ADD  CONSTRAINT [DF_Api_KeyField_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[Api_KeyField] ADD  CONSTRAINT [DF_Api_KeyField_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Api_KeyField]  WITH CHECK ADD  CONSTRAINT [FK_Api_KeyField_Api_Key] FOREIGN KEY([KeyID])
REFERENCES [dbo].[Api_Key] ([KeyID])
GO

ALTER TABLE [dbo].[Api_KeyField] CHECK CONSTRAINT [FK_Api_KeyField_Api_Key]
GO