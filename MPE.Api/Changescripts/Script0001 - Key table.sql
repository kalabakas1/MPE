CREATE TABLE [dbo].[Api_KeyMethod](
	[KeyMethodID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[KeyID] [int] NOT NULL,
	[Method] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_Api_KeyMethod] PRIMARY KEY CLUSTERED 
(
	[KeyMethodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Api_KeyMethod] ADD  CONSTRAINT [DF_Api_KeyMethod_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[Api_KeyMethod] ADD  CONSTRAINT [DF_Api_KeyMethod_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Api_KeyMethod]  WITH CHECK ADD  CONSTRAINT [FK_Api_KeyMethod_Api_Key] FOREIGN KEY([KeyID])
REFERENCES [dbo].[Api_Key] ([KeyID])
GO

ALTER TABLE [dbo].[Api_KeyMethod] CHECK CONSTRAINT [FK_Api_KeyMethod_Api_Key]
GO